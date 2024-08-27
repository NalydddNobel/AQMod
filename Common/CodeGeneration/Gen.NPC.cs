using System;

namespace Aequus.Common.CodeGeneration;

internal partial class Gen {
    /// <summary>Adds a reference to the target method in <see cref="AequusNPC"/> (<see cref="AequusNPC.OnSpawnInner(NPC, IEntitySource)"/>).</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusNPC_OnSpawn : Attribute { }
}
