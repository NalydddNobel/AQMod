using System;
using System.Collections.Generic;

namespace Aequus.Core;

internal class ContentInitializationSteps : ModSystem {
    private static bool postSetupContent = false;
    private static Queue<Action> _postSetupContentFactoryQueue = new();

    public static void EnqueuePostSetupContent(Action action) {
        if (postSetupContent) {
            action.Invoke();
        }
        else {
            _postSetupContentFactoryQueue.Enqueue(action);
        }
    }

    public override void PostSetupContent() {
        postSetupContent = true;
        while (_postSetupContentFactoryQueue.TryDequeue(out var action)) {
            action.Invoke();
        }
    }

    public override void Unload() {
        _postSetupContentFactoryQueue.Clear();
    }
}