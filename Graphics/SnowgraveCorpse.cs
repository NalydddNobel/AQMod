using Aequus.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public class SnowgraveCorpse : ABasicParticle
    {
        public static HashSet<int> NPCBlacklist { get; private set; }
        /// <summary>
        /// Parameter 1: {NPC} - The NPC which is being sudo cloned
        /// <para>Parameter 2: {NPC} - The clone result</para>
        /// </summary>
        public static Dictionary<int, Action<NPC, NPC>> OnFreezeNPC { get; private set; }
        /// <summary>
        /// Parameter 1: {NPC} - The NPC which is being sudo cloned
        /// <para>Parameter 2: {NPC} - The NPC</para>
        /// </summary>
        public static Dictionary<int, Func<SpriteBatch, ABasicParticle, ParticleRendererSettings, NPC, bool>> CustomDraw { get; private set; }
        /// <summary>
        /// Parameter 1: {ABasicParticle} - Will always be FrozenNPC, but for soft reference purposes, this is left as a generic vanilla class.
        /// <para>Parameter 2: {NPC} - The NPC</para>
        /// </summary>
        public static Dictionary<int, Func<ABasicParticle, ParticleRendererSettings, NPC, bool>> CustomUpdate { get; private set; }

        public static SoundStyle SizzleSound { get; private set; }

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
                OnFreezeNPC = new Dictionary<int, Action<NPC, NPC>>();
                CustomDraw = new Dictionary<int, Func<SpriteBatch, ABasicParticle, ParticleRendererSettings, NPC, bool>>();
                CustomUpdate = new Dictionary<int, Func<ABasicParticle, ParticleRendererSettings, NPC, bool>>();
            
                if (!Main.dedServ)
                {
                    SizzleSound = Aequus.GetSound("sizzle");
                }
            }

            void ILoadable.Unload()
            {
                NPCBlacklist?.Clear();
                NPCBlacklist = null;
                OnFreezeNPC?.Clear();
                OnFreezeNPC = null;
                CustomDraw?.Clear();
                CustomDraw = null;
                CustomUpdate?.Clear();
                CustomUpdate = null;
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

        public SnowgraveCorpse(Vector2 position, NPC npc)
        {
            maxTimeActive = 10800;
            LocalPosition = position;
            this.npc = AequusHelpers.CreateSudo(npc);
            if (OnFreezeNPC.TryGetValue(npc.netID, out var onFreeze))
            {
                onFreeze(npc, this.npc);
            }
            this.npc.active = true;
            this.npc.hide = false;
            this.npc.alpha = Math.Min(npc.alpha, 200);
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (npc == null || _anyErrors)
            {
                ShouldBeRemovedFromRenderer = true;
            }
            else
            {
                if (Amt == 0 && AmtOld > (Aequus.HQ ? 30 : 10))
                {
                    timeActive = Math.Max(timeActive, maxTimeActive - 60);
                }
                timeActive++;
                if (timeActive > maxTimeActive)
                {
                    Kill(lavaDeath: false);
                    AmtOld--;
                    ShouldBeRemovedFromRenderer = true;
                }
                Amt++;
                if (CustomUpdate.TryGetValue(npc.netID, out var customUpdate) && !customUpdate(this, settings, npc))
                {
                    return;
                }

                if (Collision.LavaCollision(TopLeft, _width, _height))
                {
                    ShouldBeRemovedFromRenderer = true;
                    SoundEngine.PlaySound(SizzleSound, LocalPosition);
                    Kill(lavaDeath: true);
                    return;
                }

                var slopeResult = Collision.WalkDownSlope(TopLeft, Velocity, _width, _height, 0.3f);
                TopLeft = new Vector2(slopeResult.X, slopeResult.Y);
                Velocity.X = slopeResult.Z;
                Velocity.Y = slopeResult.W;

                Velocity.Y += 0.3f;

                Velocity = Collision.TileCollision(TopLeft, Velocity, _width, _height);

                LocalPosition += Velocity;
            }
        }
        private void Kill(bool lavaDeath = false)
        {
            npc.HitEffect(0, npc.lifeMax);
            var topLeft = TopLeft;
            if (lavaDeath)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(topLeft, _width, _height, DustID.Smoke);
                }
            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(topLeft, _width, _height, DustID.Ice);
                }
            }
            SoundEngine.PlaySound(SoundID.Shatter, LocalPosition);
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


                    if (!CustomDraw.TryGetValue(npc.netID, out var customDraw) || customDraw(spritebatch, this, settings, npc))
                    {
                        Main.instance.DrawNPCDirect(spritebatch, npc, npc.behindTiles, Main.screenPosition);
                    }

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

        public static bool CanFreezeNPC(NPC npc)
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