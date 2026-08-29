[![](https://img.shields.io/nuget/v/soenneker.atomics.longs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.atomics.longs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.atomics.longs/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.atomics.longs/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.atomics.longs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.atomics.longs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.atomics.longs/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.atomics.longs/actions/workflows/codeql.yml)

# Soenneker.Atomics.Longs

A lightweight, allocation-free atomic `long` backed by `Volatile` and `Interlocked` operations. Intended for use as a private field / inline synchronization primitive. Because this is a mutable `struct`, avoid copying it (e.g., returning it from properties or storing it in collections where it may be copied by value).

## Install

```bash
dotnet add package Soenneker.Atomics.Longs
```

## Usage

```csharp
using Soenneker.Atomics.Longs;

var bytesProcessed = new AtomicLong();

bytesProcessed.Add(buffer.Length);
long snapshot = bytesProcessed.Read();
```

Use `SetIfGreater` to retain a high-water mark under concurrency:

```csharp
peakQueueDepth.SetIfGreater(currentQueueDepth);
```

`TrySetIfGreater` and `TrySetIfLess` perform one compare-and-exchange attempt; the retrying `SetIfGreater` and `SetIfLess` variants are preferable when the maximum or minimum must eventually reflect the supplied candidate.

`Update` and `Accumulate` run a compare-and-exchange loop and may call their delegate multiple times. The delegate must not perform logging, I/O, mutation, or other side effects.

All members operate atomically on this counter, but several separate calls are not one transaction. Use a lock when multiple values or multi-step invariants must change together.

## What you get

- `AtomicLong` — A lightweight, allocation-free atomic `long` backed by `Volatile` and `Interlocked` operations. Intended for use as a private field / inline synchronization primitive. Because this is a mutable `struct`, avoid copying it (e.g., returning it from properties or storing it in collections where it may be copied by value).

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `AtomicLong.Value` | Gets or sets the current value. | Gets or sets the current value. |
| `AtomicLong.Read()` | Reads the current value using acquire semantics. | The current value observed with acquire memory-ordering semantics. |
| `AtomicLong.Exchange(value)` | Atomically replaces the current value with `value` and returns the previous value. | The value that was stored before the atomic update. |
| `AtomicLong.CompareExchange(value, comparand)` | Atomically sets the value to `value` if the current value equals `comparand`. Returns the original value. | The value observed before the compare-and-exchange attempt. |
| `AtomicLong.TryCompareExchange(value, comparand)` | Attempts to set the value to `value` if the current value equals `comparand`. | true if the requested update was applied; otherwise, false. |
| `AtomicLong.Increment()` | Atomically increments the value and returns the incremented value. | The incremented value. |
| `AtomicLong.Decrement()` | Atomically decrements the value and returns the decremented value. | The decremented value. |
| `AtomicLong.Add(delta)` | Atomically adds `delta` and returns the resulting value. | The resulting value. |
| `AtomicLong.GetAndIncrement()` | Atomically increments the value and returns the previous value. | The value that was stored before the atomic update. |
| `AtomicLong.GetAndDecrement()` | Atomically decrements the value and returns the previous value. | The value that was stored before the atomic update. |
| `AtomicLong.GetAndAdd(delta)` | Atomically adds `delta` and returns the previous value. | The value that was stored before the atomic update. |
| `AtomicLong.AddAndGet(delta)` | Atomically adds `delta` and returns the resulting value. | The resulting value. |
| `AtomicLong.IncrementAndGet()` | Atomically increments the value and returns the resulting value. | The resulting value. |
| `AtomicLong.DecrementAndGet()` | Atomically decrements the value and returns the resulting value. | The resulting value. |
| `AtomicLong.TrySetIfGreater(value)` | Attempts to set the value to `value` if it is greater than the current value. | true if the requested update was applied; otherwise, false. |
| `AtomicLong.TrySetIfLess(value)` | Attempts to set the value to `value` if it is less than the current value. | true if the requested update was applied; otherwise, false. |
| `AtomicLong.SetIfGreater(value)` | Sets the value to `value` if it is greater than the current value, returning the effective value. | The effective value. |
| `AtomicLong.SetIfLess(value)` | Sets the value to `value` if it is less than the current value, returning the effective value. | The effective value. |
