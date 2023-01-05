using Aequus.Items.Tools.Camera;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace Aequus.Content.CarpenterBounties
{
    public struct StepInfo
    {
        public TileMapCache Map { get; private set; }
        public Rectangle SamplingArea { get; private set; }
        public int Width => Map.Width;
        public int Height => Map.Height;

        public TileDataCache this[int i, int j] { get => Map[i, j]; set => Map[i, j] = value; }
        public TileDataCache this[Point p] { get => Map[p]; set => Map[p] = value; }

        public List<StepInterest> interests;

        public StepInfo()
        {
            Map = null;
            SamplingArea = Rectangle.Empty;
            interests = new List<StepInterest>();
        }

        public StepInfo(TileMapCache map, Rectangle sampleRectangle) : this()
        {
            Map = map;
            SamplingArea = sampleRectangle;
        }

        public StepInfo(Rectangle sampleRectangle) : this(new TileMapCache(sampleRectangle), sampleRectangle)
        {
        }

        public StepInfo(ShutterstockerClip clip) : this()
        {
            Map = clip.tileMap;
            SamplingArea = new Rectangle((int)(clip.worldXPercent * Main.maxTilesX), (int)(clip.worldYPercent * Main.maxTilesY),
                clip.tileMap.Width, clip.tileMap.Height);
        }

        public void AddInterest<T>(T instance) where T : StepInterest
        {
            if (GetInterest<T>() != null)
                return;
            interests.Add(instance);
        }

        public T GetInterest<T>() where T : StepInterest
        {
            return (T)interests.Find((i) => i is T);
        }

        public void SwapWorldSample()
        {
            var map = new TileMapCache(SamplingArea);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    var p = new Point(SamplingArea.X + i, SamplingArea.Y + j);
                    Main.tile[p].Get<TileTypeData>() = this[i, j].Type;
                    Main.tile[p].Get<LiquidData>() = this[i, j].Liquid;
                    Main.tile[p].Get<TileWallWireStateData>() = this[i, j].Misc;
                    Main.tile[p].Get<WallTypeData>() = this[i, j].Wall;
                    Main.tile[p].Get<AequusTileData>() = this[i, j].Aequus;
                }
            }
            Map = map;
        }
    }
}