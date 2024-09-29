using System;

namespace Aequus.Common.Structures.ID;

internal readonly record struct LazyId(Func<int> GetIdFactory) : IProvideId {
    public int GetId() {
        return GetIdFactory();
    }
}
