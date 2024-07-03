using Aequu2.Content.Dusts;
using Aequu2.Core.Entities.Projectiles;
using Aequu2.DataSets;
using Aequu2.Old.Content.Particles;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using tModLoaderExtended.Networking;

namespace Aequu2.Old.Content.Items.Tools;

public class PhysicsGunProj : HeldProjBase {
    public Vector2 mouseWorld;
    public Color mouseColor;
    public bool realBlock;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.netImportant = true;
        Projectile.alpha = 200;
    }

    public override bool? CanDamage() {
        return false;
    }

    public void PlaceTile() {
        if (!realBlock || Main.myPlayer != Projectile.owner) {
            return;
        }

        PhysGunPickaxePowerHack(ContentSamples.ItemsByType[ItemID.CopperPickaxe].pick);

        Point tileCoords = Projectile.Center.ToTileCoordinates();
        Tile tile = Main.tile[tileCoords];

        if (!tile.HasTile || WorldGen.CanKillTile(tileCoords.X, tileCoords.Y)) {
            if (!tile.HasTile || Main.player[Projectile.owner].HasEnoughPickPowerToHurtTile(tileCoords.X, tileCoords.Y)) {
                WorldGen.KillTile(tileCoords.X, tileCoords.Y);
                if (!WorldGen.PlaceTile(tileCoords.X, tileCoords.Y, (int)Projectile.ai[0], forced: true)) {
                    SoundEngine.PlaySound(SoundID.Dig, tileCoords.ToWorldCoordinates());
                }

                tile.HasTile = true;
                tile.TileType = (ushort)Projectile.ai[0];
            }

            WorldGen.SquareTileFrame(tileCoords.X, tileCoords.Y, resetFrame: true);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, tileCoords.X, tileCoords.Y);
            }
        }

        PhysGunPickaxePowerHack(0);
    }

    public override void AI() {
        if ((int)Projectile.ai[1] == 4) {
            Projectile.extraUpdates = 1;
            if (Projectile.timeLeft < 60) {
                Projectile.alpha += 255 / 60;
            }
            Projectile.rotation += Projectile.direction * ((0.1f + Projectile.velocity.Length() * 0.02f) / (1 + Projectile.extraUpdates));
            Projectile.scale -= 0.006f;
            if (Projectile.alpha < 100 && Main.rand.NextBool(6)) {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight);
            }
            return;
        }

        var player = Main.player[Projectile.owner];
        if ((int)Projectile.ai[1] == 3) {
            if (CheckEmancipationGrills()) {
                EmancipationGrillEffects();
                return;
            }
            Projectile.velocity.Y += 0.03f;
            Projectile.rotation += Projectile.direction * ((0.1f + Projectile.velocity.Length() * 0.02f) / (1 + Projectile.extraUpdates));
            TileLight();
            return;
        }

        if (Main.myPlayer == Projectile.owner) {
            var oldMouseWorld = mouseWorld;
            mouseWorld = Main.MouseWorld;
            if (mouseWorld.X != oldMouseWorld.X || mouseWorld.Y != oldMouseWorld.Y) {
                Projectile.netUpdate = true;
            }
            mouseColor = Main.mouseColor;
        }
        if (mouseColor == Color.Transparent) {
            if (Main.myPlayer == Projectile.owner) {
                Projectile.netUpdate = true;
            }

            mouseColor = Color.White;
        }

        if (!player.channel || !player.controlUseItem) {
            if (Projectile.ai[1] == 0) {
                Projectile.Kill();
            }
            else {
                Projectile.tileCollide = true;
                Projectile.timeLeft = 2400;
                Projectile.extraUpdates = 3;
                Projectile.velocity *= 0.5f;
                if (Projectile.velocity.Length() > 24f) {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 24f;
                }
                Projectile.velocity /= 1 + Projectile.extraUpdates;
                Projectile.ai[1] = 3f;
            }
            return;
        }

        Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
        Main.player[Projectile.owner].itemTime = 2;
        Main.player[Projectile.owner].itemAnimation = 2;
        Projectile.timeLeft = 2;

        if (Projectile.numUpdates == -1) {
            if (++Projectile.frameCounter > 3) {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }
        }

        if ((int)Projectile.ai[1] == 2) {
            if (CheckEmancipationGrills()) {
                EmancipationGrillEffects();
                return;
            }

            Projectile.tileCollide = true;
            Vector2 localMouseWorld = mouseWorld;
            player.LimitPointToPlayerReachableArea(ref localMouseWorld);
            Vector2 difference = localMouseWorld - Projectile.Center;

            Projectile.velocity = difference * 0.3f;

            if (Main.myPlayer == Projectile.owner) {
                if (Main.mouseRight && Main.mouseRightRelease) {
                    PlaceTile();
                    Projectile.Kill();
                    SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                }
            }
            Projectile.rotation += player.direction * 0.1f;
            GunLight();
            TileLight();
            SetArmRotation();
            return;
        }

        Vector2 oldVelocity = Projectile.velocity;
        Projectile.velocity = Vector2.Normalize(mouseWorld - Main.player[Projectile.owner].Center);
        if (Main.myPlayer == Projectile.owner) {
            if (oldVelocity.X != Projectile.velocity.X || oldVelocity.Y != Projectile.velocity.Y) {
                Projectile.netUpdate = true;
            }

            Projectile.Center = Main.player[Projectile.owner].Center;
        }

        if (Projectile.alpha > 0) {
            Projectile.alpha -= 70;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }

        ShootLaser();

        SetArmRotation();
        GunLight();
    }

    protected void ShootLaser() {
        Player player = Main.player[Projectile.owner];

        PhysGunPickaxePowerHack(ContentSamples.ItemsByType[ItemID.CopperPickaxe].pick);

        Vector2 checkCoords = Projectile.Center;
        for (int i = 0; i < 300; i++) {
            Vector2 oldCheckCoords = checkCoords;
            player.LimitPointToPlayerReachableArea(ref checkCoords);
            if (oldCheckCoords.X != checkCoords.X || oldCheckCoords.Y != checkCoords.Y) {
                break;
            }

            Point checkTileCoords = checkCoords.ToTileCoordinates();
            if (!WorldGen.InWorld(checkTileCoords.X, checkTileCoords.Y, 10)) {
                break;
            }

            if (!StepLaser2(checkCoords, checkTileCoords, out realBlock)) {
                break;
            }

            checkCoords += Projectile.velocity * 4f;
        }

        PhysGunPickaxePowerHack(0);

        Projectile.Center = checkCoords;

        if (Projectile.alpha < 100) {
            Vector2 oldPosition = Projectile.oldPosition;
            Vector2 position = Projectile.position;

            float speed = 9f;
            float dustScaleThreshold = speed * 6f;
            Vector2 normal = (oldPosition - position).SafeNormalize(Vector2.Zero);

            float distance = Vector2.Distance(position, oldPosition);

            float travel = 0f;
            do {
                Vector2 dustCoordinates = position + normal * travel;
                int dustType = Main.rand.NextBool(8) ? ModContent.DustType<MonoSparkleDust>() : ModContent.DustType<MonoDust>();

                float scale = 0.75f;

                if (travel < dustScaleThreshold) {
                    scale += (1f - travel / dustScaleThreshold) * 0.75f;
                }

                Dust d = Dust.NewDustPerfect(dustCoordinates, dustType, newColor: mouseColor with { A = 30 } * scale, Scale: scale);
                d.velocity *= 0.5f;
                d.velocity += normal * 0.1f;
                d.fadeIn = d.scale + 0.33f;

                travel += speed;
            }
            while (travel < distance);

        }
    }

    protected bool StepLaser2(Vector2 position, Point xy, out bool realBlock) {
        Tile tile = Main.tile[xy];
        ushort tileType = tile.TileType;
        Player player = Main.player[Projectile.owner];

        realBlock = false;

        // Shimmer Blocks the physics gun laser
        if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Shimmer) {
            if (position.Y % 16f > (255 - tile.LiquidAmount) / 255f * 16f) {
                return false;
            }
        }

        if (tile.HasTile) {
            if (TileDataSet.PhysicsGunBlocksLaser.Contains(tileType)) {
                return false;
            }

            if (tile.IsFullySolid() && !Main.tileFrameImportant[tileType]) {
                if (TileDataSet.PhysicsGunCannotPickUp.Contains(tileType) || !player.HasEnoughPickPowerToHurtTile(xy.X, xy.Y) || !WorldGen.CanKillTile(xy.X, xy.Y)) {
                    return false;
                }

                Projectile.ai[0] = Main.tile[xy].TileType;
                Projectile.ai[1] = 2f;

                WorldGen.KillTile(xy.X, xy.Y, noItem: true);

                if (Main.netMode != NetmodeID.SinglePlayer) {
                    if (Main.myPlayer == Projectile.owner) {
                        tModLoaderExtended.ExtendedMod.GetPacket<PhysicsGunPickupBlockPacket>().Send(Projectile.owner, xy.X, xy.Y);
                    }
                    return false;
                }

                realBlock = true;
                return false;
            }
        }

        return true;
    }
    protected bool StepLaser(Point xy, out bool realBlock) {
        Tile tile = Main.tile[xy];
        ushort tileType = tile.TileType;
        Player player = Main.player[Projectile.owner];

        realBlock = false;

        // Shimmer Blocks the physics gun laser
        if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Shimmer) {
            return false;
        }

        if (tile.HasTile) {
            if (TileDataSet.PhysicsGunBlocksLaser.Contains(tileType)) {
                return false;
            }

            if (tile.IsFullySolid() && !Main.tileFrameImportant[tileType]) {
                if (TileDataSet.PhysicsGunCannotPickUp.Contains(tileType) || !player.HasEnoughPickPowerToHurtTile(xy.X, xy.Y) || !WorldGen.CanKillTile(xy.X, xy.Y)) {
                    return false;
                }

                Projectile.ai[0] = Main.tile[xy].TileType;
                Projectile.ai[1] = 2f;

                WorldGen.KillTile(xy.X, xy.Y, noItem: true);

                if (Main.netMode != NetmodeID.SinglePlayer) {
                    if (Main.myPlayer == Projectile.owner) {
                        tModLoaderExtended.ExtendedMod.GetPacket<PhysicsGunPickupBlockPacket>().Send(Projectile.owner, xy.X, xy.Y);
                    }
                    return false;
                }

                realBlock = true;
                return false;
            }
        }

        return true;
    }

    protected void PhysGunPickaxePowerHack(int pickaxePower) {
        Player player = Main.player[Projectile.owner];
        for (int k = 0; k < Main.InventoryItemSlotsCount; k++) {
            if (!player.inventory[k].IsAir && player.inventory[k].ModItem is PhysicsGun) {
                player.inventory[k].pick = pickaxePower;
            }
        }
    }

    public bool CheckEmancipationGrills() {
        var diff = Projectile.Center - (Projectile.oldPosition + Projectile.Size / 2f);
        int checkLength = Math.Max((int)(diff.Length() / 4f), 2);
        var checkCoords = Projectile.oldPosition;
        var velocity = Vector2.Normalize(diff);
        for (int i = 0; i < checkLength; i++) {
            var checkTileCoords = checkCoords.ToTileCoordinates();
            if (WorldGen.InWorld(checkTileCoords.X, checkTileCoords.Y, 10)) {
                Tile tile = Main.tile[checkTileCoords];
                if (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Shimmer) {
                    return true;
                }

                //if (Main.tile[checkTileCoords].HasTile && !Main.tile[checkTileCoords].IsActuated && Main.tile[checkTileCoords].TileType == ModContent.TileType<EmancipationGrillTile>()) {
                //    return true;
                //}
            }
            checkCoords += velocity * 4f;
        }
        return false;
    }

    public void EmancipationGrillEffects() {
        Projectile.ai[1] = 4f;
        Projectile.velocity *= Projectile.extraUpdates + 1;
        Projectile.velocity /= 4f;
        Projectile.timeLeft = 120;
        if (Projectile.velocity.Length() > 2f) {
            Projectile.velocity.Normalize();
            Projectile.velocity *= 2f;
        }
        Projectile.netUpdate = true;
        SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.33f, Pitch = 0.8f }, Projectile.Center);
    }

    public void GunLight() {
        var beamColor = mouseColor.HueShift(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 50f, -0.02f, 0.02f));
        Lighting.AddLight(Projectile.Center, beamColor.ToVector3() * 0.66f);
    }
    public void TileLight() {
        // TODO: make glowing tiles.. glow
    }
    public void SetArmRotation() {
        Player player = Main.player[Projectile.owner];
        Vector2 diff = Projectile.Center - player.Center;
        if (Math.Sign(diff.X) != player.direction) {
            player.ChangeDir(-player.direction);
        }

        diff.X = Math.Abs(diff.X);
        armRotation = Math.Clamp(-diff.ToRotation(), -MathHelper.PiOver2, 1f);

        base.SetArmRotation(Main.player[Projectile.owner]);
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if ((int)Projectile.ai[1] == 3) {
            var tileCoords = Projectile.Center.ToTileCoordinates();
            if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 10)) {
                PlaceTile();
                Projectile.Kill();
                return true;
            }
        }
        return false;
    }

    public override bool ShouldUpdatePosition() {
        return (int)Projectile.ai[1] != 1;
    }

    private Vector2[] _lineVertices;
    private Vector2[] _shortLineVertices;

    public override bool PreDraw(ref Color lightColor) {
        var beamColor = mouseColor.HueShift(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 50f, -0.03f, 0.03f));
        if ((int)Projectile.ai[1] < 3) {
            //mouseWorld = Main.player[Projectile.owner].MountedCenter - new Vector2(0f, 400f);
            var difference = Main.player[Projectile.owner].MountedCenter - mouseWorld;
            var dir = Vector2.Normalize(difference);

            Vector2[] vertices;
            Vector2 start = Main.player[Projectile.owner].MountedCenter + dir * -30f;
            Vector2 end = Projectile.Center;
            if ((Projectile.Center - start).Length() < 100f) {
                _shortLineVertices ??= new Vector2[2];
                vertices = _shortLineVertices;
            }
            else {
                _lineVertices ??= new Vector2[tModLoaderExtended.ExtendedMod.HighQualityEffects ? 22 : 9];
                vertices = _lineVertices;

                var linePosition = start;
                int amount = _lineVertices.Length - 2;
                float speed = (Projectile.Center - start).Length() / amount;
                var segmentVector = Vector2.Normalize(mouseWorld - start) * speed;
                for (int i = 1; i < _lineVertices.Length - 1; i++) {
                    linePosition += Vector2.Lerp(segmentVector, Vector2.Normalize(Projectile.Center - linePosition) * speed, i / (float)amount);
                    _lineVertices[i] = linePosition;
                }
            }

            vertices[0] = start;
            vertices[^1] = end;

            DrawHelper.DrawBasicVertexLine(Aequu2Textures.Trail, vertices, Helper.GenerateRotationArr(vertices),
                p => beamColor with { A = 0 },
                p => 4f,
            -Main.screenPosition);

            DrawGun();
        }

        if ((int)Projectile.ai[1] >= 2) {
            Main.instance.LoadTiles((int)Projectile.ai[0]);
            var t = TextureAssets.Tile[(int)Projectile.ai[0]].Value;
            var frame = new Rectangle(162, 54, 16, 16);
            var origin = frame.Size() / 2f;
            var drawColor = lightColor.MaxRGBA(128);

            var drawCoords = Projectile.Center - Main.screenPosition;

            if ((int)Projectile.ai[1] == 2 || (int)Projectile.ai[1] == 4) {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginWorld(shader: true);

                var s = GameShaders.Armor.GetSecondaryShader(ContentSamples.CommonlyUsedContentSamples.ColorOnlyShaderIndex, Main.LocalPlayer);
                var dd = new DrawData(t, Projectile.Center - Main.screenPosition, frame, beamColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                if ((int)Projectile.ai[1] == 4) {
                    drawColor = Color.Black * Projectile.Opacity;
                    dd.color = Color.White * Projectile.Opacity * Projectile.Opacity * Projectile.Opacity;
                }
                for (int i = 0; i < 4; i++) {
                    dd.position = drawCoords + (i * MathHelper.PiOver2).ToRotationVector2() * 2f;
                    s.Apply(null, dd);
                    dd.Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
                Main.spriteBatch.BeginWorld(shader: false); ;
            }
            Main.EntitySpriteDraw(t, drawCoords, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        return false;
    }

    public void DrawGun() {
        Projectile.GetDrawInfo(out var texture, out var _, out var frame, out var origin, out int _);
        frame.Width /= 2;
        origin = frame.Size() / 2f;

        var difference = Main.player[Projectile.owner].MountedCenter - mouseWorld;
        var dir = Vector2.Normalize(difference);
        var drawCoords = Main.player[Projectile.owner].MountedCenter + dir * -24f;
        float rotation = difference.ToRotation() + (Main.player[Projectile.owner].direction == -1 ? 0f : MathHelper.Pi);
        var spriteEffects = Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Main.EntitySpriteDraw(texture, drawCoords - Main.screenPosition, frame, ExtendLight.Get(drawCoords),
             rotation, origin, Projectile.scale, spriteEffects, 0);

        frame.X = frame.Width;
        var coloring = mouseColor;
        for (int i = 0; i < 4; i++) {
            Main.EntitySpriteDraw(texture, drawCoords + (i * MathHelper.PiOver2).ToRotationVector2() * Projectile.scale * 2f - Main.screenPosition, frame, (coloring * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0.2f, 0.5f)) with { A = 100 },
                rotation, origin, Projectile.scale, spriteEffects, 0);
        }
        Main.EntitySpriteDraw(texture, drawCoords - Main.screenPosition, frame, coloring,
            rotation, origin, Projectile.scale, spriteEffects, 0);
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.WriteVector2(mouseWorld);
        writer.WriteRGB(mouseColor);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        mouseWorld = reader.ReadVector2();
        mouseColor = reader.ReadRGB();
    }
}

public class PhysicsGunPickupBlockPacket : PacketHandler {
    public void Send(int Player, int X, int Y) {
        var p = GetPacket();
        p.Write(Player);
        p.Write(X);
        p.Write(Y);
        p.Send();
    }

    private void SendServer(int toClient) {
        GetPacket().Send(toClient: toClient);
    }

    public override void Receive(BinaryReader reader, int sender) {
        if (Main.netMode == NetmodeID.Server) {
            int Player = reader.ReadInt32();
            int X = reader.ReadInt32();
            int Y = reader.ReadInt32();

            Tile tile = Main.tile[X, Y];
            ushort tileType = tile.TileType;

            if (!tile.IsFullySolid() || Main.tileFrameImportant[tileType] || TileDataSet.PhysicsGunCannotPickUp.Contains(tileType) || !WorldGen.CanKillTile(X, Y)) {
                return;
            }

            WorldGen.KillTile(X, Y, noItem: true);
            NetMessage.SendTileSquare(-1, X, Y, 3);
            SendServer(Player);
            return;
        }

        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].ModProjectile is PhysicsGunProj physGun) {
                physGun.realBlock = true;
            }
        }
    }
}