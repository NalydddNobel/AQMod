using Aequus.Projectiles.Pets;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Pets
{
    public class FamiliarBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<AequusPlayer>().familiarPet = true;
            bool petProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<FamiliarPet>()] <= 0;
            if (petProjectileNotSpawned && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetProjectileSource_Buff(buffIndex), player.position.X + player.width / 2f, player.position.Y + player.height / 2f, 0f, 0f, ModContent.ProjectileType<FamiliarPet>(), 0, 0f, player.whoAmI, 0f, 0f);
        }
    }
}