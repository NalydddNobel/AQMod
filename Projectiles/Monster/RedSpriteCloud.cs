using Aequus.NPCs.Monsters.Sky;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Monster
{
    public class RedSpriteCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;

            //Projectile.GetGlobalProjectile<AQProjectile>().SetupTemperatureStats(20);
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (drawColor.R < 60)
            {
                drawColor.R = 60;
            }
            if (drawColor.G < 60)
            {
                drawColor.G = 60;
            }
            if (drawColor.B < 60)
            {
                drawColor.B = 60;
            }
            return drawColor;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 1)
            {
                Projectile.rotation = 0f;
                Projectile.velocity *= 0.2f;
                if (Projectile.frame <= 3)
                {
                    Projectile.frame = 4;
                }
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= Main.projFrames[Projectile.type])
                    {
                        Projectile.frame = 4;
                    }
                }
                if (Projectile.timeLeft < 60)
                {
                    if (Projectile.timeLeft < 20)
                    {
                        Projectile.scale -= Projectile.scale * 0.1f;
                    }
                    else
                    {
                        Projectile.scale += Projectile.scale * 0.02f;
                    }
                }
                Projectile.ai[1]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Projectile.ai[1] > 6f)
                    {
                        Projectile.ai[1] = 0f;
                        Projectile.NewProjectile(Projectile.GetItemSource_FromThis(), new Vector2(Projectile.position.X + Main.rand.NextFloat(Projectile.width), Projectile.position.Y + Projectile.height), new Vector2(0f, 10f), ModContent.ProjectileType<RedSpriteCloudLightning>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                    }
                }
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame > 3)
                    {
                        Projectile.frame = 0;
                    }
                }
                if (Projectile.timeLeft < 2)
                {
                    Projectile.timeLeft = 600;
                    Projectile.netUpdate = true;
                    Projectile.ai[0] = 1f;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var orig = texture.Size() / 2f;
            var drawPosition = Projectile.Center;
            float speedX = Projectile.velocity.X.Abs();
            lightColor = Projectile.GetAlpha(lightColor);
            if (speedX > 8f)
            {
                drawPosition.X -= (Projectile.scale - 1f) * 16f;
            }
            var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            var origin = frame.Size() / 2f;
            float auraIntensity = 0f;
            if ((int)Projectile.ai[0] == 1)
            {
                if (Projectile.localAI[0] < 10)
                    Projectile.localAI[0]++;
                auraIntensity += Projectile.localAI[0] / 5f;
            }
            RedSprite.DrawThingWithAura(Main.spriteBatch, texture, drawPosition - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, auraIntensity);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            for (int i = 0; i < 50; i++)
            {
                int d = Dust.NewDust(Projectile.position, 16, 16, ModContent.DustType<RedSpriteDust>());
                Main.dust[d].velocity = (Main.dust[d].position - center) / 8f;
            }
        }
    }
}