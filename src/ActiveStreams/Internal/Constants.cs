// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace ActiveStreams.Internal
{
	internal static class Constants
	{
		public const byte CarriageReturn = (byte) '\r';
		public const byte LineFeed = (byte) '\n';
		public const string Comma = ",";
		public const string Tab = "\t";
		public const string Pipe = "|";

		public const int ReadAheadSize = 128;
		public const int PadSize = 4;
		public const int BlockSize = 4096;
		public const int BufferLength = 4228; /* ReadAheadSize + BlockSize + PadSize */

		[ThreadStatic] private static byte[] _buffer;

		public static readonly UTF32Encoding BigEndianUtf32 = new UTF32Encoding(true, true);
		public static byte[] Buffer => _buffer ??= new byte[BufferLength];
	}
}