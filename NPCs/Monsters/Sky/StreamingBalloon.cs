using Aequus.Common;
using Aequus.Items.Misc.Energies;
using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace Aequus.NPCs.Monsters.Sky
{
    public class StreamingBalloon : ModNPC
    {
        private bool _setupFrame = false;
        public const int FramesX = 8;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.TrailingMode[NPC.type] = 7;
            NPCID.Sets.TrailCacheLength[NPC.type] = 20;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 26;
            NPC.height = 26;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.DeathSound = SoundID.Item111;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            //Gore.NewGore(new Vector2(NPC.position.X + NPC.width / 2, NPC.position.Y + 30),
            //    new Vector2(3f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)),
            //    ModGore.GetGoreSlot("AQMod/Gores/GaleStreams/BalloonTextureCopy"));
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int oldLife = Main.npc[(int)NPC.ai[0]].life;
                Main.npc[(int)NPC.ai[0]].SetDefaults((int)NPC.ai[2]);
                Main.npc[(int)NPC.ai[0]].life = oldLife;
                Main.npc[(int)NPC.ai[0]].target = NPC.target;
                Main.npc[(int)NPC.ai[0]].ai[1] = NPC.ai[1];
            }
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == -1)
            {
                NPC.velocity.Y -= 0.1f;
                if (NPC.timeLeft > 30)
                {
                    NPC.timeLeft = 30;
                }
                return;
            }
            if ((int)NPC.localAI[0] == 0f)
            {
                NPC.TargetClosest(faceTarget: false);
                NPC.localAI[0] = Main.rand.Next(FramesX - 1) + 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.HasValidTarget)
                    {
                        if (AprilFools.CheckAprilFools())
                        {
                            NPC.ai[2] = NPCID.LavaSlime;
                        }
                        //else if (XmasSeed.XmasWorld)
                        //{
                        //    NPC.ai[2] = Main.rand.NextBool() ? NPCID.IceSlime : NPCID.SpikedIceSlime;
                        //}
                        else
                        {
                            List<int> selectableEnemies = new List<int>()
                            {
                                NPCID.RedSlime,
                                NPCID.PurpleSlime,
                                NPCID.BlackSlime,
                                NPCID.JungleSlime,
                                NPCID.IceSlime,
                                NPCID.SpikedJungleSlime,
                                NPCID.SpikedIceSlime,
                            };
                            if (WorldFlags.HardmodeTier)
                            {
                                selectableEnemies.Add(NPCID.ToxicSludge);
                            }
                            if (Main.hardMode && Main.player[NPC.target].ZoneHallow)
                            {
                                selectableEnemies.Add(NPCID.IlluminantSlime);
                            }
                            NPC.ai[2] = selectableEnemies[Main.rand.Next(selectableEnemies.Count)];
                        }
                        int n = NPC.NewNPC(NPC.GetSpawnSource_NPCCatch(NPC.whoAmI), (int)NPC.position.X + NPC.width / 2, (int)NPC.position.Y + 66, (int)NPC.ai[2]);
                        Main.npc[n].hide = true;
                        Main.npc[n].noTileCollide = true;
                        Main.npc[n].knockBackResist = 0f;
                        if (((int)NPC.ai[2] == NPCID.BlueSlime ||
                            (int)NPC.ai[2] < 0) && Main.rand.NextBool())
                        {
                            int[] selectableLoot = new int[]
                            {
                                ItemID.ShinyRedBalloon,
                                ItemID.Starfury,
                                //ModContent.ItemType<CinnamonRoll>(),
                                //ModContent.ItemType<TemperatureHairDye>(),
                            };
                            //if (WorldFlags.SudoHardmode)
                            //{
                            //    Array.Resize(ref selectableLoot, selectableLoot.Length + 1);
                            //    selectableLoot[selectableLoot.Length - 1] = ModContent.ItemType<Items.Weapons.Magic.Umystick>();
                            //}
                            NPC.ai[1] = selectableLoot[Main.rand.Next(selectableLoot.Length)];
                        }
                        NPC.ai[0] = n;
                    }
                    else
                    {
                        NPC.life = 0;
                        NPC.active = false;
                        NPC.netUpdate = true;
                    }
                    NPC.netUpdate = true;
                }
            }
            if (!Main.npc[(int)NPC.ai[0]].active)
            {
                if (WorldFlags.HardmodeTier && Main.rand.NextBool(4))
                    Item.NewItem(NPC.GetItemSource_Loot(), new Vector2(NPC.position.X + NPC.width / 2f - Main.npc[(int)NPC.ai[0]].width / 2f, NPC.position.Y + 66 - Main.npc[(int)NPC.ai[0]].height / 2), ModContent.ItemType<AtmosphericEnergy>());
                NPC.ai[0] = -1f;
                NPC.netUpdate = true;
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (Main.npc[(int)NPC.ai[0]].netID != (int)NPC.ai[2])
                {
                    NPC.ai[0] = -1f;
                    NPC.netUpdate = true;
                    return;
                }
            }

            if (NPC.direction == 0)
            {
                if (Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f > NPC.position.X + NPC.width / 2f)
                {
                    NPC.direction = 1;
                }
                else
                {
                    NPC.direction = -1;
                }
            }

            if (NPC.direction == -1)
            {
                if (NPC.velocity.X > -7f)
                    NPC.velocity.X -= 0.06f;
            }
            else
            {
                if (NPC.velocity.X < 7f)
                    NPC.velocity.X += 0.06f;
            }

            float distance = NPC.Distance(Main.player[NPC.target].Center);
            if (distance > 1650f)
            {
                NPC.life = 0;
                SoundID.Item111.Play(NPC.Center, 1.5f, 0.9f);
                NPC.HitEffect();
                NPC.active = false;
            }

            if (Collision.SolidCollision(NPC.position, NPC.width, 100))
            {
                if (distance < 40f)
                {
                    NPC.life = 0;
                    SoundID.Item111.Play(NPC.Center, 1.5f, 0.9f);
                    NPC.HitEffect();
                    NPC.active = false;
                }
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y > -7f)
                        NPC.velocity.Y -= 0.1f;
                }
                else
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y < 7f)
                        NPC.velocity.Y += 0.1f;
                }
            }
            else
            {
                if (distance < 150f)
                {
                    NPC.life = 0;
                    SoundID.Item111.Play(NPC.Center, 1.5f, 0.9f);
                    NPC.HitEffect();
                    NPC.active = false;
                }
                if (NPC.position.Y > Main.player[NPC.target].position.Y - 100f + Math.Sin(Main.time / 60f) * 30f)
                {
                    if (NPC.velocity.Y > -7f)
                        NPC.velocity.Y -= 0.1f;
                }
                else
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.92f;
                    if (NPC.velocity.Y < 12f)
                        NPC.velocity.Y += 0.025f;
                    if ((NPC.position.X + NPC.width / 2f - Main.player[NPC.target].position.X + Main.player[NPC.target].width / 2f).Abs() < 100f)
                    {
                        NPC.life = 0;
                        SoundID.Item111?.Play(NPC.Center, 1.5f, 0.9f);
                        NPC.HitEffect();
                        NPC.active = false;
                    }
                }
            }

            NPC.rotation = NPC.velocity.X * 0.01f;

            if (NPC.active)
            {
                Main.npc[(int)NPC.ai[0]].velocity = NPC.velocity;
                Main.npc[(int)NPC.ai[0]].position.X = NPC.position.X + NPC.width / 2f - Main.npc[(int)NPC.ai[0]].width / 2f;
                Main.npc[(int)NPC.ai[0]].position.Y = NPC.position.Y + 66 - Main.npc[(int)NPC.ai[0]].height / 2;
                Main.npc[(int)NPC.ai[0]].position -= NPC.velocity;
                Main.npc[(int)NPC.ai[0]].ai[0] = 0f;
                Main.npc[(int)NPC.ai[0]].hide = true;
                Main.npc[(int)NPC.ai[0]].noTileCollide = true;
                Main.npc[(int)NPC.ai[0]].knockBackResist = 0f;
            }
            if (NPC.ai[1] > 0f)
            {
                Main.npc[(int)NPC.ai[0]].ai[1] = NPC.ai[1];
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
                NPC.frame.Width /= FramesX;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var drawPosition = NPC.Center;
            var origin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 4f);
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, new Rectangle(NPC.frame.X + NPC.frame.Width * (int)NPC.localAI[0], NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);

            if ((int)NPC.ai[0] == -1)
            {
                return false;
            }

            GetDrawInfo((int)NPC.ai[2], drawColor, out int frameX, out var alpha, out var npcClr, out float scale);
            if (scale != 1f)
            {
                float y = 70f - origin.Y;
                drawPosition.Y += y - y * scale; // keeps the slime's top on the same Y coordinate
            }

            if ((int)NPC.ai[1] > 0)
            {
                int itemType = (int)NPC.ai[1];
                float num199 = 1f;
                float num200 = 20f * scale * NPC.scale;
                float num201 = 18f * scale * NPC.scale;
                Main.instance.LoadItem(itemType);
                var itemTexture = TextureAssets.Item[itemType].Value;
                float num202 = itemTexture.Width;
                float num203 = itemTexture.Height;
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
                spriteBatch.Draw(itemTexture, new Vector2(drawPosition.X + num205 - Main.screenPosition.X, drawPosition.Y + num206 - Main.screenPosition.Y + 66f), null, drawColor, itemscale, itemTexture.Size() / 2f, num199, SpriteEffects.None, 0f);
            }

            if ((int)NPC.ai[2] == NPCID.IlluminantSlime)
            {
                for (int i = 1; i < NPC.oldPos.Length; i++)
                {
                    spriteBatch.Draw(texture, new Vector2(NPC.oldPos[i].X - Main.screenPosition.X + NPC.width / 2,
                        NPC.oldPos[i].Y - Main.screenPosition.Y + NPC.height / 2f),
                        new Rectangle(NPC.frame.X + NPC.frame.Width * frameX, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height),
                        new Color(150 * (10 - i) / 15, 100 * (10 - i) / 15, 150 * (10 - i) / 15, 50 * (10 - i) / 15), NPC.rotation, origin, NPC.scale * scale, SpriteEffects.None, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, new Rectangle(NPC.frame.X + NPC.frame.Width * frameX, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height), alpha, NPC.rotation, origin, NPC.scale * scale, SpriteEffects.None, 0f);
            if (npcClr != default(Color))
            {
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, new Rectangle(NPC.frame.X + NPC.frame.Width * frameX, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height), npcClr, NPC.rotation, origin, NPC.scale * scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}