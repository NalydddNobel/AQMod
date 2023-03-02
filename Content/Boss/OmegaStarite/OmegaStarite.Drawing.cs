using Aequus.Common.Effects;
using Aequus.Common.Preferences;
using Aequus.Common.Primitives;
using Aequus.Common.Utilities;
using Aequus.Content.Boss.OmegaStarite.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.OmegaStarite
{
    public partial class OmegaStarite
    {
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                if ((int)NPC.ai[0] == 0)
                {
                    Initalize(bestiaryDummy: true);
                    NPC.ai[0]++;
                }
                LerpToDefaultRotationVelocity();
                for (int i = 0; i < rings.Count; i++)
                {
                    rings[i].Update(NPC.Center);
                }
                NPC.scale = 0.5f;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }

    public abstract partial class OmegaStariteBase
    {
        public List<DrawData> Draw_BackBlooms { get; private set; }
        public List<DrawData> Draw_Back { get; private set; }
        public List<DrawData> Draw_Front { get; private set; }
        private List<OrbRenderData> _orbs;

        public record struct OrbRenderData(NPC npc, OmegaStariteRing parentRing, int ringIndex, int orbIndex, Vector3 position);

        private TrailRenderer prim;

        private void CheckInitializations()
        {
            if (_orbs == null)
            {
                _orbs = new List<OrbRenderData>();
            }
            if (Draw_BackBlooms == null)
            {
                Draw_BackBlooms = new List<DrawData>();
            }
            if (Draw_Back == null)
            {
                Draw_Back = new List<DrawData>();
            }
            if (Draw_Front == null)
            {
                Draw_Front = new List<DrawData>();
            }
        }

        public OrbRenderData GetOrbDrawData(List<OmegaStariteRing> ring, int ringIndex, int orbIndex)
        {
            return new OrbRenderData(
                NPC, 
                ring[ringIndex], ringIndex, orbIndex, 
                ring[ringIndex].CachedPositions[orbIndex]
                );
        }

        public virtual void DrawOrb(OrbRenderData drawData, SpriteBatch spriteBatch, Vector2 drawPos, float drawScale, Vector2 viewPos)
        {
            var texture = TextureAssets.Projectile[ModContent.ProjectileType<OmegaStariteProj>()].Value;
            var frame = texture.Frame(verticalFrames: 2, frameY: 0);

            Draw_BackBlooms.Add(new DrawData(
                texture,
                drawPos,
                frame.Frame(0, 1),
                Color.Lerp(Color.White, Color.Blue, Helper.Wave(Main.GlobalTimeWrappedHourly, 0.33f, 0.66f)),
                0f,
                frame.Size() / 2f,
                drawScale * 1.05f, SpriteEffects.None, 0
            ));

            var l = drawData.position.Z < 0f ? Draw_Front : Draw_Back;

            l.Add(new DrawData(
                texture,
                drawPos,
                frame,
                Color.White,
                0f,
                frame.Size() / 2f,
                drawScale, SpriteEffects.None, 0
            ));
        }

        public void PrepareOrbRenders()
        {
            if (rings == null)
                return;

            CheckInitializations();

            _orbs.Clear();

            for (int i = 0; i < rings.Count; i++)
            {
                for (int j = 0; j < rings[i].OrbCount; j++)
                {
                    _orbs.Add(GetOrbDrawData(rings, i, j));
                }
            }

            _orbs.Sort((o, o2) => -o.position.Z.CompareTo(o2.position.Z));
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public void DrawBody(SpriteBatch spriteBatch, Vector2 drawPos, Color drawColor)
        {
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);

            foreach (var c in Helper.CircularVector(4, Main.GameUpdateCount * 5f))
            {
                spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawPos + c * 4f, NPC.frame, drawColor with { A = 0 } * 0.04f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (rings == null)
            {
                return false;
            }

            CheckInitializations();
            Draw_BackBlooms.Clear();
            Draw_Back.Clear();
            Draw_Front.Clear();
            Main.instance.LoadProjectile(ModContent.ProjectileType<OmegaStariteProj>());
            var viewPos = NPC.IsABestiaryIconDummy ? NPC.Center : new Vector2(screenPos.X + Main.screenWidth / 2f, screenPos.Y + Main.screenHeight / 2f);
            foreach (var orb in _orbs)
            {
                DrawOrb(
                    orb,
                    spriteBatch,
                    (ViewHelper.GetViewPoint(new Vector2(orb.position.X, orb.position.Y), orb.position.Z * 0.00728f, viewPos) - screenPos).Floor(),
                    ViewHelper.GetViewScale(orb.parentRing.Scale, orb.position.Z * 0.0314f),
                    viewPos
                );
            }

            drawColor = NPC.GetNPCColorTintedByBuffs(Color.White);

            var drawPos = (NPC.Center - screenPos).Floor();
            if (NPC.IsABestiaryIconDummy)
            {
                drawPos.Y += 2f;
            }

            spriteBatch.Draw(
                Textures.Bloom[3].Value,
                drawPos,
                null,
                Color.Blue * 0.7f,
                0f, 
                Textures.Bloom[3].Value.Size() / 2f,
                NPC.scale, SpriteEffects.None, 0f
            );
            foreach (var backBloom in Draw_BackBlooms)
            {
                backBloom.Draw(spriteBatch);
            }
            foreach (var back in Draw_Back)
            {
                back.Draw(spriteBatch);
            }

            var bodyDrawPosition = drawPos 
                + (hitShake > 0 ? new Vector2(Main.rand.Next(-hitShake, hitShake), Main.rand.Next(-hitShake, hitShake)) : Vector2.Zero);
            DrawBody(spriteBatch, bodyDrawPosition, drawColor);

            foreach (var front in Draw_Front)
            {
                front.Draw(spriteBatch);
            }

            if (hitShake > 0)
            {
                hitShake--;
            }

            //var positions = new List<Vector4>();
            //for (int i = 0; i < rings.Count; i++)
            //{
            //    for (int j = 0; j < rings[i].OrbCount; j++)
            //    {
            //        positions.Add(new Vector4((int)rings[i].CachedPositions[j].X, (int)rings[i].CachedPositions[j].Y, (int)rings[i].CachedPositions[j].Z, rings[i].Scale));
            //    }
            //}
            //float intensity = 1f;

            //if (Action == -100)
            //{
            //    var focus = Main.item[(int)NPC.ai[1]].Center;
            //    intensity += NPC.localAI[3] / 60;
            //    Lighting.GlobalBrightness -= intensity * 0.2f;
            //    ScreenFlash.Flash.Set(Main.item[(int)NPC.ai[1]].Center, Math.Min(Math.Max(intensity - 1f, 0f) * 0.04f, 0.8f));
            //    ScreenShake.SetShake(intensity * 0.5f);
            //    ModContent.GetInstance<CameraFocus>().SetTarget("Omega Starite", focus, CameraPriority.VeryImportant, 0.5f, 60);
            //}
            //else if (Action == OmegaStarite.ACTION_DEAD)
            //{
            //    intensity += NPC.ai[1] / 20;
            //    if (NPC.CountNPCS(Type) == 1)
            //    {
            //        ModContent.GetInstance<CameraFocus>().SetTarget("Omega Starite", NPC.Center, CameraPriority.BossDefeat, 12f, 60);
            //    }
            //    float val = MathHelper.Clamp(3f - intensity, 0f, 1f);
            //    if (val < 0.1f)
            //    {
            //        Music = MusicID.Night;
            //    }
            //    for (int i = 0; i < Main.musicFade.Length; i++)
            //    {
            //        Main.musicFade[i] = Math.Min(Main.musicFade[i], val);
            //    }

            //    ScreenFlash.Flash.Set(NPC.Center, Math.Min(Math.Max(intensity - 1f, 0f) * 0.2f, 0.8f));
            //    ScreenShake.SetShake(intensity * 2.25f);

            //    int range = (int)intensity + 4;
            //    drawPos += new Vector2(Main.rand.Next(-range, range), Main.rand.Next(-range, range));
            //    for (int i = 0; i < positions.Count; i++)
            //    {
            //        positions[i] += new Vector4(Main.rand.Next(-range, range), Main.rand.Next(-range, range), Main.rand.Next(-range, range), 0f);
            //    }
            //}
            //else if (hit > 0)
            //{
            //    drawPos += new Vector2(Main.rand.Next(-hit, hit), Main.rand.Next(-hit, hit));
            //    hit--;
            //}
            //positions.Sort((o, o2) => -o.Z.CompareTo(o2.Z));

            //Main.instance.LoadProjectile(ModContent.ProjectileType<OmegaStariteProj>());
            //var omegiteTexture = TextureAssets.Projectile[ModContent.ProjectileType<OmegaStariteProj>()].Value;
            //var omegiteFrame = new Rectangle(0, 0, omegiteTexture.Width, omegiteTexture.Height);
            //var omegiteOrigin = omegiteFrame.Size() / 2f;

            //float xOff = (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1f);
            //var clr3 = new Color(50, 50, 50, 0) * intensity;
            //float deathSpotlightScale = 0f;
            //if (intensity > 3f)
            //    deathSpotlightScale = NPC.scale * (intensity - 2.1f) * ((float)Math.Sin(NPC.ai[1] * 0.1f) + 1f) / 2f;
            //var spotlight = Textures.Bloom[0].Value;
            //var spotlightOrig = spotlight.Size() / 2f;
            //var spotlightColor = new Color(100, 100, 255, 0);
            //var drawOmegite = new List<Aequus.LegacyDrawMethod>();
            //drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
            //if (ClientConfig.Instance.HighQuality)
            //{
            //    drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
            //    {
            //        spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * 1.33f, SpriteEffects.None, 0f);
            //    });
            //}
            //drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
            //{
            //    spriteBatch.Draw(omegiteTexture, position, omegiteFrame, drawColor, rotation, origin1, scale, SpriteEffects.None, 0f);
            //});
            //float secretIntensity = 1f;
            //if (Action == -100)
            //{
            //    DrawDeathLightRays(intensity, Main.item[(int)NPC.ai[1]].Center - screenPos, spotlight, spotlightColor, spotlightOrig, deathSpotlightScale, NPC.localAI[3] * 0.05f);
            //    float decMult = Math.Clamp(2f - intensity * 0.4f, 0f, 1f);
            //    secretIntensity *= decMult;
            //    byte a = drawColor.A;
            //    drawColor *= secretIntensity;
            //    drawColor.A = a;
            //    a = clr3.A;
            //    clr3 *= secretIntensity;
            //    clr3.A = a;
            //}
            //if (intensity * secretIntensity >= 1f)
            //{
            //    drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
            //    {
            //        for (int j = 0; j < intensity * layerDepth; j++)
            //        {
            //            spriteBatch.Draw(omegiteTexture, position + new Vector2(2f + xOff * 2f * j, 0f),
            //                omegiteFrame, clr3 * layerDepth, rotation, origin1, scale, SpriteEffects.None, 0f);
            //            spriteBatch.Draw(omegiteTexture, position + new Vector2(2f - xOff * 2f * j, 0f),
            //                omegiteFrame, clr3 * layerDepth, rotation, origin1, scale, SpriteEffects.None, 0f);
            //        }
            //    });
            //}
            //if (intensity * secretIntensity > 3f)
            //{
            //    float omegiteDeathDrawScale = deathSpotlightScale * secretIntensity * 0.5f;
            //    drawOmegite.Add(delegate (Texture2D texture1, Vector2 position, Rectangle? frame1, Color color, float scale, Vector2 origin1, float rotation, SpriteEffects effects, float layerDepth)
            //    {
            //        spriteBatch.Draw(spotlight, position, null, drawColor, rotation, spotlightOrig, scale * omegiteDeathDrawScale, SpriteEffects.None, 0f);
            //        spriteBatch.Draw(spotlight, position, null, spotlightColor, rotation, spotlightOrig, scale * omegiteDeathDrawScale * 2, SpriteEffects.None, 0f);
            //    });
            //}

            //for (int i = 0; i < positions.Count; i++)
            //{
            //    if (positions[i].Z > 0f)
            //    {
            //        var drawPosition = ViewHelper.GetViewPoint(new Vector2(positions[i].X, positions[i].Y), positions[i].Z * 0.00728f, viewPos) - screenPos;
            //        var drawScale = ViewHelper.GetViewScale(positions[i].W, positions[i].Z * 0.0314f);
            //        foreach (var draw in drawOmegite)
            //        {
            //            draw.Invoke(
            //                omegiteTexture,
            //                drawPosition,
            //                omegiteFrame,
            //                drawColor * secretIntensity,
            //                drawScale,
            //                omegiteOrigin,
            //                NPC.rotation,
            //                SpriteEffects.None,
            //                secretIntensity);
            //        }
            //        positions.RemoveAt(i);
            //        i--;
            //    }
            //}
            //var texture = TextureAssets.Npc[NPC.type].Value;
            //var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            //Vector2 origin = NPC.frame.Size() / 2f;
            //float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            //var clr = drawColor * 0.25f;
            ////for (int i = 0; i < intensity; i++)
            ////{
            ////    spriteBatch.Draw(spotlight, drawPos, null, spotlightColor, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + i, SpriteEffects.None, 0f);
            ////}
            //spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * secretIntensity, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + intensity, SpriteEffects.None, 0f);

            //spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * (1f - intensity) * secretIntensity, NPC.rotation, spotlightOrig, NPC.scale * 2.5f + (intensity + 1), SpriteEffects.None, 0f);

            //if (!NPC.IsABestiaryIconDummy)
            //{
            //    Main.spriteBatch.End();
            //    Main.spriteBatch.Begin_World(shader: true);
            //    if ((NPC.position - NPC.oldPos[1]).Length() > 0.01f)
            //    {
            //        if (prim == null)
            //        {
            //            float radius = OmegaStarite.DIAMETER / 2f;
            //            prim = new TrailRenderer(Textures.Trail[0].Value, TrailRenderer.DefaultPass, (p) => new Vector2(radius - p * radius), (p) => new Color(35, 85, 255, 0) * (1f - p), drawOffset: NPC.Size / 2f);
            //        }
            //        prim.Draw(NPC.oldPos);
            //    }
            //    else
            //    {
            //        NPC.oldPos[0] = new Vector2(0f, 0f);
            //    }
            //    Main.spriteBatch.End();
            //    Main.spriteBatch.Begin_World(shader: false); ;
            //}

            //spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            //for (int j = 0; j < intensity * secretIntensity; j++)
            //{
            //    spriteBatch.Draw(texture, drawPos + new Vector2(2f + xOff * 2f * j, 0f), NPC.frame, clr3 * secretIntensity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(texture, drawPos - new Vector2(2f + xOff * 2f * j, 0f), NPC.frame, clr3 * secretIntensity, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            //}
            //for (int i = 0; i < positions.Count; i++)
            //{
            //    var drawPosition = ViewHelper.GetViewPoint(new Vector2(positions[i].X, positions[i].Y), positions[i].Z * 0.00728f, viewPos) - screenPos;
            //    var drawScale = ViewHelper.GetViewScale(positions[i].W, positions[i].Z * 0.0314f);
            //    foreach (var draw in drawOmegite)
            //    {
            //        draw.Invoke(
            //            omegiteTexture,
            //            drawPosition,
            //            omegiteFrame,
            //            drawColor * secretIntensity,
            //            drawScale,
            //            omegiteOrigin,
            //            NPC.rotation,
            //            SpriteEffects.None,
            //            secretIntensity);
            //    }
            //}
            //if (Action != -100)
            //    DrawDeathLightRays(intensity, drawPos, spotlight, spotlightColor, spotlightOrig, deathSpotlightScale, NPC.ai[1]);
            return false;
        }

        public void DrawDeathLightRays(float intensity, Vector2 drawPos, Texture2D spotlight, Color spotlightColor, Vector2 spotlightOrig, float deathSpotlightScale, float deathTime)
        {
            if (intensity > 3f || Action == -100 && intensity > 2f)
            {
                float intensity2 = intensity - 2f;
                float raysScaler = intensity2;
                if (deathTime > OmegaStarite.DEATHTIME)
                {
                    float scale = (deathTime - OmegaStarite.DEATHTIME) * 0.2f;
                    scale *= scale;
                    raysScaler += scale;
                    Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, NPC.rotation, spotlightOrig, scale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, NPC.rotation, spotlightOrig, scale * 2.15f, SpriteEffects.None, 0f);
                }
                else
                {
                    Main.spriteBatch.Draw(spotlight, drawPos, null, new Color(120, 120, 120, 0) * intensity2, NPC.rotation, spotlightOrig, deathSpotlightScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(spotlight, drawPos, null, spotlightColor * intensity2, NPC.rotation, spotlightOrig, deathSpotlightScale * 2f, SpriteEffects.None, 0f);
                }

                var shineColor = new Color(200, 40, 150, 0) * raysScaler * NPC.Opacity;

                var lightRay = ModContent.Request<Texture2D>(Aequus.AssetsPath + "LightRay", AssetRequestMode.ImmediateLoad).Value;
                var lightRayOrigin = lightRay.Size() / 2f;

                var r = LegacyEffects.EffectRand;
                int seed = r.SetRand((int)NPC.localAI[0]);
                int i = 0;
                foreach (float f in Helper.Circular((int)(80 + r.Rand(4)), Main.GlobalTimeWrappedHourly * 0.12f + NPC.localAI[0]))
                {
                    var rayScale = new Vector2(Helper.Wave(r.Rand(MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly * r.Rand(1f, 5f) * 0.1f, 0.3f, 1f) * r.Rand(0.5f, 2.25f));
                    rayScale.X *= 0.02f;
                    rayScale.X *= (float)Math.Pow(raysScaler, Math.Min(rayScale.Y, 1f));
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * NPC.Opacity, f, lightRayOrigin, raysScaler * rayScale, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(lightRay, drawPos, null, shineColor * 0.5f * NPC.Opacity, f, lightRayOrigin, raysScaler * rayScale * 2f, SpriteEffects.None, 0f);
                    i++;
                }
                r.SetRand(seed);
                var bloom = Textures.Bloom[2].Value;
                var bloomOrigin = bloom.Size() / 2f;
                raysScaler *= 0.7f;
                Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * raysScaler * NPC.Opacity, 0f, bloomOrigin, raysScaler, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bloom, drawPos, null, shineColor * 0.5f * raysScaler * NPC.Opacity, 0f, bloomOrigin, raysScaler * 1.4f, SpriteEffects.None, 0f);

                Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
                var shine = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion].Value;
                var shineOrigin = shine.Size() / 2f;
                Main.EntitySpriteDraw(shine, drawPos, null, shineColor, 0f, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale) * raysScaler, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shine, drawPos, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(NPC.scale * 0.5f, NPC.scale * 2f) * raysScaler, SpriteEffects.None, 0);
            }
        }
    }
}