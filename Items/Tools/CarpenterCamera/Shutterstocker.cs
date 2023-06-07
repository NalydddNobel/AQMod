using Aequus;
using Aequus.NPCs.Town.CarpenterNPC.Quest;
using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items.Tools.CarpenterCamera {
    public class Shutterstocker : ModItem {
        public override void SetStaticDefaults() {
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            AequusItem.HasWeaponCooldown.Add(Type);
        }

        public override void SetDefaults() {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<ShutterstockerHeldProj>();
            Item.shootSpeed = 1f;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player) {
            return !player.Aequus().HasCooldown;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (Item.buy || !Main.LocalPlayer.TryGetModPlayer<CarpenterBountyPlayer>(out var carpenter) || carpenter.SelectedBounty <= -1)
                return;

            tooltips.Add(new TooltipLine(Mod, $"BountyName", $"{CarpenterSystem.BountiesByID[carpenter.SelectedBounty].DisplayName}"));
            for (int i = 0; i < CarpenterSystem.BountiesByID[carpenter.SelectedBounty].steps.Count; i++) {
                var step = CarpenterSystem.BountiesByID[carpenter.SelectedBounty].steps[i];
                var stepText = step.GetStepText(CarpenterSystem.BountiesByID[carpenter.SelectedBounty]);
                if (string.IsNullOrEmpty(stepText))
                    continue;

                tooltips.Add(new TooltipLine(Mod, $"BountyTooltip{i}",
                    $"[X - {Language.GetTextValue(stepText)}"));
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
            return line.Mod != "Aequus" || !line.Name.StartsWith("Bounty");
        }

        public override void PostDrawTooltip(ReadOnlyCollection<DrawableTooltipLine> lines) {
            if (lines.Count == 0 || !Main.LocalPlayer.TryGetModPlayer<CarpenterBountyPlayer>(out var carpenter) || carpenter.SelectedBounty <= -1)
                return;
            int x = lines[0].X;
            int topY = int.MaxValue;
            int bottomY = 0;
            int width = 0;
            foreach (var l in lines) {
                if (l.Mod == "Aequus" && l.Name.StartsWith("Bounty")) {
                    x = Math.Min(l.X, x);
                    topY = Math.Min(l.Y, topY);
                    bottomY = Math.Max(l.Y, bottomY);
                    width = (int)Math.Max(FontAssets.MouseText.Value.MeasureString(l.Text).X, width);
                }
            }

            if (topY == int.MaxValue || bottomY == 0 || topY == bottomY || width == 0) {
                return;
            }
            Utils.DrawInvBG(Main.spriteBatch, new Rectangle(x - 10, topY - 6, width + 20, bottomY - topY + 34));
            bool[] completedSteps = new bool[CarpenterSystem.BountiesByID[carpenter.SelectedBounty].steps.Count];
            if (CarpenterBountyPlayer.LastPhotoTakenResults != null && CarpenterBountyPlayer.LastPhotoTakenResults.Count > 0) {
                for (int i = 0; i < CarpenterBountyPlayer.LastPhotoTakenResults.Count; i++) {
                    if (CarpenterBountyPlayer.LastPhotoTakenResults[i].success) {
                        completedSteps[i] = true;
                    }
                }
            }

            var icons = AequusTextures.ShutterstockerIcons;
            for (int i = 0; i < lines.Count; i++) {
                DrawableTooltipLine l = lines[i];
                if (l.Mod == "Aequus" && l.Name.StartsWith("Bounty")) {
                    ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, l.Text, new Vector2(l.X, l.Y), Color.White, 0f, Vector2.Zero, Vector2.One);
                    if (int.TryParse(l.Name[^1].ToString(), out int num) && completedSteps.IndexInRange(num)) {
                        if (completedSteps[num]) {
                            Main.spriteBatch.Draw(icons.Value, new Vector2(l.X, l.Y), icons.Value.Frame(verticalFrames: 3, frameY: 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        else {
                            Main.spriteBatch.Draw(icons.Value, new Vector2(l.X, l.Y), icons.Value.Frame(verticalFrames: 3, frameY: 0), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }
    }
}