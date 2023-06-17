using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Aequus.Items.Vanity.Pets.SwagEye {
    public class TorraPet : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[Type] = 2;
            Main.projPet[Projectile.type] = true;
            this.SetTrail(12);
            ProjectileID.Sets.CharacterPreviewAnimations[Type] = new() { Offset = new(0f, -8f) };
        }

        public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.BabyEater);
            AIType = ProjectileID.BabyEater;
            Projectile.width = 32;
            Projectile.height = 32;
        }

        public override bool PreAI() {
            Projectile.localAI[0]--;
            if (Main.netMode != NetmodeID.Server) {
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (Main.player[i].chatOverhead.timeLeft > 0 && !string.IsNullOrEmpty(Main.player[i].chatOverhead.chatText) && Main.player[i].chatOverhead.chatText.ToLower().Contains("hi torra")) {
                        if (Main.player[i].chatOverhead.timeLeft == 300) {
                            string playerName = Main.player[i].name;
                            if (Main.myPlayer == i) {
                                playerName = Environment.UserName;
                            }
                            Main.NewText($"<Torra> OMG HI {playerName.ToUpper()}!!!!!!");
                            Projectile.localAI[0] = 400f;
                            Projectile.localAI[1] = i;
                        }
                    }
                }
            }

            Main.player[Projectile.owner].eater = false;
            if (!Helper.UpdateProjActive<TorraBuff>(Projectile)) {
                return false;
            }

            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] % 2 == 0)
                return true;

            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X.UnNaN());
            return false;
        }

        public override void AI() {
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X.UnNaN());
        }

        public override bool PreDraw(ref Color lightColor) {
            var drawCoords = Projectile.Center;

            Main.instance.LoadProjectile(ProjectileID.Blizzard);
            var texture = TextureAssets.Projectile[ProjectileID.Blizzard].Value;
            float icicleDistance = !Projectile.isAPreviewDummy ? 80f : 48f;
            for (int i = 0; i < 6; i++) {
                var rotation = i + MathHelper.TwoPi / 6f + Main.GameUpdateCount * 0.05f;
                var frame = texture.Frame(verticalFrames: 5, frameY: (Projectile.identity + i) % 5);
                var icicleDrawCoords = drawCoords + rotation.ToRotationVector2() * icicleDistance * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f + i * MathHelper.Pi / 3f, 0.8f, 1f) * Projectile.scale;
                Main.EntitySpriteDraw(texture, icicleDrawCoords - Main.screenPosition, frame, Helper.GetColor(icicleDrawCoords), rotation - MathHelper.PiOver2, new Vector2(frame.Width / 2f, frame.Height - 6), Projectile.scale, SpriteEffects.None, 0);
            }

            if (Projectile.isAPreviewDummy) {
                Projectile.rotation = -MathHelper.PiOver2;
            }
            else {
                if (Projectile.localAI[0] > 0f) {
                    int i = (int)Projectile.localAI[1];
                    string playerName = Main.player[i].name;
                    if (Main.myPlayer == i) {
                        playerName = Environment.UserName;
                    }
                    string text = $"OMG HI {playerName.ToUpper()}!!!!!!";
                    var font = FontAssets.MouseText.Value;
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, Projectile.Center + new Vector2(0f, -Projectile.height) - Main.screenPosition,
                        new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, 255), 0f, new Vector2(font.MeasureString(text).X / 2f, 0f), Vector2.One * Main.UIScale);
                }
            }
            return true;
        }
    }
}