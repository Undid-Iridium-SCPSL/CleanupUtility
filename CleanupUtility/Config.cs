﻿// -----------------------------------------------------------------------
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
        /// Gets or sets a value indicating whether items should be cleaned up.
        /// </summary>
        [Description("Gets or sets a value indicating whether items should be cleaned up.")]
        public bool CleanupItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether ragdolls should be cleaned up.
        /// </summary>
        [Description("Gets or sets a value indicating whether ragdolls should be cleaned up.")]
        public bool CleanupRagDolls { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets value indicating whether to clean in pocket dimension.
        /// </summary>
        [Description("Gets or sets a value indicating whether to clean in pocket dimension.")]
        public bool CleanInPocket { get; set; } = false;

        /// <summary>
        /// Gets or sets the time, in seconds, between each check of the list of items to delete.
        /// </summary>
        [Description("The time, in seconds, between each check of the list of items to delete.")]
        public float CheckInterval { get; set; } = 2f;

        /// <summary>
        /// Gets or sets the time, in seconds, between each check of the list of ragdolls to delete.
        /// </summary>
        [Description("The time, in seconds, between each check of the list of ragdolls to delete.")]
        public float CheckRagDollInterval { get; set; } = 2f;

        /// <summary>
        /// Gets or sets a collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.
        /// </summary>
        [Description("A collection of items that should be deleted paired with the time, in seconds, to wait before deleting them.")]
        public Dictionary<ItemType, float> ItemFilter { get; set; } = new()
        {
            { ItemType.None, 600f },
            { ItemType.KeycardJanitor, 600f },
            { ItemType.KeycardScientist, 600f },
            { ItemType.KeycardResearchCoordinator, 600f },
            { ItemType.KeycardZoneManager, 600f },
            { ItemType.KeycardGuard, 600f },
            { ItemType.KeycardMTFPrivate, 600f },
            { ItemType.KeycardContainmentEngineer, 600f },
            { ItemType.KeycardMTFOperative, 600f },
            { ItemType.KeycardMTFCaptain, 600f },
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
            { ItemType.SCP244a, 600f },
            { ItemType.SCP244b, 600f },
            { ItemType.SCP1853, 600f },
            { ItemType.ParticleDisruptor, 600f },
            { ItemType.GunCom45, 600f },
            { ItemType.SCP1576, 600f },
            { ItemType.Jailbird, 600f },
            { ItemType.AntiSCP207, 600f },
            { ItemType.GunFRMG0, 600f },
            { ItemType.GunA7, 600f },
            { ItemType.Lantern, 600f }
        };

        /// <summary>
        /// Gets or sets a collection of ItemTypes that should be deleted by Zone.
        /// </summary>
        [Description("Filter on what zone item type can be cleared from.")]
        public Dictionary<ItemType, HashSet<ZoneType>> ZoneFilter { get; set; } = new()
        {
            { ItemType.None, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardJanitor, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardScientist, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardResearchCoordinator, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardZoneManager, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardGuard, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardMTFPrivate, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardContainmentEngineer, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardMTFOperative, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.KeycardMTFCaptain, new HashSet<ZoneType>() { ZoneType.Unspecified } },
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
            { ItemType.SCP244a, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP244b, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP1853, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.ParticleDisruptor, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunCom45, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.SCP1576, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Jailbird, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.AntiSCP207, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunFRMG0, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.GunA7, new HashSet<ZoneType>() { ZoneType.Unspecified } },
            { ItemType.Lantern, new HashSet<ZoneType>() { ZoneType.Unspecified } }
        };

        /// <summary>
        /// Gets or sets existence time limit for ragdolls.
        /// </summary>
        [Description("Time limit for ragdoll existence.")]
        public float RagdollExistenceLimit { get; set; } = 10;

        /// <summary>
        /// Gets or sets a acceptable cleanup Zones.
        /// </summary>
        [Description("Filter on what zone ragdolls can be cleared from.")]
        public HashSet<ZoneType> RagdollAcceptableZones { get; set; } = new HashSet<ZoneType>()
        {
            ZoneType.Unspecified,
            ZoneType.Surface,
            ZoneType.Entrance,
            ZoneType.HeavyContainment,
            ZoneType.LightContainment,
            ZoneType.Other,
        };
    }
}