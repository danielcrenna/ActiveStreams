// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using BenchmarkDotNet.Running;

namespace ActiveStreams.Benchmarks
{
	internal class Program
	{
		private static void Main()
		{
			BenchmarkRunner.Run<LineCountBenchmarks>();
			BenchmarkRunner.Run<ParsingBenchmarks>();
		}
	}
}