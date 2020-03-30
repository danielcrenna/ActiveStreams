// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ActiveStreams.Benchmarks
{
	[SimpleJob(RuntimeMoniker.NetCoreApp31)]
	[MarkdownExporter]
	[MemoryDiagnoser]
	[CsvMeasurementsExporter]
	public class LineCountBenchmarks
	{
		private Dictionary<int, string> _files;

		[Params(10_000_000)] public int RowCount;

		[GlobalSetup]
		public void Setup()
		{
			_files = new Dictionary<int, string>
			{
				{RowCount, new FlatFileFixture(RowCount, Encoding.UTF8, null, null, true).FilePath}
			};
		}

		[GlobalCleanup]
		public void CleanUp()
		{
			foreach (var file in _files.Values)
			{
				File.Delete(file);
			}
		}

		[Benchmark(Baseline = true, OperationsPerInvoke = 1)]
		public int SystemIO_File_ReadLines()
		{
			return File.ReadLines(_files[RowCount], Encoding.UTF8).Count();
		}

		[Benchmark(OperationsPerInvoke = 1)]
		public long HQ_LineReader_CountLines()
		{
			return LineReader.CountLines(File.OpenRead(_files[RowCount]), Encoding.UTF8, null, null,
				CancellationToken.None);
		}
	}
}