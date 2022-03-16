using AQMod.Common.CrossMod;
using AQMod.Common.WorldGeneration;
using AQMod.Content.Players;
using AQMod.Content.World.Biomes;
using AQMod.Content.World.Events;
using AQMod.Content.World.Generation;
using AQMod.Items;
using AQMod.Localization;
using AQMod.NPCs.Friendly;
using AQMod.NPCs.Monsters;
using AQMod.Tiles;
using AQMod.Tiles.Furniture;
using AQMod.Tiles.Nature;
using AQMod.Tiles.Nature.CrabCrevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Cinematics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Common.Utilities.Debugging
{
    internal class NCallCommand : ModCommand
    {
        public static string DebugFolderPath => Main.SavePath + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar + "Cache" + Path.DirectorySeparatorChar + "AQMod";

        public override bool Autoload(ref string name)
        {
            return ModContent.GetInstance<AQConfigServer>().debugCommand;
        }

        public override string Command => "ncall";

        public override CommandType Type => CommandType.World;

        private static string[] lastCall = null;

        private static int mX => Main.MouseWorld.ToTileCoordinates().X; // these are highly unoptimal because I am lazy + these are test commands so idc
        private static int mY => Main.MouseWorld.ToTileCoordinates().Y;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("Please add another value to the command");
                return;
            }
            string callType = args[0];
            switch (callType)
            {
                default:
                    caller.Reply("Command doesn't exist.");
                    break;

                case "robsternet":
                    {
                        ((Robster)Main.npc[NPC.FindFirstNPC(ModContent.NPCType<Robster>())].modNPC).NPCCheck = 241;
                    }
                    break;

                case "questtile":
                    {
                        caller.Reply(Robster.TryPlaceQuestTile(mX, mY).ToString());
                    }
                    break;

                case "clr":
                    {
                        bool assignedColor = true;
                        Color color = new Color();
                        switch (args[1])
                        {
                            case "Violet":
                                color = Color.Violet;
                                break;

                            case "MediumPurple":
                                color = Color.MediumPurple;
                                break;

                            default:
                                assignedColor = false;
                                break;
                        }
                        if (!assignedColor)
                            color = (Color)typeof(Color).GetProperty(args[1], BindingFlags.Public | BindingFlags.Static).GetGetMethod(nonPublic: false).Invoke(null, null);
                        caller.Reply(color.ToString());
                    }
                    break;

                case "polaritiesinfractaldimension":
                    {
                        caller.Reply(PolaritiesModSupport.InFractalDimension().ToString());
                    }
                    break;

                case "mana":
                    {
                        caller.Player.HeldItem.mana = int.Parse(args[1]);
                    }
                    break;

                case "tframes":
                    {
                        caller.Reply(Main.tile[mX, mY].frameX.ToString());
                        caller.Reply(Main.tile[mX, mY].frameY.ToString(), Color.Yellow);
                    }
                    break;

                case "cursordye":
                    caller.Player.GetModPlayer<PlayerCursorDyes>().cursorDye = byte.Parse(args[1]);
                    break;

                case "genccmushroom":
                    NobleMushroomsNew.Place(mX, mY);
                    break;

                case "pvntinv":
                    Main.invasionDelay = 0;
                    Main.invasionType = 0;
                    break;

                case "i":
                    if (caller.Player.dead)
                    {
                        caller.Player.respawnTimer = 2;
                    }
                    break;

                case "genccship":
                    {
                        BiomeCrabCrevice.GenPirateShip(mX, mY);
                        BiomeCrabCrevice.ReplacePlatformsViaGenList();
                    }
                    break;

                case "genccsand":
                    {
                        BiomeCrabCrevice.CreateSandAreaForCrevice(mX, mY);
                    }
                    break;

                case "gencc":
                    {
                        BiomeCrabCrevice.GenerateCreviceCave(mX, mY, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
                    }
                    break;

                case "gen":
                    {
                        switch (args[1].ToLower())
                        {
                            case "buriedchests":
                                CustomChestLoot.GenerateDirtChests(null);
                                break;
                            case "waterfixer":
                                BabyPoolKiller.PassFix1TileHighWater(null);
                                break;
                        }
                    }
                    break;

                case "waterfix":
                    {
                        BabyPoolKiller.ApplyFix(mX, mY);
                    }
                    break;

                case "buriedchest":
                    {
                        CustomChestLoot.PlaceBuriedChest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y, out int chestID, caller.Player.HeldItem.createTile, WallID.Dirt, caller.Player.HeldItem.placeStyle);
                        caller.Reply(chestID.ToString());
                    }
                    break;

                case "call":
                    {
                        var argList = new string[args.Length - 1];
                        for (int i = 1; i < args.Length; i++)
                        {
                            argList[i - 1] = args[i];
                        }
                        caller.Reply(AQMod.GetInstance().Call(argList).ToString());
                    }
                    break;

                case "place":
                    {
                        int createTile = 0;
                        if (int.TryParse(args[1], out createTile))
                        {
                            caller.Player.HeldItem.createTile = createTile;
                        }
                        else
                        {
                            bool writeHelp = false;
                            try
                            {
                                bool findTerraria = true;
                                if (args.Length > 2)
                                {
                                    if (args[1] != "Terraria")
                                    {
                                        string modName = AQStringCodes.DecodeModName(args[1]);
                                        caller.Reply("Finding tile from mod: " + modName);
                                        var modType = ModLoader.GetMod(modName);
                                        if (modType == null)
                                        {
                                            caller.Reply("Mod doesn't exist.");
                                        }
                                        else
                                        {
                                            caller.Player.HeldItem.createTile = modType.TileType(args[2]);
                                            findTerraria = false;
                                        }
                                    }
                                }
                                if (findTerraria)
                                {
                                    caller.Player.HeldItem.createTile = ItemID.Search.GetId(args[1]);
                                }
                            }
                            catch
                            {
                                writeHelp = true;
                            }
                            if (writeHelp)
                            {
                                caller.Reply("You can find a tile by name by writing /ncall place {MOD} {NAME}");
                                caller.Reply("You can write no mod to try to find tiles from vanilla");
                            }
                        }
                    }
                    break;

                case "consumable":
                    {
                        caller.Player.HeldItem.consumable = !caller.Player.HeldItem.consumable;
                    }
                    break;

                case "vampirism":
                    {
                        caller.Player.GetModPlayer<VampirismPlayer>().Vampirism = ushort.Parse(args[1]);
                    }
                    break;

                case "wikiitem":
                    {
                        AutomaticWikiTestStuff.basicwikipage(AQMod.GetInstance().GetItem(args[1]));
                    }
                    break;

                case "chestloot":
                    {
                        CustomChestLoot.AddLoot(Chest.FindChest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y));
                    }
                    break;

                case "nametagitem":
                    {
                        string name = "";
                        for (int i = 1; i < args.Length; i++)
                        {
                            if (i > 1)
                                name += " ";
                            name += args[i];
                        }
                        caller.Player.HeldItem.GetGlobalItem<AQItem>().NameTag = name;
                    }
                    break;

                case "demonsiegei":
                    caller.Reply("x: " + DemonSiege.X);
                    caller.Reply("y: " + DemonSiege.Y);
                    caller.Reply("plr: " + DemonSiege.PlayerActivator + " (" + Main.player[DemonSiege.PlayerActivator] + ")");
                    if (DemonSiege.BaseItem != null)
                        caller.Reply("item: " + DemonSiege.BaseItem.type + " (" + Lang.GetItemName(DemonSiege.BaseItem.type) + ")");
                    break;

                case "glimmerxy":
                    {
                        caller.Reply("x: " + Glimmer.tileX + ", y: " + Glimmer.tileY);
                    }
                    break;

                case "initday":
                    {
                        int count = 1;
                        if (args.Length > 1)
                        {
                            count = int.Parse(args[1]);
                        }
                        Main.NewText(count);
                        if (count == -1)
                        {
                            Main.dayTime = false;
                            Main.time = Main.nightLength;
                            break;
                        }
                        var method = typeof(Main).GetMethod("UpdateTime", BindingFlags.NonPublic | BindingFlags.Static);
                        for (int i = 0; i < count; i++)
                        {
                            Main.dayTime = false;
                            Main.time = 60;
                            method.Invoke(null, null);

                            Main.time = Main.nightLength;
                            Main.fastForwardTime = false;
                            AQWorld.DayrateIncrease = 0;
                            method.Invoke(null, null);
                        }
                    }
                    break;

                case "initnight":
                    {
                        int count = 1;
                        if (args.Length > 1)
                        {
                            count = int.Parse(args[1]);
                        }
                        Main.NewText(count);
                        if (count == -1)
                        {
                            Main.dayTime = true;
                            Main.time = Main.dayLength;
                            break;
                        }
                        var method = typeof(Main).GetMethod("UpdateTime", BindingFlags.NonPublic | BindingFlags.Static);
                        for (int i = 0; i < count; i++)
                        {
                            Main.dayTime = true;
                            Main.time = 60;
                            Main.bloodMoon = false;
                            Main.stopMoonEvent();
                            method.Invoke(null, null);

                            Main.time = Main.dayLength;
                            Main.fastForwardTime = false;
                            AQWorld.DayrateIncrease = 0;
                            method.Invoke(null, null);
                        }
                    }
                    break;

                case "april":
                case "fools":
                case "aprilfools":
                    AprilFoolsJoke.Active = true;
                    break;

                case "downedglimmer":
                case "downedstars":
                    WorldDefeats.DownedGlimmer = !WorldDefeats.DownedGlimmer;
                    break;

                case "downedcrabseason":
                case "downedcrabs":
                    WorldDefeats.DownedCrabSeason = !WorldDefeats.DownedCrabSeason;
                    break;

                case "downedcrabson":
                case "downedcrab":
                    WorldDefeats.DownedCrabson = !WorldDefeats.DownedCrabson;
                    break;

                case "downedomegastarite":
                case "downedomega":
                case "downedstarite":
                    WorldDefeats.DownedStarite = !WorldDefeats.DownedStarite;
                    break;

                case "itemperature":
                    {
                        sbyte newTemp = sbyte.Parse(args[1]);
                        caller.Reply("changing temperature to: " + newTemp);
                        caller.Player.GetModPlayer<AQPlayer>().InflictTemperature(newTemp);
                    }
                    break;

                case "settemperature":
                    {
                        sbyte newTemp = sbyte.Parse(args[1]);
                        caller.Reply("changing temperature to: " + newTemp);
                        caller.Player.GetModPlayer<AQPlayer>().temperature = newTemp;
                    }
                    break;

                case "meteor":
                    {
                        var b = Main.MouseWorld.ToTileCoordinates();
                        if (args.Length > 7)
                        {
                            GaleStreams.CrashMeteor(b.X, b.Y, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), bool.Parse(args[6]), ushort.Parse(args[7]));
                        }
                        else if (args.Length > 6)
                        {
                            GaleStreams.CrashMeteor(b.X, b.Y, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), bool.Parse(args[6]));
                        }
                        else if (args.Length > 2)
                        {
                            GaleStreams.CrashMeteor(b.X, b.Y, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]));
                        }
                        else
                        {
                            GaleStreams.CrashMeteor(b.X, b.Y, int.Parse(args[1]));
                        }
                    }
                    break;

                case "resettmerch":
                    {
                        Chest.SetupTravelShop();
                    }
                    break;

                case "debugi":
                    {
                        caller.Player.HeldItem.GetGlobalItem<NCallGlobalItem>().debug = byte.Parse(args[1]);
                    }
                    break;

                case "npcssetname":
                    {
                        string name = args[1];
                        for (int i = 2; i < args.Length; i++)
                        {
                            name += " " + args[i];
                        }
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            Main.npc[i].GivenName = name;
                        }
                    }
                    break;

                case "npcssethp":
                    {
                        int hp = int.Parse(args[1]);
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            Main.npc[i].lifeMax = hp;
                            Main.npc[i].life = hp;
                        }
                    }
                    break;

                case "generateglimmers":
                    {
                        AQWorldGen.GenerateGlimmeringStatues(null);
                    }
                    break;

                case "generateglobes":
                    {
                        AQWorldGen.GenerateGlobeTemples(null);
                    }
                    break;

                case "placeglimmer":
                    {
                        caller.Reply("placed correctly: " + GlimmeringStatueTile.TryGen((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16));
                    }
                    break;

                case "createsample":
                    {
                        Texture2D result = null;
                        switch (args[1])
                        {
                            case "alphafix":
                                {
                                    result = new ImageFunctionAlphaFixer(args[2]).CreateImage(int.Parse(args[3]), int.Parse(args[4]));
                                }
                                break;
                        }
                        string path = DebugFolderPath;
                        Directory.CreateDirectory(path);
                        result.SaveAsPng(File.Create(path + Path.DirectorySeparatorChar + "ncallresult.png"), result.Width, result.Height);
                        Main.NewText("image complete!");
                        Utils.OpenFolder(path);
                    }
                    break;

                case "glimmerlayer":
                    {
                        Main.NewText(Glimmer.Layers.Count);
                        caller.Reply("glimmer layer: " + Glimmer.FindLayer(Glimmer.Distance(caller.Player)));
                    }
                    break;

                case "placeglobe":
                    {
                        caller.Reply("placed correctly: " + Globe.GenGlobeTemple((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16));
                    }
                    break;

                case "gorenests":
                    AQWorldGen.GenerateGoreNests(null);
                    break;

                case "gorenest2":
                    caller.Reply(GoreNest.TryGrowGoreNest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, true, true).ToString());
                    break;

                case "gorenest":
                    caller.Reply(GoreNest.TryGrowGoreNest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, false, false).ToString());
                    break;

                case "downeddemonsiege":
                case "downeddemon":
                case "downedsiege":
                    WorldDefeats.DownedDemonSiege = !WorldDefeats.DownedDemonSiege;
                    break;

                case "demonsiegequick":
                    DemonSiege.UpgradeItem();
                    DemonSiege.Deactivate();
                    WorldDefeats.DownedDemonSiege = true;
                    break;

                case "demonsiegeend":
                    DemonSiege.Deactivate();
                    break;

                case "alllang":
                    {
                        int i = 0;
                        while (true)
                        {
                            var g = GameCulture.FromLegacyId(i);
                            caller.Reply(i + ": " + g.CultureInfo.Name);
                            i++;
                        }
                    }

                case "windspeed":
                    {
                        Main.windSpeedSet = int.Parse(args[1]);
                    }
                    break;

                case "weirdunusedcutscenethingy":
                    {
                        CinematicManager.Instance.PlayFilm(new DD2Film());
                    }
                    break;

                case "aquest":
                    {
                        if (args.Length == 1)
                        {
                            caller.Reply("Angler Quest: " + Main.anglerQuest + " ([i:" + Main.anglerQuestItemNetIDs[Main.anglerQuest] + "])");
                            caller.Reply("Angler Quest Finished: " + Main.anglerQuestFinished);
                        }
                        else
                        {
                            switch (args[1])
                            {
                                default:
                                    {
                                        Main.anglerQuest = int.Parse(args[1]);
                                        caller.Reply("Set Angler Quest to: " + Main.anglerQuest + " ([i:" + Main.anglerQuestItemNetIDs[Main.anglerQuest] + "])");
                                    }
                                    break;

                                case "showall":
                                    {
                                        for (int i = 0; i < Main.anglerQuestItemNetIDs.Length; i++)
                                        {
                                            caller.Reply(i + ": [i:" + Main.anglerQuestItemNetIDs[i] + "]");
                                        }
                                    }
                                    break;

                                case "true":
                                    {
                                        Main.anglerQuestFinished = true;
                                        caller.Reply("Set Angler Quest Finished: " + Main.anglerQuestFinished);
                                    }
                                    break;

                                case "false":
                                    {
                                        Main.anglerQuestFinished = false;
                                        caller.Reply("Set Angler Quest Finished: " + Main.anglerQuestFinished);
                                    }
                                    break;
                            }
                        }
                    }
                    break;

                case "ravinetest":
                    {
                        Main.NewText("Generating Ravine...");
                        BiomeCrabCrevice.PlaceLegacyOceanRavine((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, caller.Player.HeldItem.placeStyle);
                        Main.NewText("Generation Complete!");
                    }
                    break;

                case "noblemushrooms":
                    {
                        caller.Reply("can place noble group: " + AQWorldGen.TryPlaceNobleGroup(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y, WorldGen.genRand.Next(3), WorldGen.genRand.Next(30, 75)).ToString());
                    }
                    break;

                case "endinvasion":
                    {
                        Main.invasionDelay = 0;
                        Main.invasionType = 0;
                    }
                    break;

                case "startinvasion1":
                case "startgoblin":
                    {
                        Main.invasionDelay = 0;
                        Main.StartInvasion(InvasionID.GoblinArmy);
                    }
                    break;

                case "startinvasion2":
                case "startsnowlegion":
                case "startfrostlegion":
                    {
                        Main.invasionDelay = 0;
                        Main.StartInvasion(InvasionID.SnowLegion);
                    }
                    break;

                case "startinvasion3":
                case "startpirate":
                case "startpirates":
                case "startpirateinvasion":
                    {
                        Main.invasionDelay = 0;
                        Main.StartInvasion(InvasionID.PirateInvasion);
                    }
                    break;

                case "startinvasion4":
                case "startmartian":
                case "startmartians":
                case "startmartianmaddness":
                    {
                        Main.invasionDelay = 0;
                        Main.StartInvasion(InvasionID.MartianMadness);
                    }
                    break;

                case "downedskeletron":
                case "downedboss3":
                    {
                        NPC.downedBoss3 = !NPC.downedBoss3;
                        caller.Reply(NPC.downedBoss3.ToString());
                    }
                    break;

                case "downedcrimson":
                case "downedcorrupt":
                case "downedevil":
                case "downedbrainofcthulhu":
                case "downedeaterofworlds":
                case "downedboc":
                case "downedeow":
                case "downedboss2":
                    {
                        NPC.downedBoss2 = !NPC.downedBoss2;
                        caller.Reply(NPC.downedBoss2.ToString());
                    }
                    break;

                case "downedeye":
                case "downedeyeofcthulhu":
                case "downedeoc":
                case "downedboss1":
                    {
                        NPC.downedBoss1 = !NPC.downedBoss1;
                        caller.Reply(NPC.downedBoss1.ToString());
                    }
                    break;

                case "downedplantera":
                case "downedplant":
                case "downedplantboss":
                    {
                        NPC.downedPlantBoss = !NPC.downedPlantBoss;
                        caller.Reply(NPC.downedPlantBoss.ToString());
                    }
                    break;

                case "downedmoonlord":
                case "downedml":
                    {
                        NPC.downedMoonlord = !NPC.downedMoonlord;
                        caller.Reply(NPC.downedMoonlord.ToString());
                    }
                    break;

                case "downedgolem":
                case "downedgolemboss":
                    {
                        NPC.downedGolemBoss = !NPC.downedGolemBoss;
                        caller.Reply(NPC.downedGolemBoss.ToString());
                    }
                    break;

                case "downedmech1":
                case "downeddestroyer":
                    {
                        NPC.downedMechBoss1 = !NPC.downedMechBoss1;
                        caller.Reply(NPC.downedMechBoss1.ToString());
                    }
                    break;

                case "downedmech2":
                case "downedtwins":
                    {
                        NPC.downedMechBoss2 = !NPC.downedMechBoss2;
                        caller.Reply(NPC.downedMechBoss2.ToString());
                    }
                    break;

                case "downedmech3":
                case "downedprime":
                case "downedskeletronprime":
                    {
                        NPC.downedMechBoss3 = !NPC.downedMechBoss3;
                        caller.Reply(NPC.downedMechBoss3.ToString());
                    }
                    break;

                case "bloodmoon":
                    {
                        Main.dayTime = false;
                        Main.time = 0;
                        Main.bloodMoon = !Main.bloodMoon;
                        Main.pumpkinMoon = false;
                        Main.snowMoon = false;
                    }
                    break;

                case "bloodmimic":
                    {
                        if (args.Length == 1 || int.Parse(args[1]) == 0)
                        {
                            NPC.NewNPC((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<BloodMimic>());
                        }
                        else if (int.Parse(args[1]) == 1)
                        {
                            NPC npc = new NPC();
                            BloodMimic._usePlayerRectangle = false;
                            npc.SetDefaults(ModContent.NPCType<BloodMimic>());
                            int tileX = (int)Main.MouseWorld.X / 16;
                            int tileY = (int)Main.MouseWorld.Y / 16;
                            npc.modNPC.SpawnNPC(tileX, tileY);
                            BloodMimic._usePlayerRectangle = true;
                        }
                        else if (int.Parse(args[1]) == 2)
                        {
                            NPC npc = new NPC();
                            npc.SetDefaults(ModContent.NPCType<BloodMimic>());
                            int tileX = (int)Main.MouseWorld.X / 16;
                            int tileY = (int)Main.MouseWorld.Y / 16;
                            npc.modNPC.SpawnNPC(tileX - (NPC.safeRangeX + 20) * caller.Player.direction, tileY);
                        }
                    }
                    break;

                case "moonphase":
                    {
                        if (args.Length > 1)
                            Main.moonPhase = int.Parse(args[1]);
                        caller.Reply(AQText.moonPhaseName(Main.moonPhase) + "(" + Main.moonPhase + ")");
                    }
                    break;

                case "hardmode":
                    {
                        Main.hardMode = !Main.hardMode;
                        if (!Main.hardMode)
                        {
                            caller.Reply("Hard Mode is disabled");
                        }
                        else
                        {
                            caller.Reply("Hard Mode is enabled");
                        }
                    }
                    break;

                case "worldmode":
                    {
                        Main.expertMode = !Main.expertMode;
                        if (!Main.expertMode)
                        {
                            caller.Reply("Expert Mode is disabled");
                        }
                        else
                        {
                            caller.Reply("Expert Mode is enabled");
                        }
                    }
                    break;

                case "otest":
                    {
                        Item item = new Item();
                        item.SetDefaults(ItemID.TwinMask);
                        var global = item.GetGlobalItem<NCallGlobalItem>();
                        for (int i = 1; i < args.Length; i++)
                        {
                            switch (args[i])
                            {
                                case "mask":
                                    {
                                        i++;
                                        global.mask = byte.Parse(args[i]);
                                    }
                                    break;

                                case "headOverlay":
                                    {
                                        i++;
                                        global.headOverlay = byte.Parse(args[i]);
                                    }
                                    break;
                            }
                        }
                        caller.Player.QuickSpawnClonedItem(item);
                    }
                    break;

                case "checkcursor":
                    {
                        var p = Main.LocalPlayer.GetModPlayer<PlayerCursorDyes>();
                        caller.Reply(nameof(PlayerCursorDyes.cursorDye) + "2:" + PlayerCursorDyes.LocalCursorDye);
                        caller.Reply(nameof(PlayerCursorDyes.cursorDye) + ":" + p.cursorDye);
                        caller.Reply(nameof(Main.cursorColor) + ":" + Main.cursorColor);
                        caller.Reply(nameof(Main.mouseColor) + ":" + Main.mouseColor);
                    }
                    break;

                case "modencode":
                    {
                        var text = AQStringCodes.EncodeModName(args[1]);
                        caller.Reply("encoded: " + text, Color.Aqua);
                        caller.Reply("read code: " + AQStringCodes.DecodeModName(text).ToString(), Color.Yellow);
                    }
                    break;

                case "anpc":
                    {
                        var field = typeof(NPC).GetField(args[1], BindingFlags.Public | BindingFlags.Static);
                        if (field != null)
                        {
                            field.SetValue(new NPC(), false);
                        }
                        else
                        {
                            caller.Reply(args[1] + " does not have a static value");
                        }
                    }
                    break;

                case "aqplrvars":
                    {
                        var aQPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
                        Main.NewText(nameof(AQPlayer.omoriDeathTimer) + ":" + aQPlayer.omoriDeathTimer);
                    }
                    break;

                case "omori":
                    {
                        Main.LocalPlayer.GetModPlayer<AQPlayer>().omoriDeathTimer = 1;
                    }
                    break;

                case "lastcall":
                    {
                        string[] oldLastCall = lastCall;
                        Action(caller, input, lastCall);
                        lastCall = oldLastCall;
                        return;
                    }
            }
            lastCall = args;
        }

        private static void createSample(object arg)
        {
        }

        public class NCallGlobalItem : GlobalItem
        {
            public override bool Autoload(ref string name)
            {
                return ModContent.GetInstance<AQConfigServer>().debugCommand;
            }

            public override bool InstancePerEntity => true;

            public override bool CloneNewInstances => true;

            public byte headOverlay = PlayerHeadAccID.None;
            public byte mask = PlayerMaskID.None;
            public byte debug = 0;

            public override void UpdateEquip(Item item, Player player)
            {
                if (headOverlay > 0)
                    player.GetModPlayer<PlayerDrawEffects>().headAcc = headOverlay;
                if (mask > 0)
                    player.GetModPlayer<PlayerDrawEffects>().mask = mask;
            }

            public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
            {
                if (headOverlay > 0)
                    tooltips.Add(new TooltipLine(mod, "headOverlay", "headOverlay: " + headOverlay));
                if (mask > 0)
                    tooltips.Add(new TooltipLine(mod, "mask", "mask: " + mask));
                switch (debug)
                {
                    case 1:
                        {
                            bool meteorTime = GaleStreams.MeteorTime();
                            tooltips.Add(new TooltipLine(mod, "0", "meteor time: " + meteorTime));
                            tooltips.Add(new TooltipLine(mod, "1", "can meteors spawn: " + (meteorTime && Main.LocalPlayer.position.Y < 2560f).ToString()));
                            tooltips.Add(new TooltipLine(mod, "2", "windy day: " + ImitatedWindyDay.IsItAHappyWindyDay));
                            tooltips.Add(new TooltipLine(mod, "3", "amtospheric currents event: " + GaleStreams.IsActive));
                        }
                        break;
                }
            }
        }
    }
}