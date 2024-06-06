using Aequus.Core.Graphics.Tiles;
using ReLogic.Content;
using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ObjectData;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequus.Core.ContentGeneration;

public abstract class UnifiedBoss(BossParams BossParams, TrophyParams Trophy = default) : ModNPC {
    public BossParams BossParams { get; private set; } = BossParams;
    private TrophyParams _trophies = Trophy;

    [CloneByReference]
    public ModItem TreasureBagItem { get; protected set; }

    [CloneByReference]
    public ModTile Trophy { get; protected set; }
    [CloneByReference]
    public ModItem TrophyItem { get; protected set; }

    [CloneByReference]
    public ModTile Relic { get; protected set; }
    [CloneByReference]
    public ModItem RelicItem { get; protected set; }

    [CloneByReference]
    public ModItem MaskItem { get; protected set; }

    public bool PreHardmode => BossParams.ItemRarity <= ItemRarityID.LightRed;

    public override void Load() {
        string itemPath = GetItemPath();

        if (TreasureBagItem == null) {
            LoadTreasureBag($"{itemPath}{Name}Bag");
        }

        if (Trophy == null) {
            LoadTrophy($"{itemPath}{Name}Trophy");
        }

        if (Relic == null) {
            RelicRenderer renderer = _trophies.Renderer ?? new RelicRenderer($"{itemPath}{Name}Relic");
            LoadRelic(renderer);
        }

        if (MaskItem == null) {
            LoadMask($"{itemPath}{Name}Mask");
        }
    }

    protected string GetItemPath() {
        return $"{this.NamespaceFilePath()}/Items/";
    }

    protected void LoadTreasureBag(string texturePath) {
        TreasureBagItem = new InstancedBossBag(this, BossParams.ItemRarity, PreHardmode);

        Mod.AddContent(TreasureBagItem);
    }

    protected void LoadTrophy(string texturePath) {
        Trophy = new InstancedTrophyTile(this, texturePath);

        TrophyItem = new InstancedTrophyItem(this, Trophy);

        Mod.AddContent(Trophy);
        Mod.AddContent(TrophyItem);
    }

    protected void LoadRelic(RelicRenderer renderer) {
        Relic = new InstancedRelicTile(this, renderer);

        RelicItem = new InstancedRelicItem(this, Relic, renderer);

        Mod.AddContent(Relic);
        Mod.AddContent(RelicItem);
    }

    protected void LoadMask(string texturePath) {
        MaskItem = new InstancedBossMask(this, texturePath);

        Mod.AddContent(MaskItem);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot) {
        if (RelicItem != null) {
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(RelicItem.Type));
        }

        if (TrophyItem != null) {
            npcLoot.Add(ItemDropRule.Common(TrophyItem.Type, chanceDenominator: 10));
        }

        if (TreasureBagItem != null) {
            npcLoot.Add(ItemDropRule.BossBag(TreasureBagItem.Type));
        }

        if (MaskItem != null) {
            npcLoot.AddBossDrop(ItemDropRule.Common(MaskItem.Type, chanceDenominator: 7), throwError: false);
        }
    }
}

internal class InstancedBossBag(ModNPC ModNPC, int InternalRarity, bool PreHardmode = false) : InstancedModItem($"{ModNPC.Name}Bag", $"{ModNPC.NamespaceFilePath()}/Items/{ModNPC.Name}Bag") {
    private readonly ModNPC _parent = ModNPC;
    private readonly int _rarity = InternalRarity;
    private readonly bool _preHardmode = PreHardmode;

    public override LocalizedText DisplayName => _parent.GetLocalization("TreasureBagDisplayName", () => $"Treasure Bag ({_parent.Name})");
    public override LocalizedText Tooltip => Language.GetText("CommonItemTooltip.RightClickToOpen");

