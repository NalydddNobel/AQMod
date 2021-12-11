using AQMod.Items.Vanities.Dyes;
using AQMod.Items.Vanities.HairDyes;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.Dyes
{
    internal static class DyeBinder
    {
        private static List<DyeItem> _loadDyes;
        private static List<HairDyeItem> _loadHairDyes;

        public static void AddDye(DyeItem item)
        {
            if (_loadDyes == null)
                _loadDyes = new List<DyeItem>();
            _loadDyes.Add(item);
        }

        public static void AddDye(HairDyeItem item)
        {
            if (_loadHairDyes == null)
                _loadHairDyes = new List<HairDyeItem>();
            _loadHairDyes.Add(item);
        }

        public static void LoadDyes()
        {
            AQMod.Debug.DebugLogger? logger = null;
            if (_loadDyes != null)
            {
                if (AQMod.Debug.LogDyeBinding)
                    logger = AQMod.Debug.GetDebugLogger();
                foreach (var dye in _loadDyes)
                {
                    setupDye(dye, logger);
                }
                _loadDyes = null;
            }
            if (_loadHairDyes != null)
            {
                if (logger != null && AQMod.Debug.LogDyeBinding)
                    logger = AQMod.Debug.GetDebugLogger();
                foreach (var dye in _loadHairDyes)
                {
                    setupDye(dye, logger);
                }
                _loadHairDyes = null;
            }
        }

        public static void Unload() // incase of cancelled loading mid-load
        {
            _loadDyes = null;
            _loadHairDyes = null;
        }

        private static void setupDye(HairDyeItem dye, AQMod.Debug.DebugLogger? debugLogger)
        {
            if (debugLogger != null)
                debugLogger.Value.Log("Binding hair dye to " + dye.Name);
            GameShaders.Hair.BindShader(dye.item.type, dye.CreateShaderData());
        }

        private static void setupDye(DyeItem dye, AQMod.Debug.DebugLogger? debugLogger)
        {
            if (debugLogger != null)
                debugLogger.Value.Log("Binding shader to " + dye.Name + " {Pass:" + dye.Pass + "}");
            GameShaders.Armor.BindShader(dye.item.type, dye.CreateShaderData());
        }
    }
}