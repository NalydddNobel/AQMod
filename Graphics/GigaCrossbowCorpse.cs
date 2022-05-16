using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class GigaCrossbowCorpse : ABasicParticle
    {
        public static HashSet<int> NPCBlacklist { get; private set; }

        public sealed class Loader : ILoadable
        {
            void ILoadable.Load(Mod mod)
            {
                NPCBlacklist = new HashSet<int>()
                {
                    NPCID.GolemHead,
                    NPCID.PrimeCannon,
                    NPCID.PrimeSaw,
                    NPCID.PrimeVice,
                    NPCID.PrimeLaser,
                    NPCID.SkeletronHand,
                };
            }

            void ILoadable.Unload()
            {
                NPCBlacklist?.Clear();
                NPCBlacklist = null;
            }
        }

        public static int AmtOld;
        public static int Amt;

        public NPC npc;

        public int timeActive;

        public int maxTimeActive;

        private bool _anyErrors;

        private int _width;
        private int _height;

        private Vector2 _scale;
        private Vector2 _iceOrigin;
        private Vector2 _bloomOrigin;

        public Vector2 TopLeft { get => LocalPosition - new Vector2(_width / 2f, _height / 2f); set => LocalPosition = value + new Vector2(_width / 2f, _height / 2f); }

        public GigaCrossbowCorpse(Vector2 position, NPC npc)
        {
            maxTimeActive = 10800;
            LocalPosition = position;
            this.npc = AequusHelpers.CreateSudo(npc);
            this.npc.active = true;
            this.npc.hide = false;
            this.npc.alpha = Math.Min(npc.alpha, 200);
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            if (npc != null)
            {
                try
                {
                    npc.Center = LocalPosition;
                    Main.instance.LoadNPC(npc.type);

                    var drawCoordinates = LocalPosition - Main.screenPosition;
                    drawCoordinates.Y += 4f;
                    var iceTexture = TextureAssets.Frozen.Value;
                    var npcTexture = TextureAssets.Npc[npc.type].Value;

                    var iceColor = new Color(210, 222, 255, 20) * 0.8f;
                    if (_scale == Vector2.Zero)
                    {
                        _scale = Draw_DetermineIceSize(iceTexture, npc.frame);
                        _width = (int)(iceTexture.Width * _scale.X);
                        _height = (int)(iceTexture.Height * _scale.Y);
                    }
                    if (_iceOrigin == Vector2.Zero)
                    {
                        _iceOrigin = iceTexture.Size() / 2f;
                    }

                    foreach (var v in AequusHelpers.CircularVector(8, Main.GlobalTimeWrappedHourly))
                    {
                        spritebatch.Draw(iceTexture, drawCoordinates + v * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 0.25f, 2f, 8f), null, iceColor * 0.125f, 0f, _iceOrigin, _scale, SpriteEffects.None, 0f);
                    }

                    var bloom = Images.Bloom[0].Value;
                    if (_bloomOrigin == Vector2.Zero)
                    {
                        _bloomOrigin = bloom.Size() / 2f;
                    }
                    spritebatch.Draw(bloom, drawCoordinates, null, Color.Blue * 0.75f, 0f, _bloomOrigin, _scale * 0.7f, SpriteEffects.None, 0f);

                    Main.instance.DrawNPCDirect(spritebatch, npc, npc.behindTiles, Main.screenPosition);

                    spritebatch.Draw(iceTexture, drawCoordinates, null, iceColor, 0f, _iceOrigin, _scale, SpriteEffects.None, 0f);
                }
                catch
                {
                    _anyErrors = true;
                }
            }
        }
        private Vector2 Draw_DetermineIceSize(Texture2D iceTexture, Rectangle npcFrame)
        {
            var scale = new Vector2(1f, 1f);
            int widthComparison = npcFrame.Width + 4;
            int heightComparison = npcFrame.Height + 4;
            if (iceTexture.Width < widthComparison)
            {
                scale.X = widthComparison / (float)iceTexture.Width;
            }
            if (iceTexture.Height < heightComparison)
            {
                scale.Y = heightComparison / (float)iceTexture.Height;
            }
            return scale;
        }

        public static bool CanConvertIntoCorpse(NPC npc)
        {
            return npc.realLife == -1 && npc.lifeMax > 5 && npc.aiStyle != NPCAIStyleID.Worm && npc.aiStyle != NPCAIStyleID.TheDestroyer && !NPCBlacklist.Contains(npc.type);
        }

        internal static void ResetCounts()
        {
            AmtOld = Amt;
            Amt = 0;
        }
    }
}