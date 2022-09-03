using Aequus.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
                Color = new Color(200, 222, 255) * (Color.A / 255f);
            }
            else
            {
                Color = new Color(255, 222, 200) * (Color.A / 255f);
            }
            origin = new Vector2(9f, 9f);
        }

        public virtual void Update(ref ParticleRendererSettings settings)
        {
            timeAlive++;
            Velocity *= 0.985f;
            float velo = Velocity.Length();
            Rotation += velo * 0.0314f;
            Scale -= 0.01f + velo / 10000f;
            foreach (var m in activeManipulators)
            {
                m.InteractWithParticle(this);
            }
            if (Scale <= 0.1f)
            {
                ShouldBeRemovedFromRenderer = true;
                return;
            }
            Position += Velocity * 0.4f;
        }

        public virtual void UpdateManipulators()
        {
            activeManipulators.Clear();
            foreach (var m in DDParticleSystem.Manipulators)
            {
                if (Vector3.Distance(Position, m.Position) < m.InteractionRange)
                {
                    activeManipulators.Add(m);
                }
            }
        }

        public virtual void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            var drawPosition = OrthographicView.GetViewPoint(Dimension2D, Position.Z * 0.01f, new Vector2(Main.screenPosition.X + Main.screenWidth / 2f, Main.screenPosition.Y + Main.screenHeight / 2f)) - Main.screenPosition;
            var drawScale = OrthographicView.GetViewScale(Scale, Position.Z * 0.01f);

            float opacity = 1f;
            if (timeAlive < 80)
            {
                opacity = timeAlive / 80f;
                drawScale *= opacity;
            }

            spritebatch.Draw(Texture, drawPosition, frame, Color * opacity, Rotation, origin, drawScale, SpriteEffects.None, 0f);
        }
    }
}