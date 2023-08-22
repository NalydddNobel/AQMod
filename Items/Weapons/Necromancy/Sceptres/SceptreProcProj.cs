using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres {
    public class SceptreProcProj : ModProjectile {
        public override string Texture => AequusTextures.ZombieSceptre.Path;

        public override void SetDefaults() {
            Projectile.DisableWorldInteractions();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 30;
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            player.itemTime = 2;
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.Center;
            if (player.direction == -1) {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, 2.5f);
            }
            else {
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -2.5f);
            }
            if (Projectile.timeLeft == 10) {
                for (int i = 0; i < Main.maxNPCs; i++) {
                    var npc = Main.npc[i];
                    if (!npc.active || !npc.playerInteraction[Projectile.owner] || !npc.TryGetAequus(out var aequusNPC) || aequusNPC.soulHealthTotal <= 0 || player.Distance(npc) > 2000f) {
                        continue;
                    }

                    if (Main.myPlayer == Projectile.owner) {
                        Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, Main.rand.NextVector2Unit() * 2f, ProjectileID.SpectreWrath, aequusNPC.soulHealthTotal * 2, 8f, Projectile.owner);
                    }
                    aequusNPC.soulHealthTotal = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var player = Main.player[Projectile.owner];
            var scepterItem = player.HeldItemFixed();
            Projectile.hide = true;
            if (scepterItem?.ModItem is not SceptreBase scepterModItem) {
                return false;
            }
            Helper.GetItemDrawData(scepterItem.type, out var frame);
            Main.instance.LoadItem(scepterItem.type);
            float animation = 1f - Projectile.timeLeft / 30f;
            var texture = TextureAssets.Item[scepterItem.type].Value;
            var drawCoords = player.Center + new Vector2(player.direction * 6f, 20f + player.gfxOffY - MathF.Sin(MathF.Pow(Math.Min(animation * 2f, 1f), 3f) * MathHelper.PiOver2) * 30f) - Main.screenPosition;
            float scepterRotation = Projectile.rotation - MathHelper.PiOver4;
            Vector2 origin = new(4f, texture.Height - 4f);
            float opacity = 1f;
            if (animation < 0.5f) {
                opacity = animation / 0.5f;
            }
            else if (animation > 0.8f) {
                opacity = 1f - (animation - 0.8f) / 0.2f;
            }

            Main.EntitySpriteDraw(texture, drawCoords, frame, lightColor * opacity, scepterRotation, origin, Projectile.scale, SpriteEffects.None);
            if (scepterItem.glowMask > 0) {
                Main.EntitySpriteDraw(TextureAssets.GlowMask[scepterItem.glowMask].Value, drawCoords, frame, Color.White with { A = 0 } * MathF.Pow(opacity, 2f), scepterRotation, origin, Projectile.scale, SpriteEffects.None);
            }
            if (animation > 0.3f) {
                var flareCoords = drawCoords + new Vector2(0f, -texture.Height);
                float flareScale = MathF.Sin(MathF.Pow((animation - 0.3f) / 0.8f, 2f) * MathHelper.Pi) * opacity;
                var flareColor = scepterModItem.GlowColor with { A = 0 } * flareScale;
                var flareOrigin = AequusTextures.Flare.Size() / 2f;
                Main.EntitySpriteDraw(AequusTextures.Flare, flareCoords, null, flareColor, 0f, flareOrigin, new Vector2(0.7f, 1.1f) * Projectile.scale * flareScale, SpriteEffects.None);
                Main.EntitySpriteDraw(AequusTextures.Flare, flareCoords, null, flareColor, MathHelper.PiOver2, flareOrigin, new Vector2(0.7f, 2f) * Projectile.scale * flareScale, SpriteEffects.None);
            }
            return false;
        }
    }
}
