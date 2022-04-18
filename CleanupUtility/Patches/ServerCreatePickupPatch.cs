// -----------------------------------------------------------------------
// <copyright file="ServerCreatePickupPatch.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility.Patches
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
#pragma warning disable SA1118
    using Exiled.API.Features.Items;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using NorthwoodLib.Pools;
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup"/> to add <see cref="Pickup"/>s to the <see cref="PickupChecker"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class ServerCreatePickupPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);


            LocalBuilder itemZone = generator.DeclareLocal(typeof(ZoneType));
            //Label skipLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + 1;

            newInstructions.InsertRange(index, new[]
            {
                // new CodeInstruction(OpCodes.Ldarg_0),
                // new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Inventory), nameof(Inventory.gameObject))),
                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh before get hub"),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                // new CodeInstruction(OpCodes.Call, Method(typeof(ReferenceHub), nameof(ReferenceHub.GetHub), new[] { typeof (UnityEngine.GameObject) })),

                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh after get hub"),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(UnityEngine.GameObject)})),
                // new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Zone))),
                // new CodeInstruction(OpCodes.Stloc, itemZone.LocalIndex),

                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh before get Ldarg_1"),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),

                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh before get hub"),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub)})),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Zone))),
                new CodeInstruction(OpCodes.Stloc, itemZone.LocalIndex),

                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh "),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh {0}"),
                // new CodeInstruction(OpCodes.Ldloc, itemZone.LocalIndex),
                // new CodeInstruction(OpCodes.Box, itemZone.LocalType),
                // new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object) })),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                // new CodeInstruction(OpCodes.Ldstr, "Woahhhh "),
                // new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0);
            newInstructions.InsertRange(index, new[]
            {

                  

                // Calls static instance
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Plugin), nameof(Plugin.Instance))),

                // Since the instance is now on the stack, we will call ProperttyGetter to get to PickupChecker object from our Instance object
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin), nameof(Plugin.PickupChecker))),

                /*
                 *      .maxstack 4
                        .locals init (
                            [0] class InventorySystem.Items.Pickups.ItemPickupBase,
                            [1] valuetype InventorySystem.Items.Pickups.PickupSyncInfo
                        )
                */

                // Load the variable unto Eval Stack [ItemPickupBase]
                new CodeInstruction(OpCodes.Ldloc_0),

                // Calls pickup method using local variable 0 (Which is on Eval Stack [ItemPickupBase]) and it gets back a Pickup object onto the EStack [Pickup]
                new CodeInstruction(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get))),

                // Calls arguemnt 1 from function call unto EStack (Zone)
                new CodeInstruction(OpCodes.Ldloc, itemZone.LocalIndex),

                // EStack variable used, [PickupChecker (Callvirt arg 0 (Instance)), Pickup (Arg 1 (Param))]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupChecker), nameof(PickupChecker.Add), new[] { typeof(Pickup), typeof(ZoneType)})),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            int count = 0;
            foreach (CodeInstruction instr in newInstructions)
            {
                Log.Info($"Current op code: {instr.opcode} and index {count}");
                count++;
            }


            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}