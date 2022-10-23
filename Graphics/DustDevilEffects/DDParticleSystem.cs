using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics.DustDevilEffects
{
    public class DDParticleSystem : ModSystem
    {
        public static List<IDDParticleManipulator> Manipulators;
        public static List<DDParticle> Particles;
        public static List<DDParticle> CachedBackParticles;
        public static List<DDParticle> CachedFrontParticles;

        public static int HardUpdate;

        public static void Layer(DDParticle p)
        {
            if (p.Z < 0f)
            {
                CachedBackParticles.Add(p);
                return;
            }
            CachedFrontParticles.Add(p);
        }

        public static void AddParticle(DDParticle p)
        {
            Particles.Add(p);
            p.UpdateManipulators();
            Layer(p);
            HardUpdate--;
        }

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Particles = new List<DDParticle>();
                CachedBackParticles = new List<DDParticle>();
                CachedFrontParticles = new List<DDParticle>();
                Manipulators = new List<IDDParticleManipulator>();
                DDParticle.Texture = ModContent.Request<Texture2D>("Aequus/NPCs/Boss/DustDevilDust", AssetRequestMode.ImmediateLoad).Value;
            }
        }

        public override void PreUpdateNPCs()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var settings = new ParticleRendererSettings();
                for (int i = 0; i < Particles.Count; i++)
                {
                    Particles[i].Update(ref settings);
                    if (Particles[i].ShouldBeRemovedFromRenderer)
                    {
                        Particles.RemoveAt(i);
                        i--;
                    }
                }

                UpdateDustLayering();
            }
        }

        public static void UpdateDustLayering()
        {
            HardUpdate--;
            if (HardUpdate <= 0)
            {
                HardUpdate = 15 + Math.Max(60 - Main.frameRate, 0);
                CachedBackParticles.Clear();
                CachedFrontParticles.Clear();
                for (int i = 0; i < Manipulators.Count; i++)
                {
                    if (!Manipulators[i].IsActive())
                    {
                        Manipulators.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < Particles.Count; i++)
                {
                    Layer(Particles[i]);
                    Particles[i].UpdateManipulators();
                }
            }
        }
    }
}