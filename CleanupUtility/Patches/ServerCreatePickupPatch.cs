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

#pragma warning disable SA1208 // System using directives should be placed before other using directives
    using System.Collections.Generic;
#pragma warning restore SA1208 // System using directives should be placed before other using directives
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

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + 1;

            Label skipLabel = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Load ItemBase to EStack
                new CodeInstruction(OpCodes.Ldarg_1),

                // Load EStack to callvirt and get owner back on Estack
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),

                // Duplicate ItemBase.Owner (if null then two nulls)
                new CodeInstruction(OpCodes.Dup),

                // If previous owner is null, escape this, still have one null on stack if that is the case
                new CodeInstruction(OpCodes.Brfalse_S, skipLabel),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Then get the player Zone
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Zone))),

                // Then save the player zone to a local variable (This is all done early because spawn deletes information and made it default to surface)
                new CodeInstruction(OpCodes.Stloc, itemZone.LocalIndex),

                // Continue without calling broken escape route
                new CodeInstruction(OpCodes.Br, continueProcessing),

                // Default value setting ZoneType to unspecified if previous owner is null by escaping to this label
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),

                // Remove current null from stack
                new CodeInstruction(OpCodes.Pop),

                // Assign unspecified enum
                new CodeInstruction(OpCodes.Ldc_I4_4),

                // Save that enum to ZoneType
                new CodeInstruction(OpCodes.Stloc, itemZone.LocalIndex),

                // Escape path for normal processing
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
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
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupChecker), nameof(PickupChecker.Add), new[] { typeof(Pickup), typeof(ZoneType) })),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            // int count = 0;
            // foreach (CodeInstruction instr in newInstructions)
            // {
            //    Log.Info($"Current op code: {instr.opcode} and index {count}");
            //    count++;
            // }
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}