using Aequus.Common.Drawing.Generative;
using Aequus.Common.Particles;
using Aequus.Particles;
using ReLogic.Content;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Tiles.Tombstones;

[LegacyName("AshTombstonesTile")]
public class HellTombstones : UnifiedTombstones {
    public const int Tombstone = 0;
    public const int Grave_Marker = 1;
    public const int Cross_Marker = 2;
    public const int Headstone = 3;
    public const int Gravestone = 4;
    public const int Obelisk = 5;

    public const int Gold_Yin = 6;
    public const int Gold_Tombstone = 7;
    public const int Gold_Imp = 8;
    public const int Gold_Quartz = 9;
    public const int Gold_Fist = 10;

    public const int Style_Count = 11;

    public const int Death_Messages_Count = 13;

    public override int StyleCount => Style_Count;

    public readonly Asset<Texture2D>[] Glow = new Asset<Texture2D>[Style_Count];

    public override void Load() {
        base.Load();
        #region Legacy
        ModTypeLookup<ModItem>.RegisterLegacyNames(Items[0], "AshTombstone");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Items[1], "AshGraveMarker");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Items[2], "AshCrossGraveMarker");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Items[3], "AshHeadstone");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Items[4], "AshGravestone");
        ModTypeLookup<ModItem>.RegisterLegacyNames(Items[5], "AshObelisk");
        #endregion
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DustType = DustID.Ash;
        AddMapEntry(new Color(101, 90, 102));
        AddMapEntry(new Color(214, 36, 36));
    }

    protected override void SetStyleTexture(int style, ITextureGenerator generator, in AtlasInfo info) {
        base.SetStyleTexture(style, generator, info);
        Glow[style] = TextureGen.New(generator, AequusTextures.HellTombstones_Glow.Asset);
    }

    public override TombstoneOverride? OverrideTombstoneDrop(Player player, bool gold, long coinsOwned) {
        if (player.position.Y < Main.UnderworldLayer * 16) {
            return null;
        }

        string message = TextHelper.GetTextValue("DeathMessage.AshTombstone." + Main.rand.Next(13));
        int projType = gold ? Main.rand.Next(Gold_Yin, Gold_Fist + 1) : Main.rand.Next(Tombstone, Obelisk + 1);
        return new TombstoneOverride(Projectiles[projType].Type, message);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        switch (Main.tile[i, j].TileFrameX / 36) {
            // Gold Ash Tombstones color
            case > Gold_Yin: {
                    r = 0.5f;
                    g = 0.1f;
                    b = 0.1f;
                }
                break;

            // Ash Tombstones color
            default: {
                    r = 0.6f;
                    g = 0.3f;
                    b = 0.1f;
                }
                break;
        }
    }

    public override ushort GetMapOption(int i, int j) {
        return (ushort)(Main.tile[i, j].TileFrameX >= (36 * Gold_Yin) ? 1 : 0);
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        Tile tile = Main.tile[i, j];
        Vector2 drawCoordinates = new Vector2(i, j) * 16f - Main.screenPosition + Helper.TileDrawOffset;
        Rectangle frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        spriteBatch.Draw(AequusTextures.HellTombstones_Glow.Value, drawCoordinates, frame, Color.White * Helper.Wave(Main.GlobalTimeWrappedHourly, 0.5f, 0.8f));
    }
}

internal class AshTombstoneProj(HellTombstones parent, int style) : InstancedTombstoneProjectile(parent, style) {
    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.friendly = true;
    }

    public override void AI() {
        if (Main.getGoodWorld) {
            Projectile.damage = Math.Max(Projectile.damage, 300);
            Projectile.hostile = true;
        }
        var tileCoords = Projectile.Center.ToTileCoordinates();
        if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 5) && Main.tile[tileCoords].LiquidAmount > 0) {
            Projectile.velocity.Y -= 0.7f;
        }
        if ((int)Projectile.ai[0] == 0) {
            Projectile.ai[0] = 6f;
        }
        base.AI();
    }

    public override bool? CanDamage() {
        return Projectile.velocity.Length() > 1f ? null : false;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Math.Abs(Projectile.velocity.Y) < 0.2f) {
            Projectile.ai[0] = -1f;
            return base.OnTileCollide(oldVelocity);
        }
        Projectile.ai[0]--;
        if (Projectile.ai[0] <= 0f) {
            Projectile.ai[0] = -1f;
        }
        if (Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Normalize(Projectile.velocity) * 0.1f,
                ModContent.ProjectileType<AshTombstoneExplosion>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner
            );
        }

        if (Projectile.velocity.X != oldVelocity.X) {
            Projectile.velocity.X = oldVelocity.X * Main.rand.NextFloat(-1.1f, -0.8f);
        }

        if (Projectile.velocity.Y != oldVelocity.Y) {
            Projectile.velocity.Y = oldVelocity.Y * Main.rand.NextFloat(-1.1f, -0.8f);
        }

        if (Main.player[Projectile.owner].active && Projectile.ai[0] > 0f) {
            var toPlayer = Main.player[Projectile.owner].Center - Projectile.Center;
            Projectile.velocity.X = Math.Abs(Projectile.velocity.X) * Math.Sign(toPlayer.X);
        }
        return false;
    }

    public override void PostDraw(Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var glowTexture = parent.Glow[style].Value;
        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
        var positionOffset = Projectile.Size / 2f;
        Vector2 origin = texture.Size() / 2f;
        for (int i = 0; i < trailLength; i++) {
            float progress = Helper.CalcProgress(trailLength, i);
            Vector2 position = Projectile.oldPos[i] + positionOffset - Main.screenPosition;
            Main.EntitySpriteDraw(texture, position, null, Color.Orange with { A = 0 } * progress, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None);
        }

        var drawCoordinates = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawCoordinates, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(glowTexture, drawCoordinates, null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
    }
}

internal class AshTombstoneExplosion : ModProjectile {
    public override string Texture => AequusTextures.Explosion1.FullPath;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults() {
        Projectile.DefaultToExplosion(90, DamageClass.Default, 20);
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 128, 60, 100);
    }

    public override void AI() {
        if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            ScreenShake.SetShake(40f, 0.85f, Projectile.Center);
            for (int i = 0; i < 20; i++) {
                var v = Main.rand.NextVector2Unit();
                ParticleSystem.New<MonoBloomParticle>(ParticleLayer.BehindPlayers).Setup(
                    Projectile.Center + v * Main.rand.NextFloat(16f),
                    v * Main.rand.NextFloat(3f, 12f),
                    new Color(255, 128, 100, 60),
                    new Color(60, 10, 0, 0),
                    1.25f,
                    0.2f
                );
            }
        }
        if (Main.getGoodWorld) {
            Projectile.hostile = true;
        }
        Projectile.frameCounter++;
        if (Projectile.frameCounter > 2) {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= Main.projFrames[Type]) {
                Projectile.hide = true;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
        Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}
