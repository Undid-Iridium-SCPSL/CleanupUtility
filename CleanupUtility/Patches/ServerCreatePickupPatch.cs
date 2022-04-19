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
                // Load ItemBase to EStack
                new CodeInstruction(OpCodes.Ldarg_1),

                // Load EStack to callvirt and get owner back on Estack
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub)})),

                // Then get the player Zone
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Zone))),

                // Then save the player zone to a local variable (This is all done early because spawn deletes information and made it default to surface)
                new CodeInstruction(OpCodes.Stloc, itemZone.LocalIndex),

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



            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}