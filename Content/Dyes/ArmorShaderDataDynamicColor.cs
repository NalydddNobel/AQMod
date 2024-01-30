using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace Aequus.Content.Dyes;

public sealed class ArmorShaderDataDynamicColor : ArmorShaderData {
    public Func<Entity, DrawData?, Color> getColor;

    public ArmorShaderDataDynamicColor(Ref<Effect> shader, string passName, Func<Entity, DrawData?, Color> func) : base(shader, passName) {
        getColor = func;
    }

    public override void Apply(Entity entity, DrawData? drawData = null) {
        UseColor(getColor(entity, drawData));
        base.Apply(entity, drawData);
    }
}