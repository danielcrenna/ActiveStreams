// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveStreams.Memory;
using Metrics;

namespace ActiveStreams.Internal
{
	internal static class BuiltInMetrics
	{
		public static void BytesPerSecond(IMetricsHost metrics, int length)
		{
			metrics?.Meter(typeof(LineReader), "bytes_read_per_second", "bytes", TimeUnit.Seconds).Mark(length);
		}

		public static void LineLength<T>(IMetricsHost metrics, int length)
		{
			metrics?.Histogram(typeof(FileMemoryProvider<T>), "line_length", SampleType.Uniform).Update(length);
		}

		public static double GetMeanLineLength<T>(IMetricsHost metrics)
		{
			return metrics?.Histogram(typeof(FileMemoryProvider<T>), "line_length", SampleType.Uniform).Mean ?? 0;
		}
	}
}