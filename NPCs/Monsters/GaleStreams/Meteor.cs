using AQMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs.Monsters.GaleStreams
{
    public class Meteor : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 30;
            npc.lifeMax = 250;
            npc.damage = 2;
            npc.defense = 45;
            npc.HitSound = SoundID.NPCHit7;
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0.3f;
            npc.value = Item.buyPrice(silver: 2);
            npc.npcSlots = 0.25f;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
        }

        public override void AI()
        {
            if ((int)npc.ai[0] == 0)
            {
                npc.ai[0] = 1f;
                npc.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                npc.localAI[0] = Main.rand.Next(Main.npcFrameCount[npc.type]) + 1f;
            }
            if (!Content.World.Events.GaleStreams.GaleStreams.InSpace(npc.position.Y))
            {
                npc.noGravity = false;
                if (npc.collideX || npc.collideY)
                {
                    npc.TargetClosest(faceTarget: false);
                    npc.defense = 0;
                    npc.ai[0] = 2f;
                    if (npc.HasValidTarget)
                        Main.player[npc.target].ApplyDamageToNPC(npc, npc.lifeMax, npc.velocity.Length(), 0, false);
                    else
                    {
                        npc.life = -1;
                        npc.HitEffect();
                        npc.active = false;
                    }
                    npc.life = -1;
                    var p = npc.Center.ToTileCoordinates();
                    Main.PlaySound(SoundID.NPCDeath14, npc.Center);
                    for (int i = 0; i < 80; i++)
                    {
                        int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 2f));
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = (Main.dust[d].position - npc.Center) / 8f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.oldVelocity.Length() > 7.5f &&
                        Content.World.Events.GaleStreams.GaleStreams.CanCrashMeteor(p.X, p.Y, 24))
                    {
                        Content.World.Events.GaleStreams.GaleStreams.CrashMeteor(p.X, p.Y, 24, scatter: 1, scatterAmount: 4, scatterChance: 10, holeSizeDivider: 3, doEffects: true, tileType: TileID.Meteorite);
                    }
                }
            }
            else if (!Content.World.Events.GaleStreams.GaleStreams.InMeteorSpawnZone(npc.position.Y))
            {
                npc.velocity.Y -= 0.01f;
            }
            else if (npc.position.Y < 200)
            {
                npc.velocity.Y += 0.05f;
            }
            npc.rotation += npc.velocity.Length() * 0.00157f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, 23, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.5f, 1f));
                    Main.dust[d].noGravity = Content.World.Events.GaleStreams.GaleStreams.InSpace(npc.position.Y);
                    Main.dust[d].velocity = (Main.dust[d].position - npc.Center) / 8f;
                }
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 1.75f));
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = (Main.dust[d].position - npc.Center) / 8f;
                }
                if (Main.netMode != NetmodeID.Server)
                    AQSound.LegacyPlay(SoundType.NPCKilled, AQSound.Paths.MeteorKilled, npc.Center, 0.6f);
            }
            else
            {
                int d = Dust.NewDust(npc.position, npc.width, npc.height, 23, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.5f, 1f));
                Main.dust[d].noGravity = Content.World.Events.GaleStreams.GaleStreams.InSpace(npc.position.Y);
                Main.dust[d].velocity = (Main.dust[d].position - npc.Center) / 8f;
            }
        }

        public override void NPCLoot()
        {
            Content.World.Events.GaleStreams.GaleStreams.ProgressEvent(Main.player[Player.FindClosest(npc.position, npc.width, npc.height)], 1);
            if ((int)npc.ai[0] == 2)
                return;
            if (Main.rand.NextBool())
                barOreDrop(ItemID.CopperOre, ItemID.TinOre, ItemID.CopperBar, ItemID.TinBar, WorldGen.CopperTierOre, TileID.Tin, 3);
            else
                barOreDrop(ItemID.SilverOre, ItemID.TungstenOre, ItemID.SilverBar, ItemID.TungstenBar, WorldGen.SilverTierOre, TileID.Tungsten, 4);
            barOreDrop(ItemID.IronOre, ItemID.LeadOre, ItemID.IronBar, ItemID.LeadBar, WorldGen.IronTierOre, TileID.Lead, 20);
            if (Main.rand.NextBool(4))
                barOreDrop(ItemID.GoldOre, ItemID.PlatinumOre, ItemID.GoldBar, ItemID.PlatinumBar, WorldGen.GoldTierOre, TileID.Platinum, 4);
            if ((NPC.downedBoss2 || Main.hardMode) && Main.rand.NextBool(32))
                Item.NewItem(npc.getRect(), ModContent.ItemType<Items.Tools.TheFan>());

        }

        private void barOreDrop(int ore1, int ore2, int bar1, int bar2, int worldOreTier, int altOreTier, int oreDropMax)
        {
            if (worldOreTier == altOreTier)
            {
                if (Main.rand.NextBool())
                    Item.NewItem(npc.getRect(), ore2, Main.rand.NextVRand(1, oreDropMax));
                else
                {
                    Item.NewItem(npc.getRect(), bar2, Math.Max(Main.rand.NextVRand(1, oreDropMax) / 3, 1));
                }
            }
            else
            {
                if (Main.rand.NextBool())
                    Item.NewItem(npc.getRect(), ore1, Main.rand.NextVRand(1, oreDropMax));
                else
                {
                    Item.NewItem(npc.getRect(), bar1, Math.Max(Main.rand.NextVRand(1, oreDropMax) / 3, 1));
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * (int)(npc.localAI[0] - 1f);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
        }
    }
}