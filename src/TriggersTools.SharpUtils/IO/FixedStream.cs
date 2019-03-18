using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TriggersTools.SharpUtils.Exceptions;

namespace TriggersTools.IO {
	/// <summary>
	///  A stream that is fixed to the specified offset and length within another stream.
	/// </summary>
	public sealed class FixedStream : Stream {
		#region Fields
		
		/// <summary>
		///  Gets the base stream this stream is fixed to.
		/// </summary>
		public Stream BaseStream { get; }
		/// <summary>
		///  The fixed length of the stream.
		/// </summary>
		private readonly int fixedLength;
		/// <summary>
		///  The zero position of the fixed streas when it was opened.
		/// </summary>
		private readonly long zeroPosition;
		/// <summary>
		///  True if this fixed stream should leave the base stream open when closing.
		/// </summary>
		private readonly bool leaveOpen;

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs a fixed stream with a base stream and length.
		/// </summary>
		/// <param name="stream">The base stream for this fixed stream.</param>
		/// <param name="length">The length of the fixed stream.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="length"/> is less than zero.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <see cref="Stream.CanRead"/> is false.
		/// </exception>
		public FixedStream(Stream stream, int length) : this(stream, length, false) { }
		/// <summary>
		///  Constructs a fixed stream with a base stream, length and option to leave the base stream open on closing.
		/// </summary>
		/// <param name="stream">The base stream for this fixed stream.</param>
		/// <param name="length">The length of the fixed stream.</param>
		/// <param name="leaveOpen">True if the base stream should be left open when this stream closes.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="length"/> is less than zero.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <see cref="Stream.CanRead"/> is false.
		/// </exception>
		public FixedStream(Stream stream, int length, bool leaveOpen) {
			BaseStream = stream ?? throw new ArgumentNullException(nameof(stream));
			if (length < 0)
				throw ArgumentOutOfRangeUtils.OutsideMin(nameof(length), length, 0, true);
			if (!stream.CanRead)
				throw new ArgumentException($"{nameof(FixedStream)} only supports streams that can read!");
			fixedLength = length;
			zeroPosition = stream.Position;
			this.leaveOpen = leaveOpen;
		}

		#endregion

		#region Stream Override Properties

		/// <summary>
		///  Gets or sets a value, in miliseconds, that determines how long the stream will attempt to read before
		///  timing out.
		/// </summary>
		/// 
		/// <exception cref="InvalidOperationException">
		///  The <see cref="Stream.ReadTimeout"/> method always throws an <see cref="InvalidOperationException"/>.
		/// </exception>
		public override int ReadTimeout {
			get => BaseStream.ReadTimeout;
			set => BaseStream.ReadTimeout = value;
		}
		/// <summary>
		///  Gets a value that determines whether the current stream can time out.
		/// </summary>
		public override bool CanTimeout => BaseStream.CanTimeout;
		/// <summary>
		///  Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead => BaseStream.CanRead;
		/// <summary>
		///  Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek => BaseStream.CanSeek;
		/// <summary>
		///  Gets a value indicating whether the current stream supports writing. Always false.
		/// </summary>
		public override bool CanWrite => BaseStream.CanWrite;
		/// <summary>
		///  Gets the length in bytes of the stream.
		/// </summary>
		public override long Length => fixedLength;
		/// <summary>
		///  Gets or sets the position within the current stream.
		/// </summary>
		/// 
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  Methods were called after the stream was closed.
		/// </exception>
		public override long Position {
			get => BaseStream.Position - zeroPosition;
			set {
				if (value < 0)
					throw ArgumentOutOfRangeUtils.OutsideMin(nameof(Position), value, 0, true);
				BaseStream.Position = value + zeroPosition;
			}
		}

		#endregion

		#region Stream Override Methods

		/// <summary>
		///  Releases all resources used by the <see cref="FixedStream"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (!leaveOpen)
					BaseStream.Dispose();
			}
		}
		/// <summary>
		///  Clears all buffers for the base stream and causes any buffered data to be written to the underlying
		///  device.
		/// </summary>
		/// 
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		public override void Flush() {
			BaseStream.Flush();
		}
		/// <summary>
		///  Sets the position within the current stream.
		/// </summary>
		/// <param name="offset">A byte offset relative to the origin parameter.</param>
		/// <param name="origin">
		///  A value of type System.IO.SeekOrigin indicating the reference point used to obtain the new position.
		/// </param>
		/// <returns>The new position within the current stream.</returns>
		/// 
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		/// <exception cref="NotSupportedException">
		///  The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  Methods were called after the stream was closed.
		/// </exception>
		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
			case SeekOrigin.Begin:
				return Position = offset;
			case SeekOrigin.Current:
				return Position += offset;
			case SeekOrigin.End:
				return Position = Length + offset;
			}
			return Position;
		}
		/// <summary>
		///  Reads a sequence of bytes from the base stream and advances the position within the stream by the number
		///  of bytes read.
		/// </summary>
		/// <param name="buffer">
		///  An array of bytes. When this method returns, the buffer contains the specified byte array with the values
		///  between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced
		///  by the bytes read from the current source.
		/// </param>
		/// <param name="offset">
		///  The zero-based byte offset in buffer at which to begin storing the data read from the current stream.
		/// </param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>
		///  The total number of bytes read into the buffer. This can be less than the number of bytes requested if
		///  that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
		/// </returns>
		/// 
		/// <exception cref="ArgumentException">
		///  The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the
		///  <paramref name="buffer"/> length.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///  <paramref name="buffer"/> is null.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="offset"/> or <paramref name="count"/> is negative.
		/// </exception>
		/// <exception cref="IOException">
		///  An I/O error occurred.
		/// </exception>
		/// <exception cref="ObjectDisposedException">
		///  Methods were called after the stream was closed.
		/// </exception>
		public override int Read(byte[] buffer, int offset, int count) {
			count = (int) Math.Min(count, fixedLength - Position);
			if (count >= 0)
				return BaseStream.Read(buffer, offset, count);
			return 0;
		}
		/// <summary>
		///  Reads a byte from the stream and advances the position within the base stream by one byte, or returns -1
		///  if at the end of the stream.
		/// </summary>
		/// <returns>The unsigned byte cast to an <see cref="int"/>, or -1 if at the end of the stream.</returns>
		/// 
		/// <exception cref="ObjectDisposedException">
		///  Methods were called after the stream was closed.
		/// </exception>
		public override int ReadByte() {
			return BaseStream.ReadByte();
		}
		/// <summary>
		///  Not supported.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException($"{nameof(Write)} is not supported by {nameof(FixedStream)}!");
		}
		/// <summary>
		///  Not supported.
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value) {
			throw new NotSupportedException($"{nameof(SetLength)} is not supported by {nameof(FixedStream)}!");
		}

		#endregion
	}
}
