using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;

namespace Aequus.Common.DataSets {
    public class ProjectileSets : DataSet {
        public static readonly Dictionary<int, float> SpriteRotation = new();

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
}