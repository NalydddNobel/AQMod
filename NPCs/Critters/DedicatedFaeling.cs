using Aequus.Common.DataSets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs.Critters;

public class DedicatedFaeling : ModNPC {
    public string dedicatedName;
    public DedicatedContentInfo dedicatedContentInfo;

    public override string Texture => Aequus.NPCTexture(NPCID.Shimmerfly);

    public override void SetStaticDefaults() {
        NPCID.Sets.NPCBestiaryDrawOffset[Type] = new(0) {
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
                    dedicatedName = dedicatedFaelingItem.dedicatedName;
                }
                else if (ItemSets.DedicatedContent.TryGetValue(item.type, out var dedicatedContentInfo)) {
                    dedicatedName = dedicatedContentInfo.DedicateeName;
                }
            }
        }
    }

    public override void AI() {
        if (string.IsNullOrEmpty(dedicatedName)) {
            dedicatedName = DedicatedContentInfo.Random().DedicateeName;
        }
        if (dedicatedContentInfo.IsInvalid) {
            if (DedicatedContentInfo.TryFromName(dedicatedName, out var info)) {
                dedicatedContentInfo = info;
            }
            else {
                dedicatedContentInfo = DedicatedContentInfo.Random();
            }
            NPC.netUpdate = true;
        }
    }

    public override void OnKill() {

    }

    // Too lazy to transcribe every local variable
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
        Texture2D npcTexture = TextureAssets.Npc[Type].Value;
        Vector2 drawCoordinates = NPC.Center - screenPos;
        int verticalFrames = 5;
        int horizontalFrames = 4;
        float hue = dedicatedContentInfo.IsInvalid ? 0f : Main.rgbToHsl(dedicatedContentInfo.GetFaelingColor()).X;
        Color glowColor = Main.hslToRgb(hue, 1f, 0.65f);
        glowColor.A /= 2;
        var spriteEffects = NPC.spriteDirection.ToSpriteEffect();
        Color white = Color.White;
        white.A /= 2;

        float rotation = NPC.rotation;
        Rectangle rectangle = npcTexture.Frame(horizontalFrames, verticalFrames, frameY: NPC.frame.Y);
        Vector2 origin = rectangle.Size() / 2f;
        float scale = NPC.scale;
        Rectangle bloomFrame = npcTexture.Frame(horizontalFrames, verticalFrames, frameX: 2);
        Color color2 = new Color(255, 255, 255, 0) * 1f;
        int trailLength = NPC.oldPos.Length;
        int num12 = trailLength - 1 - 5;
        int num13 = 5;
        int num14 = 3;
        float num15 = 32f;
        float num16 = 16f;
        Vector2 center = new Vector2(num15, num16);
        float fromMax = center.Length();
        float num17 = Utils.Remap(Vector2.Distance(NPC.oldPos[num12], NPC.position), 0f, fromMax, 0f, 100f);
        num17 = (int)num17 / 5;
        num17 *= 5f;
        num17 /= 100f;
        num16 *= num17;
        num15 *= num17;
        float num2 = 9f;
        float num3 = 0.5f;
        float num4 = MathHelper.Pi;
        for (int num5 = num12; num5 >= num13; num5 -= num14) {
            Vector2 vector2 = NPC.oldPos[num5] - NPC.position;
            float num6 = Utils.Remap(num5, 0f, trailLength, 1f, 0f);
            float num7 = 1f - num6;
            Vector2 spinningpoint = new Vector2((float)Math.Sin((double)((float)NPC.whoAmI / 17f) + Main.timeForVisualEffects / (double)num2 + (double)(num6 * 2f * ((float)Math.PI * 2f))) * num16, 0f - num15) * num7;
            Vector2 val = vector2;
            double radians = num4;
            center = default(Vector2);
            vector2 = val + spinningpoint.RotatedBy(radians, center);
            Color color3 = Main.hslToRgb((hue + num7 * num3) % 1f, 1f, 0.5f) with { A = 0 };
            spriteBatch.Draw(npcTexture, drawCoordinates + vector2, bloomFrame, color3 * num6 * 0.16f, rotation, origin, scale * Utils.Remap(num6 * num6, 0f, 1f, 0f, 2.5f), spriteEffects, 0f);
        }

        spriteBatch.Draw(npcTexture, drawCoordinates, bloomFrame, color2, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(horizontalFrames, verticalFrames, frameX: 1, frameY: NPC.frame.Y), white, rotation, origin, scale, spriteEffects, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, rectangle, glowColor, rotation, origin, scale, spriteEffects, 0f);
        float colorMultiplier = MathHelper.Clamp((float)Math.Sin(Main.timeForVisualEffects / 60.0) * 0.3f + 0.3f, 0f, 1f);
        float flashScale = 0.8f + (float)Math.Sin(Main.timeForVisualEffects / 15.0 * MathHelper.TwoPi) * 0.3f;
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(horizontalFrames, verticalFrames, 3, NPC.whoAmI % verticalFrames), Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.5f) * colorMultiplier, rotation, origin, scale * flashScale, 0, 0f);
        spriteBatch.Draw(npcTexture, drawCoordinates, npcTexture.Frame(horizontalFrames, verticalFrames, 3, 1), Color.Lerp(glowColor, new Color(255, 255, 255, 0), 0.5f) * colorMultiplier, rotation, origin, scale * flashScale, 0, 0f);
        return false;
    }

    public override void SaveData(TagCompound tag) {
        tag["DedicatedName"] = dedicatedName;
    }

    public override void LoadData(TagCompound tag) {
        if (tag.TryGet("DedicatedName", out string dedicatedName)) {
            this.dedicatedName = dedicatedName;
        }
        dedicatedContentInfo = DedicatedContentInfo.FromName(dedicatedName);
    }

    public override void SendExtraAI(BinaryWriter writer) {
        if (string.IsNullOrEmpty(dedicatedName)) {
            dedicatedName = DedicatedContentInfo.Random().DedicateeName;
        }
        writer.Write(dedicatedName);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        dedicatedName = reader.ReadString();
        dedicatedContentInfo = DedicatedContentInfo.FromName(dedicatedName);
    }
}
