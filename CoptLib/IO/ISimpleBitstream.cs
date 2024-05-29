using CommunityToolkit.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace CoptLib.IO;

public interface ISimpleBitstream : IEnumerable<bool>;

/// <summary>
/// A <see cref="ISimpleBitstream"/> that wraps a plain <see cref="IEnumerable{bool}"/>.
/// </summary>
public class EnumerableSimpleBitstream(IEnumerable<bool> bits) : ISimpleBitstream
{
    private readonly IEnumerable<bool> _bits = bits;

    public IEnumerator<bool> GetEnumerator() => _bits.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class SimpleBitstream32 : ISimpleBitstream
{
    /// <summary>
    /// Constructs a new <see cref="SimpleBitstream32"/> from a 32-bit unsigned integer,
    /// where the least significant bits are returned from the stream first.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length">The number of bits in the stream.</param>
    public SimpleBitstream32(uint value, byte length = sizeof(uint))
    {
        Guard.IsInRange(length, (byte)0, (byte)sizeof(uint));

        Value = value;
        Length = length;
    }

    public uint Value { get; }

    public byte Length { get; }

    public IEnumerator<bool> GetEnumerator()
    {
        for (int i = 0; i < Length; i++)
            yield return ((Value >> i) & 1) == 1;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class SimpleBitstreamExtensions
{
    public static BigInteger ToNumber(this ISimpleBitstream bitstream)
    {
        BigInteger result = new();

        foreach (var bit in bitstream)
        {
            result |= (bit ? 1 : 0);
            result <<= 1;
        }

        return result;
    }
}