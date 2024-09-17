using FNAUtils;
using System;
using System.Collections.Generic;

namespace Aequus.Common.Utilities.Helpers;

public class GraphicsDeviceHelper {
    private readonly ArrayInstancePool<RenderTargetBinding> _bufferPool = new();
    private readonly Queue<RenderTargetBinding[]> _bindings = new();

    public void EnqueueRenderTargetBindings(GraphicsDevice gd) {
        int count = gd.GetRenderTargetsNoAllocEXT(null);

        RenderTargetBinding[] bindings = _bufferPool.Get(count);
        gd.GetRenderTargetsNoAllocEXT(bindings);
        _bindings.Enqueue(bindings);
    }

    public void DequeueRenderTargetBindings(GraphicsDevice gd) {
        if (_bindings.Count == 0) {
            throw new Exception("There were no bindings to pop.");
        }

        RenderTargetBinding[] bindings = _bindings.Dequeue();
        gd.SetRenderTargets(bindings);
    }

    public void SetRenderTargetAndEnqueueBindings(GraphicsDevice gd, RenderTarget2D texture) {
        if (texture == null) {
            DequeueRenderTargetBindings(gd);
        }
        else {
            EnqueueRenderTargetBindings(gd);

            gd.SetRenderTarget(texture);
        }
    }
}