namespace Aequu2.Old.Content.Necromancy;

public abstract class ScepterBase : ModItem, ISceptreItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override bool MagicPrefix() {
        return true;
    }
}