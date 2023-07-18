using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.MechPetCombo {
    public class MechPetComboBuff : ModBuff {
        private short[] PetsToSummon;

        public override void SetStaticDefaults() {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
            PetsToSummon = new[] { ProjectileID.DestroyerPet, ProjectileID.SkeletronPrimePet, ProjectileID.TwinsPet };
        }

        public override void Update(Player player, ref int buffIndex) {
            player.buffTime[buffIndex] = 18000;
            player.petFlagTwinsPet = true;
            player.petFlagDestroyerPet = true;
            player.petFlagSkeletronPrimePet = true;

            if (player.whoAmI == Main.myPlayer) {
                for (int i = 0; i < PetsToSummon.Length; i++) {
                    int petProj = PetsToSummon[i];

                    if (player.ownedProjectileCounts[petProj] <= 0) {
                        Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.position.X + player.width / 2f, player.position.Y + player.height / 2f, 0f, 0f, petProj, 0, 0f, player.whoAmI, 0f, 0f);
                    }
                }
            }
        }
    }
}