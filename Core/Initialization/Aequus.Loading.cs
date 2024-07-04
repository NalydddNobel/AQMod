using System;

namespace AequusRemake;

partial class AequusRemake {
    internal static LoadActions OnPostSetupContent;
    internal static LoadActions OnAddRecipes;
    internal static LoadActions OnPostAddRecipes;

    private static void UnloadLoadingSteps() {
        OnPostSetupContent = null;
        OnAddRecipes = null;
        OnPostAddRecipes = null;
    }

    internal class LoadActions {
        private Action _action;

        public void Invoke() {
            if (_action == null) {
                ThrowLoadTimeError();
            }

            _action?.Invoke();
            _action = null;
        }

        public static LoadActions operator +(LoadActions a, Action b) {
            if (a != null) {
                if (a._action == null) {
                    ThrowLoadTimeError();
                    return a;
                }
            }
            else {
                a = new();
            }

            a._action += b;
            return a;
        }

        private static void ThrowLoadTimeError() {
            throw new Exception("Load actions were attempted to be added despite the loading step already occuring.");
        }
    }

    private class LoadingSteps : ModSystem {
        public override void PostSetupContent() {
            OnPostSetupContent?.Invoke();
        }

        public override void AddRecipes() {
            OnAddRecipes?.Invoke();
        }

        public override void PostAddRecipes() {
            OnPostAddRecipes?.Invoke();
        }
    }
}
