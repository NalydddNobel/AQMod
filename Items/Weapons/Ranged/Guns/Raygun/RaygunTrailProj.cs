using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Guns.Raygun;

public class RaygunTrailProj : ModProjectile {
    public override string Texture => Aequus.ProjectileTexture(ProjectileID.PrincessWeapon);

    public Color color;
    public static Dictionary<int, Action<Projectile, Projectile>> OnSpawnEffects { get; private set; }

    public override void Load() {
        OnSpawnEffects = new Dictionary<int, Action<Projectile, Projectile>>() {
            [ProjectileID.CrystalShard] = (p, parent) => p.scale *= 0.4f,
        };
    }

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 24;
        color = Color.White;
        color.A = 0;
    }

    public override void OnSpawn(IEntitySource source) {
        if (Helper.HereditarySource(source, out var entity)) {
            if (entity is Projectile parentProjectile) {
                if (OnSpawnEffects.TryGetValue(parentProjectile.type, out var action)) {
                    action(Projectile, parentProjectile);
                }
            }
        }
    }

    public override void AI() {
        Projectile.scale += 0.0175f;
        Projectile.alpha += 15;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public override bool PreDraw(ref Color lightColor) {
        float opacity = 1f - Projectile.alpha / 255f;
        var texture = TextureAssets.Projectile[Type];
        var origin = texture.Size() / 2f;
        var scale = new Vector2(Projectile.scale * 0.11f, Projectile.scale * 0.245f);
        Main.spriteBatch.Draw(texture.Value, Projectile.Center - Main.screenPosition, null, color * opacity, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
        return false;
    }


    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(color.R);
        writer.Write(color.G);
        writer.Write(color.B);
        writer.Write(Projectile.scale);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        color.R = reader.ReadByte();
        color.G = reader.ReadByte();
        color.B = reader.ReadByte();
        color.A = 0;
        Projectile.scale = reader.ReadSingle();
    }
}