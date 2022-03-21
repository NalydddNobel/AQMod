using AQMod.Common.Utilities.Debugging;
using AQMod.Items.Dyes;
using AQMod.Items.Dyes.Hair;
using System.Collections.Generic;
using Terraria.Graphics.Shaders;

namespace AQMod.Effects.Dyes
{
    internal static class AutoDyeBinder
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

        public static void SetupDyes()
        {
            DebugUtilities.Logger? logger = null;
            if (_loadDyes != null)
            {
                if (DebugUtilities.LogDyeBinding)
                    logger = DebugUtilities.GetDebugLogger();
                foreach (var dye in _loadDyes)
                {
                    setupDye(dye, logger);
                }
                _loadDyes = null;
            }
            if (_loadHairDyes != null)
            {
                if (logger != null && DebugUtilities.LogDyeBinding)
                    logger = DebugUtilities.GetDebugLogger();
                foreach (var dye in _loadHairDyes)
                {
                    setupDye(dye, logger);
                }
                _loadHairDyes = null;
            }
        }

        public static void Unload() // incase of cancelled loading mid-load
        {
            _loadDyes?.Clear();
            _loadDyes = null;
            _loadHairDyes?.Clear();
            _loadHairDyes = null;
        }

        private static void setupDye(HairDyeItem dye, DebugUtilities.Logger? debugLogger)
        {
            if (debugLogger != null)
                debugLogger.Value.Log("Binding hair dye to " + dye.Name);
            GameShaders.Hair.BindShader(dye.item.type, dye.CreateShaderData());
        }

        private static void setupDye(DyeItem dye, DebugUtilities.Logger? debugLogger)
        {
            if (debugLogger != null)
                debugLogger.Value.Log("Binding shader to " + dye.Name + " {Pass:" + dye.Pass + "}");
            GameShaders.Armor.BindShader(dye.item.type, dye.CreateShaderData());
        }
    }
}