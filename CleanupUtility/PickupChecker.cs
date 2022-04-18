// -----------------------------------------------------------------------
// <copyright file="PickupChecker.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility
{
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Handles the cleaning of items.
    /// </summary>
    public class PickupChecker
    {
        private readonly Plugin plugin;
        private readonly Dictionary<Pickup, float> itemTracker = new();
        private CoroutineHandle cleanupCoroutine;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupChecker"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public PickupChecker(Plugin plugin) => this.plugin = plugin;

        /// <summary>
        /// Adds an item to the tracking queue.
        /// </summary>
        /// <param name="pickup">The item to add.</param>
        public void Add(Pickup pickup)
        {
            if (plugin.Config.ItemFilter.TryGetValue(pickup.Type, out float time))
            {
                itemTracker.Add(pickup, Time.time + time);
                Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds.", plugin.Config.Debug);
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            itemTracker.Clear();
            if (cleanupCoroutine.IsRunning)
                Timing.KillCoroutines(cleanupCoroutine);

            cleanupCoroutine = Timing.RunCoroutine(CheckItems());
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            if (cleanupCoroutine.IsRunning)
                Timing.KillCoroutines(cleanupCoroutine);
        }

        private void CheckItem(Pickup pickup, float expirationTime)
        {
            if (!pickup.Base)
            {
                itemTracker.Remove(pickup);
                return;
            }

            if (pickup.InUse || Time.time < expirationTime)
                return;

            Log.Debug($"Deleting an item of type {pickup.Type} ({pickup.Serial}).", plugin.Config.Debug);
            pickup.Destroy();
            itemTracker.Remove(pickup);
        }

        private IEnumerator<float> CheckItems()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(plugin.Config.CheckInterval);
                if (itemTracker.IsEmpty())
                    continue;

                for (int i = 0; i < itemTracker.Count; i++)
                {
                    KeyValuePair<Pickup, float> item = itemTracker.ElementAt(i);
                    CheckItem(item.Key, item.Value);
                }
            }
        }
    }
}