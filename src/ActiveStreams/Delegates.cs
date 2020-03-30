// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace ActiveStreams
{
	public unsafe delegate void NewLine(long lineNumber, bool partial, byte* start, int length, Encoding encoding);

	public unsafe delegate void NewValue(long lineNumber, int index, byte* start, int length, Encoding encoding);

	public delegate void NewLineAsString(long lineNumber, string value);

	public delegate void NewValueAsSpan(long lineNumber, int index, ReadOnlySpan<byte> value, Encoding encoding);
}