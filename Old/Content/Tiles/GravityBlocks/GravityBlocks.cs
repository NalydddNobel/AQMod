using Aequu2.Core.ContentGeneration;
using Aequu2.Core.Graphics.Tiles;
using ReLogic.Content;
using System;
using Terraria.Audio;
using Terraria.Utilities;

namespace Aequu2.Old.Content.Tiles.GravityBlocks;

public class GravityBlocks : ModSystem {
    public static int MaximumSupportedReach { get; set; } = 24;

    private static int _checkItemGravity;

    private static InstancedGravityBlock _normalGravityBlock;
    private static InstancedGravityBlock _reverseGravityBlock;

    public static ModTile NormalGravityBlock => _normalGravityBlock;
    public static ModTile ReverseGravityBlock => _reverseGravityBlock;
    public static ModItem NormalGravityBlockItem => _normalGravityBlock.TileItem;
    public static ModItem ReverseGravityBlockItem => _reverseGravityBlock.TileItem;

    public static int GetGravity(Vector2 position, int width, int height) {
        int x = (int)((position.X + width / 2f) / 16f);
        int y = (int)((position.Y + height / 2f) / 16f);
        if (!WorldGen.InWorld(x, y) || !WorldGen.InWorld(x, y - MaximumSupportedReach) || !WorldGen.InWorld(x, y + MaximumSupportedReach)
            || !TileHelper.IsSectionLoaded(x, y) || !TileHelper.IsSectionLoaded(x, y - MaximumSupportedReach) || !TileHelper.IsSectionLoaded(x, y + MaximumSupportedReach)) {
            return 0;
        }

        for (int j = MaximumSupportedReach; j > -MaximumSupportedReach; j--) {
            Tile tile = Framing.GetTileSafely(x, y + j);
            if (tile.HasTile && TileLoader.GetTile(tile.TileType) is InstancedGravityBlock gravityBlock) {
                int gravity = gravityBlock.Data.Direction;
                if (Math.Sign(j) != Math.Sign(gravity)) {
                    continue;
                }
                int reach = Math.Min(gravityBlock.GetReach(x, y + j), MaximumSupportedReach);
                if (reach < Math.Abs(j)) {
                    continue;
                }
                return gravity;
            }
        }
        return 0;
    }

    public override void PreUpdateEntities() {
        if (Main.netMode == NetmodeID.MultiplayerClient || _checkItemGravity-- > 0) {
            return;
        }

        for (int i = 0; i < Main.maxItems; i++) {
            var item = Main.item[i];
            if (item.active && !item.IsAir && !ItemSets.ItemNoGravity[item.type] && item.TryGetGlobalItem<GravityBlocksGlobalItem>(out var gravItem)) {
                bool old = gravItem.HasReversedGravity;

                gravItem.HasReversedGravity = GetGravity(item.position, item.width, item.height) < 0;

                if (gravItem.HasReversedGravity != old) {
                    item.velocity.Y = -item.velocity.Y;
                    SoundEngine.PlaySound(SoundID.Item8, item.position);
                    if (Main.netMode == NetmodeID.Server) {
                        NetMessage.SendData(MessageID.SyncItem, number: i);
                    }
                }
            }
        }

        _checkItemGravity = 5;
    }

    public override void Load() {
        _normalGravityBlock = new InstancedGravityBlock("GravityBlock", Aequu2Textures.GravityBlock.Path,
            new InstancedGravityBlock.GravityBlockData(
                Reach: MaximumSupportedReach,
                Direction: 1,
                Color.Cyan,
                DustID.BlueCrystalShard
            ));
        _reverseGravityBlock = new InstancedGravityBlock("AntiGravityBlock", Aequu2Textures.AntiGravityBlock.Path,
            new InstancedGravityBlock.GravityBlockData(
                Reach: MaximumSupportedReach,
                Direction: -1,
                Color.Orange,
                DustID.OrangeStainedGlass
            ));

        Mod.AddContent(_normalGravityBlock);
        Mod.AddContent(_reverseGravityBlock);
    }

    public override void Unload() {
        _normalGravityBlock = null;
        _reverseGravityBlock = null;
    }
}

internal class InstancedGravityBlock : InstancedModTile, ISpecialTileRenderer {
    public readonly GravityBlockData Data;
    public ModItem TileItem { get; private set; }

    private Asset<Texture2D>[] _auras;
    private Asset<Texture2D> _dust;
    private readonly Vector3 _lightColor;

    public readonly record struct GravityBlockData(int Reach, int Direction, Color Color, int DustType);

    public InstancedGravityBlock(string name, string texture, GravityBlockData gravityBlock) : base(name, texture) {
        _lightColor = Vector3.Lerp(gravityBlock.Color.ToVector3(), new Vector3(1f, 1f, 1f), 0.1f);
        Data = gravityBlock;
    }

