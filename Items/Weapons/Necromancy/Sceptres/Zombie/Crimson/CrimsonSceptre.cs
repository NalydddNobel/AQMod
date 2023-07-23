using Aequus.Common;
using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Crimson {
    [AutoloadGlowMask]
    [WorkInProgress]
    public class CrimsonSceptre : SceptreBase {
        public override Color GlowColor => Color.Red;
        public override int DustSpawn => ModContent.DustType<CrimsonSceptreParticle>();

        public override void SetDefaults() {
            base.SetDefaults();
            Item.DefaultToNecromancy(11);
            Item.SetWeaponValues(5, 1f, 0);
            Item.shootSpeed = 2.2f;
            Item.shoot = ModContent.ProjectileType<CrimsonSceptreProj>();
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 6;
            Item.autoReuse = true;
        }

        public override void AddRecipes() {
#if DEBUG
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 6)
                .AddIngredient(ItemID.LifeCrystal)
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.RainbowRod);
#endif
        }
    }
}