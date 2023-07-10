using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class Mendshroom : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.ValueCrabCrevice;
            Item.shoot = ModContent.ProjectileType<MendshroomProj>();
        }
    }
}