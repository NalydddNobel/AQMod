using System;
using System.Collections.Generic;

namespace Aequus.Core.Initialization;

internal class LoadingSteps : ModSystem {
    private static bool _postSetupContent = false;
    private static readonly Queue<Action> _postSetupContentActionQueue = new();

    private static bool _addRecipes = false;
    private static readonly Queue<Action> _addRecipesActionQueue = new();

    public static void EnqueuePostSetupContent(Action action) {
        if (_postSetupContent) {
            action.Invoke();
        }
        else {
            _postSetupContentActionQueue.Enqueue(action);
        }
    }

    public static void EnqueueAddRecipes(Action action) {
        if (_addRecipes) {
            action.Invoke();
        }
        else {
            _addRecipesActionQueue.Enqueue(action);
        }
    }

    public override void PostSetupContent() {
        _postSetupContent = true;
        while (_postSetupContentActionQueue.TryDequeue(out var action)) {
            action.Invoke();
        }
    }

    public override void AddRecipes() {
        _addRecipes = true;
        while (_addRecipesActionQueue.TryDequeue(out var action)) {
            action.Invoke();
        }
    }

    public override void Unload() {
        _postSetupContent = false;
        _addRecipes = false;
        _postSetupContentActionQueue.Clear();
    }
}