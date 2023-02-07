using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Aequus.Projectiles.Summon.Necro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.CandleSpawners
{
    public class FallenAngelProj : BaseGhostSpawner
    {
        public override int NPCType()
        {
            return NPCID.Pixie;
        }

        public override int AuraColor => ColorTargetID.FriendshipMagick;

        public override void AI()
        {
            Projectile.rotation += 0.045f + (1f - Projectile.Opacity) * 0.045f;
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
                var d = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, DustID.AncientLight, Scale: Main.rand.NextFloat(0.6f, 1.5f));
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
            var ray = ModContent.Request<Texture2D>($"{Aequus.AssetsPath}LightRay").Value;
            Main.instance.LoadProjectile(ModContent.ProjectileType<ZombieBolt>());
            Main.EntitySpriteDraw(TextureAssets.Projectile[ModContent.ProjectileType<ZombieBolt>()].Value, drawCoords, null, Color.White * Projectile.Opacity,
                0f, TextureAssets.Projectile[ModContent.ProjectileType<ZombieBolt>()].Value.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(Textures.Bloom[2].Value, drawCoords, null, Color.White * 0.5f * Projectile.Opacity,
                0f, Textures.Bloom[2].Value.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(TextureAssets.Extra[ExtrasID.CultistRitual].Value, Projectile.Center - Main.screenPosition, null, Color.White.UseA(0) * Projectile.Opacity,
                -Projectile.rotation, TextureAssets.Extra[ExtrasID.CultistRitual].Value.Size() / 2f, Projectile.scale * Projectile.Opacity * 0.33f, SpriteEffects.None, 0);
            return false;
        }
    }
}