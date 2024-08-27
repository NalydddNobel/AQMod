using Aequus;
using Aequus.Common.Effects;
using Aequus.Common.Graphics.LayerRenderers;
using Aequus.Common.Graphics.RenderBatches;
using Aequus.Common.Items;
using Aequus.Common.Net;
using Aequus.Common.Particles;
using Aequus.Common.Rendering;
using Aequus.Content;
using Aequus.Content.Graphics.RenderBatches;
using Aequus.Items.Materials.Glimmer;
using Aequus.Items.Weapons.Magic.Nightfall;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Magic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Items.Weapons.Magic.Nightfall {
    [LegacyName("WowHat")]
    public class Nightfall : ModItem {
        public override void SetDefaults() {
            Item.SetWeaponValues(16, 1f);
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<NightfallProj>(), 22, 8f, true);
            Item.mana = 10;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = ItemDefaults.ValueGlimmer;
            Item.UseSound = SoundID.Item8;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Book)
                .AddIngredient<StariteMaterial>(12)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }

    public class NightfallFallEffectPacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.NightfallOnHit;

        public void Send(NPC npc) {
            var p = GetLegacyPacket();
            p.Write((byte)npc.whoAmI);
            p.Send();
        }

        public override void Receive(BinaryReader reader) {
            byte npc = reader.ReadByte();
            if (npc >= Main.maxNPCs)
                return;

            if (Main.netMode == NetmodeID.Server) {
                Send(Main.npc[npc]);
            }

            NightfallProj.ApplyOnHitEffect(Main.npc[npc], out int _);
        }
    }

    public class NightfallPushEffectPacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.NightfallPush;

        public void Send(int plr, Vector2 where, float size, int ignoreTarget = -1) {
            var p = GetLegacyPacket();
            p.Write((byte)(ignoreTarget == -1 ? 255 : ignoreTarget));
            p.Write((byte)plr);
            p.Write(where.X);
            p.Write(where.Y);
            p.Write(size);
            p.Send();
        }

        public override void Receive(BinaryReader reader) {
            int ignoreTarget = reader.ReadByte();
            int player = reader.ReadByte();
            if (ignoreTarget == 255)
                ignoreTarget = -1;
            Vector2 where = new(reader.ReadSingle(), reader.ReadSingle());
            float size = reader.ReadSingle();

            if (Main.netMode == NetmodeID.Server) {
                Send(player, where, size, ignoreTarget);
            }

            NightfallProj.ApplyPushEffect(player, where, size, ignoreTarget);
        }
    }

    public class NightfallEnemyRenderer : ScreenTarget, ILayerRenderer {
        public record struct EnemyRender(NPC npc, float opacity) {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Render() {
                if (npc.active) {
                    var bloomSize = AequusTextures.Bloom0.Size();
                    int smallestFrameSize = Math.Min(npc.frame.Width, npc.frame.Height);
                    Main.spriteBatch.Draw(
                        AequusTextures.Bloom0,
                        npc.Center - Main.screenPosition,
                        null,
                        Color.Blue * 0.03f * opacity,
                        0f,
                        bloomSize / 2f,
                        Math.Max(smallestFrameSize / bloomSize.X, 1f),
                        SpriteEffects.None, 0f);

                    int alpha = npc.alpha;
                    npc.Opacity = opacity;

                    bool gameMenu = Main.gameMenu;
                    try {
                        Main.gameMenu = true;
                        Main.instance.DrawNPC(npc.whoAmI, npc.behindTiles);
                    }
                    catch {
                    }
                    Main.gameMenu = gameMenu;

                    npc.alpha = alpha;
                }
            }
        }

        bool ILayerRenderer.IsReady => IsReady;

        public List<EnemyRender> npcs = new();

        public override void Load(Mod mod) {
            base.Load(mod);
        }

        public override void Unload() {
            npcs?.Clear();
            base.Unload();
        }

        public override void CleanUp() {
            npcs?.Clear();
        }

        protected override void DrawOntoTarget(GraphicsDevice device, SpriteBatch spriteBatch) {
            LegacyDrawList.ForceRender = true;

            spriteBatch.Begin_World(shader: false);
            foreach (var n in npcs) {
                n.Render();
            }
            spriteBatch.End();

            device.SetRenderTarget(_target);
            device.Clear(Color.Transparent);

            spriteBatch.Begin();

            var clr = Color.White with { A = 0, };

            spriteBatch.Draw(helperTarget, new Rectangle(0, 0, _target.Width, _target.Height), clr);
            if (Aequus.HQ) {
                for (int i = 0; i < 2; i++) {
                    spriteBatch.Draw(helperTarget, new Rectangle(2, 0, _target.Width, _target.Height), clr);
                    spriteBatch.Draw(helperTarget, new Rectangle(-2, 0, _target.Width, _target.Height), clr);
                    spriteBatch.Draw(helperTarget, new Rectangle(0, 2, _target.Width, _target.Height), clr);
                    spriteBatch.Draw(helperTarget, new Rectangle(0, -2, _target.Width, _target.Height), clr);
                }
            }

            spriteBatch.End();
            device.SetRenderTarget(helperTarget);

            _wasPrepared = true;
            LegacyDrawList.ForceRender = false;
        }

        protected override bool PrepareTarget() {
            return npcs.Count > 0;
        }

        public void SetupBatchLayers() {
            ModContent.GetInstance<BehindAllNPCsBatch>().Renderers.Add(this);
            ModContent.GetInstance<BehindAllNPCsNoWorldScaleBatch>().Renderers.Add(this);
        }

        public void RenderFlares(SpriteBatch spriteBatch) {
            float flareScale = Helper.Wave(Main.GlobalTimeWrappedHourly * 40f, 0.8f, 1f);
            var flareColor = new Color(130, 30, 200, 0);
            foreach (var n in npcs) {
                var npc = n.npc;

                spriteBatch.Draw(
                    AequusTextures.Flare,
                    npc.Center - Main.screenPosition,
                    null,
                    Color.White with { A = 0, } * n.opacity,
                    0f,
                    AequusTextures.Flare.Size() / 2f,
                    flareScale * 0.66f,
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(
                    AequusTextures.Flare,
                    npc.Center - Main.screenPosition,
                    null,
                    Color.White with { A = 0, } * n.opacity,
                    MathHelper.PiOver2,
                    AequusTextures.Flare.Size() / 2f,
                    new Vector2(1f, 3f) * 0.66f * flareScale,
                    SpriteEffects.None, 0f);


                spriteBatch.Draw(
                    AequusTextures.Flare,
                    npc.Center - Main.screenPosition,
                    null,
                    flareColor * n.opacity,
                    0f,
                    AequusTextures.Flare.Size() / 2f,
                    flareScale,
                    SpriteEffects.None, 0f);
                spriteBatch.Draw(
                    AequusTextures.Flare,
                    npc.Center - Main.screenPosition,
                    null,
                    flareColor * n.opacity,
                    MathHelper.PiOver2,
                    AequusTextures.Flare.Size() / 2f,
                    new Vector2(1f, 3f) * flareScale,
                    SpriteEffects.None, 0f);
            }
        }

        public void DrawToLayer(RenderLayerBatch layer, SpriteBatch spriteBatch) {
            if (_target == null || !_wasPrepared) {
                _wasPrepared = false;
                return;
            }

            if (layer is BehindAllNPCsBatch) {
                if (Aequus.HQ) {
                    RenderFlares(spriteBatch);
                }
                return;
            }

            spriteBatch.Draw(
                _target,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                Color.BlueViolet with { A = 0 });

            npcs.Clear();

            _wasPrepared = false;
        }
    }
}

