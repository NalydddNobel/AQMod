namespace AQMod.Content.WorldEffects
{
    public abstract class WorldVisualEffect
    {
        public int x;
        public int y;

        public WorldVisualEffect(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract bool Update();
    }
}