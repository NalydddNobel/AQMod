using AequusRemake.Core.CodeGeneration;

namespace AequusRemake.Content.Items.PermaPowerups.Shimmer;

[Gen.AequusSystem_WorldField<bool>("usedReforgeBook")]
public class TinkerersGuidebook : ModItem {
    public static int BonusRerolls { get; set; } = 2;

    public override void SetDefaults() {
        Item.useTime = 45;
        Item.useAnimation = 45;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override bool? UseItem(Player player) {
        if (AequusSystem.usedReforgeBook) {
            return false;
        }

        AequusSystem.usedReforgeBook = true;
        if (Main.netMode == NetmodeID.Server) {
            NetMessage.SendData(MessageID.WorldData);
        }

        WorldGen.BroadcastText(this.GetLocalization("DisplayMessage").ToNetworkText(), CommonColor.TextEvent);
        return true;
    }
}