namespace Aequus.Projectiles.Magic {
    public class NightfallProj : ModProjectile {
        public override void SetStaticDefaults() {
            LegacyPushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 30;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
        }

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft < 30) {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }
            else if (Projectile.alpha > 0) {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Projectile.ShimmerReflection();
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override void OnKill(int timeLeft) {
            var particleColor = Color.Lerp(Color.White, Color.BlueViolet, 0.66f).UseA(0);
            for (int i = 0; i < 20; i++) {
                var d = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width, Projectile.height,
                    ModContent.DustType<MonoDust>(),
                    -Projectile.velocity.X, Projectile.velocity.X - 4f,
                    newColor: particleColor * Main.rand.NextFloat(0.5f, 1f));
                d.velocity.X *= 0.5f;
                d.noGravity = true;
            }

            if (timeLeft < 3 || Main.myPlayer != Projectile.owner)
                return;

            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<NightfallPushEffectPacket>().Send(Projectile.owner, Projectile.Center, 100f);
            }
            ApplyPushEffect(Projectile.owner, Projectile.Center, 100f);
        }

        public static void ApplyPushVisual(NPC npc) {
            Vector2 bottom = new(npc.position.X - 12f, npc.position.Y + npc.height - 8f);
            for (int i = 0; i < 50; i++) {
                var d = Dust.NewDustDirect(
                    bottom,
                    npc.width + 24,
                    8,
                    ModContent.DustType<MonoDust>(),
                    npc.velocity.X, npc.velocity.Y * Main.rand.NextFloat(1f),
                    0, Color.HotPink with { A = 0, }, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                d.velocity.X *= 0.2f;
                d.noGravity = false;
            }
        }

        public static void ApplyPushEffect(int plr, Vector2 where, float size, int ignoreTarget = -1) {
            bool playSound = false;
            for (int i = 0; i < Main.maxNPCs; i++) {
                var npc = Main.npc[i];
                if (i != ignoreTarget &&
                    npc.active && npc.knockBackResist > 0f
                    && !npc.dontTakeDamage
                    && npc.Distance(where) <= size
                    && npc.CanBeChasedBy(Main.player[plr])) {
                    float kbResist = 1f - MathF.Pow(1f - npc.knockBackResist, 2f);
                    float diff = npc.Center.X - Main.player[plr].Center.X;
                    npc.velocity *= 0.5f;
                    npc.velocity.Y += 10f * -kbResist * (npc.noGravity ? 0.5f : 1f);
                    npc.velocity.X += 6f * kbResist * Math.Clamp(1f - Math.Abs(diff) / (size * 2f), 0f, 1f) * Math.Sign(diff);
                    npc.velocity.X *= 0.5f;
                    ApplyPushVisual(npc);
                    playSound = true;

                    int glint = ModContent.ProjectileType<NightfallGlint>();
                    for (int k = 0; k < Main.maxProjectiles; k++) {
                        if (Main.projectile[k].active
                            && Main.projectile[k].owner == Main.myPlayer
                            && Main.projectile[k].type == glint
                            && (int)Main.projectile[k].ai[0] == npc.whoAmI) {
                            Main.projectile[k].Kill();
                        }
                    }

                    if (plr == Main.myPlayer) {
                        Projectile.NewProjectile(
                            Main.player[plr].GetSource_FromThis(),
                            npc.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<NightfallGlint>(),
                            0, 0f, plr, npc.whoAmI
                        );
                    }
                }
            }

            if (playSound) {
                SoundEngine.PlaySound(AequusSounds.pushUp with { Volume = 0.7f, PitchVariance = 0.2f, }, where);
            }
        }

        public static void ApplyOnHitEffect(NPC target, out int fallDamage) {
            int j = 0;
            var oldPosition = target.position;
            var tileCoordinates = target.Bottom.ToTileCoordinates();
            int x = tileCoordinates.X;
            for (; j < 30; j++) {
                int y = tileCoordinates.Y + j;
                if (!WorldGen.InWorld(x, y, fluff: 40))
                    break;

                if (Main.tile[x, y].IsFullySolid())
                    break;
            }

            fallDamage = 0;
            if (j <= 1)
                return;

            float damage = (j - 3) * 15;
            if (damage > 25) {
                // all damage above 20 only adds 2.5
                damage = Helper.ScaleDown(damage, 25f, 0.5f);
            }
            if (damage > 80) {
                // all damage above 50 only adds 1.25
                damage = Helper.ScaleDown(damage, 80f, 0.5f);
            }

            fallDamage = (int)damage;

            if (fallDamage <= 0)
                return;

            int glint = ModContent.ProjectileType<NightfallGlint>();
            for (int k = 0; k < Main.maxProjectiles; k++) {
                if (Main.projectile[k].active
                    && Main.projectile[k].owner == Main.myPlayer
                    && Main.projectile[k].type == glint
                    && (int)Main.projectile[k].ai[0] == target.whoAmI) {
                    Main.projectile[k].Kill();
                }
            }

            SoundEngine.PlaySound(AequusSounds.pushDown with { Volume = 0.8f, PitchVariance = 0.1f, }, target.Center);

            target.velocity.Y -= 3f;
            target.position.Y = (tileCoordinates.Y + j) * 16f - target.height;

            for (int k = 0; k < j; k++) {
                ParticleSystem.New<DashBlurParticle>(ParticleLayer.AboveNPCs)
                    .Setup(
                    new(oldPosition.X + Main.rand.Next(target.width), oldPosition.Y + k * 16f + Main.rand.Next(target.height)),
                    Vector2.UnitY * Main.rand.NextFloat(4f, 8f),
                    Color.BlueViolet with { A = 60 },
                    2f,
                    0f);

                var d = Dust.NewDustDirect(
                    oldPosition with { Y = oldPosition.Y + k * 16f },
                    target.width,
                    target.height,
                    ModContent.DustType<MonoDust>(),
                    0f, 1f,
                    0, Color.HotPink with { A = 0, }, Scale: Main.rand.NextFloat(0.5f, 1.5f));
                d.velocity.X *= 0.1f;
                d.noGravity = false;
            }
            Collision.HitTiles(target.position with { Y = target.position.Y + target.height, }, Vector2.UnitY, target.width, 16);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.knockBackResist <= 0.02f || target.Hitbox.InSolidCollision())
                return;

            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<NightfallFallEffectPacket>().Send(target);
            }
            ApplyOnHitEffect(target, out int fallDamage);

            if (fallDamage > 0) {
                Projectile.timeLeft = 2;
                if (Main.netMode != NetmodeID.SinglePlayer) {
                    ModContent.GetInstance<NightfallPushEffectPacket>().Send(Projectile.owner, Projectile.Center, 50f, target.whoAmI);
                }
                ApplyPushEffect(Projectile.owner, Projectile.Center, 50f, target.whoAmI);

                target.StrikeNPC(target.CalculateHitInfo(fallDamage, 0));
                ScreenShake.SetShake(fallDamage / 3f, where: target.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            float rotation = Projectile.rotation + Main.GlobalTimeWrappedHourly * 1.7f;
            var drawCoords = Projectile.position + offset - Main.screenPosition;
            Main.spriteBatch.Draw(
                texture,
                drawCoords,
                frame,
                Color.Lerp(Color.White, Color.BlueViolet, 0.66f) * Projectile.Opacity,
                rotation,
                origin,
                Projectile.scale * 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(
                texture,
                drawCoords,
                frame,
                Color.BlueViolet with { A = 0 } * Projectile.Opacity,
                rotation,
                origin,
                Projectile.scale * 1.33f, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class NightfallGlint : ModProjectile {
        public NPC Host => Main.npc[(int)Projectile.ai[0]];

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.alpha = 255;
        }

        public override void AI() {
            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.5f, 1f));
            var host = Host;
            if (!host.active || host.velocity.Y == 0f) {
                Projectile.Kill();
                return;
            }
            if (Projectile.timeLeft <= 10) {
                if (Projectile.alpha < 255) {
                    Projectile.alpha += 30;
                    if (Projectile.alpha > 255)
                        Projectile.alpha = 255;
                }
            }
            else if (Projectile.alpha > 0) {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            else if (Projectile.localAI[0] < 1f) {
                Projectile.localAI[0] += 0.1f;
            }

            Projectile.Center = Host.Center;
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var origin = texture.Size() / 2f;
            var host = Host;
            var color = new Color(90, 40, 130, 0) * 0.5f * Projectile.Opacity;
            float horizontalOffset = -16f + host.frame.Width / 2f;

            ModContent.GetInstance<NightfallEnemyRenderer>().npcs.Add(new(host, Projectile.Opacity));
            Vector2 scale = new(Projectile.scale * 0.27f, Projectile.scale * 0.44f);

            Main.spriteBatch.Draw(
                texture,
                host.Center + new Vector2(-texture.Width / 3f - horizontalOffset, 0f) - Main.screenPosition,
                null,
                color,
                0f,
                origin,
                scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(
                texture,
                host.Center + new Vector2(texture.Width / 3f + horizontalOffset, 0f) - Main.screenPosition,
                null,
                color,
                0f,
                origin,
                scale, SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
    }
}