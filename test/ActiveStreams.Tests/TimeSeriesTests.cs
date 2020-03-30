// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ActiveStreams.Sampling;
using ReactiveSampling.Tests;
using Xunit;
using Xunit.Abstractions;

namespace ActiveStreams.Tests
{
	/// <summary>
	///     Examples of how to achieve time-series manipulations, specifically:
	///     - Up-sampling (fill gaps, "de-quantize")
	///     - Down-sampling (compress spans, "quantize")
	///     - Forecast (predict future values, assuming a good function)
	///     - Overlapping (resolve conflicts with overlapping, balanced streams)
	/// </summary>
	public class TimeSeriesTests
	{
		public TimeSeriesTests(ITestOutputHelper console) => _console = console;

		private readonly ITestOutputHelper _console;

		public enum SampleWith
		{
			/// <summary>
			///     Linear interpolation (smoothing over a line)
			/// </summary>
			Lerp,

			/// <summary>
			///     Simple average over period elements.
			/// </summary>
			Average
		}

		public static IEnumerable<Data> Resample(IEnumerable<DataRow> list, DateTimeOffset from, DateTimeOffset to,
			TimeSpan period, SampleWith sampleWith)
		{
			var result = list.Resample(from, to, period, ParseTimestamp,
				(ts, l, n, d) => new Data {Month = ts, Sales = Resample(sampleWith, ts, l, n, d)});
			return result;
		}

		private static double Resample(SampleWith sampleWith, DateTimeOffset ts, DataRow l, DataRow n, double d)
		{
			double sample;
			switch (sampleWith)
			{
				case SampleWith.Lerp:
					sample = ResampleFunctions.Lerp(ts, l.Sales, n.Sales, d);
					break;
				case SampleWith.Average:
					var samples = 1; // FIXME: should be number of samples in the period between l and n
					sample = ResampleFunctions.Average(samples)(ts, l.Sales, n.Sales, d);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(sampleWith), sampleWith, null);
			}

			return sample;
		}

		public static IEnumerable<Data> ResampleAverage(IEnumerable<DataRow> list, DateTimeOffset from,
			DateTimeOffset to, TimeSpan period)
		{
			var result = list.Resample(from, to, period, ParseTimestamp, (ts, l, n, d) =>
			{
				//var samples = DateSpan.GetOccurrences(from, to, period).Count;
				var avg = ResampleFunctions.Average(1);
				return new Data {Month = ts, Sales = avg(ts, l.Sales, n.Sales, d)};
			});
			return result;
		}

		public static IEnumerable<Data> Forecast(IEnumerable<DataRow> list, DateTimeOffset from, DateTimeOffset to,
			Func<DateTimeOffset, TimeSpan> period)
		{
			//
			// We can't use a function like linear interpolation for forecasting because after the first value, we'll always get the same value.
			// In this case we'll use Simple Moving Average, which will produce changing values, though this will slowly degrade to zero since
			// values are always decreasing over time. You'll have to choose a function that actually works...

			var result = list.Resample(from, to, period, ParseTimestamp,
				(ts, l, n, d) => new Data
				{
					Month = ts, Sales = ResampleFunctions.SimpleMovingAverage(ts, l.Sales, n.Sales, d)
				});
			return result;
		}

