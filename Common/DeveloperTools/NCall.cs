using AQMod.Assets.Sounds.Music;
using AQMod.Common.Config;
using AQMod.Common.CursorDyes;
using AQMod.Common.IO;
using AQMod.Common.Skies;
using AQMod.Common.WorldEvents;
using AQMod.Content;
using AQMod.Localization;
using AQMod.NPCs;
using AQMod.NPCs.Town;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Terraria;
using Terraria.Cinematics;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Common.DeveloperTools
{
    internal class NCall : ModCommand
    {
        public class NCallGlobalItem : GlobalItem
        {
            public override bool Autoload(ref string name)
            {
                return ModContent.GetInstance<AQConfigServer>().debugCommand;
            }

            public override bool InstancePerEntity => true;

            public override bool CloneNewInstances => true;

            public int headOverlay = -1;
            public int mask = -1;

            public override void UpdateEquip(Item item, Player player)
            {
                if (headOverlay > -1)
                    player.GetModPlayer<AQVisualsPlayer>().headOverlay = headOverlay;
                if (mask > -1)
                    player.GetModPlayer<AQVisualsPlayer>().mask = mask;
            }

            public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
            {
                if (headOverlay > -1)
                    tooltips.Add(new TooltipLine(mod, "headOverlay", "headOverlay: " + headOverlay));
                if (mask > -1)
                    tooltips.Add(new TooltipLine(mod, "mask", "mask: " + mask));
            }
        }

        public override bool Autoload(ref string name)
        {
            return ModContent.GetInstance<AQConfigServer>().debugCommand;
        }

        public override string Command => "ncall";

        public override CommandType Type => CommandType.World;

        private static string[] lastCall = null;

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

                case "preventstaritedeath":
                AQNPC._preventStariteDeath = !AQNPC._preventStariteDeath;
                caller.Reply(AQNPC._preventStariteDeath.ToString());
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

                case "mapstuff":
                {
                    var mapPlayer = caller.Player.GetModPlayer<MapMarkerPlayer>();
                    for (int i = 0; i < MapMarkerPlayer.MapMarkerCount; i++)
                    {
                        caller.Reply(i + ":" + mapPlayer.MarkersObtained[i] + ", " + mapPlayer.MarkersHidden[i]);
                    }
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
                break;

                case "langmerge":
                {
                    LangHelper.Merge(GameCulture.FromLegacyId(int.Parse(args[1])));
                }
                break;

                case "music":
                {
                    caller.Reply(AQMusicManager.Crabson.ToString());
                    caller.Reply(AQMusicManager.GlimmerEvent.ToString());
                    caller.Reply(AQMusicManager.OmegaStarite.ToString());
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
                WorldGen.PlaceTile((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, ModContent.TileType<Tiles.ExoticCoral>(), true, false, -1, int.Parse(args[1]));
                break;

                case "placegolden1":
                WorldGen.PlaceTile((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, ModContent.TileType<Tiles.JeweledCandelabra>());
                break;

                case "placegolden0":
                WorldGen.PlaceTile((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, ModContent.TileType<Tiles.Bottles>());
                break;

                case "randgolden1":
                {
                    caller.Reply(Robster.TryPlaceGoldenCandelabra(out byte npc) + ":" + npc);
                }
                break;

                case "randgolden0":
                {
                    caller.Reply(Robster.TryPlaceGoldenChalice(out byte npc) + ":" + npc);
                }
                break;

                case "ravinetest":
                {
                    Main.NewText("Generating Ravine...");
                    AQWorldGen.PlaceOceanRavine((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, caller.Player.HeldItem.placeStyle);
                    Main.NewText("Generation Complete!");
                }
                break;

                case "coraltest":
                {
                    caller.Reply(AQWorldGen.TryPlaceExoticBlotch((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, WorldGen.genRand.Next(3), 50).ToString());
                }
                break;

                case "lquest":
                {
                    if (args.Length == 1)
                    {
                        caller.Reply("Story Quest: " + Robster.StoryProgression);
                        caller.Reply("Random Quest: " + Robster.RandomHunt);
                    }
                    else
                    {
                        byte set = byte.Parse(args[1]);
                        Robster.StoryProgression = set;
                        caller.Reply("Set Story Quest to: " + set);
                        if (args.Length > 2)
                        {
                            set = byte.Parse(args[2]);
                            Robster.RandomHunt = set;
                            caller.Reply("Set Random Quest to: " + set);
                        }
                    }
                }
                break;

                case "tikitest":
                {
                    Main.NewText("Generating Biome...");
                    Main.NewText("Generation Complete!");
                }
                break;

                case "screenshader":
                {

                }
                break;

                case "render":
                if (args.Length > 1)
                {
                    object captureLock = new object();
                    Monitor.Enter(captureLock);
                    Main.GlobalTimerPaused = true;

                    string saveLocation = Main.SavePath + Path.DirectorySeparatorChar + "render_output_" + args[1] + Path.DirectorySeparatorChar;
                    Directory.CreateDirectory(saveLocation);

                    switch (args[1])
                    {
                        case "omegastarite":
                        {
                            NPC npc = new NPC();
                            npc.SetDefaults(ModContent.NPCType<NPCs.Glimmer.OmegaStar.OmegaStarite>());
                            var drawPos = Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
                            npc.Center = drawPos;
                            var omegaStarite = (NPCs.Glimmer.OmegaStar.OmegaStarite)npc.modNPC;
                            omegaStarite.Init();
                            for (int i = 0; i < 240; i++)
                            {
                                var capture = new RenderTarget2D(Main.spriteBatch.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, Main.spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                                var captureBatch = Main.spriteBatch;

                                captureBatch.GraphicsDevice.SetRenderTarget(capture);
                                captureBatch.GraphicsDevice.Clear(Color.Transparent);

                                captureBatch.Begin();

                                omegaStarite.npc.FindFrame();
                                omegaStarite.innerRingRotation += 0.0314f;
                                omegaStarite.innerRingRoll += 0.0157f;
                                omegaStarite.innerRingPitch += 0.01f;
                                omegaStarite.outerRingRotation += 0.0157f;
                                omegaStarite.outerRingRoll += 0.0314f;
                                omegaStarite.outerRingPitch += 0.011f;

                                omegaStarite.Spin(drawPos);
                                npc.modNPC.PreDraw(Main.spriteBatch, Color.White);

                                captureBatch.End();

                                captureBatch.GraphicsDevice.SetRenderTarget(null);

                                var stream = File.Create(saveLocation + "Frame_" + i + ".png");
                                capture.SaveAsPng(stream, capture.Width, capture.Height);
                                stream.Dispose();
                            }
                        }
                        break;

                        case "omegastaritebosschecklistrender":
                        {
                            var capture = new RenderTarget2D(Main.spriteBatch.GraphicsDevice, Main.screenWidth, Main.screenHeight, false, Main.spriteBatch.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                            var captureBatch = Main.spriteBatch;

                            captureBatch.GraphicsDevice.SetRenderTarget(capture);
                            captureBatch.GraphicsDevice.Clear(Color.Transparent);

                            captureBatch.Begin();

                            NPC npc = new NPC();
                            npc.SetDefaults(ModContent.NPCType<NPCs.Glimmer.OmegaStar.OmegaStarite>());

                            npc.Center = Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
                            var omegaStarite = (NPCs.Glimmer.OmegaStar.OmegaStarite)npc.modNPC;
                            omegaStarite.Init();
                            omegaStarite.innerRingRoll = -0.8f;
                            omegaStarite.outerRingRoll = -0.865f;
                            omegaStarite.Spin(npc.Center);
                            npc.modNPC.PreDraw(Main.spriteBatch, Color.White);

                            captureBatch.End();

                            captureBatch.GraphicsDevice.SetRenderTarget(null);

                            var stream = File.Create(saveLocation + "Frame_0.png");
                            capture.SaveAsPng(stream, capture.Width, capture.Height);
                            stream.Dispose();
                        }
                        break;
                    }

                    Monitor.Exit(captureLock);
                }
                else
                {
                    caller.Reply("render failed");
                }
                break;

                case "noblemushrooms":
                {
                    caller.Reply("can place noble group: " + AQWorldGen.TryPlaceNobleGroup(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y, WorldGen.genRand.Next(3), WorldGen.genRand.Next(30, 75)).ToString());
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

                case "initnight":
                {
                    Main.dayTime = true;
                    Main.time = Main.dayLength - 2;
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

                case "glimmerevent":
                {
                    if (args.Length == 1)
                    {
                        caller.Reply("GlimmerEvent.active: " + GlimmerEvent.IsActive);
                        caller.Reply("GlimmerEvent.X: " + GlimmerEvent.X + "(" + AQUtils.Info_GetCompass(GlimmerEvent.X * 16f) + ")");
                        caller.Reply("GlimmerEvent.Y: " + GlimmerEvent.Y + "(" + AQUtils.Info_GetDepthMeter(GlimmerEvent.Y * 16f) + ")");
                        caller.Reply("GlimmerEvent.GlimmerChance: " + GlimmerEvent.GlimmerChance);
                    }
                    else
                    {
                        switch (args[1])
                        {
                            case "checkdowned":
                            {
                                caller.Reply("WorldDefeats.DownedGlimmer: " + WorldDefeats.DownedGlimmer);
                            }
                            break;

                            case "downed":
                            {
                                WorldDefeats.DownedGlimmer = !WorldDefeats.DownedGlimmer;
                                caller.Reply("WorldDefeats.DownedGlimmer: " + WorldDefeats.DownedGlimmer);
                            }
                            break;

                            case "x":
                            if (args.Length > 2)
                            {
                                GlimmerEvent.X = (ushort)int.Parse(args[2]);
                            }
                            else
                            {
                                GlimmerEvent.X = (ushort)(caller.Player.position.X / 16f);
                            }
                            caller.Reply("GlimmerEvent.X: " + GlimmerEvent.X + "(" + AQUtils.Info_GetCompass(GlimmerEvent.X * 16f) + ")");
                            break;
                        }
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
                                global.mask = int.Parse(args[i]);
                            }
                            break;

                            case "headOverlay":
                            {
                                i++;
                                global.headOverlay = int.Parse(args[i]);
                            }
                            break;
                        }
                    }
                    caller.Player.QuickSpawnClonedItem(item);
                }
                break;

                case "checkcursor":
                {
                    var drawingPlayer = Main.LocalPlayer.GetModPlayer<AQVisualsPlayer>();
                    caller.Reply(nameof(AQVisualsPlayer.CursorDyeID) + ":" + drawingPlayer.CursorDyeID);
                    caller.Reply(nameof(AQVisualsPlayer.CursorDye) + ":" + drawingPlayer.CursorDye);
                    caller.Reply(nameof(CursorDyeManager.CursorDyeCount) + ":" + CursorDyeManager.CursorDyeCount);
                    for (int i = 0; i < CursorDyeManager.CursorDyeCount; i++)
                    {
                        var cursorDye = CursorDyeManager.CursorDyeFromID(i + 1);
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

                case "glimmer":
                caller.Reply("chance: " + GlimmerEvent.GlimmerChance);
                caller.Reply("active: " + GlimmerEvent.ActuallyActive);
                caller.Reply("fake active: " + GlimmerEvent.FakeActive);
                break;

                case "glimmerstart":
                GlimmerEvent.GlimmerChance = GlimmerEvent.GlimmerChanceMax;
                GlimmerEvent.ActuallyActive = true;
                GlimmerEventSky.InitNight();
                GlimmerEvent.Activate();
                break;

                case "glimmerend":
                GlimmerEvent.Deactivate();
                break;

                case "glimmerinit":
                GlimmerEvent.UpdateNight();
                GlimmerEventSky.InitNight();
                break;

                case "staritedisco":
                {
                    AQNPC.StariteDisco = !AQNPC.StariteDisco;
                    if (!AQNPC.StariteDisco)
                        AQNPC.StariteProjectileColor = AQNPC.StariteProjectileColorOrig;
                }
                break;

                case "staritedisco2":
                {
                    AQNPC.StariteDisco = !AQNPC.StariteDisco;
                }
                break;

                case "goodbyelonelystarite":
                GlimmerEventSky._lonelyStariteTimeLeft = 0;
                break;

                case "goodbyelonelystarite2":
                GlimmerEventSky._lonelyStariteTimeLeft = 0;
                GlimmerEventSky._lonelyStarite = null;
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
    }
}