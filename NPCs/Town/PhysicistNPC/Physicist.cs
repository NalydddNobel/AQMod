using Aequus.Common;
using Aequus.Common.NPCs;
using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Content.DronePylons.Items;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Items.Equipment.Accessories.Combat.Ranged;
using Aequus.Items.Equipment.Accessories.Misc.ItemReach;
using Aequus.Items.Equipment.Accessories.Sentry.SentryChip;
using Aequus.Items.Misc.Foods;
using Aequus.Items.Misc.Spawners;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Sentry.PhysicistSentry;
using Aequus.NPCs.Town.OccultistNPC;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Aequus.Tiles.Blocks;
using Aequus.Tiles.Blocks.GravityBlocks;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.Localization;
using static Terraria.GameContent.Profiles;

namespace Aequus.NPCs.Town.PhysicistNPC;
[AutoloadHead()]
public class Physicist : AequusTownNPC<Physicist> {
    public struct PetInfo {
        public int PetNPCIndex;
        public int SpawnPetCheck;

        public bool IsMyPet(NPC owner, NPC pet) {
            return pet.active && pet.type == ModContent.NPCType<PhysicistPet>() && pet.ai[0] == owner.whoAmI;
        }

        public void Update(NPC npc) {
            if (SpawnPetCheck > 0) {
                SpawnPetCheck--;
            }
            else if (IsMyPet(npc, Main.npc[PetNPCIndex])) {
                SpawnPetCheck = 1200;
            }
            else {
                SpawnPetCheck = 120;
                for (int i = 0; i < Main.maxNPCs; i++) {
                    if (IsMyPet(npc, Main.npc[i])) {
                        PetNPCIndex = i;
                        return;
                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient) {
                    NPC.NewNPCDirect(npc.GetSource_FromThis(), npc.Center, ModContent.NPCType<PhysicistPet>(), npc.whoAmI, npc.whoAmI);
                }
            }
        }
    }

    public static int awaitQuest;
    public PetInfo pet;

    public override List<string> SetNPCNameList() {
        return new() {
            "Lina",
            "Lumia",
            "Astra",
            "Eridani",
            "Termina",
            "Kristal",
            "Arti",
            "Gina",
        };
    }

    internal void SetupShopQuotes(Mod shopQuotes) {
        shopQuotes.Call("AddNPC", Mod, Type);
        shopQuotes.Call("SetColor", Type, Color.SkyBlue * 1.2f);
    }

    public override void Load() {
        ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, AequusTextures.NPCs_Town_PhysicistNPC_Shimmer_Physicist_Head.FullPath);
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        NPCID.Sets.AttackType[NPC.type] = 0; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
        NPCID.Sets.AttackTime[NPC.type] = 10;
        NPCID.Sets.AttackAverageChance[NPC.type] = 10;
        NPCID.Sets.HatOffsetY[NPC.type] = 2;

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

        Profile = new StackedNPCProfile(
            new DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
            new DefaultNPCProfile(AequusTextures.NPCs_Town_PhysicistNPC_Shimmer_Physicist.FullPath, ShimmerHeadIndex)
        );
    }

    public override void SetDefaults() {
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

    public override void HitEffect(NPC.HitInfo hit) {
        int dustAmount = Math.Clamp(hit.Damage / 3, NPC.life > 0 ? 1 : 40, 40);
        for (int k = 0; k < dustAmount; k++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.MartianHit);
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.DesertBiome);
    }

    public override void AddShops() {
        new NPCShop(Type)
            .Add<PhysicsGun>()
            .Add(ItemID.PortalGun, GameplayConfig.ConditionEarlyPortalGun)
            .Add(ItemID.GravityGlobe, GameplayConfig.ConditionEarlyGravityGlobe)
            .Add<LaserReticle>()
            .Add<HaltingMachine>()
            .Add<HolographicMeatloaf>(Condition.NotDontStarveWorld)
            .AddWithCustomValue(ItemID.BloodMoonStarter, Item.buyPrice(gold: 1))
            .Add<GalacticStarfruit>()
            .AddWithCustomValue(ItemID.SolarTablet, Item.buyPrice(gold: 2, silver: 50), Condition.DownedPlantera)
            .Add<PylonGunnerItem>()
            .Add<PylonHealerItem>()
            .Add<PylonCleanserItem>(Condition.NpcIsPresent(NPCID.Steampunker), Condition.NotRemixWorld)
            .Add<PhysicistSentry>(Condition.NotRemixWorld)
            .Add<Sentry6502>(Condition.RemixWorld)
            .Add<AntiGravityBlock>(Condition.NotZenithWorld)
            .Add<GravityBlock>(Condition.NotZenithWorld)
            .Add<AntiGravityBlock>(Condition.ZenithWorld, Condition.DownedEowOrBoc)
            .Add<GravityBlock>(Condition.ZenithWorld, Condition.DownedEowOrBoc)
            .Add<PhysicsBlock>()
            .Add<EmancipationGrill>()
            .Add<SupernovaFruit>(AequusConditions.DownedOmegaStarite)
            .Register();

        //shop.item[nextSlot].SetDefaults(ModContent.ItemType<Cosmicanon>());
        //nextSlot++;
        //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Transistor>());
        //if (Main.hardMode && NPC.downedMechBossAny)
        //{
        //    shop.item[nextSlot].SetDefaults(ModContent.ItemType<EclipseGlasses>());
        //    nextSlot++;
        //}

        //shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Stardrop>());
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) {
        return AequusWorld.downedUltraStarite || AequusWorld.downedOmegaStarite;
    }

