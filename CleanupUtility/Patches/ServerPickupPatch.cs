using CleanupUtility.Events;
using CleanupUtility.Utility;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using HarmonyLib;
using InventorySystem.Items;
using NorthwoodLib.Pools;
using System.Collections.Generic;
using System.Reflection.Emit;
using static HarmonyLib.AccessTools;

namespace CleanupUtility.Patches
{
	/// <summary>
	/// Patches <see cref="InventoryExtensions.ServerDropItem"/> to help manage <see cref="Player.Items"/>.
	/// </summary>
	[HarmonyPatch(typeof(ItemBase), nameof(ItemBase.ServerDropItem))]
	internal static class ServerPickupPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator generator)
		{
			List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
			const int offset = 1;//Inserts AT location, not before
			int count = 0;
			//foreach (CodeInstruction instr in newInstructions)
			//{
			//	Log.Info($"Current op code: {instr.opcode} and index {count}");
			//	count++;
			//}
			count = 0;
			int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stfld) + offset;
			//Log.Info($"What was index {index}");
			LocalBuilder playerPickup = generator.DeclareLocal(typeof(Pickup));
			Label skipLabel = generator.DefineLabel();
			Label continueProcessing = generator.DefineLabel();
			//newInstructions.InsertRange(index, new[]
			//{
			//	//new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
			//	new  CodeInstruction(OpCodes.Ldstr, "ChangingGroupEvent was not allowed"), //Our string we need added to the EStack
			//	new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string)})), //Our call to the method utilizing the EStack string

			//});
			Label retLabel = generator.DefineLabel();

			newInstructions.InsertRange(index, new[]
			{
				
				//Loads our current ItemPickupBase
				new CodeInstruction(OpCodes.Ldloc_1),
				//Duplicate to keep our itemPickupBase
				//new CodeInstruction(OpCodes.Dup),
				//itemPickupBase Stack [itemPickupBase, itemPickupBase] 
				//new CodeInstruction(OpCodes.Ldloc_1),
				//Pickup.get(itemPickupBase) Eval Stack itemPickupBase

				//Loads the Pickup.Get call with our last variable on EStack (LdLoc1)
				new CodeInstruction(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get))),
				//Saves result in EStack 
				new CodeInstruction(OpCodes.Stloc, playerPickup.LocalIndex),
				//Stores EStack value in LdLoc
				new CodeInstruction(OpCodes.Ldloc, playerPickup.LocalIndex),
				//Stores LdLoc back in EStack
				new CodeInstruction(OpCodes.Brfalse, skipLabel),
				//If it were null we go out, otherwise, continue



				
			
				//EventHandler.PickupChecker, Load static field
				new CodeInstruction(OpCodes.Ldsfld, Field(typeof(EventHandler), nameof(EventHandler.pickupChecker))),
				//EventHandler.PickupChecker.itemTrackingQueue, Load field using static
				new CodeInstruction(OpCodes.Ldfld, Field(typeof(PickupChecker), nameof(PickupChecker.itemTrackingQueue))),
				////Accessible EventHandler.PickupChecker.itemTrackingQueue, Load string to 
				//new  CodeInstruction(OpCodes.Ldstr, "We were able to pass this point because it was not null"), //Our string we need added to the EStack
				//new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string)})), //Our call to the method utilizing the EStack string

				//Load local variable to EStack
				new CodeInstruction(OpCodes.Ldloc, playerPickup.LocalIndex),
				//Call Enque using last variable on EStack, and last function result from Estack [Ldfld tracking queue, Ldloc pickup object)
				new CodeInstruction(OpCodes.Callvirt, Method(typeof(ItemCappedQueue<Pickup>), nameof(ItemCappedQueue<Pickup>.Enqueue), new[] { typeof(Pickup) })),
				//Branch to escape to not print below logic
				new CodeInstruction(OpCodes.Br, retLabel),
				//new CodeInstruction(OpCodes.Ldloc, playerPickup.LocalIndex),
				//// Stack [itemTrackingQueue, Pickup]
				//new CodeInstruction(OpCodes.Callvirt, Method(typeof(ItemCappedQueue<Pickup>), nameof(ItemCappedQueue<Pickup>))),
				//new CodeInstruction(OpCodes.Ldloc_1),

				new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel), //This is where we marked our print label to be 
				new CodeInstruction(OpCodes.Ldstr, "Server pickup patch was not able to function because a variable was null or missing"), //Our string we need added to the EStack
				new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string)})), //Our call to the method utilizing the EStack string
			});



			//Add an escape route to continue normal logic 
			newInstructions[newInstructions.Count - 2].labels.Add(retLabel);

			for (int z = 0; z < newInstructions.Count; z++)
				yield return newInstructions[z];

			//foreach (CodeInstruction instr in newInstructions)
			//{
			//	Log.Info($"Current op code: {instr.opcode} and index {count}");
			//	count++;
			//}

			ListPool<CodeInstruction>.Shared.Return(newInstructions);
		}
	}
}
