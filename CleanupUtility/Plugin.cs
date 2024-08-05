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
    using PlayerEvents = Exiled.Events.Handlers.Player;
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
        public override Version RequiredExiledVersion { get; } = new(8, 0, 0);

        /// <inheritdoc />
        public override Version Version { get; } = new(2, 0, 5);

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
            if (this.Config.CleanInPocket)
            {
                PlayerEvents.EnteringPocketDimension += this.PickupChecker.OnPocketEnter;
                PlayerEvents.EscapingPocketDimension += this.PickupChecker.OnPocketExit;

                // On Died might be overkill but OnRoleChange is much more guaranteed.
                PlayerEvents.Died += this.PickupChecker.OnDied;
                PlayerEvents.ChangingRole += this.PickupChecker.OnRoleChange;
            }

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            ServerEvents.RoundStarted -= this.PickupChecker.OnRoundStarted;
            ServerEvents.RestartingRound -= this.PickupChecker.OnRestartingRound;

            if (this.Config.CleanInPocket)
            {
                PlayerEvents.EnteringPocketDimension -= this.PickupChecker.OnPocketEnter;
                PlayerEvents.EscapingPocketDimension -= this.PickupChecker.OnPocketExit;
                PlayerEvents.Died -= this.PickupChecker.OnDied;
                PlayerEvents.ChangingRole -= this.PickupChecker.OnRoleChange;
            }

            this.PickupChecker = null;

            this.harmony.UnpatchAll(this.harmony.Id);
            this.harmony = null;

            Instance = null;
            base.OnDisabled();
        }
    }
}
