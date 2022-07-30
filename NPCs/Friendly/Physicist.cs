using Aequus.Common.Utilities;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Consumables.Drones;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Summons;
using Aequus.Items.Placeable;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly
{
    [AutoloadHead()]
    public class Physicist : ModNPC
    {
        public int spawnPet;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 25;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 400;
            NPCID.Sets.AttackType[NPC.type] = 0; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
            NPCID.Sets.AttackTime[NPC.type] = 10;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,
                Direction = -1,
            });

            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Love)
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Mechanic, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Clothier, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.DD2Bartender, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.SantaClaus, AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Wizard, AffectionLevel.Hate)
                .SetNPCAffection<Occultist>(AffectionLevel.Hate);

            ShopQuotes.Database
                .GetNPC(Type)
                .WithColor(Color.SkyBlue * 1.2f)
                .AddQuote<PhysicsGun>()
                .AddQuote<ForceAntiGravityBlock>()
                .AddQuote<ForceGravityBlock>()
                .AddQuote<PhysicsBlock>()
                .AddQuote<EmancipationGrill>()
                .AddQuote<HaltingMachine>()
                .AddQuote<HolographicMeatloaf>()
                .AddQuote(ItemID.BloodMoonStarter)
                .AddQuote<GalacticStarfruit>()
                .AddQuote(ItemID.SolarTablet)
                .AddQuote<InactivePylonGunner>()
                .AddQuote<InactivePylonHealer>()
                .AddQuote<InactivePylonCleanser>()
                .AddQuote<HomeworldPainting>()
                .AddQuote<SupernovaFruit>();
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
                .AddMainSpawn(BestiaryBuilder.DesertBiome);
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PhysicsGun>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ForceAntiGravityBlock>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ForceGravityBlock>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PhysicsBlock>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EmancipationGrill>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HaltingMachine>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HolographicMeatloaf>());
            //shop.item[nextSlot].SetDefaults(ModContent.ItemType<Cosmicanon>());
            //nextSlot++;
            //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Transistor>());
            //if (Main.hardMode && NPC.downedMechBossAny)
            //{
            //    shop.item[nextSlot].SetDefaults(ModContent.ItemType<EclipseGlasses>());
            //    nextSlot++;
            //}

            //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Stardrop>());

            shop.item[nextSlot].SetDefaults(ItemID.BloodMoonStarter);
            shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 2);
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<GalacticStarfruit>());

            if (Main.hardMode && NPC.downedPlantBoss)
            {
                shop.item[nextSlot].SetDefaults(ItemID.SolarTablet);
                shop.item[nextSlot++].shopCustomPrice = Item.buyPrice(gold: 5);
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<InactivePylonGunner>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<InactivePylonHealer>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<InactivePylonCleanser>());

            if (!Main.dayTime)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HomeworldPainting>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SupernovaFruit>());
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return AequusWorld.downedOmegaStarite;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Deahdeah",
                "Lina",
                "Peach",
                "Lumia",
                "Astra",
                "Stoffien",
                "Constructa",
                "Eridani",
                "Asphodene",
                "Termina",
                "Kristal",
                "Arti",
                "Ficeher",
                "Loada",
                "Gina",
            };
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return base.TownNPCProfile();
        }

        public override string GetChat()
        {
            var player = Main.LocalPlayer;
            var chat = new SelectableChat("Mods.Aequus.Chat.Physicist.");

            chat.Add("Basic.0");
            chat.Add("Basic.1");
            chat.Add("Basic.2");
            chat.Add("Basic.3");
            chat.Add("Basic.4");

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
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
            projType = ModContent.ProjectileType<PhysicistProj>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override void AI()
        {
            if (spawnPet < 60)
            {
                spawnPet++;
            }
            else
            {
                spawnPet = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<PhysicistPet>() && Main.npc[i].ai[0] == NPC.whoAmI)
                    {
                        return;
                    }
                }
                NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.Center, ModContent.NPCType<PhysicistPet>(), NPC.whoAmI, NPC.whoAmI);
            }
        }

        public override void OnGoToStatue(bool toKingStatue)
        {
            int pet = NPC.FindFirstNPC(ModContent.NPCType<PhysicistPet>());
            if (pet == -1)
            {
                Main.npc[pet].Center = NPC.Center;
                Main.npc[pet].netUpdate = true;
            }
        }
    }
}