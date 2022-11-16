using Aequus.Biomes;
using Aequus.Common.Utilities;
using Aequus.Content.CarpenterBounties;
using Aequus.Items.Consumables.Coatings;
using Aequus.Items.Placeable.Furniture.Paintings;
using Aequus.Items.Tools;
using Aequus.Items.Tools.Camera;
using Aequus.Items.Weapons.Ranged;
using Aequus.Projectiles.Misc;
using Aequus.Projectiles.Ranged;
using Aequus.UI.CarpenterUI;
using Microsoft.Xna.Framework;
using ShopQuotesMod;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.NPCs.Friendly.Town
{
    [AutoloadHead()]
    public class Carpenter : ModNPC
    {
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
                .SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
                .SetNPCAffection(NPCID.Clothier, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Truffle, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Hate);

            NPCHappiness.Get(NPCID.Truffle).SetNPCAffection(Type, AffectionLevel.Love);
            NPCHappiness.Get(NPCID.Dryad).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.BestiaryGirl).SetNPCAffection(Type, AffectionLevel.Like);
            NPCHappiness.Get(NPCID.Angler).SetNPCAffection(Type, AffectionLevel.Like);

            ModContent.GetInstance<QuoteDatabase>().AddNPC(Type, Mod, "Mods.Aequus.ShopQuote.")
                .UseColor(new Color(165, 140, 190));
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 50;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.1f;
            AnimationType = NPCID.Guide;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = (int)Math.Clamp(damage / 3, NPC.life > 0 ? 1 : 40, 40);
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood, newColor: new Color(Main.rand.Next(100, 155), 255, 255, 255));
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.Underground);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<BlockGlove>(chance: 6, stack: 1);
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Shutterstocker>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ShutterstockerClipAmmo>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SilkPickaxe>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SilkHammer>());
            if (NPC.AnyNPCs(NPCID.Painter))
            {
                shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ImpenetrableCoating>());
                if (!Main.dayTime)
                {
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<OliverPainting>());
                }
            }

            var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            foreach (var bounty in CarpenterSystem.BountiesByID)
            {
                if (bountyPlayer.CompletedBounties.Contains(bounty.FullName))
                {
                    shop.item[nextSlot++] = bounty.ProvideBountyRewardItem();
                }
            }
        }

        public override bool CheckConditions(int left, int right, int top, int bottom)
        {
            var stopWatch = new Stopwatch();
            var houseInsideTiles = GetHouseInsideTiles((left + right) / 2, (top + bottom) / 2);
            int decorAmt = CountDecorInsideHouse(houseInsideTiles);
            return decorAmt >= 4;
        }

        public static List<Point> GetHouseInsideTiles(int x, int y)
        {
            var addPoints = new List<Point>();
            var checkedPoints = new List<Point>() { new Point(x, y) };
            var offsets = new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1), };
            for (int k = 0; k < 1000; k++)
            {
                checkedPoints.AddRange(addPoints);
                addPoints.Clear();
                bool addedAny = false;
                if (checkedPoints.Count > 1000)
                {
                    return checkedPoints;
                }
                for (int l = 0; l < checkedPoints.Count; l++)
                {
                    for (int m = 0; m < offsets.Length; m++)
                    {
                        var newPoint = new Point(checkedPoints[l].X + offsets[m].X, checkedPoints[l].Y + offsets[m].Y);
                        if (WorldGen.InWorld(newPoint.X, newPoint.Y, 10) && !checkedPoints.Contains(newPoint) && !addPoints.Contains(newPoint) &&
                            (!Main.tile[newPoint].HasTile || (!Main.tile[newPoint].SolidType() && !Main.tile[newPoint].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsDoor))) && Main.tile[newPoint].WallType != WallID.None && Main.wallHouse[Main.tile[newPoint].WallType])
                        {
                            addPoints.Add(newPoint);
                            addedAny = true;
                        }
                    }
                }
                if (!addedAny)
                {
                    return checkedPoints;
                }
            }
            return checkedPoints;
        }

        public static void GetHouseWallTiles(List<Point> insideTiles, out List<Point> leftPoints, out List<Point> rightPoints, out List<Point> topPoints, out List<Point> bottomPoints)
        {
            leftPoints = new List<Point>();
            rightPoints = new List<Point>();
            topPoints = new List<Point>();
            bottomPoints = new List<Point>();

            foreach (var p in insideTiles)
            {
                if (Main.tile[p.X - 1, p.Y].IsSolid())
                {
                    leftPoints.Add(new Point(p.X - 1, p.Y));
                }
                if (Main.tile[p.X + 1, p.Y].IsSolid())
                {
                    rightPoints.Add(new Point(p.X + 1, p.Y));
                }
                if (Main.tile[p.X, p.Y + 1].IsSolid())
                {
                    bottomPoints.Add(new Point(p.X, p.Y + 1));
                }
                if (Main.tile[p.X, p.Y - 1].IsSolid())
                {
                    topPoints.Add(new Point(p.X, p.Y - 1));
                }
            }
        }

        public static int CountDecorInsideHouse(List<Point> insideTiles)
        {
            int decorAmt = 0;
            var tileStyleData = new Dictionary<int, List<int>>();

            foreach (var p in insideTiles)
            {
                if (Main.tile[p].HasTile)
                {
                    if (Main.tile[p].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsTable) || Main.tile[p].IsIncludedIn(TileID.Sets.RoomNeeds.CountsAsChair) || TileID.Sets.Torch[Main.tile[p].TileType])
                    {
                        continue;
                    }
                    if (!Main.tile[p].IsSolid())
                    {
                        int style = TileObjectData.GetTileStyle(Main.tile[p]);
                        if (tileStyleData.TryGetValue(Main.tile[p].TileType, out List<int> compareStyle))
                        {
                            if (compareStyle.Contains(style))
                            {
                                continue;
                            }
                            compareStyle.Add(style);
                        }
                        else
                        {
                            tileStyleData.Add(Main.tile[p].TileType, new List<int>() { style });
                        }

                        decorAmt++;
                    }
                }
            }
            return decorAmt;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return true;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Brut",
                "Hank",
                "Venom",
                "Anthony",
                "Filk",
                "Scott",
                "Charlie",
                "Mob",
                "Utopis",
                "Mine",
                "Villis",
                "Dange",
                "Cryst",
                "Carmelita",
            };
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return base.TownNPCProfile();
        }

        public override string GetChat()
        {
            var player = Main.LocalPlayer;
            var chat = new SelectableChat("Mods.Aequus.Chat.Carpenter.");

            if (GlimmerBiome.EventActive && Main.rand.NextBool())
            {
                chat.Add("Glimmer");
            }
            else if (Main.bloodMoon && Main.rand.NextBool())
            {
                chat.Add("BloodMoon");
            }
            else
            {
                chat.Add("Basic.0");
                chat.Add("Basic.1");
                chat.Add("Basic.2");
            }

            if (NPC.downedGoblins || Main.invasionType == InvasionID.GoblinArmy)
            {
                chat.Add("GoblinArmy");
            }

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = AequusText.GetText("Chat.Carpenter.UI.BountyButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
                foreach (var bounty in CarpenterSystem.BountiesByID)
                {
                    if (bounty.IsBountyAvailable() && !bountyPlayer.CompletedBounties.Contains(bounty.FullName))
                    {
                        Main.playerInventory = false;
                        Main.npcChatText = "";
                        Aequus.UserInterface.SetState(new CarpenterUIState());
                        return;
                    }
                }
                Main.npcChatText = AequusText.TryGetText("Chat.Carpenter.NoBounty");
            }
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return !toKingStatue;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 2;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<CarpenterProj>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 10f;
            gravityCorrection = 0f;
            randomOffset = 2f;
        }
    }
}