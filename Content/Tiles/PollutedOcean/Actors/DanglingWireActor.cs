using Aequus.Common.Drawing;
using Aequus.Common.Entities.TileActors;
using Aequus.Common.Structures.PhysicsBehaviors;
using Aequus.Common.Utilities;
using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Tiles.PollutedOcean.Actors;

public class DanglingWireActor : GridActor, IGridDrawSystem {
    public const float NodeLength = 16f;
    public Point Endpoint;
    public byte Length;

    public VIStringTwoPoint<VINode>? visualization;

    void DrawAllActors(SpriteBatch sb) {
        foreach (DanglingWireActor a in this.GetDrawable()) {
            if (a.visualization == null) {
                a.visualization = CreateVisualization(a.Location.ToWorldCoordinates(), a.Endpoint.ToWorldCoordinates(), a.Length);
                for (int i = 0; i < a.visualization.segments.Length * 4; i++) {
                    a.visualization.Update();
                }
            }

            // Todo -- Move physics to a method which isnt determined by frame count.
            a.visualization.Update();

            for (int i = 1; i < a.visualization.segments.Length; i++) {
                var start = a.visualization.segments[i].Position;
                var end = a.visualization.segments[i - 1].Position;
                DrawHelper.DrawLine(Main.spriteBatch.Draw, start - Main.screenPosition, end - Main.screenPosition, 2f, Color.Black);
            }
            DrawHelper.DrawLine(Main.spriteBatch.Draw, a.Endpoint.ToWorldCoordinates() - Main.screenPosition, a.visualization.segments[^1].Position - Main.screenPosition, 2f, Color.Black);
        }
    }

    void IDrawSystem.Activate() {
        Instance<DrawLayers>().WorldBehindTiles += DrawAllActors;
    }

    void IDrawSystem.Deactivate() {
        Instance<DrawLayers>().WorldBehindTiles -= DrawAllActors;
    }

    public override void SendData(BinaryWriter writer) {
        writer.Write(Endpoint.X);
        writer.Write(Endpoint.Y);
        writer.Write(Length);
    }

    public override void ReceiveData(BinaryReader reader) {
        Endpoint.X = reader.ReadInt32();
        Endpoint.Y = reader.ReadInt32();
        Length = reader.ReadByte();
    }

    const string Tag_Endpoint = "End";
    const string Tag_Length = "Seg";

    public override void SaveData(TagCompound tag) {
        tag[Tag_Endpoint] = Endpoint;
        tag[Tag_Length] = Length;
    }

    public override void LoadData(TagCompound tag) {
        Endpoint = tag.GetOrDefault(Tag_Endpoint, Location + new Point(0, 10));
        Length = tag.GetOrDefault<byte>(Tag_Length, 0);
    }

    public static int GetWantedSegmentCount(Vector2 Startpoint, Vector2 Endpoint, byte addedLength) {
        return Math.Max((int)(MathF.Ceiling(Vector2.Distance(Startpoint, Endpoint) / NodeLength) + addedLength), 3);
    }

    public static VIStringTwoPoint<VINode> CreateVisualization(Vector2 Startpoint, Vector2 Endpoint, byte length) {
        int segmentsWanted = GetWantedSegmentCount(Startpoint, Endpoint, length);
        return new VIStringTwoPoint<VINode>(Startpoint, Endpoint, segmentsWanted, NodeLength, new Vector2(0f, 0.3f), 0f, accuracy: 15);
    }

    public static bool ValidTile(Tile tile) {
        return tile.IsSolid();
    }
}

#if POLLUTED_OCEAN
public class DanglingWirePreviewProj : ModProjectile {
    public override string Texture => AequusTextures.Item(ItemID.CombatWrench);

