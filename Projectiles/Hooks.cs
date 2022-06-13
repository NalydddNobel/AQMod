namespace Aequus.Projectiles
{
    public class Hooks
    {
        public interface IOnUnmatchingProjectileParents
        {
            void OnUnmatchingProjectileParents(AequusProjectile sources, int identityFound);
        }
    }
}