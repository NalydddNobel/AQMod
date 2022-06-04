using Aequus.Common.Players;
using Aequus.Content.Necromancy;
using Aequus.Items.Misc;
using Aequus.Projectiles.Summon.Necro;
using Aequus.UI;
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

        public virtual int LifeUsed => 100;

        public NecromancyStaff()
        {
            npcSummon = NPCID.BlueSlime;
        }

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToNecromancy(20);
            Item.SetWeaponValues(1, 1f, 0);
            Item.shoot = ModContent.ProjectileType<NecromanticEnemySpawner>();
            Item.shootSpeed = 1f;
            Item.UseSound = SoundID.Item83;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.sellPrice(silver: 50);
        }

        public override bool AllowPrefix(int pre)
        {
            return !AequusItem.CritOnlyModifier.Contains(pre);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            try
            {
                tooltips.ItemName().Text = AequusText.GetText("Tooltips.NecromancyStaff", Lang.GetNPCNameValue(npcSummon));
                tooltips.UsesLife(this, LifeUsed);

                string key = NPCID.Sets.CountsAsCritter[npcSummon] ? "NecromancyStaffTooltipFriendly" : "NecromancyStaffTooltip";
                tooltips.PreTooltip(this, "NecromancyTooltip", "Tooltips." + key, Lang.GetNPCNameValue(npcSummon));

                int damage = 0;
                NPC npc = new NPC();
                npc.SetDefaults(npcSummon);
                damage = (int)(npc.damage * NecromancyNPC.GetDamageMultiplier(npc, npc.damage));
                tooltips.Find("Damage").Text = damage + Lang.tip[53].Value;
                tooltips.RemoveCritChanceModifier();
            }
            catch
            {
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (NecromancyDatabase.TryGetByNetID(npcSummon, NPCID.FromNetId(npcSummon), out var value))
            {
                var aequus = player.Aequus();
                return (aequus.ghostSlots + (value.SlotsUsed.GetValueOrDefault(1) * 2 - 1)) < aequus.ghostSlotsMax;
            }
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            player.LimitPointToPlayerReachableArea(ref position);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.GetModPlayer<SacrificingPlayer>().SacrificeLife(LifeUsed, LifeUsed / 8, reason: PlayerDeathReason.ByCustomReason(AequusText.GetText("Deaths.NecromancyStaffUsage", player.name, Lang.GetNPCName(npcSummon))));
            Projectile.NewProjectileDirect(source, position, Vector2.Zero, Item.shoot, Item.damage, 0f, player.whoAmI, npcSummon);
            return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (drawBanner <= 0)
            {
                drawBanner = DetermineBanner();
            }

            DrawBanner(spriteBatch, ItemSlotRenderer.InventoryItemGetCorner(position + new Vector2(4f, 4f) * Main.inventoryScale, frame, scale), drawColor, scale * 0.9f);
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

            foreach (var ghost in NecromancyDatabase.NPCs)
            {
                if (ghost.Value.PowerNeeded > 0f && ghost.Value.PowerNeeded <= 3f)
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