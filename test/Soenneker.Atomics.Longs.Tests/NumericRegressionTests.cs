using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Atomics.Longs.Tests;

public sealed class NumericRegressionTests
{
    [Test]
    public async Task Updates_and_accumulations_preserve_concurrent_writes()
    {
        var holder = new Holder();
        Parallel.For(0, 8, _ =>
        {
            for (int i = 0; i < 10000; i++)
            {
                holder.Value.Update(static x => x + 1);
                holder.Value.Accumulate(2, static (x, y) => x + y);
                holder.Value.Update(3, static (x, state) => x + state);
            }
        });
        await Assert.That(holder.Value.Read()).IsEqualTo((long)480000);
    }

    [Test]
    public async Task A_failed_exchange_retries_with_the_observed_value()
    {
        var holder = new Holder();
        int calls = 0;
        long result = holder.Value.Update(2, (original, state) =>
        {
            if (Interlocked.Increment(ref calls) == 1)
                holder.Value.Write(10);
            return original + state;
        });
        await Assert.That(result).IsEqualTo((long)12);
        await Assert.That(calls).IsEqualTo(2);
    }

    [Test]
    public async Task Concurrent_conditional_updates_converge_on_extremes()
    {
        var holder = new Holder();
        Parallel.For(0, 10000, i => holder.Value.SetIfGreater(i));
        await Assert.That(holder.Value.Read()).IsEqualTo((long)9999);
        Parallel.For(0, 10000, i => holder.Value.SetIfLess(-i));
        await Assert.That(holder.Value.Read()).IsEqualTo((long)(-9999));
        await Assert.That(() => holder.Value.Update(1, (Func<long, int, long>)null!)).Throws<ArgumentNullException>();
    }

    private sealed class Holder
    {
        public AtomicLong Value = new();
    }
}
