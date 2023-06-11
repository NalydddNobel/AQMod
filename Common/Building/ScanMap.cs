using Microsoft.Xna.Framework;
using System;

namespace Aequus.Common.Building {
    public class ScanMap<T> {
        private T[,] map;
        public readonly int Width;
        public readonly int Height;
        
        public T this[int x, int y] {
            get => map[x, y];
            set => map[x, y] = value;
        }

        private ScanMap() {
        }

        public ScanMap(int width, int height) {
            map = new T[width, height];
            Width = width; 
            Height = height;
        }

        public bool InBounds(int x, int y) {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public void SafeSet(int x, int y, T value) {
            if (InBounds(x, y)) {
                this[x, y] = value;
            }
        }

        public bool SafeGet(int x, int y, out T value) {
            if (InBounds(x, y)) {
                value = this[x, y];
                return true;
            }
            value = default;
            return false;
        }

        public (int MinX, int MaxX, int MinY, int MaxY) GetBounds(Predicate<T> predicate) {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (predicate(this[x, y])) {
                        minX = Math.Min(x, minX);
                        maxX = Math.Max(x, maxX);
                        minY = Math.Min(y, minY);
                        maxY = Math.Max(y, maxY);
                    }
                }
            }

            return (minX, maxX, minY, maxY);
        }

        public Rectangle GetBoundsRectangle(Predicate<T> predicate, int offsetX = 0, int offsetY = 0) {
            var bounds = GetBounds(predicate);

            if (bounds.MinX == int.MaxValue) {
                return Rectangle.Empty;
            }
            return new(bounds.MinX + offsetX, bounds.MinY + offsetY, bounds.MaxX - bounds.MinX + 1, bounds.MaxY - bounds.MinY + 1);
        }
    }
}
