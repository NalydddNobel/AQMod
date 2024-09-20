using Aequus;
using Aequus.Common.Structures;
using Aequus.Particles.Common;
using System;

namespace Aequus.Content.Items.Accessories.Breath;

[AutoloadEquip(EquipType.Back)]
[Gen.AequusPlayer_ResetField<Item>("accBreathRestore")]
[Gen.AequusPlayer_ResetField<int>("accBreathRestoreStacks")]
public class BreathConserver : ModItem {
    public static readonly float RestoreBreathMaxOnTileBreak = 1f / 15f;
    public static readonly float RestoreBreathMaxOnEnemyKill = 1f / 3f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.CloneShopValues(ItemID.Flipper);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accBreathRestore = Item;
        player.GetModPlayer<AequusPlayer>().accBreathRestoreStacks++;
    }

    [Gen.AequusPlayer_OnBreakTile]
    internal static void RestoreBreathOnBrokenTile(Player player, AequusPlayer aequus, int X, int Y) {
        if (aequus.accBreathRestoreStacks <= 0 || player.breath >= player.breathMax) {
            return;
        }

        int restoreBreath = Math.Max((int)(player.breathMax * RestoreBreathMaxOnTileBreak), 1) * aequus.accBreathRestoreStacks;
        aequus.HealBreath(restoreBreath);

        if (Main.netMode != NetmodeID.Server) {
            Vector2 tileCenter = new Vector2(X, Y).ToWorldCoordinates();
            foreach (var particle in Instance<UnderwaterBubbles>().NewMultipleReduced(12, 3)) {
                particle.Location = tileCenter;
                particle.Frame = (byte)Main.rand.Next(2);
                particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.3f);
                particle.Velocity.Y = -Math.Abs(particle.Velocity.Y);
                particle.UpLift = (1f - particle.Velocity.X) * 0.002f;
                particle.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }

            var bigBubble = Instance<UnderwaterBubbles>().New();
            bigBubble.Location = tileCenter;
            bigBubble.Frame = (byte)Main.rand.Next(3, 6);
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.005f;
        }
    }

    [Gen.AequusPlayer_OnKillNPC]
    internal static void RestoreBreathOnKillNPC(Player player, AequusPlayer aequus, EnemyKillInfo info) {
        if (aequus.accBreathRestoreStacks <= 0) {
            return;
        }

        Player? targetPlayer = null;
        int lowestBreath = int.MaxValue;
        foreach (Player nearby in player.FindNearbyTeammates(1000f)) {
            if (nearby.breath < lowestBreath && nearby.breath < nearby.breathMax) {
                lowestBreath = nearby.breath;
                targetPlayer = nearby;
            }
        }

        if (targetPlayer == null) {
            return;
        }

        if (Main.myPlayer == player.whoAmI) {
            Projectile.NewProjectile(player.GetSource_Accessory(aequus.accBreathRestore), info.Center, Vector2.UnitY, ModContent.ProjectileType<BreathConserverProj>(), 0, 0f, Owner: player.whoAmI, ai0: targetPlayer.whoAmI);
        }

        if (Main.netMode != NetmodeID.Server) {
            NPC npc = ContentSamples.NpcsByNetId[info.netID];
            int bubbleCount = Math.Clamp((int)(npc.Size.Length() / 4f), 12, 80);
            Vector2 bubbleSpawnSize = npc.Size / 4f;
            foreach (var particle in Instance<UnderwaterBubbles>().NewMultipleReduced(bubbleCount, bubbleCount / 4)) {
                particle.Location = info.Center + Main.rand.NextVector2Unit() * bubbleSpawnSize;
                particle.Frame = (byte)Main.rand.Next(2);
                particle.Velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.3f);
                particle.Velocity.Y = -Math.Abs(particle.Velocity.Y);
                particle.UpLift = (1f - particle.Velocity.X) * 0.002f;
                particle.Opacity = Main.rand.NextFloat(0.8f, 1f);
            }

            var bigBubble = Instance<UnderwaterBubbles>().New();
            bigBubble.Location = info.Center;
            bigBubble.Frame = (byte)Main.rand.Next(3, 6);
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.005f;
        }
    }
}
