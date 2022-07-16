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
    using Exiled.Events.EventArgs;
    using InventorySystem.Items.Usables.Scp330;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Handles the cleaning of items.
    /// </summary>
    public class PickupChecker
    {
        private readonly Plugin plugin;
        private readonly Dictionary<Pickup, float> itemTracker = new();
        private CoroutineHandle cleanupItemsCoroutine;
        private CoroutineHandle cleanupRagDollsCoroutine;

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
        /// <param name="curPlayer"> Current player. </param>
        public void Add(Pickup pickup, ZoneType currentZone, Player curPlayer)
        {
            try
            {
                bool foundItem = this.plugin.Config.ItemFilter.TryGetValue(pickup.Type, out float time);
                bool isPocket = curPlayer.SessionVariables.TryGetValue("InPocket", out object inPocket);
                if (foundItem && isPocket)
                {
                    if ((bool)inPocket)
                    {
                        if (this.plugin.Config.CleanInPocket)
                        {
                            Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds from pocket dimension.", this.plugin.Config.Debug);
                            this.itemTracker.Add(pickup, Time.time + time);
                            return;
                        }
                        else if (!this.plugin.Config.CleanInPocket)
                        {
                            return;
                        }
                    }
                }
                else if (foundItem && this.plugin.Config.ZoneFilter.TryGetValue(pickup.Type, out HashSet<ZoneType> acceptedZones))
                {
                    if (acceptedZones.Contains(currentZone))
                    {
                        this.itemTracker.Add(pickup, Time.time + time);

                        // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                        if (this.plugin.Config.Debug)
                        {
                            Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds.");
                        }
                    }
                    else if (acceptedZones.Contains(ZoneType.Unspecified))
                    {
                        this.itemTracker.Add(pickup, Time.time + time);

                        // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                        if (this.plugin.Config.Debug)
                        {
                            Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds with Unspecified marked as acceptable.");
                        }
                    }
                    else if (this.plugin.Config.Debug)
                    {
                        // Added this if user wants to see why item was not added. Again, condition of config file much
                        Log.Debug($"Could not add item {pickup.Type} because zones were not equal current {currentZone} vs accepted {string.Join(Environment.NewLine, acceptedZones)}");
                    }

                    return;
                }

                if (foundItem)
                {
                    // We are going to assume that the user forgot to specify the zone. Therefore, the zone is unspecified.
                    if (this.plugin.Config.Debug)
                    {
                        Log.Debug($"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds, defaulting to unspecified zone.");
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
            if (this.cleanupItemsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(this.cleanupItemsCoroutine);
            }

            if (this.cleanupRagDollsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(this.cleanupRagDollsCoroutine);
            }

            if (this.plugin.Config.CleanupItems)
            {
                this.cleanupItemsCoroutine = Timing.RunCoroutine(this.CheckItems());
            }

            if (this.plugin.Config.CleanupRagDolls)
            {
                this.cleanupRagDollsCoroutine = Timing.RunCoroutine(this.CheckRagDolls());
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            if (this.cleanupItemsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(this.cleanupItemsCoroutine);
            }

            if (this.cleanupRagDollsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(this.cleanupRagDollsCoroutine);
            }
        }

        /// <summary>
        /// When a player changes role, we will ensure InPocket flag is removed.
        /// </summary>
        /// <param name="ev"> Event for changing role. </param>
        internal void OnRoleChange(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(this.plugin.Config.CheckInterval + 5, () =>
            {
                ev?.Player?.SessionVariables.Remove("InPocket");
            });
        }

        /// <summary>
        /// When a player dies, we will ensure InPocket flag is removed.
        /// </summary>
        /// <param name="ev"> Event for dying. </param>
        internal void OnDied(DiedEventArgs ev)
        {
            Timing.CallDelayed(this.plugin.Config.CheckInterval + 5, () =>
            {
                ev?.Target?.SessionVariables.Remove("InPocket");
            });
        }

        /// <summary>
        /// When a player enteres the pocket dimension, we will mark them as left.
        /// </summary>
        /// <param name="ev"> Current pocket dimension event info. </param>
        internal void OnPocketExit(EscapingPocketDimensionEventArgs ev)
        {
            ev.Player.SessionVariables["InPocket"] = false;
        }

        /// <summary>
        /// When a player enteres the pocket dimension, we will mark them as entered.
        /// </summary>
        /// <param name="ev"> Current pocket dimension event info. </param>
        internal void OnPocketEnter(EnteringPocketDimensionEventArgs ev)
        {
            ev.Player.SessionVariables.Add("InPocket", true);
        }

        /// <summary>
        /// Coroutine to iteratively check all currently tracked items to be removed within a zone/time limit.
        /// </summary>
        /// <returns> <see cref="IEnumerable{T}"/> which is used to determine how long this generator function should wait. </returns>
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

        /// <summary>
        /// Checks the current pickup with the experitation time.
        /// </summary>
        /// <param name="pickup"> <see cref="Pickup"/> to verify whether to delete. </param>
        /// <param name="expirationTime"> Time limit for pickup. </param>
        private void CheckItem(Pickup pickup, float expirationTime)
        {
            if (pickup?.Base is null)
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

        /// <summary>
        /// Coroutine to iteratively check all currently ragdolls to be removed within a zone/time limit.
        /// </summary>
        /// <returns> <see cref="IEnumerable{T}"/> which is used to determine how long this generator function should wait. </returns>
        private IEnumerator<float> CheckRagDolls()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(this.plugin.Config.CheckRagDollInterval);
                if (Map.Ragdolls.IsEmpty())
                {
                    continue;
                }

                for (int i = 0; i < Map.Ragdolls.Count; i++)
                {
                    Ragdoll curRagdoll = Map.Ragdolls.ElementAt(i);
                    this.CheckRagDoll(curRagdoll);
                }
            }
        }

        /// <summary>
        /// Verifies the ragdoll can be cleanup, and is not null.
        /// </summary>
        /// <param name="curRagdoll"> <see cref="Ragdoll"/> to potentially clean up. </param>
        private void CheckRagDoll(Ragdoll curRagdoll)
        {
            if (curRagdoll.Base is null)
            {
                return;
            }

            if (curRagdoll.Base.Info.ExistenceTime < this.plugin.Config.RagdollExistenceLimit)
            {
                return;
            }

            if (curRagdoll.Owner?.SessionVariables.TryGetValue("InPocket", out object inPocket) ?? false)
            {
                if (!this.plugin.Config.CleanInPocket)
                {
                    return;
                }
                else if ((bool)inPocket)
                {
                    Log.Debug($"Deleting a Radoll {curRagdoll} in pocket dimension", this.plugin.Config.Debug);
                    curRagdoll.Delete();
                    return;
                }
            }

            if (!this.plugin.Config.RagdollAcceptableZones.Contains(curRagdoll.Zone))
            {
                return;
            }

            Log.Debug($"Deleting a Radoll {curRagdoll} in zone {curRagdoll.Zone}", this.plugin.Config.Debug);

            curRagdoll.Delete();
            return;
        }
    }
}