    public override void Load() {
        TileItem = new InstancedTileItem(this, rarity: ItemRarityID.Blue, value: Item.buyPrice(silver: 1));
        Mod.AddContent(TileItem);

        ModTypeLookup<ModTile>.RegisterLegacyNames(this, $"{Name}Tile");
        ModTypeLookup<ModTile>.RegisterLegacyNames(this, $"Force{Name}Tile");
        ModTypeLookup<ModItem>.RegisterLegacyNames(TileItem, $"Force{Name}");

        if (Main.netMode != NetmodeID.Server) {
            _auras = new Asset<Texture2D>[] {
                ModContent.Request<Texture2D>(Texture + "Aura_0"),
                ModContent.Request<Texture2D>(Texture + "Aura_1")
            };
            _dust = ModContent.Request<Texture2D>(Texture + "Dust");
        }
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        HitSound = SoundID.Tink;

        Main.tileLighted[Type] = true;
        DustType = Data.DustType;
        foreach (InstancedGravityBlock gravityBlock in Mod.GetContent<InstancedGravityBlock>()) {
            this.SetMerge(gravityBlock.Type);
        }
        AddMapEntry(Data.Color, CreateMapEntryName());
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = _lightColor.X;
        g = _lightColor.Y;
        b = _lightColor.Z;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].IsActuated || Main.tile[i, j].IsInvisible()) {
            return true;
        }

        Vector2 drawCoords = new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition + TileHelper.DrawOffset;
        int tileHeight = GetReach(i, j) - 1;
        if (tileHeight == 0) {
            return true;
        }

        SpecialTileRenderer.AddSolid(i, j, TileRenderLayerID.PostDrawWalls);
        int gravity = Math.Sign(Data.Direction);
        float rotation = gravity == 1 ? 0f : MathHelper.Pi;
        for (int k = 0; k < _auras.Length; k++) {
            var texture = TexturePainter.TryGetPaintedTexture(i, j, _auras[k].Name);
            Vector2 scale = new(1f, (tileHeight * 16f + 32f) / texture.Height);
            spriteBatch.Draw(
                texture,
                drawCoords,
                null,
                Color.White with { A = 0 } * 0.5f,
                rotation,
                new(texture.Width / 2f, texture.Height),
                scale,
                SpriteEffects.None, 0f
            );
        }
        return true;
    }

    protected void DrawParticles(int i, int j, int tileHeight, SpriteBatch spriteBatch) {
        ulong seed = Helper.TileSeed(i, j);
        FastRandom rand = new FastRandom(Helper.TileSeed(i, j));
        Texture2D texture = TexturePainter.TryGetPaintedTexture(i, j, _dust.Name);
        Vector2 origin = new Vector2(4f, 4f);
        Vector2 drawCoords = new Vector2(i * 16f, j * 16f + 8f) - Main.screenPosition;
        int dustAmt = (int)(rand.Next(tileHeight) / 1.5f + 2f);
        int gravity = Math.Sign(Data.Direction);

        for (int k = 0; k < dustAmt * 3; k++) {
            float p = rand.Next(50) + Main.GlobalTimeWrappedHourly * rand.NextFloat(2f, 5.2f);
            p %= 50f;
            p /= 50f;
            p = (float)Math.Pow(p, 3f);
            p *= 50f;
            p -= 2f;
            var frame = new Rectangle(0, 10 * rand.Next(3), 8, 8);
            var dustDrawOffset = new Vector2(Helper.Oscillate(Main.GlobalTimeWrappedHourly * rand.NextFloat(0.45f, 1f), 0f, 16f), tileHeight * 16f - p * 16f);
            float opacity = rand.NextFloat(0.1f, 1f);
            float scale = rand.NextFloat(0.25f, 0.7f);
            if (p > 0f && p < tileHeight) {
                if (p < 6f) {
                    float progress = p / 6f;
                    opacity *= progress;
                    scale *= progress;
                }
                spriteBatch.Draw(texture, drawCoords + dustDrawOffset with { Y = dustDrawOffset.Y * -gravity }, frame,
                    Color.White with { A = 0 } * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }

    protected virtual void OnSpecialRender(int i, int j, byte layer) {
        DrawParticles(i, j, GetReach(i, j), Main.spriteBatch);
    }

    void ISpecialTileRenderer.Render(int i, int j, byte layer) {
        OnSpecialRender(i, j, layer);
    }

    public int GetReach(int i, int j) {
        int gravity = -Data.Direction;
        for (int l = 1; l < Data.Reach; l++) {
            if (Main.tile[i, j + l * gravity].IsSolid()) {
                return l;
            }
        }

        return Data.Reach;
    }
}