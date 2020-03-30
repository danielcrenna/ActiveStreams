// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace ActiveStreams
{
	/// <summary>
	///     Used when calculating the difference between two <see cref="DateTime" /> instances
	///     with the <see cref="DateSpan" /> class.
	/// </summary>
	public enum DateInterval
	{
		/// <summary>
		///     Years
		/// </summary>
		Years,

		/// <summary>
		///     Months
		/// </summary>
		Months,

		/// <summary>
		///     Weeks
		/// </summary>
		Weeks,

		/// <summary>
		///     Days
		/// </summary>
		Days,

		/// <summary>
		///     Hours
		/// </summary>
		Hours,

		/// <summary>
		///     Minutes
		/// </summary>
		Minutes,

		/// <summary>
		///     Seconds
		/// </summary>
		Seconds
	}
}