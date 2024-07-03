using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Aequu2.Core.Structures;
using System.IO;
using System.Collections.Generic;

namespace Aequu2;

public partial class AequusSystem {
    [CompilerGenerated]
    public static int buriedChestsLooted;
    [CompilerGenerated]
    public static bool usedReforgeBook;
    [CompilerGenerated]
    public static readonly Condition ConditionUsedReforgeBook = new("Mods.Aequu2.Condition.UsedReforgeBook", () => usedReforgeBook);
    [CompilerGenerated]
    public static readonly Condition ConditionNotUsedReforgeBook = new("Mods.Aequu2.Condition.UsedReforgeBook", () => !usedReforgeBook);
    [CompilerGenerated]
    public static bool downedSalamancer;
    [CompilerGenerated]
    public static readonly Condition ConditionDownedSalamancer = new("Mods.Aequu2.Condition.DownedSalamancer", () => downedSalamancer);
    [CompilerGenerated]
    public static readonly Condition ConditionNotDownedSalamancer = new("Mods.Aequu2.Condition.DownedSalamancer", () => !downedSalamancer);
    
    [CompilerGenerated]
    private void SaveInner(TagCompound tag) {
        this.SaveObj(tag, "buriedChestsLooted", buriedChestsLooted);
        this.SaveObj(tag, "usedReforgeBook", usedReforgeBook);
        this.SaveObj(tag, "downedSalamancer", downedSalamancer);
    }
    
    [CompilerGenerated]
    private void LoadInner(TagCompound tag) {
        this.LoadObj(tag, "buriedChestsLooted", ref buriedChestsLooted);
        this.LoadObj(tag, "usedReforgeBook", ref usedReforgeBook);
        this.LoadObj(tag, "downedSalamancer", ref downedSalamancer);
    }
    
    [CompilerGenerated]
    private void SendDataInner(BinaryWriter writer) {
        this.SendObj(writer, buriedChestsLooted);
        this.SendObj(writer, usedReforgeBook);
        this.SendObj(writer, downedSalamancer);
    }
    
    [CompilerGenerated]
    private void ReceiveDataInner(BinaryReader reader) {
        this.ReceiveObj(reader, ref buriedChestsLooted);
        this.ReceiveObj(reader, ref usedReforgeBook);
        this.ReceiveObj(reader, ref downedSalamancer);
    }
}