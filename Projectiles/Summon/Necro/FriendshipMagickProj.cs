using Aequus.Content.Necromancy;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class FriendshipMagickProj : ModProjectile
    {
        public virtual float Tier => 2.33f;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.alpha = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.75f;
            Projectile.timeLeft = 250;
            Projectile.alpha = 250;
            Projectile.hide = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 100, 255, 0);
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server && Projectile.alpha < 50 && Main.GameUpdateCount % 15 == 0)
            {
                var center = Projectile.Center;
                foreach (var v in AequusHelpers.CircularVector(3, Main.GlobalTimeWrappedHourly))
                {
                    var d = Dust.NewDustPerfect(center + v * Projectile.width, ModContent.DustType<FriendshipDust>(), v.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi / 3f)) * Main.rand.NextFloat(5f), Math.Min(Projectile.alpha * 4, 255), new Color(255, 200, 255, 0),
                        Main.rand.NextFloat(0.3f, 0.75f));
                    d.velocity.Y -= 2f;
                    d.alpha = 255;
                    d.fadeIn = d.scale + 0.6f;
                    d.velocity += Projectile.velocity;
                }
            }

            int healingRange = 180;
            float velocityMultiplier = 0.88f;
            Projectile.ai[0]++;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Projectile.Distance(Main.npc[i].getRect().ClosestPointInRect(Projectile.Center)) < healingRange)
                {
                    velocityMultiplier *= 0.5f;
                    if (Projectile.ai[0] > 30f)
                    {
                        var n = Main.npc[i].GetGlobalNPC<NecromancyNPC>();
                        int healAmt = AequusHelpers.CalcHealing(Main.npc[i].life, Main.npc[i].lifeMax, Main.npc[i].lifeMax / 5);
                        if (!n.isZombie)
                        {
                            if (healAmt > 0)
                            {
                                Main.npc[i].life += healAmt;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.netUpdate = true;
                                    Main.npc[i].HealEffect(healAmt, broadcast: true);
                                    Main.npc[i].netUpdate = true;
                                }
                            }
                            else if (NecromancyDatabase.TryGet(Main.npc[i].type, out var info) && info.EnoughPower(Tier) && Main.rand.NextBool(5))
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    n.zombieOwner = Projectile.owner;
                                    n.DebuffTier(Tier);
                                    n.renderLayer = GhostOutlineRenderer.IDs.Friendship;
                                    n.SpawnZombie_SetZombieStats(Main.npc[i], Main.npc[i].Center, Main.npc[i].velocity, Main.npc[i].direction, Main.npc[i].spriteDirection, out bool playSound);
                                    n.slotsConsumed = 0;
                                    n.zombieTimer /= 10;
                                    n.zombieTimerMax /= 10;
                                    if (playSound)
                                    {
                                        SoundEngine.PlaySound(SoundID.Item4, Main.npc[i].Center);
                                    }
                                }
                            }
                        }
                        foreach (var v in AequusHelpers.CircularVector(8, Main.rand.NextFloat(MathHelper.TwoPi)))
                        {
                            float off = Main.rand.NextFloat(0.7f, 1f);
                            var d = Dust.NewDustPerfect(Main.npc[i].Center + v * Main.npc[i].Size * off, ModContent.DustType<MonoDust>(), v * -Main.npc[i].Size * 0.08f * off, newColor: new Color(255, 100, 222, 0),
                                Scale: Main.rand.NextFloat(1f, 1.6f));
                            d.alpha = 100;
                            d.fadeIn = d.scale + 0.6f;
                            d.velocity += Main.npc[i].velocity;
                        }
                    }
                }
            }
            if (Projectile.ai[0] > 30f)
            {
                Projectile.ai[0] = 0f;
            }

            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 5;
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            if (Projectile.ai[1] < healingRange)
            {
                Projectile.ai[1] += Projectile.Opacity * 4f;
                if (Projectile.ai[1] >= healingRange)
                {
                    Projectile.ai[1] = healingRange;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.velocity.Length() > 4f)
            {
                Projectile.velocity *= velocityMultiplier;
            }
            else
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity).UnNaN() * 4f;
            }
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.5f, 1f));
            foreach (var v in AequusHelpers.CircularVector(40))
            {
                Lighting.AddLight(Projectile.Center + v * (Projectile.ai[1] - 12f), new Vector3(0.44f, 0.1f, 0.44f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            var aura = ModContent.Request<Texture2D>($"{Texture}_Aura", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(aura, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.05f * Projectile.Opacity,
                0f, aura.Size() / 2f, Projectile.scale * Projectile.ai[1] / texture.Width / 4f + 0.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(aura, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0) * 0.05f * Projectile.Opacity,
                MathHelper.PiOver4, aura.Size() / 2f, Projectile.scale * Projectile.ai[1] / texture.Width / 4f + 0.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity,
                Projectile.rotation + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 10f, -0.15f, 0.15f), origin, Projectile.scale + AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 7f, -0.15f, 0.15f), SpriteEffects.None, 0f);
            return false;
        }
    }
}