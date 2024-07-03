using Aequu2.Content.Systems.Renaming;
using Aequu2.Core.Entities.Items.Components;
using Terraria.Audio;
using Terraria.DataStructures;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequu2.Content.Items.Tools.NameTag;

[FilterOverride(FilterOverride.Tools)]
public class NameTag : ModItem, ICustomNameTagPrice {
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
            NPC npc = Main.npc[i];
            if (!npc.active || !npc.getRect().Contains(screenMouseX, screenMouseY) || !npc.TryGetGlobalNPC(out RenameNPC npcNameTag) || npcNameTag.CustomName == itemNameTag.CustomName) {
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
        }

        SoundEngine.PlaySound(SoundID.Item92, Main.npc[i].Center);

        if (Main.netMode != NetmodeID.Server) {
            ModContent.GetInstance<NPCNameTagPopup>().ShowRenamePopup(i);

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
            spriteBatch.Draw(AequusTextures.NameTagBlank, ExtendItem.WorldDrawPos(Item, frame), frame, lightColor, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
            return false;
        }
        return true;
    }

    public int GetNameTagPrice() {
        return 0;
    }
}