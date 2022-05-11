using Aequus.Content.Necromancy;
using Aequus.Items.Misc;
using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Weapons.Summon.Necro
{
    public class NecromancyStaff : ModItem
    {
        public int npcSummon;
        public int drawBanner;

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(20);
            Item.shoot = ModContent.ProjectileType<NecromanticEnemySpawner>();
            Item.shootSpeed = 1f;
            Item.UseSound = SoundID.Item83;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 50);
            Item.mana = 150;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                string key = "NecromancyStaffTooltip";
                if (NPCID.Sets.CountsAsCritter[npcSummon])
                {
                    key = "NecromancyStaffTooltipFriendly";
                }
                tooltips.Insert(ItemTooltips.GetLineIndex(tooltips, "Material"),
                    new TooltipLine(Mod, "NecromancyTooltip", AequusText.GetText("Tooltips." + key, Lang.GetNPCNameValue(npcSummon))));

                ItemTooltips.ChangeVanillaLine(tooltips, "ItemName", (t) => t.Text = AequusText.GetText("Tooltips.NecromancyStaff", Lang.GetNPCNameValue(npcSummon)));
            }
            catch
            {

            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, Item.shoot, 0, 0f, player.whoAmI, npcSummon);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (drawBanner <= 0)
            {
                drawBanner = DetermineBanner();
            }

            DrawBanner(spriteBatch, AequusHelpers.InventoryItemGetCorner(position + new Vector2(4f, 4f) * Main.inventoryScale, frame, scale), drawColor, scale * 0.9f);
        }
        public int DetermineBanner()
        {
            int banner = Item.NPCtoBanner(npcSummon);
            if (banner == 0)
            {
                return ItemID.AnkhBanner;
            }
            return Item.BannerToItem(banner);
        }
        public void DrawBanner(SpriteBatch spriteBatch, Vector2 drawCoordinate, Color drawColor, float scale)
        {
            Main.instance.LoadItem(drawBanner);

            var bannerTexture = TextureAssets.Item[drawBanner].Value;
            var drawScale = new Vector2(1f, 1f);
            if (bannerTexture.Width < 12)
            {
                drawScale.X = 12 / bannerTexture.Width;
            }
            if (bannerTexture.Height < 28)
            {
                drawScale.Y = 28 / bannerTexture.Width;
            }
            drawScale *= scale;
            var origin = new Vector2(bannerTexture.Width, 4f);

            var frame = new Rectangle(0, 0, bannerTexture.Width, bannerTexture.Height);
            if (Aequus.HQ)
            {
                int segments = 8;
                int frameHeight = frame.Height / segments;
                for (int i = 0; i < segments; i++)
                {
                    float rotation = GetBannerRotation(Main.GlobalTimeWrappedHourly - i / (float)segments * 1.5f);
                    int frameY = frameHeight * i - 2;
                    if (i == segments - 1)
                    {
                        frameHeight += frame.Height - frameHeight * segments + 2;
                    }
                    spriteBatch.Draw(bannerTexture, drawCoordinate, new Rectangle(frame.X, Math.Max(frame.Y + frameY - 2, 0), frame.Width, frameHeight + 2),
                        drawColor, rotation, origin, drawScale, SpriteEffects.None, 0f);
                    drawCoordinate += (rotation + MathHelper.PiOver2).ToRotationVector2() * frameHeight * drawScale.Y;
                }
            }
            else
            {
                spriteBatch.Draw(bannerTexture, drawCoordinate, frame, drawColor, GetBannerRotation(Main.GlobalTimeWrappedHourly), origin, drawScale, SpriteEffects.None, 0f);
            }
        }
        public float GetBannerRotation(float time)
        {
            return -MathHelper.PiOver4 / 2f + AequusHelpers.Wave(time * 2f, -0.2f, 0.2f);
        }

        public override void AddRecipes()
        {
            HashSet<int> registeredBanners = new HashSet<int>()
            {
                Item.NPCtoBanner(NPCID.Zombie),
                Item.NPCtoBanner(NPCID.Mothron),
                Item.NPCtoBanner(NPCID.BloodNautilus),
            };

            NewRecipe(ItemID.ZombieBanner, NPCID.Zombie).Register();
            NewRecipe(ItemID.ZombieBanner, NPCID.MaggotZombie).AddIngredient(ItemID.Tombstone)
                .AddCondition(Recipe.Condition.InGraveyardBiome).Register();
            NewRecipe(ItemID.MothronBanner, NPCID.MothronSpawn).Register();
            NewRecipe(ItemID.MothronBanner, NPCID.Mothron).AddIngredient(ItemID.FragmentStardust, 18).Register();
            NewRecipe(ItemID.BloodNautilusBanner, NPCID.BloodNautilus).AddIngredient(ItemID.FragmentStardust, 18).Register();

            foreach (var ghost in NecromancyDatabase.NPCs)
            {
                if (ghost.Value.PowerNeeded > 0f)
                {
                    int banner = Item.NPCtoBanner(ghost.Key);
                    if (banner > 0 && !registeredBanners.Contains(banner))
                    {
                        NewRecipe(Item.BannerToItem(banner), ghost.Key).Register();
                        registeredBanners.Add(banner);
                    }
                }
            }
        }

        public Recipe NewRecipe(int bannerItem, int npc)
        {
            var r = CreateRecipe()
                .AddIngredient<UnenchantedStaff>()
                .AddIngredient(bannerItem)
                .AddIngredient<Hexoplasm>(12)
                .AddTile(TileID.MythrilAnvil);
            r.createItem.ModItem<NecromancyStaff>().npcSummon = npc;
            return r;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["npcSummon"] = npcSummon;
        }

        public override void LoadData(TagCompound tag)
        {
            npcSummon = tag.GetInt("npcSummon");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(npcSummon);
        }

        public override void NetReceive(BinaryReader reader)
        {
            npcSummon = reader.ReadInt32();
        }
    }
}