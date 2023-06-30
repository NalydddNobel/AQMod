using Aequus.Buffs.Debuffs;
using Aequus.Common.Effects;
using Aequus.Common.Particles;
using Aequus.Common.Rendering;
using Aequus.Content;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic {
    public class Gamestar : ModItem {
        public override void SetDefaults() {
            Item.SetWeaponValues(32, 3.5f);
            Item.DamageType = DamageClass.Magic;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.width = 20;
            Item.height = 20;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemDefaults.RarityOmegaStarite;
            Item.shoot = ModContent.ProjectileType<GamestarProj>();
            Item.shootSpeed = 25f;
            Item.mana = 14;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item75.WithPitch(1f);
            Item.value = ItemDefaults.ValueOmegaStarite;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<Nightfall>()
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.Anvils)
                .TryRegisterAfter(ItemID.SpaceGun);
        }
    }

    public class GamestarProj : ModProjectile {
        public override string Texture => Aequus.BlankTexture;

        public override void SetStaticDefaults() {
            this.SetTrail(10);
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.alpha = 200;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI() {
            if ((int)Projectile.ai[0] > 0f) {
                int target = (int)Projectile.ai[0] - 1;
                if ((int)Projectile.ai[1] <= 0) {
                    Projectile.ai[1] = Main.rand.Next(1, 5) * 10;
                    if (Main.myPlayer == Projectile.owner) {
                        Projectile.velocity = Vector2.Normalize(Main.npc[target].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.ai[1]--;
            }
            Projectile.velocity *= 0.9975f;
            if (Main.netMode == NetmodeID.Server) {
                return;
            }
            ScreenCulling.Prepare(20);
            if (!ScreenCulling.OnScreenWorld(Utils.CenteredRectangle(Projectile.Center, new Vector2(100f)))) {
                return;
            }
            GamestarRenderer.Particles.Add(ParticleSystem.Fetch<GamestarParticle>().Setup(Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f), Color.White, 8));

            for (int i = 0; i < Projectile.oldPos.Length; i++) {
                if (Projectile.oldPos[i] == Vector2.Zero || Main.rand.NextFloat(1f) < 0.33f)
                    continue;

                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.AncientLight, 
                    Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 40, Scale: Main.rand.NextFloat(0.8f, 1.5f));
                d.noGravity = true;
                var particle = ParticleSystem.Fetch<GamestarParticle>().Setup(
                    Projectile.oldPos[i] + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height)), 
                    Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f), 
                    Color.White,
                    Main.rand.Next(8, 14));
                GamestarRenderer.Particles.Add(particle);
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.penetrate--;
            if (Projectile.penetrate == 0) {
                return true;
            }
            if (Projectile.velocity.X != oldVelocity.X) {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public void SpawnParticles(Entity target) {
            ScreenCulling.Prepare(200);
            if (!ScreenCulling.OnScreenWorld(Utils.CenteredRectangle(Projectile.Center, new Vector2(100f)))) {
                return;
            }

            int amt = Math.Max((target.width + target.height) / 30, 5);
            for (int i = 0; i < amt; i++) {
                GamestarRenderer.Particles.Add(ParticleSystem.Fetch<GamestarParticle>().Setup(target.Center + new Vector2(Main.rand.NextFloat(-target.width, target.width), Main.rand.NextFloat(-target.height, target.height)),
                    Main.rand.NextVector2Unit(), Color.White, 20));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<BitCrushedDebuff>(), 240);
            if (Projectile.ai[0] + 1f <= target.whoAmI) {
                Projectile.ai[0] = target.whoAmI + 1;
                Projectile.timeLeft += 30;
            }
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Main.netMode != NetmodeID.Server) {
                SpawnParticles(target);
            }
            for (int i = (int)Projectile.ai[0]; i < Main.maxNPCs; i++) {
                if (Main.npc[i].CanBeChasedBy(Projectile)) {
                    Projectile.velocity = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                    return;
                }
            }
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].CanBeChasedBy(Projectile)) {
                    Projectile.velocity = Vector2.Normalize(Main.npc[i].Center - Projectile.Center).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Projectile.velocity.Length();
                    Projectile.ai[0] = 0f;
                    break;
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) {
            if (Main.netMode != NetmodeID.Server) {
                SpawnParticles(target);
            }
        }
    }

    public sealed class GamestarParticle : BaseParticle<GamestarParticle> {
        protected override void SetDefaults() {
            SetFramedTexture(AequusTextures.GamestarParticle, 1);
        }

        public override void Update(ref ParticleRendererSettings settings) {
            if (Color.A == 0) {
                Scale = 0f;
            }
            float s = Scale;
            base.Update(ref settings);
            Scale = (int)s;
            while (Scale > 0f) {
                if (Main.rand.NextBool(3)) {
                    Scale--;
                    continue;
                }
                break;
            }
            if (Scale > 4f && Main.rand.NextBool(35 + Main.rand.Next(15 + (int)Scale * 5))) {
                var particle = ParticleSystem.Fetch<GamestarParticle>();
                particle.Setup(new Vector2(Position.X + Main.rand.NextFloat(-2f, 2f) * Scale, Position.Y + Main.rand.NextFloat(-2f, 2f) * Scale),
                    Main.rand.NextVector2Unit() * Velocity.Length(), Color, Scale - Main.rand.Next((int)Scale - 4));
                GamestarRenderer.Particles.Add(particle);
            }
            Rotation = 0f;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch) {
            var drawCoords = new Vector2((int)(Position.X / 2f) * 2f, (int)(Position.Y / 2f) * 2f) - Main.screenPosition;
            spritebatch.Draw(texture, drawCoords, frame, GetParticleColor(ref settings), Rotation, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}

namespace Aequus.Buffs.Debuffs {
    public class BitCrushedDebuff : ModBuff {
        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;

            AequusBuff.AddStandardMovementDebuffImmunities(Type, bossImmune: false);
            AequusBuff.PlayerStatusBuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().lagDebuff = 7;
        }
    }
}

namespace Aequus.NPCs {
    partial class AequusNPC {
        public byte lagDebuff;

        private void UpdateNPC_Gamestar(NPC npc, int i) {
            if (Main.GameUpdateCount % lagDebuff == 0) {
                for (int k = 0; k < lagDebuff - 1; k++) {
                    Helper.iterations = k + 1;
                    npc.UpdateNPC(i);
                }
                Helper.iterations = 0;
            }
            else {
                UpdateNPC_Frozen(npc, lowerBuffTime: false);
            }
        }

        private void PreDraw_Gamestar(NPC npc, Vector2 screenPos) {
            if (lagDebuff <= 0) {
                return;
            }
            var r = LegacyEffects.EffectRand;
            r.SetRand((int)(Main.GlobalTimeWrappedHourly * 32f) / 10 + npc.whoAmI * 10);
            int amt = Math.Max((npc.width + npc.height) / 20, 1);
            for (int k = 0; k < amt; k++) {
                DrawData dd = new(AequusTextures.GamestarParticle, npc.Center - screenPos, null, Color.White, 0f, AequusTextures.GamestarParticle.GetCenteredFrameOrigin(), Vector2.Zero, SpriteEffects.None, 0);
                if (k != 0) {
                    dd.position.X += (int)r.Rand(-npc.width, npc.width);
                    dd.position.Y += (int)r.Rand(-npc.height, npc.height);
                    dd.scale = new Vector2((int)r.Rand(amt * 2, amt * 5));
                }
                else {
                    dd.scale = new Vector2(npc.frame.Width, npc.frame.Height);
                }
                GamestarRenderer.DrawData.Add(dd);
            }
        }
    }
}