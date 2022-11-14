using Aequus.Common.Utilities;
using Aequus.Content;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Summon.Necro;
using Aequus.Items.Boss.Summons;
using Aequus.Items.Consumables.SoulGems;
using Aequus.Items.Misc;
using Aequus.Items.Placeable;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools.GrapplingHooks;
using Aequus.Items.Tools.Misc;
using Aequus.Items.Weapons.Summon.Necro.Candles;
using Aequus.Items.Weapons.Summon.Necro.Tapers;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShopQuotesMod;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Town
{
    [AutoloadHead()]
    public class Occultist : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 2;
            NPCID.Sets.AttackTime[NPC.type] = 10;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1,
            });

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Lovestruck,
                }
            });

            NPC.Happiness
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Like)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Clothier, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Love)
                .SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Hate);

            NPCHappiness.Get(NPCID.TaxCollector).SetNPCAffection(Type, AffectionLevel.Love);
            NPCHappiness.Get(NPCID.ArmsDealer).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.Dryad).SetNPCAffection(Type, AffectionLevel.Dislike);
            NPCHappiness.Get(NPCID.Demolitionist).SetNPCAffection(Type, AffectionLevel.Hate);
            NPCHappiness.Get(NPCID.BestiaryGirl).SetNPCAffection(Type, AffectionLevel.Hate);

            ModContent.GetInstance<QuoteDatabase>().AddNPC(Type, Mod, "Mods.Aequus.ShopQuote.")
                .UseColor(Color.DarkRed * 1.5f);
            ExporterQuests.NPCTypesNoSpawns.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.lavaImmune = true;
            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.Underground);
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GhostlyGrave>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OccultistCandle>());
            if (NPC.downedBoss3)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SkeletronTaperCandle>());
            }
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CrownOfBlood>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CrownOfDarkness>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CrownOfTheGrounded>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Meathook>());
            shop.item[nextSlot++].SetDefaults(ItemID.ShadowKey);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<UnholyCore>());
            if (!Main.dayTime)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritBottle>());
            }
            if (Main.hardMode)
            {
                if (!Main.dayTime)
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BlackPhial>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GoreNest>());
            }
            if (NPC.AnyNPCs(NPCID.Painter))
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GoreNestPainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<InsurgentPainting>());
            }
            if (!Main.dayTime)
            {
                if (Main.bloodMoon)
                {
                    shop.item[nextSlot].SetDefaults<BloodyTearFragment>();
                    shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 1);
                    shop.item[nextSlot++].SetDefaults(ItemID.WhoopieCushion);
                }
                else
                {
                    if (Main.hardMode)
                    {
                        if (NPC.downedAncientCultist)
                        {
                            shop.item[nextSlot].SetDefaults<GrandSoulGem>();
                            shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 4);
                        }
                        if (NPC.downedPlantBoss)
                        {
                            shop.item[nextSlot].SetDefaults<GreaterSoulGem>();
                            shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 3);
                        }
                        shop.item[nextSlot].SetDefaults<LesserSoulGem>();
                        shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 2);
                    }
                    shop.item[nextSlot].SetDefaults<PettySoulGem>();
                    shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 1, silver: 25);
                }
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return AequusWorld.downedEventDemon;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Abaddon",
                "Cally",
                "Brimmy",
                "Beelzebub",
                "Lucy",
                "Sin",
                "Revengeance",
                "Archvince",
                "Vincera",
                "Baron",
                "Spectre",
                "Heretic",
                "Maykr",
            };
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return base.TownNPCProfile();
        }

        public override string GetChat()
        {
            var player = Main.LocalPlayer;
            var chat = new SelectableChat("Mods.Aequus.Chat.Occultist.");

            chat.Add("Basic.0");
            chat.Add("Basic.1");

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return !toKingStatue;
        }

        public override void AI()
        {
            if ((int)NPC.ai[0] == 14)
            {
                NPC.ai[1] += 0.9f;
                if (Main.GameUpdateCount % 7 == 0)
                {
                    var d = Dust.NewDustDirect(NPC.position + new Vector2(0f, NPC.height - 4), NPC.width, 4, DustID.PurpleCrystalShard, 0f, -4f);
                    d.velocity *= 0.5f;
                    d.velocity.X *= 0.5f;
                    d.noGravity = true;
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            int dustAmount = (int)Math.Clamp(damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SpaceSquidBlood>(), NPC.velocity.X, NPC.velocity.Y);
            }
            if (NPC.life <= 0)
            {
                var g = GoreHelper.DeathGore(NPC, "Occultist_1");
                g.rotation += MathHelper.Pi;
                GoreHelper.DeathGore(NPC, "Occultist_1");
                GoreHelper.DeathGore(NPC, "Occultist_0", new Vector2(0f, -NPC.height / 2f + 8f));

                if (Main.rand.NextBool(4))
                {
                    GoreHelper.DeathGore(NPC, "Occultist_2", default, new Vector2(0f, -2f));
                }
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 60;
            randExtraCooldown = 2;
        }

        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 0f;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            NPCID.Sets.AttackType[NPC.type] = 2;
            projType = ModContent.ProjectileType<OccultistProjSpawner>();
            attackDelay = 12;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 6f;
            randomOffset = 1.5f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.frame.Y >= NPC.frame.Height * 21)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y = NPC.frame.Height * 20;
            }
            NPC.GetDrawInfo(out var t, out var off, out var frame, out var orig, out int _);
            off.Y += NPC.gfxOffY - 4f;
            spriteBatch.Draw(t, NPC.position + off - screenPos, frame, drawColor, NPC.rotation, orig, NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            if ((int)NPC.ai[0] == 14)
            {
                var bloomFrame = TextureCache.Bloom[0].Value.Frame(verticalFrames: 2);
                spriteBatch.Draw(TextureCache.Bloom[0].Value, NPC.position + off - screenPos + new Vector2(2f * -NPC.spriteDirection, NPC.height / 2f + 6f).RotatedBy(NPC.rotation),
                    bloomFrame, Color.BlueViolet * 0.5f, NPC.rotation, TextureCache.Bloom[0].Value.Size() / 2f, NPC.scale * 0.5f, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
                var auraFrame = TextureAssets.Extra[51].Value.Frame(verticalFrames: 4, frameY: (int)(Main.GlobalTimeWrappedHourly * 9f) % 4);
                spriteBatch.Draw(TextureAssets.Extra[51].Value, NPC.position + off - screenPos + new Vector2(4f * -NPC.spriteDirection, NPC.height / 2f + 8f).RotatedBy(NPC.rotation),
                    auraFrame, Color.BlueViolet * 0.7f, NPC.rotation, new Vector2(auraFrame.Width / 2f, auraFrame.Height), NPC.scale, (-NPC.spriteDirection).ToSpriteEffect(), 0f);
            }
            return false;
        }
    }
}