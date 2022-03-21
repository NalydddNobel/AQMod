using AQMod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions.Foods
{
    public class GrapePhanta : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.Glowmask();
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 30;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 10;
            item.useTime = 10;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.Yellow;
            item.value = Item.buyPrice(silver: 20);
            item.buffType = ModContent.BuffType<GrapePhantaBuff>();
            item.buffTime = 28800;
        }
    }
}