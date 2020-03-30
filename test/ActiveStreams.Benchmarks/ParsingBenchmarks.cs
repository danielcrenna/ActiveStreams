// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ActiveStreams.Fields;
using ActiveStreams.Internal;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CsvHelper;
using CsvHelper.Configuration;
using FileHelpers;

namespace ActiveStreams.Benchmarks
{
	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	[MarkdownExporter]
	[MemoryDiagnoser]
	[CsvMeasurementsExporter]
	public class ParsingBenchmarks
	{
		private static readonly Encoding Encoding = Encoding.UTF8;
		private CsvConfiguration _configuration;
		private FileHelperAsyncEngine<Row> _engine;

		private Dictionary<int, string> _files;
		private byte[] _separator;

		[Params(1000 /*, 10_000, 100_000, 1_000_000, 10_000_000*/)]
		public int RowCount;

		[GlobalSetup]
		public void Setup()
		{
			_files = new Dictionary<int, string>
			{
				{RowCount, new FlatFileFixture(RowCount, 3, Encoding, "A,B,C", ",", true).FilePath}
			};

			_engine = new FileHelperAsyncEngine<Row> {Options = {IgnoreFirstLines = 1}};
			_configuration = new CsvConfiguration(CultureInfo.InvariantCulture);
			_separator = Encoding.GetSeparatorBuffer(",");
		}

		[GlobalCleanup]
		public void CleanUp()
		{
			foreach (var file in _files.Values)
				File.Delete(file);
		}

		[Benchmark(Baseline = false, OperationsPerInvoke = 1)]
		public int FileHelpers_Parse()
		{
			CheckRowCount(File.OpenRead(_files[RowCount]));

			var rows = 0;
			using (_engine.BeginReadFile(_files[RowCount]))
			{
				foreach (var row in _engine)
				{
					CheckRowValues(row, rows + 1);
					rows++;
				}
			}

			return CheckReadCount(rows);
		}

		[Benchmark(Baseline = false, OperationsPerInvoke = 1)]
		public int CsvHelper_Parse()
		{
			var rows = 0;

			CheckRowCount(File.OpenRead(_files[RowCount]));

			using var reader = new StreamReader(_files[RowCount]);
			using var csv = new CsvReader(reader, _configuration);

			var record = new Row();
			var records = csv.EnumerateRecords(record);
			foreach (var row in records)
			{
				CheckRowValues(row, rows + 1);
				rows++;
			}

			return CheckReadCount(rows);
		}

		private void CheckRowCount(Stream stream)
		{
			var linesInFile = LineReader.CountLines(stream, Encoding);
			if (linesInFile < RowCount)
				throw new ArgumentException($"file does not have at least {RowCount} rows");

			if (stream.CanSeek)
				stream.Position = 0L;
		}

		private int CheckReadCount(int rows)
		{
			if (rows != RowCount)
				throw new ArgumentException($"did not read every row ({rows} / {RowCount})");

			return rows;
		}

		private static void CheckRowValues(Row row, long lineNumber)
		{
			if (string.IsNullOrWhiteSpace(row.A))
				throw new InvalidOperationException($"[Line {lineNumber}] a was {row.A}");
			if (string.IsNullOrWhiteSpace(row.B))
				throw new InvalidOperationException($"[Line {lineNumber}] b was {row.B}");
			if (string.IsNullOrWhiteSpace(row.C))
				throw new InvalidOperationException($"[Line {lineNumber}] c was {row.C}");
		}

		private static void CheckRowValues(RowLayout row, long lineNumber)
		{
			if (string.IsNullOrWhiteSpace(row.A.Value))
				throw new InvalidOperationException($"[Line {lineNumber}] a was {row.A.RawValue}");
			if (string.IsNullOrWhiteSpace(row.B.Value))
				throw new InvalidOperationException($"[Line {lineNumber}] b was {row.B.RawValue}");
			if (string.IsNullOrWhiteSpace(row.C.Value))
				throw new InvalidOperationException($"[Line {lineNumber}] c was {row.C.RawValue}");
		}

