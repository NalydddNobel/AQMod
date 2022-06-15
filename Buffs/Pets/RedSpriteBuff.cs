using Aequus.Projectiles.Misc.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="Items.Misc.Pets.ToySpaceGun"/>
    /// </summary>
    public class RedSpriteBuff : PetBuffBase
    {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<RedSpritePet>();
    }
}
