namespace AQMod.Projectiles.Ranged
{
    public sealed class FlameblasterWind : FriendlyWind
    {
        public override string Texture => AQMod.TextureNone;

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.ranged = true;
        }
    }
}