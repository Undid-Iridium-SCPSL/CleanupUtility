// -----------------------------------------------------------------------
// <copyright file="ServerCreatePickupPatch.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using NorthwoodLib.Pools;
using UnityEngine;

namespace CleanupUtility.Patches
{
    using static AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup"/> to add <see cref="Pickup"/>s to the <see cref="PickupChecker"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup),
        typeof(ItemBase), typeof(PickupSyncInfo), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Action<ItemPickupBase>))]
    internal static class ServerCreatePickupPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder itemZone = generator.DeclareLocal(typeof(ZoneType));

            LocalBuilder curPlayer = generator.DeclareLocal(typeof(Player));

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldarg_0);

            Label skipLabel = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();
            Label skipException = generator.DefineLabel();

            LocalBuilder exceptionObject = generator.DeclareLocal(typeof(Exception));

            // Our Catch (Try wrapper) block
            ExceptionBlock catchBlock = new(ExceptionBlockType.BeginCatchBlock, typeof(Exception));

            // Our Exception handling start
            ExceptionBlock exceptionStart = new(ExceptionBlockType.BeginExceptionBlock, typeof(Exception));

            // Our Exception handling end
            ExceptionBlock exceptionEnd = new(ExceptionBlockType.EndExceptionBlock);

            List<CodeInstruction> instructionToAdd = new()
            {
                // Load ItemBase to EStack
                new CodeInstruction(OpCodes.Ldarg_0).WithBlocks(exceptionStart).MoveLabelsFrom(newInstructions[index]),

                // Assign unspecified enum as the default value, and then try to load the actual value
                new(OpCodes.Ldc_I4_4),

                // Then save the player zone to a local variable (This is all done early because spawn deletes information and made it default to surface)
                new(OpCodes.Stloc, itemZone.LocalIndex),
                
                //Load the current Inventory (Itembase -> (get) Inventory
                new(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.OwnerInventory))),

                // Callvirt of ItemBase -> (now) Inventory to get gameobject
                new(OpCodes.Callvirt, PropertyGetter(typeof(Inventory), nameof(Inventory.gameObject))),

                //Get current reference hub
                new(OpCodes.Call, Method(typeof(ReferenceHub), nameof(ReferenceHub.GetHub), new[] { typeof(GameObject) })),

                // Duplicate ItemBase.Owner (if null then two nulls)
                new(OpCodes.Dup),

                // If previous owner is null, escape this, still have one null on stack if that is the case
                new(OpCodes.Brfalse_S, skipLabel),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back, OK game object could be null
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Duplicate player to be copied
                new(OpCodes.Dup),

                // Store player to local var
                new(OpCodes.Stloc, curPlayer.LocalIndex),

                // Then get the player Zone (This was probably null)
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Zone))),

                // Continue without calling broken escape route
                new(OpCodes.Br_S, continueProcessing),

                // Remove current null from stack. Default value setting ZoneType to unspecified if previous owner is null by escaping to this label
                new CodeInstruction(OpCodes.Pop).WithLabels(skipLabel),

                // Then save the player zone to a local variable (This is all done early because spawn deletes information and made it default to surface)
                new CodeInstruction(OpCodes.Stloc, itemZone.LocalIndex).WithLabels(continueProcessing),

                // Correctly esacpes try-catch block.
                new(OpCodes.Leave_S, skipException),

                // Load the exception from stack
                new CodeInstruction(OpCodes.Nop).WithBlocks(catchBlock).WithBlocks(exceptionEnd),
            };

            // Add our previous instructions
            newInstructions.InsertRange(index, instructionToAdd);

            // Add our skip at end
            newInstructions[index + instructionToAdd.Count].WithLabels(skipException);

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
                new CodeInstruction(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),
            
                // Calls arguemnt 1 from function call unto EStack (Zone)
                new CodeInstruction(OpCodes.Ldloc, itemZone.LocalIndex),
            
                // Calls arguemnt 1 from function call unto EStack (Zone)
                new CodeInstruction(OpCodes.Ldloc, curPlayer.LocalIndex),
            
                // EStack variable used, [PickupChecker (Callvirt arg 0 (Instance)), Pickup (Arg 1 (Param))]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupChecker), nameof(PickupChecker.Add), new[] { typeof(Pickup), typeof(ZoneType), typeof(Player) })),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}