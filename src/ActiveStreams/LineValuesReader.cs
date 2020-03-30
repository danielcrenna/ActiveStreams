// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace ActiveStreams
{
	public static class LineValuesReader
	{
		public static unsafe void ReadValues(long lineNumber, byte* start, int length, Encoding encoding,
			string separator, NewValueAsSpan newValue)
		{
			ReadValues(lineNumber, start, length, encoding, encoding.GetSeparatorBuffer(separator), newValue);
		}

		public static unsafe void ReadValues(long lineNumber, byte* start, int length, Encoding encoding,
			byte[] separator, NewValueAsSpan newValue)
		{
			ReadValues(lineNumber, new ReadOnlySpan<byte>(start, length), encoding, separator, newValue);
		}

		public static void ReadValues(long lineNumber, ReadOnlySpan<byte> line, Encoding encoding, byte[] separator,
			NewValueAsSpan newValue)
		{
			var position = 0;
			while (true)
			{
				var next = line.IndexOf(separator);
				if (next == -1)
				{
					newValue?.Invoke(lineNumber, position, line, encoding);
					break;
				}

				newValue?.Invoke(lineNumber, position, line.Slice(0, next), encoding);
				line = line.Slice(next + separator.Length);
				position += next + separator.Length;
			}
		}

		public static unsafe void ReadValues(long lineNumber, byte* start, int length, Encoding encoding,
			string separator, NewValue newValue)
		{
			ReadValues(lineNumber, start, length, encoding, encoding.GetSeparatorBuffer(separator), newValue);
		}

		public static unsafe void ReadValues(long lineNumber, byte* start, int length, Encoding encoding,
			byte[] separator, NewValue newValue)
		{
			var position = 0;
			while (true)
			{
				var line = new ReadOnlySpan<byte>(start, length);
				var next = line.IndexOf(separator);
				if (next == -1)
				{
					newValue?.Invoke(lineNumber, position, start, length, encoding);
					break;
				}

				newValue?.Invoke(lineNumber, position, start, next, encoding);
				var consumed = next + separator.Length;
				start += consumed;
				position += consumed;
			}
		}
	}
}