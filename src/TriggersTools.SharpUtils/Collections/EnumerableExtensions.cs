using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.SharpUtils.Collections {
	public static class EnumerableExtensions {
		public static int IndexOf<T>(this IEnumerable<T> source, T value) {
			int index = 0;
			var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
			foreach (T item in source) {
				if (comparer.Equals(item, value)) return index;
				index++;
			}
			return -1;
		}
		public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> match) {
			int index = 0;
			foreach (T item in source) {
				if (match(item)) return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		///  Construct a immutable array from an enumerable source. This uses a lightweight immutable array
		///  implementation that doesn't require referencing System.Collections.Immutable.
		/// </summary>
		/// <typeparam name="T">The element type of the source.</typeparam>
		/// <param name="source">The source collection to construct the immutable array from.</param>
		/// <returns>An immutable array with the source's elements.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="source"/> is null.
		/// </exception>
		public static ImmutableArrayLW<T> ToImmutableArrayLW<T>(this IEnumerable<T> source) {
			return new ImmutableArrayLW<T>(source);
		}
		public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> source) {
			return new ReadOnlyCollection<T>(source);
		}
		public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> source)
		{
			return new ReadOnlyDictionary<TKey, TValue>(source);
		}
	}
}
