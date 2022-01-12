using AQMod.Assets;
using AQMod.Common.Graphics;
using AQMod.Common.IO;
using AQMod.Common.Utilities;
using AQMod.Common.WorldGeneration;
using AQMod.Content.CursorDyes;
using AQMod.Content.Players;
using AQMod.Content.Quest.Lobster;
using AQMod.Content.World.Events;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Content.World.Events.GaleStreams;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Content.World.Generation;
using AQMod.Effects.Particles;
using AQMod.Localization;
using AQMod.NPCs.Monsters;
using AQMod.Tiles.Furniture;
using AQMod.Tiles.Nature;
using AQMod.Tiles.Nature.CrabCrevice;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Terraria;
using Terraria.Cinematics;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Common.DeveloperTools
{
    internal class NCall : ModCommand
    {
        public static string DebugFolderPath => Main.SavePath + Path.DirectorySeparatorChar + "Mods" + Path.DirectorySeparatorChar + "Cache" + Path.DirectorySeparatorChar + "AQMod";
        private static QuickSearchList<string> quickSearchList;

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

                case "genccsand":
                    {
                        CrabCrevice.CreateSandAreaForCrevice(mX, mY);
                    }
                    break;

                case "gencc":
                    {
                        CrabCrevice.GenerateCreviceCave(mX, mY, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
                    }
                    break;

                case "enumarrayvsdictionary":
                    {
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();

                        for (int i = 0; i < 100000000; i++)
                        {
                            var t = AQTextures.Particles[ParticleTex.SpaceSquidSnowflake];
                            if (t.IsDisposed)
                                t.Dispose();
                        }

                        stopWatch.Stop();
                        AQMod.GetInstance().Logger.Debug(stopWatch.ElapsedMilliseconds);
                    }
                    break;

                case "qslget":
                    {
                        if (int.TryParse(args[1], out int result))
                        {
                            caller.Reply(quickSearchList.Find(result));
                        }
                        else
                        {
                            caller.Reply(quickSearchList.Find(args[1]));
                        }
                    }
                    break;

                case "qsl":
                    {
                        if (quickSearchList == null)
                        {
                            quickSearchList = new QuickSearchList<string>();
                        }
                        quickSearchList.Add(args[1]);
                        var arr = quickSearchList.ToArray();
                        for (int i = 0; i < arr.Length; i++)
                        {
                            caller.Reply(i + ": " + arr[i]);
                        }
                    }
                    break;

                case "gen":
                    {
                        switch (args[1].ToLower())
                        {
                            case "buriedchests":
                                ChestLoot.Buried.GenerateDirtChests(null);
                                break;
                            case "waterfixer":
                                WaterCleaner.PassFix1TileHighWater(null);
                                break;
                        }
                    }
                    break;

                case "waterfix":
                    {
                        WaterCleaner.ApplyFix(mX, mY);
                    }
                    break;

                case "buriedchest":
                    {
                        ChestLoot.Buried.PlaceBuriedChest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y, out int chestID, caller.Player.HeldItem.createTile, WallID.Dirt, caller.Player.HeldItem.placeStyle);
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
                        WikiTestStuff.basicwikipage(AQMod.GetInstance().GetItem(args[1]));
                    }
                    break;

                case "chestloot":
                    {
                        ChestLoot.AddLoot(Chest.FindChest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y));
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
                        caller.Player.HeldItem.GetGlobalItem<Content.NameTagItem>().nameTag = name;
                    }
                    break;

                case "demonsiegei":
                    caller.Reply("x: " + DemonSiege.X);
                    caller.Reply("y: " + DemonSiege.Y);
                    caller.Reply("plr: " + DemonSiege.PlayerActivator + " (" + Main.player[DemonSiege.PlayerActivator] + ")");
                    if (DemonSiege.BaseItem != null)
                        caller.Reply("item: " + DemonSiege.BaseItem.type + " (" + Lang.GetItemName(DemonSiege.BaseItem.type) + ")");
                    break;

                case "staritescene":
                    {
                        caller.Reply("scene: " + OmegaStariteScenes.SceneType + ", index: " + OmegaStariteScenes.OmegaStariteIndexCache);
                    }
                    break;

                case "glimmerxy":
                    {
                        caller.Reply("x: " + GlimmerEvent.tileX + ", y: " + GlimmerEvent.tileY);
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
                            AQSystem.DayrateIncrease = 0;
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
                            AQSystem.DayrateIncrease = 0;
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

                case "writeencore":
                    {
                        var bossEncore = caller.Player.GetModPlayer<BossEncorePlayer>();
                        string path = DebugFolderPath;
                        Directory.CreateDirectory(path);
                        var buffer = bossEncore.SerializeEncoreRecords();
                        var stream = File.Create(path + Path.DirectorySeparatorChar + "encorekills.txt", buffer.Length);
                        stream.Write(buffer, 0, buffer.Length);
                        Utils.OpenFolder(path);
                        bossEncore.DeserialzeEncoreRecords(buffer);
                        stream.Dispose();
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
                        caller.Reply("placed correctly: " + GlimmeringStatue.TryGenGlimmeringStatue((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16));
                    }
                    break;

                case "createsample":
                    {
                        if (args[1][0] == '0')
                        {
                            string path = "";
                            for (int i = 1; i < args[1].Length; i++)
                            {
                                path += args[1][i];
                            }
                            args[1] = path;
                            ThreadPool.QueueUserWorkItem(createSample, args);
                        }
                        else
                        {
                            createSample(args);
                        }
                    }
                    break;

                case "glimmerlayer":
                    {
                        caller.Reply("glimmer layer: " + GlimmerEvent.GetLayerIndexThroughTileDistance(GlimmerEvent.GetTileDistanceUsingPlayer(caller.Player)));
                    }
                    break;

                case "placeglobe":
                    {
                        caller.Reply("placed correctly: " + Globe.GenGlobeTemple((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16));
                    }
                    break;

                case "placeuglobe":
                    {
                        caller.Reply("placed correctly: " + Globe.PlaceUndiscoveredGlobe(Main.MouseWorld.ToTileCoordinates()));
                    }
                    break;

                case "robstersave2":
                    {
                        caller.Reply("Hunt Key: " + HuntSystem.Hunt.GetKey());
                        if (HuntSystem.TargetNPC != -1)
                        {
                            var tag = SpecialTagCompounds.NPC.SaveNPCID(HuntSystem._targetNPCType);
                            var dict = tag.test_RipOutTagData();
                            foreach (var pair in dict)
                            {
                                caller.Reply(pair.Key + ": " + pair.Value.ToString());
                            }
                            int npcType = SpecialTagCompounds.NPC.GetNPCID(tag);
                            HuntSystem.SetNPCTarget(npcType);
                            caller.Reply("Attempt reload NPC type: " + npcType + " (" + Lang.GetNPCNameValue(npcType) + "), ((" + Main.npc[HuntSystem.TargetNPC].FullName + "))");
                        }
                    }
                    break;

                case "robstersave":
                    {
                        caller.Reply("Hunt Key: " + HuntSystem.Hunt.GetKey());
                        if (HuntSystem.TargetNPC != -1)
                        {
                            string key = ModNPCIO.GetKey(HuntSystem._targetNPCType);
                            caller.Reply("Target NPC: " + key);
                            int npcType = ModNPCIO.GetID(key);
                            HuntSystem.SetNPCTarget(npcType);
                            caller.Reply("Attempt reload NPC type: " + npcType + " (" + Lang.GetNPCNameValue(npcType) + "), ((" + Main.npc[HuntSystem.TargetNPC].FullName + "))");
                        }
                    }
                    break;

                case "gorenests":
                    AQWorldGen.GenerateGoreNests(null);
                    break;

                case "gorenest2":
                    caller.Reply(GoreNest.GrowGoreNest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, true, true).ToString());
                    break;

                case "gorenest":
                    caller.Reply(GoreNest.GrowGoreNest((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, false, false).ToString());
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

                case "fish":
                    {
                        var aQPlayer = caller.Player.GetModPlayer<AQPlayer>();
                        caller.Reply("fishing power cache: " + aQPlayer.FishingPowerCache.ToString());
                        caller.Reply("popper power: " + aQPlayer.PopperBaitPower.ToString());
                        caller.Reply("popper type: " + aQPlayer.PopperType.ToString() + "( [i:" + aQPlayer.PopperType.ToString() + "] )");
                    }
                    break;

                case "crabseasontimer2":
                    if (args.Length > 1)
                    {
                        CrabSeason.crabSeasonTimer = int.Parse(args[1]);
                    }
                    if (CrabSeason.crabSeasonTimer < 0)
                    {
                        caller.Reply(-CrabSeason.crabSeasonTimer + " until crab season ends");
                    }
                    else
                    {
                        caller.Reply(CrabSeason.crabSeasonTimer + " until crab season starts");
                    }
                    break;

                case "crabseasontimer":
                    if (args.Length > 1)
                    {
                        CrabSeason.crabSeasonTimer = int.Parse(args[1]);
                    }
                    if (CrabSeason.crabSeasonTimer < 0)
                    {
                        caller.Reply(AQUtils.TimeText2(-CrabSeason.crabSeasonTimer) + " until crab season ends");
                    }
                    else
                    {
                        caller.Reply(AQUtils.TimeText2(CrabSeason.crabSeasonTimer) + " until crab season starts");
                    }
                    break;

                case "crabseasonstart":
                    {
                        CrabSeason.Activate();
                        AQMod.BroadcastMessage(AQText.Key + "Common.CrabSeasonWarning", CrabSeason.TextColor);
                    }
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

                case "langmerge2":
                    {
                        LangHelper.MergeEnglish(GameCulture.FromLegacyId(int.Parse(args[1])));
                    }
                    break;

                case "langmerge":
                    {
                        LangHelper.Merge(GameCulture.FromLegacyId(int.Parse(args[1])));
                    }
                    break;

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

                case "placecoral":
                    WorldGen.PlaceTile((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, ModContent.TileType<ExoticCoral>(), true, false, -1, int.Parse(args[1]));
                    break;

                case "ravinetest":
                    {
                        Main.NewText("Generating Ravine...");
                        CrabCrevice.PlaceLegacyOceanRavine((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, caller.Player.HeldItem.placeStyle);
                        Main.NewText("Generation Complete!");
                    }
                    break;

                case "coraltest":
                    {
                        caller.Reply(ExoticCoral.TryPlaceExoticBlotch((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, WorldGen.genRand.Next(3), 50).ToString());
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

                case "tikichesttrap":
                    {
                        AQWorldGen.TryPlaceFakeTikiChest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y);
                    }
                    break;

                case "tikichest":
                    {
                        AQWorldGen.TryPlaceTikiChest(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y, out int _);
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
                        var drawingPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
                        caller.Reply(nameof(AQPlayer.CursorDyeID) + ":" + drawingPlayer.CursorDyeID);
                        caller.Reply(nameof(AQPlayer.CursorDye) + ":" + drawingPlayer.CursorDye);
                        caller.Reply(nameof(CursorDyeManager.Instance.Count) + ":" + CursorDyeManager.Instance.Count);
                        for (int i = 0; i < CursorDyeManager.Instance.Count; i++)
                        {
                            var cursorDye = CursorDyeManager.Instance.GetContent(i);
                            caller.Reply(nameof(CursorDye.Mod) + i + ":" + cursorDye.Mod);
                            caller.Reply(nameof(CursorDye.Name) + i + ":" + cursorDye.Name);
                        }
                        caller.Reply(nameof(Main.cursorColor) + ":" + Main.cursorColor);
                        caller.Reply(nameof(Main.mouseColor) + ":" + Main.mouseColor);
                    }
                    break;

                case "modencode":
                    {
                        var text = AQStringCodes.EncodeModName(args[1]);
                        caller.Reply("encoded: " + text, Colors.RarityGreen);
                        caller.Reply("read code: " + AQStringCodes.DecodeModName(text).ToString(), Colors.RarityAmber);
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
            string[] args = (string[])arg;
            Texture2D result = null;
            switch (args[1])
            {
                case "alphafix":
                    {
                        result = new AlphaFixer(args[2]).CreateImage(int.Parse(args[3]), int.Parse(args[4]));
                    }
                    break;

                case "fester":
                    {
                        if (args.Length > 4)
                        {
                            result = new FesteringCircle(float.Parse(args[4]), float.Parse(args[5]), float.Parse(args[6]), float.Parse(args[7]), float.Parse(args[8])).CreateImage(int.Parse(args[2]), int.Parse(args[3]));
                        }
                        else
                        {
                            result = new FesteringCircle(1f, 1f, 1f, 0.2f, 2f).CreateImage(int.Parse(args[2]), int.Parse(args[3]));
                        }
                    }
                    break;
            }
            string path = DebugFolderPath;
            Directory.CreateDirectory(path);
            result.SaveAsPng(File.Create(path + Path.DirectorySeparatorChar + "ncallresult.png"), result.Width, result.Height);
            Main.NewText("image complete!");
            Utils.OpenFolder(path);
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

            public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
            {
                if (debug > 0 && AQMod.ItemOverlays.GetOverlay(item.type) == null)
                {
                    var drawData = new DrawData(item.GetTexture(), position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
                    Main.spriteBatch.End();
                    BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Shader);
                    var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>());
                    effect.Apply(null, drawData);
                    drawData.Draw(Main.spriteBatch);
                    Main.spriteBatch.End();
                    BatcherMethods.UI.Begin(Main.spriteBatch, BatcherMethods.Regular);
                    return false;
                }
                return true;
            }
        }
    }
}