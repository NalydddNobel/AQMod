using Aequus.Items.Pets.Light;
using Aequus.Projectiles.Misc.Pets;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="LightningRod"/>
    /// </summary>
    public class RedSpriteBuff : PetBuffBase
    {
        protected override bool LightPet => true;
        protected override int PetProj => ModContent.ProjectileType<RedSpritePet>();
    }
}
