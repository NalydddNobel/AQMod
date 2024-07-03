using Aequu2.Core.Structures;
using Terraria.Localization;

namespace Aequu2.Core.ContentGeneration;

internal class InstancedMinionBuff : InstancedBuff {
    private readonly UnifiedModMinion _parent;
    public readonly RefFunc<Player, bool> _petFlag;

    internal InstancedMinionBuff(UnifiedModMinion parent, RefFunc<Player, bool> petFlag) : base($"{parent.Name}Buff", $"{parent.Texture}Buff") {
        _parent = parent;
        _petFlag = petFlag;
    }

    public override LocalizedText DisplayName => Language.GetOrRegister(_parent.GetLocalizationKey("BuffDisplayName"));
    public override LocalizedText Description => Language.GetOrRegister(_parent.GetLocalizationKey("BuffDescription"));

    public override string LocalizationCategory => "Minions";

    public override void SetStaticDefaults() {
        Main.buffNoSave[Type] = true;
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        _petFlag(player) = true;
        if (player.ownedProjectileCounts[_parent.Type] > 0) {
            player.buffTime[buffIndex] = 18000;
        }
        else {
            player.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}