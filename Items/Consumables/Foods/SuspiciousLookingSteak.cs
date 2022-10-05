using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class SuspiciousLookingSteak : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            this.StaticDefaultsToFood(Color.Red, Color.DarkRed);
            ItemID.Sets.DrinkParticleColors[Type] = new Color[] { Color.Red, };
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item2;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 2);
            Item.buffType = BuffID.WellFed;
            Item.buffTime = 18000;
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.Regeneration, Item.buffTime);
            player.AddBuff(BuffID.NightOwl, Item.buffTime);
            player.AddBuff(BuffID.Ironskin, Item.buffTime);
            player.AddBuff(BuffID.Swiftness, Item.buffTime);
            player.AddBuff(BuffID.Gills, Item.buffTime);
            return true;
        }
    }
}