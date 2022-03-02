using AQMod.Assets;
using AQMod.Common;
using AQMod.Content.World.Events;
using AQMod.Items.Misc;
using AQMod.Items.Misc.ExporterQuest;
using AQMod.Items.Placeable.CraftingStations;
using AQMod.Items.Placeable.Furniture;
using AQMod.Items.Potions;
using AQMod.Items.Tools;
using AQMod.Items.Tools.MagicPowders;
using AQMod.Items.Weapons.Melee;
using AQMod.Localization;
using AQMod.Sounds;
using AQMod.Tiles.ExporterQuest;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.NPCs.Friendly
{
    [AutoloadHead()]
    public class Robster : ModNPC
    {
        public abstract class ThieveryQuest
        {
            public readonly string Key;

            public byte type;
            public Point location;

            protected ThieveryQuest(string key)
            {
                Key = key;
            }

            public virtual void OnActivate()
            {

            }
            public abstract int GetQuestItem();
            public abstract string QuestChat();

            public virtual bool IsHuntComplete(Player player)
            {
                return player.HasItem(GetQuestItem());
            }

            public virtual bool CanStart()
            {
                return true;
            }

            public virtual void Update()
            {

            }

            public virtual void NetSend(BinaryWriter writer)
            {

            }

            public virtual void NetRecieve(BinaryReader reader)
            {

            }

            protected void RemoveQuestTiles()
            {
                for (int i = 5; i < Main.maxTilesX - 5; i++)
                {
                    for (int j = 5; j < Main.maxTilesY - 5; j++)
                    {
                        if (Main.tile[i, j] != null && AQTile.Sets.ExporterQuestFurniture.Contains(Main.tile[i, j].type))
                            Main.tile[i, j].active(active: false);
                    }
                }
            }
            public virtual void EndHunt()
            {
                RemoveQuestTiles();
            }

            protected void RegularQuestRewards(Player player)
            {
                player.QuickSpawnItem(ModContent.ItemType<OverworldPalette>());
                if (Main.rand.NextBool())
                    player.QuickSpawnItem(ModContent.ItemType<CavernPalette>());
                if (Main.rand.NextBool(10))
                    player.QuickSpawnItem(ItemID.GoldenCrate);
            }
            public virtual void CompleteHunt(Player player)
            {
                RegularQuestRewards(player);
            }

            public virtual TagCompound Save()
            {
                return null;
            }

            public virtual void Load(TagCompound tag)
            {
            }
        }
        public sealed class StealTileQuest : ThieveryQuest
        {
            public const byte Chalice = 0;
            public const byte Candelabra = 1;
            public const byte MaxTypes = 2;

            public StealTileQuest(string key) : base(key)
            {
            }

            public override int GetQuestItem()
            {
                switch (type)
                {
                    default:
                        return ModContent.ItemType<JeweledChalice>();
                    case Candelabra:
                        return ModContent.ItemType<JeweledCandelabra>();
                }
            }
            public override string QuestChat()
            {
                switch (type)
                {
                    default:
                        return "Mods.AQMod.Exporter.Quest.JeweledChalice";
                    case Candelabra:
                        return "Mods.AQMod.Exporter.Quest.JeweledCandelabra";
                }
            }

            private bool PlaceTile(int x, int y)
            {
                if (type == Candelabra)
                {
                    if (!Framing.GetTileSafely(x, y).active() && !Framing.GetTileSafely(x, y + 1).active() && !Framing.GetTileSafely(x + 1, y).active() && !Framing.GetTileSafely(x + 1, y + 1).active() && Framing.GetTileSafely(x, y + 2).active() && Main.tileSolidTop[Main.tile[x, y + 2].type] && Framing.GetTileSafely(x + 1, y + 2).active() && Main.tileSolidTop[Main.tile[x + 1, y + 2].type])
                    {
                        WorldGen.PlaceTile(x, y, ModContent.TileType<JeweledCandelabraTile>(), true, false, -1, 0);
                        if (Framing.GetTileSafely(x, y).type == ModContent.TileType<JeweledCandelabraTile>())
                        {
                            NetMessage.SendTileSquare(-1, x - 2, y - 2, 4);
                            return true;
                        }
                    }
                }
                else
                {
                    if (!Framing.GetTileSafely(x, y).active() && Framing.GetTileSafely(x, y + 1).active() && Main.tileSolidTop[Main.tile[x, y + 1].type])
                    {
                        WorldGen.PlaceTile(x, y, ModContent.TileType<JeweledChaliceTile>(), true, false, -1, 0);
                        if (Framing.GetTileSafely(x, y).type == ModContent.TileType<JeweledChaliceTile>())
                        {
                            NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
                            return true;
                        }
                    }
                }
                return false;
            }
            private bool TryPlaceQuestTile()
            {
                List<int> townNPCs = new List<int>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].townNPC && !Main.npc[i].homeless && Main.npc[i].type != ModContent.NPCType<Robster>())
                    {
                        townNPCs.Add(i);
                    }
                }
                if (townNPCs.Count <= 0)
                {
                    return false;
                }
                for (int i = 0; i < 1000; i++)
                {
                    byte npc = (byte)townNPCs[Main.rand.Next(townNPCs.Count)];
                    int x = Main.npc[npc].homeTileX;
                    int y = Main.npc[npc].homeTileY;
                    var checkRectangle = new Rectangle(x - 8, y - 8, 16, 16);
                    for (int k = checkRectangle.X; k < checkRectangle.X + checkRectangle.Width; k++)
                    {
                        for (int l = checkRectangle.Y; l < checkRectangle.Y + checkRectangle.Height; l++)
                        {
                            int randomX = checkRectangle.X + Main.rand.Next(checkRectangle.Width);
                            int randomY = checkRectangle.Y + Main.rand.Next(checkRectangle.Height);
                            if (PlaceTile(randomX, randomY))
                            {
                                TargetNPC = npc;
                                location = new Point(x, y);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            public override void OnActivate()
            {
                type = (byte)Main.rand.Next(MaxTypes);
                location = new Point(-1, -1);
                if (!TryPlaceQuestTile())
                {
                    ActiveQuest = null;
                }
            }

            public override void CompleteHunt(Player player)
            {
                player.ConsumeItem(GetQuestItem());
                base.CompleteHunt(player);
            }
        }

        public static List<ThieveryQuest> RegisteredQuests { get; private set; }
        public static int QuestsCompleted { get; internal set; }
        public static ThieveryQuest ActiveQuest { get; internal set; }
        public static int TargetNPC { get; internal set; }

        public static bool huntComplete;
        public static byte resetQuestButton;

        public static Color JeweledTileMapColor => new Color(255, 185, 25, 255);
        public static Color RobsterBroadcastMessageColor => new Color(255, 215, 105, 255);

        public byte NetTick;

        internal static void Initialize()
        {
            resetQuestButton = 0;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 25;
            NPCID.Sets.ExtraFramesCount[npc.type] = 9;
            NPCID.Sets.AttackFrameCount[npc.type] = 4;
            NPCID.Sets.DangerDetectRange[npc.type] = 400;
            NPCID.Sets.AttackType[npc.type] = 3; // -1 is none? 0 is shoot, 1 is magic shoot?, 2 is dryad aura, 3 is melee
            NPCID.Sets.AttackTime[npc.type] = 10;
            NPCID.Sets.AttackAverageChance[npc.type] = 10;
            NPCID.Sets.HatOffsetY[npc.type] = 8;

            Initialize();
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

        public override void AI()
        {
            npc.breath = 200;
            if (ActiveQuest == null)
            {
                StartRandomHunt();
            }
            ActiveQuest?.Update();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetTick++;
                if (NetTick > 240)
                {
                    NetTick = 0;
                    if (ActiveQuest == null)
                    {
                        StartRandomHunt();
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {

                    }
                    else
                    {
                    }
                }
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return WorldDefeats.DownedCrabson;
        }

        public override string TownNPCName()
        {
            switch (WorldGen.genRand.Next(12))
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
                    return "Robson";
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
                case 11:
                    return "Robster";
            }
        }

        public override string GetChat()
        {
            try
            {
                if (ModLoader.GetMod("StarlightRiver") != null)
                {
                    npc.life = -1;
                    npc.HitEffect();
                    npc.active = false; // 😎
                }
            }
            catch
            {
            }

            resetQuestButton = 0;
            huntComplete = false;
            if (ActiveQuest != null)
            {
                huntComplete = ActiveQuest.IsHuntComplete(Main.LocalPlayer);
            }
            var potentialText = new List<string>();
            int angler = NPC.FindFirstNPC(NPCID.Angler);

            if (BirthdayParty.GenuineParty || BirthdayParty.ManualParty)
                potentialText.Add(AQText.RobsterChat(12).Value);

            if (Main.bloodMoon)
            {
                potentialText.Add(AQText.RobsterChat(7).Value);
                potentialText.Add(AQText.RobsterChat(8).Value);
                if (Main.hardMode)
                {
                    if (Main.moonPhase % 2 == 0)
                        potentialText.Add(AQText.RobsterChat(9).Value);
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
                    potentialText.Add(AQText.RobsterChat(6).Value);
            }

            if (Glimmer.IsGlimmerEventCurrentlyActive())
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
            if (resetQuestButton != 0 && resetQuestButton != 255)
            {
                button2 = Language.GetTextValue("Mods.AQMod.Exporter.Quests.UIButtonQuit");
            }
            else
            {
                button2 = huntComplete ? Language.GetTextValue("Mods.AQMod.Exporter.Quests.UIButtonComplete") : Language.GetTextValue("Mods.AQMod.Exporter.Quests.UIButtonQuest");
            }
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else
            {
                if (ActiveQuest == null)
                {
                    AQSound.Play(SoundType.Custom, "Error", 0.6f);
                    return;
                }
                if (resetQuestButton == 2)
                {
                    resetQuestButton = 0;
                    ActiveQuest?.EndHunt();
                    StartRandomHunt();
                    Main.PlaySound(SoundID.Grab);
                    OnChatButtonClicked(firstButton, ref shop);
                    return;
                }
                if (resetQuestButton == 1)
                {
                    Main.npcChatText = Language.GetTextValue("Mods.AQMod.Exporter.Quests.QuitHuntQuestion");
                }
                else
                {
                    var plr = Main.LocalPlayer;
                    if (ActiveQuest?.IsHuntComplete(plr) == true)
                    {
                        Main.PlaySound(SoundID.Grab);
                        ActiveQuest.CompleteHunt(Main.LocalPlayer);
                        Main.npcChatText = Language.GetTextValue("Mods.AQMod.Exporter.Quests.Complete." + Main.rand.Next(5));
                        huntComplete = false;
                        StartRandomHunt();
                        resetQuestButton = 255;
                        return;
                    }
                    else
                    {
                        string text = ActiveQuest.QuestChat();
                        int questItem = ActiveQuest.GetQuestItem();
                        Main.npcChatText = Language.GetTextValue(text, Lang.GetItemName(questItem));
                        Main.npcChatCornerItem = questItem;
                        if (TargetNPC != -1)
                        {
                            text = " " + Language.GetTextValue("Mods.AQMod.Exporter.Quests.AtTownNPCHouse");
                            Main.npcChatText += string.Format(text, Main.npc[TargetNPC].GivenName);
                        }
                    }
                }
                resetQuestButton++;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            if (Main.hardMode)
            {
                if (NPC.downedPirates)
                {
                    shop.item[nextSlot].SetDefaults(ItemID.DiscountCard);
                    nextSlot++;
                }
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpoilsPotion>());
                nextSlot++;
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<GoldPowder>());
                nextSlot++;
            }
            else
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<BlurryDiscountCard>());
                nextSlot++;
            }
            if (NPC.downedBoss3 && Main.moonPhase % 2 == 1)
            {
                shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
                shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 15);
                nextSlot++;
            }
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<FishingCraftingStation>());
            nextSlot++;
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<CrabClock>());
            nextSlot++;
        }

        public override bool PreNPCLoot()
        {
            return true;
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
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

        public override void DrawTownAttackSwing(ref Texture2D item, ref int itemSize, ref float scale, ref Vector2 offset)
        {
            item = TextureGrabber.GetItem(ModContent.ItemType<Crabsol>());
            itemSize = 40;
            scale = 0.5f;
            offset = new Vector2(0f, 0f);
        }

        public override void TownNPCAttackSwing(ref int itemWidth, ref int itemHeight)
        {
            itemWidth = 30;
            itemHeight = 30;
        }

        internal static void Load()
        {
            RegisteredQuests = new List<ThieveryQuest>()
            {
                new StealTileQuest("TileStealHunt"),
            };
            Initalize();
        }

        internal static void Unload()
        {
            RegisteredQuests?.Clear();
            RegisteredQuests = null;
        }

        internal static void Initalize()
        {
            ActiveQuest = null;
            QuestsCompleted = 0;
            TargetNPC = -1;
        }

        internal static void Save(TagCompound tag)
        {
            tag["Exporter_QuestsCompleted"] = QuestsCompleted;
            if (ActiveQuest == null)
            {
                return;
            }
            tag["Exporter_ActiveHunt"] = ActiveQuest.Key;
            tag["Exporter_ActiveHunt_Type"] = ActiveQuest.type;
            tag["Exporter_ActiveHunt_Location_X"] = ActiveQuest.location.X;
            tag["Exporter_ActiveHunt_Location_Y"] = ActiveQuest.location.Y;
            if (TargetNPC >= 0)
            {
                tag["Exporter_TargetNPCType"] = Main.npc[TargetNPC].type;
            }
            var huntData = ActiveQuest.Save();
            if (huntData != null)
            {
                tag["Exporter_ActiveHunt_Save"] = huntData;
            }
        }

        internal static void Load(TagCompound tag)
        {
            QuestsCompleted = tag.GetInt("Exporter_QuestsCompleted");
            if (!tag.ContainsKey("Exporter_ActiveHunt"))
            {
                return;
            }
            string key = tag.GetString("Exporter_ActiveHunt");
            ActiveQuest = RegisteredQuests.Find((h) => h.Key == key);
            if (ActiveQuest == default(ThieveryQuest))
            {
                return;
            }
            ActiveQuest.type = tag.GetByte("Exporter_ActiveHunt_Type");
            ActiveQuest.location.X = tag.GetInt("Exporter_ActiveHunt_Location_X");
            ActiveQuest.location.Y = tag.GetInt("Exporter_ActiveHunt_Location_Y");
            if (tag.ContainsKey("Exporter_TargetNPCType"))
            {
                int type = tag.GetInt("Exporter_TargetNPCType");
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == type)
                    {
                        TargetNPC = i;
                        break;
                    }
                }
            }
            if (tag.ContainsKey("Exporter_ActiveHunt_Save"))
            {
                ActiveQuest.Load(tag.GetCompound("Exporter_ActiveHunt_Save"));
            }
        }

        internal static void StartRandomHunt()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetHelper.Request(NetHelper.PacketType.RequestExporterQuestRandomize);
                return;
            }
            ActiveQuest?.EndHunt();
            List<ThieveryQuest> valid = new List<ThieveryQuest>();
            for (int i = 0; i < RegisteredQuests.Count; i++)
            {
                if (RegisteredQuests[i].CanStart())
                {
                    valid.Add(RegisteredQuests[i]);
                }
            }
            if (valid.Count <= 0)
            {
                return;
            }
            ActiveQuest = valid[valid.Count == 1 ? 0 : Main.rand.Next(valid.Count)];
            ActiveQuest.OnActivate();
        }

        public static bool? CheckTileBreakSights(int tileX, int tileY, bool alsoResetQuest = true)
        {
            var worldPosition = new Vector2(tileX * 16f + 8f, tileY * 16f + 8f);
            var closestPlayer = Main.player[Player.FindClosest(new Vector2(tileX * 16f, tileY * 16f), 16, 16)];
            float seeingDistance = 1000f;
            if (closestPlayer.dead || Vector2.Distance(closestPlayer.Center, worldPosition) > seeingDistance)
            {
                if (alsoResetQuest)
                {
                    AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterFailAccident", RobsterBroadcastMessageColor);
                    ActiveQuest?.EndHunt();
                    ActiveQuest = null;
                }
                return false;
            }
            if (closestPlayer.invis)
                seeingDistance /= 5f;
            bool foundOut = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                float npcSeeingDistance = seeingDistance;
                int directionToTile = worldPosition.X < Main.npc[i].position.X + Main.npc[i].width / 2f ? -1 : 1;
                if (-Main.npc[i].direction != directionToTile)
                    npcSeeingDistance /= 2;
                if (Main.npc[i].active && Main.npc[i].townNPC && Main.npc[i].type != ModContent.NPCType<Robster>() && Vector2.Distance(Main.npc[i].Center, worldPosition) < npcSeeingDistance && Collision.CanHitLine(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, new Vector2(tileX * 16f, tileY * 16f), 16, 16))
                {
                    foundOut = true;
                    Main.npc[i].ai[0] = 0f;
                    Main.npc[i].ai[1] = 1500f;
                    Main.npc[i].direction = directionToTile;
                    Main.npc[i].spriteDirection = directionToTile;
                    Main.npc[i].netUpdate = true;


                    int[] emoteChoices = new int[] { EmoteID.WeatherLightning, EmoteID.ItemSword, EmoteID.MiscFire };


                    int choice = Main.rand.Next(emoteChoices.Length);
                    EmoteBubble.NewBubble(emoteChoices[choice], new WorldUIAnchor(Main.npc[i]), 480);
                }
            }
            if (foundOut && alsoResetQuest)
            {
                AQMod.BroadcastMessage("Mods.AQMod.Common.RobsterFail", Robster.RobsterBroadcastMessageColor);
                ActiveQuest?.EndHunt();
                ActiveQuest = null;
            }
            return foundOut;
        }
    }
}