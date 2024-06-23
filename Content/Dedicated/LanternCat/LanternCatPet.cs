using Aequus.Common.Items.Dedications;
using Aequus.Core.ContentGeneration;
using Aequus.Core.Entities.Projectiles;
using System;

namespace Aequus.Content.Dedicated.LanternCat;

public class LanternCatPet : UnifiedModPet {
    private int frame;
    private double frameCounter;

    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 10;
        Main.projPet[Type] = true;
        ProjectileID.Sets.LightPet[Type] = true;
    }

    public override void SetDefaults() {
        Projectile.width = 20;
        Projectile.height = 14;
        Projectile.friendly = true;
        Projectile.aiStyle = ProjAIStyleID.Pet;
        DrawOriginOffsetY = -9;
        AIType = ProjectileID.BlackCat;
    }

    public override bool PreAI() {
        if (!base.PreAI()) {
            return false;
        }
        Lighting.AddLight(Projectile.Top, new Vector3(0.8f, 0.6f, 0.2f));
        if ((int)Projectile.ai[0] == 1) {
            frameCounter++;
            if (frameCounter > 6.0d) {
                frameCounter = 0.0d;
                frame++;
                if (frame >= Main.projFrames[Type]) {
                    frame = 6;
                }
            }
            if (frame < 6) {
                frame = 6;
            }
        }
        else if (Projectile.velocity.Y != 0f) {
            frame = 3;
        }
        else {
            float speedX = Math.Abs(Projectile.velocity.X);
            if (speedX < 0.1f) {
                frameCounter = 0.0d;
                frame = 0;
            }
            else {
                frameCounter += speedX * 0.5f;
                if (frameCounter > 6.0d) {
                    frameCounter = 0.0d;
                    frame++;
                    if (frame >= 6) {
                        frame = 0;
                    }
                }
            }
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor) {
        Vector2 lanternPosition = Projectile.Center;
        if (Projectile.isAPreviewDummy) {
            lanternPosition += new Vector2(-24f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 3f, -4f);
            Projectile.spriteDirection = -1;
        }

        Projectile.frame = this.frame;
        var lantern = AequusTextures.LanternCatPet_Lantern.Value;
        lanternPosition += (new Vector2(24f * -Projectile.spriteDirection, -24f + MathF.Sin(Main.GlobalTimeWrappedHourly * 2f) * 8f) + new Vector2(Projectile.velocity.X * 2f, Projectile.velocity.Y * -0.1f - Math.Abs(Projectile.velocity.X) * 0.5f)) * Projectile.scale;
        float lanternRotation = Projectile.velocity.X * 0.1f * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f, 0.5f, 1f);


        var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out _);
        Main.EntitySpriteDraw(
            texture,
            Projectile.position + offset + new Vector2(0f, DrawOriginOffsetY + Projectile.gfxOffY) - Main.screenPosition,
            frame,
            lightColor,
            Projectile.rotation,
            origin,
            Projectile.scale,
            spriteEffects,
            0
        );

        Main.EntitySpriteDraw(
            lantern,
            lanternPosition - Main.screenPosition,
            null,
            Color.White,
            lanternRotation,
            lantern.Size() / 2f,
            Projectile.scale,
            spriteEffects,
            0
        );

        Main.EntitySpriteDraw(
            AequusTextures.Bloom,
            lanternPosition - Main.screenPosition,
            null,
            Color.Orange with { A = 0 } * 0.2f,
            0f,
            AequusTextures.Bloom.Size() / 2f,
            Projectile.scale * 0.8f,
            spriteEffects,
            0
        );
        return false;
    }

    internal override InstancedPetBuff CreatePetBuff() {
        return new(this, (p) => ref p.GetModPlayer<AequusPlayer>().petLanternLuckCat, lightPet: true);
    }

    protected override void OnLoad() {
        DedicationRegistry.Register(PetItem, new DefaultDedication("Linnn", new Color(60, 60, 120)));
    }
}