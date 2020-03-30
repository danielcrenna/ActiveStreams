// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using TypeKitchen;

namespace ActiveStreams.Sampling
{
	public static class TableExtensions
	{
		public static string ToTable<T>(this IEnumerable<T> values)
		{
			return ToTable(values,
				AccessorMembers.Create(typeof(T), AccessorMemberTypes.Fields | AccessorMemberTypes.Properties,
					AccessorMemberScope.Public).Names.ToArray());
		}

		public static string ToTable<T>(this IEnumerable<T> values, params string[] columns)
		{
			return BuildTable(values.ToArray(), "<NULL>", columns);
		}

		private static string BuildTable<T>(this IReadOnlyList<T> rows, string @null, params string[] columnNames)
		{
			var accessor = ReadAccessor.Create(typeof(T), AccessorMemberTypes.Fields | AccessorMemberTypes.Properties,
				AccessorMemberScope.Public, out var members);

			var columns = members.ToArray();

			var table = new string[rows.Count + 1, columnNames.Length];
			for (var i = 0; i < table.GetLength(1); i++)
				table[0, i] = columnNames[i];

			for (var i = 1; i < table.GetLength(0); i++)
			for (var j = 0; j < table.GetLength(1); j++)
			{
				accessor.TryGetValue(rows[i - 1], columns[j].Name, out var value);
				table[i, j] = value?.ToString() ?? @null ?? "?";
			}

			return RenderTable(table);
		}

		private static string RenderTable(this string[,] table)
		{
			var cellPadding = new int[table.GetLength(1)];

			for (var i = 0; i < table.GetLength(1); i++)
			for (var j = 0; j < table.GetLength(0); j++)
			{
				var cell = table[j, i];
				if (cell.Length <= cellPadding[i])
					continue;
				cellPadding[i] = cell.Length;
			}

			return Pooling.StringBuilderPool.Scoped(sb =>
			{
				AppendBorder(sb, cellPadding);
				var rows = table.GetLength(0);
				var columns = table.GetLength(1);
				for (var i = 0; i < rows; i++)
				{
					for (var j = 0; j < columns; j++)
					{
						sb.Append(" | ");
						sb.Append(table[i, j].PadRight(cellPadding[j]));
					}

					sb.AppendLine(" | ");
					if (i == 0)
						AppendBorder(sb, cellPadding);
				}

				AppendBorder(sb, cellPadding);
			});
		}

		private static void AppendBorder(StringBuilder sb, IReadOnlyList<int> padding)
		{
			sb.Append(" +");
			var dashes = padding.Sum(width => width + 3) - 1;
			for (var i = 0; i < dashes; i++)
			{
				var drawn = false;
				var p = 0;
				for (var j = 0; j < padding.Count - 1; j++)
				{
					p += padding[j];
					if (p != i - 2) continue;
					sb.Append('+');
					drawn = true;
					break;
				}

				if (!drawn) sb.Append('-');
			}

			sb.Append("+ ");
			sb.AppendLine();
		}
	}
}