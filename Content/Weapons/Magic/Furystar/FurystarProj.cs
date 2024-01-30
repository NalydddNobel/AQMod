using Aequus.Common.Projectiles;
using System;
using Terraria.DataStructures;

namespace Aequus.Content.Weapons.Magic.Furystar;

public class FurystarProj : HeldProjBase {
    public override String Texture => ModContent.GetInstance<Furystar>().Texture;

    public override void SetDefaults() {
        Projectile.SetDefaultHeldProj();
        Projectile.DamageType = DamageClass.Melee;
        Projectile.aiStyle = -1;
        Projectile.manualDirectionChange = true;
        Projectile.hide = true;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White with { A = 200 };
    }

    public override void AI() {
        var player = Main.player[Projectile.owner];
        var aequus = player.GetModPlayer<AequusPlayer>();
        player.heldProj = Projectile.whoAmI;
        if (Main.myPlayer != Projectile.owner || player.channel) {
            Projectile.timeLeft = 2;
            player.itemTime = Math.Max(player.itemTime, 2);
            player.itemAnimation = Math.Max(player.itemAnimation, 2);
        }

        if (player.ownedProjectileCounts[Type] > 1) {
            player.ownedProjectileCounts[Type]--;
            Projectile.Kill();
        }

        if (!player.CCed) {
            Projectile.Center = player.Center;
            Single rotationBase = 0f;
            var heldItem = player.HeldItemFixed();
            if (Main.myPlayer == Projectile.owner && heldItem.shoot == Type) {
                var mouseWorld = Main.MouseWorld;
                player.LimitPointToPlayerReachableArea(ref mouseWorld);
                var playerCenter = player.Center;
                rotationBase += Math.Clamp((mouseWorld.X - playerCenter.X) / 900f, -0.8f, 0.8f);
                Projectile.direction = Math.Sign(mouseWorld.X - playerCenter.X);
                if (Projectile.localAI[0] <= 1f) {
                    if (Projectile.localAI[0] > 0f) {
                        if (!player.CheckMana(heldItem.mana, pay: true)) {
                            Projectile.Kill();
                            return;
                        }
                    }
                    Projectile.localAI[0] += heldItem.useTime + 1f;
                    if (!CombinedHooks.CanShoot(player, heldItem)) {
                        Projectile.Kill();
                        return;
                    }
                    for (Int32 i = 0; i < Furystar.MaxExtraStars; i++) {
                        var position = new Vector2(mouseWorld.X + player.direction * -600f + Main.rand.NextFloat(-260f, 260f), player.position.Y - 400f);
                        var velocity = Vector2.Normalize(mouseWorld - position) * heldItem.shootSpeed;
                        var starFuryVector = mouseWorld;
                        var toMouse = (position - mouseWorld).SafeNormalize(new Vector2(0f, -1f));
                        while (starFuryVector.Y > position.Y && WorldGen.SolidTile(starFuryVector.ToTileCoordinates())) {
                            starFuryVector += toMouse * 16f;
                        }
                        Int32 projectileType = ModContent.ProjectileType<FurystarBulletProj>();
                        Int32 damage = player.GetWeaponDamage(heldItem);
                        Single knockback = player.GetWeaponKnockback(heldItem);
                        CombinedHooks.ModifyShootStats(player, heldItem, ref position, ref velocity, ref projectileType, ref damage, ref knockback);
                        var source = player.GetSource_ItemUse_WithPotentialAmmo(heldItem, 0, "Aequus: Furystar");
                        if (CombinedHooks.Shoot(player, heldItem, (EntitySource_ItemUse_WithAmmo)source, position, velocity, projectileType, damage, knockback)) {
                            Projectile.NewProjectile(player.GetSource_ItemUse(heldItem), position, velocity, projectileType, damage, knockback, player.whoAmI, ai0: heldItem.mana * 0.5f, ai1: starFuryVector.Y);
                        }
                        if (Main.rand.NextFloat() > Furystar.ExtraStarChance) {
                            break;
                        }
                    }
                }
                else {
                    Projectile.localAI[0]--;
                }
            }
            else {
                rotationBase += 0.4f * Projectile.direction;
            }

            if (Projectile.direction != player.direction) {
                player.ChangeDir(Projectile.direction);
                Projectile.netUpdate = true;
            }

            Single wantedRotation = Helper.Oscillate(Projectile.localAI[1], rotationBase - 0.3f, rotationBase + 0.3f);
            Projectile.rotation = Projectile.rotation.AngleLerp(wantedRotation, 0.33f);
            DrawOffsetX = (Int32)(wantedRotation * 4f);
            DrawOriginOffsetY = (Int32)Math.Abs(wantedRotation * 5f);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation * 0.45f + MathHelper.Pi + 0.7f * player.direction);
            var particleBox = Utils.CenteredRectangle(player.MountedCenter + new Vector2((player.width / 2f + 6f) * player.direction, -player.height / 2f - 20f), new(20f));
            if (Main.rand.NextBool(12)) {
                var d = Dust.NewDustDirect(new Vector2(particleBox.X, particleBox.Y), particleBox.Width, particleBox.Height, DustID.Enchanted_Pink, 0f, -2f, Alpha: 150, Scale: 1.2f);
                d.fadeIn = d.scale + 0.3f;
                d.velocity *= 0.8f;
                d.velocity.Y *= 0.5f;
            }
            if (Main.rand.NextBool(40)) {
                Gore.NewGore(player.GetSource_ItemUse(heldItem), new Vector2(particleBox.X, particleBox.Y), default(Vector2), Main.rand.Next(16, 18));
            }
            Projectile.localAI[1] += 1f / 12f;
        }
    }

    public override Boolean PreDraw(ref Color lightColor) {
        var player = Main.player[Projectile.owner];
        Projectile.GetDrawInfo(out var texture, out var _, out var frame, out var _, out Int32 _);
        var origin = new Vector2(10f, frame.Height - 10f);
        var armPosition = Main.GetPlayerArmPosition(Projectile);
        var drawPosition = armPosition - Main.screenPosition + new Vector2(DrawOffsetX, DrawOriginOffsetY);
        Main.EntitySpriteDraw(texture, drawPosition.Floor(), frame, Projectile.GetAlpha(lightColor), Projectile.rotation - MathHelper.PiOver4, origin, Projectile.scale, SpriteEffects.None, 0f);
        return false;
    }
}