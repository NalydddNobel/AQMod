using System;

namespace Aequus.Common.CodeGeneration;

internal partial class Gen {
    /// <summary>Adds a reference to the target method in <see cref="AequusNPC.OnSpawn(NPC, Terraria.DataStructures.IEntitySource)"/> (<see cref="AequusNPC.OnSpawnInner(NPC, Terraria.DataStructures.IEntitySource)"/>).</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusNPC_OnSpawn : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusNPC.ModifyNPCLoot(NPC, NPCLoot)"/> (<see cref="AequusNPC.ModifyNPCLootInner(NPC, NPCLoot)"/>).</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusNPC_ModifyNPCLoot : Attribute { }
}
