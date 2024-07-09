using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;

namespace AequusRemake.Systems.Items;

public class RandomItemFrames : GlobalItem {
    public static readonly Dictionary<int, byte> FrameCount = [];

    public byte frame = 0;
    public int RealFrame => Math.Max(frame - 1, 0);

    public override bool InstancePerEntity => true;
    protected override bool CloneNewInstances => true;

    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return FrameCount.ContainsKey(entity.type);
    }

    public override void SetDefaults(Item entity) {
        frame = 0;
    }

    public override void SetStaticDefaults() {
        foreach (KeyValuePair<int, byte> pair in FrameCount) {
            Main.RegisterItemAnimation(pair.Key, new RandomFrameOffset(pair.Value));
        }
    }

    public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        if (frame == 0) {
            if (FrameCount.TryGetValue(item.type, out var randomFrame)) {
                frame = (byte)(Main.rand.Next(randomFrame) + 1);
            }
        }

        Main.itemFrameCounter[whoAmI] = RealFrame;
        return true;
    }

    public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (!FrameCount.TryGetValue(item.type, out byte frameCount)) {
            return true;
        }

        Texture2D texture = ItemTexture[item.type].Value;
        frame = texture.Frame(frameY: RealFrame, verticalFrames: frameCount);
        spriteBatch.Draw(texture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
        return false;
    }

    public override void NetSend(Item item, BinaryWriter writer) {
        writer.Write(frame);
    }

    public override void NetReceive(Item item, BinaryReader reader) {
        frame = reader.ReadByte();
    }

    private class RandomFrameOffset : DrawAnimation {
        public RandomFrameOffset(int frameCount) {
            FrameCount = frameCount;
        }

        public override void Update() { }

        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1) {
            return texture.Frame(verticalFrames: FrameCount, frameY: Math.Clamp(frameCounterOverride, 0, FrameCount), sizeOffsetY: -2);
        }
    }
}
