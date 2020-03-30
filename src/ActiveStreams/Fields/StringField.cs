// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text;

namespace ActiveStreams.Fields
{
	[DebuggerDisplay("{" + nameof(DisplayName) + "}")]
	public readonly ref struct StringField
	{
		public bool Initialized => _buffer != null;
		public string Value => RawValue;
		public string RawValue => Initialized ? _encoding.GetString(_buffer) : default;
		public int Length => _buffer.Length;

		private readonly unsafe byte* _start;
		private readonly int _length;
		private readonly Encoding _encoding;
		private readonly ReadOnlySpan<byte> _buffer;

		public unsafe StringField(byte* start, int length, Encoding encoding)
		{
			_buffer = new ReadOnlySpan<byte>(start, length);
			_start = start;
			_length = length;
			_encoding = encoding;
		}

		public unsafe StringField AddLength(int length)
		{
			return new StringField(_start, _length + length, _encoding);
		}

		public string DisplayName =>
			$"{nameof(StringField).Replace("Field", string.Empty)}: {Value} ({RawValue ?? "<NULL>"}:{_encoding.BodyName})";
	}
}