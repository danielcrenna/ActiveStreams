// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Buffers.Text;
using System.Text;

namespace ActiveStreams
{
	public static class ParsingExtensions
	{
		public static string GetString(this Encoding encoding, ReadOnlySpan<byte> buffer)
		{
			unsafe
			{
				fixed (byte* b = buffer)
				{
					return encoding.GetString(b, buffer.Length);
				}
			}
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out byte value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return byte.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out sbyte value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return sbyte.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out bool value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return bool.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out short value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return short.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out ushort value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return ushort.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out int value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return int.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out uint value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return uint.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out long value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return long.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out ulong value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return ulong.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out float value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return float.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out double value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return double.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out decimal value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return decimal.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out DateTime value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return DateTime.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out DateTimeOffset value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return DateTimeOffset.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out TimeSpan value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return TimeSpan.TryParse(field, out value);
		}

		public static bool TryParse(this Encoding encoding, ReadOnlySpan<byte> buffer, out Guid value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(buffer, out value, out _);
			}

			var field = encoding.GetString(buffer);
			return Guid.TryParse(field, out value);
		}

		#region Unsafe

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out byte value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return byte.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out sbyte value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return sbyte.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out bool value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return bool.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out short value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return short.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out ushort value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return ushort.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out int value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return int.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out uint value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return uint.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out long value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return long.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out ulong value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return ulong.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out float value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return float.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out double value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return double.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out decimal value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return decimal.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out DateTimeOffset value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return DateTimeOffset.TryParse(field, out value);
		}

		public static unsafe bool TryParse(this Encoding encoding, byte* start, int length, out TimeSpan value)
		{
			if (encoding.Equals(Encoding.UTF8))
			{
				return Utf8Parser.TryParse(new ReadOnlySpan<byte>(start, length), out value, out _);
			}

			var field = encoding.GetString(start, length);
			return TimeSpan.TryParse(field, out value);
		}

		#endregion
	}
}