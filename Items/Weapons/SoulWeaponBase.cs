using Aequus.Items.GlobalItems;
using Aequus.Items.Prefixes.Soul;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Aequus.Items.Weapons
{
    public abstract class SoulWeaponBase : ModItem, ItemHooks.IUpdateVoidBag
    {
        public int OriginalSoulLimit { get; protected set; }
        public int OriginalSoulCost { get; protected set; }

        public int soulLimit;
        public int soulCost;

        public void ClearSoulFields()
        {
            soulLimit = OriginalSoulLimit;
            soulCost = OriginalSoulCost;
        }

        public virtual void UpdatePlayerSoulLimit(Player player)
        {
            var aequus = player.Aequus();
            aequus.soulLimit = Math.Max(aequus.soulLimit, soulLimit);
        }

        public override void HoldItem(Player player)
        {
            UpdatePlayerSoulLimit(player);
        }

        public override void UpdateInventory(Player player)
        {
            UpdatePlayerSoulLimit(player);
        }

        public virtual void UpdateBank(Player player, AequusPlayer aequus, int slot, int bank)
        {
            UpdatePlayerSoulLimit(player);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int i = tooltips.GetIndex("UseMana");
            tooltips.Insert(i, new TooltipLine(Mod, "CarryingSouls", AequusText.GetText("ItemTooltip.Common.CarryingSouls", Main.LocalPlayer.Aequus().candleSouls, soulLimit)));
            tooltips.Insert(i, new TooltipLine(Mod, "UseSouls", AequusText.GetText("ItemTooltip.Common.UseSouls", soulCost)));

            TooltipsGlobalItem.PercentageModifier(soulCost, OriginalSoulCost, "PrefixSoulCost", tooltips, higherIsGood: false);
            TooltipsGlobalItem.PercentageModifier(soulLimit, OriginalSoulLimit, "PrefixSoulLimit", tooltips, higherIsGood: true);
        }

        public override bool CanUseItem(Player player)
        {
            return player.Aequus().candleSouls >= soulCost;
        }

        public override int ChoosePrefix(UnifiedRandom rand)
        {
            return SoulWeaponPrefix.ChoosePrefix(Item, rand).Type;
        }
    }
}