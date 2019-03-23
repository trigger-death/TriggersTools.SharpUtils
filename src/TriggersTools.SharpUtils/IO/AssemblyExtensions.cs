using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace TriggersTools.SharpUtils.IO {
	/// <summary>
	///  Extension methods for the <see cref="Assembly"/> class.
	/// </summary>
	public static class AssemblyExtensions {
		#region FilePath

		/// <summary>
		///  Gets the file name of the assembly.
		/// </summary>
		/// <param name="assembly">The assembly to get the location from.</param>
		/// <returns>The file name of the assembly.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="assembly"/> is null.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The current assembly is a dynamic assembly, represented by an <see cref="AssemblyBuilder"/> object.
		/// </exception>
		public static string GetFileName(this Assembly assembly) {
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));
			return Path.GetFileName(assembly.Location);
		}
		/// <summary>
		///  Gets the parent directory of the assembly location.
		/// </summary>
		/// <param name="assembly">The assembly to get the location from.</param>
		/// <returns>The parent directory of the assembly location.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="assembly"/> is null.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The current assembly is a dynamic assembly, represented by an <see cref="AssemblyBuilder"/> object.
		/// </exception>
		public static string GetDirectory(this Assembly assembly) {
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));
			return Path.GetDirectoryName(assembly.Location);
		}

		#endregion
	}
}
