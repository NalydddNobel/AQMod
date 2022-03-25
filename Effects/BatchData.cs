using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace AQMod.Effects
{
    public sealed class BatchData
    {
        private static FieldInfo spriteSortModeField;
        private static FieldInfo blendStateField;
        private static FieldInfo depthStencilStateField;
        private static FieldInfo rasterizerStateField;
        private static FieldInfo samplerStateField;
        private static FieldInfo customEffectField;
        private static FieldInfo transformMatrixField;

        public SpriteSortMode spriteSortMode;
        public BlendState blendState;
        public DepthStencilState depthStencilState;
        public RasterizerState rasterizerState;
        public SamplerState samplerState;
        public Effect customEffect;
        public Matrix transformMatrix;

        public BatchData(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState,
            RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            spriteSortMode = sortMode;
            this.blendState = blendState;
            this.samplerState = samplerState;
            this.depthStencilState = depthStencilState;
            this.rasterizerState = rasterizerState;
            customEffect = effect;
            this.transformMatrix = transformMatrix;
        }

        public BatchData(SpriteBatch spriteBatch)
        {
            FromSpriteBatch(spriteBatch);
        }

        private void FromSpriteBatch(SpriteBatch spriteBatch)
        {
            spriteSortMode = spriteSortModeField.GetValue<SpriteSortMode>(spriteBatch);
            blendState = blendStateField.GetValue<BlendState>(spriteBatch);
            samplerState = samplerStateField.GetValue<SamplerState>(spriteBatch);
            depthStencilState = depthStencilStateField.GetValue<DepthStencilState>(spriteBatch);
            rasterizerState = rasterizerStateField.GetValue<RasterizerState>(spriteBatch);
            customEffect = customEffectField.GetValue<Effect>(spriteBatch);
            transformMatrix = transformMatrixField.GetValue<Matrix>(spriteBatch);
        }

        public void Begin(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, customEffect, transformMatrix);
        }

        public void BeginCustom(SpriteBatch spriteBatch,
            NullableOption<SpriteSortMode> spriteSortMode = default(NullableOption<SpriteSortMode>),
            NullableOption<BlendState> blendState = default(NullableOption<BlendState>),
            NullableOption<SamplerState> samplerState = default(NullableOption<SamplerState>),
            NullableOption<DepthStencilState> depthStencilState = default(NullableOption<DepthStencilState>),
            NullableOption<RasterizerState> rasterizerState = default(NullableOption<RasterizerState>),
            NullableOption<Effect> customEffect = default(NullableOption<Effect>),
            NullableOption<Matrix> transformMatrix = default(NullableOption<Matrix>))
        {
            spriteBatch.Begin(spriteSortMode.Get(this.spriteSortMode), blendState.Get(this.blendState),
                samplerState.Get(this.samplerState), depthStencilState.Get(this.depthStencilState),
                rasterizerState.Get(this.rasterizerState), customEffect.Get(this.customEffect), transformMatrix.Get(this.transformMatrix));
        }

        internal static void Load()
        {
            var t = typeof(SpriteBatch);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            spriteSortModeField = t.GetField(nameof(spriteSortMode), flags);
            blendStateField = t.GetField(nameof(blendState), flags);
            samplerStateField = t.GetField(nameof(samplerState), flags);
            depthStencilStateField = t.GetField(nameof(depthStencilState), flags);
            rasterizerStateField = t.GetField(nameof(rasterizerState), flags);
            customEffectField = t.GetField(nameof(customEffect), flags);
            transformMatrixField = t.GetField(nameof(transformMatrix), flags);
        }

        internal static void Unload()
        {
            spriteSortModeField = null;
            blendStateField = null;
            samplerStateField = null;
            depthStencilStateField = null;
            rasterizerStateField = null;
            customEffectField = null;
            transformMatrixField = null;
        }
    }
}