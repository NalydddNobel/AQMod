using Aequus.Items.Accessories.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Carpentery.Bounties
{
    public class CarpenterBountyPlayer : ModPlayer
    {
        public int SelectedBounty;
        public List<string> completedBountiesOld { get; private set; }

        public override void Initialize()
        {
            completedBountiesOld = new List<string>();
            SelectedBounty = -1;
        }

        public override void SaveData(TagCompound tag)
        {
            if (SelectedBounty > 0)
                tag["SelectedBounty"] = CarpenterSystem.BountiesByID[SelectedBounty].FullName;
        }

        public override void LoadData(TagCompound tag)
        {
            SelectedBounty = -1;
            if (tag.TryGet("SelectedBounty", out string selectedBounty) && CarpenterSystem.TryGetBounty(selectedBounty, out var bounty))
            {
                SelectedBounty = bounty.Type;
            }
            completedBountiesOld = tag.Get<List<string>>("CompletedBounties");
            if (completedBountiesOld == null)
            {
                completedBountiesOld = new List<string>();
            }
        }

        public override ModPlayer Clone(Player newEntity)
        {
            var clone = (CarpenterBountyPlayer)base.Clone(newEntity);
            clone.completedBountiesOld = new List<string>(completedBountiesOld);
            clone.SelectedBounty = SelectedBounty;
            return clone;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (CarpenterBountyPlayer)clientClone;
            clone.completedBountiesOld = new List<string>(completedBountiesOld);
            clone.SelectedBounty = SelectedBounty;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            if (completedBountiesOld.Count > 0)
            {
                for (int i = 0; i < completedBountiesOld.Count; i++)
                {
                    if (CarpenterSystem.BountiesByName.TryGetValue(completedBountiesOld[i], out var b))
                    {
                        CarpenterSystem.CompleteCarpenterBounty(b);
                    }
                }
                completedBountiesOld.Clear();
            }
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer != Player.whoAmI || Main.GameUpdateCount % 10 != 0 || NPC.AnyDanger(quickBossNPCCheck: true, ignorePillars: true))
                return;

            CheckBuffBuildings();
        }

        public void CheckBuffBuildings()
        {
            if (Main.netMode == NetmodeID.SinglePlayer && completedBountiesOld.Count > 0)
            {
                CarpenterSystem.CompletedBounties.AddRange(completedBountiesOld);
                for (int i = 0; i < completedBountiesOld.Count; i++)
                    Main.NewText(completedBountiesOld[i]);
                completedBountiesOld?.Clear();
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int val = Player.Aequus().BuildingBuffRange;
            var screenRectangle = new Rectangle((int)Player.Center.X / 16 - val, (int)Player.Center.Y / 16 - val, val * 2, val * 2);

            foreach (var pair in CarpenterSystem.BuildingBuffLocations)
            {
                if (CarpenterSystem.BountiesByID[pair.Key].BuildingBuff > 0)
                {
                    foreach (var r in pair.Value)
                    {
                        //AequusHelpers.dustDebug(r.WorldRectangle(), pair.Key);
                        if (r.Intersects(screenRectangle))
                        {
                            Player.AddBuff(CarpenterSystem.BountiesByID[pair.Key].BuildingBuff, 60);
                            goto NextPair;
                        }
                    }
                NextPair:
                    continue;
                }
            }
            stopwatch.Stop();
        }
    }
}