using System.Collections.Generic;

namespace Aequus.Common.Utilities.Extensions;

public static class SpriteBatchExtensions {
    private record struct PreviousSpriteBatchState(BlendState BlendState, SamplerState SamplerState, DepthStencilState DepthStencilState, RasterizerState RasterizerState) {
        public PreviousSpriteBatchState(SpriteBatch sb) : this(sb.GraphicsDevice.BlendState, sb.GraphicsDevice.SamplerStates[0], sb.GraphicsDevice.DepthStencilState, sb.GraphicsDevice.RasterizerState) { }
    }

    private static readonly Stack<PreviousSpriteBatchState> _cache = new();

    public static void EndCached(this SpriteBatch sb) {
        _cache.Push(new PreviousSpriteBatchState(sb));
        sb.End();
    }

    public static void BeginCached(this SpriteBatch sb, SpriteSortMode sortMode, Matrix? matrix = null) {
        if (!_cache.TryPop(out PreviousSpriteBatchState prev)) {
            throw new System.Exception("SpriteBatch attempted to BeginCached without calling EndCached previously.");
        }

        sb.Begin(sortMode, prev.BlendState, prev.SamplerState, prev.DepthStencilState, prev.RasterizerState, null, matrix ?? Matrix.Identity);
    }
}
