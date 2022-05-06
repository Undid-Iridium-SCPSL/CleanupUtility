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
        public Dictionary<ItemType, float> ItemFilter { get; set; } = new ()
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
            { ItemType.MicroHID, 600f },
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
        /// Gets or sets a collection of ItemTypes that should be deleted by Zone.
        /// </summary>
        [Description("Filter on what zone item type can be cleared from.")]
        public Dictionary<ItemType, HashSet<ZoneType>> ZoneFilter { get; set; } = new ()
        {
            { ItemType.KeycardJanitor, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardScientist, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardResearchCoordinator, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardZoneManager, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardGuard, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardNTFOfficer, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardContainmentEngineer, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardNTFLieutenant, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardNTFCommander, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardFacilityManager, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardChaosInsurgency, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardO5, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Radio, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunCOM15, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Medkit, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Flashlight, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.MicroHID, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP500, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP207, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Ammo12gauge, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunE11SR, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunCrossvec, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Ammo556x45, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunFSP9, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunLogicer, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GrenadeHE, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GrenadeFlash, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Ammo44cal, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Ammo762x39, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Ammo9x19, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunCOM18, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP018, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP268, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Adrenaline, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Painkillers, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Coin, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.ArmorLight, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.ArmorCombat, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.ArmorHeavy, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunRevolver, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunAK, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunShotgun, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP330, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP2176, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP1853, new HashSet<ZoneType>() { ZoneType.Unspecified } },
        };
    }
}