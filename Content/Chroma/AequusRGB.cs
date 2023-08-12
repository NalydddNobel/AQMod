using ReLogic.Peripherals.RGB;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Chroma;

public class AequusRGB : ILoadable {
    private readonly List<ChromaShader> _registeredShaders = new();

    private void RegisterShader(ChromaShader shader, ChromaCondition condition, ShaderLayer layer) {
        _registeredShaders.Add(shader);
        Main.Chroma.RegisterShader(shader, condition, layer);
    }

    public void Load(Mod mod) {
        if (Main.dedServ) {
            return;
        }
        RegisterShader(new GlimmerShader(), new GlimmerShader.GlimmerCondition(), ShaderLayer.Event);
        RegisterShader(new OmegaStariteShader(), new OmegaStariteShader.OmegaStariteCondition(), ShaderLayer.Boss);
    }

    public void Unload() {
        if (Main.dedServ) {
            return;
        }
        foreach (var shader in _registeredShaders) {
            Main.Chroma.UnregisterShader(shader);
        }
    }
}