    public override void SetStaticDefaults() {
        ItemSets.BossBag[Type] = true;
        ItemSets.PreHardmodeLikeBossBag[Type] = _preHardmode;

        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = _rarity;
        Item.maxStack = Item.CommonMaxStack;
        Item.expert = true;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.Lerp(lightColor, Color.White, 0.4f);
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(_parent.Type));
    }

    /*
    public override void PostUpdate() {
        Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

        if (Item.timeSinceItemSpawned % 12 == 0) {
            var center = Item.Center + new Vector2(0f, Item.height * -0.1f);

            var direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
            float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
            var velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

            var d = Dust.NewDustPerfect(center + direction * distance, DustID.SilverFlame, velocity);
            d.scale = 0.5f;
            d.fadeIn = 1.1f;
            d.noGravity = true;
            d.noLight = true;
            d.alpha = 0;
        }
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        var texture = TextureAssets.Item[Item.type].Value;

        Rectangle frame;

        if (Main.itemAnimations[Item.type] != null) {
            frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
        }
        else {
            frame = texture.Frame();
        }

        var origin = frame.Size() / 2f;
        var offset = new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
        var drawPos = Item.position - Main.screenPosition + origin + offset;

        float time = Main.GlobalTimeWrappedHourly;
        float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

        time %= 4f;
        time /= 2f;

        if (time >= 1f) {
            time = 2f - time;
        }

        time = time * 0.5f + 0.5f;

        for (float i = 0f; i < 1f; i += 0.25f) {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, origin, scale, SpriteEffects.None, 0);
        }

        for (float i = 0f; i < 1f; i += 0.34f) {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, origin, scale, SpriteEffects.None, 0);
        }

        return true;
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        Texture2D texture = TextureAssets.Item[Item.type].Value;

        float time = Main.GlobalTimeWrappedHourly;
        float timer = Main.GameUpdateCount / 240f + time * 0.04f;

        time %= 4f;
        time /= 2f;

        if (time >= 1f) {
            time = 2f - time;
        }

        time = time * 0.5f + 0.5f;

        for (float i = 0f; i < 1f; i += 0.25f) {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, position + new Vector2(0f, 8f).RotatedBy(radians) * time * Main.inventoryScale, frame, new Color(90, 70, 255, 50), 0f, origin, scale, SpriteEffects.None, 0);
        }

        for (float i = 0f; i < 1f; i += 0.34f) {
            float radians = (i + timer) * MathHelper.TwoPi;

            spriteBatch.Draw(texture, position + new Vector2(0f, 4f).RotatedBy(radians) * time * Main.inventoryScale, frame, new Color(140, 120, 255, 77), 0f, origin, scale, SpriteEffects.None, 0);
        }
        return true;
    }
    */
}

internal class InstancedTrophyTile(ModNPC modNPC, string texturePath) : InstancedModTile($"{modNPC.Name}Trophy", texturePath) {
    public override void SetStaticDefaults() {
        Main.tileSpelunker[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.StyleWrapLimit = 36;
        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        TileID.Sets.DisableSmartCursor[Type] = true;
        AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
    }
}

internal class InstancedTrophyItem(ModNPC modNPC, ModTile modTile) : InstancedTileItem(modTile, value: Item.sellPrice(gold: 1), rarity: ItemRarityID.Blue, journeyOverride: new JourneySortByTileId(TileID.Painting3X3)) {
    private readonly ModNPC _parentNPC = modNPC;

    public override LocalizedText DisplayName => _parentNPC.GetLocalization("TrophyDisplayName", () => $"{_parentNPC.Name} Trophy");
    public override LocalizedText Tooltip => LocalizedText.Empty;
}

internal class InstancedRelicTile(ModNPC modNPC, RelicRenderer renderer) : InstancedModTile(modNPC.Name, AequusTextures.Tile(TileID.MasterTrophyBase)), ISpecialTileRenderer {
    private readonly RelicRenderer _renderer = renderer;

    private const int FrameWidth = 18 * 3;
    private const int FrameHeight = 18 * 4;

    public override void SetStaticDefaults() {
        Main.tileShine[Type] = 400;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.InteractibleByNPCs[Type] = true;

        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.newTile.DrawYOffset = 2;
        TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
        TileObjectData.newTile.StyleHorizontal = false;
        TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
        TileObjectData.newTile.StyleMultiplier = 2;
        TileObjectData.newTile.StyleWrapLimit = 2;
        TileObjectData.newTile.styleLineSkipVisualOverride = 0;

        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
        TileObjectData.addAlternate(1);
        TileObjectData.addTile(Type);

        AdjTiles = [TileID.MasterTrophyBase,];

        AddMapEntry(new Color(233, 207, 94, 255), Language.GetText("MapObject.Relic"));
    }

