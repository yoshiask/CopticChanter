using System;
using System.Buffers;
using System.IO;
using System.Text;

namespace CoptLib.Hyperspeed.Memory;

public class StreamBufferWriter : IBufferWriter<byte>
{
    private readonly Stream _stream;
    private readonly ArrayBufferWriter<byte> _writer;
    
    /// <summary>
    /// Creates an instance of an <see cref="StreamBufferWriter"/>, in which data can be written to,
    /// with the default initial capacity.
    /// </summary>
    public StreamBufferWriter(Stream stream)
    {
        _stream = stream;
        _writer = new();
    }

    /// <summary>
    /// Creates an instance of an <see cref="StreamBufferWriter"/>, in which data can be written to,
    /// with an initial capacity specified.
    /// </summary>
    /// <param name="initialCapacity">The minimum capacity with which to initialize the underlying buffer.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="initialCapacity"/> is not positive (i.e. less than or equal to 0).
    /// </exception>
    public StreamBufferWriter(Stream stream, int initialCapacity)
    {
        _stream = stream;
        _writer = new(initialCapacity);
    }
    
    public void Advance(int count)
    {
        _writer.Advance(count);

        var offset = _writer.WrittenCount - count;
        
#if NETCOREAPP3_0_OR_GREATER
        _stream.Write(_writer.WrittenSpan[offset..]);
        Console.WriteLine($"Wrote {count} bytes: {Encoding.UTF8.GetString(_writer.WrittenSpan[offset..])}");
#else
        var bytes = _writer.WrittenSpan.ToArray();
        _stream.Write(bytes, offset, count);
        Console.WriteLine($"Wrote {count} bytes: {Encoding.UTF8.GetString(bytes.AsSpan(offset).ToArray())}");
#endif
    }

    public Memory<byte> GetMemory(int sizeHint = 0) => _writer.GetMemory(sizeHint);

    public Span<byte> GetSpan(int sizeHint = 0) => _writer.GetSpan(sizeHint);
}