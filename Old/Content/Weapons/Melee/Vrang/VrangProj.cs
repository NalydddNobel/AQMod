using Aequus.DataSets;
using ReLogic.Content;
using System;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Aequus.Old.Content.Weapons.Melee.Vrang;

public class VrangProj : ModProjectile {
    public static Asset<Texture2D> HotTexture { get; private set; }
    public static Asset<Texture2D> ColdTexture { get; private set; }

    public sbyte temperature;

    public override void Load() {
        if (!Main.dedServ) {
            HotTexture = ModContent.Request<Texture2D>($"{Texture}_Hot");
            ColdTexture = ModContent.Request<Texture2D>($"{Texture}_Cold");
        }
    }

    public override void SetStaticDefaults() {
        ProjectileDataSet.PushableByTypeId.Add(Type);
    }

    public override void Unload() {
        HotTexture = null;
        ColdTexture = null;
    }

    public override void SetDefaults() {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.scale = 0.9f;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.alpha = 250;
        Projectile.timeLeft = 100;
        Projectile.penetrate = 3;
        Projectile.tileCollide = false;
    }

    public override void OnSpawn(IEntitySource source) {
        if (Projectile.ai[0] == 0f) {
            Projectile.ai[0] = 1f;

            OnSpawn_DetermineTemperatureValues(Main.player[Projectile.owner], out sbyte leftTemperature, out sbyte rightTemperature);
            //if (Main.player[Projectile.owner].Aequus().itemCombo > 0) {
            //    Utils.Swap(ref leftTemperature, ref rightTemperature);
            //}
            float rotationAmt = MathHelper.PiOver4 * 3f;
            float offset = 35f * Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee);
            OnSpawn_NewProj(source, Projectile.Center + Vector2.Normalize(Projectile.velocity).RotatedBy(rotationAmt) * offset, Projectile.velocity, leftTemperature, 1);
            OnSpawn_NewProj(source, Projectile.Center + Vector2.Normalize(Projectile.velocity).RotatedBy(-rotationAmt) * offset, Projectile.velocity, rightTemperature, -1);
        }
        Projectile.netUpdate = true;
    }
    public void OnSpawn_DetermineTemperatureValues(Player player, out sbyte temperature, out sbyte temperature2) {
        this.temperature = 0;
        temperature = 0;
        temperature2 = 0;
        bool isCold = OnSpawn_DetermineTempratureValues_IsCold(player);
        if (isCold) {
            this.temperature = -1;
            temperature = -1;
            temperature2 = -1;
        }
        // Cursed Inferno Flask (2), Flame Flask (3)
        if (OnSpawn_DetermineTempratureValues_IsHot(player)) {
            if (isCold) {
                this.temperature = 0;
                temperature = 0;
                temperature2 = 0;
            }
            else {
                this.temperature += 1;
                temperature = 1;
                temperature2 = 1;
            }
        }
        else if (this.temperature == 0) {
            temperature = 1;
            temperature2 = -1;
        }
    }
    public bool OnSpawn_DetermineTempratureValues_IsCold(Player player) {
        return player.frostBurn;
    }
    public bool OnSpawn_DetermineTempratureValues_IsHot(Player player) {
        return player.meleeEnchant == 2 || player.meleeEnchant == 3 || player.magmaStone;
    }
    public void OnSpawn_NewProj(IEntitySource source, Vector2 location, Vector2 velocity, sbyte temperatureCalc, int direction) {
        var p = Projectile.NewProjectileDirect(source, location, velocity, ModContent.ProjectileType<VrangProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1f);
        p.localAI[0] = direction;
        (p.ModProjectile as VrangProj).temperature = temperatureCalc;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        return true;
    }

    public override void AI() {
        if (temperature < 0) {
            Projectile.coldDamage = true;
            //Projectile.Aequus().heatDamage = false;
        }
        else if (temperature > 0) {
            Projectile.coldDamage = false;
            //Projectile.Aequus().heatDamage = true;
        }
        else {
            Projectile.coldDamage = false;
            //Projectile.Aequus().heatDamage = false;
        }
        if ((int)Projectile.ai[0] == -1) {
            Projectile.alpha += 3;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.velocity.X *= 0.99f;

            if (Projectile.velocity.Y < -4f) {
                Projectile.velocity.Y *= 0.9f;
            }
            if (Projectile.velocity.Y < 16f) {
                Projectile.velocity.Y += 0.3f;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.02f;
            return;
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] > 11f / Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee)) {
            if (Projectile.velocity.Length() < 22f * Main.player[Projectile.owner].GetAttackSpeed(DamageClass.Melee))
                Projectile.velocity += Projectile.velocity * 0.08f;

            if ((int)Projectile.localAI[0] != 0) {
                var off = new Vector2(Projectile.width / 2f, 0f).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2 * Projectile.localAI[0]);
                if (temperature < 0) {
                    int d = Dust.NewDust(Projectile.Center + off, 2, 2, DustID.FrostHydra);
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 0.8f;
                }
                else if (temperature > 0) {
                    int d = Dust.NewDust(Projectile.Center + off, 2, 2, AI_DetermineFlameDust());
                    Main.dust[d].velocity *= 0.1f;
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale = 0.8f;
                }
            }

            Projectile.tileCollide = true;

            Projectile.localAI[1] += 0.005f;
            Projectile.rotation += 0.3f + Projectile.localAI[1];
        }
        else {
            if (Projectile.velocity.Length() > 1f) {
                Projectile.velocity *= 0.78f;
            }

            Projectile.rotation += 0.3f;
        }
        if (Projectile.alpha > 0) {
            Projectile.alpha -= 25;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
    }
    public int AI_DetermineFlameDust() {
        return Main.player[Projectile.owner].meleeEnchant == 2 ? DustID.CursedTorch : DustID.Torch;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Main.netMode != NetmodeID.Server) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        if (Projectile.alpha > 100) {
            return true;
        }

        bool hitEffect = false;
        if (oldVelocity.X != Projectile.velocity.X) {
            if (Math.Abs(oldVelocity.X) > 4f) {
                hitEffect = true;
            }

            Projectile.position.X += Projectile.velocity.X;
            Projectile.velocity.X = oldVelocity.X > oldVelocity.Y ? -oldVelocity.X : -oldVelocity.X * 0.3f;
        }
        if (oldVelocity.Y != Projectile.velocity.Y) {
            if (Math.Abs(oldVelocity.Y) > 4f) {
                hitEffect = true;
            }

            Projectile.position.Y += Projectile.velocity.Y;
            Projectile.velocity.Y = oldVelocity.Y > oldVelocity.X ? -oldVelocity.Y : -oldVelocity.Y * 0.3f;
        }

        if (hitEffect) {
            Projectile.ai[0] = -1f;
            Projectile.timeLeft = 120;
            Projectile.netUpdate = true;
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f));
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            return false;
        }

        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var center = Projectile.Center;
        var frame = new Rectangle(0, 0, texture.Width, texture.Height);
        var origin = frame.Size() / 2f;
        Color drawColor = lightColor * Projectile.Opacity;
        Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        if (temperature != 0) {
            var maskTexture = temperature > 0 ? HotTexture : ColdTexture;
            Main.spriteBatch.Draw(maskTexture.Value, center - Main.screenPosition, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        }
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(temperature);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        temperature = reader.ReadSByte();
    }
}