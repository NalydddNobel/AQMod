using System;

namespace Aequus.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GlowMaskAttribute : Attribute
    {
        public readonly string[] CustomGlowmasks;
        public readonly bool AutoAssignItemID;

        public GlowMaskAttribute()
        {
            AutoAssignItemID = true;
            CustomGlowmasks = null;
        }

        public GlowMaskAttribute(params string[] glowmasks)
        {
            AutoAssignItemID = false;
            CustomGlowmasks = glowmasks;
        }
    }
}