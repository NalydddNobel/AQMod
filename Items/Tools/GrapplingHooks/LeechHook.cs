using Aequus.Projectiles.Misc.GrapplingHooks;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.GrapplingHooks
{
    public class LeechHook : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 12;
            Item.knockBack = 1f;
            Item.shoot = ModContent.ProjectileType<LeechHookProj>();
            Item.shootSpeed = 16f;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.value = ItemDefaults.BloodMimicItemValue;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            AequusTooltips.RemoveCritChance(tooltips);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.Aequus().leechHookNPC = -1;
            return true;
        }
    }
}