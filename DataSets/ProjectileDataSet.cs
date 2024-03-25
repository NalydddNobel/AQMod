using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.DataSets;

public class ProjectileDataSet : DataSet {
    /// <summary>Rotation offset of projectile sprites so it can be rotated correctly.</summary>
    public static Dictionary<IDEntry<ProjectileID>, float> SpriteRotation { get; private set; } = new();

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
}