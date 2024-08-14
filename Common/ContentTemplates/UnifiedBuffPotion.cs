using Aequus.Common.ContentTemplates.Generic;
using Aequus.Content.Items.Potions.Modifiers.Empowered;
using Terraria.Localization;

namespace Aequus.Common.ContentTemplates;

public abstract class UnifiedBuffPotion : ModItem {
    [CloneByReference]
    internal readonly InstancedBuff Buff;

    public readonly int Duration;

    /// <summary>Shorthand for <see cref="ItemID.Sets.DrinkParticleColors"/>[Type].</summary>
    protected ref Color[] DrinkColors => ref ItemID.Sets.DrinkParticleColors[Type];

    public virtual LocalizedText BuffDisplayName => this.GetLocalization("BuffDisplayName");
    public virtual LocalizedText BuffDescription => Tooltip;

    public UnifiedBuffPotion(int Duration) {
        this.Duration = Duration;
        Buff = new InstancedPotionBuff(this);
        EmpoweredBuff = new InstancedLegacyEmpoweredBuff(this);
    }

    protected override bool CloneNewInstances => true;

    public override void Load() {
        Mod.AddContent(Buff);
    }

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 20;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.RegenerationPotion);
        Item.buffType = Buff.Type;
        Item.buffTime = Duration;
    }

    /// <summary>This method is not instanced.</summary>
    public abstract void UpdateBuff(Player player, ref int buffIndex);

    #region Empowered
    public virtual LocalizedText EmpoweredTooltip => this.GetLocalization("EmpoweredTooltip");
    public virtual LocalizedText EmpoweredBuffDescription => EmpoweredTooltip;

    [CloneByReference]
    internal readonly InstancedLegacyEmpoweredBuff EmpoweredBuff;
    #endregion
}

internal class InstancedPotionBuff(UnifiedBuffPotion Parent, string nameSuffix = "") : InstancedBuff($"{Parent.Name}{nameSuffix}", $"{Parent.Texture}Buff") {
    protected UnifiedBuffPotion Parent = Parent;

    public override LocalizedText DisplayName => Parent.BuffDisplayName;
    public override LocalizedText Description => Parent.BuffDescription;

    public override void Update(Player player, ref int buffIndex) {
        Parent.UpdateBuff(player, ref buffIndex);
    }
}