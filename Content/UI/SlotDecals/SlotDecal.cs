using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.UI.SlotDecals;

public abstract class SlotDecal : ModTexturedType {
    public Asset<Texture2D> texture { get; private set; }

    public Dictionary<(int, int), float> OpacityPerSlot { get; internal set; } = new();

    protected sealed override void Register() {
        SlotDecalsLoader.Register(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
        if (!Main.dedServ) {
            texture = ModContent.Request<Texture2D>(Texture);
        }
    }

    public abstract bool CanDraw(int slot, int context);
}