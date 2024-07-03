using Aequu2.Core.Entities.Projectiles;
using Aequu2.Core.Entities.Golfing;
using Aequu2.Core.Entities.Projectiles;
using System;
using System.IO;
using Terraria.GameContent.Golf;
using Terraria.GameContent.Metadata;
using Terraria.Physics;

namespace Aequu2.Content.Items.Weapons.Ranged.Ammo;

public class PlasticDartProjectile : ModProjectile, IGolfBallProjectile {
    public const byte InitState = 0;
    public const byte DefaultState = 1;
    public const byte BrokenState = 2;
    public const byte StuckState = 3;
    public const byte ReturningState = 4;
    public const byte TakenState = 5;
    public const byte GolfTeeState = 6;

    public static float ShootOffsetAmount = 8f;
    public static float Gravity = 0.1f;
    public static float TerminalVelocity = 32f;

    public byte State { get; set; }

    public bool Broken => State >= BrokenState;

    public bool CanBeHitByGolfClub => State == GolfTeeState || State == StuckState;

    public override string Texture => ModContent.GetInstance<PlasticDart>().Texture;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 15;
        ProjectileID.Sets.TrailingMode[Type] = 0;
    }

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.Seed);
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 1;
    }

    public override bool? CanDamage() {
        return Broken || Projectile.damage <= 0 ? false : null;
    }

    private void AI_TeeState() {
        Projectile.rotation = 0f;
        for (int k = 0; k < Main.maxProjectiles; k++) {
            if (Main.projectile[k].active && Main.projectile[k].type == Type && Main.projectile[k].damage == 0 && k != Projectile.whoAmI && Main.projectile[k].owner == Projectile.owner) {
                Main.projectile[k].Kill();
                return;
            }
        }
    }

    private void AI_GrabbedByOwner() {
        Projectile.Center = Main.player[Projectile.owner].Center;
        Projectile.timeLeft = Math.Min(Projectile.timeLeft, 80);
        Projectile.extraUpdates = 1;
        if (Projectile.timeLeft > 2) {
            Projectile.timeLeft--;
        }
    }

    private void AI_ReturnToOwner() {
        Projectile.ignoreWater = true;
        Projectile.alpha = 0;
        if (!Main.player[Projectile.owner].active || Main.player[Projectile.owner].DeadOrGhost) {
            Projectile.Kill();
            return;
        }

        Projectile.ai[0] += 0.2f;
        Projectile.tileCollide = false;
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.player[Projectile.owner].Center) * Math.Min(Projectile.ai[0], 32f), 0.1f);
        Projectile.rotation += Projectile.velocity.Length() * 0.05f;
        if (Projectile.Distance(Main.player[Projectile.owner].Center) < 40f) {
            var item = new Item(Projectile.GetGlobalProjectile<ProjectileSource>().parentAmmoType);
            item = Main.player[Projectile.owner].GetItem(Projectile.owner, item, GetItemSettings.PickupItemFromWorld);
            if (item != null && !item.IsAir) {
                int newItemIndex = Item.NewItem(Projectile.GetSource_FromThis(), Main.player[Projectile.owner].getRect(), item);
                Main.item[newItemIndex].newAndShiny = false;
                if (Main.netMode == NetmodeID.MultiplayerClient) {
                    NetMessage.SendData(MessageID.SyncItem, number: newItemIndex, number2: 1f);
                }
            }
            State = TakenState;
        }
    }

    private void AI_Stuck() {
        if (Projectile.timeLeft < 550) {
            var dummyItem = new Item(ModContent.ItemType<PlasticDart>()) {
                Bottom = Projectile.Bottom
            };

            bool allowGrabbing = true;
            if (Projectile.alpha <= 0) {
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (!Main.player[i].active || Main.player[i].DeadOrGhost || !GolfHelper.IsPlayerHoldingClub(Main.player[i])) {
                        continue;
                    }

                    int grabRange = Main.player[i].GetItemGrabRange(dummyItem) * 2;
                    if (Main.player[i].Distance(Projectile.Center) > grabRange) {
                        continue;
                    }

                    allowGrabbing = false;
                    Projectile.timeLeft = Math.Max(Projectile.timeLeft, 600);
                }
            }

            if (allowGrabbing && !Projectile.noDropItem && Projectile.GetGlobalProjectile<ProjectileSource>().parentAmmoType > ItemID.None && Main.player[Projectile.owner].active && !Main.player[Projectile.owner].DeadOrGhost) {
                for (int i = 0; i < Main.maxPlayers; i++) {
                    if (!Main.player[i].active || Main.player[i].DeadOrGhost || Main.player[i].team != Main.player[Projectile.owner].team) {
                        continue;
                    }

                    int grabRange = Main.player[i].GetItemGrabRange(dummyItem);
                    if (Main.player[i].Distance(Projectile.Center) > grabRange) {
                        continue;
                    }

                    State = ReturningState;
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                    Projectile.timeLeft = 1800;
                    return;
                }
            }
        }

        var topTile = (Projectile.Top + new Vector2(0f, -4f)).ToTileCoordinates();
        var bottomTile = (Projectile.Bottom + new Vector2(0f, 4f)).ToTileCoordinates();
        if (!WorldGen.InWorld(topTile.X, topTile.Y, 20) || !WorldGen.InWorld(bottomTile.X, bottomTile.Y, 20)) {
            Projectile.Kill();
            return;
        }

        // Only run if projectile has horizontal speed, this will be the case if it hit the Top or Bottom of a tile
        if (Projectile.velocity.X != 0f) {
            // Fall if there is not a tile above you (Stuck to above tile)
            if (!Main.tile[topTile].HasUnactuatedTile || !WorldGen.SolidTile(topTile)) {
                Projectile.velocity.Y += 0.3f;
            }
            // Reduce horizontal speed
            Projectile.velocity.X *= 0.9f;
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

    private void AI_Broken() {
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.rotation += Projectile.velocity.X * 0.05f;
        Projectile.velocity.X *= 0.97f;
        Projectile.velocity.Y += 0.3f;
    }

    private void AI_BrokenOrStuck() {
        Projectile.timeLeft = Math.Min(Projectile.timeLeft, Projectile.noDropItem ? 120 : 600);
    }

    private bool AI_Initialize() {
        if (Projectile.damage == 0) {
            var tileCoordinates = Projectile.Center.ToTileCoordinates();
            var tile = Framing.GetTileSafely(tileCoordinates);
            if (tile.HasUnactuatedTile && tile.TileType == TileID.GolfTee) {
                Projectile.netUpdate = true;
                State = GolfTeeState;
                return false;
            }
        }

        State = DefaultState;
        return true;
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

        if (Broken) {
            AI_BrokenOrStuck();
        }
        switch (State) {
            case GolfTeeState:
                AI_TeeState();
                return false;

            case TakenState:
                AI_GrabbedByOwner();
                return false;

            case ReturningState:
                AI_ReturnToOwner();
                return false;

            case StuckState:
                AI_Stuck();
                return false;

            case BrokenState:
                AI_Broken();
                return false;

            case InitState:
                return AI_Initialize();
        }

        return true;
    }

    public override void AI() {
        StepVelocity(ref Projectile.velocity, delta: 1f);
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        State = BrokenState;
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

        Projectile.tileCollide = false;
        State = BrokenState;

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
            State = StuckState;
            Projectile.tileCollide = true;
        }
        else if (Projectile.velocity.Y != oldVelocity.Y) {
            // Deflect at 33% of the speed
            Projectile.velocity.Y = -oldVelocity.Y * 0.33f;
        }

        // If hitting something from the side
        if (Projectile.velocity.X != oldVelocity.X) {
            if (State == StuckState) {
                // Remove horizontal speed if you got stuck and hit from the side
                Projectile.velocity.X = 0f;
                Projectile.position = scanEnd;
                Projectile.position.X -= 4f;
            }
            else {
                // Deflect at 33% of the speed
                Projectile.velocity.X = -oldVelocity.X * 0.33f;
            }
        }

        // Add horizontal randomness and reduce velocity to darts which dont stick
        if (State != StuckState) {
            Projectile.velocity *= 0.5f;
            Projectile.velocity.X += Main.rand.NextFloat(-2f, 2f);
        }

        Projectile.netUpdate = true;
        return false;
    }

    private void DrawGolfPredictionLine(Player player, ref Color lightColor) {
        Vector2 shotVector = Main.MouseWorld - Projectile.Center;
        if (Projectile.owner == Main.myPlayer && GolfHelper.IsPlayerHoldingClub(player) && (player.ownedProjectileCounts[ProjectileID.GolfClubHelper] > 0 && player.itemAnimation >= player.itemAnimationMax || player.itemAnimation == 0) && player.velocity.Y == 0f && GolfHelper.IsGolfBallResting(Projectile) && GolfHelper.ValidateShot(Projectile, player, ref shotVector)) {
            lightColor = Color.White;
            Projectile golfClubHelper = null;
            var offset = Vector2.Normalize(shotVector) * ShootOffsetAmount;
            Projectile.position += offset;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                var p = Main.projectile[i];
                if (p.active && p.owner == Projectile.owner && p.type == ProjectileID.GolfClubHelper) {
                    golfClubHelper = p;
                    break;
                }
            }
            if (golfClubHelper != null) {
                var shotStrength = GolfHelper.CalculateShotStrength(golfClubHelper, Projectile);
                var impactVelocity = Vector2.Normalize(shotVector) * shotStrength.AbsoluteStrength;
                if (impactVelocity.Length() > 0.05f) {
                    CustomGolfPredictionLine.Draw(Projectile, impactVelocity / 3f, shotStrength.RelativeStrength, shotStrength.RoughLandResistance);
                    //GolfHelper.DrawPredictionLine(Projectile, impactVelocity, shotStrength.RelativeStrength, shotStrength.RoughLandResistance);
                }
            }
            Projectile.position -= offset;
        }
    }

    private void DrawGolfingOutline(Vector2 drawCoordinates, Rectangle frame, Vector2 origin) {
        if (GolfHelper.IsGolfBallResting(Projectile) && GolfHelper.IsPlayerHoldingClub(Main.LocalPlayer) && GolfHelper.IsGolfShotValid(Projectile, Main.LocalPlayer) && Projectile.owner == Main.myPlayer) {
            Main.EntitySpriteDraw(AequusTextures.PlasticDart_Outline, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        var player = Main.player[Projectile.owner];
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out _);
        var drawCoordinates = Projectile.position + offset + new Vector2(0f, Projectile.gfxOffY) - Main.screenPosition;
        if (CanBeHitByGolfClub) {
            DrawGolfPredictionLine(player, ref lightColor);
        }
        if (State == GolfTeeState) {
            drawCoordinates.X -= 0.5f;
            drawCoordinates.Y -= 6f;
        }

        if (State == ReturningState || State == TakenState) {
            DrawHelper.DrawBasicVertexLineWithProceduralPadding(AequusTextures.Trail, Projectile.oldPos, Projectile.oldRot,
                p => new Color(50, 50, 50, 0) * Projectile.Opacity * (1f - p),
                p => 6f * Projectile.scale,
                Projectile.Size / 2f - Main.screenPosition);

            if (State == TakenState) {
                return false;
            }
        }

        Main.EntitySpriteDraw(texture, drawCoordinates, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

        if (CanBeHitByGolfClub) {
            DrawGolfingOutline(drawCoordinates, frame, origin);
        }
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(State);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        State = reader.ReadByte();
    }

    bool IGolfBallProjectile.PreHit(Vector2 shotVector) {
        if (!CanBeHitByGolfClub) {
            return false;
        }

        var wantedPosition = Projectile.position + Vector2.Normalize(shotVector) * ShootOffsetAmount;
        if (Collision.SolidCollision(wantedPosition, Projectile.width, Projectile.height)) {
            return false;
        }

        Projectile.position = wantedPosition;
        return true;
    }

    void IGolfBallProjectile.OnHit(Vector2 velocity, GolfHelper.ShotStrength shotStrength) {
        Projectile.velocity /= 3f;
        State = DefaultState;
    }

    bool IGolfBallProjectile.StepGolfBall(ref float angularVelocity, out BallStepResult result) {
        var tileCoordinates = Projectile.Center.ToTileCoordinates();
        if (!WorldGen.InWorld(tileCoordinates.X, tileCoordinates.Y, 2)) {
            result = BallStepResult.OutOfBounds();
            return true;
        }

        float delta = Projectile.velocity.Length() / 2f;
        for (float progress = 0f; progress < 1f; progress += delta) {
            delta = Math.Min(1f - progress, delta);
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
                result = BallStepResult.Resting();
                return true;
            }
            StepVelocity(ref Projectile.velocity, delta);
            Projectile.position += Projectile.velocity;
            if (Projectile.velocity.Length() <= 0.01f) {
                result = BallStepResult.Resting();
                return true;
            }
        }

        result = BallStepResult.Moving();
        return true;
    }

    private static void StepVelocity(ref Vector2 velocity, float delta = 1f) {
        velocity.Y += Gravity * delta;
        if (velocity.Y > TerminalVelocity) {
            velocity.Y = TerminalVelocity;
        }
    }
}