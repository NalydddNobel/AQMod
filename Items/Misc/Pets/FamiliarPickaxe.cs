using Aequus.Buffs.Pets;
using Aequus.Projectiles.Misc.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Pets
{
    public class FamiliarPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);

            AequusTooltips.Dedicated[Type] = new AequusTooltips.ItemDedication(new Color(200, 65, 70, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<FamiliarPet>(), ModContent.BuffType<FamiliarBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 20);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}