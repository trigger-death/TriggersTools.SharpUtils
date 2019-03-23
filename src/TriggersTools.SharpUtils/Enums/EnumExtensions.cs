using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.SharpUtils.Enums {
	/// <summary>
	///  Extension methods for <see cref="Enum"/> types.
	/// </summary>
	public static class EnumExtensions {

		public static string ToDescription<TEnum>(this TEnum value) where TEnum : Enum {
			if (EnumInfo.Get<TEnum>().TryGetAttribute(value, out DescriptionAttribute attribute))
				return attribute.Description;
			return null;
		}
		public static string ToDescription(this Enum value) {
			if (EnumInfo.Get(value.GetType()).TryGetAttribute(value, out DescriptionAttribute attribute))
				return attribute.Description;
			return null;
		}
		public static string ToCategory<TEnum>(this TEnum value) where TEnum : Enum {
			if (EnumInfo.Get<TEnum>().TryGetAttribute(value, out CategoryAttribute attribute))
				return attribute.Category;
			return null;
		}
		public static string ToCategory(this Enum value) {
			if (EnumInfo.Get(value.GetType()).TryGetAttribute(value, out CategoryAttribute attribute))
				return attribute.Category;
			return null;
		}
	}
}
