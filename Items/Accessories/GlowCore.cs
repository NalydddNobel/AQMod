using Aequus;
using Aequus.Common;
using Aequus.Items.Consumables.Roulettes;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    public class GlowCore : DyeableAccessory
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
            LootPools.Chests.Add(new LootPools.Chests.FrontChestLoot(Type, 8), ChestTypes.Gold);
            LootPools.Chests.Add(new LootPools.Chests.FrontChestLoot(Type, 8), ChestTypes.deadMans, TileID.Containers2);
            LootPools.Bags.WoodenCrate_Secondary.Add(Type);
            GoldenRoulette.Table.Add(Type);
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            byte value = color;
            if (color != 255)
            {
                value++;
            }
            player.GetModPlayer<GlowCorePlayer>().glowCore = value;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            try
            {
                var texture = TextureAssets.Item[Type].Value;

                AequusPlayer.TeamContext = Main.LocalPlayer.team;
                var coloring = DyeColor();
                AequusPlayer.TeamContext = 0;

                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    Main.spriteBatch.Draw(texture, position + v * scale * 2f, frame, coloring.UseA(0) * 0.7f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(texture, position, frame, coloring, 0f, origin, scale, SpriteEffects.None, 0f);
            }
            catch
            {
                return true;
            }
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            try
            {
                var texture = TextureAssets.Item[Type].Value;
                var frame = texture.Frame();
                var origin = frame.Size() / 2f;
                var drawCoordinates = Item.position - Main.screenPosition + origin + new Vector2(Item.width / 2 - origin.X, Item.height - frame.Height);
                var itemOrigin = frame.Size() / 2f;

                if (Item.playerIndexTheItemIsReservedFor >= 0 && Item.playerIndexTheItemIsReservedFor != 255)
                {
                    AequusPlayer.TeamContext = Main.player[Item.playerIndexTheItemIsReservedFor].team;
                }
                else
                {
                    AequusPlayer.TeamContext = 0;
                }

                var coloring = DyeColor();
                AequusPlayer.TeamContext = 0;

                foreach (var v in AequusHelpers.CircularVector(4))
                {
                    Main.spriteBatch.Draw(texture, drawCoordinates + v * scale * 2f, frame, coloring.UseA(0) * 0.3f, rotation, origin, scale, SpriteEffects.None, 0f);
                }

                Main.spriteBatch.Draw(texture, drawCoordinates, frame, coloring, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            catch
            {
                return true;
            }
            return false;
        }

        public override void AddRecipes()
        {
            ColorRecipes<GlowCore>();
        }

        public static void AddLight(Entity entity, byte glowCore, int playerTeam = -1)
        {
            if (glowCore != 255)
            {
                glowCore--;
            }
            Lighting.AddLight(entity.Center, DyeColor(glowCore).ToVector3() * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.7f, 0.9f));
        }
    }

    public class GlowCorePlayer : ModPlayer
    {
        public byte glowCore;

        public override void ResetEffects()
        {
            glowCore = 0;
        }

        public override void PostUpdateEquips()
        {
            if (glowCore > 0)
            {
                GlowCore.AddLight(Player, glowCore);
            }
        }
    }

    public class GlowCoreProjectile : GlobalProjectile
    {
        public override void PostAI(Projectile projectile)
        {
            if (projectile.friendly && projectile.owner >= 0 && projectile.owner != 255)
            {
                var glowCore = Main.player[projectile.owner].GetModPlayer<GlowCorePlayer>();
                if (glowCore.glowCore > 0)
                {
                    AequusPlayer.TeamContext = Main.player[projectile.owner].team;
                    GlowCore.AddLight(projectile, glowCore.glowCore);
                    AequusPlayer.TeamContext = 0;
                }
            }
        }
    }
}