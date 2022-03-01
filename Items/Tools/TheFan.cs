using AQMod.Common;
using AQMod.Content.World.Events;
using AQMod.Items.DrawOverlays;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Items.Tools
{
    public class TheFan : ModItem, IItemOverlaysWorldDraw, IItemOverlaysPlayerDraw
    {
        private static readonly GlowmaskOverlay _overlay = new GlowmaskOverlay(AQUtils.GetPath<TheFan>("_Glow"));
        IOverlayDrawWorld IItemOverlaysWorldDraw.WorldDraw => _overlay;
        IOverlayDrawPlayerUse IItemOverlaysPlayerDraw.PlayerDraw => _overlay;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.knockBack = 2f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 18;
            item.useAnimation = 18;
            item.UseSound = SoundID.Item39;
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<Projectiles.FriendlyWind>();
            item.shootSpeed = 9f;
            item.autoReuse = true;
            item.value = Item.buyPrice(gold: 50);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = AQItem.FindTTLineSpot(tooltips, "Tooltip#");
            tooltips.Insert(index - 1, new TooltipLine(mod, "Knockback", AQItem.KBTooltip(item.knockBack)));
            tooltips.Insert(index - 1, new TooltipLine(mod, "Speed", AQItem.UseAnimTooltip(item.useTime)));
            if (!Main.hardMode || WorldDefeats.DownedGaleStreams)
            {
                return;
            }
            try
            {
                tooltips.Insert(index + 2, new TooltipLine(mod, "StartsGaleStreams", Language.GetTextValue("Mods.AQMod.ItemTooltipExtra.TheFan.0")) { overrideColor = AQMod.MysteriousGuideTooltip, });
            }
            catch
            {
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (speedX < 0f)
            {
                if (Main.windSpeedSet > ImitatedWindyDay.MinWindSpeed)
                {
                    Main.windSpeedSet += -0.01f * (1f - speedY.Abs() / item.shootSpeed);
                }
            }
            else
            {
                if (Main.windSpeedSet < ImitatedWindyDay.MaxWindSpeed)
                {
                    Main.windSpeedSet += 0.01f * (1f - speedY.Abs() / item.shootSpeed);
                }
            }
            Main.windSpeedTemp = Main.windSpeedSet;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetHelper.UpdateWindSpeeds();
            Projectiles.FriendlyWind.NewWind(player, position, new Vector2(speedX, speedY), item.knockBack / 10f, 60, 80);
            return false;
        }
    }
}