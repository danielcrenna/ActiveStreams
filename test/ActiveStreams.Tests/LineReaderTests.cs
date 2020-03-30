// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using ActiveStreams.Fields;
using ActiveStreams.Internal;
using Xunit;
using Xunit.Abstractions;

namespace ActiveStreams.Tests
{
	public class LineReaderTests
	{
		public LineReaderTests(ITestOutputHelper console) => _console = console;

		private readonly ITestOutputHelper _console;

		public class DummyDataMetadata
		{
			#region Attributes

			[Display(Name = "Name", Description = "Name", Order = 1, ShortName = null)]
			[Column("Name", TypeName = "string", Order = 1)]
			[Required]
			[ReadOnly(false)]
			[DataMember]
			[Description("")]

			#endregion

			public string SomeField;
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

		[Fact]
		public void Can_count_lines()
		{
			const long expected = 10000L;
			using var fixture = new FlatFileFixture((int) expected, Encoding.UTF8);

			var sw = Stopwatch.StartNew();
			var lines = LineReader.CountLines(fixture.FileStream, Encoding.UTF8);
			Assert.Equal(expected, lines);
			_console.WriteLine($"{lines} lines took {sw.Elapsed} to read.");
		}

		[Fact]
		public void Can_count_lines_ranged()
		{
			const long expected = 10000L;
			using var fixture = new FlatFileFixture((int) expected, Encoding.UTF8);

			var range = new RangeStream(fixture.FileStream, 0, 5000);
			var sw = Stopwatch.StartNew();
			var lines = LineReader.CountLines(range, Encoding.UTF8);
			Assert.True(lines < 150);
			_console.WriteLine($"{lines} lines took {sw.Elapsed} to read.");
		}

		[Fact]
		public void Can_get_header_text()
		{
			var header = LineReader.GetHeaderText<DummyDataMetadata>("|");
			Assert.NotEmpty(header); // "header was not generated"
			Assert.Equal("Name", header); // "header doesn't use display attribute name, if available"
		}

		[Fact]
		public void Can_read_string_lines()
		{
			using var fixture = new FlatFileFixture(10000, Encoding.UTF8);

			var lines = 0L;
			var sw = Stopwatch.StartNew();
			LineReader.ReadLines(fixture.FileStream, Encoding.UTF8, (lineNumber, line) =>
			{
				Assert.NotNull(line);
				lines = lineNumber;
			});
			_console.WriteLine($"{lines} lines took {sw.Elapsed} to read.");
		}

		[Fact]
		public void Can_stream_constructor_lines()
		{
			const string separator = "|";
			var encoding = Encoding.UTF8;

			var separatorBuffer = encoding.GetSeparatorBuffer(separator);

			const int rowCount = 1000;
			using var fixture = new FlatFileFixture(rowCount, 3, encoding, "A|B|C", separator);

			var lines = 0;
			var sw = Stopwatch.StartNew();

			foreach (var ctor in LineReader.StreamLines(fixture.FileStream, encoding))
			{
				var row = new RowLayout(ctor, encoding, separatorBuffer);

				if (row.A.Value == null || row.B.Value == null || row.C.Value == null)
				{
					Trace.WriteLine(lines);
				}

				Assert.NotNull(row.A.Value);
				Assert.NotNull(row.B.Value);
				Assert.NotNull(row.C.Value);
				lines++;
			}

			Assert.Equal(rowCount + 1, lines);
			_console.WriteLine($"{lines} lines took {sw.Elapsed} to read.");
		}
	}
}