// -----------------------------------------------------------------------
// <copyright file="ServerPickupPatch.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility.Patches
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem.Items;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemBase.ServerDropItem"/> to add <see cref="Pickup"/>s to the <see cref="PickupChecker"/>.
    /// </summary>
    [HarmonyPatch(typeof(ItemBase), nameof(ItemBase.ServerDropItem))]
    internal static class ServerPickupPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Plugin), nameof(Plugin.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin), nameof(Plugin.PickupChecker))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get))),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupChecker), nameof(PickupChecker.Add), new[] { typeof(Pickup) })),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}