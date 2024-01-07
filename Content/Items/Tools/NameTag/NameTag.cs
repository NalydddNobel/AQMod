using Aequus.Common.Items.Components;
using Aequus.Common.Renaming;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Aequus.Content.Items.Tools.NameTag;

public class NameTag : ModItem, ICustomNameTagPrice {
    public static int ChestSpawnrate { get; set; } = 4;

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

    public override bool CanUseItem(Player player) {
        return Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) && itemNameTag.HasCustomName;
    }

    public override bool? UseItem(Player player) {
        if (Main.myPlayer != player.whoAmI || !player.ItemTimeIsZero || !player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple) || !Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) || !itemNameTag.HasCustomName) {
            return false;
        }

        int screenMouseX = Main.mouseX + (int)Main.screenPosition.X;
        int screenMouseY = Main.mouseY + (int)Main.screenPosition.Y;
        for (int i = 0; i < Main.maxNPCs; i++) {
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

    public static void NametagEffects(int i, string nameTag) {
        if (Main.npc[i].TryGetGlobalNPC<RenameNPC>(out var npcNameTag)) {
            npcNameTag.CustomName = nameTag;
            npcNameTag.nameTagAnimation = 1f;
        }

        SoundEngine.PlaySound(SoundID.Item92, Main.npc[i].Center);

        if (Main.netMode != NetmodeID.Server) {
            for (int k = 0; k < 15; k++) {
                var d = Dust.NewDustDirect(Main.npc[i].position, Main.npc[i].width, Main.npc[i].height, DustID.AncientLight, 0f, 0f, Scale: Main.rand.NextFloat(0.5f, 0.8f));
                d.velocity *= 0.1f;
                d.velocity += Main.npc[i].velocity;
                d.fadeIn = d.scale + 0.25f;
                d.noGravity = true;
            }
        }
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
        if (!Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) || !itemNameTag.HasCustomName) {
            spriteBatch.Draw(AequusTextures.NameTagBlank, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
        if (!Item.TryGetGlobalItem<RenameItem>(out var itemNameTag) || !itemNameTag.HasCustomName) {
            Main.GetItemDrawFrame(Item.type, out var texture, out var frame);
            spriteBatch.Draw(AequusTextures.NameTagBlank, ItemHelper.WorldDrawPos(Item, texture), frame, lightColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }

    public int GetNameTagPrice() {
        return 0;
    }
}