using Aequus.Biomes.DemonSiege;
using Aequus.Common.Utilities;
using Aequus.Content.Necromancy;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod.ModCalls
{
    public class ModCallManager
    {
        public const string Success = "Success";
        public const string Failure = "Failure";

        public static object HandleModCall(object[] args)
        {
            try
            {
                string type = args[0] as string;
                var mod = args[1] as Mod;

                switch (type)
                {
                    case "downedCrabson":
                        return HandleGetterSetterCall(ref AequusWorld.downedCrabson, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedOmegaStarite":
                        return HandleGetterSetterCall(ref AequusWorld.downedOmegaStarite, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedDustDevil":
                        return HandleGetterSetterCall(ref AequusWorld.downedDustDevil, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedHyperStarite":
                        return HandleGetterSetterCall(ref AequusWorld.downedHyperStarite, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedUltraStarite":
                        return HandleGetterSetterCall(ref AequusWorld.downedUltraStarite, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedRedSprite":
                        return HandleGetterSetterCall(ref AequusWorld.downedRedSprite, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedSpaceSquid":
                        return HandleGetterSetterCall(ref AequusWorld.downedSpaceSquid, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedEventDemon":
                        return HandleGetterSetterCall(ref AequusWorld.downedEventDemon, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedEventCosmic":
                        return HandleGetterSetterCall(ref AequusWorld.downedEventCosmic, args, AequusHelpers.UnboxBoolean, 2);
                    case "downedEventAtmosphere":
                        return HandleGetterSetterCall(ref AequusWorld.downedEventAtmosphere, args, AequusHelpers.UnboxBoolean, 2);

                    case "PylonColor":
                        var key = new Point(AequusHelpers.UnboxInt.Unbox(args[2]), args.Length > 4 ? AequusHelpers.UnboxInt.Unbox(args[3]) : 0);
                        var color = (Color)args[args.Length > 4 ? 4 : 3];
                        AequusTile.PylonColors[key] = color;
                        return null;

                    case "NecromancyDatabase":
                        return NecromancyDatabase.CallAddNecromancyData(mod, args);
                    case "NecromancyNoAutogeneration":
                        return NecromancyDatabase.CallAddNecromancyModBlacklist(mod, args);

                    case "DemonSiegeSacrifice":
                        return DemonSiegeSystem.CallAddDemonSiegeData(mod, args);
                    case "DemonSiegeSacrificeHide":
                        return DemonSiegeSystem.CallHideDemonSiegeData(mod, args);
                }
            }
            catch
            {
            }
            return null;
        }

        private static T HandleGetterSetterCall<T>(ref T value, object[] args, ITypeUnboxer<T> unboxer, int index = 2) 
        {
            var currentValue = value;

            if (args.Length > index)
            {
                if (unboxer.TryUnbox(args[index], out var value2))
                {
                    value = value2;
                }
            }
            return value;
        }

        private static void UnboxFail<T>(object obj)
        {
            Aequus.Instance.Logger.Error($"Value of {(obj == null ? "null" : obj.GetType().FullName)} did not unbox into {typeof(T).FullName}.");
        }
    }
}