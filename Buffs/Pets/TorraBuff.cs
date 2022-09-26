using Aequus.Items.Pets;
using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="SwagLookingEye"/>
    /// </summary>
    public class TorraBuff : PetBuffBase
    {
        protected override int PetProj => ModContent.ProjectileType<TorraPet>();
    }
}