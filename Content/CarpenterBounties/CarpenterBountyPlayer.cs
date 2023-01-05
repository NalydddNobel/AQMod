using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterBountyPlayer : ModPlayer
    {
        public List<string> CompletedBounties { get; private set; }
        public static int RecheckDelay;

        public override void Initialize()
        {
            CompletedBounties = new List<string>();
        }

        public override void SaveData(TagCompound tag)
        {
            tag["CompletedBounties"] = CompletedBounties;
        }

        public override void LoadData(TagCompound tag)
        {
            CompletedBounties = tag.Get<List<string>>("CompletedBounties");
            if (CompletedBounties == null)
            {
                CompletedBounties = new List<string>();
            }
        }

        public override ModPlayer Clone(Player newEntity)
        {
            var clone = (CarpenterBountyPlayer)base.Clone(newEntity);
            clone.CompletedBounties = new List<string>(CompletedBounties);
            return clone;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (CarpenterBountyPlayer)clientClone;
            clone.CompletedBounties = new List<string>(CompletedBounties);
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            var client = (CarpenterBountyPlayer)clientPlayer;
            if (CompletedBounties.Count != client.CompletedBounties.Count)
            {
                PacketSystem.Send((p) =>
                {
                    p.Write(Player.whoAmI);
                    p.Write(CompletedBounties.Count);
                    for (int i = 0; i < CompletedBounties.Count; i++)
                    {
                        p.Write(CompletedBounties[i]);
                    }
                }, PacketType.CarpenterBountiesCompleted);
            }
        }

        public void RecieveClientChanges(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            CompletedBounties.Clear();
            for (int i = 0; i < CompletedBounties.Count; i++)
            {
                CompletedBounties.Add(reader.ReadString());
            }
        }

        public override void PostUpdate()
        {
            return;

            if (Main.myPlayer != Player.whoAmI)
                return;

            if (RecheckDelay > 0)
            {
                RecheckDelay--;
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var b = Main.rand.Next(CarpenterSystem.BountiesByID);
            CheckNearbyBuildings(b);
            var screenRectangle = new Rectangle((int)Player.Center.X / 16 - 20, (int)Player.Center.Y / 16 - 20, 40, 40);
            foreach (var pair in CarpenterSystem.BuildingBuffLocations)
            {
                if (CarpenterSystem.BountiesByID[pair.Key].BuildingBuff > 0)
                {
                    foreach (var r in pair.Value)
                    {
                        if (r.Intersects(screenRectangle))
                        {
                            Player.AddBuff(CarpenterSystem.BountiesByID[pair.Key].BuildingBuff, 60);
                            goto NextPair;
                        }
                    }
                NextPair:
                    break;
                }
            }
            stopwatch.Stop();
            //if (stopwatch.ElapsedMilliseconds > 10)
            //{
            //    Main.NewText($"{b.Name}: {stopwatch.ElapsedMilliseconds}");
            //}
            RecheckDelay += (int)Math.Min(stopwatch.ElapsedMilliseconds * 2, 50);
            RecheckDelay += 10;
        }

        public void CheckNearbyBuildings(CarpenterBounty b)
        {
            if (b.BuildingBuff <= 0)
            {
                return;
            }
            int x = Main.rand.Next((int)Player.Center.X / 16 - 100, (int)Player.Center.X / 16 + 100);
            int y = Main.rand.Next((int)Player.Center.Y / 16 - 100, (int)Player.Center.Y / 16 + 100);
            bool addNew = true;
            if (CarpenterSystem.BuildingBuffLocations.ContainsKey(b.Type))
            {
                foreach (var buildings in CarpenterSystem.BuildingBuffLocations[b.Type])
                {
                    if (buildings.Contains(x, y))
                    {
                        x = buildings.Center.X;
                        y = buildings.Center.Y;
                        addNew = false;
                    }
                }
            }
            var r = new Rectangle(x - 20, y - 20, 40, 40).Fluffize(20);
            var map = new TileMapCache(r);
            var result = b.CheckConditions(new StepInfo(map, r));
            if (result.success)
            {
                if (addNew)
                    CarpenterSystem.AddBuildingBuffLocation(b.Type, r);
            }
            else
            {
                CarpenterSystem.RemoveBuildingBuffLocation(b.Type, x, y);
            }
        }
    }
}