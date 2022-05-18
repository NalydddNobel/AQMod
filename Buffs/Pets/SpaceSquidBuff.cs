using Aequus.Projectiles.Misc.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    /// <summary>
    /// Applied by <see cref="Items.Misc.Pets.ToySpaceGun"/>
    /// </summary>
    public class SpaceSquidBuff : PetBuffBase
    {
        protected override ref bool ActiveFlag(Player player) => ref player.GetModPlayer<AequusPlayer>().spaceSquidPet;
        protected override int PetProj => ModContent.ProjectileType<SpaceSquidPet>();
    }
}
