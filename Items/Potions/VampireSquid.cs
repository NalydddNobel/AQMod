using AQMod.Buffs.Vampire;
using AQMod.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions
{
    public class VampireSquid : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 3);
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item2;
            item.useTime = 17;
            item.useAnimation = 17;
            item.buffTime = 10800;
            item.buffType = ModContent.BuffType<Vampirism>();
            item.maxStack = 30;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.GetModPlayer<VampirismPlayer>().IsVampire;
        }

        public override bool UseItem(Player player)
        {
            player.GetModPlayer<VampirismPlayer>().GiveVampirism(item.buffTime);
            return base.UseItem(player);
        }
    }
}