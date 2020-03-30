// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Text;
using Bogus.DataSets;
using TypeKitchen;

namespace ActiveStreams.Benchmarks
{
	public class FlatFileFixture : TemporaryFileFixture
	{
		private static readonly Lorem Lorem = new Lorem(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

		public FlatFileFixture(int lineCount, int columnCount, Encoding encoding, string header = null,
			string separator = null, bool persistent = false) : this(
			lineCount, () => columnCount, encoding, header, separator, persistent)
		{
		}

		public FlatFileFixture(int lineCount, Encoding encoding, string header = null, string separator = null,
			bool persistent = false)
			: this(lineCount, Lorem.Random.Number(1, 20), encoding, header, separator, persistent)
		{
		}

		public FlatFileFixture(int lineCount, Func<int> columnCount, Encoding encoding, string header = null,
			string separator = null, bool persistent = false) : base(persistent)
		{
			encoding ??= Encoding.UTF8;
			var separated = !string.IsNullOrWhiteSpace(separator);

			byte[] buffer;
			if (!string.IsNullOrWhiteSpace(header))
			{
				buffer = encoding.GetBytes(header + Environment.NewLine);
				FileStream.Write(buffer, 0, buffer.Length);
			}

			if (separated)
			{
				for (var i = 0; i < lineCount; i++)
				{
					buffer = encoding.GetBytes(Pooling.StringBuilderPool.Scoped(sb =>
					{
						var words = Lorem.Words(columnCount());
						for (var j = 0; j < words.Length; j++)
						{
							sb.Append(words[j]);
							if (j < words.Length - 1)
								sb.Append(separator);
						}
					}) + Environment.NewLine);
					FileStream.Write(buffer, 0, buffer.Length);
				}
			}
			else
			{
				for (var i = 0; i < lineCount; i++)
				{
					buffer = encoding.GetBytes(Lorem.Sentence() + Environment.NewLine);
					FileStream.Write(buffer, 0, buffer.Length);
				}
			}

			FileStream.Flush();
			if (persistent)
				FileStream.Close();
			else
				FileStream.Seek(0, SeekOrigin.Begin);
		}
	}
}