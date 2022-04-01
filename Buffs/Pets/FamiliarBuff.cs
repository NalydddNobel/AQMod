using Aequus.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    public class FamiliarBuff : PetBuffBase
    {
        protected override ref bool ActiveFlag(Player player) => ref player.GetModPlayer<AequusPlayer>().familiarPet;
        protected override int PetProj => ModContent.ProjectileType<FamiliarPet>();
    }
}