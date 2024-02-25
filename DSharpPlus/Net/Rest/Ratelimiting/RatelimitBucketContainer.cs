using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DSharpPlus.Net.Ratelimiting;

internal sealed class RatelimitBucketContainer
{
    private SlotTableEntry[] slotTable = new SlotTableEntry[16];
    private Entry[] entries = new Entry[16];
    private int currentLength = 16;

    private SpinLock resizeLock = new();
    private bool isResizing = false;

    public RatelimitBucketLease LeaseOrCreateRouteAsync(string route)
    {
        Debug.Assert(this.slotTable.Length == this.entries.Length);
        SpinWait.SpinUntil(() => !this.isResizing);

        for (int i = 0; i < this.slotTable.Length; i++)
        {
            scoped ref SlotTableEntry slot = ref this.slotTable[i];

            if (!slot.isLive)
            {
                continue;
            }

            slot.rwlock.EnterReadLock();

            if (this.entries[i].routes.Contains(route))
            {
                slot.rwlock.ExitReadLock();
                slot.rwlock.EnterWriteLock();
                slot.isLeased = true;

                return new(this, i);
            }

            slot.rwlock.ExitReadLock();
            continue;
        }

        // we didn't find a matching entry, create a new one
        // also set a jump label for after we resize, in case we do
        scanSlotTableForFreeSlot:

        for (int i = 0; i < this.slotTable.Length; i++)
        {
            scoped ref SlotTableEntry slot = ref this.slotTable[i];

            if (slot.isLive)
            {
                continue;
            }

            slot.rwlock.EnterWriteLock();

            scoped ref Entry entry = ref this.entries[i];
            entry.bucket = new();
            entry.hash = "pending";
            entry.routes = [route];

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
        SlotTableEntry[] newSlotTable = new SlotTableEntry[this.currentLength * 2];
        Entry[] newEntries = new Entry[this.currentLength * 2];

        this.currentLength *= 2;

        this.isResizing = true;
        this.slotTable.CopyTo(newSlotTable, 0);
        this.entries.CopyTo(newEntries, 0);

        this.slotTable = newSlotTable;
        this.entries = newEntries;
        this.isResizing = false;
    }

    // absolute horrendous garbage, but we need to get this fix out one of these days. we can improve this later
    private struct SlotTableEntry
    {
        public ReaderWriterLockSlim rwlock = new();
        public bool isLive;
        public bool isLeased;

        public SlotTableEntry()
        {
        }
    }

    private struct Entry
    {
        public string hash;
        public List<string> routes;
        public RatelimitBucket bucket;
    }
}
