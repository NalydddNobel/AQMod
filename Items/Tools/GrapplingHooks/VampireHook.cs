using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Projectiles.GrapplingHooks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Tools.GrapplingHooks
{
    public sealed class VampireHook : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.damage = 0;
            item.knockBack = 7f;
            item.shoot = ModContent.ProjectileType<VampireHookProj>();
            item.shootSpeed = 12f;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.useAnimation = 20;
            item.useTime = 20;
            item.rare = ItemRarityID.Yellow;
            item.noMelee = true;
            item.value = Item.sellPrice(gold: 5);
        }
    }
}