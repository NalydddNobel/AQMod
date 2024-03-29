﻿using Aequus.Projectiles;

namespace Aequus.Common.Projectiles {
    public class ProjectileHooks {
        public interface IOnUnmatchingProjectileParents {
            void OnUnmatchingProjectileParents(AequusProjectile sources, int identityFound);
        }
    }
}