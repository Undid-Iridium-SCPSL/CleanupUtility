// -----------------------------------------------------------------------
// <copyright file="ServerCreatePickupPatch.cs" company="Undid-Iridium">
// Copyright (c) Undid-Iridium. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupUtility.Patches
{
#pragma warning disable SA1118
	using Exiled.API.Features.Items;
	using HarmonyLib;
	using InventorySystem;
	using NorthwoodLib.Pools;
	using System.Collections.Generic;
	using System.Reflection.Emit;
	using static HarmonyLib.AccessTools;

	/// <summary>
	/// Patches <see cref="InventoryExtensions.ServerCreatePickup"/> to add <see cref="Pickup"/>s to the <see cref="PickupChecker"/>.
	/// </summary>
	[HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
	internal static class ServerCreatePickupPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

			int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0);
			newInstructions.InsertRange(index, new[]
			{
                // Calls static instance
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Plugin), nameof(Plugin.Instance))),

                // Since the instance is now on the stack, we will call ProperttyGetter to get to PickupChecker object from our Instance object
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin), nameof(Plugin.PickupChecker))),

                // Load the variable unto Eval Stack [PickupChecker]
                new CodeInstruction(OpCodes.Ldloc_0),

                // Calls pickup method using local variable 0 and it gets back a Pickup object until EStack
                new CodeInstruction(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get))),

                // EStack variable used, [PickupChecker (Callvirt arg 0 (Instance)), Pickup (Arg 1 (Param))
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupChecker), nameof(PickupChecker.Add), new[] { typeof(Pickup) })),
			});

			for (int z = 0; z < newInstructions.Count; z++)
				yield return newInstructions[z];

			ListPool<CodeInstruction>.Shared.Return(newInstructions);
		}
	}
}