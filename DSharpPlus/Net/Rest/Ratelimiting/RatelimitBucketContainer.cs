using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DSharpPlus.Net.Ratelimiting;

internal sealed class RatelimitBucketContainer
{
    private Slot[] slotTable = new Slot[16];
    private Entry[] entries = new Entry[16];
    private int currentLength = 16;

    private SpinLock resizeLock = new();
    private bool isResizing = false;

    public RatelimitBucketLease? LeaseOrCreateRouteAsync(string route)
    {
        Debug.Assert(this.slotTable.Length == this.entries.Length);
        SpinWait.SpinUntil(() => !this.isResizing);

        for (int i = 0; i < this.slotTable.Length; i++)
        {
            scoped ref Slot slot = ref this.slotTable[i];

            if (!slot.HasFlag(Slot.Live))
            {
                continue;
            }

            SpinWait.SpinUntil(() => !this.slotTable[i].HasFlag(Slot.Locked));
            slot |= Slot.Locked;

            if (this.entries[i].routes.Contains(route) && this.entries[i].bucket.CheckNextRequest())
            {
                slot ^= Slot.Locked;
                return new(this, i);
            }

            slot ^= Slot.Locked;
            continue;
        }

        // we didn't find a matching entry, create a new one
        // also set a jump label for after we resize, in case we do
        scanSlotTableForFreeSlot:

        for (int i = 0; i < this.slotTable.Length; i++)
        {
            scoped ref Slot slot = ref this.slotTable[i];

            if (slot.HasFlag(Slot.Live))
            {
                continue;
            }

            SpinWait.SpinUntil(() => !this.slotTable[i].HasFlag(Slot.Locked));
            slot |= Slot.Locked;

            slot |= Slot.Live;
            scoped ref Entry entry = ref this.entries[i];
            entry.bucket = new();
            entry.hash = "pending";
            entry.routes = [route];

            slot ^= Slot.Locked;
            return new(this, i);
        }

        // we found no free slots, resize

        bool wasAcquiredSuccessfully = false;
        bool thisVeryThreadNeedsToResize = !this.resizeLock.IsHeld;
        this.resizeLock.Enter(ref wasAcquiredSuccessfully);

        if (!wasAcquiredSuccessfully)
        {
            throw new InvalidOperationException("Failed to enter the resize lock. Ratelimit state from here on may be corrupt.");
        }

        if (thisVeryThreadNeedsToResize)
        {
            this.ResizeBackingStorage();
        }

        this.resizeLock.Exit();
        goto scanSlotTableForFreeSlot;
    }

    private void ResizeBackingStorage()
    {
        Slot[] newSlotTable = new Slot[this.currentLength * 2];
        Entry[] newEntries = new Entry[this.currentLength * 2];

        this.currentLength *= 2;

        this.isResizing = true;
        this.slotTable.CopyTo(newSlotTable, 0);
        this.entries.CopyTo(newEntries, 0);

        this.slotTable = newSlotTable;
        this.entries = newEntries;
        this.isResizing = false;
    }

    [Flags]
    private enum Slot : byte
    {
        Locked = 1 << 1,
        Live = 1 << 2,
    }

    private struct Entry
    {
        public string hash;
        public List<string> routes;
        public RatelimitBucket bucket;
    }
}
