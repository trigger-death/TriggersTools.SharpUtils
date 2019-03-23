using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  A lightweight immutable array so that including System.Collections.Immutable is not required.
	/// </summary>
	/// <typeparam name="T">The element type of the array.</typeparam>
	public class ImmutableArrayLW<T> : IReadOnlyList<T>, IList<T>, IList {
		#region Fields

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


		public T this[int index] {
			get => array[index];
			set => throw new NotSupportedException();
		}
		object IList.this[int index] {
			get => array[index];
			set => throw new NotSupportedException();
		}

		public int Count => array.Length;
		public bool IsReadOnly => true;
		public bool IsFixedSize => true;
		public object SyncRoot => array.SyncRoot;
		public bool IsSynchronized => array.IsSynchronized;

		public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>) array).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();

		public bool Contains(T item) => ((IList<T>) array).Contains(item);
		bool IList.Contains(object value) => ((IList) array).Contains(value);

		public int IndexOf(T item) => ((IList<T>) array).IndexOf(item);
		int IList.IndexOf(object value) => ((IList) array).IndexOf(value);

		public void CopyTo(T[] array, int arrayIndex) => array.CopyTo(array, arrayIndex);
		public void CopyTo(Array array, int index) => array.CopyTo(array, index);

		void ICollection<T>.Add(T item) => throw new NotSupportedException();
		int IList.Add(object value) => throw new NotSupportedException();

		void ICollection<T>.Clear() => throw new NotImplementedException();
		void IList.Clear() => throw new NotImplementedException();

		void IList<T>.Insert(int index, T item) => throw new NotSupportedException();
		void IList.Insert(int index, object value) => throw new NotSupportedException();

		void IList<T>.RemoveAt(int index) => throw new NotSupportedException();
		void IList.RemoveAt(int index) => throw new NotSupportedException();

		bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
		void IList.Remove(object value) => throw new NotSupportedException();


	}
}
