using Aequus.Old.Common.Projectiles;

namespace Aequus.Old.Content.Bosses.Cosmic.UltraStarite.Projectiles;

public class UltraStariteOuterArms : EnemyAttachedProjBase {
    public override string Texture => AequusTextures.None.Path;

    public override void SetDefaults() {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.hide = true;
        Projectile.penetrate = -1;
    }

    public override bool? CanDamage() {
        if (!Main.npc.IndexInRange(AttachedNPC)) {
            return false;
        }

        NPC npc = Main.npc[AttachedNPC];
        return (int)npc.ai[0] != UltraStarite.STATE_DEATHRAY && (int)npc.ai[0] != UltraStarite.STATE_DEATHRAY_TRANSITION_END;
    }

    protected override bool CheckAttachmentConditions(NPC npc) {
        return (int)npc.ai[0] != -1 && npc.ModNPC is UltraStarite;
    }

    protected override void AIAttached(NPC npc) {
        Projectile.position += (npc.rotation + MathHelper.TwoPi / 5f * Projectile.ai[1] - MathHelper.PiOver2).ToRotationVector2() * (npc.height * npc.scale + npc.ai[3] + 130f);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        if (target.townNPC || target.life < 5)
            modifiers.SetMaxDamage(1);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        Main.npc[AttachedNPC].ModNPC.OnHitPlayer(target, info); // janky magic :trollface:
    }
}