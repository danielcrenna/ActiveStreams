// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ActiveStreams.Fields
{
	[DebuggerDisplay("{" + nameof(DisplayName) + "}")]
	public readonly ref struct CharBooleanField
	{
		public bool Initialized => _buffer != null;
		public bool? Value => Initialized ? TryConvertValue() : default;
		public string RawValue => Initialized ? _encoding.GetString(_buffer) : default;
		public int Length => _buffer.Length;

		private bool? TryConvertValue()
		{
			var @char = !char.TryParse(_encoding.GetString(_buffer), out var value) ? default(char?) : value;
			if (!@char.HasValue)
			{
				return null;
			}

			if (_true.Contains(@char.Value))
			{
				return true;
			}

			if (_false.Contains(@char.Value))
			{
				return false;
			}

			return null;
		}

		private readonly Encoding _encoding;
		private readonly ReadOnlySpan<byte> _buffer;

		private readonly HashSet<char> _true;
		private readonly HashSet<char> _false;

		public CharBooleanField(HashSet<char> @true, HashSet<char> @false, ReadOnlySpan<byte> buffer, Encoding encoding)
		{
			_true = @true;
			_false = @false;
			_buffer = buffer;
			_encoding = encoding;
		}

		public unsafe CharBooleanField(HashSet<char> @true, HashSet<char> @false, byte* start, int length,
			Encoding encoding)
		{
			_true = @true;
			_false = @false;
			try
			{
				_buffer = new ReadOnlySpan<byte>(start, length);
				_encoding = encoding;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public string DisplayName =>
			$"{nameof(CharBooleanField).Replace("Field", string.Empty)}: {Value} ({RawValue ?? "<NULL>"}:{_encoding.BodyName})";
	}
}