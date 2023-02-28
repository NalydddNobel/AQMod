using Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties
{
    public class CarpenterBountyPlayer : ModPlayer
    {
        public static List<StepResult> LastPhotoTakenResults;

        public int SelectedBounty;
        public List<string> collectedBounties { get; private set; }

        public override void Initialize()
        {
            collectedBounties = new List<string>();
            collectedBounties = new List<string>();
            SelectedBounty = -1;
        }

        public override void SaveData(TagCompound tag)
        {
            if (SelectedBounty > 0)
            {
                tag["SelectedBounty"] = CarpenterSystem.BountiesByID[SelectedBounty].FullName;
            }
            if (collectedBounties.Count > 0)
            {
                tag["CollectedBounties"] = collectedBounties;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            SelectedBounty = -1;
            if (tag.TryGet("SelectedBounty", out string selectedBounty) && CarpenterSystem.TryGetBounty(selectedBounty, out var bounty))
            {
                SelectedBounty = bounty.Type;
            }
            collectedBounties = tag.Get<List<string>>("CollectedBounties");
            if (collectedBounties == null)
            {
                collectedBounties = new List<string>();
            }
        }

        public override ModPlayer Clone(Player newEntity)
        {
            var clone = (CarpenterBountyPlayer)base.Clone(newEntity);
            clone.collectedBounties = new List<string>(collectedBounties);
            clone.SelectedBounty = SelectedBounty;
            return clone;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (CarpenterBountyPlayer)clientClone;
            clone.collectedBounties = new List<string>(collectedBounties);
            clone.SelectedBounty = SelectedBounty;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
        }

        public override void PostUpdate()
        {
            if (Main.myPlayer != Player.whoAmI || Main.GameUpdateCount % 10 != 0 || NPC.AnyDanger(quickBossNPCCheck: true, ignorePillars: true))
                return;

            CheckBuffBuildings();
        }

        public void CheckBuffBuildings()
        {
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

        public bool HasUnclaimedBounty()
        {
            return CarpenterSystem.CompletedBounties.ContainsAny((t) => !collectedBounties.Contains(t));
        }
    }
}