// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace ActiveStreams.Sampling
{
	public static class ResampleFunctions
	{
		#region Pure Functions

		public static ResampleFunction<double, double> Lerp = (interval, lastValue, nextValue, distance) =>
			lastValue + (nextValue - lastValue) * distance;

		public static ResampleFunction<double, double> Average(int samples)
		{
			return (interval, lastValue, nextValue, distance) => (lastValue + nextValue) / samples;
		}

		#endregion

		#region Continuous Functions

		public static ResampleFunction<double, double> SimpleMovingAverage = SimpleMovingAverageImpl(1, 0);

		private static ResampleFunction<double, double> SimpleMovingAverageImpl(int count, int sum)
		{
			//
			// IMPORTANT: We need to memoize the function so we can maintain state between calls and avoid
			//            enumerating the entire sequence on each invocation.

			return (interval, lastValue, nextValue, distance) =>
			{
				return sum + nextValue / count++;
			};
		}

		#endregion
	}
}