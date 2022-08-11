using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterSystem : ModSystem
    {
        internal static List<CarpenterBounty> BountiesByID;
        internal static Dictionary<string, CarpenterBounty> BountiesByName;

        public static Dictionary<int, bool> CraftableTileLookup { get; private set; }

        public static int BountyCount => BountiesByID.Count;

        public override void Load()
        {
            CraftableTileLookup = new Dictionary<int, bool>();
        }

        public override void Unload()
        {
            BountiesByID?.Clear();
            BountiesByName?.Clear();
            CraftableTileLookup?.Clear();
        }

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

        public static bool IsTileIDCraftable(int tileID)
        {
            if (CraftableTileLookup.TryGetValue(tileID, out var val))
            {
                return val;
            }

            foreach (var rec in Main.recipe.Where((r) => r != null && !r.Disabled && r.createItem != null && r.createItem.createTile == tileID))
            {
                foreach (var i in rec.requiredItem)
                {
                    foreach (var rec2 in Main.recipe.Where((r) => r != null && !r.Disabled && r.createItem != null && r.createItem.type == i.type))
                    {
                        foreach (var i2 in rec2.requiredItem)
                        {
                            if (i2.type == rec.createItem.type)
                            {
                                goto Continue;
                            }
                        }
                    }
                }
                CraftableTileLookup.Add(tileID, true);
                return true;

            Continue:
                continue;
            }
            CraftableTileLookup.Add(tileID, false);
            return false;
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