// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace PerformanceUpToDate.Design;

public sealed class CircularQueue<T>
{
    private readonly Slot[] slots;
    private readonly int slotsMask;
    private PaddedHeadAndTail headAndTail;

    /// <summary>Initializes a new instance of the <see cref="CircularQueue{T}"/> class.</summary>
    /// <param name="boundedLength">The maximum number of elements the queue can contain. Must be a power of 2.</param>
    internal CircularQueue(int boundedLength)
    {
        // Validate the length
        Debug.Assert(boundedLength >= 2, $"Must be >= 2, got {boundedLength}");
        Debug.Assert(BitOperations.IsPow2(boundedLength), $"Must be a power of 2, got {boundedLength}");

        this.slots = new Slot[boundedLength];
        this.slotsMask = boundedLength - 1;
        for (var i = 0; i < this.slots.Length; i++)
        {
            this.slots[i].SequenceNumber = i;
        }
    }

    /// <summary>Gets the number of elements this queue can store.</summary>
    public int Capacity => this.slots.Length;

    /// <summary>
    /// Tries to dequeue an element from the circular queue.
    /// </summary>
    /// <param name="item">The dequeued item, if successful; otherwise, the default value of <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> if an item was successfully dequeued; otherwise, <see langword="false"/>.</returns>
    public bool TryDequeue([MaybeNullWhen(false)] out T item)
    {
        var slots = this.slots;
        while (true)
        {
            var currentHead = Volatile.Read(ref this.headAndTail.Head);
            var slotsIndex = currentHead & this.slotsMask;
            var sequenceNumber = Volatile.Read(ref slots[slotsIndex].SequenceNumber);
            var diff = sequenceNumber - (currentHead + 1);
            if (diff == 0)
            {
                if (Interlocked.CompareExchange(ref this.headAndTail.Head, currentHead + 1, currentHead) == currentHead)
                {
                    item = slots[slotsIndex].Item!;
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                    {
                        slots[slotsIndex].Item = default;
                    }

                    Volatile.Write(ref slots[slotsIndex].SequenceNumber, currentHead + slots.Length);
                    return true;
                }
            }
            else if (diff < 0)
            {
                int currentTail = Volatile.Read(ref this.headAndTail.Tail);
                if (currentTail - currentHead <= 0)
                {
                    item = default;
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// Tries to enqueue an element into the queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    /// <returns>
    /// <see langword="true"/> if the item was successfully enqueued; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryEnqueue(T item)
    {
        Slot[] slots = this.slots;
        while (true)
        {
            int currentTail = Volatile.Read(ref this.headAndTail.Tail);
            int slotsIndex = currentTail & this.slotsMask;
            int sequenceNumber = Volatile.Read(ref slots[slotsIndex].SequenceNumber);
            int diff = sequenceNumber - currentTail;
            if (diff == 0)
            {
                if (Interlocked.CompareExchange(ref this.headAndTail.Tail, currentTail + 1, currentTail) == currentTail)
                {
                    slots[slotsIndex].Item = item;
                    Volatile.Write(ref slots[slotsIndex].SequenceNumber, currentTail + 1);
                    return true;
                }
            }
            else if (diff < 0)
            {
                return false;
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    private struct Slot
    {
        public T? Item;
        public int SequenceNumber;
    }

    [StructLayout(LayoutKind.Explicit, Size = 3 * CacheLineSize)] // padding before/between/after fields
    private struct PaddedHeadAndTail
    {
#if TARGET_ARM64
    public const int CacheLineSize = 128;
#else
        public const int CacheLineSize = 64;
#endif

        [FieldOffset(1 * CacheLineSize)]
        public int Head;

        [FieldOffset(2 * CacheLineSize)]
        public int Tail;
    }
}
