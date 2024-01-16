using System.Collections.Generic;

namespace Aequus.Content.Critters.HorseshoeCrab;

public class HorseshoeCrabInitializer : ILoadable {
    /// <summary>
    /// Item1 is the normal Horseshoe Crab. Item2 is the Golden variant.
    /// </summary>
    public static readonly List<(ModNPC, ModNPC)> HorseshoeCrabs = new(3);

    public void Load(Mod mod) {
        // Horseshoe crabs have blue blood
        int[] blueBloodDustId = new int[] { DustID.BlueMoss };
        int[] goldBloodDustId = new int[] { DustID.GoldCritter, DustID.GoldCritter_LessOutline };

        Add("Baby", 8);
        Add("Adult", 10);
        Add("Elder", 14);

        void Add(string name, int size) {
            HorseshoeCrab normal = new HorseshoeCrab(name, size, blueBloodDustId, golden: false);
            HorseshoeCrab golden = new HorseshoeCrab("Gold"+name, size, goldBloodDustId, golden: true);

            mod.AddContent(normal);
            mod.AddContent(golden);

            HorseshoeCrabs.Add((normal, golden));
        }
    }

    public void Unload() {
        HorseshoeCrabs.Clear();
    }
}