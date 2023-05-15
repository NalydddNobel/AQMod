using Aequus.Content.CrossMod;
using Aequus.Content.CrossMod.SplitSupport;
using Aequus.Items;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    [Obsolete("Slot Machines were removed.")]
    public class GoldenRoulette : SlotMachineBase {
        public override void ModifyItemLoot(ItemLoot itemLoot) {
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
                .AddRouletteItem<Bellows>()
                .AddRouletteItem<BoneHawkRing>()
                .AddOptions(chance: 4, ItemID.Extractinator, ModContent.ItemType<GlowCore>(), ModContent.ItemType<MiningPetSpawner>())
                .Add(ItemID.Torch, chance: 2, stack: (15, 50))
                .Add(ItemID.SilverCoin, chance: 1, stack: (50, 80));
            if (ThoriumMod.Instance != null) {
                if (ThoriumMod.Instance.TryFind("EnchantedStaff", out ModItem modItem))
                    builder.AddRouletteItem(modItem.Type);
                if (ThoriumMod.Instance.TryFind("EnchantedKnife", out modItem))
                    builder.AddSpecialRouletteItem(modItem.Type, modItem.Type, 1, 80, 100);
            }
            if (Split.Instance != null) {
                if (Split.Instance.TryFind("EnchantedRacquet", out ModItem modItem))
                    builder.AddRouletteItem(modItem.Type);
                if (Split.Instance.TryFind("BrightstoneChunk", out modItem))
                    builder.AddRouletteItem(modItem.Type);
            }
            ModifyItemLoot_AddCommonDrops(itemLoot);
        }
    }
}