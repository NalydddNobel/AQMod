using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Effects {
    public class ScreenFliterEffect {
        public readonly string Path;
        public readonly string Name;
        public readonly string Pass;
        public readonly string Key;
        public readonly EffectPriority Priority;
        private ScreenShaderData _screenShaderData;

        internal ScreenFliterEffect(string path, string name, string pass, EffectPriority priority = EffectPriority.VeryLow) {
            Priority = priority;
            Pass = pass;
            Path = path;
            Name = name;
            Key = "Aequus:" + name;
        }

        internal ScreenShaderData Load() {
            Filters.Scene[Key] = new(new(new(ModContent.Request<Effect>(Path, AssetRequestMode.ImmediateLoad).Value), Pass), Priority);
            _screenShaderData = Filters.Scene[Key].GetShader();
            return _screenShaderData;
        }

        internal void Activate(Vector2 position = default, params object[] args) {
            if (Main.netMode == NetmodeID.Server || Filters.Scene[Key].IsActive()) {
                return;
            }

            Filters.Scene.Activate(Key, position, args);
        }

        internal void Deactivate(params object[] args) {
            if (Main.netMode == NetmodeID.Server || !Filters.Scene[Key].IsActive()) {
                return;
            }

            Filters.Scene.Deactivate(Key, args);
        }

        internal void Manage(bool isActive) {
            if (isActive) {
                Activate();
            }
            else {
                Deactivate();
            }
        }
    }
}