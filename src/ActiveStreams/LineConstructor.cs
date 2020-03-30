// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace ActiveStreams
{
	[DebuggerDisplay("{lineNumber}: 0-{length}")]
	public struct LineConstructor
	{
		public long lineNumber;
		public byte[] buffer;
		public int length;
	}
}