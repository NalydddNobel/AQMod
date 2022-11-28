using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Friendly
{
    public class DavyJonesAnchorProj : ModProjectile
    {
        public int AttatchedNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.alpha = 255;
            Projectile.ArmorPenetration = 10;
            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            Projectile.friendly = Projectile.alpha < 50;

            if (AttatchedNPC >= 0)
            {
                if (!Main.npc[AttatchedNPC].active)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.CollideWithOthers();
            Projectile.velocity.Y += 0.4f;
            var npcCenter = Main.npc[AttatchedNPC].Center;
            float minDistance = 100f;
            float distance = Projectile.Distance(npcCenter);
            Projectile.ai[1] = 0f;
            if (distance > minDistance)
            {
                var velocityAdd = Projectile.DirectionTo(npcCenter) * ((distance - minDistance / 2f) / minDistance) * 0.4f;
                Projectile.velocity += velocityAdd;
                if (Projectile.velocity.Length() > 15f)
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 15f;
                }
                Main.npc[AttatchedNPC].velocity -= velocityAdd * Main.npc[AttatchedNPC].knockBackResist * 0.75f;
                if ((int)Projectile.ai[1] != 1 || Projectile.localAI[0] > 20f)
                {
                    Main.npc[AttatchedNPC].netUpdate = true;
                }
                Projectile.localAI[0]++;
                Projectile.ai[1] = 1f;
            }

            if (Projectile.velocity.Y.Abs() < 0.8f)
            {
                Projectile.velocity.X *= 0.9f;
            }

            if (Projectile.velocity.Length() <= 0.11f)
            {
                return;
            }
            float wantedRotation = (npcCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2;

            Projectile.rotation = wantedRotation;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = Main.npc[AttatchedNPC].position.Y > Projectile.position.Y + Projectile.height;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((int)Projectile.ai[1] == 1 || oldVelocity.Length() > 3f)
            {
                if (oldVelocity.X != Projectile.velocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X * 0.3f;
                }
                if (oldVelocity.Y != Projectile.velocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.3f;
                }
                if (oldVelocity.Length() > 6f)
                {
                    Projectile.velocity.X += Main.rand.NextFloat(-2f, 2f);
                    Projectile.velocity.Y += Main.rand.NextFloat(-2f, 0f);
                    Projectile.netUpdate = true;
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Normalize(Projectile.velocity) * 0.01f, ModContent.ProjectileType<DavyJonesAnchorExplosionProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                    for (int i = 0; i < 8; i++)
                    {
                        var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, Projectile.velocity.X, Projectile.velocity.Y, 200, Scale: Main.rand.NextFloat(0.8f, 1.6f));
                        d.velocity *= 0.8f;
                        d.noGravity = true;
                        d.fadeIn = d.scale + 0.1f;
                    }
                }
            }
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadTiles(TileID.Rope);
            var chain = TextureAssets.Tile[TileID.Rope].Value;
            var frame = new Rectangle(90, 0, 16, 16);
            int height = frame.Height - 2;
            var currentPosition = Projectile.Center - new Vector2(0f, Projectile.height / 2f * Projectile.scale - 8f).RotatedBy(Projectile.rotation);
            var velocity = Main.npc[AttatchedNPC].Center - currentPosition;
            int length = (int)(velocity.Length() / height);
            velocity.Normalize();
            velocity *= height;
            float rotation = velocity.ToRotation() + MathHelper.PiOver2;
            var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
            for (int i = 0; i < length; i++)
            {
                var position = currentPosition + velocity * i;
                Main.EntitySpriteDraw(chain, position - Main.screenPosition, frame, AequusHelpers.GetColor(position) * Projectile.Opacity, rotation, origin, 1f, SpriteEffects.None, 0);
            }
            Projectile.GetDrawInfo(out var t, out var offset, out frame, out origin, out int _);

            Main.EntitySpriteDraw(t, Projectile.position + offset - Main.screenPosition, frame, AequusHelpers.GetColor(Projectile.position + offset) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public class DavyJonesAnchorExplosionProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.DefaultToExplosion(90, DamageClass.Generic, 20);
            Projectile.ArmorPenetration = 20;
            Projectile.hide = true;
        }
    }
}