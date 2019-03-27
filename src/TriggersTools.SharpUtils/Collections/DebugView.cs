using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TriggersTools.SharpUtils.Collections {
	//
	// VS IDE can't differentiate between types with the same name from different
	// assembly. So we need to use different names for collection debug view for 
	// collections in mscorlib.dll and system.dll.
	//
	internal sealed class TriggersTools_CollectionDebugView<T> {
		private ICollection<T> collection;

		public TriggersTools_CollectionDebugView(ICollection<T> collection) {
			this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public T[] Items {
			get {
				T[] items = new T[collection.Count];
				collection.CopyTo(items, 0);
				return items;
			}
		}
	}
	internal sealed class TriggersTools_DictionaryDebugView<K, V> {
		private IDictionary<K, V> dictionary;

		public TriggersTools_DictionaryDebugView(IDictionary<K, V> dictionary) {
			this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public KeyValuePair<K, V>[] Items {
			get {
				KeyValuePair<K, V>[] items = new KeyValuePair<K, V>[dictionary.Count];
				dictionary.CopyTo(items, 0);
				return items;
			}
		}
	}
}
