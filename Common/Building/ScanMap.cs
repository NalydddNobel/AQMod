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
    }
}
