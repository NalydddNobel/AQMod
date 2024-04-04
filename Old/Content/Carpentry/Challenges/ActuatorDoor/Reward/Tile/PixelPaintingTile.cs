using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Clip;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Common;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Tile;

public abstract class PixelPaintingTile : ModTile {
    public abstract int StateID { get; }

    public static int[] PhotoStateToTileID { get; private set; }

    public override void Load() {
        PhotoStateToTileID ??= new int[PixelCameraProj.StateID.Count];
    }

    public override void SetStaticDefaults() {
        PhotoStateToTileID[StateID] = Type;
        Main.tileFrameImportant[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
        TileObjectData.newTile.Width = PixelCameraProj.StateDimensions[StateID].X;
        TileObjectData.newTile.Height = PixelCameraProj.StateDimensions[StateID].Y;
        TileObjectData.newTile.CoordinateHeights = new int[PixelCameraProj.StateDimensions[StateID].Y];
        for (int i = 0; i < TileObjectData.newTile.CoordinateHeights.Length; i++) {
            TileObjectData.newTile.CoordinateHeights[i] = 16;
        }
        TileObjectData.newTile.Origin = new Point16(TileObjectData.newTile.Width / 2, TileObjectData.newTile.Height / 2);
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TEPixelPainting>().Hook_AfterPlacement, -1, 0, false);

        TileObjectData.addTile(Type);
        DustType = DustID.WoodFurniture;
        AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Painting"));
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY) {
        int x = i - Main.tile[i, j].TileFrameX / 18;
        int y = j - Main.tile[i, j].TileFrameY / 18;
        if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out var te) && te is TEPixelPainting painting) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                int itemIndex = Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16,
                    PixelCameraProj.StateDimensions[StateID].X * 16, PixelCameraProj.StateDimensions[StateID].Y * 16, ModContent.ItemType<PixelCameraClip>());
                if (itemIndex != -1 && itemIndex < Main.maxItems) {
                    var clip = Main.item[itemIndex].ModItem as PixelCameraClip;
                    clip.mapCache = painting.mapCache;
                    clip.timeCreatedSerialized = painting.timeCreated;
                    clip.photoState = StateID;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncItem, number: itemIndex);
                }
            }
        }
        ModContent.GetInstance<TEPixelPainting>().Kill(i, j);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        int x = i - Main.tile[i, j].TileFrameX / 18;
        int y = j - Main.tile[i, j].TileFrameY / 18;
        if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out var te) && te is TEPixelPainting painting) {
            painting.texture ??= new Ref<RenderTarget2D>();
            if (painting.texture.Value == null) {
                if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0) {
                    PixelCameraRenderer.RenderRequests.Add(new PixelCameraRenderer.RequestInfo() { width = painting.mapCache.width, height = painting.mapCache.height, arr = painting.mapCache.colorLookup, target = painting.texture, });
                }
            }
            else if (painting.texture != null && painting.texture.Value != null) {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Matrix.Identity);
                int frameX = Main.tile[i, j].TileFrameX / 18;
                int frameY = Main.tile[i, j].TileFrameY / 18;
                Main.spriteBatch.Draw(painting.texture.Value, new Vector2(i * 16f, j * 16f) + TileHelper.DrawOffset - Main.screenPosition,
                    new Rectangle(frameX * 8, frameY * 8, 8, 8), Lighting.GetColor(i, j), 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }
        }
        return false;
    }
}

public class PixelPainting_2x2 : PixelPaintingTile {
    public override int StateID => PixelCameraProj.StateID.State_2x2;
}

public class PixelPainting_2x3 : PixelPaintingTile {
    public override int StateID => PixelCameraProj.StateID.State_2x3;
}

public class PixelPainting_3x2 : PixelPaintingTile {
    public override int StateID => PixelCameraProj.StateID.State_3x2;
}

public class PixelPainting_3x3 : PixelPaintingTile {
    public override int StateID => PixelCameraProj.StateID.State_3x3;
}

public class PixelPainting_6x4 : PixelPaintingTile {
    public override int StateID => PixelCameraProj.StateID.State_6x4;
}