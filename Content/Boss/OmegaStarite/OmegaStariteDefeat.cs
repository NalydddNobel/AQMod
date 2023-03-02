using Aequus.Common.Effects;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.OmegaStarite
{
    public class OmegaStariteDefeat : OmegaStariteBase
    {
        public override string Texture => ModContent.GetInstance<OmegaStarite>().Texture;

        private bool _deathEffect;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 14;
            this.HideBestiaryEntry();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.boss = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Helper.HereditarySource(source, out var parentEntity) 
                && parentEntity is NPC parentNPC
                && parentNPC.ModNPC is OmegaStariteBase parentOmegaStarite)
            {
                rings = parentOmegaStarite.rings;
                NPC.Center = parentEntity.Center;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                ScreenShake.SetShake(40, multiplier: 0.95f, where: NPC.Center);
            }
            var center = NPC.Center;
            for (int k = 0; k < 60; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.02f)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue.UseA(25)).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.05f)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold.UseA(25)).noGravity = true;
            }
            ScreenCulling.SetPadding();
            if (ScreenCulling.OnScreenWorld(NPC.getRect()))
            {
                for (int k = 0; k < 7; k++)
                {
                    Gore.NewGore(new EntitySource_HitEffect(NPC), NPC.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            for (int i = 0; i < rings.Count; i++)
            {
                for (int j = 0; j < rings[i].OrbCount; j++)
                {
                    for (int k = 0; k < 30; k++)
                    {
                        Dust.NewDust(rings[i].CachedHitboxes[j].TopLeft(), rings[i].CachedHitboxes[j].Width, rings[i].CachedHitboxes[j].Height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
                    }
                    for (float f = 0f; f < 1f; f += 0.125f)
                    {
                        Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue.UseA(25)).noGravity = true;
                    }
                    for (float f = 0f; f < 1f; f += 0.25f)
                    {
                        Dust.NewDustPerfect(rings[i].CachedHitboxes[j].Center.ToVector2(), ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold.UseA(25)).noGravity = true;
                    }
                    if (ScreenCulling.OnScreenWorld(rings[i].CachedHitboxes[j]))
                    {
                        for (int k = 0; k < 7; k++)
                        {
                            Gore.NewGore(new EntitySource_HitEffect(NPC), NPC.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                        }
                    }
                }
            }
        }

        private void DeathEffect()
        {
            SoundEngine.PlaySound(OmegaStarite.Dead_0 with { Volume = 0.5f, }, NPC.Center);
            for (int k = 0; k < 60; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default(Color), 0.8f);
            }
            for (float f = 0f; f < 1f; f += 0.02f)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, Color.CornflowerBlue.UseA(25)).noGravity = true;
            }
            for (float f = 0f; f < 1f; f += 0.05f)
            {
                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<MonoSparkleDust>(), Vector2.UnitY.RotatedBy(f * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold.UseA(25)).noGravity = true;
            }
            ScreenCulling.SetPadding();
            if (ScreenCulling.OnScreenWorld(NPC.getRect()))
            {
                for (int k = 0; k < 7; k++)
                {
                    Gore.NewGore(new EntitySource_HitEffect(NPC), NPC.Center, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * 6f, Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
        }

        public override void AI()
        {
            if (!_deathEffect)
            {
                DeathEffect();
                _deathEffect = true;
            }
            NPC.velocity = Vector2.Zero;
            NPC.knockBackResist = 0f;
            NPC.boss = false;
            NPC.dontTakeDamage = true;
            if (Main.netMode != NetmodeID.Server)
            {
                PrepareOrbRenders();
                ModContent.GetInstance<CameraFocus>()
                    .SetTarget(
                        "Omega Starite", 
                        NPC.Center, 
                        CameraPriority.BossDefeat, 
                        12f, 
                        60
                    );
            }
        }

        public override void DrawOrb(OrbRenderData drawData, SpriteBatch spriteBatch, Vector2 drawPos, float drawScale, Vector2 viewPos)
        {
            drawPos += Main.rand.NextVector2Circular(4f, 4f);
            base.DrawOrb(drawData, spriteBatch, drawPos, drawScale, viewPos);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }
}