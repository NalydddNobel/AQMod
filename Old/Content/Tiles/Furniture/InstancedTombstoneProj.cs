using Aequus.Common.Projectiles;
using Terraria.Audio;

namespace Aequus.Old.Content.Tiles.Furniture;

internal class InstancedTombstoneProj : InstancedModProjectile {
    [CloneByReference]
    public readonly ModTile Tile;

    public readonly int Style;

    protected InstancedTombstoneProj(ModTile tile, int style = 0, string name = "") : base(tile.Name + name, tile.Texture) {
        Tile = tile;
        Style = style;
    }

    public virtual string GetTombstoneText() {
        return Projectile.miscText;
    }

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
        if (Main.tile[x, y].HasTile) {
            return;
        }

        if (TileObject.CanPlace(x, y, Tile.Type, Style, Projectile.direction, out TileObject objectData) && TileObject.Place(objectData)) {
            NetMessage.SendObjectPlacement(-1, x, y, objectData.type, objectData.style, objectData.alternate, objectData.random, Projectile.direction);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            int sign = Sign.ReadSign(x, y);
            if (sign >= 0) {
                Sign.TextSign(sign, GetTombstoneText());
                NetMessage.SendData(MessageID.ReadSign, number: sign, number3: new BitsByte(b1: true));
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