using Aequus.Core.ContentGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Aequus.Old.Content.Tiles.Ambient.BigGems;

[LegacyName("BigGemsTile")]
public class BigGem : ModTile {
    public const int GEMS_NEEDED_FOR_RECIPE = 10;

    public const int Amethyst = 0;
    public const int Topaz = 1;
    public const int Sapphire = 2;
    public const int Emerald = 3;
    public const int Ruby = 4;
    public const int Diamond = 5;
    private const int MaxGemIDs = 6;

    private static readonly Color[] GemColor = new[] {
        Color.Purple,
        Color.Yellow,
        Color.Blue,
        Color.Green,
        Color.Red,
        Color.White,
    };
    public readonly List<GemInfo> _gems = new();

    public readonly record struct GemInfo(ModGore GemGore, ModItem GemItem, int SmallGemItem, int DustType, Color GemColor, LocalizedText DisplayName, bool Loaded = true);

    public override void Load() {
        List<FieldInfo> fields = this.GetConstantFields().Where(f => f.FieldType == typeof(int)).ToList();
        fields.Sort((f1, f2) => ((int)f1.GetValue(this)).CompareTo(f2.GetValue(this)));
        foreach (FieldInfo fieldInfo in fields) {
            int style = (int)fieldInfo.GetValue(this);
            string name = fieldInfo.Name;
            if (name.Contains('_')) {
                string[] split = name.Split('_');
                name = split.Last();
                // Cross Mod big gems, implement loading code for these when some are sprited.
                // The other mod's items may not be loaded when loading Big Gems, so extra logic must be done.
                // Also, ItemID.Search doesn't isn't populated with modded entries yet.
                continue;
            }

            AddVanillaBigGem(style, name);
        }

        void AddVanillaBigGem(int style, string name) {
            int vanillaGem = ItemID.Search.GetId(name);
            ModItem item = new InstancedTileItem(this, style,
                nameSuffix: name,
                dropItem: false,
                rarity: ItemRarityID.Blue,
                value: ContentSamples.ItemsByType[vanillaGem].value * GEMS_NEEDED_FOR_RECIPE,
                researchSacrificeCount: 5
            );
            ModGore gore = new BigGemGore(this, name);
            Mod.AddContent(item);
            Mod.AddContent(gore);
            ModTypeLookup<ModItem>.RegisterLegacyNames(item, $"{name}Deposit");

            Aequus.OnAddRecipes += () => {
                item.CreateRecipe()
                    .AddIngredient(vanillaGem, GEMS_NEEDED_FOR_RECIPE)
                    .AddTile(TileID.Solidifier)
                    .Register()
                    .SortBeforeFirstRecipesOf(ItemID.LargeAmethyst);
            };

            _gems.Add(new GemInfo(
                GemGore: gore,
                GemItem: item,
                SmallGemItem: vanillaGem,
                // Haha, this is really stupid.
                DustType: (short)typeof(DustID).GetField($"Gem{name}", BindingFlags.Public | BindingFlags.Static).GetValue(obj: null),
                GemColor: GemColor[style],
                DisplayName: this.GetLocalization($"{name}.ItemDisplayName")
            ));
        }
    }

    public override void SetStaticDefaults() {
        Main.tileFrameImportant[Type] = true;
        Main.tileLighted[Type] = true;
        Main.tileShine[Type] = 32;
        Main.tileShine2[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
        TileObjectData.newTile.CoordinateHeights = new[] { 16, 20, };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
        TileObjectData.addTile(Type);
        foreach (GemInfo gem in _gems) {
            AddMapEntry(gem.GemColor, gem.DisplayName);
        }
        HitSound = SoundID.Shatter;
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)Math.Clamp(Main.tile[i, j].TileFrameX / 36, 0, MaxGemIDs - 1);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.01f;
        g = 0.01f;
        b = 0.01f;
    }

