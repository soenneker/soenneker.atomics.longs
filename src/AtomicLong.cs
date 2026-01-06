using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Soenneker.Atomics.Longs;

/// <summary>
/// A lightweight, allocation-free atomic <see cref="long"/> backed by <see cref="Volatile"/> and
/// <see cref="Interlocked"/> operations.
/// <para/>
/// Intended for use as a private field / inline synchronization primitive. Because this is a mutable
/// <see langword="struct"/>, avoid copying it (e.g., returning it from properties or storing it in collections
/// where it may be copied by value).
/// </summary>
[DebuggerDisplay("{Value}")]
public struct AtomicLong
{
    private long _value;

    /// <summary>
    /// Initializes a new <see cref="AtomicLong"/> with an optional initial value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AtomicLong(long initialValue = 0) => _value = initialValue;

    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    public long Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Volatile.Read(ref _value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interlocked.Exchange(ref _value, value);
    }

    /// <summary>
    /// Reads the current value using acquire semantics.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Read() => Volatile.Read(ref _value);

    /// <summary>
    /// Writes the value atomically.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(long value) => Interlocked.Exchange(ref _value, value);

    /// <summary>
    /// Atomically replaces the current value with <paramref name="value"/> and returns the previous value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Exchange(long value) => Interlocked.Exchange(ref _value, value);

    /// <summary>
    /// Atomically sets the value to <paramref name="value"/> if the current value equals <paramref name="comparand"/>.
    /// Returns the original value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long CompareExchange(long value, long comparand) =>
        Interlocked.CompareExchange(ref _value, value, comparand);

    /// <summary>
    /// Attempts to set the value to <paramref name="value"/> if the current value equals <paramref name="comparand"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompareExchange(long value, long comparand) =>
        Interlocked.CompareExchange(ref _value, value, comparand) == comparand;

    /// <summary>
    /// Atomically increments the value and returns the incremented value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Increment() => Interlocked.Increment(ref _value);

    /// <summary>
    /// Atomically decrements the value and returns the decremented value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Decrement() => Interlocked.Decrement(ref _value);

    /// <summary>
    /// Atomically adds <paramref name="delta"/> and returns the resulting value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Add(long delta) => Interlocked.Add(ref _value, delta);

    // ---- Get-and (returns previous) ----

    /// <summary>
    /// Atomically increments the value and returns the previous value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetAndIncrement() => Interlocked.Increment(ref _value) - 1;

    /// <summary>
    /// Atomically decrements the value and returns the previous value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetAndDecrement() => Interlocked.Decrement(ref _value) + 1;

    /// <summary>
    /// Atomically adds <paramref name="delta"/> and returns the previous value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetAndAdd(long delta) => Interlocked.Add(ref _value, delta) - delta;

    // ---- And-get (returns current) ----

    /// <summary>
    /// Atomically adds <paramref name="delta"/> and returns the resulting value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long AddAndGet(long delta) => Interlocked.Add(ref _value, delta);

    /// <summary>
    /// Atomically increments the value and returns the resulting value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long IncrementAndGet() => Interlocked.Increment(ref _value);

    /// <summary>
    /// Atomically decrements the value and returns the resulting value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long DecrementAndGet() => Interlocked.Decrement(ref _value);

    // ---- Conditional set helpers ----

    /// <summary>
    /// Attempts to set the value to <paramref name="value"/> if it is greater than the current value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySetIfGreater(long value)
    {
        long current = Volatile.Read(ref _value);
        if (value <= current)
            return false;

        return Interlocked.CompareExchange(ref _value, value, current) == current;
    }

    /// <summary>
    /// Attempts to set the value to <paramref name="value"/> if it is less than the current value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySetIfLess(long value)
    {
        long current = Volatile.Read(ref _value);
        if (value >= current)
            return false;

        return Interlocked.CompareExchange(ref _value, value, current) == current;
    }

    /// <summary>
    /// Sets the value to <paramref name="value"/> if it is greater than the current value, returning the effective value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long SetIfGreater(long value)
    {
        var spin = new SpinWait();

        while (true)
        {
            long current = Volatile.Read(ref _value);
            if (value <= current)
                return current;

            long prior = Interlocked.CompareExchange(ref _value, value, current);
            if (prior == current)
                return value;

            spin.SpinOnce();
        }
    }

    /// <summary>
    /// Sets the value to <paramref name="value"/> if it is less than the current value, returning the effective value.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long SetIfLess(long value)
    {
        var spin = new SpinWait();

        while (true)
        {
            long current = Volatile.Read(ref _value);
            if (value >= current)
                return current;

            long prior = Interlocked.CompareExchange(ref _value, value, current);
            if (prior == current)
                return value;

            spin.SpinOnce();
        }
    }

    // ---- CAS-loop transforms ----

    /// <summary>
    /// Atomically applies <paramref name="update"/> in a CAS loop and returns the updated value.
    /// </summary>
    public long Update(Func<long, long> update)
    {
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        var spin = new SpinWait();

        while (true)
        {
            long original = Volatile.Read(ref _value);
            long next = update(original);

            long prior = Interlocked.CompareExchange(ref _value, next, original);
            if (prior == original)
                return next;

            spin.SpinOnce();
        }
    }

    /// <summary>
    /// Attempts to apply <paramref name="update"/> once. Returns <see langword="true"/> on success.
    /// </summary>
    public bool TryUpdate(Func<long, long> update, out long original, out long updated)
    {
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        original = Volatile.Read(ref _value);
        updated = update(original);

        return Interlocked.CompareExchange(ref _value, updated, original) == original;
    }

    /// <summary>
    /// Atomically combines the current value with <paramref name="x"/> using <paramref name="accumulator"/>
    /// in a CAS loop and returns the resulting value.
    /// </summary>
    public long Accumulate(long x, Func<long, long, long> accumulator)
    {
        if (accumulator is null)
            throw new ArgumentNullException(nameof(accumulator));

        var spin = new SpinWait();

        while (true)
        {
            long original = Volatile.Read(ref _value);
            long next = accumulator(original, x);

            long prior = Interlocked.CompareExchange(ref _value, next, original);
            if (prior == original)
                return next;

            spin.SpinOnce();
        }
    }

    /// <summary>
    /// Returns a string representation of the current value.
    /// </summary>
    public override string ToString() => Volatile.Read(ref _value)
                                                 .ToString();
}