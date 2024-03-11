using Aequus.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Tombstones;

[LegacyName("AshTombstones", "AshTombstonesTile")]
public class Tombstones : ModTile {
    public const int STYLE_ASH_TOMBSTONE = 0;
    public const int STYLE_ASH_GRAVE_MARKER = 1;
    public const int STYLE_ASH_CROSS_MARKER = 2;
    public const int STYLE_ASH_HEADSTONE = 3;
    public const int STYLE_ASH_GRAVESTONE = 4;
    public const int STYLE_ASH_OBELISK = 5;

    public const int STYLE_GOLD_ASH_YIN = 6;
    public const int STYLE_GOLD_ASH_TOMBSTONE = 7;
    public const int STYLE_GOLD_ASH_IMP = 8;
    public const int STYLE_GOLD_ASH_QUARTZ = 9;
    public const int STYLE_GOLD_ASH_FIST = 10;

    public const int STYLE_COUNT = 11;

    public static readonly Color COLOR_ASH = new Color(101, 90, 102);
    public static readonly Color COLOR_GOLD_ASH = new Color(214, 36, 36);

    private readonly List<TombstoneSet> _tombstones = new();
    private readonly List<TombstoneDropGroup> _tombstoneGroups = new();

    public override void Load() {
        RegisterGroup(new TombstoneDropGroup(
            Regular: new TombstoneSet(STYLE_ASH_TOMBSTONE, STYLE_ASH_OBELISK, COLOR_ASH, "Ash"),
            Gold: new TombstoneSet(STYLE_GOLD_ASH_YIN, STYLE_GOLD_ASH_FIST, COLOR_GOLD_ASH, "AshGold")
        ));

        foreach (var t in _tombstones) {
            t.LoadItems(this);
        }

        void RegisterGroup(TombstoneDropGroup group) {
            _tombstoneGroups.Add(group);
            _tombstones.Add(group.Regular);
            _tombstones.Add(group.Gold);
        }
    }

    public override void SetStaticDefaults() {
        Main.tileSign[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.HasOutlines[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        DustType = 37;
        AdjTiles = new int[] { TileID.Tombstones };

        foreach (var g in _tombstones) {
            g.AddMapEntries(this);
        }
    }

    public override void Unload() {
        _tombstoneGroups.Clear();
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        switch (Main.tile[i, j].TileFrameX / 36) {
            // Ash Tombstones color
            case < STYLE_ASH_OBELISK: {
                    r = 0.6f;
                    g = 0.3f;
                    b = 0.1f;
                }
                break;
            // Gold Ash Tombstones color
            case < STYLE_GOLD_ASH_FIST: {
                    r = 0.5f;
                    g = 0.1f;
                    b = 0.1f;
                }
                break;
        }
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)Math.Clamp(Main.tile[i, j].TileFrameX / 36, 0, STYLE_COUNT);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        return true;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        Vector2 drawCoordinates = new Vector2(i, j) * 16f - Main.screenPosition + TileHelper.DrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        spriteBatch.Draw(AequusTextures.Tombstones_Glow.Value, drawCoordinates, frame, Color.White * Helper.Oscillate(Main.GlobalTimeWrappedHourly, 0.5f, 0.8f));
    }

    public record class TombstoneDropGroup(TombstoneSet Regular, TombstoneSet Gold);
    public record class TombstoneSet(int StartStyle, int EndStyle, Color MapColor, string Folder) {
        internal void LoadItems(ModTile m) {
            int k = StartStyle;
            int i = 0;
            do {
                m.Mod.AddContent(new TombstoneItem(m, k++, i++, Folder));
            }
            while (k <= EndStyle);
        }
        internal void AddMapEntries(ModTile m) {
            for (int i = StartStyle; i <= EndStyle; i++) {
                m.AddMapEntry(MapColor, m.GetLocalization($"MapEntry.{i}"));
            }
        }
    }
    private class TombstoneItem : InstancedModItem {
        private readonly ModTile _parent;
        private readonly int _style;

        public TombstoneItem(ModTile parent, int style, int textureId, string folder) : base($"Tombstone{style}", $"{typeof(Tombstones).NamespaceFilePath()}/{folder}/{folder}Tombstone{textureId}") {
            _parent = parent;
            _style = style;
        }

        public override LocalizedText DisplayName => _parent.GetLocalization($"MapEntry.{_style}");
        public override LocalizedText Tooltip => LocalizedText.Empty;

        public override void Load() {
            if (_legacyNames.TryGetValue(_style, out string legacyName)) {
                ModTypeLookup<ModItem>.RegisterLegacyNames(this, legacyName);
            }
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Tombstone);
            Item.createTile = ModContent.TileType<Tombstones>();
            Item.placeStyle = _style;
        }

        private static readonly Dictionary<int, string> _legacyNames = new() {
            [STYLE_ASH_TOMBSTONE] = "AshTombstone",
            [STYLE_ASH_GRAVE_MARKER] = "AshGraveMarker",
            [STYLE_ASH_CROSS_MARKER] = "AshCrossGraveMarker",
            [STYLE_ASH_HEADSTONE] = "AshHeadstone",
            [STYLE_ASH_GRAVESTONE] = "AshGravestone",
            [STYLE_ASH_OBELISK] = "AshObelisk",
        };
    }
}
