// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Enums;
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

        /// <summary>
        /// Gets or sets a collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.
        /// </summary>
        [Description("A collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.")]
        public Dictionary<ItemType, ZoneType> ZoneFilter { get; set; } = new()
        {
            { ItemType.KeycardJanitor, ZoneType.Unspecified },
            { ItemType.KeycardScientist, ZoneType.Unspecified },
            { ItemType.KeycardResearchCoordinator, ZoneType.Unspecified },
            { ItemType.KeycardZoneManager, ZoneType.Unspecified },
            { ItemType.KeycardGuard, ZoneType.Unspecified },
            { ItemType.KeycardNTFOfficer, ZoneType.Unspecified },
            { ItemType.KeycardContainmentEngineer, ZoneType.Unspecified },
            { ItemType.KeycardNTFLieutenant, ZoneType.Unspecified },
            { ItemType.KeycardNTFCommander, ZoneType.Unspecified },
            { ItemType.KeycardFacilityManager, ZoneType.Unspecified },
            { ItemType.KeycardChaosInsurgency, ZoneType.Unspecified },
            { ItemType.KeycardO5, ZoneType.Unspecified },
            { ItemType.Radio, ZoneType.Unspecified },
            { ItemType.GunCOM15, ZoneType.Unspecified },
            { ItemType.Medkit, ZoneType.Unspecified },
            { ItemType.Flashlight, ZoneType.Unspecified },
            { ItemType.SCP500, ZoneType.Unspecified },
            { ItemType.SCP207, ZoneType.Unspecified },
            { ItemType.Ammo12gauge, ZoneType.Unspecified },
            { ItemType.GunE11SR, ZoneType.Unspecified },
            { ItemType.GunCrossvec, ZoneType.Unspecified },
            { ItemType.Ammo556x45, ZoneType.Unspecified },
            { ItemType.GunFSP9, ZoneType.Unspecified },
            { ItemType.GunLogicer, ZoneType.Unspecified },
            { ItemType.GrenadeHE, ZoneType.Unspecified },
            { ItemType.GrenadeFlash, ZoneType.Unspecified },
            { ItemType.Ammo44cal, ZoneType.Unspecified },
            { ItemType.Ammo762x39, ZoneType.Unspecified },
            { ItemType.Ammo9x19, ZoneType.Unspecified },
            { ItemType.GunCOM18, ZoneType.Unspecified },
            { ItemType.SCP018, ZoneType.Unspecified },
            { ItemType.SCP268, ZoneType.Unspecified },
            { ItemType.Adrenaline, ZoneType.Unspecified },
            { ItemType.Painkillers, ZoneType.Unspecified },
            { ItemType.Coin, ZoneType.Unspecified },
            { ItemType.ArmorLight, ZoneType.Unspecified },
            { ItemType.ArmorCombat, ZoneType.Unspecified },
            { ItemType.ArmorHeavy, ZoneType.Unspecified },
            { ItemType.GunRevolver, ZoneType.Unspecified },
            { ItemType.GunAK, ZoneType.Unspecified },
            { ItemType.GunShotgun, ZoneType.Unspecified },
            { ItemType.SCP330, ZoneType.Unspecified },
            { ItemType.SCP2176, ZoneType.Unspecified },
            { ItemType.SCP1853, ZoneType.Unspecified },
        };
    }
}