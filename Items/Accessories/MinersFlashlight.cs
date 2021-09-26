using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories
{
    public class MinersFlashlight : ModItem
    {
        private const int FlashlightReps = 16;
        private const float FlashlightLight = 0.75f;
        private const float FlashlightMovement = 32f;

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(gold: 5);
            item.mana = 15;
            item.accessory = true;
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 255);

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (hideVisual)
                return;
            var aQPlayer = player.GetModPlayer<AQPlayer>();
            aQPlayer.spelunkerEquipTimer++;
            if (aQPlayer.spelunkerEquipTimer > 60)
            {
                if (player.CheckMana(item, -1, true))
                {
                    player.manaRegenDelay = (int)player.maxRegenDelay;
                    aQPlayer.spelunkerEquipTimer = 0;
                }
                else
                {
                    return;
                }
            }
            if (player.statMana < player.GetManaCost(item))
                return;
            var toMouseNormal = Vector2.Normalize(Main.MouseWorld - player.Center);
            var center = player.Center;
            int positionIndexOffset = 0;
            int reps = FlashlightReps;
            float lightPower = FlashlightLight;
            float mult = 1f / reps;
            for (int i = 0; i < reps; i++)
            {
                var lightSpot = center + toMouseNormal * (i - positionIndexOffset) * FlashlightMovement;
                int x3 = (int)lightSpot.X / 16;
                int y3 = (int)lightSpot.Y / 16;
                if (x3 >= 0 && x3 <= Main.maxTilesX && y3 >= 0 && y3 <= Main.maxTilesY)
                {
                    var tile = Framing.GetTileSafely(x3, y3);
                    if (tile.active() && Main.tileBlockLight[tile.type])
                    {
                        i++;
                        positionIndexOffset++;
                    }
                }
                var light = new Vector3(lightPower, lightPower, lightPower) * (mult * (reps - i));
                Lighting.AddLight(lightSpot, light);
                float sizePrecise = (float)Math.Pow(lightPower, 2) * 5f;
                int size = (int)sizePrecise;
                int sizeOff = size / 2;
                int x = x3 - sizeOff;
                int y = y3 - sizeOff;
                var rectangle = new Rectangle(x, y, size + 1, size + 1);
                for (int x2 = x; x2 <= x + size; x2++)
                {
                    if (x2 < 0 || x2 > Main.maxTilesX)
                        continue;
                    for (int y2 = y; y2 <= y + size; y2++)
                    {
                        if (y2 < 0 || y2 > Main.maxTilesY)
                            continue;
                        var tile = Framing.GetTileSafely(x2, y2);
                        if (Main.tileValue[tile.type] > 0)
                        {
                            int shineRarity = (int)(2000 - Main.tileValue[tile.type] - sizePrecise * 20);
                            if (shineRarity <= 1 || Main.rand.NextBool((int)(shineRarity * 0.075f)))
                            {
                                int d = Dust.NewDust(new Vector2(x2 * 16f, y2 * 16f), 16, 16, 204);
                                Main.dust[d].velocity *= 0.05f;
                            }
                        }
                    }
                }
            }
        }
    }
}