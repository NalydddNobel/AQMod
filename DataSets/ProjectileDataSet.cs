using Aequu2.DataSets.Structures;
using Newtonsoft.Json;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequu2.DataSets;

public class ProjectileDataSet : DataSet {
    /// <summary>Rotation offset of projectile sprites so it can be rotated correctly.</summary>
    public static Dictionary<IDEntry<ProjectileID>, float> SpriteRotation { get; private set; } = new();

    /// <summary>Color of the projectile. Used mostly by guns.</summary>
    public static Dictionary<IDEntry<ProjectileID>, Color> SpriteColor { get; private set; } = new Dictionary<IDEntry<ProjectileID>, Color>() {
        [ProjectileID.Bullet] = new Color(1, 255, 40, 255),
        [ProjectileID.MeteorShot] = new Color(30, 255, 200, 255),
        [ProjectileID.CrystalBullet] = new Color(200, 112, 145, 255),
        [ProjectileID.CrystalShard] = new Color(200, 112, 145, 255),
        [ProjectileID.CursedBullet] = new Color(120, 228, 50, 255),
        [ProjectileID.IchorBullet] = new Color(228, 200, 50, 255),
        [ProjectileID.ChlorophyteBullet] = new Color(135, 255, 120, 255),
        [ProjectileID.BulletHighVelocity] = new Color(255, 255, 235, 255),
        [ProjectileID.VenomBullet] = new Color(128, 30, 255, 255),
        [ProjectileID.NanoBullet] = new Color(60, 200, 255, 255),
        [ProjectileID.ExplosiveBullet] = new Color(255, 120, 60, 255),
        [ProjectileID.GoldenBullet] = new Color(255, 255, 10, 255),
        [ProjectileID.MoonlordBullet] = new Color(60, 215, 245, 255),
        [ProjectileID.PartyBullet] = Color.HotPink,
    };

    /// <summary>Projectiles in this set do not damage the <see cref="Old.Content.TownNPCs.OccultistNPC.Occultist"/>.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<ProjectileID>> OccultistIgnore { get; private set; } = new();

    /// <summary>Whether or not this projectile is a "Star", this is automatically populated with anything that has "Star" in their internal name.</summary>
    public static HashSet<IDEntry<ProjectileID>> IsStar { get; private set; } = new();

    /// <summary>Projectiles in this set deal 'heat' damage. This damage can be resisted using the Frost Potion.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<ProjectileID>> DealsHeatDamage { get; private set; } = new();

    [JsonProperty]
    public static HashSet<IDEntry<ProjectileID>> PushableByTypeId { get; private set; } = new();

    [JsonProperty]
    public static HashSet<IDEntry<ProjectileID>> PushableByAI { get; private set; } = new();

    public override void PostSetupContent() {
        for (int i = 0; i < ProjectileLoader.ProjectileCount; i++) {
            if (ProjectileID.Search.TryGetName(i, out string name)) {
                if (name.Contains('/')) {
                    name = name.Split('/')[^1];
                }
                if (name.Contains("Star") && !name.Contains("Start")) {
                    IsStar.Add(i);
                }
            }
        }
    }

    public static float GetSpriteRotation(int projectileId) {
        if (SpriteRotation.TryGetValue(projectileId, out var rotation)) {
            return rotation;
        }

        if (Main.dedServ) {
            return 0f;
        }

        Main.instance.LoadProjectile(projectileId);
        var texture = TextureAssets.Projectile[projectileId].Value;
        int height = texture.Height / Main.projFrames[projectileId];
        // Projectile sprites which are tall are assumed to be facing upwards.
        if (texture.Width < height) {
            return MathHelper.PiOver2;
        }
        return 0f;
    }

    public static Color GetColor(Projectile projectile) {
        if (SpriteColor.TryGetValue(projectile.type, out Color color)) {
            return color;
        }
        if (Main.netMode == NetmodeID.Server) {
            return Color.White;
        }
        Color clr = CheckRayColor(projectile);
        SpriteColor[projectile.type] = clr;
        return clr;
    }

    public static Color CheckRayColor(Projectile projectile) {
        Asset<Texture2D> asset = TextureAssets.Projectile[projectile.type];
        if (asset == null || !asset.IsLoaded) {
            return Color.White;
        }

        Texture2D texture = asset.Value;

        int r = 0;
        int g = 0;
        int b = 0;
        int count = 0;

        try {
            Color[] colors = new Color[texture.Width * texture.Height];
            asset.Value.GetData(colors);

            for (int i = 0; i < colors.Length; i++) {
                if (colors[i].A == 255) {
                    r += colors[i].R;
                    g += colors[i].G;
                    b += colors[i].B;
                    count++;
                }
            }
        }
        catch (Exception ex) {
            Log.Error(ex);
        }

        return count == 0 ? Color.White : new Color(r / count, g / count, b / count);
    }
}