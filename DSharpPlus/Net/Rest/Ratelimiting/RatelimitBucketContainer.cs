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

            if ((this.entries[i].routes?.Contains(route) ?? false) && this.entries[i].bucket.CheckNextRequest())
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

    public void ReturnLease(RatelimitCandidateBucket candidate, string route, int index)
    {
        scoped ref Slot slot = ref this.slotTable[index];
        scoped ref Entry entry = ref this.entries[index];

        SpinWait.SpinUntil(() => !this.slotTable[index].HasFlag(Slot.Locked));
        slot |= Slot.Locked;

        // if the hash has changed, check whether we need to migrate this route
        if (entry.hash != candidate.Hash)
        {
            for (int i = 0; i < this.slotTable.Length; i++)
            {
                // don't try to inspect the slot we already have, that'll deadlock
                if (i == index)
                {
                    continue;
                }

                scoped ref Slot potential = ref this.slotTable[i];

                if (!potential.HasFlag(Slot.Live))
                {
                    continue;
                }

                SpinWait.SpinUntil(() => !this.slotTable[i].HasFlag(Slot.Locked));
                potential |= Slot.Locked;

                if (this.entries[i].hash == candidate.Hash)
                {
                    entry.bucket.CancelReservation();
                    scoped ref Entry actualEntry = ref this.entries[i];

                    if (!actualEntry.routes?.Contains(route) ?? false)
                    {
                        actualEntry.routes ??= [];
                        actualEntry.routes = [.. actualEntry.routes, route];
                    }

                    actualEntry.bucket.UpdateBucket(candidate.Maximum, candidate.Remaining, candidate.Reset);

                    // if this was the last route keeping the old slot alive, unregister and mark as dead
                    entry.routes?.Remove(route);

                    if (entry.routes is null or [])
                    {
                        slot ^= Slot.Live;
                    }

                    // remove all locks and vacate the premises
                    potential ^= Slot.Locked;
                    slot ^= Slot.Locked;

                    return;
                }

                potential ^= Slot.Locked;
                continue;
            }

            // we couldn't find a matching bucket, edit this one
            entry.hash = candidate.Hash;
            entry.bucket.UpdateBucket(candidate.Maximum, candidate.Remaining, candidate.Reset);

            slot ^= Slot.Locked;
            return;
        }

        // the hash has not changed. update the bucket.
        entry.bucket.UpdateBucket(candidate.Maximum, candidate.Remaining, candidate.Reset);

        slot ^= Slot.Locked;
        return;
    }

    private void ResizeBackingStorage()
    {
        Slot[] newSlotTable = new Slot[this.currentLength * 2];
        Entry[] newEntries = new Entry[this.currentLength * 2];

        this.currentLength *= 2;

        this.isResizing = true;
        Array.Copy(newSlotTable, this.slotTable, this.slotTable.Length);
        Array.Copy(newEntries, this.entries, this.entries.Length);

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
        public List<string>? routes;
        public RatelimitBucket bucket;
    }
}
