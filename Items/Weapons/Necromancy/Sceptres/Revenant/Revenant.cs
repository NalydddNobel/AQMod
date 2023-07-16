using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Revenant {
    [AutoloadGlowMask]
    public class Revenant : SceptreBase {
        public override Color GlowColor => Color.Blue;
        public override int DustSpawn => ModContent.DustType<RevenantParticle>();

        public override void SetDefaults() {
            Item.DefaultToNecromancy(10);
            Item.SetWeaponValues(10, 1f, 0);
            Item.shootSpeed = 2.5f;
            Item.shoot = ModContent.ProjectileType<RevenantSceptreProj>();
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.ValueDungeon;
            Item.mana = 6;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
        }
    }
}