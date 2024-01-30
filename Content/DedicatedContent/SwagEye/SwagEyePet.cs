﻿using Aequus.Content.Pets;
using System;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Aequus.Content.DedicatedContent.SwagEye;

public class SwagEyePet : ModPet {
    public override void SetStaticDefaults() {
        Main.projFrames[Type] = 2;
        Main.projPet[Projectile.type] = true;
        ProjectileID.Sets.CharacterPreviewAnimations[Type] = new() { Offset = new(0f, -8f) };

        ItemID.Sets.ShimmerTransformToItem[ItemID.SuspiciousLookingEye] = PetItem.Type;
    }

    public override void SetDefaults() {
        Projectile.CloneDefaults(ProjectileID.BabyEater);
        AIType = ProjectileID.BabyEater;
        Projectile.width = 32;
        Projectile.height = 32;
    }

    public override Boolean PreAI() {
        base.PreAI();
        Projectile.localAI[0]--;
        if (Main.netMode != NetmodeID.Server) {
            for (Int32 i = 0; i < Main.maxPlayers; i++) {
                if (Main.player[i].chatOverhead.timeLeft > 0 && !String.IsNullOrEmpty(Main.player[i].chatOverhead.chatText) && Main.player[i].chatOverhead.chatText.ToLower().Contains("hi torra")) {
                    if (Main.player[i].chatOverhead.timeLeft == 300) {
                        String playerName = Main.player[i].name;
                        if (Main.myPlayer == i) {
                            playerName = Environment.UserName;
                        }
                        Main.NewText($"<Torra> OMG HI {playerName.ToUpper()}!!!!!!");
                        Projectile.localAI[0] = 400f;
                        Projectile.localAI[1] = i;
                    }
                }
            }
        }

        Main.player[Projectile.owner].eater = false;

        Projectile.ai[0]++;
        if ((Int32)Projectile.ai[0] % 2 == 0)
            return true;

        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
        return false;
    }

    public override void AI() {
        Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
    }

    public override Boolean PreDraw(ref Color lightColor) {
        var drawCoords = Projectile.Center;

        Main.instance.LoadProjectile(ProjectileID.Blizzard);
        var texture = TextureAssets.Projectile[ProjectileID.Blizzard].Value;
        Single icicleDistance = !Projectile.isAPreviewDummy ? 80f : 48f;
        for (Int32 i = 0; i < 6; i++) {
            var rotation = i + MathHelper.TwoPi / 6f + Main.GameUpdateCount * 0.05f;
            var frame = texture.Frame(verticalFrames: 5, frameY: (Projectile.identity + i) % 5);
            var icicleDrawCoords = drawCoords + rotation.ToRotationVector2() * icicleDistance * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 5f + i * MathHelper.Pi / 3f, 0.8f, 1f) * Projectile.scale;
            Main.EntitySpriteDraw(texture, icicleDrawCoords - Main.screenPosition, frame, ExtendLight.Get(icicleDrawCoords), rotation - MathHelper.PiOver2, new Vector2(frame.Width / 2f, frame.Height - 6), Projectile.scale, SpriteEffects.None, 0);
        }

        if (Projectile.isAPreviewDummy) {
            Projectile.rotation = -MathHelper.PiOver2;
        }
        else {
            if (Projectile.localAI[0] > 0f) {
                Int32 i = (Int32)Projectile.localAI[1];
                String playerName = Main.player[i].name;
                if (Main.myPlayer == i) {
                    playerName = Environment.UserName;
                }
                String text = $"OMG HI {playerName.ToUpper()}!!!!!!";
                var font = FontAssets.MouseText.Value;
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, Projectile.Center + new Vector2(0f, -Projectile.height) - Main.screenPosition,
                    new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, 255), 0f, new Vector2(font.MeasureString(text).X / 2f, 0f), Vector2.One * Main.UIScale);
            }
        }
        return true;
    }

    internal override InstancedPetBuff CreatePetBuff() {
        return new(this, (p) => ref p.GetModPlayer<AequusPlayer>().petSwagEye, lightPet: false);
    }

    internal override InstancedPetItem CreatePetItem() {
        return new DedicatedPetItem(this, "torra th", new Color(80, 60, 255), nameHidden: false);
    }
}