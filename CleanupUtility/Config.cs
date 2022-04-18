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
        public float CheckInterval { get; set; } = 0.2f;

        /// <summary>
        /// Gets or sets the item filter.
        /// </summary>
        [Description("The item filter. If you want an item to be removed, add it here with the time in seconds.")]
        public Dictionary<ItemType, float> ItemFilter { get; set; } = new()
        {
            { ItemType.GrenadeHE, 15f },
        };
    }
}