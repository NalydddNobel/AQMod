using AQMod.Assets;
using AQMod.Assets.DrawCode;
using AQMod.Common.Config;
using AQMod.Common.IO;
using AQMod.Common.Skies;
using AQMod.Common.Utilities;
using AQMod.Common.WorldGeneration;
using AQMod.Content.CursorDyes;
using AQMod.Content.RobsterQuests;
using AQMod.Content.WorldEvents;
using AQMod.Content.WorldEvents.AzureCurrents;
using AQMod.Content.WorldEvents.CrabSeason;
using AQMod.Content.WorldEvents.DemonSiege;
using AQMod.Content.WorldEvents.GlimmerEvent;
using AQMod.Localization;
using AQMod.NPCs.Monsters;
using AQMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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
            public byte debug = 0;

            public override void UpdateEquip(Item item, Player player)
            {
                if (headOverlay > -1)
                    player.GetModPlayer<AQPlayer>().headOverlay = headOverlay;
                if (mask > -1)
                    player.GetModPlayer<AQPlayer>().mask = mask;
            }

            public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
            {
                if (headOverlay > -1)
                    tooltips.Add(new TooltipLine(mod, "headOverlay", "headOverlay: " + headOverlay));
                if (mask > -1)
                    tooltips.Add(new TooltipLine(mod, "mask", "mask: " + mask));
                switch (debug)
                {
                    case 1:
                    {
                        bool meteorTime = AzureCurrents.MeteorTime();
                        tooltips.Add(new TooltipLine(mod, "0", "meteor time: " + meteorTime));
                        tooltips.Add(new TooltipLine(mod, "1", "can meteors spawn: " + (meteorTime && Main.LocalPlayer.position.Y < AQMod.SpaceLayer - (40 * 16f)).ToString()));
                        tooltips.Add(new TooltipLine(mod, "2", "windy day: " + ImitatedWindyDay.IsItAHappyWindyDay));
                        tooltips.Add(new TooltipLine(mod, "3", "amtospheric currents event: " + AQMod.AtmosphericEvent.IsActive));
                    }
                    break;
                }
            }

            public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
            {
                if (debug > 0 && AQMod.ItemOverlays.GetOverlay(item.type) == null)
                {
                    bool resetBatch = false;
                    var drawData = new DrawData(item.GetTexture(), position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
                    if (AQConfigClient.Instance.ScrollShader)
                    {
                        resetBatch = true;
                        Main.spriteBatch.End();
                        BatcherMethods.StartShaderBatch_UI(Main.spriteBatch);
                        var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<Items.Vanities.Dyes.EnchantedDye>());
                        effect.Apply(null, drawData);
                    }
                    drawData.Draw(Main.spriteBatch);
                    if (resetBatch)
                    {
                        Main.spriteBatch.End();
                        BatcherMethods.StartBatch_UI(Main.spriteBatch);
                    }
                    return false;
                }
                return true;
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

                case "meteor":
                {
                    var b = Main.MouseWorld.ToTileCoordinates();
                    if (args.Length > 7)
                    {
                        AzureCurrents.CrashMeteor(b.X, b.Y, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), bool.Parse(args[6]), ushort.Parse(args[7]));
                    }
                    else if (args.Length > 6)
                    {
                        AzureCurrents.CrashMeteor(b.X, b.Y, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]), bool.Parse(args[6]));
                    }
                    else if (args.Length > 2)
                    {
                        AzureCurrents.CrashMeteor(b.X, b.Y, int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]), int.Parse(args[5]));
                    }
                    else
                    {
                        AzureCurrents.CrashMeteor(b.X, b.Y, int.Parse(args[1]));
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
                    var aQPlayer = caller.Player.GetModPlayer<AQPlayer>();
                    string path = AQMod.DebugFolderPath;
                    Directory.CreateDirectory(path);
                    var buffer = aQPlayer.SerializeBossKills();
                    var stream = File.Create(path + Path.DirectorySeparatorChar + "encorekills.txt", buffer.Length);
                    stream.Write(buffer, 0, buffer.Length);
                    Utils.OpenFolder(path);
                    aQPlayer.DeserialzeBossKills(buffer);
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
                    ThreadPool.QueueUserWorkItem(createSample, args);
                }
                break;

                case "glimmerlayer":
                {
                    caller.Reply("glimmer layer: " + GlimmerEvent.GetLayerIndex(AQMod.CosmicEvent.GetTileDistance(caller.Player)));
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

                case "robstersave":
                {
                    caller.Reply("Hunt Key: " + HuntSystem.Hunt.GetKey());
                    if (HuntSystem.TargetNPC != -1)
                    {
                        var modNPCIO = new ModNPCIO();
                        string key = modNPCIO.GetKey(HuntSystem._targetNPCType);
                        caller.Reply("Target NPC: " + key);
                        int npcType = modNPCIO.GetID(key);
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
                WorldGen.PlaceTile((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, ModContent.TileType<Tiles.ExoticCoral>(), true, false, -1, int.Parse(args[1]));
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
                    caller.Reply(ExoticCoral.TryPlaceExoticBlotch((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16, WorldGen.genRand.Next(3), 50).ToString());
                }
                break;

                case "tikitest":
                {
                    Main.NewText("Generating Biome...");
                    Main.NewText("Generation Complete!");
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
                            npc.SetDefaults(ModContent.NPCType<NPCs.Boss.Starite.OmegaStarite>());
                            var drawPos = Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
                            npc.Center = drawPos;
                            var omegaStarite = (NPCs.Boss.Starite.OmegaStarite)npc.modNPC;
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
                            npc.SetDefaults(ModContent.NPCType<NPCs.Boss.Starite.OmegaStarite>());

                            npc.Center = Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
                            var omegaStarite = (NPCs.Boss.Starite.OmegaStarite)npc.modNPC;
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
                    var drawingPlayer = Main.LocalPlayer.GetModPlayer<AQPlayer>();
                    caller.Reply(nameof(AQPlayer.CursorDyeID) + ":" + drawingPlayer.CursorDyeID);
                    caller.Reply(nameof(AQPlayer.CursorDye) + ":" + drawingPlayer.CursorDye);
                    caller.Reply(nameof(AQMod.CursorDyes.Count) + ":" + AQMod.CursorDyes.Count);
                    for (int i = 0; i < AQMod.CursorDyes.Count; i++)
                    {
                        var cursorDye = AQMod.CursorDyes.GetContent(i);
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

                case "spotlight":
                {
                    result = new SpotlightCircle().CreateImage(int.Parse(args[2]), int.Parse(args[3]));
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

                case "aura":
                {
                    var getTexture = ModContent.GetTexture(args[2]);
                    int maxWidth = 4;
                    if (args.Length > 3)
                    {
                        maxWidth = int.Parse(args[3]);
                    }
                    Vector3 tint = new Vector3(1f, 1f, 1f);
                    if (args.Length > 4)
                    {
                        tint.X = float.Parse(args[4]);
                        tint.Y = float.Parse(args[5]);
                        tint.Z = float.Parse(args[6]);
                    }
                    result = new Aura(getTexture, tint, maxWidth).CreateImage(getTexture.Width, getTexture.Height);
                }
                break;
            }
            string path = AQMod.DebugFolderPath;
            Directory.CreateDirectory(path);
            result.SaveAsPng(File.Create(path + Path.DirectorySeparatorChar + "ncallresult.png"), result.Width, result.Height);
            Main.NewText("image complete!");
            Utils.OpenFolder(path);
        }
    }
}