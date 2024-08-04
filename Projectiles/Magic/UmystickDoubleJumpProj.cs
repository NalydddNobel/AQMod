using System.IO;
using Terraria.Audio;

namespace Aequus.Projectiles.Magic;
public class UmystickDoubleJumpProj : ModProjectile {
    private bool _playedSound;
    private bool controlMouseRight;

    public override string Texture => Aequus.BlankTexture;

    public override void SetDefaults() {
        Projectile.width = 2;
        Projectile.height = 2;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
    }

    public override void AI() {
        Projectile.position = Main.player[Projectile.owner].position;
        if (!_playedSound) {
            controlMouseRight = true;
            _playedSound = true;
            SoundEngine.PlaySound(AequusSounds.UmystickJump, Main.player[Projectile.owner].Center);
        }

        if (Main.myPlayer == Projectile.owner) {
            bool flag = controlMouseRight;
            controlMouseRight = Main.mouseRight;
            if (flag != controlMouseRight) {
                Projectile.netUpdate = true;
            }
        }


    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(controlMouseRight);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        controlMouseRight = reader.ReadBoolean();
    }
}