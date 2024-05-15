using System;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Old.Content.Equipment.GrapplingHooks.EnemyGrappleHook;

public class MeathookProj : ModProjectile {
    private bool _playedChainSound;
    public int ConnectedNPC { get; set; } = -1;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.SingleGrappleHook[Type] = true;
    }

    public override void SetDefaults() {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.aiStyle = 7;
        Projectile.netImportant = true;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft *= 10;
        Projectile.extraUpdates = 1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 5;
        ConnectedNPC = -1;
    }

    public override void ModifyDamageHitbox(ref Rectangle hitbox) {
        hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2(), hitbox.Size() * 6f);
    }

    public override bool PreAI() {
        if (Projectile.position.HasNaNs()) {
            Projectile.Kill();
        }
        if (ConnectedNPC > -1 && !Main.npc[ConnectedNPC].active) {
            ConnectedNPC = -1;
        }
        var player = Main.player[Projectile.owner];
        if (ConnectedNPC != -1 && !player.dead) {
            Projectile.ai[0] = 2f;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].aiStyle == ProjAIStyleID.Hook && Main.projectile[i].owner == Projectile.owner) {
                    Main.projectile[i].Kill();
                }
            }
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
                Projectile.Kill();
                return false;
            }

            Main.npc[ConnectedNPC].AddBuff(ModContent.BuffType<MeathookDebuff>(), 20, quiet: true);

            Projectile.Center = Main.npc[ConnectedNPC].Center;
            float distance = Projectile.Distance(player.Center);
            if (player.grapCount < 10) {
                float size = Main.npc[ConnectedNPC].Size.Length();
                if (distance < size * 6f) {
                    if (!_playedChainSound) {
                        SoundEngine.PlaySound(AequusSounds.MeathookPull with { Volume = 0.8f, Pitch = -0.1f, PitchVariance = 0.1f, }, Main.npc[ConnectedNPC].Center);
                        _playedChainSound = true;
                    }
                }

                if (distance < size * 2f && player.TryGetModPlayer(out AequusPlayer aequusPlayer) && !aequusPlayer.TimerActive(Meathook.IMMUNE_TIMER)) {
                    player.immune = true;
                    player.immuneTime = 12;
                    player.GetModPlayer<AequusPlayer>().SetTimer(Meathook.IMMUNE_TIMER, player.immuneTime * 2);
                }
                if (distance < 64f) {
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 2);
                }
                player.grappling[player.grapCount] = Projectile.whoAmI;
                player.grapCount++;
            }
            return false;
        }
        if ((int)Projectile.ai[0] == 1) {
            Projectile.damage = 0;
        }
        return true;
    }

    public override bool? CanHitNPC(NPC target) {
        return target.immortal ? false : null;
    }

    public override bool? CanUseGrapple(Player player) {
        for (int l = 0; l < Main.maxProjectiles; l++) {
            if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type) {
                return (int)Main.projectile[l].ai[0] == 2;
            }
        }
        return true;
    }

    public override void UseGrapple(Player player, ref int type) {
        int hooksOut = 0;
        int oldestHookIndex = -1;
        int oldestHookTimeLeft = 100000;
        for (int i = 0; i < 1000; i++) {
            if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.whoAmI && Main.projectile[i].type == Projectile.type) {
                hooksOut++;
                if (Main.projectile[i].timeLeft < oldestHookTimeLeft) {
                    oldestHookIndex = i;
                    oldestHookTimeLeft = Main.projectile[i].timeLeft;
                }
            }
        }
        if (hooksOut > 1)
            Main.projectile[oldestHookIndex].Kill();
    }

    public override float GrappleRange() {
        return 480f;
    }

    public override void NumGrappleHooks(Player player, ref int numHooks) {
        numHooks = 1;
    }

    public override void GrappleRetreatSpeed(Player player, ref float speed) {
        speed = 12f;
    }

    public override void GrapplePullSpeed(Player player, ref float speed) {
        speed = ConnectedNPC > 0 ? 12f : 12f;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
        modifiers.SetMaxDamage(target.life / 2);
        modifiers.Knockback *= 0.25f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.ai[0] = 0f;
        ConnectedNPC = target.whoAmI;
        Projectile.tileCollide = false;
        Projectile.netUpdate = true;
        SoundEngine.PlaySound(AequusSounds.Meathook, target.Center);
    }

    public override bool PreDrawExtras() {
        return false;
    }

    public override bool PreDraw(ref Color lightColor) {
        var player = Main.player[Projectile.owner];
        float playerLength = (player.Center - Projectile.Center).Length();
        Helper.DrawChain(AequusTextures.MeathookProj_Chain, Projectile.Center, player.Center, Main.screenPosition);
        var texture = TextureAssets.Projectile[Type].Value;
        var drawPosition = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(ConnectedNPC);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        ConnectedNPC = reader.ReadInt32();
    }
}
