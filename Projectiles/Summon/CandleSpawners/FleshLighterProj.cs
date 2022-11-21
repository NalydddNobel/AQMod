﻿using Aequus.Content.Necromancy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Summon.CandleSpawners
{
    public class FleshLighterProj : BaseGhostSpawner
    {
        public override int NPCType()
        {
            return NPCID.BloodCrawler;
        }

        public override void AI()
        {
            Projectile.rotation += 0.045f + (1f - Projectile.Opacity) * 0.05f;
            base.AI();
        }

        protected override void OnSpawnZombie(NPC npc, NecromancyNPC zombie)
        {
            base.OnSpawnZombie(npc, zombie);
        }

        protected override void SpawnEffects()
        {
            var r = Utils.CenteredRectangle(Projectile.Center, new Vector2(32f));
            for (int i = 0; i < 40; i++)
            {
                var d = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, DustID.Blood, Scale: Main.rand.NextFloat(0.6f, 1.5f));
                d.velocity *= 2f;
                d.velocity.Y -= 3f;
                d.noGravity = true;
                d.fadeIn = d.scale + 1f;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath52.WithPitchOffset(-0.5f).WithVolume(0.66f), Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var drawCoords = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(TextureCache.Bloom[3].Value, drawCoords, null, Color.Black * Projectile.Opacity,
                0f, TextureCache.Bloom[3].Value.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureCache.Bloom[0].Value, drawCoords, null, Color.Black * Projectile.Opacity,
                0f, TextureCache.Bloom[0].Value.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.WebbedAir].Value, Projectile.Center - Main.screenPosition, null, Color.Red.UseA(128) * Projectile.Opacity,
                Projectile.rotation, new Vector2(37f, 39f), Projectile.scale * Projectile.Opacity * 0.8f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.CultistRitual].Value, Projectile.Center - Main.screenPosition, null, Color.Red.UseA(0) * Projectile.Opacity,
                -Projectile.rotation, TextureAssets.Extra[ExtrasID.CultistRitual].Value.Size() / 2f, Projectile.scale * Projectile.Opacity * 0.33f, SpriteEffects.None, 0);
            return false;
        }
    }
}