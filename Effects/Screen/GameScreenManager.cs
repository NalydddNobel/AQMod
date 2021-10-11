using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;

namespace AQMod.Effects.Screen
{
    public sealed class GameScreenManager
    {
        private static List<ScreenFX> _screenEffects;
        private static Dictionary<string, ScreenFX> _channeledFX;

        internal static void Load()
        {
            _screenEffects = new List<ScreenFX>();
            _channeledFX = new Dictionary<string, ScreenFX>();
        }

        internal static void Unload()
        {
            _channeledFX = null;
            _screenEffects = null;
        }

        public static void AddEffect(ScreenFX effect)
        {
            effect.Setup();
            _screenEffects.Add(effect);
        }

        public static void ChannelEffect(string name, ScreenFX effect)
        {
            if (_channeledFX.ContainsKey(name))
            {
                _channeledFX[name].AdoptChannel(effect);
            }
            else
            {
                effect.Setup();
                _channeledFX.Add(name, effect);
            }
        }

        internal static void Update()
        {
            for (int i = 0; i < _screenEffects.Count; i++)
            {
                ScreenFX e = _screenEffects[i];
                if (!e.Update())
                {
                    _screenEffects.RemoveAt(i);
                    i--;
                }
                else if (e.UpdateBiomeVisuals)
                    e.Apply();
            }
            List<string> removeKeys = new List<string>();
            foreach (var pair in _channeledFX)
            {
                var e = pair.Value;
                if (!e.Update())
                {
                    removeKeys.Add(pair.Key);
                }
                else if (e.UpdateBiomeVisuals)
                    e.Apply();
            }
            foreach (var s in removeKeys)
            {
                _channeledFX.Remove(s);
            }
        }

        internal static void ModifyScreenPosition()
        {
            foreach (var e in _screenEffects)
            {
                if (!e.UpdateBiomeVisuals)
                    e.Apply();
            }
            foreach (var pair in _channeledFX)
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