    public override void RandomUpdate(int i, int j) {
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    private void ShatterGore<T>(int i, int j) where T : ModGore {
        Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16f + Main.rand.Next(16) - 8f, j * 16f + Main.rand.Next(10)), Main.rand.NextVector2Circular(2f, 2f), ModContent.GoreType<T>());
    }

    public override bool CreateDust(int i, int j, ref int type) {
        int style = Main.tile[i, j].TileFrameX / 36;
        if (Main.netMode == NetmodeID.Server || !_gems.IndexInRange(style)) {
            return true;
        }

        GemInfo gem = _gems[style];
        type = gem.DustType;
        Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16f + Main.rand.Next(16) - 8f, j * 16f + Main.rand.Next(10)), Main.rand.NextVector2Circular(2f, 2f), gem.GemGore.Type);
        return true;
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        int style = Main.tile[i, j].TileFrameX / 36;
        if (!_gems.IndexInRange(style)) {
            yield break;
        }

        GemInfo gem = _gems[style];
        yield return new Item(gem.SmallGemItem, stack: Main.rand.Next(GEMS_NEEDED_FOR_RECIPE / 2, GEMS_NEEDED_FOR_RECIPE + 1));
    }

    private bool ShouldDrawSnowTile(int left, int bottom) {
        return TileID.Sets.IcesSnow[Main.tile[left, bottom + 1].TileType] && TileID.Sets.IcesSnow[Main.tile[left + 1, bottom + 1].TileType];
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var tile = Main.tile[i, j];

        int style = Math.Clamp(tile.TileFrameX / 36, 0, _gems.Count - 1);
        GemInfo gem = _gems[style];

        int left = i - tile.TileFrameX % 36 / 18;
        int top = j - tile.TileFrameY / 18;
        bool drawSnow = ShouldDrawSnowTile(left, top + 1);

        FastRandom rand = new(left + top * left);
        rand.NextSeed();
        var texture = TextureAssets.Tile[Type].Value;
        var baseDrawCoords = new Vector2(i * 16f, j * 16f + 12f) - Main.screenPosition + TileHelper.DrawOffset;
        var drawCoords = new Vector2(baseDrawCoords.X, baseDrawCoords.Y - (drawSnow ? 14 : 12) - rand.Next(4) * 2f);
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 18, tile.TileFrameY > 0 ? 22 : 18);

        var lighting = Lighting.GetColor(i, j);
        float intensity = Math.Clamp((lighting.R + lighting.G + lighting.B) / 255f, 0.01f, 1f);
        intensity *= MathF.Pow(Helper.Oscillate((Main.GameUpdateCount / 360f + rand.NextFloat()) * MathHelper.TwoPi, 0.66f, 1f), 3f);
        //for (int k = 0; k < 4; k++) {
        //    spriteBatch.Draw(
        //        texture,
        //        drawCoords + (k * MathHelper.PiOver2).ToRotationVector2() * 2f,
        //        frame,
        //        Color.White.UseA(0) * intensity * Helper.Wave(Main.GlobalTimeWrappedHourly * 3f + k * MathHelper.PiOver2, 0.2f, 1f));
        //}

        if (tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0) {
            Texture2D bloom = AequusTextures.Bloom;
            Vector2 bloomOrigin = bloom.Size() / 2f;
            Color bloomColor = gem.GemColor with { A = 0 } * 0.5f * intensity;
            float scale = 0.66f;
            Vector2 drawCoordinates = baseDrawCoords + new Vector2(16f, 8f);
            Main.spriteBatch.Draw(bloom, drawCoordinates, null, bloomColor, 0f, bloomOrigin, scale, SpriteEffects.None, 0f);
        }

        if (tile.IsTileInvisible) {
            return true;
        }

        spriteBatch.Draw(
            texture,
            drawCoords,
            frame,
            lighting);

        if (ExtendedMod.GameWorldActive && Main.rand.NextBool(Main.tileShine[Type])) {
            Dust d = Dust.NewDustDirect(new Vector2(i, j) * 16f, 16, 16, DustID.TintableDustLighted, Alpha: 254, newColor: gem.GemColor, Scale: 0.5f);
            d.velocity *= 0f;
            d.noLight = true;
            d.noLightEmittence = true;
        }

        if (drawSnow && tile.TileFrameY > 0) {
            Main.instance.LoadTiles(TileID.SmallPiles);
            texture = TextureAssets.Tile[TileID.SmallPiles].Value;
            frame = new Rectangle(900 + 36 * rand.Next(0, 6) + tile.TileFrameX % 36, 18, 16, 16);
            lighting = ExtendLight.GetLightingSection(i - 1, j, 2, 1);

            spriteBatch.Draw(
                texture,
                new Vector2(baseDrawCoords.X, baseDrawCoords.Y - 10f),
                frame,
                lighting);
        }
        return false;
    }
}