    public override bool CreateDust(int i, int j, ref int type) {
        return false;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) {
        tileFrameX %= FrameWidth;
        tileFrameY %= FrameHeight * 2;
    }

    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
        if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) {
            SpecialTileRenderer.Add(i, j, TileRenderLayerID.PostDrawMasterRelics);
        }
    }

    void ISpecialTileRenderer.Render(int i, int j, byte layer) {
        Point p = new Point(i, j);
        Tile tile = Main.tile[p];
        if (!tile.HasTile) {
            return;
        }

        Rectangle relicFrame = new Rectangle(tile.TileFrameX, FrameHeight * 4, FrameWidth, FrameHeight);
        Vector2 origin = relicFrame.Size() / 2f;
        Vector2 worldCoordinates = p.ToWorldCoordinates(24f, 64f);
        Color relicColor = Lighting.GetColor(i, j);
        SpriteEffects relicEffects = tile.TileFrameY / FrameHeight != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 5f);
        Vector2 drawCoordinates = worldCoordinates - Main.screenPosition + new Vector2(0f, -44f) + new Vector2(0f, offset * 4f);

        _renderer.Draw(new(i, j, drawCoordinates, relicColor, relicEffects, offset));
    }
}

internal class InstancedRelicItem(ModNPC modNPC, ModTile modTile, RelicRenderer renderer) : InstancedTileItem(modTile, value: Item.sellPrice(gold: 1), rarity: ItemRarityID.Master, journeyOverride: new JourneySortByTileId(TileID.MasterTrophyBase)) {
    private readonly ModNPC _parentNPC = modNPC;
    private readonly RelicRenderer _renderer = renderer;

    public override string Texture => _renderer.TexturePath;

    public override LocalizedText DisplayName => _parentNPC.GetLocalization("RelicDisplayName", () => $"{_parentNPC.Name} Relic");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.width = 30;
        Item.height = 40;
        Item.master = true;
    }
}

[AutoloadEquip(EquipType.Head)]
internal class InstancedBossMask(ModNPC modNPC, string texturePath) : InstancedModItem($"{modNPC.Name}Mask", texturePath) {
    private readonly ModNPC _parent = modNPC;

    public override LocalizedText DisplayName => _parent.GetLocalization("MaskDisplayName", () => $"{_parent.Name} Mask");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 75);
        Item.vanity = true;
    }
}

public record struct BossParams(int ItemRarity);

public record struct TrophyParams(int? LegacyId = null, RelicRenderer Renderer = null);

public class RelicRenderer(string texture) {
    public readonly string TexturePath = texture;
    protected Asset<Texture2D> Texture { get; private set; }

    public void Draw(DrawParams drawInfo) {
        Texture ??= ModContent.Request<Texture2D>(TexturePath);
        DrawInner(in drawInfo);
    }

    protected virtual void DrawInner(in DrawParams drawInfo) {
        var frame = Texture.Value.Bounds;
        DrawWithGlow(Main.spriteBatch, Texture.Value, drawInfo.Position, frame, drawInfo.DrawColor, frame.Size() / 2f, drawInfo.SpriteEffects, drawInfo.Glow);
    }

    protected static void DrawWithGlow(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Rectangle frame, Color color, Vector2 origin, SpriteEffects effects, float glow) {
        //drawPos /= 4f;
        //drawPos.Floor();
        //drawPos *= 4f;

        spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

        float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3f + 0.7f;
        var effectColor = color with { A = 0 } * 0.1f * scale;
        for (float radian = 0f; radian < 1f; radian += MathHelper.PiOver2) {
            spriteBatch.Draw(texture, drawPos + radian.ToRotationVector2() * (6f + glow * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }

    public record struct DrawParams(int X, int Y, Vector2 Position, Color DrawColor, SpriteEffects SpriteEffects, float Glow);
}
