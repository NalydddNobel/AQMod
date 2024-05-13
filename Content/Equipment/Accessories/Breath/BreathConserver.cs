using Aequus.Common;
using Aequus.Common.Projectiles;
using Aequus.Content.Dusts;
using Aequus.Content.Graphics.Particles;
using Aequus.Core.CodeGeneration;
using System;

namespace Aequus.Content.Equipment.Accessories.Breath;

[AutoloadEquip(EquipType.Back)]
[ResetPlayerField("accBreathRestore", "Item")]
[ResetPlayerField("accBreathRestoreStacks", "int")]
public class BreathConserver : ModItem {
    public static float RestoreBreathMaxOnTileBreak { get; private set; } = 1f / 15f;
    public static float RestoreBreathMaxOnEnemyKill { get; private set; } = 1f / 3f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = Commons.Rare.PollutedOceanLoot;
        Item.value = Commons.Cost.PollutedOceanLoot;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accBreathRestore = Item;
        player.GetModPlayer<AequusPlayer>().accBreathRestoreStacks++;
    }
}

public class BreathConserverProj : ModProjectile {
    public override string Texture => AequusTextures.Projectile(ProjectileID.VampireHeal);

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.VampireHeal);
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
        Projectile.hide = true;
    }

    public override void AI() {
        Player owner = Main.player[Projectile.owner];
        AequusPlayer aequus = owner.GetModPlayer<AequusPlayer>();
        Player target = Main.player[(int)Projectile.ai[0]];

        if (aequus.accBreathRestoreStacks <= 0 || target.breath >= target.breathMax) {
            Projectile.Kill();
            return;
        }

        AIClones.AI_52_Heal(Projectile, target, HealBreath);

        if (Main.netMode != NetmodeID.Server) {
            Terraria.Dust d = Terraria.Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(30, 60, 88, 30), Scale: 2f);
            d.velocity *= 0.2f;
            if (Projectile.timeLeft % 12 == 0 && Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height)) {
                var bigBubble = ModContent.GetInstance<UnderwaterBubbleParticles>().New();
                bigBubble.Location = Projectile.Center;
                bigBubble.Frame = (byte)Main.rand.Next(1, 3);
                bigBubble.Velocity = Main.rand.NextVector2Unit() * -Projectile.velocity * 0.05f;
                bigBubble.UpLift = Main.rand.NextFloat(0.001f, 0.003f);
            }
        }
    }

    private void HealBreath(Player target) {
        Player owner = Main.player[Projectile.owner];
        AequusPlayer aequus = owner.GetModPlayer<AequusPlayer>();

        int restoreBreath = Math.Max((int)(target.breathMax * BreathConserver.RestoreBreathMaxOnEnemyKill), 1) * aequus.accBreathRestoreStacks;
        target.HealBreath(restoreBreath);

        if (Main.netMode != NetmodeID.Server) {
            var bigBubble = ModContent.GetInstance<UnderwaterBubbleParticles>().New();
            bigBubble.Location = target.MouthPosition ?? target.Center;
            bigBubble.Frame = (byte)Main.rand.Next(5, 7);
            bigBubble.Velocity = Vector2.Zero;
            bigBubble.UpLift = 0.005f;
        }
    }
}