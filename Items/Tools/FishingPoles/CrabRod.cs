using Aequus.Content.Fishing;
using Aequus.Projectiles.Misc.Bobbers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.FishingPoles {
    public class CrabRod : FishingPoleItem {
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.WoodFishingPole);
            Item.fishingPole = 45;
            Item.shootSpeed = 24f;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(silver: 80);
            Item.shoot = ModContent.ProjectileType<CrabBobber>();
        }

        public override void ModifyDrawnFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) {
            lineOriginOffset = new(38f * Main.player[bobber.owner].direction, -34f);
            lineColor = new Color(255, 200, 200, 255);
        }
    }
}