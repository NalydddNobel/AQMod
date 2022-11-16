using Aequus.Buffs.Pets;
using Aequus.Items.GlobalItems;
using Aequus.Projectiles.Misc.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Pets
{
    public class SwagLookingEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            TooltipsGlobal.Dedicated.Add(Type, new TooltipsGlobal.ItemDedication(new Color(80, 60, 255, 255)));
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<TorraPet>(), ModContent.BuffType<TorraBuff>());
            Item.width = 20;
            Item.height = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }
    }
}