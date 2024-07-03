using Aequu2.Core.Graphics;
using Aequu2.Core.Particles;
using Terraria.GameContent;

namespace Aequu2.Old.Content.Items.Weapons.Ranged.Raygun;

public class RaygunTrail : ConcurrentParticles<RaygunTrail.Particle> {
    public override void Activate() {
        DrawLayers.Instance.PostDrawLiquids += Draw;
    }

    public override void Deactivate() {
        DrawLayers.Instance.PostDrawLiquids -= Draw;
    }

    public override void Draw(SpriteBatch spriteBatch, Particle t) {
        Main.instance.LoadProjectile(ProjectileID.PrincessWeapon);
        Texture2D texture = TextureAssets.Projectile[ProjectileID.PrincessWeapon].Value;
        Vector2 drawCoordinates = t.Position - Main.screenPosition;
        float rotation = t.Velocity.ToRotation();
        Vector2 scale = new Vector2(0.1f, 0.2f) * t.Scale;
        Color color = t.Color * t.Alpha;
        spriteBatch.Draw(texture, drawCoordinates, null, color, rotation, texture.Size() / 2f, scale, SpriteEffects.None, 0f);
    }

    public override void Update(Particle t) {
        t.Scale *= 1.07f;
        t.Alpha *= 0.6f;
        t.Active = t.Alpha > 0.01f;
    }

    public class Particle : IParticle {
        public bool Active { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Scale;
        public float Alpha = 1f;

        public void Setup(Vector2 position, Vector2 velocity, Color color, float scale = 1f) {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
        }
    }
}
