// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Text;
using Xunit;

namespace ActiveStreams.Tests
{
	public class LineValuesReaderTests
	{
		[Fact]
		public unsafe void Can_read_line_values()
		{
			var values = 0;
			var encoding = Encoding.UTF8;
			using var fixture = new FlatFileFixture(1000, encoding, ",");

			var sw = Stopwatch.StartNew();

			LineReader.ReadLines(fixture.FileStream, encoding, ",", (n, i, v, e, m) => { values++; });

			Trace.WriteLine($"{values} cells took {sw.Elapsed} to read.");
		}
	}
}