// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text;

namespace ActiveStreams.Fields
{
	[DebuggerDisplay("{" + nameof(DisplayName) + "}")]
	public readonly ref struct DecimalField
	{
		public bool Initialized => _buffer != null;

		public decimal? Value =>
			Initialized ? !_encoding.TryParse(_buffer, out decimal value) ? default(decimal?) : value : default;

		public string RawValue => Initialized ? _encoding.GetString(_buffer) : default;
		public int Length => _buffer.Length;

		private readonly Encoding _encoding;
		private readonly ReadOnlySpan<byte> _buffer;

		public DecimalField(ReadOnlySpan<byte> buffer, Encoding encoding)
		{
			_buffer = buffer;
			_encoding = encoding;
		}

		public unsafe DecimalField(byte* start, int length, Encoding encoding)
		{
			_buffer = new ReadOnlySpan<byte>(start, length);
			_encoding = encoding;
		}

		public string DisplayName =>
			$"{nameof(DecimalField).Replace("Field", string.Empty)}: {Value} ({RawValue ?? "<NULL>"}:{_encoding.BodyName})";
	}
}