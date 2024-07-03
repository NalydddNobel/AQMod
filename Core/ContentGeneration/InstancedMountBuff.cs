using Terraria.Localization;

namespace Aequu2.Core.ContentGeneration;

internal class InstancedMountBuff : InstancedModBuff {
    protected readonly UnifiedModMount _parent;

    public InstancedMountBuff(UnifiedModMount parent)
        : base(parent.Name, parent.NamespaceFilePath() + $"/{parent.Name.Replace("Mount", "")}Buff") {
        _parent = parent;
    }

    public override LocalizedText DisplayName => Language.GetOrRegister(_parent.GetLocalizationKey("BuffDisplayName"));
    public override LocalizedText Description => Language.GetOrRegister(_parent.GetLocalizationKey("BuffDescription"));

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.mount.SetMount(_parent.Type, player);
        player.buffTime[buffIndex] = 10;
    }
}