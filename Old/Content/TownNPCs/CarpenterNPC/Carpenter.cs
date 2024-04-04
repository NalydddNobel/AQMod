﻿using Aequus.Common.NPCs.Bestiary;
using Aequus.Content.TownNPCs;
using System;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Personalities;
using Terraria.Localization;
using Terraria.ObjectData;
using static Terraria.GameContent.Profiles;

namespace Aequus.NPCs.Town.CarpenterNPC;

[AutoloadHead()]
public class Carpenter : AequusTownNPC<Carpenter> {
    private int thunderDelay;

    public override List<string> SetNPCNameList() {
        return new() {
            "Brut",
            "Filk",
            "Mob",
            "Utopis",
            "Mine",
            "Villis",
            "Cryst",
            "Pilk",
        };
    }

    internal void SetupShopQuotes(Mod shopQuotes) {
        shopQuotes.Call("AddNPC", Mod, Type);
        shopQuotes.Call("SetColor", Type, new Color(165, 140, 190));
    }

    public override void Load() {
        ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        Profile = new StackedNPCProfile(
            new DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
            new DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex)
        );
        NPCID.Sets.AttackType[NPC.type] = 0;
        NPCID.Sets.AttackTime[NPC.type] = 10;
        NPCID.Sets.AttackAverageChance[NPC.type] = 10;
        NPCID.Sets.HatOffsetY[NPC.type] = 2;

