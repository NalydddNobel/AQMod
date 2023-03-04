using Aequus.Items;
using Aequus.Projectiles.Misc.Bobbers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Fishing.Poles
{
    [AutoloadGlowMask]
    public class Nimrod : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.WoodFishingPole);
            Item.value = Item.buyPrice(gold: 15);
            Item.fishingPole = 10;
            Item.shootSpeed = 16f;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<NimrodBobber>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<NimrodCloudBobber>();
        }
    }
}