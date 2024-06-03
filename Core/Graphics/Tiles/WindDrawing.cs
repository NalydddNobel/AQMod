using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.GameContent.Drawing;

namespace Aequus.Core.Graphics.Tiles;

public class WindDrawing : ModSystem {
    public override void Load() {
        IL_TileDrawing.DrawGrass += IL_TileDrawing_DrawGrass;
    }

    private void IL_TileDrawing_DrawGrass(ILContext il) {
        ILCursor cursor = new ILCursor(il);

        if (!cursor.TryGotoNext(i => i.MatchLdfld(typeof(Point), nameof(Point.X)))) {
            Mod.Logger.Error("Could not match load field for Point.X in TileDrawing.DrawGrass"); return;
        }

        // Step to the stloc instruction
        cursor.Index++;
        // Copy the operand, so we don't need to hard-code local variable indices
        object indexTileX = cursor.Instrs[cursor.Index].Operand;

        // Copy paste of the previous, this gets the local variable index for Y
        if (!cursor.TryGotoNext(i => i.MatchLdfld(typeof(Point), nameof(Point.Y)))) {
            Mod.Logger.Error("Could not match load field for Point.Y in TileDrawing.DrawGrass"); return;
        }

        cursor.Index++;
        object indexTileY = cursor.Instrs[cursor.Index].Operand;

        // Move down to the only draw call in the method
        // (Other than the glowmask draw call)
        if (!cursor.TryGotoNext(i => i.MatchCallvirt(typeof(SpriteBatch), nameof(SpriteBatch.Draw)))) {
            Mod.Logger.Error("Could not match call virt for SpriteBatch.Draw in TileDrawing.DrawGrass"); return;
        }

        // Index of the first draw call
        int endIndex = cursor.Index;

        ILLabel loopLabel = null;
        if (!cursor.TryGotoNext(i => i.MatchBrfalse(out loopLabel)) || loopLabel == null) {
            Mod.Logger.Error("Could not match brfalse after SpriteBatch.Draw in TileDrawing.DrawGrass"); return;
        }

        // Move backwards to when Main.spriteBatch is pushed onto the stack,
        // as we want to copy all of the parameters for drawing the sprite.
        if (!cursor.TryGotoPrev(i => i.MatchLdsfld(typeof(Main), nameof(Main.spriteBatch)))) {
            Mod.Logger.Error("Could not match load static field Main.spriteBatch in TileDrawing.DrawGrass"); return;
        }

        // Index of when Main.spriteBatch is pushed onto the stack for the first draw call.
        int startIndex = cursor.Index;

        // Copy all of the instructions for preparing the draw call.
        (OpCode OpCode, object Operand)[] instructionsToCopy = new (OpCode, object)[endIndex - startIndex];
        for (int i = 0; i < instructionsToCopy.Length; i++) {
            Instruction instructionToCopy = il.Instrs[i + startIndex];

            instructionsToCopy[i] = (instructionToCopy.OpCode, instructionToCopy.Operand);
        }

        // Push the tile's X and Y position
        cursor.Emit(OpCodes.Ldloc_S, indexTileX);
        cursor.Emit(OpCodes.Ldloc_S, indexTileY);

        // Emit all of the copied instructions
        for (int i = 0; i < instructionsToCopy.Length; i++) {
            cursor.Emit(instructionsToCopy[i].OpCode, instructionsToCopy[i].Operand);
        }

        // Emit a delegate which will uses all of the sprite drawing data.
        cursor.EmitDelegate((int X, int Y, SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth) => {
            if (TileLoader.GetTile(Main.tile[X, Y].TileType) is IDrawWindyGrass drawWindyGrass) {
                return drawWindyGrass.DrawWindyGrass(new TileDrawCall(X, Y, spriteBatch, texture, position, frame, color, rotation, origin, scale, effects, layerDepth));
            }

            // test
            //for (int i = 0; i < 4; i++) {
            //    spriteBatch.Draw(texture, position + (i * MathHelper.PiOver2).ToRotationVector2() * 2f, frame, Main.DiscoColor with { A = 0 }, rotation, origin, scale, effects, layerDepth);
            //}
            return true;
        });

        // Branches if the delegate emitted above returns false.
        cursor.Emit(OpCodes.Brfalse, loopLabel);

        //MonoModHooks.DumpIL(Mod, il);
    }
}

public record struct TileDrawCall(int X, int Y, SpriteBatch SpriteBatch, Texture2D Texture, Vector2 Position, Rectangle? Frame, Color Color, float Rotation, Vector2 Origin, float Scale, SpriteEffects SpriteEffects, float _layerDepth) {
    public void DrawSelf() {
        SpriteBatch.Draw(Texture, Position, Frame, Color, Rotation, Origin, Scale, SpriteEffects, _layerDepth);
    }
}

public interface IDrawWindyGrass {
    /// <param name="drawInfo"></param>
    /// <returns>Whether to run vanilla rendering. Return false to prevent vanilla rendering.</returns>
    bool DrawWindyGrass(TileDrawCall drawInfo);
}