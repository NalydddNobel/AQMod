using Aequus.Common.Structures;
using System;

namespace Aequus.Common.CodeGeneration;

internal partial class Gen {
    /// <summary>Generates a field with <paramref name="Name"/> in <see cref="AequusPlayer"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AequusPlayer_FieldAttribute<T>(string Name) : Attribute { }

    /// <summary><inheritdoc cref="AequusPlayer_FieldAttribute{T}"/> This field is reset in <see cref="AequusPlayer.ResetEffects"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AequusPlayer_ResetFieldAttribute<T>(string Name) : Attribute { }

    /// <summary><inheritdoc cref="AequusPlayer_FieldAttribute{T}"/> This field is reset in <see cref="AequusPlayer.ResetInfoAccessories"/> and copied to other AequusPlayer instances in <see cref="AequusPlayer.RefreshInfoAccessoriesFromTeamPlayers(Player)"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AequusPlayer_InfoFieldAttribute(string Name) : Attribute { }

    /// <summary><inheritdoc cref="AequusPlayer_FieldAttribute{T}"/> This field is Saved and Loaded through <see cref="AequusPlayer.SaveData(Terraria.ModLoader.IO.TagCompound)"/> and <see cref="AequusPlayer.LoadData(Terraria.ModLoader.IO.TagCompound)"/>.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    internal class AequusPlayer_SavedFieldAttribute<T>(string Name) : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.OnRespawn"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_OnRespawn : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.SetControls"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_SetControls : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.ResetEffects"/>-<see cref="AequusPlayer.ResetEffectsInner"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_ResetEffects : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.PostUpdateEquips"/>-<see cref="AequusPlayer.PostUpdateEquipsInner"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_PostUpdateEquips : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.ModifyHitNPCWithProj(Projectile, NPC, ref NPC.HitModifiers)"/>-<see cref="AequusPlayer.ModifyHitNPCInner(NPC, ref NPC.HitModifiers)"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_ModifyHitNPCWithProj : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.ModifyHitNPC(NPC, ref NPC.HitModifiers)"/>-<see cref="AequusPlayer.ModifyHitNPCInner(NPC, ref NPC.HitModifiers)"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_ModifyHitNPC : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.OnHitNPC(NPC, NPC.HitInfo, int)"/>-<see cref="AequusPlayer.OnHitNPCInner(NPC, NPC.HitInfo)"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_OnHitNPC : Attribute { }

    /// <summary>Adds a reference to the target method in <see cref="AequusPlayer.OnKillEffect(EnemyKillInfo)"/>-<see cref="AequusPlayer.OnKillNPCInner(EnemyKillInfo)"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_OnKillNPC : Attribute { }

    /// <summary>Adds a reference to the target method <see cref="AequusPlayer.OnBreakTileInner(int, int)"/>.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    internal class AequusPlayer_OnBreakTile : Attribute { }
}