        NPC.Happiness
            .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
            .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Like)
            .SetBiomeAffection<JungleBiome>(AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Painter, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Clothier, AffectionLevel.Love)
            .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like)
            .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
            .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike)
            .SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate);

        NPCHappiness.Get(NPCID.Truffle).SetNPCAffection(Type, AffectionLevel.Love);
        NPCHappiness.Get(NPCID.Dryad).SetNPCAffection(Type, AffectionLevel.Like);
        NPCHappiness.Get(NPCID.BestiaryGirl).SetNPCAffection(Type, AffectionLevel.Like);
        NPCHappiness.Get(NPCID.Angler).SetNPCAffection(Type, AffectionLevel.Like);
    }

    public override void HitEffect(NPC.HitInfo hit) {
        int dustAmount = Math.Clamp(hit.Damage / 3, NPC.life > 0 ? 1 : 40, 40);
        for (int k = 0; k < dustAmount; k++) {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, newColor: new Color(Main.rand.Next(100, 155), 255, 255, 255));
        }
    }

    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
        this.CreateEntry(database, bestiaryEntry)
            .AddMainSpawn(BestiaryBuilder.Underground);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BlockGlove>(), chanceDenominator: 6));
    }

    public static Condition AllCarpenterBountiesCompleteCondition => new(TextHelper.GetText("Condition.AllCarpenterBountiesComplete"), () => {
        foreach (var buildChallenge in BuildChallengeLoader.registeredBuildChallenges) {
            if (!CarpentrySystem.CompletedBounties.ContainsChallenge(buildChallenge)) {
                return false;
            }
        }
        return true;
    });

    public override void AddShops() {
        NPCShop shop = new(Type);
        shop.Add<Shutterstocker>()
            .AddCustomValue(ItemID.IvyChest, Item.buyPrice(gold: 1), Condition.TimeDay)
            .AddCustomValue(ItemID.WebCoveredChest, Item.buyPrice(gold: 1), Condition.TimeNight)
            .Add<Aequus.Old.Content.Carpentry.Items.LavaproofMitten>(AequusConditions.DownedDemonSiege)
            .Add<CarpenterResetSheet>(AllCarpenterBountiesCompleteCondition)
            .AddCustomValue(ItemID.Seed, Item.sellPrice(copper: 2));

        foreach (var buildChallenge in BuildChallengeLoader.registeredBuildChallenges) {
            var rewards = buildChallenge.GetRewards();
            var condition = AequusConditions.HasCompletedBuildChallenge(buildChallenge);
            foreach (var item in rewards) {
                item.stack = 1;
                shop.Add(item, condition);
            }
        }

        shop.Register();
    }

    public override void ModifyActiveShop(string shopName, Item[] items) {
        //int nextSlot = Helper.FindNextShopSlot(items);
        //var carpentryPlayer = Main.LocalPlayer.GetModPlayer<CarpentryPlayer>();
    }

    public override bool CheckConditions(int left, int right, int top, int bottom) {
        if (Main.zenithWorld) {
            return true;
        }

        //var stopWatch = new Stopwatch();
        var houseInsideTiles = GetHouseInsideTiles((left + right) / 2, (top + bottom) / 2);
        int decorAmt = CountDecorInsideHouse(houseInsideTiles);
        return decorAmt >= 4;
    }

    public static List<Point> GetHouseInsideTiles(int x, int y) {
        var addPoints = new List<Point>();
        var checkedPoints = new List<Point>() { new Point(x, y) };
        var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
        for (int k = 0; k < 1000; k++) {
            checkedPoints.AddRange(addPoints);
            addPoints.Clear();
            bool addedAny = false;
            if (checkedPoints.Count > 1000) {
                return checkedPoints;
            }
            for (int l = 0; l < checkedPoints.Count; l++) {
                for (int m = 0; m < offsets.Length; m++) {
                    var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                    if (WorldGen.InWorld(newPoint.X, newPoint.Y, 10) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) &&
                        (!Main.tile[newPoint].HasTile || !Main.tile[newPoint].SolidType() && !Main.tile[newPoint].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor)) && Main.tile[newPoint].WallType != WallID.None && Main.wallHouse[Main.tile[newPoint].WallType]) {
                        addPoints.Add(newPoint);
                        addedAny = true;
                    }
                }
            }
            if (!addedAny) {
                return checkedPoints;
            }
        }
        return checkedPoints;
    }

    public static int CountDecorInsideHouse(List<Point> insideTiles) {
        int decorAmt = 0;
        var tileStyleData = new Dictionary<int, List<int>>();

        foreach (var p in insideTiles) {
            if (Main.tile[p].HasTile) {
                if (Main.tile[p].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsTable) || Main.tile[p].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsChair) || TileID.Sets.Torch[Main.tile[p].TileType]) {
                    continue;
                }
                if (!Main.tile[p].IsSolid()) {
                    int style = TileObjectData.GetTileStyle(Main.tile[p]);
                    if (tileStyleData.TryGetValue(Main.tile[p].TileType, out List<int> compareStyle)) {
                        if (compareStyle.Contains(style)) {
                            continue;
                        }
                        compareStyle.Add(style);
                    }
                    else {
                        tileStyleData.Add(Main.tile[p].TileType, new List<int>() { style });
                    }

                    decorAmt++;
                }
            }
        }
        return decorAmt;
    }

    public override bool CanTownNPCSpawn(int numTownNPCs) {
        return true;
    }

    public override void TalkNPCUpdate(Player player) {
        if (!Main.LocalPlayer.TryGetModPlayer<CarpentryPlayer>(out var carpenterPlayer)) {
            return;
        }

        if (!carpenterPlayer.CanClaimFreeShutterstockerGift) {
            carpenterPlayer.CanClaimFreeShutterstockerGift = true;
            if (Main.myPlayer == player.whoAmI) {
                player.QuickSpawnItem(NPC?.GetSource_GiftOrReward("FreeShutterstocker"), ModContent.ItemType<Shutterstocker>());
            }
        }
    }

    public override string GetChat() {
        if (Main.LocalPlayer.TryGetModPlayer<CarpentryPlayer>(out var carpenterPlayer)) {
            if (!carpenterPlayer.CanClaimFreeShutterstockerGift) {
                return TextHelper.GetTextValue("Carpenter.Dialogue.GiveCamera");
            }
        }

        CheckExclamationTimer = 0;
        var player = Main.LocalPlayer;
        SelectableChatHelper chat = new("Mods.Aequus.Carpenter.Dialogue.");

        if (GlimmerZone.EventActive && Main.rand.NextBool()) {
            chat.Add("Glimmer");
        }
        else if (Main.bloodMoon && Main.rand.NextBool()) {
            chat.Add("BloodMoon");
        }

        if (Main.IsItAHappyWindyDay) {
            chat.Add("WindyDay");
        }
        else if (Main.IsItStorming) {
            chat.Add("Thunderstorm");
        }

        if (Main.raining) {
            chat.Add("Rain");
        }
        if (Main.rand.NextBool(7)) {
            chat.Add("Basic.Rare");
        }
        else {
            chat.Add("Basic.0");
            chat.Add("Basic.1");
            chat.Add("Basic.2");
        }

        if (NPC.downedGoblins || Main.invasionType == InvasionID.GoblinArmy) {
            chat.Add("GoblinArmy");
        }
        if (BirthdayParty.PartyIsUp) {
            chat.Add("Party");
        }
        if (Main.LocalPlayer.ZoneGraveyard) {
            chat.Add("Graveyard");
        }

        return chat.Get();
    }

    public override void SetChatButtons(ref string button, ref string button2) {
        button = Language.GetTextValue("LegacyInterface.28");
        button2 = TextHelper.GetTextValue("Chat.Carpenter.UI.BountyButton");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
        base.OnChatButtonClicked(firstButton, ref shopName);
        if (firstButton) {
            shopName = "Shop";
            return;
        }

        var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpentryPlayer>();
        foreach (var buildChallenge in BuildChallengeLoader.registeredBuildChallenges) {
            if (CarpentrySystem.CompletedBounties.ContainsChallenge(buildChallenge) && !bountyPlayer.CollectedBounties.ContainsChallenge(buildChallenge.FullName)) {
                bountyPlayer.CollectedBounties.Add(buildChallenge);
                buildChallenge.OnCompleteBounty(Main.LocalPlayer, NPC);
                Main.npcChatText = buildChallenge.GetCompletionMessage().Value;
                return;
            }
        }

        Main.playerInventory = false;
        Main.npcChatText = "";
        Aequus.UserInterface.SetState(new BountyBoardUIState());
    }

    protected override bool CheckExclamation() {
        if (!Main.LocalPlayer.TryGetModPlayer<CarpentryPlayer>(out var carpentryPlayer) || !carpentryPlayer.CanClaimFreeShutterstockerGift) {
            return false;
        }
        if (carpentryPlayer.HasUnclaimedBounty()) {
            CheckExclamationTimer = 200;
            return true;
        }

        return false;
    }

    public override void AI() {
        base.AI();
        if (thunderDelay > 0) {
            thunderDelay--;
            NPC.velocity.X *= 0f;
        }
        else if (Main.lightning > 0.5f) {
            thunderDelay = 60;
            NPC.velocity.Y = -4f;
            NPC.netUpdate = true;
            var d = Dust.NewDustPerfect(NPC.Top, ModContent.DustType<CarpenterSurpriseDust>(), Scale: 0.25f);
            d.velocity.X *= 0f;
            d.velocity.Y = -3f;
            d.position.Y -= 8f;
            d.noLight = true;
            d.fadeIn = 1f;
        }
    }

    public override bool CanGoToStatue(bool toKingStatue) {
        return true;
    }

    protected override bool PreDrawExclamation(SpriteBatch spriteBatch, Vector2 screenPos, Color npcDrawColor) {
        return thunderDelay <= 0;
    }

    public override void TownNPCAttackStrength(ref int damage, ref float knockback) {
        damage = 20;
        knockback = 2f;
    }

    public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown) {
        cooldown = 10;
        randExtraCooldown = 2;
    }

    public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        projType = ModContent.ProjectileType<CarpenterProj>();
        attackDelay = 1;
    }

    public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset) {
        multiplier = 10f;
        gravityCorrection = 0f;
        randomOffset = 2f;
    }

    public override void ModifyShoppingSettings(Player player, NPC npc, ref ShoppingSettings settings, ShopHelper shopHelper) {
        Helper.ReplaceText(ref settings.HappinessReport, "[LikeBiomeQuote]", TextHelper.GetTextValue($"TownNPCMood.Carpenter.LikeBiome_{(npc.homeTileY < Main.worldSurface ? "Forest" : "Underground")}"));
    }
}