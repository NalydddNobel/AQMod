using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents.ProgressBars
{
    internal sealed class BasicEventProgressBar : EventProgressBar
    {
        private readonly Func<bool> _isActive;
        private readonly Func<float> _getProgress;
        private readonly Ref<Texture2D> _texture;
        private readonly string _key;
        private readonly Color _bgClr;

        public override Texture2D IconTexture => _texture.Value;
        public override string EventName => Language.GetTextValue(_key);
        public override Color NameBGColor => _bgClr;
        public override float EventProgress => _getProgress();

        public override bool IsActive()
        {
            return _isActive();
        }

        public BasicEventProgressBar(Func<bool> isActive, Func<float> getProgress, string texture, string name, Color bgColor)
        {
            _isActive = isActive;
            _getProgress = getProgress;
            _texture = new Ref<Texture2D>(ModContent.GetTexture(texture));
            _key = name;
            _bgClr = bgColor;
        }
    }
}