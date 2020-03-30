// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data;
using Microsoft.ML;
using TypeKitchen;

namespace ActiveStreams
{
	public class DataHelpers
	{
		public static IEnumerable<T> StreamFrame<T>(string filePath, bool reuseRowObject = false) where T : class, new()
		{
			// We need to be explicit here because the CSV reader will guess at primitives like `float` when `double` is defined in the mode.
			var members = AccessorMembers.Create(typeof(T), AccessorMemberTypes.Fields | AccessorMemberTypes.Properties,
				AccessorMemberScope.Public);
			var inputFrame = DataFrame.ReadCsv(filePath, columnNames: members.Names.ToArray(),
				dataTypes: members.Select(x => x.Type).ToArray());
			var context = new MLContext();
			var enumerable = context.Data.CreateEnumerable<T>(inputFrame, reuseRowObject);
			return enumerable;
		}
	}
}