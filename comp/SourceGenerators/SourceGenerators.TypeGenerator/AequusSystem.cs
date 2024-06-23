using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequus.Core.Structures;
using System.IO;
using System.Collections.Generic;

namespace Aequus;

public partial class AequusSystem {
    [CompilerGenerated]
    public static bool downedSalamancer;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedSalamancer = new("Mods.Aequus.Condition.DownedSalamancer", () => downedSalamancer);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedSalamancer = new("Mods.Aequus.Condition.DownedSalamancer", () => !downedSalamancer);
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
        this.SaveObj(tag, "buriedChestsLooted", buriedChestsLooted);
        this.SaveObj(tag, "usedReforgeBook", usedReforgeBook);
    }
    
    [CompilerGenerated]
    private void LoadInner(TagCompound tag) {
        this.LoadObj(tag, "downedSalamancer", ref downedSalamancer);
        this.LoadObj(tag, "buriedChestsLooted", ref buriedChestsLooted);
        this.LoadObj(tag, "usedReforgeBook", ref usedReforgeBook);
    }
    
    [CompilerGenerated]
    private void SendDataInner(BinaryWriter writer) {
        this.SendObj(writer, downedSalamancer);
        this.SendObj(writer, buriedChestsLooted);
        this.SendObj(writer, usedReforgeBook);
    }
    
    [CompilerGenerated]
    private void ReceiveDataInner(BinaryReader reader) {
        this.ReceiveObj(reader, ref downedSalamancer);
        this.ReceiveObj(reader, ref buriedChestsLooted);
        this.ReceiveObj(reader, ref usedReforgeBook);
    }
}