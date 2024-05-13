global using Aequus.Core.Utilities;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;

#region Aliases
global using BestiaryTimeTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times;
global using BestiaryInvasionTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions;
global using BestiaryBiomeTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes;
global using BestiaryEventTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events;
global using BestiaryVisualsTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals;
global using BestiaryFilters = Terraria.GameContent.Bestiary.Filters;
global using SysColor = System.Drawing.Color;
global using SysVector2 = System.Numerics.Vector2;
global using SysVector3 = System.Numerics.Vector3;
global using SysVector4 = System.Numerics.Vector4;
global using ILoad = Terraria.ModLoader.ILoadable;
global using ItemSets = Terraria.ID.ItemID.Sets;
global using NPCSets = Terraria.ID.NPCID.Sets;
global using BuffSets = Terraria.ID.BuffID.Sets;
global using AmmoSets = Terraria.ID.AmmoID.Sets;
global using HelmetEquipSets = Terraria.ID.ArmorIDs.Head.Sets;
global using BodyEquipSets = Terraria.ID.ArmorIDs.Body.Sets;
global using LegEquipSets = Terraria.ID.ArmorIDs.Legs.Sets;
global using BackEquipSets = Terraria.ID.ArmorIDs.Back.Sets;
global using BalloonEquipSets = Terraria.ID.ArmorIDs.Balloon.Sets;
global using BeardEquipSets = Terraria.ID.ArmorIDs.Beard.Sets;
global using ShoeEquipSets = Terraria.ID.ArmorIDs.Shoe.Sets;
global using WaistEquipSets = Terraria.ID.ArmorIDs.Waist.Sets;
global using WingEquipSets = Terraria.ID.ArmorIDs.Wing.Sets;
global using GoreSets = Terraria.ID.GoreID.Sets;
global using HairSets = Terraria.ID.HairID.Sets;
#endregion

using Terraria.Localization;
using Terraria.Utilities;
using tModLoaderExtended;

namespace Aequus;

public partial class Aequus : ExtendedMod {
    public static new Mod Instance => ExtendedMod.Instance;
    public static Mod MusicMod { get; private set; }

    public override void OnLoad() {
        MusicMod = ModLoader.GetMod("AequusMusic");
        LoadModCalls();
    }

    public override void OnUnload() {
        MusicMod = null;
        UnloadLoadingSteps();
        UnloadModCalls();
    }

    /// <returns>A random name. Used in naturally generated underworld tombstones.</returns>
    public static LocalizedText GetRandomName(UnifiedRandom random = null) {
        string filter = "Mods.Aequus.Names";
        return Language.SelectRandom((key, value) => key.StartsWith(filter), random);
    }
}