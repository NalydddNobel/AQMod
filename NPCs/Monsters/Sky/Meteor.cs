using Aequus.Biomes;
using Aequus.Common;
using Aequus.Common.ItemDrops;
using Aequus.Items.Tools;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Sky
{
    public class Meteor : ModNPC
    {
        public static SoundStyle CRUNCHSonicBreakingSound { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                CRUNCHSonicBreakingSound = Aequus.GetSound("sonicmeteor", 0.6f);
            }
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
            PushablesDatabase.NPCs.Add(Type);
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, 8f),
            });
            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                ImmuneToAllBuffsThatAreNotWhips = true,
                ImmuneToWhips = true,
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 250;
            NPC.damage = 2;
            NPC.defense = 45;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.knockBackResist = 0.3f;
            NPC.value = Item.buyPrice(silver: 2);
            NPC.npcSlots = 0.25f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.SkyBiome)
                .QuickUnlock();
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == 0)
            {
                NPC.ai[0] = 1f;
                NPC.velocity = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                NPC.localAI[0] = Main.rand.Next(Main.npcFrameCount[NPC.type]) + 1f;
            }
            if (!GaleStreamsInvasion.IsThisSpace(NPC.position.Y))
            {
                NPC.noGravity = false;
                if (NPC.collideX || NPC.collideY)
                {
                    NPC.TargetClosest(faceTarget: false);
                    NPC.defense = 0;
                    NPC.ai[0] = 2f;
                    if (NPC.HasValidTarget)
                    {
                        Main.player[NPC.target].ApplyDamageToNPC(NPC, NPC.lifeMax, NPC.velocity.Length(), 0, false);
                    }
                    else
                    {
                        NPC.life = -1;
                        NPC.HitEffect();
                        NPC.active = false;
                    }
                    NPC.life = -1;
                    var p = NPC.Center.ToTileCoordinates();
                    SoundEngine.PlaySound(SoundID.NPCDeath14);
                    for (int i = 0; i < 80; i++)
                    {
                        int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 2f));
                        Main.dust[d].noGravity = true;
                        Main.dust[d].velocity = (Main.dust[d].position - NPC.Center) / 8f;
                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.oldVelocity.Length() > 7.5f)
                    {
                        GaleStreamsInvasion.CrashMeteor(p.X, p.Y, 24, scatter: 1, scatterAmount: 4, scatterChance: 10, holeSizeDivider: 3, doEffects: true, tileType: TileID.Meteorite);
                    }
                }
            }
            else if (!GaleStreamsInvasion.IsThisSpace(NPC.position.Y + 600f))
            {
                NPC.velocity.Y -= 0.01f;
            }
            else if (NPC.position.Y < 200)
            {
                NPC.velocity.Y += 0.05f;
            }
            NPC.rotation += NPC.velocity.Length() * 0.00157f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_Meteor, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.5f, 1f));
                    Main.dust[d].noGravity = GaleStreamsInvasion.IsThisSpace(NPC.position.Y);
                    Main.dust[d].velocity = (Main.dust[d].position - NPC.Center) / 8f;
                }
                for (int i = 0; i < 10; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.8f, 1.75f));
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity = (Main.dust[d].position - NPC.Center) / 8f;
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(CRUNCHSonicBreakingSound, NPC.Center);
                }
            }
            else
            {
                int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_Meteor, 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.5f, 1f));
                Main.dust[d].noGravity = GaleStreamsInvasion.IsThisSpace(NPC.position.Y);
                Main.dust[d].velocity = (Main.dust[d].position - NPC.Center) / 8f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            int frame = (int)(NPC.localAI[0] - 1f);
            if (NPC.IsABestiaryIconDummy)
            {
                frame = AequusHelpers.TimedBasedOn((int)Main.GameUpdateCount, 30, Main.npcFrameCount[NPC.type]);
            }
            NPC.frame.Y = frameHeight * frame;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<Pumpinator>(chance: 15, stack: 1)
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, false), new ItemDrop(ItemID.CopperOre, 3), new ItemDrop(ItemID.CopperBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, true), new ItemDrop(ItemID.TinOre, 3), new ItemDrop(ItemID.TinBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, false), new ItemDrop(ItemID.IronOre, 3), new ItemDrop(ItemID.IronBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, true), new ItemDrop(ItemID.LeadOre, 3), new ItemDrop(ItemID.LeadBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, false), new ItemDrop(ItemID.SilverOre, 4), new ItemDrop(ItemID.SilverBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, true), new ItemDrop(ItemID.TungstenOre, 4), new ItemDrop(ItemID.TungstenBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, false), new ItemDrop(ItemID.GoldOre, 4), new ItemDrop(ItemID.GoldBar, 1)))
                .Add(new OneFromOptionsWithStackWithCondition(new OreTierCondition(0, true), new ItemDrop(ItemID.PlatinumOre, 4), new ItemDrop(ItemID.PlatinumBar, 1)));
        }
    }
}