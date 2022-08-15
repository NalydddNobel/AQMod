using Aequus.Items.Pets;
using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="FamiliarPickaxe"/>
    /// </summary>
    public class FamiliarBuff : PetBuffBase
    {
        protected override int PetProj => ModContent.ProjectileType<FamiliarPet>();
    }
}