using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Items;
using Aequus.Items.Misc.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Boss.OmegaStarite.Misc {
    public class SupernovaFruit : ModItem/*, GlowmaskData.IPlayerHeld*/
    {
        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.Abeemination];
            Item.ResearchUnlockCount = 1;
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override bool CanUseItem(Player player) {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool? UseItem(Player player) {
            if (Main.netMode == NetmodeID.SinglePlayer) {
                NPC.SpawnBoss((int)player.position.X, (int)player.position.Y - 1600, ModContent.NPCType<OmegaStarite>(), player.whoAmI);
            }
            else if (Main.myPlayer == player.whoAmI) {
                NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: ModContent.NPCType<OmegaStarite>());
            }
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: true, useScissorRectangle: true);
            var drawData = new DrawData(texture, position, null, itemColor.A > 0 ? itemColor : Main.inventoryBack, 0f, origin, scale, SpriteEffects.None, 0);
            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<EnchantedDye>());
            effect.Apply(null, drawData);
            drawData.Draw(spriteBatch);
            Main.spriteBatch.End();
            spriteBatch.Begin_UI(immediate: false, useScissorRectangle: true);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: true);

            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + frame.Width / 2 + Item.width / 2 - frame.Width / 2, Item.position.Y - Main.screenPosition.Y + frame.Height / 2 + Item.height - frame.Height);
            Vector2 origin = frame.Size() / 2f;
            var drawData = new DrawData(texture, drawPosition, frame, Item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<EnchantedDye>());
            effect.Apply(null, drawData);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin_World(shader: false); ;
            return false;
        }
    }
}