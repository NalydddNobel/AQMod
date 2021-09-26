using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Config;
using AQMod.Common.WorldEvents;
using AQMod.Items.Accessories.ShopCards;
using AQMod.Items.BossItems.Crabson;
using AQMod.Items.BossItems.Starite;
using AQMod.Items.BuffItems;
using AQMod.Items.GrapplingHooks.Barbs;
using AQMod.Items.Misc.Energies;
using AQMod.Items.Misc.QuestRobster;
using AQMod.Items.Placeable.Mushrooms;
using AQMod.Items.Weapons.Magic.Staffs;
using AQMod.Items.Weapons.Melee.Flails;
using AQMod.Items.Weapons.Ranged.Bows;
using AQMod.Localization;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.NPCs.Town
{
    [AutoloadHead()]
    public class Robster : ModNPC
    {
        public const int STORY_NONE = 2;

        public const int STORY_FIND_SNOBSTER = 0;
        public const int STORY_FIND_CRAB_CARD = 1;

        public const int RANDOM_COUNT = 2;

        public const int RANDOM_JEWELED_CHALICE = 1;
        public const int RANDOM_JEWELED_CANDELABRA = 2;

        private static byte _resetQuest;

        public static byte StoryProgression { get; set; }
        public static byte RandomHunt { get; set; }
        public static uint RandomsCompleted { get; set; }
        public static bool HuntInitialized { get; set; }
        public static byte TargetNPC { get; set; } = 255;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 400;
            NPCID.Sets.AttackType[npc.type] = 0;
            NPCID.Sets.AttackTime[npc.type] = 10;
            NPCID.Sets.AttackAverageChance[npc.type] = 10;
            NPCID.Sets.HatOffsetY[npc.type] = 8;
            Initalize();
        }

        internal static void Initalize()
        {
            HuntInitialized = true;
            StoryProgression = 0;
            RandomHunt = 0;
            RandomsCompleted = 0;
            _resetQuest = 0;
            TargetNPC = 255;
        }

        public override void SetDefaults()
        {
            npc.townNPC = true;
            npc.friendly = true;
            npc.width = 18;
            npc.height = 40;
            npc.aiStyle = 7;
            npc.damage = 10;
            npc.defense = 15;
            npc.lifeMax = 250;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.5f;
            animationType = NPCID.Guide;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = npc.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return WorldDefeats.DownedCrabson;
        }

        public override string TownNPCName()
        {
            if (StoryProgression > STORY_FIND_CRAB_CARD)
                return "Robster";
            switch (WorldGen.genRand.Next(11))
            {
                default:
                return "Larry";
                case 0:
                return "Ronald";
                case 1:
                return "Captain";
                case 2:
                return "Crabort";
                case 3:
                return "Crabulon";
                case 4:
                return "Geezer";
                case 5:
                return "Albrecht";
                case 6:
                return "Eugene";
                case 7:
                return "Utagawa";
                case 8:
                return "Ebirah";
                case 9:
                return "Tamatoa";
                case 10:
                return "Crablante";
            }
        }

        public override string GetChat()
        {
            try
            {
                if (ModLoader.GetMod("StarlightRiver") != null)
                {
                    npc.life = -1;
                    npc.active = false;
                    npc.HitEffect();
                }
            }
            catch
            {
            }
            _resetQuest = 0;
            List<string> potentialText = new List<string>();
            int angler = NPC.FindFirstNPC(NPCID.Angler);

            if (StoryProgression > STORY_FIND_CRAB_CARD)
            {
                potentialText.Add(AQText.RobsterChat(13).Value);
                potentialText.Add(AQText.RobsterChat(14).Value);
            }

            if (BirthdayParty.GenuineParty || BirthdayParty.ManualParty)
            {
                potentialText.Add(AQText.RobsterChat(12).Value);
            }

            if (Main.bloodMoon)
            {
                potentialText.Add(AQText.RobsterChat(7).Value);
                potentialText.Add(AQText.RobsterChat(8).Value);
                if (Main.hardMode)
                {
                    if (Main.moonPhase % 2 == 0)
                    {
                        potentialText.Add(AQText.RobsterChat(9).Value);
                    }
                    else
                    {
                        potentialText.Add(AQText.RobsterChat(10).Value);
                    }
                    if (angler != -1)
                    {
                        string text = AQText.RobsterChat(11).Value;
                        potentialText.Add(string.Format(text, Main.npc[angler].GivenName));
                    }
                }
            }

            if (Main.eclipse)
            {
                potentialText.Add(AQText.RobsterChat(5).Value);
                if (NPC.downedGolemBoss)
                {
                    potentialText.Add(AQText.RobsterChat(6).Value);
                }
            }

            if (GlimmerEvent.IsActive)
            {
                potentialText.Add(AQText.RobsterChat(2).Value);
                potentialText.Add(AQText.RobsterChat(3).Value);
                potentialText.Add(AQText.RobsterChat(4).Value);
            }

            potentialText.Add(AQText.RobsterChat(0).Value);
            potentialText.Add(AQText.RobsterChat(1).Value);
            return Language.GetTextValue(potentialText[Main.rand.Next(potentialText.Count)]);
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = _resetQuest != 0 ? AQText.RobsterQuitHunt().Value : AQText.RobsterHunt().Value;
        }

        public static int GetRobsterRandomQuestValue()
        {
            return Main.rand.Next(15 + (int)RandomsCompleted * 2, 50 + (int)RandomsCompleted * 2);
        }

        public static int[] RandomReward(Player player, int value)
        {
            int[] rewards = new int[ItemLoader.ItemCount];
            while (value > 0)
            {
                if (NPC.downedBoss1)
                {
                    if (Main.rand.NextBool())
                    {
                        switch (Main.rand.Next(6))
                        {
                            default:
                            if (value > 50)
                            {
                                rewards[ModContent.ItemType<UltimateEnergy>()]++;
                                value -= 50;
                            }
                            break;

                            case 1:
                            rewards[ModContent.ItemType<AquaticEnergy>()]++;
                            value -= 10;
                            break;

                            case 2:
                            rewards[ModContent.ItemType<AtmosphericEnergy>()]++;
                            value -= 10;
                            break;

                            case 3:
                            rewards[ModContent.ItemType<OrganicEnergy>()]++;
                            value -= 10;
                            break;

                            case 4:
                            rewards[ModContent.ItemType<DemonicEnergy>()]++;
                            value -= 15;
                            break;

                            case 5:
                            rewards[ModContent.ItemType<CosmicEnergy>()]++;
                            value -= 15;
                            break;
                        }
                    }
                    if (value > 45 && !player.HasItem(ModContent.ItemType<UltimateStarfruit>()) && Main.rand.NextBool((Main.hardMode || WorldDefeats.DownedStarite) ? 20 : 6))
                    {
                        rewards[ModContent.ItemType<MythicStarfruit>()]++;
                        value -= 45;
                    }
                }
                if (NPC.downedBoss3 && Main.rand.NextBool(Main.hardMode ? 15 : 4))
                {
                    rewards[ItemID.Bone]++;
                    value -= 8;
                }
                if (Main.hardMode)
                {
                    if (Main.rand.NextBool(!NPC.downedMechBoss1 && !NPC.downedMechBoss2 && !NPC.downedMechBoss3 ? 3 : 40))
                    {
                        if (value > 40)
                        {
                            rewards[ModContent.ItemType<OpposingPotion>()]++;
                            value -= 40;
                        }
                    }
                    if (NPC.downedMechBoss1)
                    {
                        if (value > 20 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 10 : 4))
                        {
                            rewards[ItemID.SoulofMight] += Main.rand.Next(3) + 1;
                            value -= 20;
                        }
                    }
                    if (NPC.downedMechBoss2)
                    {
                        if (value > 20 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 10 : 4))
                        {
                            rewards[ItemID.SoulofSight] += Main.rand.Next(3) + 1;
                            value -= 20;
                        }
                    }
                    if (NPC.downedMechBoss3)
                    {
                        if (value > 20 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 10 : 4))
                        {
                            rewards[ItemID.SoulofFright] += Main.rand.Next(3) + 1;
                            value -= 22;
                        }
                    }
                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    {
                        if (value > 40 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 15 : 6 : 3))
                        {
                            rewards[ItemID.TurtleShell]++;
                            value -= 40;
                        }
                    }
                    if (NPC.downedPlantBoss)
                    {
                        if (value > 18 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 8 : 2))
                        {
                            rewards[ItemID.Ectoplasm] += Main.rand.Next(6) + 2;
                            value -= 18;
                        }
                    }
                }

                value--;
            }
            return rewards;
        }

        public static int[] RandomReward_Mushroom(Player player, int value)
        {
            int[] rewards = new int[ItemLoader.ItemCount];
            while (value > 0)
            {
                if (NPC.downedPlantBoss)
                {
                    if (value > 30 && Main.rand.NextBool(4))
                    {
                        rewards[ItemID.ShroomiteBar] += Main.rand.Next(3) + 1;
                        value -= 30;
                    }
                }
                if (value > 5 && Main.rand.NextBool(6))
                {
                    switch (Main.rand.Next(5))
                    {
                        default:
                        rewards[ItemID.Mushroom]++;
                        break;
                        case 1:
                        rewards[ItemID.VileMushroom]++;
                        break;
                        case 2:
                        rewards[ItemID.ViciousMushroom]++;
                        break;
                        case 3:
                        rewards[ItemID.VilePowder] += 5;
                        break;
                        case 4:
                        rewards[ItemID.ViciousPowder] += 5;
                        break;
                    }
                    value -= 5;
                }
                if (value > 50 && Main.rand.NextBool(6))
                {
                    rewards[ItemID.MushroomDye]++;
                    value -= 10;
                }
                if (value > 20 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 10 : 6))
                {
                    rewards[ItemID.TealMushroom]++;
                    value -= 20;
                }
                if (value > 20 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 10 : 6))
                {
                    rewards[ItemID.GreenMushroom]++;
                    value -= 20;
                }
                if (value > 20 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 20 : 10 : 6))
                {
                    rewards[ItemID.MushroomStatue]++;
                    value -= 20;
                }
                if (value > 10 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 18 : 9 : 5))
                {
                    rewards[ItemID.MushroomGrassSeeds]++;
                    value -= 10;
                }
                if (value > 3 && Main.rand.NextBool(NPC.downedGolemBoss ? NPC.downedMoonlord ? 10 : 4 : 2))
                {
                    rewards[ItemID.GlowingMushroom] += Main.rand.Next(40) + 1;
                    value -= 3;
                }
                if (Main.rand.NextBool(Main.hardMode ? 15 : 3))
                {
                    switch (Main.rand.Next(3))
                    {
                        default:
                        rewards[ModContent.ItemType<ArgonMushroom>()]++;
                        break;
                        case 1:
                        rewards[ModContent.ItemType<KryptonMushroom>()]++;
                        break;
                        case 2:
                        rewards[ModContent.ItemType<XenonMushroom>()]++;
                        break;
                    }
                    value -= 5;
                }
            }
            return rewards;
        }

        public static int[] RandomReward_Bar(Player player, int value)
        {
            int[] rewards = new int[ItemLoader.ItemCount];
            bool copper = Main.rand.NextBool();
            bool iron = Main.rand.NextBool();
            bool silver = Main.rand.NextBool();
            bool gold = Main.rand.NextBool();
            bool cobalt = Main.rand.NextBool();
            bool mythril = Main.rand.NextBool();
            bool adamantite = Main.rand.NextBool();
            bool shroomite = Main.rand.NextBool();
            bool crimson = Main.rand.NextBool();
            while (value > 0)
            {
                if (NPC.downedMoonlord && value > 30)
                {
                    rewards[ItemID.LunarBar]++;
                }
                if (NPC.downedPlantBoss && value > 30 && Main.rand.NextBool(4))
                {
                    if (shroomite)
                    {
                        rewards[ItemID.ShroomiteBar]++;
                    }
                    else
                    {
                        rewards[ItemID.SpectreBar]++;
                    }
                    value -= 25;
                }
                if (NPC.downedMechBoss1 || NPC.downedMechBoss2 || NPC.downedMechBoss3)
                {
                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && value > 25 && Main.rand.NextBool(4))
                    {
                        rewards[ItemID.ChlorophyteBar]++;
                        value -= 25;
                    }
                    if (value > 20 && Main.rand.NextBool(3))
                    {
                        rewards[ItemID.HallowedBar]++;
                        value -= 20;
                    }
                }
                if (Main.hardMode)
                {
                    if (value > 10 && Main.rand.NextBool())
                    {
                        if (cobalt)
                        {
                            rewards[ItemID.CobaltBar]++;
                        }
                        else
                        {
                            rewards[ItemID.PalladiumBar]++;
                        }
                        value -= 10;
                    }
                    if (value > 10 && Main.rand.NextBool())
                    {
                        if (cobalt)
                        {
                            rewards[ItemID.MythrilBar]++;
                        }
                        else
                        {
                            rewards[ItemID.OrichalcumBar]++;
                        }
                        value -= 10;
                    }
                    if (value > 15 && Main.rand.NextBool())
                    {
                        if (cobalt)
                        {
                            rewards[ItemID.AdamantiteBar]++;
                        }
                        else
                        {
                            rewards[ItemID.TitaniumBar]++;
                        }
                        value -= 15;
                    }
                }
                if (NPC.downedBoss2)
                {
                    if (value > 10 && Main.rand.NextBool(4))
                    {
                        rewards[ItemID.HellstoneBar]++;
                        value -= 10;
                    }
                    if (value > 10 && Main.rand.NextBool())
                    {
                        rewards[ItemID.MeteoriteBar]++;
                        value -= 10;
                    }
                    if (value > 5 && Main.rand.NextBool())
                    {
                        if (crimson)
                        {
                            rewards[ItemID.ShadowScale]++;
                        }
                        else
                        {
                            rewards[ItemID.TissueSample]++;
                        }
                        value -= 5;
                    }
                }
                if (value > 5 && NPC.downedBoss1)
                {
                    if (crimson)
                    {
                        rewards[ItemID.DemoniteBar]++;
                    }
                    else
                    {
                        rewards[ItemID.CrimtaneBar]++;
                    }
                    value -= 5;
                }
                switch (Main.rand.Next(4))
                {
                    default:
                    if (copper)
                    {
                        rewards[ItemID.CopperBar]++;
                    }
                    else
                    {
                        rewards[ItemID.TinBar]++;
                    }
                    break;

                    case 1:
                    if (iron)
                    {
                        rewards[ItemID.IronBar]++;
                    }
                    else
                    {
                        rewards[ItemID.LeadBar]++;
                    }
                    break;

                    case 2:
                    if (silver)
                    {
                        rewards[ItemID.SilverBar]++;
                    }
                    else
                    {
                        rewards[ItemID.TungstenBar]++;
                    }
                    break;

                    case 3:
                    if (silver)
                    {
                        rewards[ItemID.GoldBar]++;
                    }
                    else
                    {
                        rewards[ItemID.PlatinumBar]++;
                    }
                    break;
                }
                value -= 2;
            }
            return rewards;
        }

        public static void RandomHuntReward(Player player)
        {
            switch (RandomsCompleted)
            {
                case 0:
                {
                    Main.npcChatText += " " + AQText.RobsterExtraReward(0);
                    player.QuickSpawnItem(ItemID.GoldenCrate);
                }
                break;

                case 5:
                {
                    Main.npcChatText += " " + AQText.RobsterExtraReward(1);
                    player.QuickSpawnItem(ItemID.IronCrate, 5);
                }
                break;

                case 10:
                {
                    Main.npcChatText += " " + AQText.RobsterExtraReward(1);
                    player.QuickSpawnItem(ItemID.IronCrate, 5);
                }
                break;

                case 15:
                case 20:
                {
                    Main.npcChatText += " " + AQText.RobsterExtraReward(1);
                    player.QuickSpawnItem(ItemID.GoldenCrate);
                    player.QuickSpawnItem(ItemID.JungleFishingCrate);
                    player.QuickSpawnItem(ItemID.DungeonFishingCrate);
                    player.QuickSpawnItem(ItemID.FloatingIslandFishingCrate);
                    if (WorldGen.crimson)
                    {
                        player.QuickSpawnItem(ItemID.CorruptFishingCrate);
                    }
                    else
                    {
                        player.QuickSpawnItem(ItemID.CrimsonFishingCrate);
                    }
                }
                break;
            }
            int value = GetRobsterRandomQuestValue();
            int[] rewards;
            switch (Main.rand.Next(3))
            {
                default:
                rewards = RandomReward(player, value);
                break;

                case 1:
                rewards = RandomReward_Mushroom(player, value);
                break;

                case 2:
                rewards = RandomReward_Bar(player, value);
                break;
            }
            for (int i = 0; i < rewards.Length; i++)
            {
                if (rewards[i] > 0)
                {
                    player.QuickSpawnItem(i, rewards[i]);
                }
            }
        }

        private static Rectangle getCheckRectangle(int x, int y)
        {
            return new Rectangle(x - 8, y - 8, 16, 16);
        }

        public static bool TryPlaceGoldenChalice(out byte npc)
        {
            for (int i = 0; i < 1000; i++)
            {
                npc = (byte)Main.rand.Next(Main.maxNPCs);
                if (Main.npc[npc].townNPC && !Main.npc[npc].homeless && Main.npc[npc].type != ModContent.NPCType<Robster>())
                {
                    int x = Main.npc[npc].homeTileX;
                    int y = Main.npc[npc].homeTileY;
                    var checkRectangle = getCheckRectangle(x, y);
                    for (int k = checkRectangle.X; k < checkRectangle.X + checkRectangle.Width; k++)
                    {
                        for (int l = checkRectangle.Y; l < checkRectangle.Y + checkRectangle.Height; l++)
                        {
                            int randomX = checkRectangle.X + Main.rand.Next(checkRectangle.Width);
                            int randomY = checkRectangle.Y + Main.rand.Next(checkRectangle.Height);
                            if (!Framing.GetTileSafely(randomX, randomY).active() && Framing.GetTileSafely(randomX, randomY + 1).active() && Main.tileSolidTop[Main.tile[randomX, randomY + 1].type])
                            {
                                WorldGen.PlaceTile(randomX, randomY, ModContent.TileType<Bottles>(), true, false, -1, 0);
                                if (Framing.GetTileSafely(randomX, randomY).type == ModContent.TileType<Bottles>())
                                {
                                    HuntInitialized = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            npc = 0;
            return false;
        }

        public static bool TryPlaceGoldenCandelabra(out byte npc)
        {
            for (int i = 0; i < 1000; i++)
            {
                npc = (byte)Main.rand.Next(Main.maxNPCs);
                if (Main.npc[npc].townNPC && !Main.npc[npc].homeless && Main.npc[npc].type != ModContent.NPCType<Robster>())
                {
                    int x = Main.npc[npc].homeTileX;
                    int y = Main.npc[npc].homeTileY;
                    var checkRectangle = getCheckRectangle(x, y);
                    for (int k = checkRectangle.X; k < checkRectangle.X + checkRectangle.Width; k++)
                    {
                        for (int l = checkRectangle.Y; l < checkRectangle.Y + checkRectangle.Height; l++)
                        {
                            int randomX = checkRectangle.X + Main.rand.Next(checkRectangle.Width);
                            int randomY = checkRectangle.Y + Main.rand.Next(checkRectangle.Height);
                            if (!Framing.GetTileSafely(randomX, randomY).active() && !Framing.GetTileSafely(randomX, randomY + 1).active() && !Framing.GetTileSafely(randomX + 1, randomY).active() && !Framing.GetTileSafely(randomX + 1, randomY + 1).active() && Framing.GetTileSafely(randomX, randomY + 2).active() && Main.tileSolidTop[Main.tile[randomX, randomY + 2].type] && Framing.GetTileSafely(randomX + 1, randomY + 2).active() && Main.tileSolidTop[Main.tile[randomX + 1, randomY + 2].type])
                            {
                                WorldGen.PlaceTile(randomX, randomY, ModContent.TileType<Tiles.JeweledCandelabra>(), true, false, -1, 0);
                                if (Framing.GetTileSafely(randomX, randomY).type == ModContent.TileType<Tiles.JeweledCandelabra>())
                                {
                                    HuntInitialized = true;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            npc = 0;
            return false;
        }

        public static void RandomizeHunt(int avoid = -1)
        {
            _resetQuest = 0;
            TargetNPC = 255;
            HuntInitialized = false;
            while (true)
            {
                RandomHunt = (byte)(Main.rand.Next(RANDOM_COUNT) + 1);
                if (RandomHunt != avoid)
                {
                    return;
                }
            }
        }

        private static int _bottlesTileTypeCache;
        private static int _jeweledCandelabraCache;

        private static void DestroyRandomHuntTiles()
        {
            _bottlesTileTypeCache = ModContent.TileType<Tiles.Bottles>();
            _jeweledCandelabraCache = ModContent.TileType<Tiles.JeweledCandelabra>();
            for (int i = 5; i < Main.maxTilesX - 5; i++)
            {
                for (int j = 5; j < Main.maxTilesY - 5; j++)
                {
                    var tile = Main.tile[i, j];
                    if (IsRandomHuntTile(tile))
                    {
                        tile.active(active: false);
                    }
                }
            }
        }

        private static bool IsRandomHuntTile(Tile tile)
        {
            if (tile.type == _bottlesTileTypeCache)
            {
                return tile.frameX == 0 && tile.frameY == 0;
            }
            return tile.type == _jeweledCandelabraCache;
        }

        private static int _recheckCount = 0;
        private const int MAX_RECHECKS = 10;

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                if (_recheckCount >= MAX_RECHECKS)
                {
                    Main.npcChatText = AQText.ModText("Common.RobsterCantDoRandomHunt").Value;
                    return;
                }
                if (_resetQuest == 2)
                {
                    Main.PlaySound(SoundID.Grab);
                    RandomizeHunt(RandomHunt);
                    DestroyRandomHuntTiles();
                }
                switch (StoryProgression)
                {
                    default:
                    {
                        Main.npcChatText = "My mind is going blank... I seem to have forgotten what I wanted to say.";
                        StoryProgression = STORY_FIND_SNOBSTER;
                    }
                    break;

                    case STORY_NONE:
                    {
                        if (RandomHunt == 0)
                        {
                            RandomizeHunt();
                        }
                        switch (RandomHunt)
                        {
                            default:
                            {
                                Main.npcChatText = "What";
                                Main.npcChatCornerItem = Main.rand.Next(ItemLoader.ItemCount);
                            }
                            break;

                            case RANDOM_JEWELED_CHALICE:
                            {
                                if (!HuntInitialized)
                                {
                                    if (TryPlaceGoldenChalice(out byte npc))
                                    {
                                        TargetNPC = npc;
                                    }
                                    else
                                    {
                                        RandomizeHunt(RandomHunt);
                                        _recheckCount++;
                                        OnChatButtonClicked(false, ref shop);
                                        _recheckCount = 0;
                                        return;
                                    }
                                }
                                int item = ModContent.ItemType<JeweledChalice>();
                                var player = Main.LocalPlayer;
                                if (player.ConsumeItem(item))
                                {
                                    Main.PlaySound(SoundID.Grab);
                                    Main.npcChatText = AQText.RobsterRandomHuntComplete(0).Value;
                                    RandomHuntReward(player);
                                    RandomsCompleted++;
                                    if (Main.myPlayer == player.whoAmI && AQConfigClient.Instance.ShowCompletedRobsterQuestsCount)
                                    {
                                        CombatText.NewText(player.getRect(), Color.OrangeRed, (int)RandomsCompleted);
                                    }
                                    RandomizeHunt(RandomHunt);
                                    DestroyRandomHuntTiles();
                                }
                                else
                                {
                                    if (_resetQuest == 1)
                                    {
                                        Main.npcChatText = AQText.RobsterQuitHuntQuestion().Value;
                                    }
                                    else
                                    {
                                        string text = AQText.RobsterRandomHunt(Main.rand).Value;
                                        Main.npcChatText = string.Format(text, Language.GetTextValue("Mods.AQMod.ItemName.JeweledChalice"));
                                        if (TargetNPC != 255)
                                        {
                                            text = " " + AQText.RobsterSawInSomeonesHouse();
                                            Main.npcChatText += string.Format(text, Main.npc[TargetNPC].GivenName);
                                        }
                                        Main.npcChatCornerItem = item;
                                    }
                                    _resetQuest++;
                                }
                            }
                            break;

                            case RANDOM_JEWELED_CANDELABRA:
                            {
                                if (!HuntInitialized)
                                {
                                    if (TryPlaceGoldenCandelabra(out byte npc))
                                    {
                                        TargetNPC = npc;
                                    }
                                    else
                                    {
                                        RandomizeHunt(RandomHunt);
                                        _recheckCount++;
                                        OnChatButtonClicked(false, ref shop);
                                        _recheckCount = 0;
                                        return;
                                    }
                                }
                                int item = ModContent.ItemType<Items.Misc.QuestRobster.JeweledCandelabra>();
                                var player = Main.LocalPlayer;
                                if (player.ConsumeItem(item))
                                {
                                    Main.PlaySound(SoundID.Grab);
                                    Main.npcChatText = AQText.RobsterRandomHuntComplete(0).Value;
                                    RandomHuntReward(player);
                                    RandomsCompleted++;
                                    if (Main.myPlayer == player.whoAmI && AQConfigClient.Instance.ShowCompletedRobsterQuestsCount)
                                    {
                                        CombatText.NewText(player.getRect(), Color.OrangeRed, (int)RandomsCompleted);
                                    }
                                    RandomizeHunt(RandomHunt);
                                    DestroyRandomHuntTiles();
                                }
                                else
                                {
                                    if (_resetQuest == 1)
                                    {
                                        Main.npcChatText = AQText.RobsterQuitHuntQuestion().Value;
                                    }
                                    else
                                    {
                                        string text = AQText.RobsterRandomHunt(Main.rand).Value;
                                        Main.npcChatText = string.Format(text, Language.GetTextValue("Mods.AQMod.ItemName.JeweledCandelabra"));
                                        if (TargetNPC != 255)
                                        {
                                            text = " " + AQText.RobsterSawInSomeonesHouse();
                                            Main.npcChatText += string.Format(text, Main.npc[TargetNPC].GivenName);
                                        }
                                        Main.npcChatCornerItem = item;
                                    }
                                    _resetQuest++;
                                }
                            }
                            break;
                        }
                    }
                    break;

                    case STORY_FIND_SNOBSTER:
                    {
                        var player = Main.LocalPlayer;
                        if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<Snobster>()))
                        {
                            Main.PlaySound(SoundID.Grab);
                            Main.npcChatText = AQText.RobsterStoryHuntComplete(0).Value + "\n" + AQText.PressHuntToContinue().Value;
                            Main.npcChatCornerItem = ModContent.ItemType<Snobster>();
                            player.QuickSpawnItem(ModContent.ItemType<Items.Placeable.FishingCraftingStation>());
                            StoryProgression = STORY_FIND_CRAB_CARD;
                            return;
                        }
                        else
                        {
                            Main.npcChatText = AQText.RobsterStoryHunt(0).Value;
                            Main.npcChatCornerItem = 0;
                        }
                    }
                    break;

                    case STORY_FIND_CRAB_CARD:
                    {
                        var player = Main.LocalPlayer;
                        if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<CrabCard>()))
                        {
                            Main.PlaySound(SoundID.Grab);
                            int npc = NPC.FindFirstNPC(ModContent.NPCType<Robster>());
                            string text = AQText.RobsterStoryHuntComplete(1).Value;
                            Main.npcChatText = string.Format(text, Main.npc[npc].GivenName);
                            Main.npc[npc].GivenName = "Robster";
                            StoryProgression = STORY_NONE;
                            player.QuickSpawnItem(ModContent.ItemType<BlurryDiscountCard>());
                            RandomizeHunt();
                            return;
                        }
                        else
                        {
                            Main.npcChatText = AQText.RobsterStoryHunt(1).Value;
                            Main.npcChatCornerItem = ModContent.ItemType<CrabCard>();
                        }
                    }
                    break;
                }
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<JerryClawFlail>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CinnabarBow>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Bubbler>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CrabBarb>());
            nextSlot++;
            if (Main.hardMode)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Crabsol>());
                nextSlot++;
                if (NPC.downedPirates)
                {
                    if (Main.moonPhase <= 2)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.DiscountCard);
                        nextSlot++;
                    }
                    else if (Main.moonPhase <= 5)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.LuckyCoin);
                        nextSlot++;
                    }
                    else
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.GreedyRing);
                        nextSlot++;
                    }
                    shop.item[nextSlot].SetDefaults(ItemID.Cutlass);
                    nextSlot++;
                    if (NPC.downedPlantBoss)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.CoinGun);
                        nextSlot++;
                    }
                    switch (Main.moonPhase)
                    {
                        default:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenChair);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenTable);
                        nextSlot++;
                        break;
                        case 1:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenWorkbench);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenDoor);
                        nextSlot++;
                        break;
                        case 2:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenBed);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenSofa);
                        nextSlot++;
                        break;
                        case 3:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenChest);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenDresser);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        break;
                        case 4:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenToilet);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenBathtub);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenSink);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);

                        nextSlot++;
                        break;
                        case 5:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenCandle);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenLamp);
                        nextSlot++;
                        break;
                        case 6:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenClock);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenPiano);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenBookcase);
                        nextSlot++;
                        break;
                        case 7:
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenChandelier);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenLantern);
                        nextSlot++;
                        break;
                    }
                    if (Main.moonPhase % 2 == 0)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenPlatform);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 2);
                        nextSlot++;
                    }
                    if (AQMod.AprilFools) // graveyard
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.RichGravestone1);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.RichGravestone2);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.RichGravestone3);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.RichGravestone4);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.RichGravestone5);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 10);
                        nextSlot++;
                    }
                }
            }
            if (NPC.downedBoss3 && Main.moonPhase % 2 == 1)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 15);
                nextSlot++;
            }
            if (StoryProgression > STORY_FIND_CRAB_CARD)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Placeable.FishingCraftingStation>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                nextSlot++;
            }
            else if (StoryProgression > STORY_FIND_SNOBSTER)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Placeable.FishingCraftingStation>());
                nextSlot++;
            }
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return true;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 8f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 10;
            randExtraCooldown = 5;
        }

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = ModContent.GetTexture("AQMod/" + AQTextureAssets.None);
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 20;
            itemHeight = 20;
        }
    }
}