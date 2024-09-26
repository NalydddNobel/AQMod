namespace Aequus.Common.Utilities;

public sealed class SpriteBatchCache {
    public BlendState? BlendState { get; private set; }
    public DepthStencilState? DepthStencilState { get; private set; }
    public RasterizerState? RasterizerState { get; private set; }
    public SamplerState? SamplerState { get; private set; }

    public void Begin(SpriteBatch spriteBatch, SpriteSortMode sortMode, Effect? customEffect = null, Matrix? transform = null) {
        spriteBatch.Begin(sortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, customEffect, transform ?? Matrix.Identity);
    }

    /// <summary>
    /// Note: This calls <see cref="SpriteBatch.End"/>, and then grabs all of the graphics device info and caches it into the various properties.
    /// Use <see cref="Begin(SpriteBatch, SpriteSortMode, Effect, Matrix?)"/> to restart <paramref name="spriteBatch"/> back to its original state, with its <see cref="SpriteSortMode"/> needing to be respecified.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <exception cref="System.InvalidOperationException"></exception>
    public void InheritFrom(SpriteBatch spriteBatch) {
        spriteBatch.End();

        GraphicsDevice g = spriteBatch.GraphicsDevice;

        BlendState = g.BlendState;
        SamplerState = g.SamplerStates[0];
        DepthStencilState = g.DepthStencilState;
        RasterizerState = g.RasterizerState;
    }
}
