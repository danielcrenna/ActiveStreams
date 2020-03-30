// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace ReactiveSampling.Tests
{
	internal static class TestExtensions
	{
		public static DateTimeOffset Timestamp(this string yearMonth)
		{
			return DateTimeOffset.ParseExact(yearMonth, "yyyy-MM", CultureInfo.InvariantCulture);
		}
	}
}