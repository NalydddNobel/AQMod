using Aequus.Content.ItemPrefixes.Necromancy;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public abstract class SceptreBase : ModItem {
        public int HealAmount;

        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
        }

        public override void SetDefaults() {
            Item.DamageType = Aequus.NecromancyMagicClass;
        }

        public override bool AllowPrefix(int pre) {
            return PrefixLoader.GetPrefix(pre) is NecromancyPrefixBase;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return player.altFunctionUse != 2;
        }
    }
}