using Aequus.Common.Utilities;
using Aequus.Systems.Chests;
using System;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.Localization;

namespace Aequus.Content.Tiles.Paintings;

public sealed class Paintings : ModType {
    public readonly Color DefaultPaintingColor = new Color(120, 85, 60);
    public readonly LocalizedText DefaultPaintingName = Language.GetText("MapObject.Painting");

    public static Paintings Instance => ModContent.GetInstance<Paintings>();

    public PaintingSets Sets = new();

    public IPainting? Rockman { get; private set; }
    public IPainting? RockmanLemons { get; private set; }
    public IPainting? Yin { get; private set; }
    public IPainting? Yang { get; private set; }
    public IPainting? Narry { get; private set; }

    protected override void Register() {
        Rockman = New("Rockman", AequusTextures.RockmanPainting.FullPath, null,
            W: 2, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray,
            CustomName: ALanguage.L_GetItemName<global::Aequus.Items.Weapons.Melee.Swords.RockMan.RockMan>()
        ).AddEntry(Sets.DesertPictures);

        RockmanLemons = New("RockmanLemons", AequusTextures.RockmanLemonsPainting.FullPath, null,
            W: 6, H: 4,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.DesertPictures);

        Yin = New("Yin", AequusTextures.YinPainting.FullPath, null,
            W: 2, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.DesertPictures);

        Yang = New("Yang", AequusTextures.YangPainting.FullPath, null,
            W: 2, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.DesertPictures);

        Narry = New("Narry", AequusTextures.NarryPainting.FullPath, null,
            W: 2, H: 3,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.DungeonPictures);

        New("BongBong", AequusTextures.BongBongPainting.FullPath, null,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.GenericPictures);

        New("YinYang", AequusTextures.YinYangPainting.FullPath, null,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.DesertPictures);

        New("RockForce", AequusTextures.RockForce.FullPath, null,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.RockPictures);

        New("RockBalance", AequusTextures.RockBalance.FullPath, null,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.RockPictures);

        New("RockPush", AequusTextures.RockPush.FullPath, null,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.RockPictures);

        New("Oliver", AequusTextures.OliverPainting.FullPath, null,
            W: 3, H: 2,
            Rare: ItemRarityID.White,
            Value: Item.sellPrice(silver: 10),
            MapColor: Color.Gray
        ).AddEntry(Sets.GenericPictures);
    }

    InstancedPainting New(string name, string texture, string? ItemTexture, int W, int H, int Rare, int Value, Color? MapColor = null, Lazy<LocalizedText>? CustomName = null) {
        ItemTexture ??= $"{texture}Item";
        InstancedPainting nextPainting = new InstancedPainting(name, texture, ItemTexture, W, H, Rare, Value, MapColor, CustomName);
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
        ChestLootDatabase.Instance.RegisterIndexed(2, ChestPool.Sky, [
            //new CommonChestRule(ModContent.ItemType<ExLydSpacePainting>()),
            //new CommonChestRule(ModContent.ItemType<OmegaStaritePainting>()),
            //new CommonChestRule(ModContent.ItemType<OmegaStaritePainting2>()),
            //new CommonChestRule(ModContent.ItemType<HomeworldPainting>())
        ]);

        // Generic paintings which spawn in underground houses.
        //GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.BongBongPainting });
        //GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.OliverPainting });
        //GenericPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings6x4>(), style = WallPaintings6x4.BreadRoachPainting });

        // Paintings which spawn on rock walls.
        //RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.Fus });
        //RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.Ro });
        //RockPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.DAH });

        // Paintings which spawn in the Desert.
        //DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x2>(), style = WallPaintings3x2.YinYangPainting });
        //DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings2x2>(), style = WallPaintings2x2.YinPainting });
        //DesertPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings2x2>(), style = WallPaintings2x2.YangPainting });

        // Paintings which spawn in the Dungeon.
        //DungeonPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings2x3>(), style = WallPaintings2x3.NarryPainting });
        //DungeonPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.RockFromAnAlternateUniversePainting });

        // Paintings which spawn in the Underworld.
        //HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.GoreNestPainting });
        //HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.GoreNest2Painting });
        //HellPictures.Add(new PaintingEntry() with { tileType = ModContent.TileType<WallPaintings3x3>(), style = WallPaintings3x3.InsurgentPainting });

        SetStaticDefaults();
    }

    private PaintingEntry On_WorldGen_RandHellPicture(On_WorldGen.orig_RandHellPicture orig) {
        if (WorldGen.genRand.NextBool(5)) {
            return WorldGen.genRand.Next(Instance.Sets.HellPictures);
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandBonePicture(On_WorldGen.orig_RandBonePicture orig) {
        if (WorldGen.genRand.NextBool(3)) {
            return WorldGen.genRand.Next(Instance.Sets.DungeonPictures);
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandHousePictureDesert(On_WorldGen.orig_RandHousePictureDesert orig) {
        if (WorldGen.genRand.NextBool(3)) {
            return WorldGen.genRand.Next(Instance.Sets.DesertPictures);
        }

        return orig();
    }

    private static PaintingEntry On_WorldGen_RandHousePicture(On_WorldGen.orig_RandHousePicture orig) {
        if (WorldGen.genRand.NextBool(4)) {
            return WorldGen.genRand.Next(Instance.Sets.GenericPictures);
        }

        return orig();
    }
}

public class PaintingSets {
    public readonly List<PaintingEntry> RockPictures = [];

    public readonly List<PaintingEntry> GenericPictures = [];

    public readonly List<PaintingEntry> DesertPictures = [];

    public readonly List<PaintingEntry> DungeonPictures = [];

    public readonly List<PaintingEntry> HellPictures = [];
}