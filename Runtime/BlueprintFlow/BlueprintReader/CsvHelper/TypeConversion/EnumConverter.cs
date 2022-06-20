// Copyright 2009-2021 Josh Close
// This file is a part of CsvHelper and is dual licensed under MS-PL and Apache 2.0.
// See LICENSE.txt for details or visit http://www.opensource.org/licenses/ms-pl.html for MS-PL and http://opensource.org/licenses/Apache-2.0 for Apache 2.0.
// https://github.com/JoshClose/CsvHelper

namespace GameFoundation.BlueprintFlow.BlueprintReader.CsvHelper.TypeConversion
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Converts an <see cref="Enum"/> to and from a <see cref="string"/>.
	/// </summary>
	public class EnumConverter : DefaultTypeConverter
	{
		private readonly Dictionary<string, string> enumNamesByAttributeNames = new Dictionary<string, string>();
		private readonly Dictionary<string, string> enumNamesByAttributeNamesIgnoreCase = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<object, string> attributeNamesByEnumValues = new Dictionary<object, string>();

		// enumNamesByAttributeNames
		// enumNamesByAttributeNamesIgnoreCase
		// [Name("Foo")]:One

		// attributeNamesByEnumValues
		// 1:[Name("Foo")]
		

		/// <inheritdoc/>
		public override object ConvertFromString(string text, Type typeInfo)
		{
			if (text != null)
			{
				if (this.enumNamesByAttributeNames.ContainsKey(text))
				{
					return Enum.Parse(typeInfo, this.enumNamesByAttributeNames[text]);
				}
			}

#if NET45 || NET47 || NETSTANDARD2_0
			try
			{
				return Enum.Parse(type, text, ignoreCase);
			}
			catch
			{
				return base.ConvertFromString(text, row, memberMapData);
			}
#else
			if (Enum.TryParse(typeInfo, text, false, out var value))
			{
				return value;
			}
			else
			{
				return base.ConvertFromString(text, typeInfo);
			}
#endif
		}

		/// <inheritdoc/>
		public override string ConvertToString(object value, Type typeInfo)
		{
			if (value != null && this.attributeNamesByEnumValues.ContainsKey(value))
			{
				return this.attributeNamesByEnumValues[value];
			}

            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }
	}
}
