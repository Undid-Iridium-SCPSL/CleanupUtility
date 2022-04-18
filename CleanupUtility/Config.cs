// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Interfaces;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug logs should be shown.
        /// </summary>
        [Description("Whether debug logs should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the time, in seconds, between each check of the list of items to delete.
        /// </summary>
        [Description("The time, in seconds, between each check of the list of items to delete.")]
        public float CheckInterval { get; set; } = 2f;

        /// <summary>
        /// Gets or sets a collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.
        /// </summary>
        [Description("A collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.")]
        public Dictionary<ItemType, float> ItemFilter { get; set; } = new()
        {
            { ItemType.KeycardJanitor, 600f },
            { ItemType.KeycardScientist, 600f },
            { ItemType.KeycardResearchCoordinator, 600f },
            { ItemType.KeycardZoneManager, 600f },
            { ItemType.KeycardGuard, 600f },
            { ItemType.KeycardNTFOfficer, 600f },
            { ItemType.KeycardContainmentEngineer, 600f },
            { ItemType.KeycardNTFLieutenant, 600f },
            { ItemType.KeycardNTFCommander, 600f },
            { ItemType.KeycardFacilityManager, 600f },
            { ItemType.KeycardChaosInsurgency, 600f },
            { ItemType.KeycardO5, 600f },
            { ItemType.Radio, 600f },
            { ItemType.GunCOM15, 600f },
            { ItemType.Medkit, 600f },
            { ItemType.Flashlight, 600f },
            { ItemType.SCP500, 600f },
            { ItemType.SCP207, 600f },
            { ItemType.Ammo12gauge, 600f },
            { ItemType.GunE11SR, 600f },
            { ItemType.GunCrossvec, 600f },
            { ItemType.Ammo556x45, 600f },
            { ItemType.GunFSP9, 600f },
            { ItemType.GunLogicer, 600f },
            { ItemType.GrenadeHE, 600f },
            { ItemType.GrenadeFlash, 600f },
            { ItemType.Ammo44cal, 600f },
            { ItemType.Ammo762x39, 600f },
            { ItemType.Ammo9x19, 600f },
            { ItemType.GunCOM18, 600f },
            { ItemType.SCP018, 600f },
            { ItemType.SCP268, 600f },
            { ItemType.Adrenaline, 600f },
            { ItemType.Painkillers, 600f },
            { ItemType.Coin, 600f },
            { ItemType.ArmorLight, 600f },
            { ItemType.ArmorCombat, 600f },
            { ItemType.ArmorHeavy, 600f },
            { ItemType.GunRevolver, 600f },
            { ItemType.GunAK, 600f },
            { ItemType.GunShotgun, 600f },
            { ItemType.SCP330, 600f },
            { ItemType.SCP2176, 600f },
            { ItemType.SCP1853, 600f },
        };
    }
}