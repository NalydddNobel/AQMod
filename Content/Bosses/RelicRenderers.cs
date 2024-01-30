using Aequus.Core.Assets;
using System;

namespace Aequus.Content.Bosses;

public interface IRelicRenderer {
    void DrawRelic(Int32 x, Int32 y, Vector2 drawCoordinates, Color drawColor, SpriteEffects spriteEffects, Single glow);

    public static void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 drawPos, Rectangle frame, Color color, Vector2 origin, SpriteEffects effects, Single glow) {
        drawPos /= 4f;
        drawPos.Floor();
        drawPos *= 4f;

        spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

        Single scale = (Single)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi / 2f) * 0.3f + 0.7f;
        var effectColor = color with { A = 0 } * 0.1f * scale;
        for (Single theta = 0f; theta < 1f; theta += 355f / (678f * MathHelper.Pi)) {
            spriteBatch.Draw(texture, drawPos + (MathHelper.TwoPi * theta).ToRotationVector2() * (6f + glow * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }
}

public record class BasicRelicRenderer(RequestCache<Texture2D> Texture) : IRelicRenderer {
    public void DrawRelic(Int32 x, Int32 y, Vector2 drawCoordinates, Color drawColor, SpriteEffects spriteEffects, Single glow) {
        var frame = Texture.Bounds();
        IRelicRenderer.Draw(Main.spriteBatch, Texture.Value, drawCoordinates, frame, drawColor, frame.Size() / 2f, spriteEffects, glow);
    }
}

public record class OmegaStariteRelicRenderer(RequestCache<Texture2D> Texture, Int32 FrameCount) : IRelicRenderer {
    public void DrawRelic(Int32 x, Int32 y, Vector2 drawCoordinates, Color drawColor, SpriteEffects spriteEffects, Single glow) {
        var tile = Main.tile[x, y];
        var baseFrame = new Rectangle(tile.TileFrameX, 0, 48, 48);

        var texture = Texture.Value;
        var baseOrbFrame = new Rectangle(baseFrame.X, baseFrame.Y + baseFrame.Height + 2, 18, 18);
        var orbFrame = baseOrbFrame;
        var orbOrigin = orbFrame.Size() / 2f;
        Single f = Main.GlobalTimeWrappedHourly % (MathHelper.TwoPi / 5f) - MathHelper.PiOver2;
        Int32 k = 0;
        for (; f <= MathHelper.Pi - MathHelper.PiOver2; f += MathHelper.TwoPi / 5f) {
            Single wave = (Single)Math.Sin(f);
            Single z = (Single)Math.Sin(f + MathHelper.PiOver2);
            orbFrame.Y = baseOrbFrame.Y + (Int32)MathHelper.Clamp(2 + z * 2.5f, 0f, FrameCount) * orbFrame.Height;
            k++;
            IRelicRenderer.Draw(Main.spriteBatch, texture, drawCoordinates + new Vector2(wave * baseFrame.Width / 2f, wave * orbFrame.Height * 0.4f - 8f), orbFrame, drawColor, orbOrigin, spriteEffects, glow);
        }
        IRelicRenderer.Draw(Main.spriteBatch, texture, drawCoordinates, baseFrame, drawColor, baseFrame.Size() / 2f, spriteEffects, glow);
        for (; k < 5; f += MathHelper.TwoPi / 5f) {
            Single wave = (Single)Math.Sin(f);
            Single z = (Single)Math.Sin(f + MathHelper.PiOver2);
            orbFrame.Y = baseOrbFrame.Y + (Int32)MathHelper.Clamp(2 + z * 2.5f, 0f, 5f) * orbFrame.Height;
            k++;
            IRelicRenderer.Draw(Main.spriteBatch, texture, drawCoordinates + new Vector2(wave * baseFrame.Width / 2f, wave * orbFrame.Height * 0.4f - 8f), orbFrame, drawColor, orbOrigin, spriteEffects, glow);
        }

    }
}