using Aequus.Biomes;
using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Content.AnalysisQuests;
using Aequus.Content.Personalities;
using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Boss.Summons;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Misc.Drones;
using Aequus.Items.Placeable;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools;
using Aequus.Projectiles.Misc;
using Microsoft.Xna.Framework;
using ShopQuotesMod;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.NPCs.Friendly.Town
{
    [AutoloadHead()]
    public class Physicist : ModNPC, IModifyShoppingSettings
    {
        public static int awaitQuest;

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
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Like)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Hate)
                .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Love)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Mechanic, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Wizard, AffectionLevel.Hate)
                .SetNPCAffection<Occultist>(AffectionLevel.Hate);

            NPCHappiness.Get(NPCID.GoblinTinkerer).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.Cyborg).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.Steampunker).SetNPCAffection(Type, AffectionLevel.Dislike);
            NPCHappiness.Get(NPCID.Mechanic).SetNPCAffection(Type, AffectionLevel.Dislike);

            ModContent.GetInstance<QuoteDatabase>().AddNPC(Type, Mod, "Mods.Aequus.ShopQuote.")
                .UseColor(Color.SkyBlue * 1.2f);
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
            int dustAmount = (int)Math.Clamp(damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MartianHit);
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
            if (GameplayConfig.Instance.EarlyPortalGun)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.PortalGun);
            }
            if (GameplayConfig.Instance.EarlyGravityGlobe)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.GravityGlobe);
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PrecisionGloves>());
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
            if (NPC.AnyNPCs(NPCID.Steampunker))
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<InactivePylonCleanser>());
            }

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ForceAntiGravityBlock>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ForceGravityBlock>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PhysicsBlock>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<EmancipationGrill>());

            if (AequusWorld.downedOmegaStarite)
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SupernovaFruit>());

            if (NPC.AnyNPCs(NPCID.Painter))
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ExLydSpacePainting>());
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<HomeworldPainting>());
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return AequusWorld.downedUltraStarite || AequusWorld.downedOmegaStarite;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Lina",
                "Lumia",
                "Astra",
                "Stoffien",
                "Eridani",
                "Asphodene",
                "Termina",
                "Kristal",
                "Arti",
                "Ficeher",
                "Gina",
            };
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return base.TownNPCProfile();
        }

        public override string GetChat()
        {
            if (Main.invasionType == InvasionID.MartianMadness)
            {
                return AequusText.GetText("Chat.Physicist.MartianMadness");
            }

            var player = Main.LocalPlayer;
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.Physicist.");

            if (GlimmerBiome.EventActive && Main.rand.NextBool())
            {
                chat.Add("Glimmer.0");
                chat.Add("Glimmer.1");
            }
            else if (Main.bloodMoon && Main.rand.NextBool())
            {
                chat.Add("BloodMoon.0");
                chat.Add("BloodMoon.1");
            }
            else
            {
                chat.Add("Basic.0");
                chat.Add("Basic.1");
                chat.Add("Basic.2");
                chat.Add("Basic.3");
                if (Main.dayTime)
                {
                    chat.Add("Night.0");
                }
            }
            if (Main.IsItAHappyWindyDay)
            {
                chat.Add("WindyDay");
            }
            if (Main.raining)
            {
                chat.Add("Rain");
            }
            if (Main.IsItStorming)
            {
                chat.Add("Thunderstorm");
            }
            if (BirthdayParty.PartyIsUp)
            {
                chat.Add("Party");
            }
            if (player.ZoneGraveyard)
            {
                chat.Add("Graveyard.0");
                chat.Add("Graveyard.1");
            }
            if (ModLoader.TryGetMod("SoTS", out var soTS))
            {
                chat.Add("SoTSMod.0");
                chat.Add("SoTSMod.1");
            }
            if (NPC.downedGolemBoss && NPC.AnyNPCs(NPCID.Mechanic) && ModLoader.TryGetMod("Polarities", out var polarities))
            {
                chat.Add("PolaritiesMod");
            }
            if (NPC.downedMartians)
            {
                chat.Add("MartianMadness2");
            }
            if (NPC.TowerActiveNebula || NPC.TowerActiveSolar || NPC.TowerActiveStardust || NPC.TowerActiveVortex || NPC.MoonLordCountdown > 0 || NPC.AnyNPCs(NPCID.MoonLordCore))
            {
                chat.Add("LunarPillars");
            }

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = Main.npcChatCornerItem > 0 ? AequusText.GetText("Chat.Physicist.AnalysisButtonComplete") : AequusText.GetText("Chat.Physicist.AnalysisButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
                return;
            }

            awaitQuest = 30;
            QuestButtonPressed();
        }
        public static void QuestButtonPressed()
        {
            var player = Main.LocalPlayer;
            var questPlayer = player.GetModPlayer<AnalysisPlayer>();
            //Main.NewText(questPlayer.completed);
            if (!questPlayer.quest.isValid && questPlayer.timeForNextQuest == 0 && questPlayer.questResetTime <= 0)
            {
                questPlayer.quest = default(QuestInfo);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    var p = Aequus.GetPacket(PacketType.RequestAnalysisQuest);
                    p.Write(Main.myPlayer);
                    p.Write(questPlayer.completed);
                    p.Send();
                }
                else
                {
                    questPlayer.RefreshQuest(questPlayer.completed);
                }
            }
            if (!questPlayer.quest.isValid || questPlayer.timeForNextQuest > 0)
            {
                Main.npcChatText = AequusText.GetTextWith("Chat.Physicist.AnalysisRarityQuestNoQuest", new { Time = AequusText.WatchTime(questPlayer.timeForNextQuest, questPlayer.dayTimeForNextQuest), });
                return;
            }

            var validItem = FindPotentialQuestItem(player, questPlayer.quest);
            if (Main.npcChatCornerItem > 0 && validItem != null)
            {
                var popupItem = validItem.Clone();
                popupItem.position = player.position;
                popupItem.width = player.width;
                popupItem.height = player.height;
                popupItem.stack = 1;
                var itemText = PopupText.NewText(PopupTextContext.RegularItemPickup, popupItem, 1);
                Main.popupText[itemText].name = $"{Main.popupText[itemText].name} (-1)";
                Main.popupText[itemText].position.X = Main.LocalPlayer.Center.X - FontAssets.ItemStack.Value.MeasureString(Main.popupText[itemText].name).X / 2f;

                validItem.stack--;
                if (validItem.stack <= 0)
                {
                    validItem.TurnToAir();
                }
                questPlayer.completed++;
                var items = questPlayer.GetAnalysisRewardDrops();
                var source = player.talkNPC != -1 ? Main.npc[player.talkNPC].GetSource_GiftOrReward() : player.GetSource_GiftOrReward();
                foreach (var i in items)
                {
                    player.QuickSpawnClonedItem(source, i, i.stack);
                }
                int time = Main.rand.Next(28800, 43200);
                AequusHelpers.AddToTime(Main.time, time, Main.dayTime, out double result, out bool dayTime);
                questPlayer.timeForNextQuest = (int)Math.Min(result, dayTime ? Main.dayLength - 60 : Main.nightLength - 60);
                questPlayer.dayTimeForNextQuest = dayTime;
                SoundEngine.PlaySound(SoundID.Grab);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = AequusText.GetText("Chat.Physicist.AnalysisRarityQuestComplete");
                return;
            }
            Main.npcChatText = QuestChat(questPlayer.quest);

            if (validItem != null)
            {
                Main.npcChatCornerItem = validItem.type;
                Main.npcChatText += $"\n{AequusText.GetTextWith("Chat.Physicist.AnalysisRarityQuest2", new { Item = validItem.Name, })}";
            }
        }
        public static Item FindPotentialQuestItem(Player player, QuestInfo questInfo)
        {
            for (int i = 0; i < Main.InventorySlotsTotal; i++)
            {
                if (CanBeQuestItem(player.inventory[i], questInfo))
                {
                    return player.inventory[i];
                }
            }
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (CanBeQuestItem(player.bank4.item[i], questInfo))
                {
                    return player.bank4.item[i];
                }
            }
            return null;
        }
        public static bool CanBeQuestItem(Item item, QuestInfo questInfo)
        {
            return !item.favorited && !item.IsAir && !item.IsACoin && 
                item.OriginalRarity == questInfo.itemRarity && !AnalysisSystem.IgnoreItem.Contains(item.type) && !Main.itemAnimationsRegistered.Contains(item.type);
        }
        public static string QuestChat(QuestInfo questInfo)
        {
            return AequusText.GetTextWith("Chat.Physicist.AnalysisRarityQuest", new { Rarity = AequusText.ColorCommand(AequusText.GetRarityNameValue(questInfo.itemRarity), AequusHelpers.GetRarityColor(questInfo.itemRarity)), });
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
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

        public void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper)
        {
            AequusHelpers.ReplaceTextWithStringArgs(ref settings.HappinessReport, "[HateBiomeQuote]|",
                $"Mods.Aequus.TownNPCMood.Physicist.HateBiome_{(player.ZoneHallow ? "Hallow" : "Evils")}", (s) => new { BiomeName = s[1], });
        }
    }
}