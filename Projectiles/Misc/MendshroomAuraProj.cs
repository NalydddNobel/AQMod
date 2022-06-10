using Aequus.Items.Accessories.Healing;
using Aequus.Items.Accessories.Summon.Sentry;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class MendshroomAuraProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

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
            MendshroomPlayer mendshroom;
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
                mendshroom = value.dummyPlayer?.GetModPlayer<MendshroomPlayer>();
                //if (mendshroom == null)
                //{
                //    Main.NewText("Dummy player is null for mendshroom");
                //}
                //else
                //{
                //    Main.NewText(mendshroom.diameter);
                //    Main.NewText(mendshroom.EffectActive);
                //}
            }
            else
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
                mendshroom = Main.player[Projectile.owner].GetModPlayer<MendshroomPlayer>();
            }

            if (mendshroom?.EffectActive == true)
            {
                mendshroom.HealPlayers();
                if (Main.rand.NextBool(10))
                {
                    var v = Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2();
                    Dust.NewDustPerfect(mendshroom.Player.Center + v * Main.rand.NextFloat(Projectile.scale / 2f * 0.9f), ModContent.DustType<MendshroomDustSpore>(), -v, 255, new Color(10, 100, 20, 25));
                }
                Lighting.AddLight(mendshroom.Player.Center, Color.Green.ToVector3());
                Projectile.scale = MathHelper.Lerp(Projectile.scale, mendshroom.diameter, 0.2f);
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
            AequusPlayer.DrawLegacyAura(Projectile.Center, Projectile.scale, Projectile.Opacity, new Color(10, 128, 10, 0));
            return false;
        }
    }
}