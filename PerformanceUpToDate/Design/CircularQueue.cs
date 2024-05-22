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

        // Loop in case of contention...
        SpinWait spinner = default;
        while (true)
        {
            // Get the head at which to try to dequeue.
            int currentHead = Volatile.Read(ref this.headAndTail.Head);
            int slotsIndex = currentHead & this.slotsMask;

            // Read the sequence number for the head position.
            int sequenceNumber = Volatile.Read(ref slots[slotsIndex].SequenceNumber);

            // We can dequeue from this slot if it's been filled by an enqueuer, which
            // would have left the sequence number at pos+1.
            int diff = sequenceNumber - (currentHead + 1);
            if (diff == 0)
            {
                // We may be racing with other dequeuers.  Try to reserve the slot by incrementing
                // the head.  Once we've done that, no one else will be able to read from this slot,
                // and no enqueuer will be able to read from this slot until we've written the new
                // sequence number. WARNING: The next few lines are not reliable on a runtime that
                // supports thread aborts. If a thread abort were to sneak in after the CompareExchange
                // but before the Volatile.Write, enqueuers trying to enqueue into this slot would
                // spin indefinitely.  If this implementation is ever used on such a platform, this
                // if block should be wrapped in a finally / prepared region.
                if (Interlocked.CompareExchange(ref this.headAndTail.Head, currentHead + 1, currentHead) == currentHead)
                {
                    // Successfully reserved the slot.  Note that after the above CompareExchange, other threads
                    // trying to dequeue from this slot will end up spinning until we do the subsequent Write.
                    item = slots[slotsIndex].Item!;

                    // If we're preserving, though, we don't zero out the slot, as we need it for
                    // enumerations, peeking, ToArray, etc.  And we don't update the sequence number,
                    // so that an enqueuer will see it as full and be forced to move to a new segment.
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
                    {
                        slots[slotsIndex].Item = default;
                    }

                    Volatile.Write(ref slots[slotsIndex].SequenceNumber, currentHead + slots.Length);
                    return true;
                }

                // The head was already advanced by another thread. A newer head has already been observed and the next
                // iteration would make forward progress, so there's no need to spin-wait before trying again.
            }
            else if (diff < 0)
            {
                // The sequence number was less than what we needed, which means this slot doesn't
                // yet contain a value we can dequeue, i.e. the segment is empty.  Technically it's
                // possible that multiple enqueuers could have written concurrently, with those
                // getting later slots actually finishing first, so there could be elements after
                // this one that are available, but we need to dequeue in order.  So before declaring
                // failure and that the segment is empty, we check the tail to see if we're actually
                // empty or if we're just waiting for items in flight or after this one to become available.
                int currentTail = Volatile.Read(ref this.headAndTail.Tail);
                if (currentTail - currentHead <= 0)
                {
                    item = default;
                    return false;
                }

                // It's possible it could have become frozen after we checked _frozenForEnqueues
                // and before reading the tail.  That's ok: in that rare race condition, we just
                // loop around again. This is not necessarily an always-forward-progressing
                // situation since this thread is waiting for another to write to the slot and
                // this thread may have to check the same slot multiple times. Spin-wait to avoid
                // a potential busy-wait, and then try again.
                spinner.SpinOnce(sleep1Threshold: -1);
            }
            else
            {
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

        // Loop in case of contention...
        while (true)
        {
            // Get the tail at which to try to return.
            int currentTail = Volatile.Read(ref this.headAndTail.Tail);
            int slotsIndex = currentTail & this.slotsMask;

            // Read the sequence number for the tail position.
            int sequenceNumber = Volatile.Read(ref slots[slotsIndex].SequenceNumber);

            // The slot is empty and ready for us to enqueue into it if its sequence
            // number matches the slot.
            int diff = sequenceNumber - currentTail;
            if (diff == 0)
            {
                // We may be racing with other enqueuers.  Try to reserve the slot by incrementing
                // the tail.  Once we've done that, no one else will be able to write to this slot,
                // and no dequeuer will be able to read from this slot until we've written the new
                // sequence number. WARNING: The next few lines are not reliable on a runtime that
                // supports thread aborts. If a thread abort were to sneak in after the CompareExchange
                // but before the Volatile.Write, other threads will spin trying to access this slot.
                // If this implementation is ever used on such a platform, this if block should be
                // wrapped in a finally / prepared region.
                if (Interlocked.CompareExchange(ref this.headAndTail.Tail, currentTail + 1, currentTail) == currentTail)
                {
                    // Successfully reserved the slot.  Note that after the above CompareExchange, other threads
                    // trying to return will end up spinning until we do the subsequent Write.
                    slots[slotsIndex].Item = item;
                    Volatile.Write(ref slots[slotsIndex].SequenceNumber, currentTail + 1);
                    return true;
                }

                // The tail was already advanced by another thread. A newer tail has already been observed and the next
                // iteration would make forward progress, so there's no need to spin-wait before trying again.
            }
            else if (diff < 0)
            {
                // The sequence number was less than what we needed, which means this slot still
                // contains a value, i.e. the segment is full.  Technically it's possible that multiple
                // dequeuers could have read concurrently, with those getting later slots actually
                // finishing first, so there could be spaces after this one that are available, but
                // we need to enqueue in order.
                return false;
            }
            else
            {
                // Either the slot contains an item, or it is empty but because the slot was filled and dequeued. In either
                // case, the tail has already been updated beyond what was observed above, and the sequence number observed
                // above as a volatile load is more recent than the update to the tail. So, the next iteration of the loop
                // is guaranteed to see a new tail. Since this is an always-forward-progressing situation, there's no need
                // to spin-wait before trying again.
            }
        }
    }

    [StructLayout(LayoutKind.Auto)]
    internal struct Slot
    {
        public T? Item;
        public int SequenceNumber;
    }
}

[StructLayout(LayoutKind.Explicit, Size = 3 * CacheLineSize)] // padding before/between/after fields
internal struct PaddedHeadAndTail
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
