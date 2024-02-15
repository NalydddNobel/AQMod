using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.TownNPCs;
using Aequus.Core;
using Aequus.Old.Content.Events.Glimmer;
using Aequus.Old.Content.TownNPCs.OccultistNPC;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.Localization;

namespace Aequus.Old.Content.TownNPCs.PhysicistNPC;

[AutoloadHead()]
public partial class Physicist : AequusTownNPC<Physicist> {
    public static int AwaitQuest { get; private set; }
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
        ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, AequusTextures.Physicist_Shimmer_Head.Path);
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        NPCSets.AttackType[NPC.type] = 0;
        NPCSets.AttackTime[NPC.type] = 10;
        NPCSets.AttackAverageChance[NPC.type] = 10;
        NPCSets.HatOffsetY[NPC.type] = 2;

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

    public override bool CanTownNPCSpawn(int numTownNPCs) {
        return WorldState.DownedCosmicBoss || WorldState.DownedTrueCosmicBoss;
    }

    public override ITownNPCProfile TownNPCProfile() {
        return base.TownNPCProfile();
    }

    public override string GetChat() {
        if (Main.invasionType == InvasionID.MartianMadness) {
            return this.GetDialogue("MartianMadnessOccuring").Value;
        }

        return Main.rand.Next(GetAvailableChat(Main.LocalPlayer).ToArray());
    }

    private IEnumerable<string> GetAvailableChat(Player player) {
        if (GlimmerZone.EventActive) {
            for (int i = 0; i < 2; i++) {
                yield return this.GetDialogue($"Glimmer.{i}").Value;
            }
        }
        else if (Main.bloodMoon) {
            for (int i = 0; i < 2; i++) {
                yield return this.GetDialogue($"BloodMoon.{i}").Value;
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                yield return this.GetDialogue($"Basic.{i}").Value;
            }
            if (!Main.dayTime) {
                for (int i = 0; i < 1; i++) {
                    yield return this.GetDialogue($"Night.{i}").Value;
                }
            }
        }

        if (Main.IsItAHappyWindyDay) {
            yield return this.GetDialogue("WindyDay").Value;
            yield return this.GetDialogue("Pylon").Value;
        }
        if (Main.raining) {
            yield return this.GetDialogue("Rain").Value;
        }
        if (Main.IsItStorming) {
            yield return this.GetDialogue("Thunderstorm").Value;
        }
        if (BirthdayParty.PartyIsUp) {
            yield return this.GetDialogue("Party").Value;
        }
        if (player.ZoneGraveyard) {
            for (int i = 0; i < 2; i++) {
                yield return this.GetDialogue($"Graveyard.{i}").Value;
            }
        }
        if (ModLoader.TryGetMod("SoTS", out var soTS)) {
            for (int i = 0; i < 2; i++) {
                yield return this.GetDialogue($"SoTSMod.{i}").Value;
            }
        }
        if (NPC.downedGolemBoss && NPC.AnyNPCs(NPCID.Mechanic) && ModLoader.TryGetMod("Polarities", out var polarities)) {
            yield return this.GetDialogue("PolaritiesMod").Value;
        }
        if (NPC.downedMartians) {
            yield return this.GetDialogue("MartianMadness").Value;
        }
        if (NPC.TowerActiveNebula || NPC.TowerActiveSolar || NPC.TowerActiveStardust || NPC.TowerActiveVortex || NPC.MoonLordCountdown > 0 || NPC.AnyNPCs(NPCID.MoonLordCore)) {
            yield return this.GetDialogue("LunarPillars").Value;
        }
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
        //button2 = Main.npcChatCornerItem > 0 ? TextHelper.GetTextValue("Chat.Physicist.AnalysisButtonComplete") : TextHelper.GetTextValue("Chat.Physicist.AnalysisButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        if (firstButton) {
            shopName = "Shop";
            return;
        }

        AwaitQuest = 30;
        //QuestButtonPressed();
    }

    //public static void QuestButtonPressed() {
    //    var player = Main.LocalPlayer;
    //    var questPlayer = player.GetModPlayer<AnalysisPlayer>();
    //    //Main.NewText(questPlayer.completed);
    //    if (!questPlayer.quest.isValid && questPlayer.timeForNextQuest == 0 && questPlayer.questResetTime <= 0) {
    //        questPlayer.quest = default(QuestInfo);
    //        if (Main.netMode == NetmodeID.MultiplayerClient) {
    //            var p = Aequus.GetPacket(PacketType.RequestAnalysisQuest);
    //            p.Write(Main.myPlayer);
    //            p.Write(questPlayer.completed);
    //            p.Send();
    //        }
    //        else {
    //            questPlayer.RefreshQuest(questPlayer.completed);
    //        }
    //    }
    //    if (!questPlayer.quest.isValid || questPlayer.timeForNextQuest > 0) {
    //        Main.npcChatText = TextHelper.GetTextValueWith("Chat.Physicist.AnalysisRarityQuestNoQuest", new { Time = TextHelper.WatchTime(questPlayer.timeForNextQuest, questPlayer.dayTimeForNextQuest), });
    //        return;
    //    }

    //    var validItem = FindPotentialQuestItem(player, questPlayer.quest);
    //    if (Main.npcChatCornerItem > 0 && validItem != null) {
    //        var popupItem = validItem.Clone();
    //        popupItem.position = player.position;
    //        popupItem.width = player.width;
    //        popupItem.height = player.height;
    //        popupItem.stack = 1;
    //        var itemText = PopupText.NewText(PopupTextContext.RegularItemPickup, popupItem, 1);
    //        Main.popupText[itemText].name = $"{Main.popupText[itemText].name} (-1)";
    //        Main.popupText[itemText].position.X = Main.LocalPlayer.Center.X - FontAssets.ItemStack.Value.MeasureString(Main.popupText[itemText].name).X / 2f;

    //        validItem.stack--;
    //        if (validItem.stack <= 0) {
    //            validItem.TurnToAir();
    //        }
    //        questPlayer.completed++;
    //        var items = questPlayer.GetAnalysisRewardDrops();
    //        var source = player.talkNPC != -1 ? Main.npc[player.talkNPC].GetSource_GiftOrReward() : player.GetSource_GiftOrReward();
    //        foreach (var i in items) {
    //            player.QuickSpawnItem(source, i, i.stack);
    //        }
    //        int time = Main.rand.Next(28800, 43200);
    //        Helper.AddToTime(Main.time, time, Main.dayTime, out double result, out bool dayTime);
    //        questPlayer.timeForNextQuest = (int)Math.Min(result, dayTime ? Main.dayLength - 60 : Main.nightLength - 60);
    //        questPlayer.dayTimeForNextQuest = dayTime;
    //        SoundEngine.PlaySound(SoundID.Grab);
    //        Main.npcChatCornerItem = 0;
    //        Main.npcChatText = TextHelper.GetTextValue("Chat.Physicist.AnalysisRarityQuestComplete");
    //        return;
    //    }
    //    Main.npcChatText = QuestChat(questPlayer.quest);

    //    if (validItem != null) {
    //        Main.npcChatCornerItem = validItem.type;
    //        Main.npcChatText += $"\n{TextHelper.GetTextValueWith("Chat.Physicist.AnalysisRarityQuest2", new { Item = validItem.Name, })}";
    //    }
    //}
    //public static Item FindPotentialQuestItem(Player player, QuestInfo questInfo) {
    //    for (int i = 0; i < Main.InventorySlotsTotal; i++) {
    //        if (CanBeQuestItem(player.inventory[i], questInfo)) {
    //            return player.inventory[i];
    //        }
    //    }
    //    for (int i = 0; i < Chest.maxItems; i++) {
    //        if (CanBeQuestItem(player.bank4.item[i], questInfo)) {
    //            return player.bank4.item[i];
    //        }
    //    }
    //    return null;
    //}

    //public static bool CanBeQuestItem(Item item, QuestInfo questInfo) {
    //    return !item.favorited && !item.IsAir && !item.IsACoin &&
    //        item.OriginalRarity == questInfo.itemRarity && !AnalysisSystem.IgnoreItem.Contains(item.type) && !Main.itemAnimationsRegistered.Contains(item.type);
    //}

    //public static string QuestChat(QuestInfo questInfo) {
    //    return TextHelper.GetTextValueWith("Chat.Physicist.AnalysisRarityQuest", new { Rarity = TextHelper.ColorCommand(TextHelper.GetRarityNameValue(questInfo.itemRarity), Helper.GetRarityColor(questInfo.itemRarity)), });
    //}

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
                AequusTextures.Physicist_Shimmer_Glow,
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
        DialogueHack.ReplaceKeys(ref settings.HappinessReport, "[HateBiomeQuote]|",
            $"Mods.Aequus.TownNPCMood.Physicist.HateBiome_{(player.ZoneHallow ? "Hallow" : "Evils")}", (s) => new { BiomeName = s[1], });
    }

    public struct PetInfo {
        public int PetNPCIndex;
        public int SpawnPetCheck;

        public static bool IsMyPet(NPC owner, NPC pet) {
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
}