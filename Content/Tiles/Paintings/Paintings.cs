using Aequus.Common.Utilities;
using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.Generation;
using Terraria.Localization;

namespace Aequus.Content.Tiles.Paintings;

public sealed class Paintings : ModType, ILocalizedModType {
    public readonly Color DefaultPaintingColor = new Color(120, 85, 60);
    public readonly LocalizedText DefaultPaintingName = Language.GetText("MapObject.Painting");

    public static Paintings Instance => ModContent.GetInstance<Paintings>();

    public PaintingSets Sets = new();

    string ILocalizedModType.LocalizationCategory => "Tiles";

    public LocalizedText SignatureNalyddd => this.GetLocalization("Signature.Nalyddd");

    protected override void Register() {
        New("Rockman", AequusTextures.RockmanPainting.FullPath,
            W: 2, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            CustomName: ALanguage.L_GetItemName<global::Aequus.Items.Weapons.Melee.Swords.RockMan.RockMan>(),
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.DesertPictures);

        New("RockmanLemons", AequusTextures.RockmanLemonsPainting.FullPath,
            W: 6, H: 4,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.DesertPictures);

        New("Yin", AequusTextures.YinPainting.FullPath,
            W: 2, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.DesertPictures)
        .RegisterLegacy(0);

        New("Yang", AequusTextures.YangPainting.FullPath,
            W: 2, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.DesertPictures)
        .RegisterLegacy(1);

        New("Narry", AequusTextures.NarryPainting.FullPath,
            W: 2, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.DungeonPictures)
        .RegisterLegacy(0);

        New("BongBong", AequusTextures.BongBongPainting.FullPath,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.GenericPictures)
        .RegisterLegacy(0)
        .RegisterLegacy(1);

        New("YinYang", AequusTextures.YinYangPainting.FullPath,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.DesertPictures)
        .RegisterLegacy(2);

        New("RockForce", AequusTextures.RockForce.FullPath,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.RockPictures)
        .RegisterLegacy(3);

        New("RockBalance", AequusTextures.RockBalance.FullPath,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.RockPictures)
        .RegisterLegacy(4);

        New("RockPush", AequusTextures.RockPush.FullPath,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.RockPictures)
        .RegisterLegacy(5);

        New("Oliver", AequusTextures.OliverPainting.FullPath,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.GenericPictures)
        .RegisterLegacy(6);

        global::Aequus.NPCs.BossMonsters.OmegaStarite.OmegaStarite.NoHitItem =
        New("Origin", AequusTextures.OriginPainting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.LightRed,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).RegisterLegacy(0).ItemType;

        New("Catalyst", AequusTextures.CatalystPainting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.GenericPictures)
        .RegisterLegacy(1);

        New("OmegaStarite2", AequusTextures.OmegaStarite2Painting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.SkyPictures)
        .RegisterLegacy(2);

        New("GoreNest", AequusTextures.GoreNestPainting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.HellPictures)
        .RegisterLegacy(3);

        New("Space", AequusTextures.SpacePainting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.SkyPictures)
        .RegisterLegacy(4);

        New("GoreNest2", AequusTextures.GoreNest2Painting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.HellPictures)
        .RegisterLegacy(5);

        New("Insurgent", AequusTextures.InsurgentPainting.FullPath,
            W: 3, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            Tooltip: SignatureNalyddd
        ).AddEntry(Sets.HellPictures)
        .RegisterLegacy(6);
    }

    InstancedPainting New(string name, string texture, int W, int H, int Rare, int Value, LocalizedText? Tooltip = null, string? ItemTexture = null, Color? MapColor = null, Lazy<LocalizedText>? CustomName = null) {
        ItemTexture ??= $"{texture}Item";
        InstancedPainting nextPainting = new InstancedPainting(name, texture, ItemTexture, W, H, Rare, Value, Tooltip, MapColor, CustomName);
        Mod.AddContent(nextPainting);
        return nextPainting;
    }

    public override void Load() {
        On_WorldGen.RandHousePicture += On_WorldGen_RandHousePicture;
        On_WorldGen.RandHousePictureDesert += On_WorldGen_RandHousePictureDesert;
        On_WorldGen.RandBonePicture += On_WorldGen_RandBonePicture;
        On_WorldGen.RandHellPicture += On_WorldGen_RandHellPicture;
    }

    public sealed override void SetupContent() {
        // Add sky paintings to skyware chest loot pool.
        ChestLootDatabase.Instance.RegisterIndexed(1, ChestPool.Sky, Sets.SkyPictures.Select((p) => new CommonChestRule(p.ItemType)).ToArray());

        // Generic paintings which spawn in underground houses.
        //GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x2.BongBongPainting });
        //GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x2.OliverPainting });
        //GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings6x4>(), style = WallPaintings6x4.BreadRoachPainting });

        // Paintings which spawn on rock walls.
        //RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x2.Fus });
        //RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x2.Ro });
        //RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x2.DAH });

        // Paintings which spawn in the Desert.
        //DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x2.YinYangPainting });
        //DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings2x2.YinPainting });
        //DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings2x2.YangPainting });

        // Paintings which spawn in the Dungeon.
        //DungeonPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings2x3.NarryPainting });
        //DungeonPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x3.RockFromAnAlternateUniversePainting });

        // Paintings which spawn in the Underworld.
        //HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x3.GoreNestPainting });
        //HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x3.GoreNest2Painting });
        //HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType(), style = WallPaintings3x3.InsurgentPainting });

        SetStaticDefaults();
    }

    private PaintingEntry On_WorldGen_RandHellPicture(On_WorldGen.orig_RandHellPicture orig) {
        if (WorldGen.genRand.NextBool(5)) {
            return WorldGen.genRand.Next(Instance.Sets.HellPictures).ToEntry();
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandBonePicture(On_WorldGen.orig_RandBonePicture orig) {
        if (WorldGen.genRand.NextBool(3)) {
            return WorldGen.genRand.Next(Instance.Sets.DungeonPictures).ToEntry();
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandHousePictureDesert(On_WorldGen.orig_RandHousePictureDesert orig) {
        if (WorldGen.genRand.NextBool(3)) {
            return WorldGen.genRand.Next(Instance.Sets.DesertPictures).ToEntry();
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandHousePicture(On_WorldGen.orig_RandHousePicture orig) {
        if (WorldGen.genRand.NextBool(4)) {
            return WorldGen.genRand.Next(Instance.Sets.GenericPictures).ToEntry();
        }

        return orig();
    }
}

public class PaintingSets {
    public readonly List<IPainting> RockPictures = [];

    public readonly List<IPainting> GenericPictures = [];

    public readonly List<IPainting> DesertPictures = [];

    public readonly List<IPainting> DungeonPictures = [];

    public readonly List<IPainting> SkyPictures = [];

    public readonly List<IPainting> HellPictures = [];
}