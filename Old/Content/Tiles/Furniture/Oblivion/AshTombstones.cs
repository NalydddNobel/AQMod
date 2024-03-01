using Aequus.Common.Tiles;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Initialization;
using Aequus.Old.Content.Particles;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Tiles.Furniture.Oblivion;

[LegacyName("Tombstones", "AshTombstonesTile")]
public class AshTombstones : ModTile {
    public const int STYLE_AshTombstone = 0;
    public const int STYLE_AshGraveMarker = 1;
    public const int STYLE_AshCrossGraveMarker = 2;
    public const int STYLE_AshHeadstone = 3;
    public const int STYLE_AshGravestone = 4;
    public const int STYLE_AshObelisk = 5;
    public const int STYLE_COUNT = 6;

    private static ModItem[] _tombstones;

    public static List<ModProjectile> TombstoneProjectiles { get; private set; } = new();

    public override void Load() {
        _tombstones = new ModItem[STYLE_COUNT];
        Add("Tombstone", STYLE_AshTombstone, ItemID.Tombstone);
        Add("GraveMarker", STYLE_AshGraveMarker, ItemID.GraveMarker);
        Add("CrossGraveMarker", STYLE_AshCrossGraveMarker, ItemID.CrossGraveMarker);
        Add("Headstone", STYLE_AshHeadstone, ItemID.Headstone);
        Add("Gravestone", STYLE_AshGravestone, ItemID.Gravestone);
        Add("Obelisk", STYLE_AshObelisk, ItemID.Obelisk);

        ModItem Add(string name, int style, int ingredient) {
            ModItem item = new InstancedTileItem(this, style, name, rarity: ItemRarityID.Green, researchSacrificeCount: 2);
            _tombstones[style] = item;

            ModProjectile proj = new InstancedAshTombstoneProj(this, style, name);

            Mod.AddContent(item);
            Mod.AddContent(proj);
            ModTypeLookup<ModItem>.RegisterLegacyNames(item, $"Ash{name}");
            TombstoneProjectiles.Add(proj);

            Aequus.OnAddRecipes += () => {
                item.CreateRecipe()
                    .AddIngredient(ingredient)
                    .AddIngredient(ItemID.AshBlock, 25)
                    .AddTile<OblivionCraftingStation>()
                    .Register();
            };

            return item;
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
        TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.LavaDeath = false;
        TileObjectData.addTile(Type);
        AddMapEntry(new Color(100, 20, 10, 255), LocalizedText.Empty, MapEntryName);
        DustType = 37;
        AdjTiles = new int[] { TileID.Tombstones };
    }

    private static string MapEntryName(string name, int i, int j) {
        int style = Math.Clamp(Main.tile[i, j].TileFrameX / 36, 0, STYLE_COUNT);

        return LanguageDatabase.GetItemName(_tombstones[style].Type).Value;
    }

    public override void Unload() {
        TombstoneProjectiles.Clear();
    }

    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) {
        return true;
    }

    public override bool RightClick(int i, int j) {
        return true;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) {
        num = fail ? 1 : 3;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        Sign.KillSign(i, j);
    }

    public override void MouseOverFar(int i, int j) {
        MouseOver(i, j);
    }

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
        r = 0.04f;
        g = 0.02f;
        b = 0.001f;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        if (Main.tile[i, j].IsTileInvisible) {
            return;
        }

        var frame = new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16);
        if (Main.tile[i, j].TileFrameY >= 18) {
            frame.Y = 18;
        }

        Main.spriteBatch.Draw(AequusTextures.AshTombstones_Glow, new Vector2(i * 16f - Main.screenPosition.X, j * 16f - Main.screenPosition.Y) + TileHelper.DrawOffset,
            frame, new Color(200, 100, 100, 0) * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 3f, 0.2f, 0.5f), 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0f);
    }
}

internal class InstancedAshTombstoneProj : InstancedTombstoneProj {
    public InstancedAshTombstoneProj(ModTile tile, int style, string name) : base(tile, style, name) { }

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }

    public override string GetTombstoneText() {
        return Language.GetTextValue("Mods.Aequus.DeathMessage.AshTombstone." + Main.rand.Next(13), base.GetTombstoneText());
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
            Projectile.velocity.X = oldVelocity.X * Main.rand.NextFloat(-1.15f, -0.9f);
        }
        if (Projectile.velocity.Y != oldVelocity.Y) {
            Projectile.velocity.Y = oldVelocity.Y * Main.rand.NextFloat(-1.15f, -0.9f);
        }
        if (Main.player[Projectile.owner].active) {
            var toPlayer = Main.player[Projectile.owner].Center - Projectile.Center;
            Projectile.velocity.X = Math.Abs(Projectile.velocity.X) * Math.Sign(toPlayer.X);
        }
        return false;
    }

    public void DrawGrave(Vector2 drawCoordinates, Texture2D tileTexture, float rotation, Color color) {
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                Rectangle frame = new(18 * (Style * 2 + i), 18 * j, 16, j == 1 ? 18 : 16);
                var tileSegmentCoords = drawCoordinates + new Vector2(frame.Width * (i - 1), frame.Height * (j - 1)).RotatedBy(rotation);
                Main.spriteBatch.Draw(
                    tileTexture,
                    tileSegmentCoords,
                    frame,
                    color,
                    rotation,
                    Vector2.Zero,
                    Projectile.scale,
                    SpriteEffects.None, 0f
                );
            }
        }
    }

    public override void PostDraw(Color lightColor) {
        var tileTexture = TextureAssets.Tile[Tile.Type].Value;
        var glowTexture = AequusTextures.AshTombstones_Glow.Value;
        int trailLength = ProjectileID.Sets.TrailCacheLength[Type];
        var positionOffset = Projectile.Size / 2f;
        for (int i = 0; i < trailLength; i++) {
            float progress = 1f - i / (float)trailLength;
            DrawGrave(Projectile.oldPos[i] + positionOffset - Main.screenPosition, tileTexture, Projectile.oldRot[i], Color.Orange with { A = 0 } * progress);
        }
        var drawCoordinates = Projectile.Center - Main.screenPosition;
        DrawGrave(drawCoordinates, tileTexture, Projectile.rotation, lightColor);
        DrawGrave(drawCoordinates, glowTexture, Projectile.rotation, Color.White);
    }
}

public class AshTombstoneExplosion : ModProjectile {
    public override string Texture => AequusTextures.GenericExplosion.Path;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 7;
    }

    public override void SetDefaults() {
        Projectile.SetDefaultNoInteractions();
        Projectile.width = 90;
        Projectile.height = 90;
        Projectile.timeLeft = 20;
    }

    public override Color? GetAlpha(Color lightColor) {
        return new Color(255, 128, 60, 100);
    }

    public override void AI() {
        if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            ViewHelper.PunchCameraTo(Projectile.Center, 8f, frames: 30);
            foreach (var p in ModContent.GetInstance<LegacyBloomParticle>().NewMultiple(20)) {
                var randomVector2 = Main.rand.NextVector2Unit();

                p.Location = Projectile.Center + randomVector2 * Main.rand.NextFloat(16f);
                p.Velocity = randomVector2 * Main.rand.NextFloat(3f, 12f);
                p.Color = new Color(255, 128, 100, 60);
                p.BloomColor = new Color(30, 5, 0, 0);
                p.Scale = 2f;
                p.BloomScale = 0.5f;
                p.dontEmitLight = false;
                p.Frame = (byte)Main.rand.Next(3);
                p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
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
