using System;
using System.Linq;
using System.Reflection;

namespace Aequus.Common.NPCs.Bestiary;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class ModBiomesAttribute : Attribute {
    public readonly Type[] _modBiomeTypes;
    public readonly int[] _modBiomeIds;

    public ModBiomesAttribute(params Type[] modBiomeTypes) {
        _modBiomeTypes = modBiomeTypes;
        _modBiomeIds = new int[modBiomeTypes.Length];
    }

    public void CacheIds() {
        int i = 0;
        try {
            for (; i < _modBiomeTypes.Length; i++) {
                _modBiomeIds[i] = ModContent.GetContent<ModBiome>().Where((mb) => mb.GetType().Equals(_modBiomeTypes[i])).First().Type;
            }
        }
        catch (Exception ex) {
            throw new Exception($"Could not find {_modBiomeTypes[i].FullName} as a ModBiome.", ex);
        }
    }
}

internal class ModBiomesGlobalNPC : GlobalNPC {
    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
        return npc.ModNPC != null && npc.ModNPC.GetType().GetCustomAttributes<ModBiomesAttribute>().Any();
    }

    public override void SetStaticDefaults() {
        foreach (var npc in Mod.GetContent<ModNPC>().Where((npc) => AppliesToEntity(npc.NPC, lateInstantiation: true))) {
            foreach (var attr in npc.GetType().GetCustomAttributes<ModBiomesAttribute>()) {
                attr.CacheIds();
            }
        }
    }

    public override void SetDefaults(NPC npc) {
        foreach (var attr in npc.ModNPC.GetType().GetCustomAttributes<ModBiomesAttribute>()) {
            int i;
            if (npc.ModNPC.SpawnModBiomes == null) {
                i = 0;
                npc.ModNPC.SpawnModBiomes = new int[attr._modBiomeTypes.Length];
            }
            else {
                var arr = npc.ModNPC.SpawnModBiomes;
                i = arr.Length;
                Array.Resize(ref arr, arr.Length + attr._modBiomeTypes.Length);
                npc.ModNPC.SpawnModBiomes = arr;
            }

            int k = 0;
            do {
                npc.ModNPC.SpawnModBiomes[i] = attr._modBiomeIds[k];
                i++;
                k++;
            }
            while (i < npc.ModNPC.SpawnModBiomes.Length && k < attr._modBiomeIds.Length);
        }
    }
}