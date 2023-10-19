using Aequus;
using Aequus.Common.Graphics;
using Aequus.Content.DataSets;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.Pumpinator;

public class PumpinatorProj : ModProjectile {
    public virtual bool PushPlayers => true;
    public virtual bool PushMyProjectiles => true;
    public virtual bool OnlyPushHostileProjectiles => false;
    public virtual bool OnlyPushHostilePlayers => false;
    public virtual bool PushItems => true;
    public virtual bool PushDust => true;
    public virtual bool PushGore => true;
    public virtual bool PushRain => true;
    public virtual bool PushUIObjects => true;

    public int dustPushIndex;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 90;
        Projectile.ignoreWater = true;
        Projectile.alpha = 255;
        Projectile.extraUpdates = 1;
    }

    public override bool? CanCutTiles() {
        return false;
    }

    public virtual Vector2 GetWindVelocity(Vector2 entityLocation, Vector2 entityVelocity) {
        return Projectile.velocity;
    }
    public virtual float GetWindSpeed(Vector2 entityLocation, Vector2 entityVelocity, Vector2 wantedVelocity) {
        return Math.Min(entityVelocity.Length(), wantedVelocity.Length() * (Projectile.extraUpdates + 1));
    }

    public virtual void OnPushNPC(NPC npc) {
    }
    public virtual void OnPushProj(Projectile proj) {
    }
    public virtual void OnPushItem(Item item) {
    }
    public virtual void OnPushPlayer(Player player) {
    }

    public override void AI() {
        if (Projectile.ai[0] == 0f) {
            Projectile.ai[0] = Main.rand.NextFloat(-0.3f, 0.3f);
            Projectile.scale *= Main.rand.NextFloat(0.2f, 1.25f);
            Projectile.velocity *= Main.rand.NextFloat(0.9f, 1.1f);
            Projectile.netUpdate = true;
            int frameID = Main.rand.Next(4);
            Projectile.ai[1] = (frameID) switch {
                > 3 => frameID + 10,
                _ => frameID + 14
            };
        }
        Projectile.velocity = Projectile.velocity.RotatedBy(MathF.Sin(Projectile.timeLeft * Projectile.ai[0]) * Projectile.ai[0] * 0.09f);
        int minX = (int)Projectile.position.X / 16;
        int minY = (int)Projectile.position.Y / 16;
        int maxX = minX + Math.Min(Projectile.width / 16, 1);
        int maxY = minY + Math.Min(Projectile.height / 16, 1);
        int colldingTiles = 0;
        for (int i = minX; i < maxX; i++) {
            for (int j = minY; j < maxY; j++) {
                if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType]) {
                    colldingTiles++;
                }
            }
        }
        if (colldingTiles > 8) {
            Projectile.velocity *= 0.97f - (colldingTiles - 8) * 0.01f;
        }
        if (Projectile.numUpdates == -1) {
            PushEntites();
        }

        DoDust();
        Projectile.rotation = (Projectile.velocity).ToRotation();
        if (Projectile.timeLeft < 40) {
            Projectile.alpha += 6;
        }
        else if (Projectile.alpha > 0) {
            Projectile.alpha -= 2 + (255 - Projectile.alpha) / 24;
            if (Projectile.alpha < 0) {
                Projectile.alpha = 0;
            }
        }
    }

    public Vector2 GetPushVelocity(Vector2 location, Vector2 velocity, float kbResist) {
        var wantedVelocity = GetWindVelocity(location, velocity);
        float speed = GetWindSpeed(location, velocity, wantedVelocity);
        var v = Vector2.Lerp(velocity, wantedVelocity, Projectile.knockBack * 0.01f * kbResist * Projectile.Opacity);
        if (v.Length() < speed) {
            v.Normalize();
            v *= speed;
        }
        return v;
    }

    public virtual void PushEntites() {
        var myRect = Projectile.getRect();
        for (int i = 0; i < Main.maxNPCs; i++) {
            var npc = Main.npc[i];
            if (!npc.active || npc.dontTakeDamage || npc.immortal || !Projectile.Colliding(myRect, npc.getRect())
                || (!NPCSets.PushableByTypeId.Contains(Main.npc[i].type) && !NPCSets.PushableByAI.Contains(Main.npc[i].aiStyle))) {
                continue;
            }
            if (npc.friendly || npc.townNPC || npc.isLikeATownNPC || npc.IsProbablyACritter()) {
                foreach (var buff in BuffSets.ProbablyFireDebuff) {
                    npc.ClearBuff(buff);
                }
            }
            npc.velocity = GetPushVelocity(npc.Center, npc.velocity, npc.knockBackResist);
            npc.netUpdate = true;
            OnPushNPC(npc);
        }
        for (int i = 0; i < Main.maxProjectiles; i++) {
            var proj = Main.projectile[i];
            if (i != Projectile.whoAmI && proj.active) {
                bool canPush = false;
                if (PushMyProjectiles) {
                    if (proj.owner == Main.myPlayer && !proj.GetGlobalProjectile<AequusProjectile>().HasNPCOwner) {
                        canPush = true;
                    }
                }
                if (!canPush && OnlyPushHostileProjectiles && !proj.hostile
                    || (!ProjectileSets.PushableByTypeId.Contains(Main.projectile[i].type) && !ProjectileSets.PushableByAI.Contains(Main.projectile[i].aiStyle)) || !Projectile.Colliding(myRect, proj.getRect())) {
                    continue;
                }

                proj.velocity = GetPushVelocity(proj.Center, proj.velocity, 1f);
                proj.netUpdate = true;
                OnPushProj(proj);
            }
        }
        if (PushItems) {
            for (int i = 0; i < Main.maxItems; i++) {
                var item = Main.item[i];
                if (item.active &&
                    Projectile.Colliding(myRect, item.getRect())) {
                    item.velocity = GetPushVelocity(item.Center, item.velocity, 2f);
                    OnPushItem(item);
                }
            }
        }
        if (PushPlayers) {
            for (int i = 0; i < Main.maxPlayers; i++) {
                var target = Main.player[i];
                if (i != Projectile.owner && target.active && !target.noKnockback) {
                    bool friendly = target.team == Main.player[Projectile.owner].team;
                    if (OnlyPushHostilePlayers && friendly) {
                        continue;
                    }
                    if (Projectile.Colliding(myRect, target.getRect())) {
                        if (friendly) {
                            foreach (var debuff in BuffSets.ProbablyFireDebuff) {
                                target.ClearBuff(debuff);
                            }
                        }
                        target.velocity = GetPushVelocity(target.Center, target.velocity, 0.5f);
                        OnPushPlayer(target);
                    }
                }
            }
        }
        myRect.Inflate(30, 30);
        if (Main.netMode != NetmodeID.Server) {
            if (PushDust) {
                var dustStopWatch = new Stopwatch();
                dustStopWatch.Start();
                for (; dustPushIndex < Main.maxDust; dustPushIndex++) {
                    int i = dustPushIndex;
                    if (Main.dust[i].active && Main.dust[i].scale <= 3f && myRect.Contains(Main.dust[i].position.ToPoint())) {
                        Main.dust[i].scale = Math.Max(Main.dust[i].scale, 0.2f);
                        Main.dust[i].velocity = GetPushVelocity(Main.dust[i].position, Main.dust[i].velocity.RotatedBy(Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f + i, -0.1f, 0.1f)), 2f);
                        if (dustStopWatch.ElapsedMilliseconds >= 1) {
                            break;
                        }
                    }
                }
                if (dustPushIndex >= Main.maxDust - 1) {
                    dustPushIndex = 0;
                }
                dustStopWatch.Stop();
            }
            if (PushGore) {
                for (int i = 0; i < Main.maxGore; i++) {
                    if (Main.gore[i].active && Main.gore[i].scale <= 3f && myRect.Contains(Main.gore[i].position.ToPoint())) {
                        Main.gore[i].timeLeft = Math.Max(Main.gore[i].timeLeft, 60);
                        Main.gore[i].velocity = GetPushVelocity(Main.gore[i].position, Main.gore[i].velocity, 1f);
                    }
                }
            }
            if (PushRain) {
                for (int i = 0; i < Main.maxRain; i++) {
                    if (Main.rain[i].active && myRect.Contains(Main.rain[i].position.ToPoint())) {
                        Main.rain[i].velocity = GetPushVelocity(Main.rain[i].position, Main.rain[i].velocity, 1f);
                    }
                }
            }
            if (PushUIObjects) {
                for (int i = 0; i < Main.maxCombatText; i++) {
                    if (Main.combatText[i].active && myRect.Contains(Main.combatText[i].position.ToPoint())) {
                        Main.combatText[i].lifeTime = Math.Max(Main.combatText[i].lifeTime, 90);
                        Main.combatText[i].velocity = GetPushVelocity(Main.combatText[i].position, Main.combatText[i].velocity, 1f);
                    }
                }
                for (int i = 0; i < Main.maxItemText; i++) {
                    if (Main.popupText[i].active && myRect.Contains(Main.popupText[i].position.ToPoint())) {
                        Main.popupText[i].lifeTime = Math.Max(Main.popupText[i].lifeTime, 90);
                        Main.popupText[i].velocity = GetPushVelocity(Main.popupText[i].position, Main.popupText[i].velocity, 1f);
                    }
                }
            }
        }
    }

    public virtual void DoDust() {
        if (Main.rand.NextFloat() < Projectile.Opacity / 10f) {
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDust, Alpha: 200, newColor: Color.White with { A = 128 }, Scale: 0.75f);
            d.velocity = new Vector2(-Projectile.velocity.X * 0.33f + Main.rand.NextFloat(-0.33f, 0.33f) + Main.windSpeedCurrent, -Projectile.velocity.Y * 0.33f + Main.rand.NextFloat(-0.33f, 0.33f));
            d.fadeIn = 1.5f;
            d.noGravity = true;
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(Projectile.width);
        writer.Write(Projectile.height);
        writer.Write(Projectile.extraUpdates);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        Projectile.width = reader.ReadInt32();
        Projectile.height = reader.ReadInt32();
        Projectile.extraUpdates = reader.ReadInt32();
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int _);
        var texture = TextureAssets.Cloud[(int)Projectile.ai[1]].Value;
        AequusDrawing.DrawBasicVertexLine(texture, Projectile.oldPos, Projectile.oldRot,
            (p) => Lighting.GetColor(Projectile.Center.ToTileCoordinates()) with { A = 200 } * 0.2f * (1f - p) * Projectile.Opacity * Projectile.scale,
            (p) => (0.2f + MathF.Sin(p * MathHelper.PiOver2) * 0.7f * Projectile.Opacity * Projectile.scale) * texture.Height,
            -Main.screenPosition + Projectile.Size / 2f
        );
        //Main.EntitySpriteDraw(t, Projectile.position + off - Main.screenPosition, frame, new Color(128, 128, 128, 0) * Projectile.Opacity, Projectile.rotation,
        //    origin, new Vector2(scale * 1.3f, scale * 0.9f), SpriteEffects.None, 0);
        return false;
    }
}