using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterSystem : ModSystem
    {
        internal static List<CarpenterBounty> BountiesByID;
        internal static Dictionary<string, CarpenterBounty> BountiesByName;

        public static int BountyCount => BountiesByID.Count;

        public static void RegisterBounty(CarpenterBounty bounty)
        {
            if (BountiesByID == null || BountiesByName == null)
            {
                BountiesByID = new List<CarpenterBounty>();
                BountiesByName = new Dictionary<string, CarpenterBounty>();
            }

            string name = bounty.FullName;
            if (BountiesByName.ContainsKey(name))
                throw new System.Exception($"{bounty.Mod.Name} added two bounties with the same name ({bounty.Name})");
            bounty.Type = BountyCount;
            ModTypeLookup<CarpenterBounty>.Register(bounty);
            BountiesByID.Add(bounty);
            BountiesByName.Add(name, bounty);
        }

        public static CarpenterBounty GetBounty(int type)
        {
            return BountiesByID[type];
        }

        public static CarpenterBounty GetBounty(string mod, string name)
        {
            return BountiesByName[mod + "." + name];
        }

        public static CarpenterBounty GetBounty(Mod mod, string name)
        {
            return GetBounty(mod.Name, name);
        }

        public static CarpenterBounty GetBounty<T>() where T : CarpenterBounty
        {
            return ModContent.GetInstance<T>();
        }
    }
}