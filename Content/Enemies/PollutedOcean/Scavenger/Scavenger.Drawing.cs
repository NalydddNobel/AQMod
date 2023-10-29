using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class Scavenger {
    public static readonly HashSet<int> HeadOverride = new();

    private record struct DrawInfo(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Rectangle bodyFrame, int frameWidth, int frameHeight, float Opacity, SpriteEffects ArmorSpriteEffects);

    private void SetupDrawLookups() {
        HeadOverride.Add(ArmorIDs.Head.SilverHelmet);
        HeadOverride.Add(ArmorIDs.Head.TungstenHelmet);
    }

    private void DrawHelmet(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Texture2D helmetTexture, Rectangle bodyFrame, SpriteEffects ArmorSpriteEffects) {
        spriteBatch.Draw(helmetTexture, drawCoordinates, bodyFrame, drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
    }

    private void DrawBody(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Texture2D bodyTexture, Rectangle bodyFrame, int frameWidth, int frameHeight, SpriteEffects ArmorSpriteEffects) {
        var bodyOffset = Main.OffsetsPlayerHeadgear[bodyFrame.Y / frameHeight] + new Vector2(0f, -2f);
        spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new(frameWidth * 3, frameHeight * 3, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);

        spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new(0, 0, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
        DrawFrontArm(spriteBatch, drawCoordinates, drawColor, bodyTexture, bodyFrame, frameWidth, frameHeight, ArmorSpriteEffects, layerFront: false);
        spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new(0, frameHeight, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
    }

    private void DrawFrontArm(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Texture2D bodyTexture, Rectangle bodyFrame, int frameWidth, int frameHeight, SpriteEffects ArmorSpriteEffects, bool layerFront = false) {
        var bodyOffset = Main.OffsetsPlayerHeadgear[bodyFrame.Y / frameHeight] + new Vector2(0f, -2f);
        if (!layerFront) {
            spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new(frameWidth * 3, frameHeight, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
        }
    }

    private bool PrepareDraw(int item, Func<Item, int> getSlot, Action<int> load, out int slotId) {
        if (armor[item] == null) {
            slotId = 0;
            return false;
        }
        slotId = getSlot(armor[item]);
        if (slotId <= 0) {
            return false;
        }
        load(slotId);
        return true;
    }

    private void DrawBodyFull(int item, Func<Item, int> getSlot, Action<int> load, Asset<Texture2D>[] textureArr, DrawInfo drawInfo) {
        if (!PrepareDraw(item, getSlot, load, out int slotId)) {
            return;
        }
        DrawBody(drawInfo.spriteBatch, drawInfo.drawCoordinates, drawInfo.drawColor, textureArr[slotId].Value, drawInfo.bodyFrame, drawInfo.frameWidth, drawInfo.frameHeight, drawInfo.ArmorSpriteEffects);
    }

    private void DrawFrontArmFull(int item, Func<Item, int> getSlot, Action<int> load, Asset<Texture2D>[] textureArr, DrawInfo drawInfo, bool layerFront) {
        if (!PrepareDraw(item, getSlot, load, out int slotId)) {
            return;
        }
        DrawFrontArm(drawInfo.spriteBatch, drawInfo.drawCoordinates, drawInfo.drawColor, textureArr[slotId].Value, drawInfo.bodyFrame, drawInfo.frameWidth, drawInfo.frameHeight, drawInfo.ArmorSpriteEffects, layerFront);
    }

    private void DrawHelmetFull(int item, Func<Item, int> getSlot, Action<int> load, Asset<Texture2D>[] textureArr, DrawInfo drawInfo) {
        if (!PrepareDraw(item, getSlot, load, out int slotId)) {
            return;
        }
        DrawHelmet(drawInfo.spriteBatch, drawInfo.drawCoordinates, drawInfo.drawColor, textureArr[slotId].Value, drawInfo.bodyFrame, drawInfo.ArmorSpriteEffects);
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        drawColor = NPC.GetNPCColorTintedByBuffs(drawColor);
        int x = 40;
        int y = 56;
        var bodyFrame = (NPC.frame.Y / NPC.frame.Height) switch {
            0 => new Rectangle(0, y * 5, x, y),
            _ => new Rectangle(0, y * (NPC.frame.Y / NPC.frame.Height + 5), x, y)
        };
        var positionOffset = Main.OffsetsPlayerHeadgear[bodyFrame.Y / y];
        var drawCoordinates = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY - 4f);
        var armorSpriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        float opacity = NPC.Opacity * (1f - NPC.shimmerTransparency);

        DrawHelmet(spriteBatch, drawCoordinates, drawColor, AequusTextures.ScavengerBag_Back, bodyFrame, armorSpriteEffects);

        if (armor[AccSlot].balloonSlot > 0) {
            Main.instance.LoadAccBalloon(armor[AccSlot].balloonSlot);
            var balloonTexture = TextureAssets.AccBalloon[armor[AccSlot].balloonSlot].Value;
            if (!ArmorIDs.Balloon.Sets.UsesTorsoFraming[armor[AccSlot].balloonSlot]) {
                var balloonFrame = balloonTexture.Frame(verticalFrames: 4, frameY: DateTime.Now.Millisecond % 800 / 200);
                spriteBatch.Draw(balloonTexture, drawCoordinates + new Vector2((NPC.width / 2f - 6f) * NPC.spriteDirection, -8f) * NPC.scale, balloonFrame, drawColor, 0f, balloonFrame.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
            }
            else {
                DrawHelmet(spriteBatch, drawCoordinates, drawColor, balloonTexture, bodyFrame, armorSpriteEffects);
            }
        }

        if (armor[LegSlot].legSlot <= 0 || !ArmorIDs.Legs.Sets.HidesTopSkin[armor[LegSlot].legSlot]) {
            var legsFrame = bodyFrame;
            if ((armor[AccSlot].shoeSlot > 0 && ArmorIDs.Shoe.Sets.OverridesLegs[armor[AccSlot].shoeSlot]) || (armor[LegSlot].legSlot > 0 && ArmorIDs.Legs.Sets.HidesBottomSkin[armor[LegSlot].legSlot])) {
                legsFrame.Height -= 14;
            }
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates, legsFrame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
        }

        var drawInfo = new DrawInfo(spriteBatch, drawCoordinates, drawColor, bodyFrame, x, y, opacity, armorSpriteEffects);
        DrawBody(spriteBatch, drawCoordinates, drawColor, AequusTextures.ScavengerBody, bodyFrame, x, y, armorSpriteEffects);
        DrawBodyFull(AccSlot, (i) => i.handOffSlot, Main.instance.LoadAccHandsOff, TextureAssets.AccHandsOff, drawInfo);
        DrawBodyFull(BodySlot, (i) => i.bodySlot, Main.instance.LoadArmorBody, TextureAssets.ArmorBodyComposite, drawInfo);
        if (!armor[BodySlot].IsAir) {
            spriteBatch.Draw(AequusTextures.ScavengerBag_Strap.Value, drawCoordinates + new Vector2(positionOffset.X + NPC.spriteDirection * 3f, 4f + positionOffset.Y).RotatedBy(NPC.rotation) * NPC.scale, null, drawColor, NPC.rotation, AequusTextures.ScavengerBag_Strap.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
        }
        if (armor[HeadSlot].headSlot <= 0 || (!HeadOverride.Contains(armor[HeadSlot].headSlot) && ArmorIDs.Head.Sets.DrawHead[armor[HeadSlot].headSlot])) {
            spriteBatch.Draw(AequusTextures.ScavengerHead.Value, drawCoordinates + new Vector2(positionOffset.X, -2f + positionOffset.Y).RotatedBy(NPC.rotation) * NPC.scale, null, drawColor, NPC.rotation, AequusTextures.ScavengerHead.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
        }
        DrawBodyFull(AccSlot, (i) => i.handOnSlot, Main.instance.LoadAccHandsOn, TextureAssets.AccHandsOn, drawInfo);
        DrawHelmetFull(HeadSlot, (i) => i.headSlot, Main.instance.LoadArmorHead, TextureAssets.ArmorHead, drawInfo with { drawCoordinates = drawInfo.drawCoordinates + new Vector2(2f * NPC.spriteDirection, 0f) });
        DrawFrontArm(spriteBatch, drawCoordinates, drawColor, AequusTextures.ScavengerBody, bodyFrame, x, y, armorSpriteEffects, layerFront: true);
        DrawFrontArmFull(BodySlot, (i) => i.bodySlot, Main.instance.LoadArmorBody, TextureAssets.ArmorBodyComposite, drawInfo, layerFront: true);
        DrawFrontArmFull(AccSlot, (i) => i.handOnSlot, Main.instance.LoadAccHandsOn, TextureAssets.AccHandsOn, drawInfo, layerFront: true);

        bool hideShoes = armor[LegSlot].legSlot > 0 && ArmorIDs.Legs.Sets.OverridesLegs[armor[LegSlot].legSlot];
        if (hideShoes || armor[AccSlot].shoeSlot <= 0 || !ArmorIDs.Shoe.Sets.OverridesLegs[armor[AccSlot].shoeSlot]) {
            DrawHelmetFull(LegSlot, (i) => i.legSlot, Main.instance.LoadArmorLegs, TextureAssets.ArmorLeg, drawInfo);
        }
        if (!hideShoes) {
            DrawHelmetFull(AccSlot, (i) => i.shoeSlot, Main.instance.LoadAccShoes, TextureAssets.AccShoes, drawInfo);
        }
        DrawHelmetFull(AccSlot, (i) => i.shieldSlot, Main.instance.LoadAccShield, TextureAssets.AccShield, drawInfo);
        DrawHelmetFull(AccSlot, (i) => i.waistSlot, Main.instance.LoadAccWaist, TextureAssets.AccWaist, drawInfo);
        return false;
    }
}