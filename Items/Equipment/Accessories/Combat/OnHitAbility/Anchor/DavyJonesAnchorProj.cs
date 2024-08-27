using Aequus;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.Anchor;

public class DavyJonesAnchorProj : ModProjectile {
    public int AttatchedNPC { get => (int)Projectile.ai[0]; set => Projectile.ai[0] = value; }

    private int _hitNPCs;

    public override void SetStaticDefaults() {
        LegacyPushableEntities.AddProj(Type);
    }

    public override void SetDefaults() {
        Projectile.aiStyle = -1;
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.hide = true;
        Projectile.penetrate = -1;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.alpha = 255;
        Projectile.ArmorPenetration = 10;
        Projectile.timeLeft = 720;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Generic;
        _hitNPCs = 1;
    }

    public override bool? CanCutTiles() {
        return false;
    }
    public override bool? CanDamage() {
        return _hitNPCs > 0;
    }

    public override bool? CanHitNPC(NPC target) {
        return target.whoAmI != AttatchedNPC;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        _hitNPCs--;
    }

    public override void AI() {
        if (AttatchedNPC >= 0) {
            if (!Main.npc[AttatchedNPC].active) {
                Projectile.Kill();
                return;
            }

            if (Projectile.timeLeft > 10 && Projectile.ai[2] > 30f) {
                for (int i = 0; i < Main.maxProjectiles; i++) {
                    var p = Main.projectile[i];
                    if (p.active && p.type == Type && p.owner == Projectile.owner && p.timeLeft >= Projectile.timeLeft && Projectile.whoAmI != i && (int)p.ai[0] == AttatchedNPC) {
                        Projectile.timeLeft = 10;
                        return;
                    }
                }
            }
        }

        if (Projectile.timeLeft <= 10) {
            if (Projectile.alpha < 255) {
                Projectile.alpha += 20;
                if (Projectile.alpha > 255) {
                    Projectile.alpha = 255;
                }
            }
        }
        else {
            if (Projectile.alpha > 0) {
                Projectile.alpha -= 20;
                if (Projectile.alpha < 0) {
                    Projectile.alpha = 0;
                }
            }
        }

        Projectile.CollideWithOthers();

        if (Projectile.ai[2] > 20f) {
            if (Projectile.tileCollide) {
                Projectile.velocity.Y += 0.4f;
            }
        }
        else if (Projectile.velocity.Length() < 6f) {
            Projectile.velocity.Normalize();
            Projectile.velocity *= 14f;
        }
        else {
            Projectile.velocity *= 0.95f;
        }
        Projectile.ai[2]++;

        var npc = Main.npc[AttatchedNPC];
        var npcCenter = npc.Center;
        float minDistance = 100f;
        float distance = Projectile.Distance(npcCenter);
        Projectile.ai[1] = 0f;
        bool solidCollision = Collision.SolidCollision(Projectile.Center - new Vector2(4f), 8, 8);
        if (!npc.noTileCollide && !solidCollision) {
            Projectile.tileCollide = true;
        }
        if (distance > minDistance) {
            var v = Projectile.DirectionTo(npcCenter);
            var velocityAdd = v * ((distance - minDistance / 2f) / minDistance) * 0.4f;
            float pullResist = Math.Clamp(MathF.Pow(npc.knockBackResist, 2f) * 2f, 0f, 1f);
            float anchorResist = Projectile.ai[2] < 60f ? 1f : 0.1f;
            if (distance > minDistance * 2f) {
                Projectile.tileCollide = false;
            }
            if (!Projectile.tileCollide) {
                Projectile.velocity += v;
            }
            if (solidCollision) {
                anchorResist = 1f;
            }

            Projectile.velocity += velocityAdd * anchorResist * MathHelper.Clamp(1f - pullResist, 0f, 1f);
            if (Projectile.velocity.Length() > 15f) {
                Projectile.velocity *= 0.9f;
            }

            var oldVelocity = npc.velocity;
            float speed = oldVelocity.Length();
            npc.velocity -= velocityAdd * pullResist;
            if (speed > 6f) {
                npc.velocity.Normalize();
                npc.velocity *= speed;
            }
            if ((int)Projectile.ai[1] != 1 || Projectile.localAI[0] > 20f) {
                npc.netUpdate = true;
            }
            Projectile.localAI[0]++;
            Projectile.ai[1] = 1f;
        }

        if (Projectile.velocity.Y.Abs() < 0.8f) {
            Projectile.velocity.X *= 0.9f;
        }

        if (Projectile.velocity.Length() <= 0.11f) {
            return;
        }

        Projectile.rotation = (npcCenter - Projectile.Center).ToRotation() + MathHelper.PiOver2;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
        width = 10;
        height = 10;
        fallThrough = Main.npc[AttatchedNPC].position.Y > Projectile.position.Y + Projectile.height;
        return true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if ((int)Projectile.ai[1] == 1 || oldVelocity.Length() > 3f) {
            float length = oldVelocity.Length();
            if (oldVelocity.X != Projectile.velocity.X) {
                Projectile.velocity.X = -oldVelocity.X * 0.3f;
            }
            if (oldVelocity.Y != Projectile.velocity.Y) {
                Projectile.velocity.Y = -oldVelocity.Y * 0.3f;
            }
            if (length > 6f) {
                Projectile.velocity.X += Main.rand.NextFloat(-2f, 2f);
                Projectile.velocity.Y += Main.rand.NextFloat(-2f, 0f);
                Projectile.netUpdate = true;
                Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            }
        }
        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        behindNPCs.Add(index);
    }

