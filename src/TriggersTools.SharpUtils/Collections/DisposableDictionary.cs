using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  A dictionary of disposable objects that can be cleaned up in one using statement.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of disposable object contained in the collection.</typeparam>
	/// 
	/// <remarks>
	///  Calling the <see cref="DictionaryCollection{TKey, TValue}.Clear()"/>,
	///  <see cref="DictionaryCollection{TKey, TValue}.Remove(TKey)"/>, or <see cref="ICollection{T}.Remove(T)"/>
	///  methods will dispose of removed items if <see cref="DisposeOnRemove"/> is true.
	///  <para/>
	///  Use <see cref="DisposableDictionary{TKey, TValue}.Clear(bool)"/> or
	///  <see cref="DisposableDictionary{TKey, TValue}.Remove(TKey, bool)"/> to choose whether items are disposed of.
	///  <para/>
	///  Replacing an item in the dictionary with the indexer will NOT dispose of the item.
	///  <para/>
	///  Values are always disposed of before removal, so if an error occurrs during disposal, the item is not removed.
	/// </remarks>
	public class DisposableDictionary<TKey, TValue> : DictionaryCollection<TKey, TValue>, IDisposable
		where TValue : IDisposable
	{
		#region Properties

		/// <summary>
		///  Gets or sets if elements should be disposed of on removal by default. This is true by default.
		/// </summary>
		public bool DisposeOnRemove { get; set; } = true;

		#endregion

		#region Constructors
		
		/// <summary>
		///  Constructs an empty dictionary of disposable type <typeparamref name="TValue"/>.
		/// </summary>
		public DisposableDictionary() { }
		/// <summary>
		///  Constructs a wrapper for the dictionary of disposable type <typeparamref name="TValue"/>.
		/// </summary>
		/// <param name="dictionary">The dictionary to wrap the dictionary collection around.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="dictionary"/> is null.
		/// </exception>
		public DisposableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

		#endregion

		#region DictionaryCollection Overrides

		protected override void ClearItems() {
			Clear(DisposeOnRemove);
		}
		protected override void RemoveItem(TKey key, TValue value) {
			if (DisposeOnRemove)
				value?.Dispose();
			base.RemoveItem(key, value);
		}

		#endregion

		public void Clear(bool dispose) {
			if (dispose)
				Dispose();
			base.ClearItems();
		}
		public bool Remove(TKey key, bool dispose) {
			if (TryGetValue(key, out TValue value)) {
				if (dispose)
					value?.Dispose();
				base.RemoveItem(key, value);
				return true;
			}
			return false;
		}
		public bool Remove(TKey key, TValue value, bool dispose) {
			var pair = new KeyValuePair<TKey, TValue>(key, value);
			if (Items.Contains(pair)) {
				if (dispose)
					value?.Dispose();
				base.RemoveItem(key, value);
				return true;
			}
			return false;
		}

		#region IDisposable Implementation

		/// <summary>
		///  Disposes of all the items in the dictionary.
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}
		/// <summary>
		///  An overrideable method to change how items in the dictionary are disposed of.
		/// </summary>
		/// <param name="disposing">True if disposing is going on.</param>
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				foreach (TValue value in Items.Values) {
					value?.Dispose();
				}
			}
		}

		#endregion
	}
}
