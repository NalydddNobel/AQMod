using Aequu2.Core.Entities.Projectiles;
using System.Collections.Generic;
using Terraria.Audio;

namespace Aequu2.Content.Items.Weapons.Summon.Whips.DemonCorruptWhip;

public class SoulscourgeTagProj : ModProjectile {
    public override string Texture => AequusTextures.NPC(NPCID.BoneSerpentBody);

    private bool _immuneToggle;
    private float Z { get => Projectile.localAI[0]; set => Projectile.localAI[0] = value; }

    public void GetOrbitPosition(out Vector3 coords) {
        Player player = Main.player[Projectile.owner];

        Vector2 anchor2D = player.Center;
        coords = new Vector3(anchor2D.X, anchor2D.Y, 0f);

        float rotationTime = Main.GlobalTimeWrappedHourly;
    }

    public override void SetStaticDefaults() {
    }

    public override void SetDefaults() {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.timeLeft = 360;
        Projectile.ArmorPenetration = 20;
        Projectile.hide = true;
    }

    public override void AI() {
        Player player = Main.player[Projectile.owner];

        if (player.immune) {
            if (!_immuneToggle) {

            }
            _immuneToggle = true;
        }
        else {
            _immuneToggle = false;
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity) {
        if (Projectile.velocity.X != oldVelocity.X) {
            Projectile.velocity.X = -oldVelocity.X;
        }
        if (Projectile.velocity.Y != oldVelocity.Y) {
            Projectile.velocity.Y = -oldVelocity.Y;
        }
        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
        if (Z > 0f) {
            behindProjectiles.Add(index);
        }
        else {
            overPlayers.Add(index);
        }
    }

    public override void OnKill(int timeLeft) {
        if (timeLeft <= 0) {
            SoundEngine.PlaySound(SoundID.Item112 with { Pitch = 0.8f, }, Projectile.Center);
        }

        for (int i = 0; i < 10; i++) {
            Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, Scale: 1.25f);
            d.velocity *= 1.5f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Projectile.GetDrawInfo(out Texture2D texture, out Vector2 offset, out Rectangle frame, out Vector2 origin, out int _);

        Vector2 position = Projectile.position + offset;
        Player player = Main.player[Projectile.owner];
        Vector2 endPosition = player.Center;
        Vector2 difference = endPosition - position;
        float length = difference.Length();

        Main.EntitySpriteDraw(texture, position - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(30f, frame.Height / 2f - 1f), Projectile.scale, SpriteEffects.None);

        return false;
    }
}
