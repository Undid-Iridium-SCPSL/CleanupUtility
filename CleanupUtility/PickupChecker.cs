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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Handles the cleaning of items.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:Prefix local calls with this", Justification = "<Rider disagrees.>")]
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
                bool foundItem = plugin.Config.ItemFilter.TryGetValue(pickup.Type, out float time);
                bool isPocket = curPlayer.SessionVariables.TryGetValue("InPocket", out object inPocket);
                if (foundItem && time >= 0)
                {
                    if (isPocket)
                    {
                        if ((bool)inPocket)
                        {
                            if (plugin.Config.CleanInPocket)
                            {
                                Log.Debug(
                                    $"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds from pocket dimension.");
                                itemTracker.Add(pickup, Time.time + time);
                            }

                            return;
                        }
                    }
                    else if (
                             plugin.Config.ZoneFilter.TryGetValue(
                                 pickup.Type,
                                 out HashSet<ZoneType> acceptedZones))
                    {
                        if (acceptedZones.Contains(currentZone))
                        {
                            itemTracker.Add(pickup, Time.time + time);

                            // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                            if (plugin.Config.Debug)
                            {
                                Log.Debug(
                                    $"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds.");
                            }
                        }
                        else if (acceptedZones.Contains(ZoneType.Unspecified))
                        {
                            itemTracker.Add(pickup, Time.time + time);

                            // These types of calls get expensive, going to have branch logic first, then allocation of calls.
                            if (plugin.Config.Debug)
                            {
                                Log.Debug(
                                    $"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds with Unspecified marked as acceptable.");
                            }
                        }
                        else if (plugin.Config.Debug)
                        {
                            // Added this if user wants to see why item was not added. Again, condition of config file much
                            Log.Debug(
                                $"Could not add item {pickup.Type} because zones were not equal current {currentZone} vs accepted {string.Join(Environment.NewLine, acceptedZones)}");
                        }

                        return;
                    }

                    // We are going to assume that the user forgot to specify the zone. Therefore, the zone is unspecified.
                    if (plugin.Config.Debug)
                    {
                        Log.Debug(
                            $"Added a {pickup.Type} ({pickup.Serial}) to the tracker to be deleted in {time} seconds, defaulting to unspecified zone.");
                    }

                    itemTracker.Add(pickup, Time.time + time);
                }
            }
            catch (Exception ex)
            {
                Log.Debug($"Pickup.add failed because of {ex}");
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnRoundStarted()
        {
            itemTracker.Clear();
            if (cleanupItemsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(cleanupItemsCoroutine);
            }

            if (cleanupRagDollsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(cleanupRagDollsCoroutine);
            }

            if (plugin.Config.CleanupItems)
            {
                cleanupItemsCoroutine = Timing.RunCoroutine(CheckItems());
            }

            if (plugin.Config.CleanupRagDolls)
            {
                cleanupRagDollsCoroutine = Timing.RunCoroutine(CheckRagDolls());
            }
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            if (cleanupItemsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(cleanupItemsCoroutine);
            }

            if (cleanupRagDollsCoroutine.IsRunning)
            {
                Timing.KillCoroutines(cleanupRagDollsCoroutine);
            }
        }

        /// <summary>
        /// When a player changes role, we will ensure InPocket flag is removed.
        /// </summary>
        /// <param name="ev"> Event for changing role. </param>
        internal void OnRoleChange(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(plugin.Config.CheckInterval + 5, () =>
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
            Timing.CallDelayed(plugin.Config.CheckInterval + 5, () =>
            {
                ev?.Player?.SessionVariables.Remove("InPocket");
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
                yield return Timing.WaitForSeconds(plugin.Config.CheckInterval);
                if (itemTracker.IsEmpty())
                {
                    continue;
                }

                for (int i = 0; i < itemTracker.Count; i++)
                {
                    KeyValuePair<Pickup, float> item = itemTracker.ElementAt(i);
                    CheckItem(item.Key, item.Value);
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
                itemTracker.Remove(pickup);
                return;
            }

            if (pickup.InUse || Time.time < expirationTime)
            {
                return;
            }

            Log.Debug($"Deleting an item of type {pickup.Type} ({pickup.Serial}).");
            pickup.Destroy();
            itemTracker.Remove(pickup);
        }

        /// <summary>
        /// Coroutine to iteratively check all currently ragdolls to be removed within a zone/time limit.
        /// </summary>
        /// <returns> <see cref="IEnumerable{T}"/> which is used to determine how long this generator function should wait. </returns>
        private IEnumerator<float> CheckRagDolls()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(plugin.Config.CheckRagDollInterval);
                if (Map.Ragdolls.IsEmpty())
                {
                    continue;
                }

                for (int i = 0; i < Map.Ragdolls.Count; i++)
                {
                    Exiled.API.Features.Ragdoll curRagdoll = Map.Ragdolls.ElementAt(i);
                    CheckRagDoll(curRagdoll);
                }
            }
        }

        /// <summary>
        /// Verifies the ragdoll can be cleanup, and is not null.
        /// </summary>
        /// <param name="curRagdoll"> <see cref="Exiled.API.Features.Ragdoll"/> to potentially clean up. </param>
        private void CheckRagDoll(Exiled.API.Features.Ragdoll curRagdoll)
        {
            if (curRagdoll.Base is null)
            {
                return;
            }

            if (curRagdoll.Base.Info.ExistenceTime < plugin.Config.RagdollExistenceLimit)
            {
                return;
            }

            if (curRagdoll.Owner?.SessionVariables.TryGetValue("InPocket", out object inPocket) ?? false)
            {
                if ((bool)inPocket)
                {
                    Log.Debug($"Deleting a Radoll {curRagdoll} in pocket dimension");
                    curRagdoll.Delete();
                }

                return;
            }

            if (!plugin.Config.RagdollAcceptableZones.Contains(curRagdoll.Zone))
            {
                return;
            }

            Log.Debug($"Deleting a Radoll {curRagdoll} in zone {curRagdoll.Zone}");

            curRagdoll.Delete();
        }
    }
}