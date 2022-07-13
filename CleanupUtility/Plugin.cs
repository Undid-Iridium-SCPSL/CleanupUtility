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
        public override Version RequiredExiledVersion { get; } = new(5, 3, 0);

        /// <inheritdoc />
        public override Version Version { get; } = new(1, 2, 0);

        /// <summary>
        /// Gets an instance of the <see cref="PickupChecker"/> class.
        /// </summary>
        public PickupChecker PickupChecker { get; private set; }

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Instance = this;

            this.harmony = new Harmony($"com.Undid-Iridium.CleanupUtility.{DateTime.UtcNow.Ticks}");
            this.harmony.PatchAll();

            this.PickupChecker = new PickupChecker(this);
            ServerEvents.RoundStarted += this.PickupChecker.OnRoundStarted;
            ServerEvents.RestartingRound += this.PickupChecker.OnRestartingRound;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            ServerEvents.RoundStarted -= this.PickupChecker.OnRoundStarted;
            ServerEvents.RestartingRound -= this.PickupChecker.OnRestartingRound;

            this.PickupChecker = null;

            this.harmony.UnpatchAll(this.harmony.Id);
            this.harmony = null;

            Instance = null;
            base.OnDisabled();
        }
    }
}