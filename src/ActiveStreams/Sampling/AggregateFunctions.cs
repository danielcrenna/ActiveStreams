// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace ActiveStreams.Sampling
{
	public static class AggregateFunctions<T>
	{
		public static AggregateFunction<T> LastValueSeen = (a, b) => b;
	}
}