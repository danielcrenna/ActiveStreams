// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;

namespace ActiveStreams.Sampling
{
	public static class PeriodFunctions
	{
		public static Func<DateTimeOffset, TimeSpan> FirstOfTheMonth = d =>
		{
			var first = new DateTimeOffset(d.Year, d.Month, 1, 0, 0, 0, d.Offset);
			Debug.Assert(first.Day == 1);
			var span = first.AddMonths(1) - first;
			Debug.Assert((first + span).Day == 1);
			return span;
		};

		public static Func<DateTimeOffset, TimeSpan> LastOfTheMonth = d =>
		{
			var first = new DateTimeOffset(d.Year, d.Month, 1, 0, 0, 0, d.Offset);
			Debug.Assert(first.Day == 1);
			var span = first.AddMonths(2).AddDays(-1) - first;
			Debug.Assert((first + span).AddDays(1).Day == 1);
			return span;
		};

		public static Func<DateTimeOffset, TimeSpan> Fixed(TimeSpan period)
		{
			return d => period;
		}
	}
}