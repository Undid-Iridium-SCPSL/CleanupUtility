using CleanupUtility.Events;
using CleanupUtility.Utility;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms.Ammo;
using NorthwoodLib.Pools;
using System.Collections.Generic;
using System.Reflection.Emit;

using static HarmonyLib.AccessTools;


namespace CleanupUtility.Patches
{
	/// <summary>
	/// If you wonder why I did not patch ServerCreatePickup it is because information can be lost in that scenario.
	/// These functions add a little more information, AND is done after a lot of networking has been issued. This means
	/// there is a higher chance this will occur later than the minmium safe time required
	/// Patches <see cref="InventoryExtensions.ServerDropAmmo"/> to help manage <see cref="Player.Items"/>.
	/// </summary>
	[HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
	internal static class ServerDropAmmoPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
			ILGenerator generator)
		{
			List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
			const int offset = 0;//Inserts AT location, not before
			int count = 0;
			//foreach (CodeInstruction instr in newInstructions)
			//{
			//	Log.Info($"Current op code: {instr.opcode} and index {count}");
			//	count++;
			//}
			count = 0;
			int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldfld) + offset;
			Log.Info($"What was index {index}");
			//LocalBuilder playerPickup = generator.DeclareLocal(typeof(Pickup));
			//Label skipLabel = generator.DefineLabel();
			//Label continueProcessing = generator.DefineLabel();
			LocalBuilder playerPickup = generator.DeclareLocal(typeof(Pickup));
			LocalBuilder ammoPickup = generator.DeclareLocal(typeof(AmmoPickup));
			Label skipLabel = generator.DefineLabel();

			Label retLabel = generator.DefineLabel();

			Label continueProcessing = generator.DefineLabel();
			newInstructions.InsertRange(index, new[]
			{
				//This could be reduced to a DUP but I rather have access to the variable code wise. v
				new CodeInstruction(OpCodes.Stloc, ammoPickup.LocalIndex),
				//Stores EStack value in LdLoc
				new CodeInstruction(OpCodes.Ldloc, ammoPickup.LocalIndex),

				//new CodeInstruction(OpCodes.Castclass, playerPickup),
				//Loads the Pickup.Get call with our last variable on EStack (LdLoc1)
				new CodeInstruction(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof (AmmoPickup)})),
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
				new CodeInstruction(OpCodes.Callvirt, Method(typeof(PickupTrackingQueue<Pickup>), nameof(PickupTrackingQueue<Pickup>.Enqueue), new[] { typeof(Pickup) })),
				//Branch to escape to not print below logic
				new CodeInstruction(OpCodes.Br, continueProcessing),


				new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel), //This is where we marked our print label to be 
				new CodeInstruction(OpCodes.Ldstr, "ServerDropAmmo patch was not able to function because a variable was null or missing"), //Our string we need added to the EStack
				new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string)})), //Our call to the method utilizing the EStack string

				new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),


				//Need to put back our variable we stole. 
				new CodeInstruction(OpCodes.Ldloc, ammoPickup.LocalIndex),

			});




			for (int z = 0; z < newInstructions.Count; z++)
				yield return newInstructions[z];



			ListPool<CodeInstruction>.Shared.Return(newInstructions);
		}
	}
}
