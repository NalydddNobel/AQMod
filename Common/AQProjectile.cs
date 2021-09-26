using AQMod.Items.Fishing.Bait;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Common
{
    public class AQProjectile : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            if (projectile.aiStyle == 61)
            {
                if (projectile.ai[1] > 0f && projectile.localAI[1] >= 0f) // on catch effects
                {
                    //var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    //if (aQPlayer.PopperType > 0)
                    //{
                    //    var item = new Item();
                    //    item.SetDefaults(aQPlayer.PopperType);
                    //    ((PopperBait)item.modItem).PopperEffects(Main.player[projectile.owner], aQPlayer, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
                    //}
                }
                else if (projectile.ai[0] <= 0f && projectile.wet && projectile.rotation != 0f) // When it enters the water
                {
                    var aQPlayer = Main.player[projectile.owner].GetModPlayer<AQPlayer>();
                    if (aQPlayer.PopperType > 0)
                    {
                        var item = new Item();
                        item.SetDefaults(aQPlayer.PopperType);
                        ((PopperBait)item.modItem).PopperEffects(Main.player[projectile.owner], aQPlayer, projectile, Framing.GetTileSafely(projectile.Center.ToTileCoordinates()));
                    }
                }
            }
            return true;
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (projectile.aiStyle == 61)
            {
                if (projectile.ai[0] >= 1f && projectile.ai[1] > 0f && Main.myPlayer == projectile.owner)
                {
                    var plr = Main.player[projectile.owner];
                    var aQPlr = plr.GetModPlayer<AQPlayer>();
                    var item = new Item();
                    item.SetDefaults((int)projectile.ai[1]);
                    if (aQPlr.goldSeal && item.value > Item.sellPrice(gold: 1))
                    {
                        int sonarPotionIndex = plr.FindBuffIndex(BuffID.Sonar);
                        if (sonarPotionIndex != -1)
                        {
                            plr.buffTime[sonarPotionIndex] += 6000;
                        }
                        else
                        {
                            plr.AddBuff(BuffID.Sonar, 6000);
                        }
                    }
                    if (aQPlr.silverSeal)
                    {
                        int fishingPotionIndex = plr.FindBuffIndex(BuffID.Fishing);
                        if (fishingPotionIndex != -1)
                        {
                            plr.buffTime[fishingPotionIndex] += 600;
                        }
                        else
                        {
                            plr.AddBuff(BuffID.Fishing, 600);
                        }
                    }
                    switch (projectile.ai[1])
                    {
                        case ItemID.CorruptFishingCrate:
                        case ItemID.CrimsonFishingCrate:
                        case ItemID.DungeonFishingCrate:
                        case ItemID.FloatingIslandFishingCrate:
                        case ItemID.HallowedFishingCrate:
                        case ItemID.JungleFishingCrate:
                        case ItemID.GoldenCrate:
                        case ItemID.IronCrate:
                        case ItemID.WoodenCrate:
                        if (aQPlr.copperSeal)
                        {
                            int cratePotionIndex = plr.FindBuffIndex(BuffID.Crate);
                            if (cratePotionIndex != -1)
                            {
                                plr.buffTime[cratePotionIndex] += 1800;
                            }
                            else
                            {
                                plr.AddBuff(BuffID.Crate, 1800);
                            }
                        }
                        break;

                        default:
                        if (projectile.ai[1] > Main.maxItemTypes)
                        {
                            var modItem = ItemLoader.GetItem((int)projectile.ai[1]);
                            if (modItem.CanRightClick())
                                goto case ItemID.WoodenCrate;
                        }
                        break;
                    }
                }
            }
        }
    }
}