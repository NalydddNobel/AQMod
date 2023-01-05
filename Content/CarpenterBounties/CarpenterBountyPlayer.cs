using Aequus.Items.Accessories.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterBountyPlayer : ModPlayer
    {
        public List<string> completedBountiesOld { get; private set; }

        public override void Initialize()
        {
            completedBountiesOld = new List<string>();
        }

        public override void SaveData(TagCompound tag)
        {
        }

        public override void LoadData(TagCompound tag)
        {
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
            return clone;
        }

        public override void clientClone(ModPlayer clientClone)
        {
            var clone = (CarpenterBountyPlayer)clientClone;
            clone.completedBountiesOld = new List<string>(completedBountiesOld);
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

            if (Main.netMode == NetmodeID.SinglePlayer && completedBountiesOld.Count > 0)
            {
                CarpenterSystem.CompletedBounties.AddRange(completedBountiesOld);
                for (int i = 0; i < completedBountiesOld.Count; i++)
                    Main.NewText(completedBountiesOld[i]);
                completedBountiesOld?.Clear();
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var screenRectangle = new Rectangle((int)Player.Center.X / 16 - 20, (int)Player.Center.Y / 16 - 20, 40, 40);

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