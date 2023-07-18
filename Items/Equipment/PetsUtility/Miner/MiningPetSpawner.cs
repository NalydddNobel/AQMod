using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsUtility.Miner {
    public class MiningPetSpawner : ModItem {
        public override void SetDefaults() {
            Item.DefaultToVanitypet(ModContent.ProjectileType<MiningPet>(), ModContent.BuffType<MiningPetBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}