    public override bool PreDraw(ref Color lightColor) {
        Main.instance.LoadTiles(TileID.Rope);
        var chain = TextureAssets.Tile[TileID.Rope].Value;
        var frame = new Rectangle(90, 0, 16, 16);
        int height = frame.Height - 2;
        var currentPosition = Projectile.Center - new Vector2(0f, Projectile.height / 2f * Projectile.scale - 8f).RotatedBy(Projectile.rotation);
        var velocity = Main.npc[AttatchedNPC].Center - currentPosition;
        int length = (int)(velocity.Length() / height);
        velocity.Normalize();
        velocity *= height;
        float rotation = velocity.ToRotation() + MathHelper.PiOver2;
        var origin = new Vector2(frame.Width / 2f, frame.Height / 2f);
        int fade = Math.Min(5, length);
        for (int i = 0; i < length; i++) {
            var position = currentPosition + velocity * i;
            float opacity = 1f;
            if (i > length - fade) {
                opacity *= 1f - (i - (length - fade)) / 5f;
            }
            Main.EntitySpriteDraw(chain, position - Main.screenPosition, frame, Helper.GetColor(position) * Projectile.Opacity * opacity, rotation, origin, 1f, SpriteEffects.None, 0);
        }
        Projectile.GetDrawInfo(out var t, out var offset, out frame, out origin, out int _);

        int trailTime = 30;
        if (Projectile.ai[2] < trailTime && Projectile.velocity.Length() > 2f) {
            float opacity = Math.Clamp((1f - Projectile.ai[2] / trailTime) * (Projectile.velocity.Length() / 12f) * Projectile.Opacity, 0f, 1f);
            var color = Color.CadetBlue.HueAdd(Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0f, 0.1f)) with { A = 50 } * opacity;
            int trailLength = 14;
            for (int i = 0; i < trailLength; i++) {
                Main.EntitySpriteDraw(
                    t,
                    Projectile.position + offset - Main.screenPosition - Projectile.velocity * 0.33f * i,
                    frame,
                    color * Helper.CalcProgress(trailLength, i),
                    Projectile.rotation,
                    origin,
                    Projectile.scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(
                AequusTextures.Bloom0,
                Projectile.position + offset - Main.screenPosition,
                null,
                color,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                Projectile.scale * 0.8f,
                SpriteEffects.None,
                0
            );
        }

        Main.EntitySpriteDraw(t, Projectile.position + offset - Main.screenPosition, frame, Helper.GetColor(Projectile.position + offset) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
}