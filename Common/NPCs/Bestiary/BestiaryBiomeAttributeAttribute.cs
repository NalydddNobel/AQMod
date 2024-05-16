using System;

namespace Aequus.Common.NPCs.Bestiary;

internal interface IModBiomesAttribute {
    ModBiome GetModBiome();
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class BestiaryBiomeAttribute<T> : Attribute, IModBiomesAttribute where T : ModBiome {
    public ModBiome GetModBiome() {
        return ModContent.GetInstance<T>();
    }
}

internal class ModBiomesGlobalNPC : GlobalNPC {
    public override bool AppliesToEntity(NPC npc, bool lateInstantiation) {
        return npc.ModNPC != null && Attribute.IsDefined(npc.ModNPC.GetType(), typeof(BestiaryBiomeAttribute<>));
    }

    public override void SetDefaults(NPC npc) {
        if (npc.ModNPC == null) {
            return;
        }

        int[] modBiomeIndices = [];
        foreach (Attribute attr in Attribute.GetCustomAttributes(npc.ModNPC.GetType(), typeof(BestiaryBiomeAttribute<>))) {
            if (attr is not IModBiomesAttribute modBiomeAttribute) {
                continue;
            }

            Array.Resize(ref modBiomeIndices, modBiomeIndices.Length + 1);
            modBiomeIndices[^1] = modBiomeAttribute.GetModBiome().Type;
        }
        npc.ModNPC.SpawnModBiomes = modBiomeIndices;
    }
}