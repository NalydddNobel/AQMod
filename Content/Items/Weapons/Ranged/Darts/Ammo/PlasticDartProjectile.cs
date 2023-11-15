using Aequus.Common.Graphics;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Ranged.Darts.Ammo;

public class PlasticDartProjectile : ModProjectile {
    public byte State { get; set; }

    public bool Broken => State > 0;

    public bool Stuck => State == 2;

    public bool Returning => State == 3;

    public override string Texture => ModContent.GetInstance<PlasticDart>().Texture;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.PoisonDart);
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 1;
    }

    public override bool? CanDamage() {
        return !Broken;
    }

    public override bool PreAI() {
        if (Projectile.timeLeft < 60) {
            Projectile.Opacity = Projectile.timeLeft / 60f;
        }

        Projectile.oldRot[0] = Projectile.rotation;
        Projectile.oldRot[1] = Projectile.velocity.ToRotation();
        for (int i = Projectile.oldRot.Length - 1; i > 1; i--) {
            Projectile.oldRot[i] = Projectile.oldRot[i - 1];
        }

        var aequusProjectile = Projectile.GetGlobalProjectile<AequusProjectile>();
        if (State == 4) {
            Projectile.Center = Main.player[Projectile.owner].Center;
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 80);
            Projectile.extraUpdates = 1;
            if (Projectile.timeLeft > 2) {
                Projectile.timeLeft--;
            }
            return false;
        }

        if (State == 3) {
            Projectile.alpha = 0;
            if (!Main.player[Projectile.owner].active || Main.player[Projectile.owner].DeadOrGhost) {
                Projectile.Kill();
                return false;
            }

            Projectile.ai[0] += 0.2f;
            Projectile.tileCollide = false;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[Projectile.owner].Center) * Math.Min(Projectile.ai[0], 32f), 0.1f);
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            if (Projectile.Distance(Main.player[Projectile.owner].Center) < 40f) {
                var item = new Item(aequusProjectile.parentAmmoType);
                item = Main.player[Projectile.owner].GetItem(Projectile.owner, item, GetItemSettings.PickupItemFromWorld);
                if (item != null && !item.IsAir) {
                    int newItemIndex = Item.NewItem(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].getRect(), item);
                    Main.item[newItemIndex].newAndShiny = false;
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        NetMessage.SendData(MessageID.SyncItem, number: newItemIndex, number2: 1f);
                    }
                }
                State = 4;
            }
            return false;
        }

        if (Broken) {
            Projectile.damage = 0;
            Projectile.timeLeft = Math.Min(Projectile.timeLeft, 600);
            if (Stuck) {
                if (!Projectile.noDropItem && Projectile.timeLeft < 550 && aequusProjectile.parentAmmoType > ItemID.None && Main.player[Projectile.owner].active && !Main.player[Projectile.owner].DeadOrGhost) {
                    var dummyItem = new Item(ModContent.ItemType<PlasticDart>()) {
                        Bottom = Projectile.Bottom
                    };
                    for (int i = 0; i < Main.maxPlayers; i++) {
                        if (!Main.player[i].active || Main.player[i].DeadOrGhost || Main.player[i].team != Main.player[Projectile.owner].team) {
                            continue;
                        }

                        int grabRange = Main.player[i].GetItemGrabRange(dummyItem);
                        if (Main.player[i].Distance(Projectile.Center) > grabRange) {
                            continue;
                        }

                        State = 3;
                        Projectile.ai[0] = 0f;
                        Projectile.netUpdate = true;
                        Projectile.timeLeft = 1800;
                        return false;
                    }
                }

                var topTile = (Projectile.Top + new Vector2(0f, -4f)).ToTileCoordinates();
                var bottomTile = (Projectile.Bottom + new Vector2(0f, 4f)).ToTileCoordinates();
                if (!WorldGen.InWorld(topTile.X, topTile.Y, 20) || !WorldGen.InWorld(bottomTile.X, bottomTile.Y, 20)) {
                    Projectile.Kill();
                    return false;
                }

                // Only run if projectile has horizontal speed, this will be the case if it hit the Top or Bottom of a tile
                if (Projectile.velocity.X != 0f) {
                    // Fall if there is not a tile above you (Stuck to above tile)
                    if (!Main.tile[topTile].HasUnactuatedTile || !WorldGen.SolidTile(topTile)) {
                        Projectile.velocity.Y += 0.3f;
                    }
                    // Reduce horizontal speed
                    Projectile.velocity.X *= 0.8f;
                }
                else {
                    // Remove vertical speed if there is no horizontal speed (Stuck to the side of a tile)
                    Projectile.velocity.Y = 0f;
                }
                Projectile.tileCollide = true;
                
                if (Math.Abs(Projectile.velocity.X) > 0.5f) {
                    // Emit particles and get the tile Id for physics values
                    int d = -1;
                    ushort tilePhysicsType = ushort.MaxValue;
                    if (Main.tile[bottomTile].HasUnactuatedTile && WorldGen.SolidTile(bottomTile)) {
                        d = WorldGen.KillTile_MakeTileDust(bottomTile.X, bottomTile.Y, Main.tile[bottomTile]);
                        Main.dust[d].position = Projectile.Bottom;
                        tilePhysicsType = Main.tile[topTile].TileType;
                    }
                    else if (Main.tile[topTile].HasUnactuatedTile && WorldGen.SolidTile(topTile)) {
                        d = WorldGen.KillTile_MakeTileDust(topTile.X, topTile.Y, Main.tile[topTile]);
                        Main.dust[d].position = Projectile.Top;
                        tilePhysicsType = Main.tile[topTile].TileType;
                    }

                    if (d != -1) {
                        Main.dust[d].velocity *= 0.1f;
                        Main.dust[d].noGravity = true;
                        Main.dust[d].scale *= 1.2f;
                    }

                    // Reduce horizontal dart speed by tile dampening resistence
                    if (tilePhysicsType != ushort.MaxValue) {
                        Projectile.velocity.X *= TileMaterials.GetByTileId(tilePhysicsType).GolfPhysics.ImpactDampeningResistanceEfficiency;
                    }
                }
            }
            else {
                Projectile.tileCollide = false;
                Projectile.rotation += Projectile.velocity.X * 0.05f;
                Projectile.velocity.X *= 0.97f;
                Projectile.velocity.Y += 0.3f;
            }
            return false;
        }
        return true;
    }

    public override void AI() {
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        State = 1;
        Projectile.damage = 0;
        Projectile.velocity = -Projectile.velocity * 0.3f;
        if (Projectile.velocity.Y > -8f) {
            Projectile.velocity.Y = Math.Max(Projectile.velocity.Y - Main.rand.NextFloat(4f), -8f);
        }
        Projectile.tileCollide = false;
        Projectile.netUpdate = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Broken) {
            return false;
        }

        Projectile.damage = 0;
        Projectile.tileCollide = false;
        State = 1;

        // Scan forward for the impact position
        var scanEnd = Projectile.Center;
        var scanDirection = Vector2.Normalize(oldVelocity) * 2f;
        int scans = 0;
        do {
            scanEnd += scanDirection;
            scans++;
        }
        while (scans < 100 && !Collision.SolidCollision(scanEnd, 0, 0));

        // Make the dart stick if it hit a soft tile
        var tile = Framing.GetTileSafely(scanEnd + scanDirection * 2f);
        if (tile.HasUnactuatedTile && Main.rand.NextFloat() < TileMaterials.GetByTileId(tile.TileType).GolfPhysics.ImpactDampeningResistanceEfficiency) {
            State = 2;
            Projectile.tileCollide = true;
        }
        else if (Projectile.velocity.Y != oldVelocity.Y) {
            // Deflect at 33% of the speed
            Projectile.velocity.Y = -oldVelocity.Y * 0.33f;
        }

        // If hitting something from the side
        if (Projectile.velocity.X != oldVelocity.X) {
            if (Stuck) {
                // Remove horizontal speed if you got stuck and hit from the side
                Projectile.velocity.X = 0f;
                Projectile.position = scanEnd;
            }
            else {
                // Deflect at 33% of the speed
                Projectile.velocity.X = -oldVelocity.X * 0.33f;
            }
        }

        // Overall 50% velocity reduction multiplier
        Projectile.velocity *= 0.5f;

        // Add horizontal randomness to darts which dont stick
        if (!Stuck) {
            Projectile.velocity.X += Main.rand.NextFloat(-2f, 2f);
        }

        Projectile.netUpdate = true;
        return false;
    }

    public override bool PreDraw(ref Color lightColor) {
        if (State == 3 || State == 4) {

            AequusDrawing.DrawBasicVertexLine(AequusTextures.Trail, Projectile.oldPos, Projectile.oldRot, 
                p => new Color(50, 50, 50, 0) * Projectile.Opacity * (1f - p),
                p => 6f * Projectile.scale,
                Projectile.Size / 2f - Main.screenPosition);

            if (State == 4) {
                return false;
            }
        }
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out _);
        Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(State);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        State = reader.ReadByte();
    }
}