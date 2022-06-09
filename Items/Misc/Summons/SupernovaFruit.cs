using Aequus.Graphics;
using Aequus.Items.Misc.Dyes;
using Aequus.Items.Misc.Energies;
using Aequus.NPCs.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Summons
{
    public class SupernovaFruit : ModItem/*, GlowmaskData.IPlayerHeld*/
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = ItemID.Sets.SortingPriorityBossSpawns[ItemID.Abeemination];
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime /*&& !Glimmer.IsGlimmerEventCurrentlyActive()*/ && !NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
        }

        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.SpawnBoss((int)player.position.X, (int)player.position.Y - 1600, ModContent.NPCType<OmegaStarite>(), player.whoAmI);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);
            }
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            CommonSpriteBatchBegins.UI.Begin(spriteBatch, CommonSpriteBatchBegins.Shader);
            var drawData = new DrawData(texture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<EnchantedDye>());
            effect.Apply(null, drawData);
            drawData.Draw(spriteBatch);
            Main.spriteBatch.End();
            CommonSpriteBatchBegins.UI.Begin(spriteBatch, CommonSpriteBatchBegins.Regular);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            var texture = TextureAssets.Item[Type].Value;

            Main.spriteBatch.End();
            CommonSpriteBatchBegins.GeneralEntities.BeginShader(Main.spriteBatch);

            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = new Vector2(Item.position.X - Main.screenPosition.X + frame.Width / 2 + Item.width / 2 - frame.Width / 2, Item.position.Y - Main.screenPosition.Y + frame.Height / 2 + Item.height - frame.Height);
            Vector2 origin = frame.Size() / 2f;
            var drawData = new DrawData(texture, drawPosition, frame, Item.GetAlpha(lightColor), rotation, origin, scale, SpriteEffects.None, 0);

            var effect = GameShaders.Armor.GetShaderFromItemId(ModContent.ItemType<EnchantedDye>());
            effect.Apply(null, drawData);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.End();
            CommonSpriteBatchBegins.GeneralEntities.Begin(Main.spriteBatch);
            return false;
        }

        //void GlowmaskData.IPlayerHeld.Draw(GlowmaskData glowmask, Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        //{
        //    var texture = Main.itemTexture[Item.type];
        //    Vector2 drawCoordinates;
        //    float drawRotation;
        //    Vector2 origin;
        //    Rectangle drawFrame;
        //    if (Item.useStyle == ItemUseStyleID.HoldingOut)
        //    {
        //        if (Item.staff[Item.type])
        //        {
        //            drawRotation = info.drawPlayer.itemRotation + 0.785f * info.drawPlayer.direction;
        //            int offsetX1 = 0;
        //            int offsetY = 0;
        //            origin = new Vector2(0f, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
        //            if (info.drawPlayer.gravDir == -1f)
        //            {
        //                if (info.drawPlayer.direction == -1)
        //                {
        //                    drawRotation += 1.57f;
        //                    origin = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, 0f);
        //                    offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
        //                }
        //                else
        //                {
        //                    drawRotation -= 1.57f;
        //                    origin = Vector2.Zero;
        //                }
        //            }
        //            else if (info.drawPlayer.direction == -1)
        //            {
        //                origin = new Vector2(Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width, Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Height);
        //                offsetX1 -= Main.itemTexture[info.drawPlayer.inventory[info.drawPlayer.selectedItem].type].Width;
        //            }
        //            Vector2 holdoutOrigin = Vector2.Zero;
        //            ItemLoader.HoldoutOrigin(info.drawPlayer, ref holdoutOrigin);
        //            drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X + origin.X + offsetX1), (int)(info.itemLocation.Y - Main.screenPosition.Y + offsetY));
        //            drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
        //            origin += holdoutOrigin;
        //            Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, Item.scale, info.spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<EnchantedDye>()) });
        //            return;
        //        }
        //        var spriteEffects = (SpriteEffects)(player.gravDir != 1f ? player.direction != 1 ? 3 : 2 : player.direction != 1 ? 1 : 0);
        //        var offset = new Vector2(texture.Width / 2, texture.Height / 2);
        //        Vector2 holdoutOffset = Item.modItem.HoldoutOffset().GetValueOrDefault(new Vector2(10f, 0f)) * player.gravDir;
        //        int offsetX = (int)holdoutOffset.X;
        //        offset.Y += holdoutOffset.Y;
        //        origin = player.direction == -1 ? new Vector2(texture.Width + offsetX, texture.Height / 2) : new Vector2(-offsetX, texture.Height / 2);
        //        drawCoordinates = new Vector2((int)(player.itemLocation.X - Main.screenPosition.X + offset.X), (int)(player.itemLocation.Y - Main.screenPosition.Y + offset.Y));
        //        drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
        //        drawRotation = player.itemRotation;
        //        Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, Item.scale, spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<EnchantedDye>()) });
        //        return;
        //    }
        //    if (player.gravDir == -1f)
        //    {
        //        drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
        //        drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
        //        drawRotation = player.itemRotation;
        //        origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, 0f);
        //        Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, Item.scale, info.spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<EnchantedDye>()) });
        //        return;
        //    }
        //    drawCoordinates = new Vector2((int)(info.itemLocation.X - Main.screenPosition.X), (int)(info.itemLocation.Y - Main.screenPosition.Y));
        //    drawFrame = new Rectangle(0, 0, texture.Width, texture.Height);
        //    drawRotation = player.itemRotation;
        //    origin = new Vector2(texture.Width * 0.5f - texture.Width * 0.5f * player.direction, texture.Height);
        //    Main.playerDrawData.Add(new DrawData(texture, drawCoordinates, drawFrame, new Color(255, 255, 255, 255), drawRotation, origin, Item.scale, info.spriteEffects, 0) { shader = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<EnchantedDye>()) });
        //}

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CosmicEnergy>()
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.DemonAltar)
                .Register();




            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.HellstoneBar, 15)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}