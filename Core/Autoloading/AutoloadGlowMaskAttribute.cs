using System;

namespace Aequus.Core.Autoloading;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class AutoloadGlowMaskAttribute : AutoloadXAttribute {
    public readonly string[] CustomGlowmasks;
    public readonly bool AutoAssignID;

    public AutoloadGlowMaskAttribute() {
        AutoAssignID = true;
        CustomGlowmasks = null;
    }

    public AutoloadGlowMaskAttribute(params string[] glowmasks) {
        AutoAssignID = false;
        CustomGlowmasks = glowmasks;
    }

    internal override void Load(ModType modType) {
        if (Main.dedServ || modType is not ModItem m) {
            return;
        }

        string modItemTexture = m.Texture;
        // Check if this item will be registering multiple glowmasks
        if (CustomGlowmasks != null) {
            foreach (var customGlowmaskPath in CustomGlowmasks) {
                GlowMasksLoader.AddGlowmask(modItemTexture + customGlowmaskPath);
            }
            return;
        }

        // Otherwise, get a _Glow texture using the item's Texture property
        short glowmaskId = GlowMasksLoader.AddGlowmask(modItemTexture + "_Glow");

        // Assign an ItemID for this glowmask to be connected with, so it is automatically applied
        if (AutoAssignID) {
            GlowMasksLoader.ItemIdToGlowMaskId.Add(m.Type, glowmaskId);
        }
    }
}