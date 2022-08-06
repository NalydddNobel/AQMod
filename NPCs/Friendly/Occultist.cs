using Aequus.Biomes;
using Aequus.Common.Utilities;
using Aequus.Content;
using Aequus.Items.Accessories;
using Aequus.Items.Placeable;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools;
using Aequus.Items.Tools.GrapplingHooks;
using Aequus.Items.Weapons.Summon.Candles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly
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
            NPCID.Sets.AttackType[NPC.type] = 3; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
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

            ShopQuotes.Database
                .GetNPC(Type)
                .WithColor(Color.DarkRed * 1.5f)
                .AddQuote<GhostlyGrave>()
                .AddQuote<OccultistCandle>()
                .AddQuote<Meathook>()
                .AddQuote(ItemID.ShadowKey)
                .AddQuote<UnholyCore>()
                .AddQuote<GoreNest>()
                .AddQuote<InsurgentPainting>()
                .AddQuote<BlackPhial>()
                .AddQuote(ItemID.WhoopieCushion);

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
            AnimationType = NPCID.Guide;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = NPC.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
            }
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
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Meathook>());
            shop.item[nextSlot++].SetDefaults(ItemID.ShadowKey);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<UnholyCore>());
            if (Main.hardMode)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GoreNest>());
            }
            if (!Main.dayTime)
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<InsurgentPainting>());
            }
            if (Main.hardMode)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BlackPhial>());
            if (!Main.dayTime && Main.bloodMoon)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.WhoopieCushion);
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

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 12;
            randExtraCooldown = 20;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.Hellwing;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 6f;
            randomOffset = 1.5f;
        }
    }
}