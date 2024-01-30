using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Old.Content.TownNPCs.OccultistNPC;

public class OccultistProjSpawner : ModProjectile {
    public override System.String Texture => AequusTextures.TemporaryBuffIcon;

    public System.Int32 NPCIndex { get => (System.Int32)Projectile.ai[0] - 1; set => Projectile.ai[0] = value + 1; }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.hide = true;
        Projectile.npcProj = true;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Parent parentSource && parentSource.Entity is NPC npc) {
            NPCIndex = npc.whoAmI;
        }
    }

    public override void AI() {
        if (NPCIndex == -1) {
            NPCIndex = NPC.FindFirstNPC(ModContent.NPCType<Occultist>());
            if (NPCIndex == -1) {
                Projectile.Kill();
                return;
            }
        }
        else if (!Main.npc[NPCIndex].active || !Main.npc[NPCIndex].townNPC) {
            Projectile.Kill();
            return;
        }
        Projectile.Center = Main.npc[NPCIndex].Center;

        System.Int32 timeBetweenShots = 6;
        if ((System.Int32)Projectile.ai[1] % timeBetweenShots == 0) {
            var v = Vector2.Normalize(Projectile.velocity).RotatedBy(Projectile.ai[1] / timeBetweenShots / 5f * MathHelper.TwoPi);
            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Main.npc[NPCIndex].GetSource_FromAI(), Projectile.Center + v * 48f, v * 5f, ModContent.ProjectileType<OccultistProj>(), Projectile.damage / 2, Projectile.knockBack / 2f, Projectile.owner);
            if (Projectile.ai[1] >= timeBetweenShots * 4f)
                Projectile.Kill();
        }
        Projectile.ai[1]++;
    }

    public override void DrawBehind(System.Int32 index, List<System.Int32> behindNPCsAndTiles, List<System.Int32> behindNPCs, List<System.Int32> behindProjectiles, List<System.Int32> overPlayers, List<System.Int32> overWiresUI) {
    }

    public override System.Boolean PreDraw(ref Color lightColor) {
        return false;
    }
}