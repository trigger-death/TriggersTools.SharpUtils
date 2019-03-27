using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  A collection of disposable objects that can be cleaned up in one using statement.
	/// </summary>
	/// <typeparam name="T">The type of disposable object contained in the collection.</typeparam>
	/// 
	/// <remarks>
	///  Calling the <see cref="Collection{T}.Clear()"/>, <see cref="Collection{T}.Remove(T)"/>, or
	///  <see cref="Collection{T}.RemoveAt(int)"/> methods will dispose of removed items if
	///  <see cref="DisposeOnRemove"/> is true.
	///  <para/>
	///  Use <see cref="DisposableCollection{T}.Clear(bool)"/>, <see cref="DisposableCollection{T}.Remove(T, bool)"/>,
	///  or <see cref="DisposableCollection{T}.RemoveAt(int, bool)"/> to choose whether items are disposed of.
	///  <para/>
	///  Replacing an item in the collection with the indexer will NOT dispose of the item.
	///  <para/>
	///  Values are always disposed of before removal, so if an error occurrs during disposal, the item is not removed.
	/// </remarks>
	public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable {
		#region Properties

		/// <summary>
		///  Gets or sets if elements should be disposed of on removal by default. This is true by default.
		/// </summary>
		public bool DisposeOnRemove { get; set; } = true;

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an empty collection of disposable type <typeparamref name="T"/>.
		/// </summary>
		public DisposableCollection() { }
		/// <summary>
		///  Constructs a wrapper for the list of disposable type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="list">The list to wrap the collection around.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="list"/> is null.
		/// </exception>
		public DisposableCollection(IList<T> list) : base(list) { }

		#endregion

		#region Collection Overrides

		/// <summary>
		///  Removes all elements from the <see cref="DisposableCollection{T}"/> and disposes of them.
		/// </summary>
		protected override void ClearItems() {
			Clear(DisposeOnRemove);
			//Dispose();
			//base.ClearItems();
		}
		/// <summary>
		///  Removes the element from the <see cref="DisposableCollection{T}"/> and disposes of it.
		/// </summary>
		/// <param name="index">The index of the element to remove.</param>
		protected override void RemoveItem(int index) {
			RemoveAt(index, DisposeOnRemove);
			//T item = this[index];
			//item?.Dispose();
			//base.RemoveItem(index);
		}

		#endregion

		#region Clear/Remove (Optional Dispose)

		/// <summary>
		///  Removes all elements from the <see cref="DisposableCollection{T}"/> and optionally disposes of them.
		/// </summary>
		/// <param name="dispose">True if all elements should be disposed of.</param>
		public void Clear(bool dispose) {
			if (dispose)
				Dispose();
			base.ClearItems();
		}
		/// <summary>
		///  Removes the element from the <see cref="DisposableCollection{T}"/> and optionally disposes of it.
		/// </summary>
		/// <param name="item">The element to remove from the collection.</param>
		/// <param name="dispose">True if the element should be disposed of if it exists in the collection.</param>
		/// <returns>True if the element was removed from the collection and optionally disposed of.</returns>
		public bool Remove(T item, bool dispose) {
			int index = Items.IndexOf(item);
			if (index < 0) return false;
			if (dispose)
				item?.Dispose();
			base.RemoveItem(index);
			return true;
		}
		/// <summary>
		///  Removes the element at the specified index in the <see cref="DisposableCollection{T}"/> and optionally
		///  disposes of it.
		/// </summary>
		/// <param name="index">The index of the element to remove from the collection.</param>
		/// <param name="dispose">True if the element should be disposed of.</param>
		public void RemoveAt(int index, bool dispose) {
			if (dispose)
				Items[index]?.Dispose();
			base.RemoveItem(index);
		}

		#endregion

		#region IDisposable Implementation

		/// <summary>
		///  Disposes of all the items in the collection.
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}
		/// <summary>
		///  An overrideable method to change how items in the collection are disposed of.
		/// </summary>
		/// <param name="disposing">True if disposing is going on.</param>
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				foreach (T item in Items) {
					item?.Dispose();
				}
			}
		}

		#endregion
	}
}