    public override ITownNPCProfile TownNPCProfile() {
        return base.TownNPCProfile();
    }

    public override string GetChat() {
        if (Main.invasionType == InvasionID.MartianMadness) {
            return TextHelper.GetTextValue("Chat.Physicist.MartianMadness");
        }

        var player = Main.LocalPlayer;
        var chat = new SelectableChatHelper("Mods.Aequus.Chat.Physicist.");

        if (GlimmerZone.EventActive && Main.rand.NextBool()) {
            chat.Add("Glimmer.0");
            chat.Add("Glimmer.1");
        }
        else if (Main.bloodMoon && Main.rand.NextBool()) {
            chat.Add("BloodMoon.0");
            chat.Add("BloodMoon.1");
        }
        else {
            chat.Add("Basic.0");
            chat.Add("Basic.1");
            chat.Add("Basic.2");
            chat.Add("Basic.3");
            if (Main.dayTime) {
                chat.Add("Night.0");
            }
        }
        if (Main.IsItAHappyWindyDay) {
            chat.Add("WindyDay");
        }
        if (Main.raining) {
            chat.Add("Rain");
        }
        if (Main.IsItStorming) {
            chat.Add("Thunderstorm");
        }
        if (BirthdayParty.PartyIsUp) {
            chat.Add("Party");
        }
        if (player.ZoneGraveyard) {
            chat.Add("Graveyard.0");
            chat.Add("Graveyard.1");
        }
        if (ModLoader.TryGetMod("SoTS", out var soTS)) {
            chat.Add("SoTSMod.0");
            chat.Add("SoTSMod.1");
        }
        if (NPC.downedGolemBoss && NPC.AnyNPCs(NPCID.Mechanic) && ModLoader.TryGetMod("Polarities", out var polarities)) {
            chat.Add("PolaritiesMod");
        }
        if (NPC.downedMartians) {
            chat.Add("MartianMadness2");
        }
        if (NPC.TowerActiveNebula || NPC.TowerActiveSolar || NPC.TowerActiveStardust || NPC.TowerActiveVortex || NPC.MoonLordCountdown > 0 || NPC.AnyNPCs(NPCID.MoonLordCore)) {
            chat.Add("LunarPillars");
        }

        return chat.Get();
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
        button2 = Main.npcChatCornerItem > 0 ? TextHelper.GetTextValue("Chat.Physicist.AnalysisButtonComplete") : TextHelper.GetTextValue("Chat.Physicist.AnalysisButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
            return;
        }

        awaitQuest = 30;
        QuestButtonPressed();
    }
    public static void QuestButtonPressed() {
        var player = Main.LocalPlayer;
        var questPlayer = player.GetModPlayer<AnalysisPlayer>();
        //Main.NewText(questPlayer.completed);
        if (!questPlayer.quest.isValid && questPlayer.timeForNextQuest == 0 && questPlayer.questResetTime <= 0) {
            questPlayer.quest = default(QuestInfo);
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                var p = Aequus.GetPacket(PacketType.RequestAnalysisQuest);
                p.Write(Main.myPlayer);
                p.Write(questPlayer.completed);
                p.Send();
            }
            else {
                questPlayer.RefreshQuest(questPlayer.completed);
            }
        }
        if (!questPlayer.quest.isValid || questPlayer.timeForNextQuest > 0) {
            Main.npcChatText = TextHelper.GetTextValueWith("Chat.Physicist.AnalysisRarityQuestNoQuest", new { Time = TextHelper.WatchTime(questPlayer.timeForNextQuest, questPlayer.dayTimeForNextQuest), });
            return;
        }

