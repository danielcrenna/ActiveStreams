// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace ActiveStreams.Sampling
{
	public static class FunctionExtensions
	{
		public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f)
		{
			var d = new Dictionary<T, TResult>();
			return x =>
			{
				if (d.TryGetValue(x, out var r))
					return r;
				r = f(x);
				d.Add(x, r);
				return r;
			};
		}
	}
}