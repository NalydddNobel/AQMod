using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Scepters {
    public abstract class ScepterBase : ModItem {
        public override void SetStaticDefaults() {
            Item.staff[Type] = true;
        }

        public override bool AllowPrefix(int pre) {
            return !AequusItem.CritOnlyModifier.Contains(pre);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            try {
                tooltips.RemoveCritChanceModifier();
            }
            catch {
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return player.altFunctionUse != 2;
        }
    }
}