		private static DateTimeOffset ParseTimestamp(DataRow d)
		{
			// The timestamp can't be parsed normally since it's missing days
			var tokens = d.Month.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries);
			return new DateTimeOffset(int.Parse(tokens[0].Trim('"')), int.Parse(tokens[1].Trim('"')), 1, 0, 0, 0,
				TimeSpan.Zero);
		}

		public class DataRow
		{
			public string Month;
			public float Sales;
		}

		public class Data
		{
			public DateTimeOffset Month { get; set; }
			public double Sales { get; set; }
		}

		[Fact]
		public void Can_compress_range()
		{
			//
			// Now we want to down-sample, zooming out to a lower resolution and deciding how to compress the values into a smaller space.

			var sw = Stopwatch.StartNew();
			var enumerable = DataHelpers.StreamFrame<DataRow>("InputFiles/shampoo.txt");
			var list = enumerable.ToList();
			_console.WriteLine("Original Data (by month):");
			_console.WriteLine("*************************");
			_console.WriteLine(list.ToTable());
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");

			//
			// Re-sample entire data set to years
			sw.Restart();
			var yearly = ResampleAverage(list, "2001-01".Timestamp(), "2003-12".Timestamp(), TimeSpan.FromDays(365))
				.AsParallel().ToList();
			_console.WriteLine("Normalized Data (by year, incorrect):");
			_console.WriteLine("************************************");
			_console.WriteLine(yearly.ToTable("Years", "Sales"));
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");
		}

		[Fact]
		public void Can_fill_gaps()
		{
			//
			// When we have unbalanced data (different time resolutions), we need to be able to re-sample.
			var sw = Stopwatch.StartNew();
			var enumerable = DataHelpers.StreamFrame<DataRow>("InputFiles/shampoo.txt");
			var list = enumerable.ToList();

			_console.WriteLine("Original Data (by month):");
			_console.WriteLine("*************************");
			_console.WriteLine(list.ToTable());
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");

			sw.Restart();
			var weeks = Resample(list, "2001-01".Timestamp(), "2003-12".Timestamp(), TimeSpan.FromDays(7),
				SampleWith.Lerp).AsParallel().ToList();
			_console.WriteLine("Normalized Data (by week)");
			_console.WriteLine("*************************");
			_console.WriteLine(weeks.ToTable("Weeks", "Sales"));
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");

			sw.Restart();
			var days = Resample(list, "2001-01".Timestamp(), "2003-12".Timestamp(), TimeSpan.FromDays(1),
				SampleWith.Lerp).AsParallel().ToList();
			_console.WriteLine("Normalized Data (by day)");
			_console.WriteLine("*************************");
			_console.WriteLine(days.ToTable("Days", "Sales"));
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");
		}

		[Fact]
		public void Can_forecast_the_future()
		{
			// Forecasting is predicting (i.e. extrapolating) values where we don't have data.
			// It's different than re-sampling only in that we re-sample using the same time resolution as the balanced data,
			// but *extend the time*, by starting before the beginning or ending after the end, or both.

			var enumerable = DataHelpers.StreamFrame<DataRow>("InputFiles/shampoo.txt");
			var sw = Stopwatch.StartNew();
			var list = enumerable.ToList();

			_console.WriteLine("Original Data:");
			_console.WriteLine("**************");
			_console.WriteLine(list.ToTable());
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");
			sw.Restart();

			// Extend the data by an additional year, and use the first of the month to match the input data
			var from = "2004-01".Timestamp(); // end of input data (2004-01)
			var to = "2005-01".Timestamp();
			var forecast = Forecast(list, from, to, PeriodFunctions.FirstOfTheMonth).AsParallel().ToList();
			_console.WriteLine("Forecast Data:");
			_console.WriteLine("**************");
			_console.WriteLine(forecast.ToTable("Month", "Sales"));
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");
		}

		[Fact]
		public void Can_resolve_overlaps()
		{
			//
			// When we have overlapping, balanced data, we need to be able to decide which value wins between overlaps.
			// If the data is unbalanced, we need to re-sample it first, as shown in the tests above.

			var sw = Stopwatch.StartNew();
			var source = DataHelpers.StreamFrame<DataRow>("InputFiles/shampoo.txt").ToList();

			// A is the full data set in days (2001-2003)
			var a = Resample(source, "2001-01".Timestamp(), "2003-12".Timestamp(), TimeSpan.FromDays(1),
					SampleWith.Lerp).AsParallel()
				.ToList();

			// B is the middle of the data set (2002)
			var b = Resample(source, "2002-01".Timestamp(), "2003-01".Timestamp().Subtract(TimeSpan.FromDays(1)),
					TimeSpan.FromDays(1), SampleWith.Lerp).AsParallel()
				.ToList();

			// With this balanced set, we need to resolve conflicts (they overlap in the middle year).
			// For this simple example, we'll just choose the last observed values (the overlapping value wins)

			// FYI: Use this function to make this concept more clear: all overlaps reduce to 0
			/*
			AggregateFunction<Data> zeroOut = (x, y) => new Data
			{
			    Month = x.Month,
			    Sales = 0
			};
			*/

			// The result set will be the same as the original set, because we didn't change any data values, but it will not contain duplicates.
			var c = a.Overlap(b, d => d.Month, AggregateFunctions<Data>.LastValueSeen);

			_console.WriteLine("Aggregated Data (by month):");
			_console.WriteLine("***************************");
			_console.WriteLine(c.ToTable());
			_console.WriteLine("({0})", sw.Elapsed);
			_console.WriteLine("");
		}
	}
}