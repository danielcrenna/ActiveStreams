// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace ActiveStreams.Sampling
{
	public delegate TOut ResampleFunction<in TIn, out TOut>(DateTimeOffset sample, TIn lastValue, TIn nextValue,
		double distance /* between 0 and 1 */);
}