using System;

namespace Aequus.NPCs.Town;

[Autoload(false)]
internal class InstancedEmote(string name, string texture, int emoteCategory, Func<bool> IsUnlocked = null) : ModEmoteBubble {
    private readonly int _emoteCategory = emoteCategory;

    private readonly Func<bool> _isUnlocked = IsUnlocked;

    private readonly string _name = name;
    private readonly string _texture = texture;

    public override string Name => _name;
    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    public override void SetStaticDefaults() {
        AddToCategory(_emoteCategory);
    }

    public override bool IsUnlocked() {
        return _isUnlocked?.Invoke() ?? true;
    }
}

internal class InstancedNPCEmote(ModNPC modNPC, int emoteCategory, Func<bool> IsUnlocked = null) : InstancedEmote($"{modNPC.Name}", $"{modNPC.NamespacePath()}/Emotes/{modNPC.Name}Emote", emoteCategory, IsUnlocked) {
    private readonly ModNPC _parentNPC = modNPC;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        if (_parentNPC.NPC.townNPC) {
            NPCID.Sets.FaceEmote[_parentNPC.Type] = Type;
        }
    }
}
