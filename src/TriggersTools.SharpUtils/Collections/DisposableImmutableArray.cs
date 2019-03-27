using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.SharpUtils.Collections {
	/// <summary>
	///  An immutable array of disposable objects that can be cleaned up in one using statement.
	/// </summary>
	/// <typeparam name="T">The type of disposable object contained in the collection.</typeparam>
	public class DisposableImmutableArray<T> : ImmutableArrayLW<T>, IDisposable where T : IDisposable {
		#region Constructors

		/// <summary>
		///  Constructs an immutable array of disposable type <typeparamref name="T"/>.
		/// </summary>
		/// <param name="source">The collection to populate the immutable array with.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="source"/> is null.
		/// </exception>
		public DisposableImmutableArray(IEnumerable<T> source) : base(source) { }

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
				foreach (T item in this) {
					item?.Dispose();
				}
			}
		}

		#endregion
	}
}
