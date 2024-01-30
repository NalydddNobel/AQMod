using System;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DedicatedContent;

public class DedicatedFaeling : ModNPC {
    public Int32 dedicatedItemId;

    public override String Texture => AequusTextures.NPC(NPCID.Shimmerfly);

    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new() {
            Hide = true,
        };
        NPCID.Sets.CountsAsCritter[Type] = true;
        NPCID.Sets.TrailingMode[Type] = NPCID.Sets.TrailingMode[NPCID.Shimmerfly];
        NPCID.Sets.TrailCacheLength[Type] = NPCID.Sets.TrailCacheLength[NPCID.Shimmerfly];
    }

    public override void SetDefaults() {
        NPC.CloneDefaults(NPCID.Shimmerfly);
        NPC.dontTakeDamage = true;
        AIType = NPCID.Shimmerfly;
        AnimationType = NPCID.Shimmerfly;
        NPC.catchItem = ModContent.ItemType<DedicatedFaelingItem>();
    }

    public override void OnSpawn(IEntitySource source) {
        if (source is EntitySource_Parent parentSource) {
            Item item = null;
            if (parentSource.Entity is Player player) {
                item = player.HeldItemFixed();
            }
            else if (parentSource.Entity is Item sourceItem) {
                item = sourceItem;
            }
            if (item != null) {
                if (item.ModItem is DedicatedFaelingItem dedicatedFaelingItem) {
                    dedicatedItemId = item.type;
                }
            }
        }
    }

    public override void AI() {
        if (ItemLoader.GetItem(dedicatedItemId) is not IDedicatedItem) {
            dedicatedItemId = IDedicatedItem.GetRandomItemId();
            NPC.netUpdate = true;
        }
    }

    public override void OnKill() {
    }

    // Too lazy to transcribe every local variable
    public override Boolean PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        if (ItemLoader.GetItem(dedicatedItemId) is not IDedicatedItem dedicatedItem) {
            return false;
        }

        Texture2D npcTexture = TextureAssets.Npc[Type].Value;
        Vector2 drawCoordinates = NPC.Center - screenPos;
        Int32 verticalFrames = 5;
        Int32 horizontalFrames = 4;
        Single hue = Main.rgbToHsl(dedicatedItem.FaelingColor).X;
        Color glowColor = Main.hslToRgb(hue, 1f, 0.65f);
        glowColor.A /= 2;
        var spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        Color white = Color.White;
        white.A /= 2;

        Single rotation = NPC.rotation;
        Rectangle rectangle = npcTexture.Frame(horizontalFrames, verticalFrames, frameY: NPC.frame.Y);
        Vector2 origin = rectangle.Size() / 2f;
        Single scale = NPC.scale;
        Rectangle bloomFrame = npcTexture.Frame(horizontalFrames, verticalFrames, frameX: 2);
        Color color2 = new Color(255, 255, 255, 0) * 1f;
        Int32 trailLength = NPC.oldPos.Length;
        Int32 num12 = trailLength - 1 - 5;
        Int32 num13 = 5;
        Int32 num14 = 3;
        Single num15 = 32f;
        Single num16 = 16f;
        Vector2 center = new Vector2(num15, num16);
        Single fromMax = center.Length();
        Single num17 = Utils.Remap(Vector2.Distance(NPC.oldPos[num12], NPC.position), 0f, fromMax, 0f, 100f);
        num17 = (Int32)num17 / 5;
        num17 *= 5f;
        num17 /= 100f;
        num16 *= num17;
        num15 *= num17;
        Single num2 = 9f;
        Single num3 = 0.5f;
        Single num4 = MathHelper.Pi;
        for (Int32 num5 = num12; num5 >= num13; num5 -= num14) {
            Vector2 vector2 = NPC.oldPos[num5] - NPC.position;
            Single num6 = Utils.Remap(num5, 0f, trailLength, 1f, 0f);
            Single num7 = 1f - num6;
            Vector2 spinningpoint = new Vector2((Single)Math.Sin((Double)(NPC.whoAmI / 17f) + Main.timeForVisualEffects / (Double)num2 + (Double)(num6 * 2f * ((Single)Math.PI * 2f))) * num16, 0f - num15) * num7;
            Vector2 val = vector2;
            Double radians = num4;
            center = default(Vector2);
            vector2 = val + spinningpoint.RotatedBy(radians, center);
            Color color3 = Main.hslToRgb((hue + num7 * num3) % 1f, 1f, 0.5f) with { A = 0 };
            spriteBatch.Draw(npcTexture, drawCoordinates + vector2, bloomFrame, color3 * num6 * 0.16f, rotation, origin, scale * Utils.Remap(num6 * num6, 0f, 1f, 0f, 2.5f), spriteEffects, 0f);
        }

        spriteBatch.Draw(npcTexture, drawCoordinates, bloomFrame, color2, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(horizontalFrames, verticalFrames, frameX: 1, frameY: NPC.frame.Y), white, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, rectangle, glowColor, rotation, origin, scale, spriteEffects, 0f);
        Single colorMultiplier = MathHelper.Clamp((Single)Math.Sin(Main.timeForVisualEffects / 60.0) * 0.3f + 0.3f, 0f, 1f);
        Single flashScale = 0.8f + (Single)Math.Sin(Main.timeForVisualEffects / 15.0 * MathHelper.TwoPi) * 0.3f;
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(horizontalFrames, verticalFrames, 3, NPC.whoAmI % verticalFrames), Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.5f) * colorMultiplier, rotation, origin, scale * flashScale, 0, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(horizontalFrames, verticalFrames, 3, 1), Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.5f) * colorMultiplier, rotation, origin, scale * flashScale, 0, 0f);
        return false;
    }

    public override void SaveData(TagCompound tag) {
        if (ItemLoader.GetItem(dedicatedItemId) is not IDedicatedItem dedicatedItem) {
            return;
        }
        tag["DedicatedName"] = dedicatedItem.DedicateeName;
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("DedicatedName", out String name)) {
            dedicatedItemId = IDedicatedItem.GetItemIdFromName(name);
        }
    }

    public override void SendExtraAI(BinaryWriter writer) {
        writer.Write(dedicatedItemId);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        dedicatedItemId = reader.ReadInt32();
    }
}
