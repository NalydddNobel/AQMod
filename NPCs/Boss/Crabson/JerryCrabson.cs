using AQMod.Common;
using AQMod.Common.CrossMod.BossChecklist;
using AQMod.Common.WorldGeneration;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Items.BossItems.Crabson;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Tools;
using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Melee;
using AQMod.Items.Weapons.Ranged;
using AQMod.Localization;
using AQMod.Projectiles.Monster;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Boss.Crabson
{
    [AutoloadBossHead]
    public class JerryCrabson : AQBoss, IModifiableMusicNPC
    {
        public override BossEntry? BossChecklistEntry => new BossEntry(
            () => WorldDefeats.DownedCrabson,
            2f,
            ModContent.NPCType<JerryCrabson>(),
            AQText.chooselocalizationtext("Jerry Crabson", "巨蟹蛤"),
            ModContent.ItemType<MushroomClam>(),
            new List<int>() {
                ModContent.ItemType<Crabsol>(),
                ModContent.ItemType<JerryClawFlail>(),
                ModContent.ItemType<CinnabarBow>(),
                ModContent.ItemType<Bubbler>(),
                ModContent.ItemType<AquaticEnergy>(),
            },
            new List<int>()
            {
                ModContent.ItemType<CrabsonTrophy>(),
                ModContent.ItemType<CrabsonMask>(),
            },
            AQText.chooselocalizationtext(
                en_US: "Summoned by using a [i:" + ModContent.ItemType<MushroomClam>() + "] at the beach.",
                zh_Hans: null),
            "AQMod/Assets/BossChecklist/JerryCrabson");

        public override void SetDefaults()
        {
            npc.width = 90;
            npc.height = 60;
            npc.lifeMax = 1000;
            npc.damage = 32;
            npc.defense = 8;
            npc.aiStyle = -1;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.boss = true;
            npc.behindTiles = true;
            bossBag = ModContent.ItemType<CrabsonBag>();
            npc.buffImmune[BuffID.Suffocation] = true;
            if (AQMod.CanUseAssets)
            {
                music = GetMusic().GetMusicID();
                musicPriority = MusicPriority.BossLow;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.5f * 1.5f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position + new Vector2(npc.width / 2f - 10f, 0f), npc.velocity, mod.GetGoreSlot("Gores/JerryCrabson_0"));
                Gore.NewGore(npc.position + new Vector2(npc.width / 2f + 10f, 0f), npc.velocity, mod.GetGoreSlot("Gores/JerryCrabson_0"));
                Gore.NewGore(npc.position + new Vector2(npc.width / 2f, 10f), npc.velocity, mod.GetGoreSlot("Gores/JerryCrabson_1"));
                for (int i = 0; i < 50; i++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
                if (Main.netMode != NetmodeID.Server)
                {
                    var chain = ModContent.GetTexture(AQUtils.GetPath<JerryClaw>("_Chain"));
                    int height = chain.Height - 2;
                    var origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
                    var endPosition = Main.npc[(int)npc.localAI[0]].Center;
                    var velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - npc.Center) * height;
                    var position = npc.Center;
                    for (int i = 0; i < 50; i++)
                    {
                        Gore.NewGore(position + new Vector2(10f, 0f), npc.velocity, mod.GetGoreSlot("Gores/JerryChain_0"));
                        Gore.NewGore(position + new Vector2(-10f, 0f), npc.velocity, mod.GetGoreSlot("Gores/JerryChain_0"));
                        Gore.NewGore(position, npc.velocity, mod.GetGoreSlot("Gores/JerryChain_1"));
                        velo = Vector2.Normalize(Vector2.Lerp(velo, endPosition - position, 0.02f + MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 300f, 0f, 0.98f))) * height;
                        position += velo;
                        if (Vector2.Distance(position, endPosition) <= height)
                            break;
                    }
                    endPosition = Main.npc[(int)npc.localAI[1]].Center;
                    velo = Vector2.Normalize(endPosition + new Vector2(0f, height * 4f) - npc.Center) * height;
                    position = npc.Center;
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(position - new Vector2(height, height), height * 2, height * 2, DustID.Blood);
                        Gore.NewGore(position + new Vector2(10f, 0f), npc.velocity, mod.GetGoreSlot("Gores/JerryChain_0"));
                        Gore.NewGore(position + new Vector2(-10f, 0f), npc.velocity, mod.GetGoreSlot("Gores/JerryChain_0"));
                        Gore.NewGore(position, npc.velocity, mod.GetGoreSlot("Gores/JerryChain_1"));
                        velo = Vector2.Normalize(Vector2.Lerp(velo, endPosition - position, 0.02f + MathHelper.Clamp(1f - Vector2.Distance(endPosition, position) / 300f, 0f, 0.98f))) * height;
                        position += velo;
                        if (Vector2.Distance(position, endPosition) <= height)
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < damage / 20 + 1; i++)
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
            }
        }

        public override void AI()
        {
            Vector2 center = npc.Center;
            if (npc.ai[3] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[3] = 1;
                npc.netUpdate = true;
                int n = NPC.NewNPC((int)center.X, (int)center.Y, ModContent.NPCType<JerryClaw>());
                npc.localAI[0] = n;
                Main.npc[n].position.X -= 150f;
                Main.npc[n].ai[0] = npc.whoAmI;
                Main.npc[n].direction = -1;
                Main.npc[n].spriteDirection = -1;
                int n1 = NPC.NewNPC((int)center.X, (int)center.Y, ModContent.NPCType<JerryClaw>());
                npc.localAI[1] = n1;
                Main.npc[n1].position.X += 150f;
                Main.npc[n1].ai[0] = npc.whoAmI;
                Main.npc[n1].direction = 1;
                Main.npc[n1].spriteDirection = 1;
                int length = 35;
                Main.npc[n].localAI[0] = length;
                Main.npc[n1].localAI[0] = length;
                npc.TargetClosest();
                Main.npc[n].target = npc.target;
                Main.npc[n1].target = npc.target;
                CrabSeason.CrabsonCachedID = (short)npc.whoAmI;
            }
            if (!Main.npc[(int)npc.localAI[0]].active || !Main.npc[(int)npc.localAI[1]].active)
            {
                npc.lifeMax = -1;
                npc.HitEffect();
                npc.active = false;
                return;
            }
            if (!npc.HasValidTarget)
            {
                npc.ai[2] = -1;
                if (npc.timeLeft > 80)
                    npc.timeLeft = 80;
            }
            if ((int)npc.ai[2] == -1)
            {
                if (npc.velocity.Y < 0)
                    npc.velocity.Y = 0;
                npc.velocity.Y += 0.1f;
                return;
            }
            bool wet = Collision.WetCollision(npc.position, npc.width, npc.height);
            if (wet)
            {
                if ((int)npc.ai[1] == 0)
                    npc.ai[1] = 1f;
            }
            else
            {
                npc.ai[1] = 2f;
            }
            if ((int)npc.ai[2] == 0 || (int)npc.ai[2] == 1)
            {
                NPC right = Main.npc[(int)npc.localAI[0]];
                NPC left = Main.npc[(int)npc.localAI[1]];
                Vector2 center1 = right.Center;
                Vector2 center2 = left.Center;
                float x = center1.X - (center1 - center2).X / 2;
                float distanceX = x - center.X;
                if (!HandleGravity(center, center1, center2))
                {
                    TileDust();
                    npc.soundDelay += (int)npc.velocity.X.Abs();
                    if (npc.soundDelay > 50)
                    {
                        npc.soundDelay = 0;
                        Main.PlaySound(SoundID.Roar, npc.position, 1);
                        Collision.HitTiles(npc.position, npc.velocity, npc.width, npc.height);
                    }
                    if (Math.Abs(distanceX) > 12f)
                    {
                        int dir = distanceX > 0 ? 1 : -1;
                        float lerpAmount = 0.02f;
                        npc.velocity.X = MathHelper.Lerp(npc.velocity.X, (Main.expertMode ? 10f : 4f) * dir, lerpAmount);
                    }
                }
                else
                {
                    if (npc.velocity.Y > 0.1f)
                        npc.velocity.X *= 0.96f;
                }
                npc.ai[3]++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if ((int)npc.ai[2] == 1)
                    {
                        if ((int)npc.ai[3] < 240)
                        {
                            if ((int)npc.ai[3] % 60 == 0)
                            {
                                Projectile.NewProjectile(new Vector2(npc.position.X + npc.width / 2f, npc.position.Y), new Vector2(0f + npc.velocity.X * 0.25f, -10f), ModContent.ProjectileType<JerryBubbleBig>(), 10, 1f, Main.myPlayer);
                                Main.PlaySound(SoundID.Item85, npc.position);
                            }
                        }
                        int resetTime = 450;
                        if (!wet)
                            resetTime = 250;
                        if ((int)npc.ai[3] > resetTime)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[3] = 1f;
                        }
                    }
                    else
                    {
                        if (!wet)
                        {
                            if (npc.ai[3] > 300)
                            {
                                if (npc.ai[3] % (Main.expertMode ? 30 : 50) == 0f)
                                {
                                    Projectile.NewProjectile(new Vector2(npc.position.X + npc.width / 2f, npc.position.Y), new Vector2(0f + npc.velocity.X * 0.25f, -6f), ModContent.ProjectileType<JerryBubble>(), 10, 1f, Main.myPlayer);
                                    Main.PlaySound(SoundID.Item85, npc.position);
                                }
                                if (npc.ai[3] > 450)
                                {
                                    npc.ai[3] = 1f;
                                    if (Main.rand.NextBool(3) && AQProjectile.CountProjectiles(ModContent.ProjectileType<JerryBubbleBig>()) <= 1)
                                        npc.ai[2] = 1f;
                                }
                            }
                        }
                        else
                        {
                            if (npc.ai[3] >= (Main.expertMode ? 20 : 35))
                            {
                                Projectile.NewProjectile(new Vector2(npc.position.X + npc.width / 2f, npc.position.Y), new Vector2(0f + npc.velocity.X * 0.25f, -6f), ModContent.ProjectileType<JerryBubble>(), 10, 1f, Main.myPlayer);
                                Main.PlaySound(SoundID.Item85, npc.position);
                                npc.ai[3] = 1f;
                                if (Main.rand.NextBool(10))
                                    npc.ai[2] = 1f;
                            }
                        }
                    }
                }
            }
        }

        private void FallDust(int x, int y, int crabsonX, int crabsonY)
        {
            switch (Main.tile[x, y].type)
            {
                case TileID.Sand:
                {
                    int crabsonX2 = crabsonX + npc.width / 2;
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, 32);
                        //var position = new Vector2(crabsonX + Main.rand.Next(npc.width), crabsonY);
                        //Main.dust[Dust.NewDust(position, 2, 2, 51, 0f, 0f, 0, Color.White, 1f)].velocity = new Vector2(position.X * 0.125f - crabsonX2, -9f);
                    }
                    Main.PlaySound(SoundID.Item14, npc.position);
                }
                break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center">The center of Crabson</param>
        /// <param name="center1">The center of the right Claw</param>
        /// <param name="center2">The center of the left Claw</param>
        /// <returns>Whether Crabson should be obeying gravity</returns>
        private bool HandleGravity(Vector2 center, Vector2 center1, Vector2 center2)
        {
            var solidPosition = npc.position + new Vector2(-1f, -npc.height / 2f);
            bool fall = !Collision.SolidCollision(solidPosition, npc.width, npc.height) &&
                !Collision.WetCollision(solidPosition, npc.width, npc.height);
            bool aboveTargets = center.Y >= center1.Y &&
                center.Y >= center2.Y &&
                center.Y >= Main.player[npc.target].position.Y;
            bool forceNoGravity = Main.player[npc.target].position.Y < npc.position.Y - 400f;
            if ((fall || !aboveTargets) && !forceNoGravity)
            {
                if (aboveTargets && npc.velocity.Y > 0f)
                {
                    int crabsonX = (int)npc.position.X;
                    int crabsonY = (int)npc.position.Y + npc.height / 2;
                    int crabsonX2 = crabsonX / 16;
                    int crabsonY2 = crabsonY / 16;
                    int tileWidth = (int)(npc.width * npc.scale / 16f);
                    int tileHeight = (int)(npc.height * npc.scale / 16f);
                    for (int i = 0; i < tileWidth; i++)
                    {
                        for (int j = 0; j < tileHeight; j++)
                        {
                            if (AQWorldGen.ActiveAndSolid(crabsonX2 + i, crabsonY2 + j))
                            {
                                if (npc.Center.Y + npc.velocity.Y > (crabsonY2 + j) * 16f)
                                {
                                    npc.position.Y = (int)((crabsonY2 + j) * 16f - npc.height / 2f);
                                    if (npc.velocity.Y > 3f)
                                        FallDust(crabsonX2 + i, crabsonY2 + j, crabsonX, crabsonY);
                                    npc.velocity.Y = 0f;
                                    return false;
                                }
                                break;
                            }
                        }
                    }
                }
                npc.velocity.Y += 0.2f;
                if (npc.velocity.Y > 8)
                    npc.velocity.Y = 8f;
                return true;
            }
            else
            {
                if (npc.velocity.Y > 0)
                {
                    npc.velocity.Y = 0f;
                }
                else
                {
                    npc.velocity.Y -= 0.1f;
                    if (npc.velocity.Y < -8)
                        npc.velocity.Y = -8f;
                }
                return false;
            }
        }

        private void TileDust()
        {
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(ModContent.BuffType<Buffs.Debuffs.PickBreak>(), 480);
            }
        }

        public bool ShouldDropCrabsol()
        {
            return (int)npc.ai[1] == 1;
        }

        public override bool PreNPCLoot()
        {
            npc.position.Y -= npc.height / 2f;
            CrabSeason.CrabsonCachedID = -1;
            return true;
        }

        public override void NPCLoot()
        {
            Rectangle rect = npc.getRect();
            if (ShouldDropCrabsol())
            {
                npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), ModContent.ItemType<BreakdownDye>());
                npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), ModContent.ItemType<Crabsol>());
            }
            if (Main.rand.NextBool(10))
                Item.NewItem(rect, ModContent.ItemType<CrabsonTrophy>());
            if (CrabSeason.Active)
            {
                WorldDefeats.DownedCrabSeason = true;
            }
            WorldDefeats.DownedCrabson = true;
            if (Main.expertMode)
            {
                npc.DropBossBags();
                return;
            }
            if (Main.rand.NextBool(6))
                Item.NewItem(rect, ModContent.ItemType<CrabClock>());
            Item.NewItem(rect, ModContent.ItemType<AquaticEnergy>(), Main.rand.NextVRand(3, 5));
            switch (Main.rand.Next(3))
            {
                default:
                {
                    Item.NewItem(rect, ModContent.ItemType<Bubbler>());
                }
                break;

                case 1:
                {
                    Item.NewItem(rect, ModContent.ItemType<CinnabarBow>());
                }
                break;

                case 2:
                {
                    Item.NewItem(rect, ModContent.ItemType<JerryClawFlail>());
                }
                break;
            }
            npc.position.Y += npc.height / 2f;
        }

        public ModifiableMusic GetMusic() => AQMod.CrabsonMusic;
    }
}