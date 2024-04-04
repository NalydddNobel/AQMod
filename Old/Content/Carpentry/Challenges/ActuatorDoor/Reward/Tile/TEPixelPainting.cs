using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Clip;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Common;
using System.IO;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Tile;

public class TEPixelPainting : ModTileEntity {
    public PixelPaintingData mapCache;
    public long timeCreated;
    public Ref<RenderTarget2D> texture;
    private int netSpam = -1;

    public override bool IsTileValidForEntity(int x, int y) {
        return TileLoader.GetTile(Main.tile[x, y].TileType) is PixelPaintingTile;
    }

    public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate) {
        if (Main.netMode == NetmodeID.Server || Main.LocalPlayer.HeldItemFixed()?.ModItem is not PixelCameraClip clip || type < TileID.Count || TileLoader.GetTile(type) is not PixelPaintingTile painting) {
            return -1;
        }
        int offX = PixelCameraProj.StateDimensions[painting.StateID].X / 2;
        int offY = PixelCameraProj.StateDimensions[painting.StateID].Y / 2;
        int x = i - offX;
        int y = j - offY;
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            NetMessage.SendTileSquare(Main.myPlayer, x, y, 6, 4);
            var p = Aequus.GetPacket(PacketType.PlacePixelPainting);
            p.Write(x);
            p.Write(y);
            p.Write(clip.timeCreatedSerialized);
            clip.mapCache.NetSend(p);
            p.Send();
            return -1;
        }
        int id = Place(x, y);
        if (ByID.TryGetValue(id, out var te) && te is TEPixelPainting paintingTE) {
            paintingTE.timeCreated = clip.timeCreatedSerialized;
            paintingTE.mapCache = clip.mapCache;
        }
        return id;
    }

    public override void Update() {
        if (netSpam > 0)
            netSpam--;
    }

    public override void SaveData(TagCompound tag) {
        tag["TimeCreated"] = timeCreated;
        mapCache.Save(tag);
    }

    public override void LoadData(TagCompound tag) {
        timeCreated = tag.Get<long>("TimeCreated");
        mapCache = PixelPaintingData.Load(tag);
    }

    public override void NetSend(BinaryWriter writer) {
        base.NetSend(writer);
        writer.Write(timeCreated);
        if (netSpam > 0) {
            writer.Write(true);
            return;
        }
        netSpam += 30;
        if (netSpam < 0) {
            return;
        }
        writer.Write(false);
        mapCache.NetSend(writer);
    }

    public override void NetReceive(BinaryReader reader) {
        base.NetReceive(reader);
        timeCreated = reader.ReadInt64();
        if (reader.ReadBoolean()) {
            return;
        }

        mapCache = PixelPaintingData.NetReceive(reader);
    }

    public override void OnNetPlace() {
        netSpam = -30;
    }
}