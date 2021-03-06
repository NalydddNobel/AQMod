using Aequus.Projectiles.Misc.Bobbers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.FishingRods
{
    [GlowMask]
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
            Item.fishingPole = 20;
            Item.shootSpeed = 16f;
            Item.rare = ItemRarityID.LightPurple;
            Item.shoot = ModContent.ProjectileType<NimrodBobber>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<NimrodCloudBobber>();
        }
    }
}