using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.ItemOverlays;
using AQMod.Common.Utilities;
using AQMod.Effects.Screen;
using AQMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Magic
{
    public class PagetFizzler : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
            if (!Main.dedServ)
                AQMod.ItemOverlays.Register(new GlowmaskOverlayData(AQUtils.GetPath(this) + "_Glow", new Color(128, 128, 128, 0)), item.type);
        }

        public override void SetDefaults()
        {
            item.damage = 22;
            item.magic = true;
            item.useTime = 10;
            item.useAnimation = 10;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Orange;
            item.shoot = ModContent.ProjectileType<UnstableBubble>();
            item.shootSpeed = 4f;
            item.mana = 5;
            item.autoReuse = true;
            item.UseSound = SoundID.Item8;
            item.value = AQItem.DemonSiegeWeaponValue;
            item.knockBack = 8f;
        }
    }
}