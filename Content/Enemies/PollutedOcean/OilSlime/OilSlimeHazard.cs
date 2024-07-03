using Aequu2.Content.Elements;
using Aequu2.Core.Entities;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Utilities;

namespace Aequu2.Content.Enemies.PollutedOcean.OilSlime;

public class OilSlimeHazard() : FloorHazard(new Info(HurtsPlayers: true, HurtsNPCs: true, HurtsProjectiles: true)) {
    public bool OnFire { get; set; }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Parent parent && parent.Entity is Projectile projectile && projectile.ModProjectile is IOilSlimeInheritedBurning burningInst) {
            OnFire = burningInst.OnFire;
        }
    }

    public override void AI() {
        if (HazardPoints.Count <= 0) {
            Projectile.position.Y += 2f;
            MarkAllAvailable();
            Projectile.timeLeft = 3600;
            return;
        }

        if (OnFire) {
            if (Projectile.soundDelay == 0) {
                SoundEngine.PlaySound(AequusSounds.Looping_Oil with { Volume = 0.25f, PitchVariance = 0.5f, MaxInstances = 20 }, Main.rand.NextVector2FromRectangle(Projectile.Hitbox));
                Projectile.soundDelay = Main.rand.Next(150, 500);
            }
            else {
                Projectile.soundDelay--;
            }
        }

        base.AI();

        int timeBetweenSpread = 5;
        int spreadCount = 6;
        if ((int)Projectile.ai[0] <= timeBetweenSpread * spreadCount) {
            if ((int)Projectile.ai[0] == 0) {
                HazardPoints.Clear();
            }
            Projectile.ai[0]++;
            if ((int)Projectile.ai[0] % timeBetweenSpread == 0) {
                Spread(OnSpread);
            }
        }

        if (OnFire && HazardPoints.Count > 0) {
            int dustPerTick = (int)Math.Max((TileWidth + TileHeight) * (1f + Main.gfxQuality) * 0.33f, 1);
            for (int i = 0; i < dustPerTick; i++) {
                if (!Main.rand.NextBool(Projectile.alpha / 20 + 1)) {
                    continue;
                }

                Point p = Main.rand.Next(HazardPoints);
                Vector2 worldCoordinates = p.ToWorldCoordinates(Main.rand.NextFloat(16f), Main.rand.NextFloat(24f));
                worldCoordinates.Y -= 4f;
                float scale = Main.rand.NextFloat(0.3f, 2f);

                if (Main.rand.NextBool()) {
                    Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.Torch, Alpha: 100, Scale: scale);
                    if (!Main.rand.NextBool(7)) {
                        d.fadeIn = d.scale;
                        d.noGravity = true;
                        d.scale *= 0.3f;
                        d.velocity.Y -= scale * 0.5f;
                    }
                    else if (!Main.rand.NextBool(3)) {
                        d.scale *= 2;
                        d.noGravity = true;
                        d.velocity *= Main.rand.NextFloat(0.1f);
                    }
                    else {
                        d.scale *= 0.8f;
                        d.noLight = true;
                        d.noLightEmittence = true;
                        d.velocity.Y -= scale * 1f;
                    }
                }
                else {
                    Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.Smoke, Alpha: 150, Scale: scale * 0.5f);
                    d.velocity *= 0.4f;
                    d.velocity.Y -= scale * 0.3f;
                }
            }
        }

        if (Projectile.timeLeft < 30) {
            Projectile.alpha += 9;
        }

        void OnSpread(int i, int j) {
            SoundEngine.PlaySound(SoundID.Item154, new Vector2(i, j) * 16f);
            for (int k = 0; k < 9; k++) {
                float scale = Main.rand.NextFloat(0.3f, 2f);
                Vector2 worldCoordinates = new Vector2(i + Main.rand.NextFloat(1f), j) * 16f;
                worldCoordinates.Y += 4f;

                Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.TintableDust, Scale: scale * 0.5f, newColor: OilSlime.SlimeColor);
                d.velocity *= 0.4f;
                d.velocity.Y -= scale * 0.3f;
            }

            if (OnFire) {
                for (int k = 0; k < 9; k++) {
                    float scale = Main.rand.NextFloat(0.3f, 2f);
                    Vector2 worldCoordinates = new Vector2(i + Main.rand.NextFloat(1f), j + Main.rand.NextFloat(0.25f)) * 16f;
                    worldCoordinates.Y += 4f;

                    Dust d = Dust.NewDustPerfect(worldCoordinates, DustID.Torch, Scale: Main.rand.NextFloat(1f, 2.5f));
                    d.velocity *= 0.4f;
                    d.velocity.Y -= Math.Abs(Projectile.Center.X - worldCoordinates.X) * Main.rand.NextFloat(0.01f, 0.04f);
                    d.noGravity = true;
                }
            }
        }
    }

    public override void OnHazardCollideWithPlayer(Player target) {
        if (OnFire) {
            target.AddBuff(BuffID.OnFire, 120);
        }
        else {
            target.slippy = true;
        }

        bool flammableTarget = target.onFire || target.onFire2 || target.onFire3;
        Item heldItem = target.HeldItem;
        if (heldItem != null && heldItem.createTile > -1 && TileID.Sets.Torch[heldItem.createTile]) {
            flammableTarget = true;
        }

        if (flammableTarget) {
            StartBurning();
        }
    }

    public override void OnHazardCollideWithNPC(NPC target) {
        if (OnFire) {
            target.AddBuff(BuffID.OnFire, 120);
        }

        if (target.onFire || target.onFire2 || target.onFire3 || target.shadowFlame) {
            OnFire = true;
        }
    }

    public override bool IsValidSpotForHazard(int i, int j) {
        if (!base.IsValidSpotForHazard(i, j)) {
            return false;
        }

        return Framing.GetTileSafely(i, j - 1).LiquidType <= 0;
    }

    public override void OnHazardCollideWithProjectile(Projectile other) {
        if (Projectile.timeLeft <= 60 || OnFire || !other.GetItemSource(out int itemSource, out int ammoSource)) {
            return;
        }

        if (Element.Flame.ContainsItem(itemSource) || Element.Flame.ContainsItem(ammoSource)) {
            StartBurning();
        }
    }

    private void StartBurning() {
        if (!OnFire && Projectile.timeLeft > 60 && Main.myPlayer == Projectile.owner) {
            Projectile.timeLeft = 30;

            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
            (Main.projectile[p].ModProjectile as OilSlimeHazard).OnFire = true;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        float opacity = Projectile.Opacity;

        Vector2 origin = new Vector2(8f, 8f);

        foreach (Point p in HazardPoints) {
            FastRandom random = Helper.RandomTileCoordinates(p.X, p.Y);
            Vector2 worldCoordinates = p.ToWorldCoordinates();
            Color hazardColor = Lighting.GetColor(p);
            Rectangle frame = new Rectangle(18 * random.Next(6), 0, 16, 34);
            SpriteEffects effects = SpriteEffects.None;
            Vector2 scale = new Vector2(1f, 1f);
            float rotation = 0f;

            Tile tile = Framing.GetTileSafely(p);

            if (tile.IsHalfBlock) {
                worldCoordinates.Y += 6f;
                frame.Y = 36;
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

                frame.Y += 36 * connection switch {
                    0 => 0,
                    4 => 2,
                    _ => 1,
                };

                if (tile.Slope > 0) {
                    effects = tile.Slope != SlopeType.SlopeDownLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    frame.Y = 108;
                }

                worldCoordinates.Y -= 4f;
            }

            if (OnFire) {
                hazardColor = hazardColor.MultiplyRGB(new Color(40, 40, 40, 255));
            }
            hazardColor *= 0.8f;

            Main.EntitySpriteDraw(texture, worldCoordinates - Main.screenPosition, frame, hazardColor * opacity, rotation, origin, scale, effects);
        }

        bool CheckConnection(int i, int j) => !Contains(i, j) || Framing.GetTileSafely(i, j).IsHalfBlock;
        return false;
    }
}
