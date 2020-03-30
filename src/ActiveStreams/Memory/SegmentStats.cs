// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace ActiveStreams.Memory
{
	public struct SegmentStats
	{
		public long RecordCount { get; set; }
		public int RecordLength { get; set; }
		public int SegmentCount { get; set; }
	}
}