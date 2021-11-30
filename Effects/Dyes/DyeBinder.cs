using AQMod.Items.Vanities.Dyes;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.Dyes
{
    internal static class DyeBinder
    {
        private static List<DyeItem> _loadDyes;

        public static void AddToDyeLoad(DyeItem item)
        {
            if (_loadDyes == null)
                _loadDyes = new List<DyeItem>();
            _loadDyes.Add(item);
        }

        public static void LoadDyes()
        {
            if (_loadDyes == null)
                return;
            AQMod.Debug.DebugLogger? logger = null;
            if (AQMod.Debug.LogDyeBinding)
                logger = AQMod.Debug.GetDebugLogger();
            foreach (var dye in _loadDyes)
            {
                setupDye(dye, logger);
            }
            _loadDyes = null;
        }

        public static void Unload() // incase the player decides to cancel loading midway
        {
            _loadDyes = null;
        }

        private static void setupDye(DyeItem dye, AQMod.Debug.DebugLogger? debugLogger)
        {
            if (debugLogger != null)
                debugLogger.Value.Log("Binding shader to " + dye.Name + " {Pass:" + dye.Pass + "}");
            GameShaders.Armor.BindShader(dye.item.type, dye.CreateShaderData);
        }
    }
}