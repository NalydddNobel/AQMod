using Terraria.GameContent;

namespace Aequus.Old.Content.Building;

[AutoloadEquip(EquipType.HandsOn, EquipType.HandsOff)]
public class LavaproofMitten : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 5);
    }

    public override void UpdateEquip(Player player) {
        player.GetModPlayer<AequusPlayer>().accLavaPlacement = true;
    }
}

public class LavaproofMittenGlobalItem : GlobalItem {
    public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
        return entity.createTile > -1 || entity.tileWand > -1;
    }

    public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox) {
    }

    public override bool? UseItem(Item item, Player player) {
        if (Main.myPlayer != player.whoAmI || Main.netMode == NetmodeID.Server) {
            return null;
        }

        int x = Player.tileTargetX;
        int y = Player.tileTargetY;
        Tile tile = Framing.GetTileSafely(x, y);
        if (Main.GameUpdateCount % 2 != 0 || !WorldGen.InWorld(x, y, 5) ||
            ((tile.LiquidAmount == 0 || tile.LiquidType != LiquidID.Lava) && (!player.TileReplacementEnabled || !WorldGen.WouldTileReplacementBeBlockedByLiquid(x, y, LiquidID.Lava)))) {
            return null;
        }

        Main.instance.LoadItem(item.type);
        Texture2D texture = TextureAssets.Item[item.type].Value;
        int itemWidth = texture.Width;
        int itemHeight = texture.Height;
        Rectangle hitbox = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, itemWidth, itemHeight);
        if (player.direction == -1) {
            hitbox.X -= hitbox.Width;
        }
        if (player.gravDir == 1f) {
            hitbox.Y -= hitbox.Height;
        }
        if (player.itemAnimation < player.itemAnimationMax * 0.333) {
            if (player.direction == -1) {
                hitbox.X -= (int)(hitbox.Width * 1.4 - hitbox.Width);
            }
            hitbox.Width = (int)(hitbox.Width * 1.4);
            hitbox.Y += (int)(hitbox.Height * 0.5 * player.gravDir);
            hitbox.Height = (int)(hitbox.Height * 1.1);
        }
        else if (!(player.itemAnimation < player.itemAnimationMax * 0.666)) {
            if (player.direction == 1) {
                hitbox.X -= (int)(hitbox.Width * 1.2);
            }
            hitbox.Width *= 2;
            hitbox.Y -= (int)((hitbox.Height * 1.4 - hitbox.Height) * player.gravDir);
            hitbox.Height = (int)(hitbox.Height * 1.4);
        }

        Dust d = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Torch, player.direction * 1.5f, -2f, Scale: 2f);
        d.noGravity = true;
        return null;
    }
}
