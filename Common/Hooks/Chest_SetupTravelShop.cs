using Aequus.Content.Dedicated.BeyondCoin;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    public const int MaxTravellingMerchantSlotsAdded = 20;

    private void IL_Chest_SetupTravelShop(ILContext il) {
        ILCursor c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(typeof(NPC), nameof(NPC.peddlersSatchelWasUsed)))) {
            Mod.Logger.Error($"Could not find NPC.peddlersSatchelWasUsed ldsfld code."); return;
        }

        c.Emit(OpCodes.Ldloc_1);
        c.EmitDelegate((int numSlots) => {
            if (numSlots > MaxTravellingMerchantSlotsAdded) {
                return numSlots;
            }

            ShimmerCoinNPC.PeddlersSatchel(ref numSlots);

            return Math.Min(numSlots, MaxTravellingMerchantSlotsAdded);
        });
        c.Emit(OpCodes.Stloc_1);
    }
}
