using Aequus.Common;
using Aequus.Common.Primitives;
using Aequus.Common.Utilities;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.Town;
using Aequus.Items.Accessories.Misc.Building;
using Aequus.Items.Consumables;
using Aequus.Items.Tools.CarpenterCamera;
using Aequus.Items.Weapons.Ranged.Thrown;
using Aequus.NPCs;
using Aequus.NPCs.Town.CarpenterNPC.Quest;
using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;
using Aequus.Particles.Dusts;
using Aequus.Tiles.Furniture.Paintings.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.GameContent.Profiles;

namespace Aequus.NPCs.Town.CarpenterNPC {
    [AutoloadHead()]
    public class Carpenter : AequusTownNPC<Carpenter> {
        public static int showExclamation;

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
            // Adds our Shimmer Head to the NPCHeadLoader.
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            Profile = new StackedNPCProfile(
                new DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
                new DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex)
            );
            NPCID.Sets.AttackType[NPC.type] = 0; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
            NPCID.Sets.AttackTime[NPC.type] = 10;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.HatOffsetY[NPC.type] = 2;

            NPCID.Sets.DebuffImmunitySets.Add(Type, new NPCDebuffImmunityData() {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Wet,
                    BuffID.Confused,
                    BuffID.Lovestruck,
                }
            });

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
            this.CreateLoot(npcLoot)
                .Add<BlockGlove>(chance: 6, stack: 1);
        }

        public void AddResetSheet(Chest shop, ref int nextSlot) {
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CarpenterResetSheet>());
        }

        private static Condition AllBountiesCompleteCondition => new(TextHelper.GetText("Condition.DemonSiege"), () => {
            foreach (var bounty in CarpenterSystem.BountiesByID) {
                if (!CarpenterSystem.CompletedBounties.Contains(bounty.FullName)) {
                    return false;
                }
            }
            return true;
        });

        public override void AddShops() {
            NPCShop shop = new(Type);
            shop.Add<Shutterstocker>()
                .Add<ShutterstockerClipAmmo>()
                .Add<OliverPainting>(Condition.NightOrEclipse, Condition.NpcIsPresent(NPCID.Painter))
                .AddWithCustomValue(ItemID.IvyChest, Item.buyPrice(gold: 1), Condition.TimeDay)
                .AddWithCustomValue(ItemID.WebCoveredChest, Item.buyPrice(gold: 1), Condition.TimeNight)
                .Add<LavaproofMitten>(AequusConditions.DownedDemonSiege)
                .Add<CarpenterResetSheet>(AllBountiesCompleteCondition)
                .AddWithCustomValue(ItemID.Seed, Item.sellPrice(copper: 2))
                .Register();
        }
        public override void ModifyActiveShop(string shopName, Item[] items) {
            int nextSlot = Helper.FindNextShopSlot(items);
            var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            foreach (var bounty in CarpenterSystem.BountiesByID) {
                if (nextSlot >= items.Length) {
                    break; // guh
                }
                if (CarpenterSystem.CompletedBounties.Contains(bounty.FullName)) {
                    var rewards = bounty.ProvideBountyRewardItems();
                    foreach (var item in rewards) {
                        item.stack = 1;
                        items[nextSlot++] = item;
                    }
                }
                else {
                    items[nextSlot++] = bounty.ProvidePortableBounty().Item;
                }
            }
        }

        public override bool CheckConditions(int left, int right, int top, int bottom) {
            var stopWatch = new Stopwatch();
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

        public override bool CanTownNPCSpawn(int numTownNPCs)/* tModPorter Suggestion: Copy the implementation of NPC.SpawnAllowed_Merchant in vanilla if you to count money, and be sure to set a flag when unlocked, so you don't count every tick. */ {
            return true;
        }

        public override string GetChat() {
            showExclamation = 0;
            var player = Main.LocalPlayer;
            var chat = new SelectableChatHelper("Mods.Aequus.Chat.Carpenter.");

            if (GlimmerBiomeManager.EventActive && Main.rand.NextBool()) {
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
            showExclamation = 0;
            if (firstButton) {
                shopName = "Shop";
            }
            else {
                var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
                foreach (var bounty in CarpenterSystem.BountiesByID) {
                    if (CarpenterSystem.CompletedBounties.Contains(bounty.FullName) && !bountyPlayer.collectedBounties.Contains(bounty.FullName)) {
                        bountyPlayer.collectedBounties.Add(bounty.FullName);
                        bounty.OnCompleteBounty(Main.LocalPlayer, NPC);
                        Main.npcChatText = $"Congrats on completing {bounty.DisplayName}!";
                        return;
                    }
                }
                Main.playerInventory = false;
                Main.npcChatText = "";
                Aequus.UserInterface.SetState(new BountyUIState());
            }
        }

        public override void AI() {
            if (showExclamation == 0 && Main.netMode != NetmodeID.Server) {
                if (Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>().HasUnclaimedBounty()) {
                    showExclamation = 1;
                }
                else {
                    showExclamation = -1;
                }
            }

            if (thunderDelay > 0) {
                thunderDelay--;
                NPC.velocity.X *= 0f;
            }
            else if (Main.lightning > 0.5f) {
                thunderDelay = 60;
                NPC.velocity.Y = -4f;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            if (thunderDelay <= 0 && showExclamation > 0 && !NPC.IsABestiaryIconDummy) {
                spriteBatch.Draw(TrailTextures.TownNPCExclamation.Value, NPC.Top + new Vector2(0f, -26f) - screenPos, null,
                    new Color(150, 150, 255, 222), 0f, TrailTextures.TownNPCExclamation.Value.Size() / 2f, Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.9f, 1f), SpriteEffects.None, 0f);
            }
            return true;
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
}