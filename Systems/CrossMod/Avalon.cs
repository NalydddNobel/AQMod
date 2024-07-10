using System.Reflection;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Systems.CrossMod;

internal class Avalon : SupportedMod<Avalon> {
    private static ModSystem _AvalonWorld;
    private static FieldInfo _AvalonWorld_WorldEvil;
    private static bool _contagionReflectionBroken;
    public static bool IsContagionWorld() {
        if (!Enabled || _contagionReflectionBroken) {
            return false;
        }

        if (_AvalonWorld_WorldEvil == null) {
            if (Instance.TryFind("AvalonWorld", out _AvalonWorld)) {
                _contagionReflectionBroken = true;
                return false;
            }

            _AvalonWorld_WorldEvil = _AvalonWorld.GetType().GetField("WorldEvil", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            if (_AvalonWorld_WorldEvil == null) {
                _contagionReflectionBroken = true;
                return false;
            }
        }

        return _AvalonWorld_WorldEvil.GetValue(_AvalonWorld)?.ToString() == "Contagion";
    }

    public class ItemDropConditions {
        public class IsContagion : IItemDropRuleCondition {
            public bool CanDrop(DropAttemptInfo info) {
                return IsContagionWorld();
            }

            public bool CanShowItemDropInUI() {
                return IsContagionWorld();
            }

            public string GetConditionDescription() {
                return ModContent.GetInstance<Avalon>().GetLocalizedValue("ItemDropRules.IsContagion");
            }
        }
        public class IsNotContagion : IItemDropRuleCondition {
            public bool CanDrop(DropAttemptInfo info) {
                return !IsContagionWorld();
            }

            public bool CanShowItemDropInUI() {
                return !IsContagionWorld();
            }

            public string GetConditionDescription() {
                return ModContent.GetInstance<Avalon>().GetLocalizedValue("ItemDropRules.IsNotContagion");
            }
        }
    }
}