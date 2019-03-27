using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  A dictionary implementation of <see cref="Collection{T}"/> that can be inherited and extended.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	[Serializable]
	[DebuggerDisplay("Length = {Count}")]
	[DebuggerTypeProxy(typeof(TriggersTools_DictionaryDebugView<,>))]
	public class DictionaryCollection<TKey, TValue>
		: IDictionary<TKey, TValue>, ICollection, IReadOnlyDictionary<TKey, TValue>
	{
		#region Fields

		/// <summary>
		///  The internal dictionary wrapped by the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		protected IDictionary<TKey, TValue> Items { get; }
		/// <summary>
		///  The synchronization root object.
		/// </summary>
		[NonSerialized]
		private object syncRoot;

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an empty <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		public DictionaryCollection() {
			Items = new Dictionary<TKey, TValue>();
		}
		/// <summary>
		///  Constructs a wrapper for the dictionary with <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <param name="dictionary">
		///  The dictionary to wrap the <see cref="DictionaryCollection{TKey, TValue}"/> around.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="dictionary"/> is null.
		/// </exception>
		public DictionaryCollection(IDictionary<TKey, TValue> dictionary) {
			// Behave just like Collection<T> where we act as a wrapper for the collection.
			Items = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
		}

		#endregion

		#region Properties

		/// <summary>
		///  Gets the number of elements contained within the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		public int Count => Items.Count;
		/// <summary>
		///  Gets an <see cref="ICollection{T}"/> containing the keys of the
		///  <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		public ICollection<TKey> Keys => Items.Keys;
		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Items.Keys;
		/// <summary>
		///  Gets an <see cref="ICollection{T}"/> containing the values in the
		///  <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		public ICollection<TValue> Values => Items.Values;
		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Items.Values;

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => Items.IsReadOnly;
		bool ICollection.IsSynchronized => false;
		object ICollection.SyncRoot {
			get {
				if (syncRoot == null) {
					if (Items is ICollection c)
						return c.SyncRoot;
					else
						return Interlocked.CompareExchange(ref syncRoot, new object(), null);
				}
				return syncRoot;
			}
		}

		#endregion

		#region IDictionary Implementation (Accessor)

		/// <summary>
		///  Determines whether the <see cref="DictionaryCollection{TKey, TValue}"/> contains the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="DictionaryCollection{TKey, TValue}"/>.</param>
		/// <returns>
		///  True if the System.Collections.Generic.Dictionary`2 contains an element with the specified key; otherwise,
		///  false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool ContainsKey(TKey key) => Items.ContainsKey(key);
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
			return Items.Contains(item);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			Items.CopyTo(array, arrayIndex);
		}
		void ICollection.CopyTo(Array array, int index) {
			if (Items is ICollection c)
				c.CopyTo(array, index);

			if (array == null)
				throw new ArgumentNullException(nameof(array));

			if (array.Rank != 1)
				throw new ArgumentException("Multidimensional arrays are not supported!");

			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException("Lower bound of array is less than zero!");

			if (array.Length - index < Count)
				throw new ArgumentException("Array plus offset is to small to copy to!");

			if (array is KeyValuePair<TKey, TValue>[] tArray) {
				Items.CopyTo(tArray, index);
			}
			else {
				//
				// Catch the obvious case assignment will fail.
				// We can found all possible problems by doing the check though.
				// For example, if the element type of the Array is derived from T,
				// we can't figure out if we can successfully copy the element beforehand.
				//
				Type targetType = array.GetType().GetElementType();
				Type sourceType = typeof(KeyValuePair<TKey, TValue>);
				if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
					throw new ArgumentException("Invalid array type!");

				//
				// We can't cast array of value type to object[], so we don't support 
				// widening of primitive types here.
				//
				object[] objects = array as object[];
				if (objects == null)
					throw new ArgumentException("Invalid array type!");

				int count = Items.Count;
				try {
					foreach (KeyValuePair<TKey, TValue> pair in Items) {
						objects[index++] = pair;
					}
				} catch (ArrayTypeMismatchException) {
					throw new ArgumentException("Invalid array type!");
				}
			}
		}
		
		/// <summary>
		///  Gets the value associated with the specified key in the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <param name="value">
		///  When this method returns, the value associated with the specified key, if the key is found; otherwise, the
		///  default value for the type of the value parameter. This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		///  True if the <see cref="DictionaryCollection{TKey, TValue}"/> contains an element with the specified key;
		///  otherwise, false.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool TryGetValue(TKey key, out TValue value) => Items.TryGetValue(key, out value);

		/// <summary>
		///  Returns an enumerator that iterates through the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <returns>
		///  An <see cref="IDictionaryEnumerator"/> for the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

		#endregion

		#region IDictionary Implementation (Mutator)

		/// <summary>
		///  Removes all keys and values from the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// 
		/// <exception cref="NotSupportedException">
		///  The <see cref="DictionaryCollection{TKey, TValue}"/> is read-only.
		/// </exception>
		public void Clear() {
			ThrowIfReadOnly();
			ClearItems();
		}

		/// <summary>
		///  Adds the specified key and value to the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <param name="key"> The key of the element to add.</param>
		/// <param name="value">The value of the element to add. The value can be null for reference types.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  An element with the same key already exists in the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The <see cref="DictionaryCollection{TKey, TValue}"/> is read-only.
		/// </exception>
		public void Add(TKey key, TValue value) {
			ThrowIfReadOnly();
			if (Items.ContainsKey(key))
				throw new ArgumentException("The given key already exists in the dictionary!");
			AddItem(key, value);
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
			Add(item.Key, item.Value);
		}

		/// <summary>
		///  Removes the value with the specified key from the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <returns>
		///  True if the element is successfully found and removed; otherwise, false. This method returns false if key
		///  is not found in the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool Remove(TKey key) {
			ThrowIfReadOnly();
			if (Items.TryGetValue(key, out TValue value)) {
				RemoveItem(key, value);
				return true;
			}
			return false;
		}
		/// <summary>
		///  Removes the pair with the specified key and value from the
		///  <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <param name="value">The value that must be present for the given key.</param>
		/// <returns>
		///  True if the element is successfully found and removed; otherwise, false. This method returns false if
		///  <paramref name="key"/> or <paramref name="value"/> are not found in the
		///  <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool Remove(TKey key, TValue value) {
			ThrowIfReadOnly();
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			if (Items.Contains(pair)) {
				RemoveItem(key, value);
				return true;
			}
			return false;
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			return Remove(item.Key, item.Value);
		}

		/// <summary>
		///  Gets or sets the element with the specified key in the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		/// <param name="key">The key of the element to get or set.</param>
		/// <returns>The element with the specified key.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  The property is retrieved and <paramref name="key"/> is not found.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The property is set and the <see cref="DictionaryCollection{TKey, TValue}"/> is read-only.
		/// </exception>
		public TValue this[TKey key] {
			get => Items[key];
			set {
				ThrowIfReadOnly();
				if (Items.TryGetValue(key, out TValue oldValue))
					SetItem(key, value, oldValue);
				else
					AddItem(key, value);
			}
		}

		#endregion
		
		#region Virtual Methods

		/// <summary>
		///  Removes all elements from the <see cref="DictionaryCollection{TKey, TValue}"/>.
		/// </summary>
		protected virtual void ClearItems() => Items.Clear();
		/// <summary>
		///  Adds an element to the <see cref="DictionaryCollection{TKey, TValue}"/>. This method is only called if
		///  <paramref name="key"/> does not exist.
		/// </summary>
		/// <param name="key">The object to use as the key of the element to add.</param>
		/// <param name="value">The object to use as the value of the element to add.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  An element with the same key already exists in the <see cref="IDictionary{TKey, TValue}"/>.
		/// </exception>
		protected virtual void AddItem(TKey key, TValue value) => Items.Add(key, value);
		/// <summary>
		///  Removes the key value pair from the <see cref="DictionaryCollection{TKey, TValue}"/>. This method is only
		///  called if <paramref name="key"/> exists.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <param name="value">The value of the element to remove.</param>
		/// <returns>
		///  True if the element is successfully found and removed; otherwise, false. This method returns false if key
		///  is not found in the <see cref="IDictionary{TKey, TValue}"/>.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		protected virtual void RemoveItem(TKey key, TValue value) {
			Items.Remove(new KeyValuePair<TKey, TValue>(key, value));
		}
		/// <summary>
		///  Gets or sets the element with the specified key in the <see cref="DictionaryCollection{TKey, TValue}"/>.
		///  This method is only called if <paramref name="key"/> exists.
		/// </summary>
		/// <param name="key">The key of the element to get or set.</param>
		/// <param name="value">The element with the specified key to set.</param>
		/// <param name="oldValue">The old value being removed.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		protected virtual void SetItem(TKey key, TValue value, TValue oldValue) => Items[key] = value;

		#endregion

		#region Private Helpers

		/// <summary>
		///  Throws a <see cref="NotSupportedException"/> if <see cref="ICollection{T}.IsReadOnly"/> is true.
		/// </summary>
		/// 
		/// <exception cref="NotSupportedException">
		///  <see cref="ICollection{T}.IsReadOnly"/> is true.
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfReadOnly() {
			if (Items.IsReadOnly)
				throw new NotSupportedException("Dictionary is read-only!");
		}

		#endregion
	}
}
