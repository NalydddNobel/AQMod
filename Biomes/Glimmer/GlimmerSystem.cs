using Aequus.UI.EventProgressBars;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Biomes.Glimmer
{
    public class GlimmerSystem : ModSystem
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                LegacyEventProgressBarLoader.AddBar(new GlimmerProgressBar()
                {
                    EventKey = "Mods.Aequus.BiomeName.GlimmerBiome",
                    Icon = Aequus.AssetsPath + "UI/EventIcons/Glimmer",
                    backgroundColor = new Color(20, 75, 180, 128),
                });
            }
        }

        public static void OnTransitionToNight()
        {
            int chance = 9;
            if (Main.tenthAnniversaryWorld)
            {
                chance = 6;
            }
            if (AequusWorld.downedOmegaStarite)
            {
                chance *= 4;
            }
            if (Main.rand.NextBool(chance) && !WorldGen.spawnEye && Main.GetMoonPhase() != MoonPhase.Full && !Main.bloodMoon && NPC.AnyNPCs(NPCID.Dryad))
            {
                if (BeginEvent())
                {
                    AequusText.Broadcast("Announcement.GlimmerStart", GlimmerBiome.TextColor);
                }
            }
        }

        public override void PreUpdatePlayers()
        {
            GlimmerBiome.omegaStarite = -1;
            if (GlimmerScene.cantTouchThis > 0)
                GlimmerScene.cantTouchThis--;

            if (GlimmerBiome.EventActive)
            {
                if (Main.dayTime)
                {
                    if (EndEvent() && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        AequusText.Broadcast("Announcement.GlimmerEnd", GlimmerBiome.TextColor);
                    }
                    return;
                }

                var x = GlimmerBiome.TileLocation.X;
                if (GlimmerBiome.TileLocation.Y == -1 || GlimmerBiome.TileLocation.Y == (int)Main.worldSurface)
                {
                    GlimmerBiome.TileLocation = FindGroundFor(GlimmerBiome.TileLocation);
                }
                else if (AequusHelpers.IsSectionLoaded(GlimmerBiome.TileLocation))
                {
                    if (!Main.tile[GlimmerBiome.TileLocation].IsSolid())
                    {
                        GlimmerBiome.TileLocation = FindGroundFor(GlimmerBiome.TileLocation);
                    }
                }

                if (Main.netMode == NetmodeID.Server && GlimmerBiome.TileLocation.X != x)
                {
                    SendGlimmerStatus();
                }
            }
        }

        public static void BeginEvent(Point where)
        {
            GlimmerBiome.TileLocation = FindGroundFor(where);

            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                SendGlimmerStatus();
            }
        }

        public static bool BeginEvent()
        {
            for (int i = 0; i < 100; i++)
            {
                int x = Main.rand.Next(200, Main.maxTilesX - 200);
                if ((x - Main.spawnTileX).Abs() < 100)
                    continue;
                for (int j = 0; j < Main.maxPlayers; j++)
                {
                    if (Main.player[j].active && !Main.player[j].dead)
                    {
                        int playerX = (int)(Main.player[j].position.X / 16);
                        if ((x - playerX).Abs() < GlimmerBiome.SuperStariteTile)
                        {
                            goto FoundSpot;
                        }
                    }
                }

                continue;

            FoundSpot:
                BeginEvent(new Point(x, -1));
                return true;
            }
            return false;
        }

        public static bool EndEvent()
        {
            if (!GlimmerBiome.EventActive)
            {
                return false;
            }

            GlimmerBiome.TileLocation = Point.Zero;
            if (Main.netMode == NetmodeID.Server)
            {
                SendGlimmerStatus();
            }
            return true;
        }

        public static int CalcTiles(Player player)
        {
            return (int)((player.position.X + player.width) / 16 - GlimmerBiome.TileLocation.X).Abs();
        }

        public static Point FindGroundFor(Point p)
        {
            if (p.Y <= 30)
            {
                p.Y = 30;
            }

            for (ushort j = 180; j <= Main.worldSurface; j++)
            {
                if (!AequusHelpers.IsSectionLoaded(p.X, j))
                    continue;

                if (Main.tile[p.X, j].IsSolid())
                {
                    p.Y = j;
                    break;
                }
            }
            if (p.Y != (int)Main.worldSurface)
            {
                for (ushort j = (ushort)p.Y; j > 40; j--)
                {
                    for (ushort k = 0; k < 10; k++)
                    {
                        if (!AequusHelpers.IsSectionLoaded(p.X, j - k))
                            continue;

                        if (Main.tile[p.X, j - k].IsSolid())
                        {
                            goto FoundInvalidSpot;
                        }
                    }
                    p.Y = j + 1;
                    return p;

                FoundInvalidSpot:
                    continue;
                }
            }
            p.Y = (ushort)Main.worldSurface;
            return p;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (GlimmerBiome.TileLocation == Point.Zero)
            {
                return;
            }
            tag["GlimmerX"] = GlimmerBiome.TileLocation.X;
            tag["GlimmerY"] = GlimmerBiome.TileLocation.Y;
        }

        public override void OnWorldLoad()
        {
            GlimmerBiome.TileLocation = Point.Zero;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet("GlimmerX", out int x) && tag.TryGet("GlimmerY", out int y))
                GlimmerBiome.TileLocation = new Point(x, y);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(GlimmerBiome.EventActive);
            if (GlimmerBiome.EventActive)
            {
                writer.Write((ushort)GlimmerBiome.TileLocation.X);
                writer.Write((ushort)GlimmerBiome.TileLocation.Y);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            if (reader.ReadBoolean())
            {
                GlimmerBiome.TileLocation = new Point(reader.ReadUInt16(), reader.ReadUInt16());
            }
        }

        public override void PostUpdateTime()
        {
            if (!CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled && GlimmerBiome.EventActive)
            {
                int playersInTimeWound = 0;
                int maxPlayers = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead)
                    {
                        maxPlayers++;
                        if (Main.player[i].Distance(GlimmerBiome.TileLocation.ToWorldCoordinates()) < 250f * 16f)
                        {
                            playersInTimeWound++;
                        }
                    }
                }
                if (maxPlayers > 0 && (playersInTimeWound / (float)maxPlayers) > 0.5f)
                {
                    Main.time -= 2.0d * CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
                    if (Main.time <= 1.0d)
                    {
                        Main.time = 1.0d;
                    }
                }
            }
        }

        public static void SendGlimmerStatus()
        {
            PacketSystem.Send((p) =>
            {
                p.Write(GlimmerBiome.EventActive);
                if (GlimmerBiome.EventActive)
                {
                    p.Write((ushort)GlimmerBiome.TileLocation.X);
                    p.Write((ushort)GlimmerBiome.TileLocation.Y);
                }
            }, PacketType.GlimmerStatus);
        }

        public static void ReadGlimmerStatus(BinaryReader r)
        {
            if (r.ReadBoolean())
            {
                GlimmerBiome.TileLocation = new Point(r.ReadUInt16(), r.ReadUInt16());
                return;
            }
            GlimmerBiome.TileLocation = Point.Zero;
        }
    }
}