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

using log4net;
using System.Reflection;
using tModLoaderExtended.Terraria;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using Terraria.Utilities;

namespace Aequus;

public partial class Aequus : Mod {
    public static Aequus Instance { get; private set; }
    public static Mod MusicMod { get; private set; }

    /// <summary>Shorthand for <see cref="Instance"/>.Logger.</summary>
    public static ILog Log => Instance.Logger;

    /// <summary>Shorthand for typeof(<see cref="Main"/>).Assembly.</summary>
    public static Assembly TerrariaAssembly => typeof(Main).Assembly;

    /// <summary>Shorthand for checking if <see cref="Main.gfxQuality"/> is greater than 0.</summary>
    public static bool HighQualityEffects => Main.gfxQuality > 0f;

    /// <summary>Shorthand for checking if <see cref="Lighting.NotRetro"/> is true, and <see cref="FilterManager.CanCapture"/>.</summary>
    public static bool ScreenShadersActive => Lighting.NotRetro && Filters.Scene.CanCapture();

    /// <summary>Shorthand for a bunch of checks determining whether the game is unpaused.</summary>
    public static bool GameWorldActive => (Main.instance.IsActive && !Main.gamePaused && !Main.gameInactive) || Main.netMode != NetmodeID.SinglePlayer;

    public override void Load() {
        Instance = this;
        MusicMod = ModLoader.GetMod("AequusMusic");
        new ModBind(this);
        LoadModCalls();
    }

    public override void Unload() {
        Instance = null;
        MusicMod = null;
        UnloadLoadingSteps();
        UnloadModCalls();
        UnloadPackets();
    }

    /// <returns>A random name. Used in naturally generated underworld tombstones.</returns>
    public static LocalizedText GetRandomName(UnifiedRandom random = null) {
        string filter = "Mods.Aequus.Names";
        return Language.SelectRandom((key, value) => key.StartsWith(filter), random);
    }
}