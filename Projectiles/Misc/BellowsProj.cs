using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Misc
{
    public class BellowsProj : PumpinatorProj
    {
        public bool _didDust;

        public override bool PushPlayers => false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hide = true;
            Projectile.timeLeft = 60 * (1+Projectile.extraUpdates);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void DoDust()
        {
            if (Main.player[Projectile.owner].itemAnimation >= 45)
            {
                _didDust = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    var v = Vector2.Normalize(Projectile.velocity);
                    var spawnPos = Main.player[Projectile.owner].MountedCenter + v * (Main.player[Projectile.owner].width + 10);
                    if (Main.rand.NextBool(4))
                    {
                        var g = Gore.NewGoreDirect(Main.player[Projectile.owner].GetSource_ItemUse(Main.player[Projectile.owner].HeldItem), spawnPos,
                            v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f), GoreID.Smoke1 + Main.rand.Next(3));
                        Main.instance.LoadGore(g.type);
                        g.position -= TextureAssets.Gore[g.type].Size() / 2f;
                        g.scale = Main.rand.NextFloat(0.5f, 1.1f);
                        g.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    }
                    var d = Dust.NewDustDirect(spawnPos, 10, 10, DustID.Smoke);
                    d.velocity *= 0.1f;
                    d.velocity += v.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.5f, 4f);
                    d.scale = Main.rand.NextFloat(0.8f, 1.5f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                }
            }
        }

        public override void AI()
        {
            base.AI();
            if (Main.myPlayer == Projectile.owner && Projectile.numUpdates <= 0 && Main.player[Projectile.owner].itemAnimation >= 45)
            {
                var player = Main.player[Projectile.owner];
                var v = Vector2.Normalize(Main.MouseWorld - player.Center).UnNaN();
                if (v.Y > 0f)
                {
                    v.Y *= player.gravity / 0.4f;
                }
                player.velocity -= v * GetPushForce(player, player.HeldItemFixed());
                if (player.velocity.X < 4f)
                {
                    player.fallStart = (int)player.position.Y / 16;
                }
            }
            //Projectile.velocity = Vector2.Normalize();
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            if (Projectile.numUpdates != -1)
                return;
            if (Projectile.timeLeft < 20 * (Projectile.extraUpdates + 1))
            {
                if (Projectile.frame > 0)
                {
                    if (Projectile.frameCounter++ > 2)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame--;
                    }
                }
            }
            else if (Projectile.frame < 3)
            {
                if (Projectile.frameCounter++ > 2)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
            }
        }
        public float GetPushForce(Player player, Item item)
        {
            float force = item.knockBack;
            if (player.mount != null && player.mount.Active && player.mount._data.usesHover)
            {
                force *= 0.33f;
            }
            force /= Math.Max(player.velocity.Length().UnNaN() / 8f, 1f);
            return force;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var _, out var frame, out var origin, out int _);

            var difference = -Projectile.velocity;
            var dir = Vector2.Normalize(difference);
            var drawCoords = Main.player[Projectile.owner].MountedCenter + dir * -20f;
            drawCoords.Y += 2f + Projectile.gfxOffY;
            float rotation = difference.ToRotation() + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
            var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, drawCoords - Main.screenPosition, frame, AequusHelpers.GetColor(drawCoords),
                 rotation, origin, 1f, spriteEffects, 0);
            return false;
        }
    }
}