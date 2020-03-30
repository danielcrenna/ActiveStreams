// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Text;
using Xunit;

namespace ActiveStreams.Tests
{
	public class ParsingTests
	{
		[Fact]
		public void Can_parse_line_values()
		{
			var values = 0;
			var encoding = Encoding.UTF8;
			using var fixture = new FlatFileFixture(1000, encoding, ",");
			var sw = Stopwatch.StartNew();
			unsafe
			{
				LineReader.ReadLines(fixture.FileStream, encoding, ",", (n, i, start, length, e) =>
				{
					values++;
					e.TryParse(start, length, out bool _);
				});
			}

			Trace.WriteLine($"{values} cells took {sw.Elapsed} to parse.");
		}
	}
}