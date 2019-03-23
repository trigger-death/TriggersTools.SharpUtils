using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.SharpUtils.Collections {
	public class ImmutableDictionaryLW<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> {
		#region Fields

		private readonly Dictionary<TKey, TValue> dictionary;

		#endregion

		#region Constructors

		public ImmutableDictionaryLW(IEnumerable<KeyValuePair<TKey, TValue>> source) {
			dictionary = new Dictionary<TKey, TValue>(source);
		}

		#endregion
	}
}
