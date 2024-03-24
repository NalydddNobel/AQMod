using System;

namespace Aequus.Core.ContentGeneration;

[Autoload(false)]
internal class InstancedEmote : ModEmoteBubble {
    private readonly int _emoteCategory;

    private readonly Func<bool> _isUnlocked;

    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;
    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    public InstancedEmote(string name, string texture, int emoteCategory, Func<bool> IsUnlocked = null) {
        _name = name;
        _texture = texture;
        _emoteCategory = emoteCategory;
        _isUnlocked = IsUnlocked;
    }

    public override void SetStaticDefaults() {
        AddToCategory(_emoteCategory);
    }

    public override bool IsUnlocked() {
        return _isUnlocked?.Invoke() ?? true;
    }
}

internal class InstancedNPCEmote : InstancedEmote {
    private readonly ModNPC _parentNPC;

    public InstancedNPCEmote(ModNPC modNPC, int emoteCategory, Func<bool> IsUnlocked = null) : base($"{modNPC.Name}", $"{modNPC.NamespaceFilePath()}/Emotes/{modNPC.Name}Emote", emoteCategory, IsUnlocked) {
        _parentNPC = modNPC;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        if (_parentNPC.NPC.townNPC) {
            NPCSets.FaceEmote[Type] = Type;
        }
    }
}
