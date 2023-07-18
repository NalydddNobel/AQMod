using Aequus.Common.Utilities;
using Aequus.Items.Equipment.Accessories.Money.FaultyCoin;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus;

public partial class AequusPlayer {
    public long accFaultyCoinLoan;
    [SaveData("Debt")]
    public long accFaultyCoinDebt;
    public Item accFaultyCoinItem;
    private int _faultyCoinCheck;

    private void ResetEffects_FaultyCoin() {
        accFaultyCoinLoan = 0;
        accFaultyCoinItem = null;
    }

    private void PostUpdate_FaultyCoin() {

        if (accFaultyCoinItem == null) {
            _faultyCoinCheck = 0;
            return;
        }

        _faultyCoinCheck++;
        long loan = accFaultyCoinLoan - accFaultyCoinDebt;
        if (loan > 0) {
            Helper.DropMoney(Player.GetSource_Accessory(accFaultyCoinItem), Player.Hitbox, loan);
            accFaultyCoinDebt = accFaultyCoinLoan;
        }

        if (_faultyCoinCheck >= 90) {
            if (!Player.CanAfford((int)accFaultyCoinDebt)) {
                Player.AddBuff(ModContent.BuffType<FaultyCoinBuff>(), 120);
            }
            _faultyCoinCheck = 0;
        }
    }

    private static bool ItemSlot_OverrideLeftClick_FaultyCoin(Item[] inv, int context, int slot) {
        if (inv[slot].IsAir || inv[slot].ModItem is not FaultyCoin faultyCoin) {
            return false;
        }
        int realContext = Math.Abs(context);
        if (realContext == ItemSlot.Context.EquipAccessory || realContext == ItemSlot.Context.EquipAccessoryVanity) {
            long amount = Main.LocalPlayer.Aequus().accFaultyCoinDebt;
            if (amount <= 0) {
                return false;
            }

            if (!Main.LocalPlayer.CanAfford((int)amount)) {
                faultyCoin.OnUnsuccessfulRemove(Main.LocalPlayer);
                return true;
            }
            faultyCoin.OnRemoveAccessory(Main.LocalPlayer);
        }
        return false;
    }
}