        var validItem = FindPotentialQuestItem(player, questPlayer.quest);
        if (Main.npcChatCornerItem > 0 && validItem != null) {
            var popupItem = validItem.Clone();
            popupItem.position = player.position;
            popupItem.width = player.width;
            popupItem.height = player.height;
            popupItem.stack = 1;
            var itemText = PopupText.NewText(PopupTextContext.RegularItemPickup, popupItem, 1);
            Main.popupText[itemText].name = $"{Main.popupText[itemText].name} (-1)";
            Main.popupText[itemText].position.X = Main.LocalPlayer.Center.X - FontAssets.ItemStack.Value.MeasureString(Main.popupText[itemText].name).X / 2f;

            validItem.stack--;
            if (validItem.stack <= 0) {
                validItem.TurnToAir();
            }
            questPlayer.completed++;
            var items = questPlayer.GetAnalysisRewardDrops();
            var source = player.talkNPC != -1 ? Main.npc[player.talkNPC].GetSource_GiftOrReward() : player.GetSource_GiftOrReward();
            foreach (var i in items) {
                player.QuickSpawnItem(source, i, i.stack);
            }
            int time = Main.rand.Next(28800, 43200);
            Helper.AddToTime(Main.time, time, Main.dayTime, out double result, out bool dayTime);
            questPlayer.timeForNextQuest = (int)Math.Min(result, dayTime ? Main.dayLength - 60 : Main.nightLength - 60);
            questPlayer.dayTimeForNextQuest = dayTime;
            SoundEngine.PlaySound(SoundID.Grab);
            Main.npcChatCornerItem = 0;
            Main.npcChatText = TextHelper.GetTextValue("Chat.Physicist.AnalysisRarityQuestComplete");
            return;
        }
        Main.npcChatText = QuestChat(questPlayer.quest);

        if (validItem != null) {
            Main.npcChatCornerItem = validItem.type;
            Main.npcChatText += $"\n{TextHelper.GetTextValueWith("Chat.Physicist.AnalysisRarityQuest2", new { Item = validItem.Name, })}";
        }
    }
    public static Item FindPotentialQuestItem(Player player, QuestInfo questInfo) {
        for (int i = 0; i < Main.InventorySlotsTotal; i++) {
            if (CanBeQuestItem(player.inventory[i], questInfo)) {
                return player.inventory[i];
            }
        }
        for (int i = 0; i < Chest.maxItems; i++) {
            if (CanBeQuestItem(player.bank4.item[i], questInfo)) {
                return player.bank4.item[i];
            }
        }
        return null;
    }
    public static bool CanBeQuestItem(Item item, QuestInfo questInfo) {
        return !item.favorited && !item.IsAir && !item.IsACoin &&
            item.OriginalRarity == questInfo.itemRarity && !AnalysisSystem.IgnoreItem.Contains(item.type) && !Main.itemAnimationsRegistered.Contains(item.type);
    }
    public static string QuestChat(QuestInfo questInfo) {
        return TextHelper.GetTextValueWith("Chat.Physicist.AnalysisRarityQuest", new { Rarity = TextHelper.ColorCommand(TextHelper.GetRarityNameValue(questInfo.itemRarity), Helper.GetRarityColor(questInfo.itemRarity)), });
    }

    public override bool CanGoToStatue(bool toKingStatue) {
        return !toKingStatue;
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        damage = 20;
        knockback = 8f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
        cooldown = 900;
        randExtraCooldown = 120;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        projType = ModContent.ProjectileType<PhysicistTownSentryProj>();
        attackDelay = 1;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
        multiplier = 9f;
        randomOffset = 1f;
    }

    public override void AI() {
        pet.Update(NPC);
    }

    public override void OnGoToStatue(bool toKingStatue) {
        int pet = NPC.FindFirstNPC(ModContent.NPCType<PhysicistPet>());
        if (pet == -1) {
            Main.npc[pet].Center = NPC.Center;
            Main.npc[pet].netUpdate = true;
        }
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (NPC.IsShimmerVariant && (int)NPC.ai[0] != 25) {
            var spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(
                AequusTextures.Physicist_Glow,
                NPC.Center + new Vector2(0f, -6f + NPC.gfxOffY) - Main.screenPosition,
                NPC.frame,
                Color.White * (NPC.Opacity * (1f - NPC.shimmerTransparency)),
                NPC.rotation,
                NPC.frame.Size() / 2f,
                NPC.scale, spriteEffects, 0f
            );
        }
    }

    public override void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
        Helper.ReplaceTextWithStringArgs(ref settings.HappinessReport, "[HateBiomeQuote]|",
            $"Mods.Aequus.TownNPCMood.Physicist.HateBiome_{(player.ZoneHallow ? "Hallow" : "Evils")}", (s) => new { BiomeName = s[1], });
    }
}