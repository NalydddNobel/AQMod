using Aequus.Common.EntitySources;
using Aequus.Items.Accessories.Misc.Fishing.NeonGenesis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus; 

partial class AequusPlayer {
    public Item accNeonGenesis;
    public int neonGenesisTimer;

    private void PostUpdateEquips_NeonGenesis() {
        if (neonGenesisTimer > 0) {
            neonGenesisTimer--;
        }
    }

    public void UseNeonGenesis(Projectile projectile) {
        if (accNeonGenesis == null || neonGenesisTimer > 0) {
            return;
        }

        int target = projectile.FindTargetWithLineOfSight(500f);
        if (target != -1) {
            EntitySource_ItemUse_WithEntity source = new(projectile, Player, accNeonGenesis);
            Projectile.NewProjectile(
                source,
                projectile.Center,
                Vector2.Normalize(Main.npc[target].Center - projectile.Center) * 25f,
                ModContent.ProjectileType<NeonFishLaser>(),
                (int)(Main.player[projectile.owner].HeldItem.fishingPole * (Main.hardMode ? 1f : 1.5f) * accNeonGenesis.EquipmentStacks()),
                12f,
                projectile.owner
            );
            neonGenesisTimer = 120;
        }
    }
}