using Aequus.Common.Net;
using Aequus.Items.Materials.Gems;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc {
    public class ShimmerSundialCharge : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults() {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.width = 16;
            Item.height = 16;
            Item.consumable = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(silver: 10);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool? UseItem(Player player) {
            if (Main.myPlayer != player.whoAmI) {
                return false;
            }

            int x = Player.tileTargetX;
            int y = Player.tileTargetY;
            if (!WorldGen.InWorld(x, y, 40) || !player.IsInTileInteractionRange(x, y)) {
                return false;
            }

            return Use(x, y, player.whoAmI);
        }

        private static void CheckSundial(int x, int y, int plr, ref int cooldown) {
            if (cooldown <= 0) {
                return;
            }

            cooldown = 0;
            SoundEngine.PlaySound(SoundID.Item4, new Vector2(x, y) * 16f);
            if (Main.netMode != NetmodeID.SinglePlayer && plr == Main.myPlayer) {
                ModContent.GetInstance<SundialResetPacket>().Send(x, y, plr);
            }
        }
        public static bool? Use(int x, int y, int plr) {
            var tile = Main.tile[x, y];
            if (tile.TileType == TileID.Sundial) {
                CheckSundial(x, y, plr, ref Main.sundialCooldown);
                return true;
            }
            if (tile.TileType == TileID.Moondial) {
                CheckSundial(x, y, plr, ref Main.moondialCooldown);
                return true;
            }
            return false;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<OmniGem>(5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class SundialResetPacket : PacketHandler {
        public override PacketType LegacyPacketType => PacketType.UseSundialResetItem;

        public void Send(int i, int j, int plr) {
            var p = GetPacket();
            p.Write((byte)plr);
            p.Write((ushort)i);
            p.Write((ushort)j);
            p.Send(ignoreClient: plr);
        }

        public override void Receive(BinaryReader reader, int sender) {
            var plr = reader.ReadByte();
            var i = reader.ReadUInt16();
            var j = reader.ReadUInt16();

            if (plr == Main.myPlayer || !WorldGen.InWorld(i, j)) {
                return;
            }

            if (Main.netMode == NetmodeID.Server) {
                ModContent.GetInstance<SundialResetPacket>().Send(i, j, plr);
            }

            ShimmerSundialCharge.Use(i, j, plr);
        }
    }
}