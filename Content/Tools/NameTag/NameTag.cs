using Aequus.Common.Items.Components;
using Aequus.Common.Renaming;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Aequus.Content.Tools.NameTag;

public class NameTag : ModItem, ICustomNameTagPrice {
    public static System.Int32 ChestSpawnrate { get; set; } = 4;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }

    public override void SetDefaults() {
        Item.width = 24;
        Item.height = 24;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.consumable = true;
        Item.rare = ItemRarityID.White;
        Item.value = Item.buyPrice(gold: 1);
        Item.maxStack = Item.CommonMaxStack;
    }

    public override System.Boolean CanUseItem(Player player) {
        return Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) && itemNameTag.HasCustomName;
    }

    public override System.Boolean? UseItem(Player player) {
        if (Main.myPlayer != player.whoAmI || !player.ItemTimeIsZero || !player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple) || !Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) || !itemNameTag.HasCustomName) {
            return false;
        }

        System.Int32 screenMouseX = Main.mouseX + (System.Int32)Main.screenPosition.X;
        System.Int32 screenMouseY = Main.mouseY + (System.Int32)Main.screenPosition.Y;
        for (System.Int32 i = 0; i < Main.maxNPCs; i++) {
            var npc = Main.npc[i];
            if (!npc.active || !npc.getRect().Contains(screenMouseX, screenMouseY) || !npc.TryGetGlobalNPC<RenameNPC>(out var npcNameTag) || npcNameTag.CustomName == itemNameTag.CustomName) {
                continue;
            }

            NametagEffects(i, itemNameTag.CustomName);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                ModContent.GetInstance<NameTagPacket>().Send(i, itemNameTag.CustomName);
            }
            return true;
        }

        return false;
    }

    public static void NametagEffects(System.Int32 i, System.String nameTag) {
        if (Main.npc[i].TryGetGlobalNPC<RenameNPC>(out var npcNameTag)) {
            npcNameTag.CustomName = nameTag;
        }

        SoundEngine.PlaySound(SoundID.Item92, Main.npc[i].Center);

        if (Main.netMode != NetmodeID.Server) {
            ModContent.GetInstance<NPCNameTagPopup>().ShowRenamePopup(i);

            for (System.Int32 k = 0; k < 15; k++) {
                var d = Dust.NewDustDirect(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, DustID.AncientLight, 0f, 0f, Scale: Main.rand.NextFloat(0.5f, 0.8f));
                d.velocity *= 0.1f;
                d.velocity += Main.npc[i].velocity;
                d.fadeIn = d.scale + 0.25f;
                d.noGravity = true;
            }
        }
    }

    public override System.Boolean PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, System.Single scale) {
        if (!Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) || !itemNameTag.HasCustomName) {
            spriteBatch.Draw(AequusTextures.NameTagBlank, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }

    public override System.Boolean PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref System.Single rotation, ref System.Single scale, System.Int32 whoAmI) {
        if (!Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) || !itemNameTag.HasCustomName) {
            Main.GetItemDrawFrame(Item.type, out var texture, out var frame);
            spriteBatch.Draw(AequusTextures.NameTagBlank, ExtendItem.WorldDrawPos(Item, frame), frame, lightColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }

    public System.Int32 GetNameTagPrice() {
        return 0;
    }
}