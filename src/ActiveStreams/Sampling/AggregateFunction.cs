// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace ActiveStreams.Sampling
{
	public delegate T AggregateFunction<T>(T a, T b);
}