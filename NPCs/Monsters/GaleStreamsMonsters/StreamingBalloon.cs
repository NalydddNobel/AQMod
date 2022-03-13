using AQMod.Common;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Items.Dyes.Hair;
using AQMod.Items.Potions.Foods;
using AQMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreamsMonsters
{
    public class StreamingBalloon : ModNPC
    {
        private bool _setupFrame = false;
        public const int FramesX = 8;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 2;

            NPCID.Sets.TrailingMode[npc.type] = 7;
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.width = 26;
            npc.height = 26;
            npc.lifeMax = 1;
            npc.damage = 0;
            npc.defense = 0;
            npc.DeathSound = SoundID.Item111;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.noTileCollide = true;
            AQNPC.ImmuneToAllBuffs(npc);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Gore.NewGore(new Vector2(npc.position.X + npc.width / 2, npc.position.Y + 30),
                new Vector2(3f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)),
                ModGore.GetGoreSlot("AQMod/Gores/GaleStreams/BalloonTextureCopy"));
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int oldLife = Main.npc[(int)npc.ai[0]].life;
                Main.npc[(int)npc.ai[0]].SetDefaults((int)npc.ai[2]);
                Main.npc[(int)npc.ai[0]].life = oldLife;
                Main.npc[(int)npc.ai[0]].target = npc.target;
                Main.npc[(int)npc.ai[0]].ai[1] = npc.ai[1];
            }
        }

        public override void AI()
        {
            if ((int)npc.ai[0] == -1)
            {
                npc.velocity.Y -= 0.1f;
                if (npc.timeLeft > 30)
                {
                    npc.timeLeft = 30;
                }
                return;
            }
            if ((int)npc.localAI[0] == 0f)
            {
                npc.TargetClosest(faceTarget: false);
                npc.localAI[0] = Main.rand.Next(FramesX - 1) + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.HasValidTarget)
                    {
                        if (AprilFoolsJoke.Active)
                        {
                            npc.ai[2] = NPCID.LavaSlime;
                        }
                        else if (XmasSeeds.XmasWorld)
                        {
                            npc.ai[2] = Main.rand.NextBool() ? NPCID.IceSlime : NPCID.SpikedIceSlime;
                        }
                        else
                        {
                            int[] selectableEnemies = new int[]
                            {
                                NPCID.RedSlime,
                                NPCID.PurpleSlime,
                                NPCID.BlackSlime,
                                NPCID.JungleSlime,
                                NPCID.IceSlime,
                                NPCID.SpikedJungleSlime,
                                NPCID.SpikedIceSlime,
                            };
                            if (WorldDefeats.SudoHardmode)
                            {
                                Array.Resize(ref selectableEnemies, selectableEnemies.Length + 1);
                                selectableEnemies[selectableEnemies.Length - 1] = NPCID.ToxicSludge;
                            }
                            if (Main.hardMode && Main.player[npc.target].ZoneHoly)
                            {
                                Array.Resize(ref selectableEnemies, selectableEnemies.Length + 1);
                                selectableEnemies[selectableEnemies.Length - 1] = NPCID.IlluminantSlime;
                            }
                            npc.ai[2] = selectableEnemies[Main.rand.Next(selectableEnemies.Length)];
                        }
                        int n = NPC.NewNPC((int)npc.position.X + npc.width / 2, (int)npc.position.Y + 66, (int)npc.ai[2]);
                        Main.npc[n].hide = true;
                        Main.npc[n].noTileCollide = true;
                        Main.npc[n].knockBackResist = 0f;
                        if (((int)npc.ai[2] == NPCID.BlueSlime ||
                            (int)npc.ai[2] < 0) && Main.rand.NextBool())
                        {
                            int[] selectableLoot = new int[]
                            {
                                ItemID.ShinyRedBalloon,
                                ItemID.Starfury,
                                ModContent.ItemType<CinnamonRoll>(),
                                ModContent.ItemType<TemperatureHairDye>(),
                            };
                            if (WorldDefeats.SudoHardmode)
                            {
                                Array.Resize(ref selectableLoot, selectableLoot.Length + 1);
                                selectableLoot[selectableLoot.Length - 1] = ModContent.ItemType<Items.Weapons.Magic.Umystick>();
                            }
                            npc.ai[1] = selectableLoot[Main.rand.Next(selectableLoot.Length)];
                        }
                        npc.ai[0] = n;
                    }
                    else
                    {
                        npc.life = 0;
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    npc.netUpdate = true;
                }
            }
            if (!Main.npc[(int)npc.ai[0]].active)
            {
                if (WorldDefeats.SudoHardmode && Main.rand.NextBool(4))
                    Item.NewItem(new Vector2(npc.position.X + npc.width / 2f - Main.npc[(int)npc.ai[0]].width / 2f, npc.position.Y + 66 - Main.npc[(int)npc.ai[0]].height / 2), ModContent.ItemType<Items.Materials.Energies.AtmosphericEnergy>());
                npc.ai[0] = -1f;
                npc.netUpdate = true;
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.npc[(int)npc.ai[0]].netID != (int)npc.ai[2])
                {
                    npc.ai[0] = -1f;
                    npc.netUpdate = true;
                    return;
                }
            }

            if (npc.direction == 0)
            {
                if (Main.player[npc.target].position.X + Main.player[npc.target].width / 2f > npc.position.X + npc.width / 2f)
                {
                    npc.direction = 1;
                }
                else
                {
                    npc.direction = -1;
                }
            }

            if (npc.direction == -1)
            {
                if (npc.velocity.X > -7f)
                    npc.velocity.X -= 0.06f;
            }
            else
            {
                if (npc.velocity.X < 7f)
                    npc.velocity.X += 0.06f;
            }

            float distance = npc.Distance(Main.player[npc.target].Center);
            if (distance > 1650f)
            {
                npc.life = 0;
                SoundID.Item111.Play(npc.Center, 1.5f, 0.9f);
                npc.HitEffect();
                npc.active = false;
            }

            if (Collision.SolidCollision(npc.position, npc.width, 100))
            {
                if (distance < 40f)
                {
                    npc.life = 0;
                    SoundID.Item111.Play(npc.Center, 1.5f, 0.9f);
                    npc.HitEffect();
                    npc.active = false;
                }
                if (npc.position.Y > Main.player[npc.target].position.Y)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.92f;
                    if (npc.velocity.Y > -7f)
                        npc.velocity.Y -= 0.1f;
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.92f;
                    if (npc.velocity.Y < 7f)
                        npc.velocity.Y += 0.1f;
                }
            }
            else
            {
                if (distance < 150f)
                {
                    npc.life = 0;
                    SoundID.Item111.Play(npc.Center, 1.5f, 0.9f);
                    npc.HitEffect();
                    npc.active = false;
                }
                if (npc.position.Y > Main.player[npc.target].position.Y - 100f + Math.Sin(Main.time / 60f) * 30f)
                {
                    if (npc.velocity.Y > -7f)
                        npc.velocity.Y -= 0.1f;
                }
                else
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.92f;
                    if (npc.velocity.Y < 12f)
                        npc.velocity.Y += 0.025f;
                    if ((npc.position.X + npc.width / 2f - Main.player[npc.target].position.X + Main.player[npc.target].width / 2f).Abs() < 100f)
                    {
                        npc.life = 0;
                        SoundID.Item111.Play(npc.Center, 1.5f, 0.9f);
                        npc.HitEffect();
                        npc.active = false;
                    }
                }
            }

            npc.rotation = npc.velocity.X * 0.01f;

            if (npc.active)
            {
                Main.npc[(int)npc.ai[0]].velocity = npc.velocity;
                Main.npc[(int)npc.ai[0]].position.X = npc.position.X + npc.width / 2f - Main.npc[(int)npc.ai[0]].width / 2f;
                Main.npc[(int)npc.ai[0]].position.Y = npc.position.Y + 66 - Main.npc[(int)npc.ai[0]].height / 2;
                Main.npc[(int)npc.ai[0]].position -= npc.velocity;
                Main.npc[(int)npc.ai[0]].ai[0] = 0f;
                Main.npc[(int)npc.ai[0]].hide = true;
                Main.npc[(int)npc.ai[0]].noTileCollide = true;
                Main.npc[(int)npc.ai[0]].knockBackResist = 0f;
            }
            if (npc.ai[1] > 0f)
            {
                Main.npc[(int)npc.ai[0]].ai[1] = npc.ai[1];
            }
        }

        public static void GetDrawInfo(int netID, Color lightColor, out int frameX, out Color alpha, out Color npcClr, out float scale)
        {
            NPC npc = new NPC();
            npc.SetDefaults(netID);
            frameX = NPCIDToFrameIndex(npc.type);
            alpha = npc.GetAlpha(lightColor);
            scale = npc.scale;
            npcClr = npc.color;
            if (npcClr != default(Color))
            {
                npcClr = npc.GetColor(lightColor);
            }
        }

        public static int NPCIDToFrameIndex(int type)
        {
            switch (type)
            {
                case NPCID.BlueSlime:
                    return 1;
                case NPCID.LavaSlime:
                    return 2;
                case NPCID.ToxicSludge:
                    return 3;
                case NPCID.SpikedJungleSlime:
                    return 4;
                case NPCID.IceSlime:
                    return 5;
                case NPCID.SpikedIceSlime:
                    return 6;
                case NPCID.IlluminantSlime:
                    return 7;
            }
            return -1;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!_setupFrame)
            {
                _setupFrame = true;
                npc.frame.Width /= FramesX;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            var drawPosition = npc.Center;
            var origin = new Vector2(npc.frame.Width / 2f, npc.frame.Height / 4f);
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, npc.frame, drawColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, new Rectangle(npc.frame.X + npc.frame.Width * (int)npc.localAI[0], npc.frame.Y, npc.frame.Width, npc.frame.Height), drawColor, npc.rotation, origin, npc.scale, SpriteEffects.None, 0f);

            if ((int)npc.ai[0] == -1)
            {
                return false;
            }

            GetDrawInfo((int)npc.ai[2], drawColor, out int frameX, out var alpha, out var npcClr, out float scale);
            if (scale != 1f)
            {
                float y = 70f - origin.Y;
                drawPosition.Y += y - y * scale; // keeps the slime's top on the same Y coordinate
            }

            if ((int)npc.ai[1] > 0)
            {
                int itemType = (int)npc.ai[1];
                float num199 = 1f;
                float num200 = 20f * scale * npc.scale;
                float num201 = 18f * scale * npc.scale;
                float num202 = Main.itemTexture[itemType].Width;
                float num203 = Main.itemTexture[itemType].Height;
                if (num202 > num200)
                {
                    num199 *= num200 / num202;
                    num203 *= num199;
                }
                if (num203 > num201)
                {
                    num199 *= num201 / num203;
                }
                float num205 = -1f;
                float num206 = 1f;
                int somethingelse = 13;
                num206 -= 13f;
                float itemscale = 0.2f;
                itemscale -= 0.3f * somethingelse;
                spriteBatch.Draw(Main.itemTexture[itemType], new Vector2(drawPosition.X + num205 - Main.screenPosition.X, drawPosition.Y + num206 - Main.screenPosition.Y + 66f), null, drawColor, itemscale, new Vector2(Main.itemTexture[itemType].Width / 2, Main.itemTexture[itemType].Height / 2), num199, SpriteEffects.None, 0f);
            }

            if ((int)npc.ai[2] == NPCID.IlluminantSlime)
            {
                for (int i = 1; i < npc.oldPos.Length; i++)
                {
                    spriteBatch.Draw(texture, new Vector2(npc.oldPos[i].X - Main.screenPosition.X + npc.width / 2,
                        npc.oldPos[i].Y - Main.screenPosition.Y + npc.height / 2f),
                        new Rectangle(npc.frame.X + npc.frame.Width * frameX, npc.frame.Y + npc.frame.Height, npc.frame.Width, npc.frame.Height),
                        new Color(150 * (10 - i) / 15, 100 * (10 - i) / 15, 150 * (10 - i) / 15, 50 * (10 - i) / 15), npc.rotation, origin, npc.scale * scale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, new Rectangle(npc.frame.X + npc.frame.Width * frameX, npc.frame.Y + npc.frame.Height, npc.frame.Width, npc.frame.Height), alpha, npc.rotation, origin, npc.scale * scale, SpriteEffects.None, 0f);
            if (npcClr != default(Color))
            {
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, new Rectangle(npc.frame.X + npc.frame.Width * frameX, npc.frame.Y + npc.frame.Height, npc.frame.Width, npc.frame.Height), npcClr, npc.rotation, origin, npc.scale * scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}