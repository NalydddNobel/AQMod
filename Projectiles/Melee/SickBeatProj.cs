using Aequus.Content;
using Aequus.Items.Weapons.Melee.Misc.SickBeat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee {
    public class SickBeatProj : ModProjectile, IAddRecipes
    {
        public override string Texture => Helper.GetPath<SickBeat>();
        public float musicVolume;
        public int musicChoice;
        public bool activateMusic;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            LegacyPushableEntities.AddProj(Type);
        }

        public void AddRecipes(Aequus mod)
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 2;
            Projectile.manualDirectionChange = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 1200;
            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public void DoMusic()
        {
            try
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (Main.audioSystem is LegacyAudioSystem audioSystem && Main.musicFade.Length > musicChoice && audioSystem.AudioTracks.Length > musicChoice)
                    {
                        if (activateMusic && audioSystem.AudioTracks[musicChoice] != null)
                        {
                            musicVolume += 0.022f;
                            Main.musicFade[musicChoice] = Math.Min(Math.Max(musicVolume, Main.musicFade[musicChoice]) * 1.22f, 1.22f);
                            if (!audioSystem.AudioTracks[musicChoice].IsPlaying)
                            {
                                SoundEngine.PlaySound(SoundID.Item166);
                                if (Main.musicVolume > 0f)
                                {
                                    audioSystem.AudioTracks[musicChoice].Reuse();
                                    audioSystem.AudioTracks[musicChoice].SetVariable("Volume", Main.musicFade[musicChoice] * Main.soundVolume);
                                    audioSystem.AudioTracks[musicChoice].Play();
                                }
                            }
                            if (musicVolume >= 2.5f)
                            {
                                musicVolume = 1f;
                                activateMusic = false;
                            }
                        }
                        else if (musicVolume > 0f)
                        {
                            musicVolume *= 0.98f;
                            musicVolume -= 0.02f;
                            if (Main.curMusic != musicChoice)
                            {
                                Main.musicFade[musicChoice] = Math.Min(Main.musicFade[musicChoice], musicVolume);
                            }
                            if (musicVolume < 0f)
                                musicVolume = 0f;
                        }
                        if (audioSystem.AudioTracks[musicChoice].IsPlaying)
                        {
                            audioSystem.AudioTracks[musicChoice].SetVariable("Volume", Main.musicFade[musicChoice] * Main.soundVolume);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public override void AI()
        {
            if (activateMusic)
            {
                //DoMusic();
            }
            if ((int)Projectile.ai[0] == 0)
            {
                Projectile.direction = Projectile.velocity.X < 0f ? -1 : 1;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 60f)
                Projectile.tileCollide = false;
            if (Projectile.ai[0] > 40f)
            {
                float speed = Math.Max((Main.player[Projectile.owner].Center - Projectile.Center).Length() / 20f, 2f) + Projectile.ai[0] * 0.03f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * speed, Math.Max(1f - (Main.player[Projectile.owner].Center - Projectile.Center).Length() / 100f, 0.08f));
                if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() < 20f)
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation += 0.125f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            activateMusic = musicVolume <= 0f;
            Projectile.ai[0] += 5f;
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Collision.HitTiles(Projectile.position + new Vector2(Projectile.width / 4f, Projectile.height / 4f), oldVelocity, Projectile.width / 2, Projectile.height / 2);
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 250);
        }

        public void MusicExplosion(Entity target)
        {
            for (int i = 0; i < 10; i++)
            {
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_OnHit(target), target.Center + new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, 100f)), Vector2.Zero, ProjectileID.QuarterNote + Main.rand.Next(3), Projectile.damage, 0f, Projectile.owner);
                p.timeLeft = Math.Min(p.timeLeft, 20);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //MusicExplosion(target);
            SoundEngine.PlaySound(SoundID.Item166);
            activateMusic = true;
            Projectile.damage = (int)(Projectile.damage * 0.9f);
            Projectile.ai[0] -= 10f;
            if (Projectile.ai[0] < 0f)
                Projectile.ai[0] = 0f;
            Projectile.velocity = Projectile.DirectionFrom(Main.player[Projectile.owner].Center) * Projectile.velocity.Length();
            if (Projectile.penetrate == 1)
            {
                Projectile.penetrate = -1;
                if (Projectile.ai[0] < 60f)
                {
                }
                Projectile.ai[0] = 60f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var offset = new Vector2(Projectile.width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color(150, 150, 150, 40);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, color * (1f - progress), Projectile.rotation, origin, Projectile.scale, effects, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}