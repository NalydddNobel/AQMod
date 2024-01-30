using System.Collections.Generic;

namespace Aequus.Core;

public sealed class NewNPCCache : ILoadable {
    public static readonly List<NPC> NPCs = new();
    public static System.Boolean QueueNPCs { get; private set; }

    public void Load(Mod mod) {
        On_NPC.NewNPC += On_NPC_NewNPC;
        QueueNPCs = false;
    }

    private static System.Int32 On_NPC_NewNPC(On_NPC.orig_NewNPC orig, Terraria.DataStructures.IEntitySource source, System.Int32 X, System.Int32 Y, System.Int32 Type, System.Int32 Start, System.Single ai0, System.Single ai1, System.Single ai2, System.Single ai3, System.Int32 Target) {
        if (QueueNPCs) {
            var npc = new NPC();
            npc.SetDefaults(Type);
            npc.position.X = X - npc.width / 2f;
            npc.position.Y = Y - npc.height / 2f;
            npc.ai[0] = ai0;
            npc.ai[1] = ai1;
            npc.ai[2] = ai2;
            npc.ai[3] = ai3;
            npc.target = Target;
            NPCs.Add(npc);
            return Main.maxNPCs;
        }

        return orig(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
    }

    public void Unload() {
    }

    public static void Begin() {
        NPCs.Clear();
        QueueNPCs = true;
    }

    public static void End() {
        QueueNPCs = false;
    }
}