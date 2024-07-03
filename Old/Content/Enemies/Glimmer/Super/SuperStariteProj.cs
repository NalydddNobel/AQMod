using Aequu2.Old.Common.Projectiles;

namespace Aequu2.Old.Content.Enemies.Glimmer.Super;

public class SuperStariteProj : EnemyAttachedProjBase {
    public override string Texture => Aequu2Textures.None.Path;

    public override void SetDefaults() {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.hide = true;
    }

    protected override bool CheckAttachmentConditions(NPC npc) {
        return (int)npc.ai[0] != -1 && npc.ModNPC is SuperStarite;
    }

    protected override void AIAttached(NPC npc) {
        Projectile.position += (npc.rotation + MathHelper.TwoPi / 5f * Projectile.ai[1] - MathHelper.PiOver2).ToRotationVector2() * (npc.height * npc.scale + npc.ai[3] + 18f);
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        if (target.townNPC || target.life < 5)
            modifiers.SetMaxDamage(1);
    }
}