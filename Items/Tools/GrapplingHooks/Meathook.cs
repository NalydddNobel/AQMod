using Aequus.Common.Audio;
using Aequus.Projectiles.Misc.GrapplingHooks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Tools.GrapplingHooks {
    public class Meathook : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.DualHook);
            Item.damage = 30;
            Item.knockBack = 0f;
            Item.shoot = ModContent.ProjectileType<MeathookProj>();
            Item.value = Item.buyPrice(gold: 10);
            Item.shootSpeed /= 2f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            tooltips.RemoveCritChance();
            tooltips.RemoveKnockback();
        }

        public override bool WeaponPrefix() {
            return true;
        }
    }
}

namespace Aequus.Projectiles.Misc.GrapplingHooks {
    public class MeathookProj : ModProjectile {
        private bool _playedChainSound;
        public int connectedNPC;

        public MeathookProj() {
            connectedNPC = -1;
        }

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
            connectedNPC = -1;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) {
            hitbox = Utils.CenteredRectangle(hitbox.Center.ToVector2(), hitbox.Size() * 6f);
        }

        public override bool PreAI() {
            if (Projectile.position.HasNaNs()) {
                Projectile.Kill();
            }
            if (connectedNPC > -1 && !Main.npc[connectedNPC].active) {
                connectedNPC = -1;
            }
            if (connectedNPC != -1) {
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

                var aequusNPC = Main.npc[connectedNPC].Aequus();
                aequusNPC.meathookDamage = Math.Max(aequusNPC.meathookDamage, 0.1f);
                aequusNPC.meathookDamageTime = Math.Max(aequusNPC.meathookDamageTime, 20);
                var player = Main.player[Projectile.owner];
                player.Aequus().grappleNPC = connectedNPC;
                Projectile.Center = Main.npc[connectedNPC].Center;
                float distance = Projectile.Distance(player.Center);
                if (player.grapCount < 10) {
                    float size = Main.npc[connectedNPC].Size.Length();
                    if (distance < size * 6f) {
                        if (!_playedChainSound) {
                            SoundEngine.PlaySound(AequusSounds.meathookPull with { Volume = 0.8f, Pitch = -0.1f, PitchVariance = 0.1f, }, Main.npc[connectedNPC].Center);
                            _playedChainSound = true;
                        }
                    }
                    if (distance < size * 2f) {
                        player.immune = true;
                        player.immuneTime = 12;
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
            speed = connectedNPC > 0 ? 12f : 12f;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
            modifiers.SetMaxDamage(target.life / 2);
            modifiers.Knockback *= 0.25f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.ai[0] = 0f;
            connectedNPC = target.whoAmI;
            Projectile.tileCollide = false;
            ModContent.GetInstance<MeathookOnHitSound>().Play(target.Center);
        }

        public override bool PreDrawExtras() {
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            var player = Main.player[Projectile.owner];
            float playerLength = (player.Center - Projectile.Center).Length();
            Helper.DrawChain(ModContent.Request<Texture2D>(Texture + "_Chain", AssetRequestMode.ImmediateLoad).Value, Projectile.Center, player.Center, Main.screenPosition);
            var texture = TextureAssets.Projectile[Type].Value;
            var drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            return false;
        }
    }

    public class MeathookOnHitSound : NetSound {
        protected override SoundStyle InitDefaultSoundStyle() {
            return AequusSounds.meathookConnect with { Volume = 0.8f };
        }
    }

    public class MeathookIncreasedDamageSound : NetSound {
        protected override SoundStyle InitDefaultSoundStyle() {
            return AequusSounds.meathook.Sound with { Volume = 0.6f, PitchVariance = 0.1f, MaxInstances = 8, };
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {

        /// <summary>
        /// Local variable for the meathook damage increase. Is not synced between clients and server.
        /// </summary>
        public float meathookDamage;
        /// <summary>
        /// Local variable for how long the meathook damage increase lasts before it gets cleared. Is not synced between clients and server.
        /// </summary>
        public int meathookDamageTime;
        private bool _meathookSound;

        private void ResetEffects_Meathook() {
            if (meathookDamageTime > 0) {
                meathookDamageTime--;
            }
            else {
                meathookDamage = 0f;
            }
            _meathookSound = false;
        }

        private void ModifyHit_ProcMeathook(NPC target, ref NPC.HitModifiers modifiers) {
            if (meathookDamage > 0f) {
                _meathookSound = true;
                modifiers.FinalDamage += meathookDamage;
                modifiers.Knockback *= 0.1f;
            }
        }

        private void OnHit_PlayMeathookSound(NPC target) {
            if (_meathookSound) {
                ModContent.GetInstance<MeathookIncreasedDamageSound>().Play(target.Center);
            }
        }
    }
}