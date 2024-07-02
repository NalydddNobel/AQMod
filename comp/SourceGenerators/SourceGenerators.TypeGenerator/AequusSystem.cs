using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;
using System.IO;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusSystem {
    [CompilerGenerated]
    public static int buriedChestsLooted;
    [CompilerGenerated]
    public static bool usedReforgeBook;
    [CompilerGenerated]
    public static readonly Condition ConditionUsedReforgeBook = new("Mods.Aequus.Condition.UsedReforgeBook", () => usedReforgeBook);
    [CompilerGenerated]
    public static readonly Condition ConditionNotUsedReforgeBook = new("Mods.Aequus.Condition.UsedReforgeBook", () => !usedReforgeBook);
    [CompilerGenerated]
    public static bool downedSalamancer;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedSalamancer = new("Mods.Aequus.Condition.DownedSalamancer", () => downedSalamancer);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedSalamancer = new("Mods.Aequus.Condition.DownedSalamancer", () => !downedSalamancer);
    [CompilerGenerated]
    public static bool downedDustDevil;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedDustDevil = new("Mods.Aequus.Condition.DownedDustDevil", () => downedDustDevil);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedDustDevil = new("Mods.Aequus.Condition.DownedDustDevil", () => !downedDustDevil);
    [CompilerGenerated]
    public static bool downedOmegaStarite;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedOmegaStarite = new("Mods.Aequus.Condition.DownedOmegaStarite", () => downedOmegaStarite);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedOmegaStarite = new("Mods.Aequus.Condition.DownedOmegaStarite", () => !downedOmegaStarite);
    [CompilerGenerated]
    public static bool downedRedSprite;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedRedSprite = new("Mods.Aequus.Condition.DownedRedSprite", () => downedRedSprite);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedRedSprite = new("Mods.Aequus.Condition.DownedRedSprite", () => !downedRedSprite);
    [CompilerGenerated]
    public static bool downedSpaceSquid;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedSpaceSquid = new("Mods.Aequus.Condition.DownedSpaceSquid", () => downedSpaceSquid);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedSpaceSquid = new("Mods.Aequus.Condition.DownedSpaceSquid", () => !downedSpaceSquid);
    [CompilerGenerated]
    public static bool downedUltraStarite;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedUltraStarite = new("Mods.Aequus.Condition.DownedUltraStarite", () => downedUltraStarite);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedUltraStarite = new("Mods.Aequus.Condition.DownedUltraStarite", () => !downedUltraStarite);
    [CompilerGenerated]
    public static bool downedDemonSiege;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedDemonSiege = new("Mods.Aequus.Condition.DownedDemonSiege", () => downedDemonSiege);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedDemonSiege = new("Mods.Aequus.Condition.DownedDemonSiege", () => !downedDemonSiege);
    [CompilerGenerated]
    public static bool metOccultist;
    [CompilerGenerated]
    public static readonly Condition ConditionMetOccultist = new("Mods.Aequus.Condition.MetOccultist", () => metOccultist);
    [CompilerGenerated]
    public static readonly Condition ConditionNotMetOccultist = new("Mods.Aequus.Condition.MetOccultist", () => !metOccultist);
    
    [CompilerGenerated]
    private void SaveInner(TagCompound tag) {
        this.SaveObj(tag, "buriedChestsLooted", buriedChestsLooted);
        this.SaveObj(tag, "usedReforgeBook", usedReforgeBook);
        this.SaveObj(tag, "downedSalamancer", downedSalamancer);
        this.SaveObj(tag, "downedDustDevil", downedDustDevil);
        this.SaveObj(tag, "downedOmegaStarite", downedOmegaStarite);
        this.SaveObj(tag, "downedRedSprite", downedRedSprite);
        this.SaveObj(tag, "downedSpaceSquid", downedSpaceSquid);
        this.SaveObj(tag, "downedUltraStarite", downedUltraStarite);
        this.SaveObj(tag, "downedDemonSiege", downedDemonSiege);
        this.SaveObj(tag, "metOccultist", metOccultist);
    }
    
    [CompilerGenerated]
    private void LoadInner(TagCompound tag) {
        this.LoadObj(tag, "buriedChestsLooted", ref buriedChestsLooted);
        this.LoadObj(tag, "usedReforgeBook", ref usedReforgeBook);
        this.LoadObj(tag, "downedSalamancer", ref downedSalamancer);
        this.LoadObj(tag, "downedDustDevil", ref downedDustDevil);
        this.LoadObj(tag, "downedOmegaStarite", ref downedOmegaStarite);
        this.LoadObj(tag, "downedRedSprite", ref downedRedSprite);
        this.LoadObj(tag, "downedSpaceSquid", ref downedSpaceSquid);
        this.LoadObj(tag, "downedUltraStarite", ref downedUltraStarite);
        this.LoadObj(tag, "downedDemonSiege", ref downedDemonSiege);
        this.LoadObj(tag, "metOccultist", ref metOccultist);
    }
    
    [CompilerGenerated]
    private void SendDataInner(BinaryWriter writer) {
        this.SendObj(writer, buriedChestsLooted);
        this.SendObj(writer, usedReforgeBook);
        this.SendObj(writer, downedSalamancer);
        this.SendObj(writer, downedDustDevil);
        this.SendObj(writer, downedOmegaStarite);
        this.SendObj(writer, downedRedSprite);
        this.SendObj(writer, downedSpaceSquid);
        this.SendObj(writer, downedUltraStarite);
        this.SendObj(writer, downedDemonSiege);
        this.SendObj(writer, metOccultist);
    }
    
    [CompilerGenerated]
    private void ReceiveDataInner(BinaryReader reader) {
        this.ReceiveObj(reader, ref buriedChestsLooted);
        this.ReceiveObj(reader, ref usedReforgeBook);
        this.ReceiveObj(reader, ref downedSalamancer);
        this.ReceiveObj(reader, ref downedDustDevil);
        this.ReceiveObj(reader, ref downedOmegaStarite);
        this.ReceiveObj(reader, ref downedRedSprite);
        this.ReceiveObj(reader, ref downedSpaceSquid);
        this.ReceiveObj(reader, ref downedUltraStarite);
        this.ReceiveObj(reader, ref downedDemonSiege);
        this.ReceiveObj(reader, ref metOccultist);
    }
}