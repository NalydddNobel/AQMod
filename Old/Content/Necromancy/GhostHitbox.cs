using Aequus.Common;
using Aequus.Old.Content.Necromancy.Rendering;
using System;

namespace Aequus.Old.Content.Necromancy;

public class GhostHitbox : ModProjectile {
    public override string Texture => AequusTextures.TemporaryBuffIcon;

    public int NPC => (int)Projectile.ai[0];

    public override void SetStaticDefaults() {
        ProjectileID.Sets.MinionShot[Type] = true;
    }

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.penetrate = -1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.DamageType = Commons.NecromancyClass;
        Projectile.localNPCHitCooldown = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 120;
        Projectile.ignoreWater = true;
        Projectile.hide = true;
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public override void AI() {
        int npc = (int)Projectile.ai[0];
        if (Main.npc[npc].active && Main.npc[npc].TryGetGlobalNPC<NecromancyNPC>(out var zombie) && zombie.isZombie) {
            if (Main.netMode != NetmodeID.SinglePlayer && zombie.zombieTimer > zombie.zombieTimerMax - 30) {
                Projectile.timeLeft = Math.Max(Projectile.timeLeft, 10);
            }
            else {
                Projectile.timeLeft = 2;
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 20f) {
                Main.npc[npc].netUpdate = true;
            }
        }
        int size = (int)Projectile.ai[1];
        Projectile.position = Main.npc[npc].position - new Vector2(size / 2f);
        Projectile.width = Main.npc[npc].width + size;
        Projectile.height = Main.npc[npc].height + size;
        Projectile.wet = Main.npc[npc].wet;
        Projectile.lavaWet = Main.npc[npc].lavaWet;
        Projectile.honeyWet = Main.npc[npc].honeyWet;
        Projectile.velocity = Vector2.Normalize(Main.npc[npc].velocity) * 0.1f;
    }

    public override bool? CanHitNPC(NPC target) {
        return NPCLoader.CanHitNPC(Main.npc[NPC], target) ? null : false;
    }

    public override bool CanHitPlayer(Player target) {
        int c = CooldownSlot;
        var value = NPCLoader.CanHitPlayer(Main.npc[NPC], target, ref c);
        CooldownSlot = c;
        return value;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        if (target.life > 0 && Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().ghostChains > 0) {
            var zombie = Main.npc[NPC].GetGlobalNPC<NecromancyNPC>();
            zombie.ghostChainsTime = 300 * Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().ghostChains;
            if (zombie.ghostChainsNPC != target.whoAmI) {
                zombie.ghostChainsNPC = target.whoAmI;
                var diff = target.Center - Projectile.Center;
                int amt = (int)(diff.Length() / 4f);
                var clr = GhostRenderer.GetColorTarget(Main.player[Projectile.owner], zombie.renderLayer).getDrawColor() with { A = 25 } * 0.8f;
                for (int i = 0; i < amt; i++) {
                    var d = Dust.NewDustDirect(Projectile.Center + diff * Main.rand.NextFloat(1f) - new Vector2(12f, 12f), 24, 24,
                        DustID.SilverFlame, newColor: clr.HueAdd(Main.rand.NextFloat(-0.02f, 0.02f)) with { A = 25 }, Scale: Main.rand.NextFloat(0.75f, 1.33f));
                    d.velocity *= 1.2f;
                    d.fadeIn = d.scale + 1f;
                    d.noGravity = true;
                }
            }
        }
    }
}