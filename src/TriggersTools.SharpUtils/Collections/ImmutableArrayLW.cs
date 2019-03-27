using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  A lightweight immutable array so that including System.Collections.Immutable is not required.
	/// </summary>
	/// <typeparam name="T">The element type of the array.</typeparam>
	[Serializable]
	[DebuggerDisplay("Length = {Count}")]
	[DebuggerTypeProxy(typeof(TriggersTools_CollectionDebugView<>))]
	public class ImmutableArrayLW<T> : IReadOnlyList<T>, IList<T>, IList {
		#region Fields

		/// <summary>
		///  The wrapped array of items contained in the immutable array.
		/// </summary>
		private readonly T[] array;

		#endregion

		#region Constructors

		public ImmutableArrayLW(IEnumerable<T> source) {
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (source is ICollection<T> collection) {
				array = new T[collection.Count];
				collection.CopyTo(array, 0);
			}
			else if (source is IReadOnlyList<T> readOnlyList) {
				array = new T[readOnlyList.Count];
				for (int i = 0; i < array.Length; i++)
					array[i] = readOnlyList[i];
			}
			else if (source is IReadOnlyCollection<T> readOnlyCollection) {
				array = new T[readOnlyCollection.Count];
				int i = 0;
				foreach (T item in readOnlyCollection) {
					array[i] = item;
					i++;
				}
			}
			else {
				array = source.ToArray();
			}
		}

		#endregion

		#region Properties

		public int Count => array.Length;
		bool IList.IsFixedSize => true;
		bool IList.IsReadOnly => true;
		bool ICollection<T>.IsReadOnly => true;
		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot => array.SyncRoot;

		#endregion
		
		#region IList Implementation (Accessors)

		public T this[int index] {
			get => array[index];
			set => ThrowImmutable();
		}
		object IList.this[int index] {
			get => array[index];
			set => ThrowImmutable();
		}

		public bool Contains(T item) => ((ICollection<T>) array).Contains(item);
		bool IList.Contains(object value) => ((IList) array).Contains(value);

		public void CopyTo(T[] array, int arrayIndex) => ((ICollection<T>) array).CopyTo(array, arrayIndex);
		void ICollection.CopyTo(Array array, int index) => array.CopyTo(array, index);

		public int IndexOf(T item) => ((IList<T>) array).IndexOf(item);
		int IList.IndexOf(object value) => ((IList) array).IndexOf(value);

		public T[] ToArray() {
			T[] newArray = new T[array.Length];
			Array.Copy(array, newArray, array.Length);
			return newArray;
		}

		public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>) array).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

		#endregion

		#region IList Implementation (Not Supported)

		void ICollection<T>.Add(T item) => ThrowImmutable();
		int IList.Add(object value) {
			ThrowImmutable();
			return 0;
		}

		void ICollection<T>.Clear() => ThrowImmutable();
		void IList.Clear() => ThrowImmutable();

		void IList<T>.Insert(int index, T item) => ThrowImmutable();
		void IList.Insert(int index, object value) => ThrowImmutable();

		void IList<T>.RemoveAt(int index) => ThrowImmutable();
		void IList.RemoveAt(int index) => ThrowImmutable();

		bool ICollection<T>.Remove(T item) {
			ThrowImmutable();
			return false;
		}
		void IList.Remove(object value) => ThrowImmutable();

		#endregion

		#region Private Helpers

		/// <summary>
		///  Throws a <see cref="NotSupportedException"/> within mutator methods.
		/// </summary>
		/// 
		/// <exception cref="NotSupportedException">
		///  A mutator method is being called.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowImmutable() {
			throw new NotSupportedException("Collection is immutable!");
		}

		#endregion
	}
}
