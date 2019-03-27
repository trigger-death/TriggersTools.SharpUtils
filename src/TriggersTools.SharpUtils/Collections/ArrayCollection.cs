using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  A fixed-size implementation of <see cref="Collection{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	[Serializable]
	[DebuggerDisplay("Length = {Count}")]
	[DebuggerTypeProxy(typeof(TriggersTools_CollectionDebugView<>))]
	public class ArrayCollection<T> : IList<T>, IList, IReadOnlyList<T> {
		#region Fields

		/// <summary>
		///  The internal array wrapped by the <see cref="ArrayCollection{T}"/>.
		/// </summary>
		protected T[] Items { get; }

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs a fixed-length <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <param name="length">The fixed length of the array.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="length"/> is less than zero.
		/// </exception>
		public ArrayCollection(int length) {
			if (length < 0)
				throw new ArgumentOutOfRangeException(nameof(length));
			Items = new T[length];
		}
		/// <summary>
		///  Constructs a wrapper for the array with <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <param name="array">The array to wrap the <see cref="ArrayCollection{T}"/> around.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="array"/> is null.
		/// </exception>
		public ArrayCollection(T[] array) {
			// Behave just like Collection<T> where we act as a wrapper for the collection.
			Items = array ?? throw new ArgumentNullException(nameof(array));
		}

		#endregion

		#region IList Properties

		/// <summary>
		///  Gets the number of elements contained within the <see cref="ArrayCollection{T}"/>.
		/// </summary>
		public int Count => Items.Length;

		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot => Items.SyncRoot;
		bool ICollection<T>.IsReadOnly => false;
		bool IList.IsReadOnly => false;
		bool IList.IsFixedSize => true;

		#endregion

		#region Virtual Methods

		/// <summary>
		///  Replaces the element at the specified index in the <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">
		///  The new value for the element at the specified index. The value can be null for reference types.
		/// </param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="index"/> is less than zero.-or- <paramref name="index"/> is greater than
		///  <see cref="ArrayCollection{T}.Count"/>.
		/// </exception>
		protected virtual void SetItem(int index, T item) => Items[index] = item;

		#endregion

		#region IList Implementation (Accessor)

		/// <summary>
		///  Determines whether an element is in the <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <param name="item">
		///  The object to locate in the <see cref="ArrayCollection{T}"/>. The value can be null for reference types.
		/// </param>
		/// <returns>True if item is found in the <see cref="ArrayCollection{T}"/>; otherwise, false.</returns>
		public bool Contains(T item) => ((ICollection<T>) Items).Contains(item);
		bool IList.Contains(object value) => ((IList) Items).Contains(value);

		/// <summary>
		///  Copies the entire <see cref="ArrayCollection{T}"/> to a compatible one-dimensional <see cref="Array"/>,
		///  starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">
		///  The one-dimensional <see cref="Array"/> that is the destination of the elements copied from
		///  <see cref="ArrayCollection{T}"/>. The System.Array must have zero-based indexing.
		/// </param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="array"/> is null.
		/// </exception>
		public void CopyTo(T[] array, int index) => ((ICollection<T>) Items).CopyTo(array, index);
		void ICollection.CopyTo(Array array, int index) => Items.CopyTo(array, index);

		/// <summary>
		///  Searches for the specified object and returns the zero-based index of the first occurrence within the
		///  entire <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <param name="item">
		///  The object to locate in the <see cref="ArrayCollection{T}"/>. The value can be null for reference types.
		/// </param>
		/// <returns>
		///  The zero-based index of the first occurrence of item within the entire <see cref="ArrayCollection{T}"/>,
		///  if found; otherwise, -1.
		/// </returns>
		public int IndexOf(T item) => ((ICollection<T>) Items).IndexOf(item);
		int IList.IndexOf(object value) => ((IList) Items).IndexOf(value);

		/// <summary>
		///  Copies the elements in the <see cref="ArrayCollection{T}"/> to a new array and returns it.
		/// </summary>
		/// <returns>A new array containing a copy of the elements in the <see cref="ArrayCollection{T}"/>.</returns>
		public T[] ToArray() {
			T[] array = new T[Items.Length];
			Array.Copy(Items, array, Items.Length);
			return array;
		}

		/// <summary>
		///  Returns an enumerator that iterates through the <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <returns>An <see cref="IEnumerator{T}"/> for the <see cref="ArrayCollection{T}"/>.</returns>
		public IEnumerator<T> GetEnumerator() => Items.Cast<T>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

		#endregion

		#region IList Implementation (Mutator Supported)

		/// <summary>
		///  Gets or sets the element at the specified index in the <see cref="ArrayCollection{T}"/>.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="index"/> is less than zero.-or- <paramref name="index"/> is equal to or greater than
		///  <see cref="ArrayCollection{T}.Count"/>.
		/// </exception>
		public T this[int index] {
			get => Items[index];
			set {
				if (index < 0 || index >= Items.Length)
					throw new ArgumentOutOfRangeException(nameof(index));
				SetItem(index, value);
			}
		}
		object IList.this[int index] {
			get => Items[index];
			set => this[index] = (T) value;
		}

		#endregion

		#region IList Implementation (Not Supported)

		int IList.Add(object value) {
			ThrowFixedSize();
			return 0;
		}
		void ICollection<T>.Add(T item) => ThrowFixedSize();
		void IList.Clear() => ThrowFixedSize();
		void ICollection<T>.Clear() => ThrowFixedSize();
		void IList.Insert(int index, object value) => ThrowFixedSize();
		void IList<T>.Insert(int index, T item) => ThrowFixedSize();
		void IList.Remove(object value) => ThrowFixedSize();
		bool ICollection<T>.Remove(T item) {
			ThrowFixedSize();
			return false;
		}
		void IList.RemoveAt(int index) => ThrowFixedSize();
		void IList<T>.RemoveAt(int index) => ThrowFixedSize();

		#endregion

		#region Private Methods

		/// <summary>
		///  Throws a <see cref="NotSupportedException"/> within non indexer mutator methods.
		/// </summary>
		/// 
		/// <exception cref="NotSupportedException">
		///  A non indexer mutator method is being called.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ThrowFixedSize() {
			throw new NotSupportedException("Collection is fixed-size!");
		}

		#endregion
	}
}