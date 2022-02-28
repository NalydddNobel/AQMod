using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.Fishing
{
    public class CrabRod : ModItem
    {
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WoodFishingPole);
            item.fishingPole = 18;
            item.shootSpeed = 10f;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<Projectiles.Fishing.CrabBobber>();
        }
    }
}