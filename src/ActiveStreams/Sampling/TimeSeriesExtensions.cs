// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ActiveStreams.Sampling
{
	public static class TimeSeriesExtensions
	{
		public static IEnumerable<TOut> Resample<TIn, TOut>(this IEnumerable<TIn> stream, DateTimeOffset to,
			TimeSpan period, Func<TIn, DateTimeOffset> getTimestamp,
			Func<DateTimeOffset, TIn, TIn, double, TOut> gapFill)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			var first = stream.First();
			var from = getTimestamp(first);

			// ReSharper disable once PossibleMultipleEnumeration
			return stream.Resample(from, to, period, getTimestamp, gapFill);
		}

		public static IEnumerable<TOut> Resample<TIn, TOut>(this IEnumerable<TIn> stream, DateTimeOffset to,
			Func<DateTimeOffset, TimeSpan> period, Func<TIn, DateTimeOffset> getTimestamp,
			Func<DateTimeOffset, TIn, TIn, double, TOut> gapFill)
		{
			// ReSharper disable once PossibleMultipleEnumeration
			var first = stream.First();
			var startAt = getTimestamp(first);

			// ReSharper disable once PossibleMultipleEnumeration
			return stream.Resample(startAt, to, period, getTimestamp, gapFill);
		}

		public static IEnumerable<TOut> Resample<TIn, TOut>(this IEnumerable<TIn> stream, DateTimeOffset from,
			DateTimeOffset to, TimeSpan period, Func<TIn, DateTimeOffset> getTimestamp,
			Func<DateTimeOffset, TIn, TIn, double, TOut> gapFill)
		{
			return stream.Resample(from, to, PeriodFunctions.Fixed(period), getTimestamp, gapFill);
		}

		public static IEnumerable<TOut> Resample<TIn, TOut>(this IEnumerable<TIn> stream, DateTimeOffset from,
			DateTimeOffset to, Func<DateTimeOffset, TimeSpan> period, Func<TIn, DateTimeOffset> getTimestamp,
			Func<DateTimeOffset, TIn, TIn, double, TOut> gapFill)
		{
			using (var e = stream.GetEnumerator())
			{
				if (!e.MoveNext())
					yield break;

				var row = e.Current;
				var timestamp = getTimestamp(row);

				foreach (var @out in FillRange(row, from, timestamp, period, gapFill, 0))
					yield return @out;

				while (from <= to && e.MoveNext())
				{
					var nextRow = e.Current;
					var nextTimestamp = getTimestamp(nextRow);

					var interval = nextTimestamp - timestamp;

					while (from <= to && from < nextTimestamp)
					{
						var distance = (from - timestamp).TotalMilliseconds / interval.TotalMilliseconds;
						yield return gapFill(from, row, nextRow, distance);
						from += period(from);
					}

					foreach (var @out in FillRange(row, from, timestamp, period, gapFill, 0))
						yield return @out;

					row = nextRow;
					timestamp = nextTimestamp;
				}

				foreach (var @out in FillRange(row, from, to, period, gapFill, 1))
					yield return @out;
			}
		}

		public static IEnumerable<T> Merge<T>(this IEnumerable<T> a, IEnumerable<T> b,
			Func<T, DateTimeOffset> getTimestamp)
		{
			using (var x = a.GetEnumerator())
			using (var y = b.GetEnumerator())
			{
				var xr = x.MoveNext();
				var yr = y.MoveNext();
				while (xr || yr)
				{
					if (!xr)
					{
						do
							yield return y.Current;
						while (y.MoveNext());

						yield break;
					}

					if (!yr)
					{
						do
							yield return x.Current;
						while (x.MoveNext());

						yield break;
					}

					if (getTimestamp(x.Current) < getTimestamp(y.Current))
					{
						yield return x.Current;
						xr = x.MoveNext();
					}
					else
					{
						yield return y.Current;
						yr = y.MoveNext();
					}
				}
			}
		}

		public static IEnumerable<T> Overlap<T>(this IEnumerable<T> mergeInto, IEnumerable<T> mergeFrom,
			Func<T, DateTimeOffset> getTimestamp, AggregateFunction<T> overlap)
		{
			var merged = mergeInto.Merge(mergeFrom, getTimestamp);

			// ReSharper disable once PossibleMultipleEnumeration
			var previous = merged.First();
			var previousTimestamp = getTimestamp(previous);

			// ReSharper disable once PossibleMultipleEnumeration
			foreach (var next in merged.Skip(1))
			{
				var nextTimestamp = getTimestamp(next);
				if (nextTimestamp == previousTimestamp)
				{
					previous = overlap(previous, next);
					previousTimestamp = getTimestamp(previous);
					continue;
				}

				yield return previous;
				previous = next;
				previousTimestamp = nextTimestamp;
			}

			yield return previous;
		}

		private static IEnumerable<TOut> FillRange<TIn, TOut>(TIn row, DateTimeOffset from, DateTimeOffset to,
			Func<DateTimeOffset, TimeSpan> period, Func<DateTimeOffset, TIn, TIn, double, TOut> gapFill,
			double distance)
		{
			while (from <= to)
			{
				yield return gapFill(from, row, row, distance);
				from += period(from);
			}
		}
	}
}