using Aequus.Core.ContentGeneration;

namespace Aequus.Old.Common.Carpentry;

public class InstancedBuilderBuff : InstancedModBuff {
    private BuildChallenge _parent;

    public InstancedBuilderBuff(BuildChallenge parent) : base(parent.Name, $"{parent.NamespaceFilePath()}/{parent.Name.Replace("Challenge", "")}Buff") {
        _parent = parent;
    }

    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        _parent.UpdateBuff(player, ref buffIndex);
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}
