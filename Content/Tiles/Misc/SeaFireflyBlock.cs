using Aequus.Content.Critters.SeaFirefly;
using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Graphics.Textures;
using Aequus.Core.Particles;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;

namespace Aequus.Content.Tiles.Misc;

internal class SeaFireflyBlock(string NameSuffix, byte ColorId) : InstancedTile($"SeaFireflyBlock{NameSuffix}", AequusTextures.SeaFireflyBlock.Path) {
    public readonly byte _color = ColorId;
    public ISeaFireflyInstanceData Current => SeaFirefly.GetPalette(_color);

    public override void Load() {
        ModItem item = new SeaFireflyBlockItem(this);
        Mod.AddContent(item);
    }

    public override void SetStaticDefaults() {
        if (!Main.dedServ && Current.ColorEffect != null) {
            Main.QueueMainThreadAction(() => TextureAssets.Tile[Type] = TextureGen.PerPixel(Current.ColorEffect, TextureAssets.Tile[Type]));
        }

        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;

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
        if (!Main.rand.NextBool(16)) {
            return;
        }

        Vector2 spawnCoordinates = new Vector2(i, j).ToWorldCoordinates() + Main.rand.NextVector2Circular(32f, 32f);

        if (!Collision.WetCollision(spawnCoordinates, 0, 0)) {
            return;
        }

        Particle<SeaFireflyClusters.Particle>.New().Setup(spawnCoordinates, (byte)Main.rand.Next(1, 3), (int)Main.GameUpdateCount, _color);
    }
}

internal class SeaFireflyBlockItem(SeaFireflyBlock block) : InstancedModItem(block.Name, block.Texture + "Item") {
    public readonly ModTile _parent = block;
    public readonly byte _color = block._color;
    public ISeaFireflyInstanceData Current => SeaFirefly.GetPalette(_color);

    public override LocalizedText DisplayName => Language.GetOrRegister($"Mods.Aequus.Tiles.SeaFireflyBlock.DisplayName{Current.Name}", () => $"{Current.Name} Sea Firefly Block");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetStaticDefaults() {
        if (!Main.dedServ && Current.ColorEffect != null) {
            Main.QueueMainThreadAction(() => TextureAssets.Item[Type] = TextureGen.PerPixel(Current.ColorEffect, TextureAssets.Item[Type]));
        }

        foreach (SeaFireflyItem item in ModContent.GetContent<SeaFireflyItem>()) {
            if (item.Color == _color) {
                TrashCompactorRecipe.AddCustomRecipe(item.Type, (Type, 10));
            }
        }

        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(_parent.Type);
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(copper: 2);
    }
}
