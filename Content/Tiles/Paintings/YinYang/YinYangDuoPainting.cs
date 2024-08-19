using Aequus.Common.Utilities.Helpers;
using Aequus.Systems.Shimmer;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Paintings.YinYang;

public class YinYangDuoPainting : ModTile, IPainting {
    public static bool HardmodeCurse => Main.hardMode;

    public override string LocalizationCategory => "Tiles.Paintings";

    // When Hardmode occurs, all paintings will swap their styles.
    public static int GetRealStyle(int i, int j) {
        bool cursed = Main.tile[i, j].TileFrameX >= 54;

        if (HardmodeCurse) {
            cursed = !cursed;
        }

        return cursed ? 1 : 0;
    }

    public override void SetStaticDefaults() {
        new PaintingInitInfo() {
            Width = 3,
            Height = 2,
            CoordinateHeightsOverride = [16, 26]
        }.SetupPainting(this, TileObjectData.newTile);
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;

        LocalizedText mapEntry = CreateMapEntryName();
        AddMapEntry(new Color(215, 65, 225), mapEntry);
        AddMapEntry(new Color(145, 127, 139), mapEntry);

        Paintings.Instance.Sets.DungeonPictures.Add(this);
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        tileFrameX = (short)(GetRealStyle(i, j) * 54 + tileFrameX % 54);
        tileFrameY = (short)(tileFrameY % 46);
        //tileFrameY = (short)((Main.GameUpdateCount + tileFrameY) % 92);
    }

    public override IEnumerable<Item> GetItemDrops(int i, int j) {
        Item drop = new Item(ModContent.ItemType<YinYangDuoPaintingItem>());
        if (drop.ModItem is YinYangDuoPaintingItem modItem) {
            modItem.SetAltState(Main.tile[i, j].TileFrameX >= 54);
        }

        yield return drop;
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)GetRealStyle(i, j);
    }

    #region Painting Interface
    Mod IPainting.Mod => Mod;
    int IPainting.Width => 3;
    int IPainting.Height => 2;
    ushort IPainting.TileType => Type;
    int IPainting.ItemType => ModContent.ItemType<YinYangDuoPaintingItem>();
    #endregion
}

public class YinYangDuoPaintingItem : ModItem, IShimmerOverride {
    private bool _shimmerAlt;

    public bool IsCursed => _shimmerAlt ? !YinYangDuoPainting.HardmodeCurse : YinYangDuoPainting.HardmodeCurse;

    public int PlaceStyle => (YinYangDuoPainting.HardmodeCurse ? 2 : 0) + (_shimmerAlt ? 1 : 0);

    public override LocalizedText DisplayName => ModContent.GetInstance<YinYangDuoPainting>().CreateMapEntryName();
    public override LocalizedText Tooltip => ModContent.GetInstance<YinYangDuoPainting>().GetLocalization("Tooltip");

    public void SetAltState(bool value) {
        _shimmerAlt = value;
        Item.placeStyle = PlaceStyle;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<YinYangDuoPainting>(), PlaceStyle);
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 1);
    }

    // Remove tooltip if not cursed.
    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        if (!IsCursed) {
            tooltips.RemoveAll(t => t.Name.Equals("Tooltip0"));
        }
    }

    // Update place style constantly just in case.
    public override void HoldItem(Player player) {
        Item.placeStyle = PlaceStyle;
    }

    public override void UpdateInventory(Player player) {
        Item.placeStyle = PlaceStyle;
    }

    public override bool CanStack(Item source) {
        return _shimmerAlt == ((YinYangDuoPaintingItem)source.ModItem)._shimmerAlt;
    }

    private Texture2D GetTexture() {
        return IsCursed ? AequusTextures.YinYangDuoPaintingHardmodeItem : TextureAssets.Item[Type].Value;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        Texture2D texture = GetTexture();
        spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        Texture2D texture = GetTexture();
        Main.GetItemDrawFrame(Type, out _, out Rectangle frame);
        Vector2 drawCoordinates = Helper.WorldDrawPos(Item, frame);
        Color drawColor = LightingHelper.Get(Item.Center);
        Vector2 origin = frame.Size() / 2f;
        spriteBatch.Draw(texture, drawCoordinates, frame, drawColor, rotation, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    #region IO
    public override void NetSend(BinaryWriter writer) {
        writer.Write(_shimmerAlt);
    }

    public override void NetReceive(BinaryReader reader) {
        bool alt = reader.ReadBoolean();
        SetAltState(alt);
    }
    #endregion

    #region Shimmer
    bool? IShimmerOverride.CanShimmer(Item item, int type) {
        return YinYangDuoPainting.HardmodeCurse ? true : null;
    }

    bool IShimmerOverride.GetShimmered(Item item, int type) {
        if (!YinYangDuoPainting.HardmodeCurse) {
            return false;
        }

        _shimmerAlt = !_shimmerAlt;
        Shimmer.GetShimmered(item);
        return true;
    }
    #endregion
}
