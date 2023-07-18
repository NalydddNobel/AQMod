using Aequus.Common.ModPlayers;
using Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.Anchor;
using Terraria;
using Terraria.ModLoader;

namespace Aequus;

public partial class AequusPlayer {
    public Accessory<IDavyJonesAnchor> accDavyJonesAnchor = new();

    private void OnHit_Anchor(NPC target, NPC.HitInfo hit) {
        if (!accDavyJonesAnchor.TryGetItem(out var item, out var davyJonesAnchor) || Main.myPlayer != Player.whoAmI) {
            return;
        }
        int amt = accDavyJonesAnchor.EquipmentStacks();
        if (Player.RollLuck(davyJonesAnchor.AnchorSpawnChance) == 0) {
            float closestDistance = 400f;
            int targetNPC = -1;
            for (int i = 0; i < Main.maxNPCs; i++) {
                var npc = Main.npc[i];
                if (!npc.active || i == target.whoAmI || !npc.CanBeChasedBy(Player)) {
                    continue;
                }
                float distance = target.Distance(npc.Center);
                if (distance > closestDistance
                    || !Collision.CanHitLine(target.position, target.width, target.height, npc.position, npc.width, npc.height)) {
                    continue;
                }

                closestDistance = distance;
                targetNPC = i;
            }

            var velocity = targetNPC == -1 ? Main.rand.NextVector2Unit() : target.DirectionTo(Main.npc[targetNPC].Center);

            Projectile.NewProjectile(
                accDavyJonesAnchor.GetSource(Player),
                target.Center,
                velocity * 12f,
                ModContent.ProjectileType<DavyJonesAnchorProj>(),
                hit.SourceDamage * amt,
                2f,
                Player.whoAmI,
                ai0: target.whoAmI
            );
        }
    }
}