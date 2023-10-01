using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc {
    public class SplashPotionProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public int ItemType { get => Math.Abs((int)Projectile.ai[0]); set => Projectile.ai[0] = value; }
        public int BuffTime { get => (int)Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public int AreaOfEffect { get => 120; }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source) {
            if (source is not EntitySource_ItemUse itemUse || itemUse.Item == null || itemUse.Item.IsAir) {
                return;
            }

            Projectile.ai[0] = itemUse.Item.type;
            Projectile.ai[1] = itemUse.Item.buffTime;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Projectile.netUpdate = true;
            Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 10f;
        }

        public void EnterSplashState()
        {
            Projectile.ai[0] = -Projectile.ai[0];
            Projectile.timeLeft = (AreaOfEffect / 16 + 1) * 4;
            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
            Projectile.hide = true;
            Projectile.tileCollide = false;
        }

        public void HandleSplashState()
        {
            int buffID = ContentSamples.ItemsByType[ItemType].buffType;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && Projectile.Distance(Main.player[i].getRect().ClosestPointInRect(Projectile.Center)) < AreaOfEffect)
                {
                    Main.player[i].AddBuff(buffID, BuffTime);
                }
            }

            int progress = (Projectile.timeLeft - 4) / 4;

            var color = PotionColorsDatabase.GetColorFromItemID(ItemType);
            int startX = (int)(Projectile.Center.X - AreaOfEffect) / 16;
            int startY = (int)(Projectile.Center.Y - AreaOfEffect) / 16;
            int endX = (int)(Projectile.Center.X + AreaOfEffect) / 16;
            int endY = (int)(Projectile.Center.Y + AreaOfEffect) / 16;
            int middleX = (int)Projectile.Center.X / 16;
            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    if (!WorldGen.InWorld(i, j, 4))
                        continue;

                    if (Main.tile[i, j].IsFullySolid() && !Main.tile[i, j - 1].IsFullySolid())
                    {
                        int x = AreaOfEffect / 16 - Math.Abs(i - middleX);
                        if (progress == x)
                        {
                            for (int m = 0; m < x; m++)
                            {
                                if (Main.rand.NextBool(2))
                                {
                                    var d = Dust.NewDustPerfect(new Vector2(i * 16f + Main.rand.NextFloat(16f), j * 16f + Main.rand.NextFloat(-2f, 2f)), ModContent.DustType<MonoSparkleDust>(), Vector2.Zero, newColor: color.UseA(Main.rand.Next(200)) * Math.Min(x / 4f, 1f), Scale: Main.rand.NextFloat(0.75f, 1.5f));
                                    d.velocity.X = Math.Sign(d.position.X - Projectile.Center.X) * Main.rand.NextFloat(3f) * x / 3f;
                                    d.velocity.Y -= (16f / Math.Max(x, 1f) / 2f) * Main.rand.NextFloat(1f);
                                    if (d.velocity.Length() > 4f)
                                    {
                                        d.velocity.Normalize();
                                        d.velocity *= 4f;
                                    }
                                    d.fadeIn = d.scale + 0.4f;
                                }
                            }
                        }
                    }
                }
            }
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 16; i++)
                {
                    var normal = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(Projectile.Center + normal * Main.rand.NextFloat(AreaOfEffect / 8f), ModContent.DustType<MonoSparkleDust>(), normal * Main.rand.NextFloat(2f), newColor: color.UseA(Main.rand.Next(200)), Scale: Main.rand.NextFloat(0.5f, 1f));
                    d.fadeIn = d.scale + 0.4f;
                }
                for (int i = 0; i < 16; i++)
                {
                    var normal = Main.rand.NextVector2Unit();
                    normal.Y = -Math.Abs(normal.Y) * 2f;
                    Dust.NewDustPerfect(Projectile.Center + normal * Main.rand.NextFloat(16f), DustID.Glass, normal * Main.rand.NextFloat(2f), Scale: Main.rand.NextFloat(0.5f, 1f));
                }
                SoundEngine.PlaySound(SoundID.Shatter.WithPitchOffset(Main.rand.NextFloat(0.2f, 0.4f)));
                Projectile.localAI[0] = 1f;
            }

            for (int i = 0; i < 4; i++)
            {
                if (Main.rand.NextBool(2))
                {
                    var normal = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(Projectile.Center + normal * Main.rand.NextFloat(progress * 4f), ModContent.DustType<MonoSparkleDust>(),
                        normal * Main.rand.NextFloat(progress * 1f), newColor: color.UseA(Main.rand.Next(200)), Scale: Main.rand.NextFloat(0.5f, 1f));
                    d.fadeIn = d.scale + 0.4f;
                }
            }
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 0f)
            {
                Projectile.hide = true;
                Projectile.tileCollide = false;
                HandleSplashState();
                return;
            }
            if (ItemType == 0)
                ItemType = ItemID.RegenerationPotion;
            if (BuffTime == 0)
                BuffTime = 3600;

            var entityCollideRect = Projectile.getRect();
            entityCollideRect.Inflate(16, 16);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && !Main.player[i].ghost && Projectile.Colliding(entityCollideRect, Main.player[i].getRect()))
                {
                    if (i == Projectile.owner && Projectile.timeLeft > 580)
                    {
                        continue;
                    }
                    EnterSplashState();
                    return;
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Projectile.Colliding(entityCollideRect, Main.npc[i].getRect()))
                {
                    EnterSplashState();
                    return;
                }
            }
            Projectile.velocity.Y += 0.25f;
            Projectile.velocity.X *= 0.99f;
            Projectile.rotation += Projectile.velocity.X * 0.02f;
            if (Main.GameUpdateCount % 7 == 0 || Main.rand.NextBool(12))
            {
                var color = PotionColorsDatabase.GetColorFromItemID(ItemType);
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(), Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, newColor: color.UseA(0) * Main.rand.NextFloat(0.66f, 1f), Scale: Main.rand.NextFloat(0.8f, 1.5f));
                d.velocity *= 0.1f;
                d.velocity += Projectile.velocity * 0.1f;
                d.fadeIn = d.scale + 0.4f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            EnterSplashState();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 8;
            height = 8;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadItem(ItemType);

            Helper.GetItemDrawData(ItemType, out var frame);
            var origin = frame.Size() / 2;
            Main.EntitySpriteDraw(TextureAssets.Item[ItemType].Value, Projectile.Center - Main.screenPosition, frame, Helper.GetColor(Projectile.Center), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
