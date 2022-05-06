// -----------------------------------------------------------------------
// <copyright file="PickupChecker.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Handles the cleaning of items.
    /// </summary>
    public class PickupChecker
    {
        private readonly Plugin plugin;
        private readonly Dictionary<Pickup, float> itemTracker = new ();
        private CoroutineHandle cleanupCoroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupChecker"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public PickupChecker(Plugin plugin)
        {
            this.plugin = plugin;
        }

        /// <summary>
        /// Adds an item to the tracking queue.
        /// </summary>
        /// <param name="pickup">The item to add.</param>
        /// <param name="currentZone"> Current player zone for player hub. </param>
        public void Add(Pickup pickup, ZoneType currentZone)
        {
            try
            {
                bool foundItem = this.plugin.Config.ItemFilter.TryGetValue(pickup.Type, out float time);
                if (foundItem && this.plugin.Config.ZoneFilter.TryGetValue(pickup.Type, out HashSet<ZoneType> acceptedZones))
                {
                    if (acceptedZones.Contains(currentZone))
                    {
                        this.itemTracker.Add(pickup, Time.time + time);

                        // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                        if (this.plugin.Config.Debug)
                        {
                            Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds.", true);
                        }
                    }
                    else if (acceptedZones.Contains(ZoneType.Unspecified))
                    {
                        this.itemTracker.Add(pickup, Time.time + time);

                        // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                        if (this.plugin.Config.Debug)
                        {
                            Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds with Unspecified marked as acceptable.", true);
                        }
                    }
                    else if (this.plugin.Config.Debug)
                    {
                        // Added this if user wants to see why item was not added. Again, condition of config file much
                        Log.Debug($"Could not add item {pickup.Type} because zones were not equal current {currentZone} vs accepted {string.Join(Environment.NewLine, acceptedZones)}", true);
                    }
                }
                else if (foundItem)
                {
                    // We are going to assume that the user forgot to specify the zone. Therefore, the zone is unspecified.
                    if (this.plugin.Config.Debug)
                    {
                        Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds, defaulting to unspecified zone.", true);
                    }

                    this.itemTracker.Add(pickup, Time.time + time);
                }
            }
            catch (Exception ex)
            {
                Log.Debug($"Pickup.add failed because of {ex}", this.plugin.Config.Debug);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            this.itemTracker.Clear();
            if (this.cleanupCoroutine.IsRunning)
            {
                Timing.KillCoroutines(this.cleanupCoroutine);
            }

            this.cleanupCoroutine = Timing.RunCoroutine(this.CheckItems());
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            if (this.cleanupCoroutine.IsRunning)
            {
                Timing.KillCoroutines(this.cleanupCoroutine);
            }
        }

        private void CheckItem(Pickup pickup, float expirationTime)
        {
            if (!pickup.Base)
            {
                this.itemTracker.Remove(pickup);
                return;
            }

            if (pickup.InUse || Time.time < expirationTime)
            {
                return;
            }

            Log.Debug($"Deleting an item of type {pickup.Type} ({pickup.Serial}).", this.plugin.Config.Debug);
            pickup.Destroy();
            this.itemTracker.Remove(pickup);
        }

        private IEnumerator<float> CheckItems()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(this.plugin.Config.CheckInterval);
                if (this.itemTracker.IsEmpty())
                {
                    continue;
                }

                for (int i = 0; i < this.itemTracker.Count; i++)
                {
                    KeyValuePair<Pickup, float> item = this.itemTracker.ElementAt(i);
                    this.CheckItem(item.Key, item.Value);
                }
            }
        }
    }
}