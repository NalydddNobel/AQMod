using Aequus.Core.DataSets;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.DataSets;

public class ProjectileSets : DataSet {
    public static Dictionary<ProjectileEntry, float> SpriteRotation { get; private set; } = new();

    public static HashSet<ProjectileEntry> IsStar { get; private set; } = new();

    /// <summary>
    /// Projectiles in this set deal 'heat' damage. This damage can be resisted using the Frost Potion.
    /// </summary>
    [JsonProperty]
    public static HashSet<ProjectileEntry> DealsHeatDamage { get; private set; } = new();

    [JsonProperty]
    public static HashSet<ProjectileEntry> PushableByTypeId { get; private set; } = new();

    [JsonProperty]
    public static HashSet<ProjectileAIEntry> PushableByAI { get; private set; } = new();

    public override void PostSetupContent() {
        for (int i = 0; i < ProjectileLoader.ProjectileCount; i++) {
            if (ProjectileID.Search.TryGetName(i, out string name)) {
                if (name.Contains('/')) {
                    name = name.Split('/')[^1];
                }
                if (name.Contains("Star") && !name.Contains("Start")) {
                    IsStar.Add((ProjectileEntry)i);
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
}