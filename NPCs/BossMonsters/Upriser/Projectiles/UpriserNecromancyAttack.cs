using Aequus.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Aequus.NPCs.BossMonsters.Upriser.Projectiles;

public class UpriserNecromancyAttack : ModProjectile {
    public int ConnectedNPC { get => (int)Projectile.ai[0] - 1; set => Projectile.ai[0] = value + 1; }

    public override string Texture => AequusTextures.UpriserChainsUV.FullPath;

    private Vector2[] _chainSegments;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults() {
        Projectile.DisableWorldInteractions();
        Projectile.width = 16;
        Projectile.height = 16;
        _chainSegments = new Vector2[20];
    }

    public override void AI() {
        if (ConnectedNPC < 0) {
            ConnectedNPC = NPC.FindFirstNPC(ModContent.NPCType<Upriser>());
        }
        if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height)) {
            Projectile.velocity *= 0.85f;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        if (ConnectedNPC < 0) {
            return false;
        }
        var upriser = Main.npc[ConnectedNPC];
        var texture = TextureAssets.Projectile[Type].Value;
        var difference = (upriser.Center - Projectile.Center);
        var directionTowards = Vector2.Normalize(difference);
        var offsetVector = Vector2.One * 8f;
        var chainLocation = Projectile.Center - directionTowards * 16f;
        //for (int i = 0; i < chainLength; i++) {
        //    _chainSegments[i] = chainLocation + directionTowards + offsetVector.RotatedBy(MathF.Sin(Helper.CalcProgress(chainLength, i) * MathHelper.Pi) * MathF.Sin(Main.GlobalTimeWrappedHourly * 2.5f + i * 0.5f + Projectile.whoAmI * 30));
        //    chainLocation += directionTowards;
        //}
        float chainDistance = difference.Length();
        var textureOrigin = texture.Size() / 2f;
        float chainHeight = texture.Height;
        Vector2 chainVelocity = directionTowards * chainHeight;
        float chainDistanceMax = chainDistance;
        for (int i = 0; i < 300 && chainDistance > 0f; i++) {
            float chainRotation = chainVelocity.ToRotation();
            Main.spriteBatch.Draw(texture, chainLocation - Main.screenPosition, null, Color.White, chainRotation, textureOrigin, Projectile.scale, SpriteEffects.None, 0f);
            chainVelocity = Vector2.Normalize(Vector2.Lerp(chainVelocity.RotatedBy(MathF.Sin(i * 0.33f) * 0.2f), directionTowards, 1f - chainDistance / chainDistanceMax)) * chainHeight;
            chainLocation += chainVelocity;
            chainDistance -= chainHeight;
        }
        return false;
    }
}