namespace Soenneker.Atomics.Longs.Abstract;

/// <summary>
/// Represents a thread-safe 64-bit integer backed by atomic operations
/// using <see cref="System.Threading.Interlocked"/> and
/// <see cref="System.Threading.Volatile"/>.
/// </summary>
/// <remarks>
/// All operations are lock-free and safe for concurrent use.
/// </remarks>
public interface IAtomicLong
{
    /// <summary>
    /// Gets or sets the current value atomically.
    /// </summary>
    long Value { get; set; }

    /// <summary>
    /// Reads the current value atomically.
    /// </summary>
    /// <returns>The current value.</returns>
    long Read();

    /// <summary>
    /// Writes a new value atomically.
    /// </summary>
    /// <param name="value">The value to write.</param>
    void Write(long value);

    /// <summary>
    /// Atomically sets the value to <paramref name="value"/> and returns the previous value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <returns>The original value before the exchange.</returns>
    long Exchange(long value);

    /// <summary>
    /// Atomically sets the value to <paramref name="value"/> if the current value
    /// equals <paramref name="comparand"/>.
    /// </summary>
    /// <param name="value">The value to set if the comparison succeeds.</param>
    /// <param name="comparand">The expected current value.</param>
    /// <returns>
    /// The original value that was present before the operation, regardless of success.
    /// </returns>
    long CompareExchange(long value, long comparand);

    /// <summary>
    /// Attempts to atomically set the value to <paramref name="value"/> if the current
    /// value equals <paramref name="comparand"/>.
    /// </summary>
    /// <param name="value">The value to set if the comparison succeeds.</param>
    /// <param name="comparand">The expected current value.</param>
    /// <returns>
    /// <c>true</c> if the value was updated; otherwise, <c>false</c>.
    /// </returns>
    bool TryCompareExchange(long value, long comparand);

    /// <summary>
    /// Atomically increments the current value by one.
    /// </summary>
    /// <returns>The incremented value.</returns>
    long Increment();

    /// <summary>
    /// Atomically decrements the current value by one.
    /// </summary>
    /// <returns>The decremented value.</returns>
    long Decrement();

    /// <summary>
    /// Atomically adds <paramref name="delta"/> to the current value.
    /// </summary>
    /// <param name="delta">The value to add.</param>
    /// <returns>The resulting value after the addition.</returns>
    long Add(long delta);

    /// <summary>
    /// Atomically increments the value and returns the previous value.
    /// </summary>
    /// <returns>The value before the increment.</returns>
    long GetAndIncrement();

    /// <summary>
    /// Atomically decrements the value and returns the previous value.
    /// </summary>
    /// <returns>The value before the decrement.</returns>
    long GetAndDecrement();

    /// <summary>
    /// Atomically adds <paramref name="delta"/> to the value and returns the previous value.
    /// </summary>
    /// <param name="delta">The value to add.</param>
    /// <returns>The value before the addition.</returns>
    long GetAndAdd(long delta);

    /// <summary>
    /// Atomically adds <paramref name="delta"/> to the value and returns the updated value.
    /// </summary>
    /// <param name="delta">The value to add.</param>
    /// <returns>The value after the addition.</returns>
    long AddAndGet(long delta);

    /// <summary>
    /// Atomically increments the value and returns the updated value.
    /// </summary>
    /// <returns>The value after the increment.</returns>
    long IncrementAndGet();

    /// <summary>
    /// Atomically decrements the value and returns the updated value.
    /// </summary>
    /// <returns>The value after the decrement.</returns>
    long DecrementAndGet();

    /// <summary>
    /// Attempts to set the value to <paramref name="value"/> if it is greater than
    /// the current value.
    /// </summary>
    /// <param name="value">The candidate value.</param>
    /// <returns>
    /// <c>true</c> if the value was updated; otherwise, <c>false</c>.
    /// </returns>
    bool TrySetIfGreater(long value);

    /// <summary>
    /// Attempts to set the value to <paramref name="value"/> if it is less than
    /// the current value.
    /// </summary>
    /// <param name="value">The candidate value.</param>
    /// <returns>
    /// <c>true</c> if the value was updated; otherwise, <c>false</c>.
    /// </returns>
    bool TrySetIfLess(long value);

    /// <summary>
    /// Ensures the value is at least <paramref name="value"/>, retrying until successful
    /// or the current value is already greater.
    /// </summary>
    /// <param name="value">The minimum value to enforce.</param>
    /// <returns>The resulting value.</returns>
    long SetIfGreater(long value);

    /// <summary>
    /// Ensures the value is at most <paramref name="value"/>, retrying until successful
    /// or the current value is already less.
    /// </summary>
    /// <param name="value">The maximum value to enforce.</param>
    /// <returns>The resulting value.</returns>
    long SetIfLess(long value);

    /// <summary>
    /// Atomically updates the value using a compare-and-swap loop.
    /// </summary>
    /// <param name="update">
    /// A function that computes the new value from the current value.
    /// </param>
    /// <returns>The updated value.</returns>
    long Update(System.Func<long, long> update);

    /// <summary>
    /// Attempts to atomically update the value using a single compare-and-swap operation.
    /// </summary>
    /// <param name="update">A function that computes the new value.</param>
    /// <param name="original">The value observed before the update.</param>
    /// <param name="updated">The computed new value.</param>
    /// <returns>
    /// <c>true</c> if the update succeeded; otherwise, <c>false</c>.
    /// </returns>
    bool TryUpdate(System.Func<long, long> update, out long original, out long updated);

    /// <summary>
    /// Atomically combines the current value with <paramref name="x"/> using the
    /// specified accumulator function.
    /// </summary>
    /// <param name="x">The second operand.</param>
    /// <param name="accumulator">
    /// A function that combines the current value and <paramref name="x"/>.
    /// </param>
    /// <returns>The updated value.</returns>
    long Accumulate(long x, System.Func<long, long, long> accumulator);
}