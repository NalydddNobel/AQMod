using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequu2.Core.ContentGeneration;

public abstract class UnifiedCritter() : ModNPC {
    public virtual int BestiaryCritterSort => 0;

    [CloneByReference]
    protected ModItem CatchItem { get; private set; }

    protected override bool CloneNewInstances => true;

    public virtual void OnLoad() { }
    public virtual void OnSetStaticDefaults() { }
    public virtual void OnSetDefaults() { }
    /// <summary>This method is not instanced.</summary>
    /// <param name="Item"></param>
    public virtual void SetItemDefaults(Item Item) { }

    public sealed override void Load() {
        CatchItem = new InstancedCritterItem(this);
        Mod.AddContent(CatchItem);
        OnLoad();
    }

    public sealed override void SetStaticDefaults() {
        Main.npcCatchable[Type] = true;
        NPCSets.CountsAsCritter[Type] = true;
        NPCSets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
        NPCSets.TownCritter[Type] = true;

        int critterSort = BestiaryCritterSort;
        if (critterSort != 0) {
            NPCSets.NormalGoldCritterBestiaryPriority.Insert(NPCSets.NormalGoldCritterBestiaryPriority.IndexOf(critterSort) + 1, Type);
        }

        OnSetStaticDefaults();
    }

    public sealed override void SetDefaults() {
        NPC.lifeMax = 5;
        NPC.catchItem = CatchItem.Type;
        OnSetDefaults();
    }

    public static bool FromCritterItem(IEntitySource source, out Item critterItem) {
        if (source is EntitySource_Parent parentSource) {
            if (parentSource.Entity is Player player) {
                critterItem = player.HeldItemFixed();
                return true;
            }
            else if (parentSource.Entity is Item sourceItem) {
                critterItem = sourceItem;
                return true;
            }
        }

        critterItem = null;
        return false;
    }
}

internal class InstancedCritterItem(UnifiedCritter Critter, string NameSuffix = "") : InstancedModItem(Critter.Name + NameSuffix, Critter.Texture + "Item") {
    protected readonly UnifiedCritter Critter = Critter;

    public override LocalizedText DisplayName => Critter.DisplayName;
    public override LocalizedText Tooltip => Critter.GetLocalization("ItemTooltip");

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.DefaultToCapturedCritter(Critter.Type);
        Critter.SetItemDefaults(Item);
    }
}