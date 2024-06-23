using Aequus.Common.Elements;
using Aequus.Core.Entities;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Aequus.Content.Enemies.PollutedOcean.OilSlime;

public class OilSlimeDeathProj() : FloorHazard(new Info(HurtsPlayers: true, HurtsNPCs: true, HurtsProjectiles: true)) {
    public bool burning;

    public override void AI() {
        if (HazardPoints.Count <= 0) {
            Projectile.position.Y += 2f;
            MarkAllAvailable();
            return;
        }

        base.AI();

        int timeBetweenSpread = 35;
        int spreadCount = 5;
        if ((int)Projectile.ai[0] <= timeBetweenSpread * spreadCount) {
            if ((int)Projectile.ai[0] == 0) {
                HazardPoints.Clear();
            }
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] % timeBetweenSpread == 0) {
                Spread(OnSpread);
            }
        }

        if (burning) {
            int dustPerTick = Math.Max((TileWidth + TileHeight) / 4, 1);
            for (int i = 0; i < dustPerTick; i++) {
                Point p = Main.rand.Next(HazardPoints);
                Vector2 worldCoordinates = p.ToWorldCoordinates(Main.rand.NextFloat(16f), 0f);
                worldCoordinates.Y += 4f;
                float scale = Main.rand.NextFloat(0.3f, 2f);

                if (Main.rand.NextBool()) {
                    Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.Torch, Alpha: 100, Scale: scale);
                    if (!Main.rand.NextBool(7)) {
                        d.fadeIn = d.scale;
                        d.noGravity = true;
                        d.scale *= 0.3f;
                    }
                    else {
                        d.scale *= 0.8f;
                        d.noLight = true;
                        d.noLightEmittence = true;
                    }
                    d.velocity.Y -= scale * 0.6f;
                }
                else {
                    Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.Smoke, Alpha: 150, Scale: scale * 0.5f);
                    d.velocity *= 0.4f;
                    d.velocity.Y -= scale * 0.3f;
                }
            }
        }

        void OnSpread(int i, int j) {
            SoundEngine.PlaySound(SoundID.Item154, new Vector2(i, j) * 16f);
            for (int k = 0; k < 9; k++) {
                float scale = Main.rand.NextFloat(0.3f, 2f);
                Vector2 worldCoordinates = new Vector2(i + Main.rand.NextFloat(1f), j) * 16f;
                worldCoordinates.Y += 4f;

                Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.Obsidian, Scale: scale * 0.5f);
                d.velocity *= 0.4f;
                d.velocity.Y -= scale * 0.3f;
            }
        }

    }

    public override void OnHazardCollideWithPlayer(Player target) {
        if (burning) {
            target.AddBuff(BuffID.OnFire, 120);
        }
        else {
            target.slippy = true;
        }

        if (target.onFire || target.onFire2 || target.onFire3) {
            burning = true;
        }
    }

    public override void OnHazardCollideWithNPC(NPC target) {
        if (burning) {
            target.AddBuff(BuffID.OnFire, 120);
        }

        if (target.onFire || target.onFire2 || target.onFire3 || target.shadowFlame) {
            burning = true;
        }
    }

    public override bool IsValidSpotForHazard(int i, int j) {
        if (!base.IsValidSpotForHazard(i, j)) {
            return false;
        }

        return Framing.GetTileSafely(i, j - 1).LiquidType <= 0;
    }

    public override void OnHazardCollideWithProjectile(Projectile other) {
        if (burning || !other.GetItemSource(out int itemSource, out int ammoSource)) {
            return;
        }

        if (Element.Flame.ContainsItem(itemSource) || Element.Flame.ContainsItem(ammoSource)) {
            burning = true;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        float opacity = Projectile.Opacity;
        foreach (Point p in HazardPoints) {
            FastRandom random = Helper.RandomTileCoordinates(p.X, p.Y);
            Vector2 worldCoordinates = p.ToWorldCoordinates();
            Color hazardColor = Lighting.GetColor(p) * opacity;
            Rectangle frame = new Rectangle(18 * random.Next(6), 0, 18, 18);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 scale = new Vector2(1f, 1f);
            float rotation = 0f;

            Tile tile = Framing.GetTileSafely(p);

            if (tile.IsHalfBlock) {
                worldCoordinates.Y += 7f;
                frame.Y = 36;
            }
            else if (tile.Slope > 0) {
                switch (tile.Slope) {
                    case SlopeType.SlopeDownLeft:
                        rotation = MathHelper.PiOver4;
                        worldCoordinates.Y += 6f;
                        scale.X *= 1.33f;
                        break;

                    case SlopeType.SlopeDownRight:
                        rotation = -MathHelper.PiOver4;
                        worldCoordinates.Y += 6f;
                        scale.X *= 1.33f;
                        break;
                }
            }
            else {
                byte connection = 0;
                if (CheckConnection(p.X - 1, p.Y)) {
                    connection += 1 << 0;
                }
                if (CheckConnection(p.X + 1, p.Y)) {
                    connection += 1 << 1;
                    effects = SpriteEffects.FlipHorizontally;
                }

                frame.Y += 18 * connection switch {
                    0 => 0,
                    4 => 2,
                    _ => 1,
                };

                worldCoordinates.Y -= 1f;
            }

            hazardColor = hazardColor.MultiplyRGB(new Color(40, 40, 40, 200));

            Main.EntitySpriteDraw(texture, worldCoordinates - Main.screenPosition, frame, hazardColor, rotation, frame.Size() / 2f, scale, effects);
        }

        bool CheckConnection(int i, int j) => !Contains(i, j) || Framing.GetTileSafely(i, j).IsHalfBlock;
        return false;
    }
}
