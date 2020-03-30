// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Globalization;

namespace ActiveStreams.Internal
{
	internal static class FloatExtensions
	{
		public static bool TryParseFast(string s, out float result)
		{
			// See: https://github.com/dotnet/coreclr/issues/20938
			var info = NumberFormatInfo.CurrentInfo;
			return float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, info, out result);
		}
	}
}