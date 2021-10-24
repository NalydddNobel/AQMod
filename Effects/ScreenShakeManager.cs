using AQMod.Effects.ScreenEffects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects
{
    public sealed class ScreenShakeManager
    {
        private static List<ScreenShakeFX> _screenShakes;
        private static Dictionary<string, ScreenShakeFX> _channeledShakes;

        internal static void Load()
        {
            _screenShakes = new List<ScreenShakeFX>();
            _channeledShakes = new Dictionary<string, ScreenShakeFX>();
        }

        internal static void Unload()
        {
            _channeledShakes = null;
            _screenShakes = null;
        }

        public static void AddEffect(ScreenShakeFX effect)
        {
            effect.Setup();
            _screenShakes.Add(effect);
        }

        public static void ChannelEffect(string name, ScreenShakeFX effect)
        {
            if (_channeledShakes.ContainsKey(name))
            {
                _channeledShakes[name].AdoptChannel(effect);
            }
            else
            {
                effect.Setup();
                _channeledShakes.Add(name, effect);
            }
        }

        internal static void Update()
        {
            for (int i = 0; i < _screenShakes.Count; i++)
            {
                ScreenShakeFX e = _screenShakes[i];
                if (!e.Update())
                {
                    _screenShakes.RemoveAt(i);
                    i--;
                }
                else if (e.UpdateBiomeVisuals)
                {
                    e.Apply();
                }
            }
            var removeKeys = new List<string>();
            foreach (var pair in _channeledShakes)
            {
                var e = pair.Value;
                if (!e.Update())
                    removeKeys.Add(pair.Key);
                else if (e.UpdateBiomeVisuals)
                    e.Apply();
            }
            foreach (var s in removeKeys)
            {
                _channeledShakes.Remove(s);
            }
        }

        internal static void ModifyScreenPosition()
        {
            foreach (var e in _screenShakes)
            {
                if (!e.UpdateBiomeVisuals)
                    e.Apply();
            }
            foreach (var pair in _channeledShakes)
            {
                var e = pair.Value;
                if (!e.UpdateBiomeVisuals)
                    e.Apply();
            }
        }

        public static float ScreenShakeYMagnitude { get; set; } = 0f;
        public static float ScreenShakeYLerp { get; set; } = 0.1f;
        public static int ScreenShakeYOffsetTime = 6;

        public static Vector2 UpsideDownScreenSupport(Vector2 position)
        {
            return Main.player[Main.myPlayer].gravDir == -1 ? new Vector2(position.X, -position.Y + Main.screenHeight) : new Vector2(position.X, position.Y);
        }
    }
}