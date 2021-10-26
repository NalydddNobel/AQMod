using AQMod.Assets;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Common.WorldGeneration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee
{
    public class HellsBoon : ModItem
    {
        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new LegacyGlowmaskOverlayData(AQUtils.GetPath(this) + "_Glow", new Color(200, 200, 200, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 40;
            item.damage = 22;
            item.useTime = 38;
            item.useAnimation = 19;
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 1);
            item.melee = true;
            item.knockBack = 3f;
            item.shootSpeed = 35f;
            item.shoot = ModContent.ProjectileType<Projectiles.Melee.HellsBoon>();
            item.scale = 1.25f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return AQItem.GetAlphaDemonSiegeWeapon(lightColor);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectiles.Melee.HellsBoon.SpawnCluster(Main.MouseWorld, (int)(item.shootSpeed / player.meleeSpeed), damage, knockBack, player);
            return false;
        }
    }
}