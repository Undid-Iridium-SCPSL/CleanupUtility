namespace CleanupUtility.Patches
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading.Tasks;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Armor;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerDropAmmo(Inventory, ItemType, ushort, bool)"/> to add <see cref="ReferenceHub"/>s to the <see cref="ItemBase"/> for ammo.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropAmmo))]
    internal static class RemoveEverythingExceedingLimitsPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder itemOwner = generator.DeclareLocal(typeof(ReferenceHub));

            Label nullObject = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction =>
            instruction.StoresField(typeof(Inventory).GetField("SendAmmoNextFrame", BindingFlags.Public | BindingFlags.Instance)));

            newInstructions.InsertRange(index, new[]
            {

                new CodeInstruction(OpCodes.Ldarg_0),
                // No getter method meaning we can do field
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Brfalse, nullObject),
                // Has getter method, must call through wrapper
                //new CodeInstruction(OpCodes.Ldstr, "Woahhhh {0}"),
                //new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.playerId))),
                //new CodeInstruction(OpCodes.Box, typeof(int)),
                //new CodeInstruction(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(int) })),
                //new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                // Then save the player zone to a local variable (This is all done early because spawn deletes information and made it default to surface)
                new CodeInstruction(OpCodes.Stloc, itemOwner.LocalIndex),

                new CodeInstruction(OpCodes.Br, continueProcessing),

                new CodeInstruction(OpCodes.Nop).WithLabels(nullObject),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldstr, "Well, our stupid reference hub was null"),
                new CodeInstruction(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            int loadOffset = -4;
            index = newInstructions.FindLastIndex(instruction =>
            instruction.opcode == OpCodes.Isinst) + loadOffset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Ldloc, itemOwner.LocalIndex),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(Inventory), nameof(Inventory._hub))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            Log.Info($"What was our index {index}");

            int count = 0;
            foreach (CodeInstruction instr in newInstructions)
            {
                Log.Info($"Current op code: {instr.opcode} and index {count}");
                count++;
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
