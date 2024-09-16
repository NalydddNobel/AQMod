using Aequus.Common.Utilities.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace Aequus.Content.Monsters.PollutedOcean.Scavenger;

public partial class Scavenger {
    public static readonly HashSet<int> HeadSkinOverride = [ArmorIDs.Head.SilverHelmet, ArmorIDs.Head.TungstenHelmet];
    public static readonly HashSet<int> BackArmSkinOverride = [ArmorIDs.Body.SilverChainmail, ArmorIDs.Body.TungstenChainmail, ArmorIDs.Body.GoldChainmail, ArmorIDs.Body.PlatinumChainmail];

    private record struct DrawInfo(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Rectangle bodyFrame, int frameWidth, int frameHeight, float Opacity, SpriteEffects ArmorSpriteEffects);

    public override void FindFrame(int frameHeight) {
        base.FindFrame(frameHeight);
    }

    private void DrawHelmet(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Texture2D helmetTexture, Rectangle bodyFrame, SpriteEffects ArmorSpriteEffects) {
        spriteBatch.Draw(helmetTexture, drawCoordinates, bodyFrame, drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
    }

    private void DrawBody(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Texture2D bodyTexture, Rectangle bodyFrame, int frameWidth, int frameHeight, SpriteEffects ArmorSpriteEffects, bool drawBackArm = true) {
        var bodyOffset = Main.OffsetsPlayerHeadgear[bodyFrame.Y / frameHeight] + new Vector2(0f, -2f);
        spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new Rectangle(frameWidth * 3, frameHeight * 3, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);

        spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new Rectangle(0, 0, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
        DrawFrontArm(spriteBatch, drawCoordinates, drawColor, bodyTexture, bodyFrame, frameWidth, frameHeight, ArmorSpriteEffects, layerFront: false);
        spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new Rectangle(0, frameHeight, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
    }

    private void DrawFrontArm(SpriteBatch spriteBatch, Vector2 drawCoordinates, Color drawColor, Texture2D bodyTexture, Rectangle bodyFrame, int frameWidth, int frameHeight, SpriteEffects ArmorSpriteEffects, bool layerFront = false) {
        var bodyOffset = Main.OffsetsPlayerHeadgear[bodyFrame.Y / frameHeight] + new Vector2(0f, -2f);
        if (!layerFront) {
            spriteBatch.Draw(bodyTexture, drawCoordinates + bodyOffset, new Rectangle(frameWidth * 3, frameHeight, frameWidth, frameHeight), drawColor, NPC.rotation, bodyFrame.Size() / 2f, NPC.scale, ArmorSpriteEffects, 0f);
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

        NPCTools.ManuallyDrawNPCStatusEffects(spriteBatch, NPC, screenPos);

        DrawHelmet(spriteBatch, drawCoordinates, drawColor, AequusTextures.ScavengerBag_Back, bodyFrame, armorSpriteEffects);

        if (armor[Slot_Accs].balloonSlot > 0) {
            Main.instance.LoadAccBalloon(armor[Slot_Accs].balloonSlot);
            var balloonTexture = TextureAssets.AccBalloon[armor[Slot_Accs].balloonSlot].Value;
            if (!ArmorIDs.Balloon.Sets.UsesTorsoFraming[armor[Slot_Accs].balloonSlot]) {
                var balloonFrame = balloonTexture.Frame(verticalFrames: 4, frameY: DateTime.Now.Millisecond % 800 / 200);
                spriteBatch.Draw(balloonTexture, drawCoordinates + new Vector2((NPC.width / 2f - 6f) * NPC.spriteDirection, -8f) * NPC.scale, balloonFrame, drawColor, 0f, balloonFrame.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
            }
            else {
                DrawHelmet(spriteBatch, drawCoordinates, drawColor, balloonTexture, bodyFrame, armorSpriteEffects);
            }
        }

        if (armor[Slot_Legs].legSlot <= 0 || !ArmorIDs.Legs.Sets.HidesTopSkin[armor[Slot_Legs].legSlot]) {
            var legsFrame = bodyFrame;
            if ((armor[Slot_Accs].shoeSlot > 0 && ArmorIDs.Shoe.Sets.OverridesLegs[armor[Slot_Accs].shoeSlot]) || (armor[Slot_Legs].legSlot > 0 && ArmorIDs.Legs.Sets.HidesBottomSkin[armor[Slot_Legs].legSlot])) {
                legsFrame.Height -= 14;
            }
            spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawCoordinates, legsFrame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
        }

        var drawInfo = new DrawInfo(spriteBatch, drawCoordinates, drawColor, bodyFrame, x, y, opacity, armorSpriteEffects);
        DrawBody(spriteBatch, drawCoordinates, drawColor, AequusTextures.ScavengerBody, bodyFrame, x, y, armorSpriteEffects, drawBackArm: armor[Slot_Body].IsAir || !BackArmSkinOverride.Contains(armor[Slot_Body].bodySlot));
        DrawBodyFull(Slot_Accs, (i) => i.handOffSlot, Main.instance.LoadAccHandsOff, TextureAssets.AccHandsOff, drawInfo);
        DrawBodyFull(Slot_Body, (i) => i.bodySlot, Main.instance.LoadArmorBody, TextureAssets.ArmorBodyComposite, drawInfo);
        if (!armor[Slot_Body].IsAir) {
            spriteBatch.Draw(AequusTextures.ScavengerBag_Strap.Value, drawCoordinates + new Vector2(positionOffset.X + NPC.spriteDirection * 3f, 4f + positionOffset.Y).RotatedBy(NPC.rotation) * NPC.scale, null, drawColor, NPC.rotation, AequusTextures.ScavengerBag_Strap.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
        }
        if (armor[Slot_Head].headSlot <= 0 || (!HeadSkinOverride.Contains(armor[Slot_Head].headSlot) && ArmorIDs.Head.Sets.DrawHead[armor[Slot_Head].headSlot])) {
            spriteBatch.Draw(AequusTextures.ScavengerHead.Value, drawCoordinates + new Vector2(positionOffset.X, -2f + positionOffset.Y).RotatedBy(NPC.rotation) * NPC.scale, null, drawColor, NPC.rotation, AequusTextures.ScavengerHead.Size() / 2f, NPC.scale, armorSpriteEffects, 0f);
        }
        DrawBodyFull(Slot_Accs, (i) => i.handOnSlot, Main.instance.LoadAccHandsOn, TextureAssets.AccHandsOn, drawInfo);
        DrawHelmetFull(Slot_Head, (i) => i.headSlot, Main.instance.LoadArmorHead, TextureAssets.ArmorHead, drawInfo with { drawCoordinates = drawInfo.drawCoordinates + new Vector2(2f * NPC.spriteDirection, 0f) });
        DrawFrontArm(spriteBatch, drawCoordinates, drawColor, AequusTextures.ScavengerBody, bodyFrame, x, y, armorSpriteEffects, layerFront: true);
        DrawFrontArmFull(Slot_Body, (i) => i.bodySlot, Main.instance.LoadArmorBody, TextureAssets.ArmorBodyComposite, drawInfo, layerFront: true);
        DrawFrontArmFull(Slot_Accs, (i) => i.handOnSlot, Main.instance.LoadAccHandsOn, TextureAssets.AccHandsOn, drawInfo, layerFront: true);

        bool hideShoes = armor[Slot_Legs].legSlot > 0 && ArmorIDs.Legs.Sets.OverridesLegs[armor[Slot_Legs].legSlot];
        if (hideShoes || armor[Slot_Accs].shoeSlot <= 0 || !ArmorIDs.Shoe.Sets.OverridesLegs[armor[Slot_Accs].shoeSlot]) {
            DrawHelmetFull(Slot_Legs, (i) => i.legSlot, Main.instance.LoadArmorLegs, TextureAssets.ArmorLeg, drawInfo);
        }
        if (!hideShoes) {
            DrawHelmetFull(Slot_Accs, (i) => i.shoeSlot, Main.instance.LoadAccShoes, TextureAssets.AccShoes, drawInfo);
        }
        DrawHelmetFull(Slot_Accs, (i) => i.shieldSlot, Main.instance.LoadAccShield, TextureAssets.AccShield, drawInfo);
        DrawHelmetFull(Slot_Accs, (i) => i.waistSlot, Main.instance.LoadAccWaist, TextureAssets.AccWaist, drawInfo);
        return false;
    }

    public override void HitEffect(NPC.HitInfo hit) {
        if (Main.netMode == NetmodeID.Server) {
            return;
        }

        var source = NPC.GetSource_FromThis();

        if (NPC.life <= 0) {
            for (int i = 0; i < 20; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, 2.5f * hit.HitDirection, -2.5f);
            }

            NPC.NewGore(AequusTextures.ScavengerGoreHead, NPC.position, NPC.velocity, Scale: NPC.scale);
            for (int i = 0; i < 2; i++) {
                Gore.NewGore(source, NPC.position + new Vector2(0f, 20f), NPC.velocity, 43, NPC.scale);
                Gore.NewGore(source, NPC.position + new Vector2(0f, 34f), NPC.velocity, 44, NPC.scale);
            }
        }
        else {
            for (int i = 0; i < hit.Damage / (double)NPC.lifeMax * 50f; i++) {
                Terraria.Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, hit.HitDirection, -1f);
            }
        }
    }
}
