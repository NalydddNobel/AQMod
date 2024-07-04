using AequusRemake.Content.Critters.SeaFirefly;
using AequusRemake.Content.Tiles.CraftingStations.TrashCompactor;
using AequusRemake.Core.ContentGeneration;
using AequusRemake.Core.Graphics.Textures;
using System.Linq;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace AequusRemake.Content.Tiles.Misc.SeaFireflyBlock;

internal class SeaFireflyBlock(string NameSuffix, byte ColorId) : InstancedTile($"SeaFireflyBlock{NameSuffix}", AequusTextures.SeaFireflyBlock.Path) {
    public readonly byte _color = ColorId;
    public ISeaFireflyInstanceData Current => SeaFireflyRegistry.GetPalette(_color);

    public override void Load() {
        ModItem item = new SeaFireflyBlockItem(this);
        Mod.AddContent(item);
    }

    public override void SetStaticDefaults() {
        if (!Main.dedServ && Current.TileEffect != null) {
            Main.QueueMainThreadAction(() => TextureAssets.Tile[Type] = TextureGen.PerPixel(Current.TileEffect, TextureAssets.Tile[Type]));
        }

        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;
        TileID.Sets.CanPlaceNextToNonSolidTile[Type] = true;

        AddMapEntry(Current.GetBugColor());
        DustType = DustID.AncientLight;
        HitSound = SoundID.Dig;
        MineResist = 0.75f;
    }

    public virtual Color GetColor() {
        return Color.White;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        Vector3 rgb = Current.GetLightColor(new Vector2(i, j) * 16f);
        r = rgb.X;
        g = rgb.Y;
        b = rgb.Z;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (Main.rand.NextBool(90) && GameWorldActive) {
            ModContent.GetInstance<SeaFireflyBlockClusters>().New(new Point16(i, j));
        }
    }
}

internal class SeaFireflyBlockItem(SeaFireflyBlock block) : InstancedModItem(block.Name, block.Texture + "Item") {
    public readonly ModTile _parent = block;
    public readonly byte _color = block._color;
    public ISeaFireflyInstanceData Current => SeaFireflyRegistry.GetPalette(_color);

    public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.AequusRemake.Tiles.SeaFireflyBlock.DisplayName{Current.Name}", () => $"{Current.Name} Sea Firefly Block");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetStaticDefaults() {
        if (!Main.dedServ && Current.ColorEffect != null) {
            Main.QueueMainThreadAction(() => TextureAssets.Item[Type] = TextureGen.PerPixel(Current.ColorEffect, TextureAssets.Item[Type]));
        }

        foreach (var pair in ContentSamples.ItemsByType.Where(i => i.Value.makeNPC == ModContent.NPCType<SeaFirefly>())) {
            if (pair.Value.ModItem is SeaFireflyItem seaFireflyItem) {
                if (seaFireflyItem.Color != _color) {
                    continue;
                }
            }
            else if (_color != 0) {
                continue;
            }

            TrashCompactorRecipe.AddCustomRecipe(pair.Key, (Type, 10));
        }

        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(_parent.Type);
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(copper: 2);
    }
}
