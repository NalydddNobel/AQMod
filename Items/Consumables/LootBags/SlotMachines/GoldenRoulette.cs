using Aequus.Content.CrossMod;
using Aequus.Items.Accessories;
using Aequus.Items.Tools;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.SlotMachines
{
    public class GoldenRoulette : SlotMachineItemBase
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            var builder = this.CreateLoot(itemLoot);
            builder.AddRouletteItem(ItemID.BandofRegeneration)
                .AddRouletteItem(ItemID.MagicMirror)
                .AddRouletteItem(ItemID.CloudinaBottle)
                .AddRouletteItem(ItemID.HermesBoots)
                .AddRouletteItem(ItemID.LuckyHorseshoe)
                .AddRouletteItem(ItemID.ShoeSpikes)
                .AddRouletteItem(ItemID.Mace)
                .AddRouletteItem(ItemID.FlareGun)
                .AddSpecialRouletteItem(ItemID.Flare, ItemID.FlareGun, 1, 50, 80)
                .AddRouletteItem<BattleAxe>()
                .AddRouletteItem<Bellows>()
                .AddRouletteItem<BoneRing>()
                .AddOptions(chance: 4, ItemID.Extractinator, ModContent.ItemType<GlowCore>())
                .Add(ItemID.Torch, chance: 2, stack: (15, 50))
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80));
            if (ThoriumModSupport.ThoriumMod != null)
            {
                if (ThoriumModSupport.ThoriumMod.TryFind("EnchantedStaff", out ModItem modItem))
                    builder.AddRouletteItem(modItem.Type);
                if (ThoriumModSupport.ThoriumMod.TryFind("EnchantedKnife", out modItem))
                    builder.AddSpecialRouletteItem(modItem.Type, modItem.Type, 1, 80, 100);
            }
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}