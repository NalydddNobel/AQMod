using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class MendshroomProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.timeLeft = 20;
        }

        public override void AI()
        {
            float scale = Projectile.scale;

            if (Main.player[Projectile.owner].ownedProjectileCounts[Type] > 10)
            {
                for (int i = Projectile.whoAmI; i >= 0; i--)
                {
                    if (Projectile.CheckHeredity(Main.projectile[i]))
                    {
                        Projectile.timeLeft = Math.Min(Projectile.timeLeft, 2);
                        return;
                    }
                }
            }

            AequusPlayer aequus;
            int projIdentity = (int)Projectile.ai[0] - 1;
            if (projIdentity > -1)
            {
                projIdentity = AequusHelpers.FindProjectileIdentity(Projectile.owner, projIdentity);
                if (projIdentity == -1 || !Main.projectile[projIdentity].active || !Main.projectile[projIdentity].TryGetGlobalProjectile<SantankSentryProjectile>(out var value))
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.Kill();
                        Projectile.netUpdate = true;
                    }
                    return;
                }

                Projectile.Center = Main.projectile[projIdentity].Center;
                aequus = value.dummyPlayer?.Aequus();
            }
            else
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                aequus = Main.player[Projectile.owner].Aequus();
            }

            if (aequus?.healingMushroomItem != null && aequus?.MendshroomActive == true)
            {
                aequus.Mendshroom();
                if (Main.rand.NextBool(7))
                {
                    var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    var d = Dust.NewDustPerfect(aequus.Player.Center + v * Main.rand.NextFloat(0.2f, Projectile.scale / 2f * 0.95f), ModContent.DustType<MendshroomDustSpore>(), -v * Main.rand.NextFloat(0.1f, 1f), 255, new Color(10, 100, 20, 25));
                    if (aequus.cHealingMushroom != 0)
                    {
                        d.shader = GameShaders.Armor.GetSecondaryShader(aequus.cHealingMushroom, Main.player[Projectile.owner]);
                    }
                }
                Lighting.AddLight(aequus.Player.Center, Color.Green.ToVector3());
                Projectile.scale = MathHelper.Lerp(Projectile.scale, aequus.mendshroomDiameter, 0.2f);
            }
            else
            {
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 0f, 0.2f);
            }

            if (Projectile.scale > 0.1f)
            {
                Projectile.timeLeft = 2;
            }
            if (scale != Projectile.scale)
            {
                Projectile.netUpdate = true;
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.PrepareDrawnEntityDrawing(Projectile, Main.player[Projectile.owner].Aequus().cHealingMushroom);
            DrawAura(Projectile.Center - Main.screenPosition, Projectile.scale, Projectile.Opacity, ModContent.Request<Texture2D>(Texture + "Aura").Value, TextureAssets.Projectile[Type].Value);
            return false;
        }

        public static void DrawAura(Vector2 location, float diameter, float opacity, Texture2D texture, Texture2D circle)
        {
            var origin = texture.Size() / 2f;
            location = location.Floor();
            float scale = diameter / texture.Width;
            opacity = Math.Min(opacity * scale, 1f);

            var color = new Color(255, 255, 255, 0);
            Main.EntitySpriteDraw(texture, location, null,
                color * 0.4f * opacity, 0f, origin, scale, SpriteEffects.None, 0);
            texture = circle;

            foreach (var v in AequusHelpers.CircularVector(8))
            {
                Main.EntitySpriteDraw(texture, location + v * 2f * scale, null,
                    color * 0.66f * opacity, 0f, origin, scale, SpriteEffects.None, 0);
            }

            foreach (var v in AequusHelpers.CircularVector(4))
            {
                Main.EntitySpriteDraw(texture, location + v * scale, null,
                    color * opacity, 0f, origin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, location, null,
                Color.White * opacity, 0f, origin, scale, SpriteEffects.None, 0);
        }
    }
}