using Aequu2.Core;
using Aequu2.Content.Dusts;
using Aequu2.DataSets;
using Aequu2.Old.Content.Items.Materials;
using Aequu2.Old.Content.Particles;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;
using tModLoaderExtended.Networking;

namespace Aequu2.Old.Content.Items.Weapons.Magic.Nightfall;

[LegacyName("WowHat")]
public class Nightfall : ModItem {
    public override void SetDefaults() {
        Item.SetWeaponValues(16, 1f);
        Item.DefaultToMagicWeapon(ModContent.ProjectileType<NightfallProj>(), 22, 8f, true);
        Item.mana = 10;
        Item.width = 24;
        Item.height = 24;
        Item.rare = Commons.Rare.EventGlimmer;
        Item.value = Commons.Cost.EventGlimmer;
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

public class NightfallProj : ModProjectile {
    public override void SetStaticDefaults() {
        ProjectileDataSet.IsStar.Add(Type);
        ProjectileDataSet.PushableByTypeId.Add(Type);
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
            if (Projectile.alpha > 255) {
                Projectile.alpha = 255;
            }
        }
        else if (Projectile.alpha > 0) {
            Projectile.alpha -= 20;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
        Projectile.UpdateShimmerReflection();
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 4;
        height = 4;
        fallThrough = true;
        return true;
    }

    public override void OnKill(int timeLeft) {
        var particleColor = Color.Lerp(Color.White, Color.BlueViolet, 0.66f) with { A = 0 };
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
                float kbResist = 1f - MathF.Pow(1f - npc.knockBackResist, 8f);
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
            SoundEngine.PlaySound(Aequu2Sounds.NightfallPushUp with { Volume = 0.7f, PitchVariance = 0.2f, }, where);
        }
    }

    public static void ApplyOnHitEffect(NPC target, out int fallDamage) {
        int fallHeight = 0;
        var oldPosition = target.position;
        var tileCoordinates = target.Bottom.ToTileCoordinates();
        int x = tileCoordinates.X;
        for (; fallHeight < 30; fallHeight++) {
            int y = tileCoordinates.Y + fallHeight;
            if (!WorldGen.InWorld(x, y, fluff: 40)) {
                break;
            }

            if (Main.tile[x, y].IsFullySolid()) {
                break;
            }
        }

        fallDamage = 0;
        if (fallHeight <= 1) {
            return;
        }

        float damage = (fallHeight - 3) * 15;
        if (damage > 25) {
            damage = Helper.MultiplyAboveMin(damage, 25f, 0.5f);
        }
        if (damage > 80) {
            damage = Helper.MultiplyAboveMin(damage, 80f, 0.5f);
        }

        fallDamage = (int)damage;

        if (fallDamage <= 0) {
            return;
        }

        int glint = ModContent.ProjectileType<NightfallGlint>();
        for (int k = 0; k < Main.maxProjectiles; k++) {
            if (Main.projectile[k].active
                && Main.projectile[k].owner == Main.myPlayer
                && Main.projectile[k].type == glint
                && (int)Main.projectile[k].ai[0] == target.whoAmI) {
                Main.projectile[k].Kill();
            }
        }

        SoundEngine.PlaySound(Aequu2Sounds.NightfallPushDown with { Volume = 0.8f, PitchVariance = 0.1f, }, target.Center);

        target.velocity.Y -= 3f;
        target.position.Y = (tileCoordinates.Y + fallHeight) * 16f - target.height;

        if (Main.netMode != NetmodeID.Server) {
            for (int k = 0; k < fallHeight; k++) {
                var particle = DashBlurParticle.New();
                particle.Location = new Vector2(oldPosition.X + Main.rand.Next(target.width), oldPosition.Y + k * 16f + Main.rand.Next(target.height));
                particle.Velocity = Vector2.UnitY * Main.rand.NextFloat(4f, 8f);
                particle.Color = Color.BlueViolet with { A = 60 };
                particle.Scale = 1f;
                particle.Rotation = 0f;

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
        }
        Collision.HitTiles(target.position with { Y = target.position.Y + target.height, }, Vector2.UnitY, target.width, 16);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (target.knockBackResist <= 0.02f || target.Hitbox.SolidCollision()) {
            return;
        }

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
            ViewHelper.PunchCameraScalingFrom(Vector2.UnitY, target.Center, fallDamage / 3f, 4f, (int)(fallDamage * 0.33f));
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

        ModContent.GetInstance<NightfallEffectRenderer>().Enqueue(new(host, Projectile.Opacity));
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

public class NightfallFallEffectPacket : PacketHandler {
    public void Send(NPC npc) {
        var p = GetPacket();
        p.Write((byte)npc.whoAmI);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        byte npc = reader.ReadByte();
        if (!Main.npc.IndexInRange(npc)) {
            return;
        }

        if (Main.netMode == NetmodeID.Server) {
            Send(Main.npc[npc]);
        }

        NightfallProj.ApplyOnHitEffect(Main.npc[npc], out int _);
    }
}

public class NightfallPushEffectPacket : PacketHandler {
    public void Send(int plr, Vector2 where, float size, int ignoreTarget = -1) {
        var p = GetPacket();
        p.Write((byte)(ignoreTarget == -1 ? 255 : ignoreTarget));
        p.Write((byte)plr);
        p.Write(where.X);
        p.Write(where.Y);
        p.Write(size);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int ignoreTarget = reader.ReadByte();
        int player = reader.ReadByte();
        if (ignoreTarget == 255) {
            ignoreTarget = -1;
        }

        Vector2 where = new(reader.ReadSingle(), reader.ReadSingle());
        float size = reader.ReadSingle();

        if (Main.netMode == NetmodeID.Server) {
            Send(player, where, size, ignoreTarget);
        }

        NightfallProj.ApplyPushEffect(player, where, size, ignoreTarget);
    }
}
