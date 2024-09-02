using Aequus.Common.ContentTemplates.Generic;
using Aequus.Common.Drawing.Generative;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.Tombstones;
public abstract class UnifiedTombstones : ModTile {
    public abstract int StyleCount { get; }

    public readonly ModItem[] Items;
    public readonly ModProjectile[] Projectiles;

    public UnifiedTombstones() {
        Items = new ModItem[StyleCount];
        Projectiles = new ModProjectile[StyleCount];
    }

    public abstract TombstoneOverride? OverrideTombstoneDrop(Player player, bool gold, long coinsOwned);

    protected virtual ModItem CreateItem(int style) {
        return new InstancedTombstoneItem(this, style);
    }

    protected virtual ModProjectile CreateProjectile(int style) {
        return new InstancedTombstoneProjectile(this, style);
    }

    public override void Load() {
        for (int i = 0; i < StyleCount; i++) {
            ModItem nextItem = Items[i] = CreateItem(i);
            ModProjectile nextProj = Projectiles[i] = CreateProjectile(i);

            Mod.AddContent(nextItem);
            Mod.AddContent(nextProj);
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
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        AdjTiles = [TileID.Tombstones];
        DustType = DustID.Ash;

        if (!Main.dedServ) {
            Main.QueueMainThreadAction(SetTextures);
        }
    }

    void SetTextures() {
        AtlasInfo info = new AtlasInfo(16, 2, [16, 18], 2);

        for (int i = 0; i < StyleCount; i++) {
            ITextureGenerator generator = new EffectAtlasMerge(36 * i, 0, 2, 2, info);
            SetStyleTexture(i, generator, in info);
        }
    }

    protected virtual void SetStyleTexture(int style, ITextureGenerator generator, in AtlasInfo info) {
        var texture = TextureGen.New(generator, TextureAssets.Tile[Type]);

        int itemId = Items[style].Type;
        int projId = Projectiles[style].Type;

        TextureAssets.Projectile[projId] = texture;
        TextureAssets.Item[itemId] = texture;
    }
}

public record struct TombstoneOverride(int ProjType, string? TombstoneText);

internal class InstancedTombstoneItem(UnifiedTombstones parent, int style) : InstancedModItem($"Tombstone{style}", parent.Texture) {
    public override LocalizedText DisplayName => parent.GetLocalization($"MapEntry.{style}");
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.Tombstone);
        Item.createTile = parent.Type;
        Item.placeStyle = style;
    }
}

internal class InstancedTombstoneProjectile(UnifiedTombstones parent, int style) : InstancedProjectile($"Tombstone{style}", parent.Texture) {
    public override LocalizedText DisplayName => parent.GetLocalization($"MapEntry.{style}");

    public override void SetDefaults() {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.knockBack = 12f;
    }

    public override void AI() {
        if (Projectile.velocity.Y == 0f) {
            Projectile.velocity.X *= 0.98f;
        }
        Projectile.rotation += Projectile.velocity.X * 0.1f;
        Projectile.velocity.Y += 0.2f;
        if (Projectile.owner != Main.myPlayer || Projectile.ai[0] > 0f) {
            return;
        }
        int x = (int)((Projectile.position.X + Projectile.width / 2) / 16f);
        int y = (int)((Projectile.position.Y + Projectile.height - 4f) / 16f);
        if (Main.tile[x, y] == null || Main.tile[x, y].HasTile) {
            return;
        }
        WorldGen.PlaceTile(x, y, parent.Type, mute: false, forced: false, Projectile.owner, style: style);
        if (Main.tile[x, y].HasTile) {
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 1, x, y, parent.Type, style);
            }
            int sign = Sign.ReadSign(x, y);
            if (sign >= 0) {
                Sign.TextSign(sign, Projectile.miscText);
            }
            Projectile.Kill();
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Projectile.velocity.X != oldVelocity.X) {
            Projectile.velocity.X = oldVelocity.X * -0.75f;
        }
        if (Projectile.velocity.Y != oldVelocity.Y && Projectile.velocity.Y > 1.5) {
            Projectile.velocity.Y = oldVelocity.Y * -0.7f;
        }
        Projectile.netUpdate = true;
        return false;
    }
}