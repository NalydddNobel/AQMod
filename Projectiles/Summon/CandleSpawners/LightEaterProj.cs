using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Projectiles.Summon.CandleSpawners
{
    public class LightEaterProj : BaseGhostSpawner
    {
        public override int NPCType()
        {
            return NPCID.EaterofSouls;
        }

        public override int AuraColor => ColorTargetID.DemonPurple;

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
                var d = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, DustID.GreenBlood, Scale: Main.rand.NextFloat(0.6f, 1.5f));
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
            Main.EntitySpriteDraw(AequusTextures.Bloom3, drawCoords, null, Color.Black * Projectile.Opacity,
                0f, AequusTextures.Bloom3.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(AequusTextures.Bloom0, drawCoords, null, Color.Black * Projectile.Opacity,
                0f, AequusTextures.Bloom0.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            QuickDrawAura(drawCoords, Color.BlueViolet, Color.Blue * 0.5f);
            Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.CultistRitual].Value, Projectile.Center - Main.screenPosition, null, Color.BlueViolet.UseA(0) * Projectile.Opacity,
                -Projectile.rotation, TextureAssets.Extra[ExtrasID.CultistRitual].Value.Size() / 2f, Projectile.scale * Projectile.Opacity * 0.33f, SpriteEffects.None, 0);
            return false;
        }
    }
}