using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes;

public class ArmorShaderDataDynamicColour : ArmorShaderData {
    public Func<Entity, DrawData?, Color> getColor;

    public ArmorShaderDataDynamicColour(Ref<Effect> shader, string passName, Func<Entity, DrawData?, Color> func) : base(shader, passName) {
        getColor = func;
    }

    public override void Apply(Entity entity, DrawData? drawData = null) {
        UseColor(getColor(entity, drawData));
        base.Apply(entity, drawData);
    }
}