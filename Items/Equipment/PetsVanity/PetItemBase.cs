using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity {
    public abstract class PetItemBase : ModItem {
        public abstract int ProjId { get; }
        public abstract int BuffId { get; }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.DefaultToVanitypet(ProjId, BuffId);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}