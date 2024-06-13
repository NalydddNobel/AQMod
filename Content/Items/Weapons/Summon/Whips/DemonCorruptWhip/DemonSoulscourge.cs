using Aequus.Common;
using Aequus.Common.Elements;
using Aequus.Core.ContentGeneration;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Content.Items.Weapons.Summon.Whips.DemonCorruptWhip;

[WorkInProgress]
public class DemonSoulscourge : UnifiedWhipItem, IMinionTagController {
    public ModBuff TagBuff { get; set; }
    public int TagDuration => 240;

    public override void SetStaticDefaults() {
        Element.Flame.AddItem(Type);
    }

    public override void SetDefaults() {
        Item.DefaultToWhip(WhipProjectile.Type, 32, 2f, 5f, animationTotalTime: 30);
        Item.rare = Commons.Rare.EventDemonSiege;
        Item.value = Commons.Cost.EventDemonSiege;
    }

    public override void SetWhipSettings(Projectile projectile, ref WhipSettings settings) {
        settings.Segments = 27;
        settings.RangeMultiplier = 1f;
    }

    public override Color GetWhipStringColor(Vector2 position) {
        return ExtendLight.Get(position, Color.BlueViolet);
    }

    public override void DrawWhip(IWhipController.WhipDrawParams drawInfo) {
        Texture2D texture = drawInfo.Texture;
        int i = drawInfo.SegmentIndex;
        int count = drawInfo.SegmentCount;

        Vector2 originOffset = Vector2.Zero;
        int frameIndex;
        if (i == count - 2) { frameIndex = 0; }
        //else if (i > 10) { frameIndex = 1; }
        //else if (i > 5) { frameIndex = 2; }
        //else if (i > 0) { frameIndex = 1; }
        else if (i > 0) { frameIndex = i % 3 + 1; }
        else {
            frameIndex = 4;
            originOffset = new Vector2(0f, 8f);
        }

        Rectangle frame = texture.Frame(2, 5, 0, frameIndex);

        Color lightColor = ExtendLight.Get(drawInfo.Position);
        float rotation = (drawInfo.Position - drawInfo.Next).ToRotation() - MathHelper.PiOver2;
        Vector2 origin = frame.Size() / 2f + originOffset;
        float scale = drawInfo.Projectile.scale;
        SpriteEffects effects = drawInfo.SpriteEffects;

        Main.EntitySpriteDraw(texture, drawInfo.Position - Main.screenPosition, frame, lightColor, rotation, origin, scale, effects, 0);
        Main.EntitySpriteDraw(texture, drawInfo.Position - Main.screenPosition, frame with { X = frame.Width }, Color.White, rotation, origin, scale, effects, 0);
    }

    public override void WhipAI(Projectile projectile, List<Vector2> WhipPoints, float Progress) {
        Vector2 endPoint = WhipPoints[^1];
        if (Vector2.Distance(endPoint, Main.player[projectile.owner].Center) > 32f && projectile.localAI[0] > 0f) {
            Vector2 velocity = (endPoint - new Vector2(projectile.localAI[0], projectile.localAI[1])) * 0.1f;

            for (int i = 0; i < 3; i++) {
                Dust d = Dust.NewDustDirect(endPoint - new Vector2(16f), 32, 32, DustID.Shadowflame, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(0.6f, 2f));
                d.noGravity = true;
            }
        }

        projectile.localAI[0] = endPoint.X;
        projectile.localAI[1] = endPoint.Y;
    }

    public override void OnWhipHitNPC(ref float damagePenalty, Projectile whip, NPC target, in NPC.HitInfo hit, int damageDone) {
        target.AddBuff(BuffID.ShadowFlame, 240 + Main.rand.Next(2) * 60);
    }

    void IMinionTagNPCController.OnMinionHit(NPC npc, Projectile minionProj, in NPC.HitInfo hit, int damageDone) {
        IEntitySource source = minionProj.GetSource_OnHit(npc);
        Vector2 spawnLocation = npc.Center;
        int type = ModContent.ProjectileType<SoulscourgeTagProj>();
        int damage = minionProj.damage;
        float knockback = minionProj.knockBack;
        int owner = minionProj.owner;

        Projectile.NewProjectile(source, npc.Center, Main.rand.NextVector2Unit(), type, damage, knockback, owner);

        this.RemoveTagBuff(npc);
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override bool AltFunctionUse(Player player) {
        return player.ownedProjectileCounts[ModContent.ProjectileType<SoulscourgeTagProj>()] > 0;
    }

}
