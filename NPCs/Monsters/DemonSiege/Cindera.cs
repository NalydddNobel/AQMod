using AQMod.Assets;
using AQMod.Items.Accessories;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Vanities.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.DemonSiege
{
    public sealed class Cindera : AIBat, IDecideFallThroughPlatforms
    {
        public const int FRAME_FLY_0 = 0;
        public const int FRAME_FLY_1 = 1;
        public const int FRAME_FLY_2 = 2;
        public const int FRAME_FLY_3 = 3;

        public const int FRAME_OPEN_MOUTH_0 = 4;
        public const int FRAME_OPEN_MOUTH_1 = 5;
        public const int FRAME_OPEN_MOUTH_2 = 6;
        public const int FRAME_OPEN_MOUTH_3 = 7;

        public const int FRAME_CHOMP_PRE = 8;
        public const int FRAME_CHOMP_0 = 9;
        public const int FRAME_CHOMP_1 = 10;
        public const int FRAME_CHOMP_2 = 11;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 12;
            NPCID.Sets.TrailCacheLength[npc.type] = 4;
            NPCID.Sets.TrailingMode[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.width = 24;
            npc.height = 24;
            npc.aiStyle = -1;
            npc.damage = 30;
            npc.defense = 12;
            npc.lifeMax = 130;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.noGravity = true;
            npc.alpha = 50;
            npc.lavaImmune = true;
            npc.trapImmune = true;
            npc.value = 250f;
            npc.knockBackResist = 0.2f;
            npc.buffImmune[BuffID.Poisoned] = true;
            npc.buffImmune[BuffID.OnFire] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
            npc.buffImmune[BuffID.Confused] = false;
            npc.gfxOffY = -6f;
            npc.SetLiquidSpeed(lava: 1f);
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.Placeable.Banners.CinderaBanner>();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.65f);
        }

        protected override float MaxSpeedX => 6f;
        protected override float SpeedYMax => 2f;
        protected override float SpeedY => 0.125f;

        public override void AI()
        {
            bool canHitPlayer = false;
            if (npc.HasValidTarget)
            {
                var target = Main.player[npc.target];
                canHitPlayer = Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height);
            }
            if (!canHitPlayer)
            {
                if (npc.ai[3] < 300f)
                {
                    npc.ai[3]++;
                }
                else
                {
                    npc.ai[0] = 10f;
                    npc.ai[3] = 301f;
                    npc.noTileCollide = true;
                }
            }
            else
            {
                if ((int)npc.ai[3] == 301)
                {
                    npc.ai[3] = 0f;
                    npc.noTileCollide = false;
                }
                else if (npc.ai[3] > 0f)
                {
                    npc.ai[3]--;
                }
            }
            if ((int)npc.ai[0] <= 120f)
            {
                npc.knockBackResist = 0.2f;
                base.AI();
                var target = Main.player[npc.target];
                var differenceY = target.position.Y + target.height / 2f - (npc.position.Y + npc.height / 2f);
                float differenceYAbs = differenceY.Abs();
                if (differenceYAbs < target.height + npc.height)
                {
                    npc.ai[0]++;
                }
                else if (differenceYAbs > target.height * 6 + npc.height)
                {
                    if (npc.ai[0] > 0)
                        npc.ai[0]--;
                    if (npc.ai[0] < 60f && Collision.CanHitLine(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                        npc.velocity.Y += 0.1f * npc.directionY;
                }
                npc.rotation = npc.velocity.X * 0.0628f;
                npc.spriteDirection = npc.velocity.X < 0f ? -1 : 1;
            }
            else if (npc.ai[0] < 300f)
            {
                npc.velocity *= 0.94f;
                npc.rotation = npc.velocity.X * 0.0628f;
                npc.ai[0]++;
                npc.knockBackResist = 0.05f;
                if (npc.velocity.Length() < 1f || npc.ai[0] > 300f)
                {
                    npc.knockBackResist = 0f;
                    npc.spriteDirection = npc.direction;
                    npc.velocity.X = npc.direction * (Main.expertMode ? 12f : 6f);
                    npc.velocity.Y = 0f;
                    npc.rotation = 0f;
                    npc.ai[0] = 301f;
                    Main.PlaySound(SoundID.DD2_FlameburstTowerShot, npc.Center);
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, 31);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                    }
                }
            }
            else if (npc.ai[0] < 800f)
            {
                npc.knockBackResist = 0f;
                if (npc.collideX)
                {
                    npc.ai[0] = 802f;
                    npc.direction = -npc.direction;
                    npc.velocity.X = -npc.oldVelocity.X * 0.5f;
                    Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode, npc.Center);
                    for (int i = 0; i < 30; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, 31);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                    }
                    for (int i = 0; i < 2; i++)
                        Gore.NewGore(npc.Center + npc.velocity, npc.velocity * 0.2f, 61 + Main.rand.Next(3));
                }
                var target = Main.player[npc.target];
                var differenceX = target.position.X + target.width / 2f - (npc.position.X + npc.width / 2f);
                var differenceY = target.position.Y + target.height / 2f - (npc.position.Y + npc.height / 2f);
                npc.ai[0]++;
                if (differenceY.Abs() > target.height * 2 + npc.height)
                    npc.ai[0] += 3f;
                if (npc.ai[0] > 800f)
                    npc.ai[0] = 802f;
                if (npc.direction == -1)
                {
                    if (differenceX > target.width + npc.width * 4)
                    {
                        npc.ai[0] = 801f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (differenceX < -(target.width + npc.width * 4))
                    {
                        npc.ai[0] = 801f;
                        npc.netUpdate = true;
                    }
                }
                if (npc.ai[0] == 801f)
                {
                    Main.PlaySound(SoundID.Item1, npc.Center);
                    npc.ai[0] = 0f;
                    for (int i = 0; i < 10; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        int d = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, 31);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale = Main.rand.NextFloat(0.9f, 3f);
                    }
                    Gore.NewGore(npc.Center + npc.velocity, npc.velocity * 0.1f, 61 + Main.rand.Next(3));
                }
                int d2 = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                Main.dust[d2].noGravity = true;
                Main.dust[d2].scale = Main.rand.NextFloat(0.9f, 2f);
            }
            else
            {
                if ((int)npc.ai[0] == 802)
                {
                    npc.velocity *= 0.98f;
                    if (npc.velocity.X.Abs() < MaxSpeedX / 4f)
                        npc.ai[0] = -50f;
                }
                else
                {
                    npc.velocity *= 0.9f;
                    if (npc.velocity.X.Abs() < MaxSpeedX / 2f)
                        npc.ai[0] = 0f;
                }
            }
            if (Main.rand.NextBool(3))
            {
                int d3 = Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
                Main.dust[d3].noGravity = Main.rand.NextBool();
            }
        }

        private static byte GetIntensity()
        {
            return (byte)(150 + (int)((Math.Sin(Main.GlobalTime * 5f) + 1.0) / 2.0 * 80.0));
        }

        public override Color? GetAlpha(Color drawColor)
        {
            byte i = GetIntensity();
            return new Color(i * 2, i * 2, i * 2, i);
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] <= 120f)
            {
                npc.frameCounter++;
                if (npc.frameCounter > 5.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y += frameHeight;
                    if (npc.frame.Y > frameHeight * FRAME_FLY_3)
                        npc.frame.Y = 0;
                }
            }
            else if (npc.ai[0] < 300f)
            {
                npc.frameCounter = 0.0;
                if (npc.velocity.X > 2f)
                {
                    npc.frame.Y = frameHeight * FRAME_OPEN_MOUTH_0;
                }
                else
                {
                    npc.frame.Y = frameHeight * FRAME_OPEN_MOUTH_1;
                }
            }
            else if (npc.ai[0] < 800f)
            {
                var difference = npc.Center - Main.player[npc.target].Center;
                float length = difference.Length();
                if ((int)npc.frameCounter > 0 || length < npc.width * 1.5f)
                {
                    if ((int)npc.frameCounter == 0)
                    {
                        npc.frame.Y = frameHeight * FRAME_CHOMP_0;
                    }
                    else
                    {
                        if ((int)npc.frameCounter / 4 % 2 == 0)
                        {
                            npc.frame.Y = frameHeight * FRAME_CHOMP_1;
                        }
                        else
                        {
                            npc.frame.Y = frameHeight * FRAME_CHOMP_2;
                        }
                    }
                    npc.frameCounter++;
                }
                else if (length < npc.width * 3f)
                {
                    npc.frame.Y = frameHeight * FRAME_OPEN_MOUTH_3;
                }
                else if (length < npc.width * 6f)
                {
                    npc.frame.Y = frameHeight * FRAME_OPEN_MOUTH_2;
                }
            }
            else
            {
                if (npc.frameCounter > 5.0)
                {
                    if (npc.frameCounter > 9.0)
                        npc.frameCounter = 9.0;
                    npc.frameCounter--;
                    npc.frame.Y = frameHeight * FRAME_OPEN_MOUTH_2;
                }
                else
                {
                    npc.frameCounter += MathHelper.Clamp(npc.velocity.X.Abs() / 12f, 0.25f, 1f);
                    if (npc.frameCounter > 5.0)
                    {
                        npc.frameCounter = 0.0;
                        npc.frame.Y += frameHeight;
                        if (npc.frame.Y > frameHeight * FRAME_FLY_3)
                            npc.frame.Y = 0;
                    }
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int count = 1;
            if (npc.life <= 0)
                count = 20;
            for (int i = 0; i < count; i++)
            {
                Dust.NewDust(npc.position + new Vector2(0f, -8f), npc.width, npc.height, DustID.Fire);
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.NextBool(24))
                Item.NewItem(npc.getRect(), ModContent.ItemType<HellBeamDye>());
            if (Main.rand.NextBool(12))
                Item.NewItem(npc.getRect(), ItemID.MagmaStone);
            if (Main.rand.NextBool(Main.expertMode ? 12 : 16) && Content.World.Events.DemonSiege.DemonSiege.IsActive)
                Item.NewItem(npc.getRect(), ModContent.ItemType<DegenerationRing>());
            if (Main.rand.NextBool())
                Item.NewItem(npc.getRect(), ModContent.ItemType<DemonicEnergy>());
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            var screenPos = Main.screenPosition;
            var texture = Main.npcTexture[npc.type];
            var origin = npc.frame.Size() / 2f;
            var offset = new Vector2(npc.width / 2f, npc.height / 2f + npc.gfxOffY);

            drawColor = npc.GetAlpha(drawColor);

            byte intensity = GetIntensity();
            if ((int)npc.ai[0] > 300 && (int)npc.ai[0] < 800)
                intensity = 0;

            var effects = npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (intensity > 150)
            {
                intensity = (byte)(int)(intensity * AQConfigClient.Instance.EffectIntensity);
                float value = (intensity - 150) / 33f;
                var c = drawColor * 0.08f * value;
                if (AQConfigClient.Instance.EffectQuality >= 1f)
                {
                    var spotlight = AQTextures.Lights[LightTex.Spotlight66x66];
                    var spotlightOrigin = spotlight.Size() / 2f;
                    Main.spriteBatch.Draw(spotlight, npc.position + offset - screenPos, null, new Color(255, 150, 10, 0) * value, npc.rotation, spotlightOrigin, npc.scale * value * 0.15f, effects, 0f);
                    Main.spriteBatch.Draw(spotlight, npc.position + offset - screenPos, null, new Color(255, 150, 10, 0) * value * 0.1f, npc.rotation, spotlightOrigin, npc.scale * value * 0.3f, effects, 0f);
                }
                Main.spriteBatch.Draw(texture, npc.position + offset - screenPos, npc.frame, drawColor, npc.rotation, origin, npc.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, npc.position + offset + new Vector2(value, 0f) - screenPos, npc.frame, c, npc.rotation, origin, npc.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, npc.position + offset + new Vector2(-value, 0f) - screenPos, npc.frame, c, npc.rotation, origin, npc.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, npc.position + offset + new Vector2(0f, value) - screenPos, npc.frame, c, npc.rotation, origin, npc.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, npc.position + offset + new Vector2(0f, -value) - screenPos, npc.frame, c, npc.rotation, origin, npc.scale, effects, 0f);
            }
            else
            {
                Main.spriteBatch.Draw(texture, npc.position + offset - screenPos, npc.frame, drawColor, npc.rotation, origin, npc.scale, effects, 0f);
            }
            return false;
        }

        bool IDecideFallThroughPlatforms.Decide()
        {
            if (Main.player[npc.target].dead)
            {
                return true;
            }
            else
            {
                return Main.player[npc.target].position.Y
                    > npc.position.Y + npc.height;
            }
        }
    }
}