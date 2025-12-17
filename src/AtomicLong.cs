using Soenneker.Atomics.Longs.Abstract;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Soenneker.Atomics.Longs;

/// <inheritdoc cref="IAtomicLong"/>
public sealed class AtomicLong : IAtomicLong
{
    private long _value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AtomicLong()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AtomicLong(long initialValue) => _value = initialValue;

    public long Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Volatile.Read(ref _value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Interlocked.Exchange(ref _value, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Read() => Volatile.Read(ref _value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(long value) => Interlocked.Exchange(ref _value, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Exchange(long value) => Interlocked.Exchange(ref _value, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long CompareExchange(long value, long comparand) =>
        Interlocked.CompareExchange(ref _value, value, comparand);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompareExchange(long value, long comparand) =>
        Interlocked.CompareExchange(ref _value, value, comparand) == comparand;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Increment() => Interlocked.Increment(ref _value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Decrement() => Interlocked.Decrement(ref _value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Add(long delta) => Interlocked.Add(ref _value, delta);

    // ---- Get-and (returns previous) ----

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetAndIncrement() => Interlocked.Increment(ref _value) - 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetAndDecrement() => Interlocked.Decrement(ref _value) + 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long GetAndAdd(long delta) => Interlocked.Add(ref _value, delta) - delta;

    // ---- And-get (returns current) ----

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long AddAndGet(long delta) => Interlocked.Add(ref _value, delta);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long IncrementAndGet() => Interlocked.Increment(ref _value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long DecrementAndGet() => Interlocked.Decrement(ref _value);

    // ---- Conditional set helpers ----

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySetIfGreater(long value)
    {
        long current = Volatile.Read(ref _value);
        if (value <= current)
            return false;

        return Interlocked.CompareExchange(ref _value, value, current) == current;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TrySetIfLess(long value)
    {
        long current = Volatile.Read(ref _value);
        if (value >= current)
            return false;

        return Interlocked.CompareExchange(ref _value, value, current) == current;
    }

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

    public bool TryUpdate(Func<long, long> update, out long original, out long updated)
    {
        if (update is null)
            throw new ArgumentNullException(nameof(update));

        original = Volatile.Read(ref _value);
        updated = update(original);

        return Interlocked.CompareExchange(ref _value, updated, original) == original;
    }

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

    public override string ToString() => Volatile.Read(ref _value).ToString();
}