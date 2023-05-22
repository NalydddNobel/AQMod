using Aequus.Items.Accessories;
using Aequus.Items.Accessories.Combat.OnHit.Anchor;
using Aequus.Projectiles.Misc.Friendly;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.OnHit.Anchor {
    public interface IDavyJonesAnchor : IAccessoryData {
        int AnchorSpawnChance { get; }
    }
    public class DavyJonesAnchor : ModItem, IDavyJonesAnchor {
        public virtual int AnchorSpawnChance => 10;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.ChanceFrac(AnchorSpawnChance));

        public IEntitySource GetEntitySource(Entity entity) {
            throw new System.NotImplementedException();
        }

        public override void SetDefaults() {
            Item.DefaultToAccessory();
            Item.rare = ItemDefaults.RarityCrabCrevice;
            Item.value = ItemDefaults.ValueCrabCrevice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().accDavyJonesAnchor.SetAccessory(Item, this);
        }
    }
}

namespace Aequus {
    partial class AequusPlayer {
        public Accessory<IDavyJonesAnchor> accDavyJonesAnchor = new();

        private void ProcAnchor(NPC target, NPC.HitInfo hit) {
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
}