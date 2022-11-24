using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Graphics.DustDevilEffects
{
    public class DDParticle
    {
        public static Texture2D Texture;

        public Vector3 Position;
        public Vector3 Velocity;
        public Color Color;
        public float Scale;
        public float Rotation;

        public Rectangle frame;
        public Vector2 origin;

        public int timeAlive;

        public readonly uint Identifier;

        public static uint NextIdentifier;

        public Vector2 Dimension2D => new Vector2(Position.X, Position.Y);
        public float Z => Position.Z;

        public List<IDDParticleManipulator> activeManipulators;

        public bool ShouldBeRemovedFromRenderer { get; private set; }

        public DDParticle(Vector3 position, Vector3 velocity, Color color = default(Color), float scale = 1f, float rotation = 0f)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Rotation = rotation;
            Identifier = NextIdentifier;
            NextIdentifier++;
            activeManipulators = new List<IDDParticleManipulator>();
            OnAdd();
        }

        public virtual void OnAdd()
        {
            frame = new Rectangle(20 * Main.rand.Next(2), 20 * Main.rand.Next(20), 18, 18);
            if (frame.X > 0)
            {
                Color = Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0f, 0.5f)) * (Color.A / 255f);
            }
            else
            {
                Color = Color.Lerp(Color.White, Color.Orange, Main.rand.NextFloat(0f, 0.5f)) * (Color.A / 255f);
            }
            origin = new Vector2(9f, 9f);
        }

        public virtual void Update(ref ParticleRendererSettings settings)
        {
            timeAlive++;
            float velo = Velocity.Length();
            Scale -= 0.01f + velo / 7500f;
            foreach (var m in activeManipulators)
            {
                m.InteractWithParticle(this);
            }
            if (Scale <= 0.1f)
            {
                ShouldBeRemovedFromRenderer = true;
                return;
            }
            Velocity *= 0.985f;
            Rotation += velo * 0.0314f;
            Position += Velocity * 0.3f;
        }

        public virtual void UpdateManipulators()
        {
            activeManipulators.Clear();
            foreach (var m in DustDevilParticleSystem.Manipulators)
            {
                if (Vector3.Distance(Position, m.Position) < m.InteractionRange)
                {
                    activeManipulators.Add(m);
                }
            }
        }

        public virtual void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            float zMult = 0.01f;
            if (Identifier % 50 == 0)
                zMult = 0.1f;
            var drawPosition = OrthographicView.GetViewPoint(Dimension2D, Position.Z * zMult, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f)) - Main.screenPosition;
            var drawScale = OrthographicView.GetViewScale(Scale, Position.Z * zMult);

            float opacity = 1f;
            if (timeAlive < 80)
            {
                opacity = timeAlive / 80f;
                drawScale *= opacity;
            }
            if (Position.Z * Scale > 50f)
            {
                opacity -= Position.Z / 100f;
                if (opacity < 0f)
                    return;
            }

            spritebatch.Draw(Texture, drawPosition, frame, AequusHelpers.GetColor(drawPosition + Main.screenPosition, Color * (1f - Color.A / 255f + 1f)) * opacity * (Color.A / 255f) * 0.75f, Rotation, origin, drawScale, SpriteEffects.None, 0f);
        }
    }
}