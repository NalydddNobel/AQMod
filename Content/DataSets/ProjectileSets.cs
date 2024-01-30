using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.DataSets;

public class ProjectileSets : DataSet {
    public static Dictionary<ProjectileEntry, System.Single> SpriteRotation { get; private set; } = new();

    /// <summary>
    /// Projectiles in this set do not damage the <see cref="Old.Content.TownNPCs.OccultistNPC.Occultist"/>.
    /// </summary>
    [JsonProperty]
    public static HashSet<ProjectileEntry> OccultistIgnore { get; private set; } = new();
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
        for (System.Int32 i = 0; i < ProjectileLoader.ProjectileCount; i++) {
            if (ProjectileID.Search.TryGetName(i, out System.String name)) {
                if (name.Contains('/')) {
                    name = name.Split('/')[^1];
                }
                if (name.Contains("Star") && !name.Contains("Start")) {
                    IsStar.Add((ProjectileEntry)i);
                }
            }
        }
    }

    public static System.Single GetSpriteRotation(System.Int32 projectileId) {
        if (SpriteRotation.TryGetValue(projectileId, out var rotation)) {
            return rotation;
        }

        if (Main.dedServ) {
            return 0f;
        }

        Main.instance.LoadProjectile(projectileId);
        var texture = TextureAssets.Projectile[projectileId].Value;
        System.Int32 height = texture.Height / Main.projFrames[projectileId];
        // Projectile sprites which are tall are assumed to be facing upwards.
        if (texture.Width < height) {
            return MathHelper.PiOver2;
        }
        return 0f;
    }
}