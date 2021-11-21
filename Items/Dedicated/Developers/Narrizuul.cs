using AQMod.Assets.Graphics;
using AQMod.Assets.Graphics.SceneLayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dedicated.Developers
{
    public class Narrizuul : ModItem, IDedicatedItem, ICustomPickupText
    {
        public override void SetStaticDefaults()
        {
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 80;
            item.height = 80;
            item.damage = 777;
            item.knockBack = 7.77f;
            item.crit = 3;
            item.magic = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 7;
            item.useAnimation = 14;
            item.rare = ItemRarityID.Purple;
            item.shootSpeed = 27.77f;
            item.autoReuse = true;
            item.noMelee = true;
            item.UseSound = SoundID.Item1;
            item.value = Item.sellPrice(gold: 50);
            item.mana = 7;
            item.shoot = ModContent.ProjectileType<Projectiles.Magic.Narrizuul>();
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                DedicatedItemTooltips.DrawNarrizuulText(line);
                return false;
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver4 * 0.5f), type, damage, knockBack, player.whoAmI); // kinda lazy so why not?
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(-MathHelper.PiOver4 * 0.5f), type, damage, knockBack, player.whoAmI);
            return true;
        }

        Color IDedicatedItem.DedicatedItemColor => new Color(160, 80, 250, 255);
        IDedicationType IDedicatedItem.DedicationType => new ContributorDedication();

        bool ICustomPickupText.OnSpawnText(Item newItem, int stack, bool noStack, bool longText)
        {
            if (Main.showItemText && Main.netMode != NetmodeID.Server)
            {
                CustomPickupTextLayer.NewText(new NarrizuulPickupDrawObject(
                    "Mods.AQMod.ItemName.Narrizuul",
                    Main.player[Main.myPlayer].Center + new Vector2(0f, -Main.player[Main.myPlayer].height),
                    new Vector2(0f, -10f),
                    new Color(255, 255, 255, 255), 0f, 0.125f, 120));
            }

            return true;
        }
    }
}