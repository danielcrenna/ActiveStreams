// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using ActiveStreams.Internal;

namespace ActiveStreams
{
	public static class EncodingExtensions
	{
		#region Header

		public static string GetHeaderText<TMetadata>(this Encoding encoding, ReadOnlySpan<byte> separator)
		{
			return LineReader.GetHeaderText<TMetadata>(encoding.GetString(separator));
		}

		#endregion

		#region Separator

		public static byte[] GetSeparatorBuffer(this Encoding encoding, string separator)
		{
			if (!WorkingSeparators.TryGetValue(encoding, out var buffers))
			{
				WorkingSeparators.Add(encoding, buffers = new Dictionary<string, byte[]>());
			}

			if (!buffers.TryGetValue(separator, out var buffer))
			{
				buffers.Add(separator, buffer = BuildSeparatorBuffer(encoding, separator));
			}

			return buffer;
		}

		private static unsafe byte[] BuildSeparatorBuffer(Encoding encoding, string delimiter)
		{
			byte[] separator;
			fixed (char* chars = delimiter)
			{
				var byteCount = encoding.GetByteCount(chars, delimiter.Length);
				fixed (byte* buffer = separator = new byte[byteCount])
				{
					encoding.GetBytes(chars, delimiter.Length, buffer, byteCount);
				}
			}

			return separator;
		}

		private static readonly Dictionary<Encoding, Dictionary<string, byte[]>> WorkingSeparators =
			new Dictionary<Encoding, Dictionary<string, byte[]>>
			{
				{Encoding.UTF7, BuildSeparatorBuffers(Encoding.UTF7)},
				{Encoding.UTF8, BuildSeparatorBuffers(Encoding.UTF8)},
				{Encoding.Unicode, BuildSeparatorBuffers(Encoding.Unicode)},
				{Encoding.BigEndianUnicode, BuildSeparatorBuffers(Encoding.BigEndianUnicode)},
				{Encoding.UTF32, BuildSeparatorBuffers(Encoding.UTF32)},
				{Constants.BigEndianUtf32, BuildSeparatorBuffers(Constants.BigEndianUtf32)}
			};

		private static Dictionary<string, byte[]> BuildSeparatorBuffers(Encoding encoding)
		{
			return new Dictionary<string, byte[]>
			{
				{Constants.Comma, BuildSeparatorBuffer(encoding, Constants.Comma)},
				{Constants.Tab, BuildSeparatorBuffer(encoding, Constants.Tab)},
				{Constants.Pipe, BuildSeparatorBuffer(encoding, Constants.Pipe)}
			};
		}

		#endregion

		#region Preamble

		public static byte[] GetPreambleBuffer(this Encoding encoding)
		{
			if (!WorkingPreambles.TryGetValue(encoding, out var buffer))
			{
				WorkingPreambles.Add(encoding, buffer = encoding.GetPreamble());
			}

			return buffer;
		}

		private static readonly Dictionary<Encoding, byte[]> WorkingPreambles = new Dictionary<Encoding, byte[]>
		{
			{Encoding.UTF7, Encoding.UTF7.GetPreamble()},
			{Encoding.UTF8, Encoding.UTF8.GetPreamble()},
			{Encoding.Unicode, Encoding.Unicode.GetPreamble()},
			{Encoding.BigEndianUnicode, Encoding.BigEndianUnicode.GetPreamble()},
			{Encoding.UTF32, Encoding.UTF32.GetPreamble()},
			{Constants.BigEndianUtf32, Encoding.UTF32.GetPreamble()}
		};

		#endregion

		#region CharBuffer

		public static char[] GetCharBuffer(this Encoding encoding)
		{
			if (!WorkingChars.TryGetValue(encoding, out var buffer))
			{
				WorkingChars.Add(encoding, buffer = new char[encoding.GetMaxCharCount(Constants.BufferLength)]);
			}

			return buffer;
		}

		private static readonly Dictionary<Encoding, char[]> WorkingChars = new Dictionary<Encoding, char[]>
		{
			{Encoding.UTF7, new char[Encoding.UTF7.GetMaxCharCount(Constants.BufferLength)]},
			{Encoding.UTF8, new char[Encoding.UTF8.GetMaxCharCount(Constants.BufferLength)]},
			{Encoding.Unicode, new char[Encoding.Unicode.GetMaxCharCount(Constants.BufferLength)]},
			{
				Encoding.BigEndianUnicode,
				new char[Encoding.BigEndianUnicode.GetMaxCharCount(Constants.BufferLength)]
			},
			{Encoding.UTF32, new char[Encoding.UTF32.GetMaxCharCount(Constants.BufferLength)]},
			{Constants.BigEndianUtf32, new char[Encoding.UTF32.GetMaxCharCount(Constants.BufferLength)]}
		};

		#endregion
	}
}