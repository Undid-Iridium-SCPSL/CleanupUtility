// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility
{
    using System;
    using Exiled.API.Features;
    using HarmonyLib;
    using ServerEvents = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config>
    {
        private Harmony harmony;

        /// <summary>
        /// Gets a static instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Author => "Undid-Iridium";

        /// <inheritdoc />
        public override string Name => "CleanupUtility";

        /// <inheritdoc />
        public override Version RequiredExiledVersion { get; } = new(5, 1, 3);

        /// <inheritdoc />
        public override Version Version { get; } = new(1, 1, 0);

        /// <summary>
        /// Gets an instance of the <see cref="PickupChecker"/> class.
        /// </summary>
        public PickupChecker PickupChecker { get; private set; }

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Instance = this;

            harmony = new Harmony($"com.Undid-Iridium.CleanupUtility.{DateTime.UtcNow.Ticks}");
            harmony.PatchAll();

            PickupChecker = new PickupChecker(this);
            ServerEvents.RoundStarted += PickupChecker.OnRoundStarted;
            ServerEvents.RestartingRound += PickupChecker.OnRestartingRound;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            ServerEvents.RoundStarted -= PickupChecker.OnRoundStarted;
            ServerEvents.RestartingRound -= PickupChecker.OnRestartingRound;
            PickupChecker = null;

            harmony.UnpatchAll(harmony.Id);
            harmony = null;

            Instance = null;
            base.OnDisabled();
        }
    }
}