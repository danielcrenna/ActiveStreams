// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using ActiveLogging;
using Metrics;

namespace ActiveStreams.Memory
{
	public interface IExternalMemoryProvider<T>
	{
		IEnumerable<Stream> GetAllSegments(string label);
		Stream CreateSegment(string label, int index);
		void DeleteSegment(string label, int index);

		IEnumerable<T> Read(string label, int index, IComparer<T> sort = null, ISafeLogger logger = null,
			IMetricsHost metrics = null, CancellationToken cancellationToken = default);

		IEnumerable<T> Read(Stream stream, IComparer<T> sort, ISafeLogger logger = null, IMetricsHost metrics = null,
			CancellationToken cancellationToken = default);

		SegmentStats Segment(string label, IEnumerable<T> stream, int maxWorkingMemoryBytes, ISafeLogger logger = null,
			IMetricsHost metrics = null, CancellationToken cancellationToken = default);

		void Sort(string fromLabel, string toLabel, IComparer<T> sort, ISafeLogger logger = null,
			IMetricsHost metrics = null, CancellationToken cancellationToken = default);

		IEnumerable<T> Merge(string label, SegmentStats stats, int maxWorkingMemoryBytes, ISafeLogger logger = null,
			IMetricsHost metrics = null, CancellationToken cancellationToken = default);
	}
}