		[DelimitedRecord(",")]
		public class Row
		{
			public string A { get; set; }
			public string B { get; set; }
			public string C { get; set; }
		}

		#region HQ (Manual)

		[Benchmark(Baseline = false, OperationsPerInvoke = 1)]
		public int HQ_Parse_Manual()
		{
			var rows = 0;
			var fs = File.OpenRead(_files[RowCount]);

			CheckRowCount(fs);

			foreach (var ctor in LineReader.StreamLines(fs, Encoding))
			{
				var row = new RowLayout(ctor, Encoding, _separator);
				CheckRowValues(row, ctor.lineNumber);
				rows++;
			}

			return CheckReadCount(rows);
		}

		public ref struct RowLayout
		{
			public StringField A;
			public StringField B;
			public StringField C;
			public StringField ExtraFields;

			public RowLayout(LineConstructor constructor, Encoding encoding, byte[] separator)
			{
				A = default;
				B = default;
				C = default;
				ExtraFields = default;

				SetFromLineConstructor(constructor, encoding, separator);
			}

			private unsafe void SetFromLineConstructor(LineConstructor constructor, Encoding encoding, byte[] separator)
			{
				fixed (byte* from = constructor.buffer)
				{
					var start = from;
					var length = constructor.length;
					var column = 0;
					while (true)
					{
						var line = new ReadOnlySpan<byte>(start, length);
						var next = line.IndexOf(separator);
						if (next == -1)
						{
							if (line.IndexOf(Constants.CarriageReturn) > -1)
							{
								C = new StringField(start, length - 2, encoding);
							}
							else if (line.IndexOf(Constants.LineFeed) > -1)
							{
								C = new StringField(start, length - 1, encoding);
							}
							else
							{
								C = new StringField(start, length, encoding);
							}

							break;
						}

						var consumed = next + separator.Length;
						length -= next + separator.Length;

						switch (column)
						{
							case 0:
								A = new StringField(start, next, encoding);
								break;
							case 1:
								B = new StringField(start, next, encoding);
								break;
							case 2:
								C = new StringField(start, next, encoding);
								break;
							default:
								ExtraFields = ExtraFields.Initialized
									? ExtraFields.AddLength(next)
									: new StringField(start, next, encoding);
								break;
						}

						start += consumed;
						column++;
					}
				}
			}
		}

		#endregion

		#region Hand-Made

		[Benchmark(Baseline = true, OperationsPerInvoke = 1)]
		public int Handmade_Parse()
		{
			var rows = 0;

			var fs = File.OpenRead(_files[RowCount]);
			CheckRowCount(fs);

			foreach (var row in StreamRows(fs, 1))
			{
				CheckRowValues(row, rows + 1);
				rows++;
			}

			return CheckReadCount(rows);
		}

		private static IEnumerable<Row> StreamRows(Stream stream, int skipRows)
		{
			var cb = new char[1];
			using var sr = new StreamReader(stream, Encoding);

			for (var i = 0; i < skipRows; i++)
				sr.ReadLine();

			var col = 0;
			var quoted = false;

			var a = new StringBuilder();
			var b = new StringBuilder();
			var c = new StringBuilder();

			while (sr.Read(cb) > 0)
			{
				switch (cb[0])
				{
					case '\n':
						continue;
					case '\r':
						yield return new Row {A = a.ToString(), B = b.ToString(), C = c.ToString()};
						a.Clear();
						b.Clear();
						c.Clear();
						col = 0;
						continue;
					case '"':
						quoted = !quoted;
						break;
				}

				if (cb[0] == ',' && !quoted)
					col++;
				else
				{
					switch (col)
					{
						// A
						case 0:
							a.Append(cb[0]);
							break;

						// B
						case 1:
							b.Append(cb[0]);
							break;

						// C
						case 2:
							c.Append(cb[0]);
							break;
					}
				}
			}
		}

		#endregion
	}
}