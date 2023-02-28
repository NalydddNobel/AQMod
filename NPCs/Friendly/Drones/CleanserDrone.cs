using Aequus.Content.DronePylons;
using Aequus.Items.Misc.Drones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Drones
{
    public class CleanserDrone : TownDroneBase
    {
        public override int ItemDrop => ModContent.ItemType<PylonCleanserItem>();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 12;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 20;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 250;
            NPC.dontTakeDamage = true;
            NPC.damage = 15;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            DrawOffsetY = -10;
        }

        public override void AI()
        {
            var pylonDiff = pylonSpot.ToWorldCoordinates() - NPC.Center;
            NPC.CollideWithOthers(0.1f);
            CleanserDroneSlot slot = null;
            if (pylonSpot != Point.Zero)
            {
                foreach (var drones in PylonManager.ActiveDrones)
                {
                    if (drones is CleanserDroneSlot cleanserSlot)
                    {
                        slot = cleanserSlot;
                    }
                }
                if (slot == null)
                {
                    NPC.Kill();
                    return;
                }
            }

            int amtPerFrame = 6;
            switch ((int)NPC.ai[0])
            {
                case 0:
                case 3:
                    {
                        if (NPC.ai[3] < 5f)
                        {
                            NPC.ai[3] += 5f;
                            NPC.ai[3] += Main.rand.NextFloat(100f);
                            NPC.netUpdate = true;
                        }
                        NPC.ai[3] += 1 / 60f;
                        if (NPC.velocity.X.Abs() < 0.1f)
                        {
                            NPC.ai[3] += 1 / 50f;
                        }
                        if (NPC.localAI[3] == 0f || NPC.localAI[3] == MathHelper.Pi)
                        {
                            NPC.localAI[3] = 0f;
                        }
                        else
                        {
                            NPC.localAI[3] = Utils.AngleTowards(NPC.localAI[3], NPC.direction == -1 ? MathHelper.Pi : 0f, 0.1f);
                        }
                        bool doAnimation = true;
                        if (NPC.ai[0] != 3)
                        {
                            float wave = Helper.Wave(NPC.ai[3] * 0.1f, -1f, 1f);
                            NPC.velocity.X = Math.Clamp(NPC.velocity.X + wave * 0.05f, -1.5f, 1.5f);
                            base.AI();
                            var loc = NPC.Center.ToTileCoordinates();
                            if (Main.tile[loc.X + NPC.direction, loc.Y].IsFullySolid() || NPC.collideX)
                            {
                                if (NPC.collideX)
                                {
                                    doAnimation = false;
                                    NPC.frameCounter += NPC.velocity.Y.Abs() * 0.25;
                                    if (NPC.frameCounter > 11 * amtPerFrame)
                                    {
                                        NPC.frameCounter = 0.0;
                                    }
                                    NPC.frame.Y = NPC.frame.Height * (int)(NPC.frameCounter / amtPerFrame + 1);
                                    if (Main.tile[loc.X + NPC.direction, loc.Y].IsFullySolid())
                                    {
                                        NPC.rotation = Utils.AngleLerp(NPC.rotation, MathHelper.PiOver2 * -NPC.direction, 0.2f);
                                    }
                                }

                                NPC.velocity.Y = -5.5f;
                                if (Main.tile[loc.X, loc.Y - 1].IsFullySolid() && NPC.collideY)
                                {
                                    NPC.velocity.X -= NPC.direction * 3f;
                                    NPC.ai[3] += 1f;
                                }
                            }
                            else
                            {
                                NPC.rotation *= 0.9f;
                            }
                            if (Main.rand.NextBool(1000))
                            {
                                NPC.ai[0] = 3f;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity.X *= 0.96f;
                            if (Main.rand.NextBool(NPC.velocity.Y.Abs() > 1f ? 30 : 500))
                            {
                                NPC.ai[0] = 0f;
                            }
                        }
                        if (doAnimation)
                        {
                            if (NPC.velocity.Y.Abs() < 0.1f)
                            {
                                if (NPC.velocity.X.Abs() < 0.1f)
                                {
                                    NPC.frameCounter = 0.0;
                                    NPC.frame.Y = 0;
                                }
                                else
                                {
                                    NPC.frameCounter += NPC.velocity.X.Abs() * 0.7;
                                    if (NPC.frameCounter > 11 * amtPerFrame)
                                    {
                                        NPC.frameCounter = 0.0;
                                    }
                                    NPC.frame.Y = NPC.frame.Height * (int)(NPC.frameCounter / amtPerFrame + 1);
                                }
                            }
                            else
                            {
                                NPC.frameCounter -= 0.0;
                                NPC.frame.Y = NPC.frame.Height * 6;
                            }
                        }
                        if (pylonDiff.Length() > 1700f || NPC.ai[1] >= 5000f)
                        {
                            NPC.velocity *= 0.8f;
                            NPC.ai[0] = 2f;
                            NPC.ai[1] = 0f;
                            return;
                        }

                        if (NPC.alpha > 0)
                        {
                            NPC.alpha -= 10;
                            if (NPC.alpha < 0)
                            {
                                NPC.alpha = 0;
                            }
                            return;
                        }

                        NPC.ai[1]++;
                        NPC.spriteDirection = NPC.direction;

                        if (NPC.ai[1] > 60f && slot != null)
                        {
                            int solution = slot.GetSolutionProjectileID();
                            var convertibleTile = FindConvertibleTile(slot, solution);
                            if (convertibleTile == Point.Zero)
                                return;

                            if (solution > 0f)
                            {
                                NPC.ai[0] = 1f;
                                NPC.ai[1] = convertibleTile.X;
                                NPC.ai[2] = convertibleTile.Y;
                                NPC.ai[3] = solution;
                                NPC.localAI[2] = 0f;
                                NPC.localAI[3] = NPC.direction == -1 ? MathHelper.Pi : 0f;
                                NPC.direction = Main.rand.NextBool() ? -1 : 1;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                    break;

                case 1:
                    {
                        NPC.velocity *= 0.95f;
                        var p = new Point((int)NPC.ai[1], (int)NPC.ai[2]);
                        NPC.localAI[3] = Utils.AngleTowards(NPC.localAI[3], Math.Abs((p.ToWorldCoordinates() - NPC.Center - new Vector2(0f, 20f).RotatedBy(NPC.rotation) * NPC.scale).ToRotation()) + NPC.localAI[2] / 30f * NPC.direction, 0.4f);
                        if (NPC.localAI[2] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int solution = (int)NPC.ai[3];

                            if (solution > 0)
                            {
                                var spawnPosition = NPC.Center - new Vector2(0f, 20f);
                                var n = Vector2.Normalize(p.ToWorldCoordinates() - spawnPosition).RotatedBy(NPC.localAI[2] / 30f * NPC.direction);
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition + n * -90f,
                                     n * 10f, solution, 0, 0, Main.myPlayer);
                            }
                        }
                        if (NPC.localAI[2] > 40f)
                        {
                            NPC.ai[0] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = Main.rand.NextFloat(100f);
                            NPC.localAI[2] = 0f;
                            NPC.netUpdate = true;
                        }
                        if ((int)(NPC.localAI[2] - 5f) % 20 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                        }
                        NPC.localAI[2]++;
                    }
                    break;

                case 2:
                    {
                        NPC.velocity.X *= 0.95f;
                        NPC.alpha += 10;
                        if (NPC.alpha >= 255)
                        {
                            NPC.alpha = 255;
                            NPC.Center = pylonSpot.ToWorldCoordinates() + new Vector2(16f, 0f);
                            NPC.ai[0] = 0f;
                        }
                    }
                    break;
            }
        }

        public Point FindConvertibleTile(CleanserDroneSlot slot, int solution)
        {
            return slot.FindConvertibleTile(NPC.Center.ToTileCoordinates(), solution);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y += NPC.frame.Height;
                    if (NPC.frame.Y > NPC.frame.Height * (Main.npcFrameCount[Type] - 1))
                    {
                        NPC.frame.Y = NPC.frame.Height;
                    }
                }
                NPC.alpha = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC.GetDrawInfo(out var texture, out var off, out var frame, out var origin, out int _);
            off.Y += DrawOffsetY;
            var color = GetPylonColor();

            float gunRotation = NPC.localAI[3];
            var gun = ModContent.Request<Texture2D>($"{Texture}_Gun", AssetRequestMode.ImmediateLoad).Value;
            var gunOrigin = new Vector2(29.5f, gun.Height / 2f);
            var lightColor = NPC.GetNPCColorTintedByBuffs(drawColor) * NPC.Opacity;

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.alpha = 0;
                lightColor = Color.White;
            }

            var gunPosition = NPC.position + off - new Vector2(0f, 19f).RotatedBy(NPC.rotation) * NPC.scale;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var gunEffects = effects;
            if (gunRotation.Abs() > 0.01f)
            {
                gunEffects = gunRotation.Abs() > MathHelper.PiOver2 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            }
            else
            {
                gunRotation += MathHelper.Pi;
            }
            if (gunEffects == SpriteEffects.FlipHorizontally)
            {
                gunOrigin.X = gun.Width - gunOrigin.X;
            }
            spriteBatch.Draw(texture, NPC.position + off - screenPos, frame, lightColor,
                NPC.rotation, origin, NPC.scale, effects, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, NPC.position + off - screenPos, frame, color * SpawnInOpacity * NPC.Opacity,
                NPC.rotation, origin, NPC.scale, effects, 0f);

            spriteBatch.Draw(gun, gunPosition - screenPos, null, lightColor,
                gunRotation + MathHelper.Pi, gunOrigin, NPC.scale, gunEffects, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>($"{Texture}_Gun_Glow", AssetRequestMode.ImmediateLoad).Value, gunPosition - screenPos, null, color * SpawnInOpacity * NPC.Opacity,
                gunRotation + MathHelper.Pi, gunOrigin, NPC.scale, gunEffects, 0f);
            return false;
        }
    }
}