    public VIStringTwoPoint<VINode>? visualization;

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.DisableWorldInteractions();
    }

    public override void AI() {
        if (Main.myPlayer != Projectile.owner) {
            return;
        }

        int x = Player.tileTargetX;
        int y = Player.tileTargetY;
        Player player = Main.player[Projectile.owner];

        if (player.HeldItem.ModItem is not DanglingWirePlacer) {
            Projectile.Kill();
            return;
        }

        visualization?.Update();

        if (!player.IsInTileInteractionRange(x, y, TileReachCheckSettings.Simple)) {
            if (visualization == null) {
                Projectile.Kill();
            }
            return;
        }

        byte length = (byte)Projectile.ai[1];

        if (player.controlUp) {
            if (Projectile.localAI[0] <= 0f) {
                Projectile.ai[1]++;
                if (Projectile.ai[1] > 10f) {
                    Projectile.ai[1] = 10f;
                }
            }

            Projectile.localAI[0] = 1f;
        }
        else if (player.controlDown) {
            if (Projectile.localAI[0] >= 0f) {
                Projectile.ai[1]--;
                if (Projectile.ai[1] < 0f) {
                    Projectile.ai[1] = 0f;
                }
            }

            Projectile.localAI[0] = -1f;
        }
        else {
            Projectile.localAI[0] = 0f;
        }

        Vector2 hoverCoords = new Vector2(x, y).ToWorldCoordinates();
        if (visualization == null) {
            Tile tile = Framing.GetTileSafely(hoverCoords.ToTileCoordinates());
            if (!DanglingWireActor.ValidTile(tile)) {
                Projectile.Kill();
                return;
            }

            Projectile.Center = hoverCoords;
            visualization = DanglingWireActor.CreateVisualization(hoverCoords, hoverCoords + Vector2.UnitY, length);
        }

        Projectile.Center = Vector2.Lerp(Projectile.Center, hoverCoords, 0.3f);
        visualization.EndPosition = hoverCoords;

        int totalLength = DanglingWireActor.GetWantedSegmentCount(visualization.StartPos, hoverCoords, length);
        if (totalLength != visualization.segments.Length) {
            Array.Resize(ref visualization.segments, totalLength);
            for (int i = 0; i < totalLength; i++) {
                if (visualization.segments[i].Position == Vector2.Zero) {
                    visualization.segments[i].Position = hoverCoords;
                    visualization.segments[i].OldPosition = hoverCoords + Vector2.UnitY;
                }
            }
        }

        if (!player.ItemAnimationActive) {
            Projectile.localAI[1] = 1f;
        }
        else if (Projectile.localAI[1] > 0f) {
            Tile tile = Framing.GetTileSafely(x, y);
            if (DanglingWireActor.ValidTile(tile)) {
                Projectile.Kill();
                DanglingWireActor a = (DanglingWireActor)Instance<GridActorSystem>().Place(x, y, Instance<DanglingWireActor>().Type);

                // Make sure its in a resting position.
                for (int i = 0; i < totalLength * 4; i++) {
                    visualization.Update();
                }

                for (int i = 0; i < totalLength; i++) {
                    Vector2 where = visualization.segments[i].Position - new Vector2(8f, 8f);

                    for (int j = 0; j < 4; j++) {
                        Dust d = Dust.NewDustDirect(where, 16, 16, DustID.Smoke, Alpha: 150);
                        d.velocity *= 0.25f;
                    }
                }

                Instance<TileDrawSystem>().AddGridActor(a);

                a.Endpoint = visualization.StartPos.ToTileCoordinates();
                a.Length = length;

                Instance<GridActorPacket>().Send(a);
            }
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        if (visualization == null) {
            return false;
        }

        for (int i = 1; i < visualization.segments.Length; i++) {
            var start = visualization.segments[i].Position;
            var end = visualization.segments[i - 1].Position;
            DrawHelper.DrawLine(Main.EntitySpriteDraw, start - Main.screenPosition, end - Main.screenPosition, 2f, Color.Black);
        }
        DrawHelper.DrawLine(Main.EntitySpriteDraw, Projectile.Center - Main.screenPosition, visualization.segments[^1].Position - Main.screenPosition, 2f, Color.Black);

        return false;
    }
}

public class DanglingWirePlacer : ModItem {
    public override string Texture => AequusTextures.Item(ItemID.CombatWrench);

    public override void SetDefaults() {
        Item.width = 16;
        Item.height = 16;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.tileBoost = 5;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.color = Color.Blue;
        Item.shoot = ModContent.ProjectileType<DanglingWirePreviewProj>();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        return player.ownedProjectileCounts[Item.shoot] == 0;
    }
}
#endif