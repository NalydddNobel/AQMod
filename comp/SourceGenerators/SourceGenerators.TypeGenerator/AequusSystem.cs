using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;
using System.IO;

namespace Aequus;

public partial class AequusSystem {
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
    public static int buriedChestsLooted;
    [CompilerGenerated]
    public static bool usedReforgeBook;
    [CompilerGenerated]
    public static readonly Condition ConditionUsedReforgeBook = new("Mods.Aequus.Condition.UsedReforgeBook", () => usedReforgeBook);
    [CompilerGenerated]
    public static readonly Condition ConditionNotUsedReforgeBook = new("Mods.Aequus.Condition.UsedReforgeBook", () => !usedReforgeBook);
    
    [CompilerGenerated]
    private void SaveInner(TagCompound tag) {
        this.SaveObj(tag, "downedSalamancer", downedSalamancer);
        this.SaveObj(tag, "downedDustDevil", downedDustDevil);
        this.SaveObj(tag, "downedRedSprite", downedRedSprite);
        this.SaveObj(tag, "downedSpaceSquid", downedSpaceSquid);
        this.SaveObj(tag, "buriedChestsLooted", buriedChestsLooted);
        this.SaveObj(tag, "usedReforgeBook", usedReforgeBook);
    }
    
    [CompilerGenerated]
    private void LoadInner(TagCompound tag) {
        this.LoadObj(tag, "downedSalamancer", ref downedSalamancer);
        this.LoadObj(tag, "downedDustDevil", ref downedDustDevil);
        this.LoadObj(tag, "downedRedSprite", ref downedRedSprite);
        this.LoadObj(tag, "downedSpaceSquid", ref downedSpaceSquid);
        this.LoadObj(tag, "buriedChestsLooted", ref buriedChestsLooted);
        this.LoadObj(tag, "usedReforgeBook", ref usedReforgeBook);
    }
    
    [CompilerGenerated]
    private void SendDataInner(BinaryWriter writer) {
        this.SendObj(writer, downedSalamancer);
        this.SendObj(writer, downedDustDevil);
        this.SendObj(writer, downedRedSprite);
        this.SendObj(writer, downedSpaceSquid);
        this.SendObj(writer, buriedChestsLooted);
        this.SendObj(writer, usedReforgeBook);
    }
    
    [CompilerGenerated]
    private void ReceiveDataInner(BinaryReader reader) {
        this.ReceiveObj(reader, ref downedSalamancer);
        this.ReceiveObj(reader, ref downedDustDevil);
        this.ReceiveObj(reader, ref downedRedSprite);
        this.ReceiveObj(reader, ref downedSpaceSquid);
        this.ReceiveObj(reader, ref buriedChestsLooted);
        this.ReceiveObj(reader, ref usedReforgeBook);
    }
}