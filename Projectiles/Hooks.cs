namespace Aequus.Projectiles
{
    public class Hooks
    {
        public interface IOnUnmatchingProjectileParents
        {
            void OnUnmatchingProjectileParents(ProjectileSources sources, int identityFound);
        }
    }
}