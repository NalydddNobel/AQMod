using System;

namespace AQMod.Common.Utilities
{
    internal sealed class CachedTask
    {
        public readonly object Obj;
        public readonly Func<object, object> Task;

        public CachedTask(object obj, Func<object, object> task)
        {
            Obj = obj;
            Task = task;
        }

        public object Invoke()
        {
            return Task.Invoke(Obj);
        }
    }
}