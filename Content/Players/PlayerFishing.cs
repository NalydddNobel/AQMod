using AQMod.Common;
using AQMod.Items.Accessories.Summon;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Misc;
using AQMod.Items.Misc.Bait;
using AQMod.Items.Placeable.CrabCrevice;
using AQMod.Items.Potions;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public sealed class PlayerFishing : ModPlayer
    {
        internal static class WorldLayers
        {
            public const int Space = 0;
            public const int Overworld = 1;
            public const int UndergroundLayer = 2;
            public const int CavernLayer = 3;
            public const int HellLayer = 4;
        }

        public int popperType;
        public int popperBaitPower;
        public int fishingLevel;

        private void FishingLevel_CheckPopper(Item bait)
        {
            if (bait.modItem is PopperBaitItem popper)
            {
                int popperPower = popper.GetExtraFishingPower(player, this);
                if (popperPower > 0)
                {
                    popperType = (ushort)bait.type;
                    popperBaitPower = (ushort)popperPower;
                }
                else
                {
                    popperType = 0;
                    popperBaitPower = 0;
                }
            }
            else
            {
                popperType = 0;
                popperBaitPower = 0;
            }
        }
        public override void GetFishingLevel(Item fishingRod, Item bait, ref int fishingLevel)
        {
            FishingLevel_CheckPopper(bait);
            fishingLevel += popperBaitPower;
            this.fishingLevel = fishingLevel;
        }

        private int Pool_GlimmerEvent()
        {
            if (Main.rand.NextBool(3))
            {
                return ModContent.ItemType<Blobfish>();
            }
            else if (Main.rand.NextBool(3))
            {
                return ModContent.ItemType<Nessie>();
            }
            else if (Main.rand.NextBool(3))
            {
                return ModContent.ItemType<UltraEel>();
            }
            else if (WorldDefeats.DownedStarite && Main.rand.NextBool(4))
            {
                return ModContent.ItemType<LightMatter>();
            }
            else if (WorldDefeats.DownedStarite && Main.rand.NextBool(4))
            {
                return ModContent.ItemType<CosmicEnergy>();
            }
            return ModContent.ItemType<Molite>();
        }
        private int Pool_CrabCrevice()
        {
            if (Main.rand.NextBool(8))
            {
                return ModContent.ItemType<CrustaciumBlob>();
            }
            else if (Main.rand.NextBool(8))
            {
                return ModContent.ItemType<CrustaciumOre>();
            }
            else if (Main.rand.NextBool(8))
            {
                return ModContent.ItemType<CrustaciumBar>();
            }
            else if (WorldDefeats.DownedCrabson && Main.rand.NextBool(8))
            {
                return ModContent.ItemType<AquaticEnergy>();
            }
            else if (Main.rand.NextBool(3))
            {
                return ModContent.ItemType<CrabShell>();
            }
            else if (Main.rand.NextBool(3))
            {
                return ModContent.ItemType<ExoticCoral>();
            }
            return ModContent.ItemType<SeaPickle>();
        }
        private bool TryCatchQuestFish(int questFish, ref int newFish)
        {
            if (questFish == ModContent.ItemType<WaterFisg>() && Main.rand.NextBool(25))
            {
                newFish = ModContent.ItemType<WaterFisg>();
                return true;
            }
            if (questFish == ModContent.ItemType<Crabdaughter>() && player.ZoneBeach && Main.rand.NextBool(8))
            {
                newFish = ModContent.ItemType<Crabdaughter>();
                return true;
            }
            return false;
        }
        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
        {
            if (junk)
            {
                return;
            }
            if (TryCatchQuestFish(questFish, ref caughtType))
            {
                return;
            }

            var biomes = player.Biomes();
            if (liquidType == Tile.Liquid_Water)
            {
                if (worldLayer <= WorldLayers.Overworld)
                {
                    if (!Main.dayTime)
                    {
                        if (biomes.zoneGlimmerEvent && Main.rand.NextBool())
                        {
                            caughtType = Pool_GlimmerEvent();
                            junk = false;
                            return;
                        }
                        if (Main.bloodMoon)
                        {
                            if (Main.rand.NextBool(25))
                            {
                                caughtType = ModContent.ItemType<BloodPlasma>();
                                return;
                            }
                            if (Main.rand.NextBool(30))
                            {
                                caughtType = ModContent.ItemType<PalePufferfish>();
                                return;
                            }
                            if (Main.rand.NextBool(30))
                            {
                                caughtType = ModContent.ItemType<VampireSquid>();
                                return;
                            }
                        }
                    }
                    if (player.ZoneBeach)
                    {
                        if (Main.rand.NextBool(15))
                        {
                            caughtType = ModContent.ItemType<Squyp>();
                            return;
                        }
                    }
                }
                else if (worldLayer < WorldLayers.HellLayer)
                {
                    if (biomes.zoneCrabCrevice)
                    {
                        caughtType = Pool_CrabCrevice();
                        junk = false;
                        return;
                    }
                }
                if (worldLayer < WorldLayers.HellLayer)
                {
                    if (player.ZoneCorrupt && Main.rand.NextBool(4))
                    {
                        if (!Main.dayTime && Main.rand.NextBool())
                        {
                            caughtType = ModContent.ItemType<Fizzler>();
                            return;
                        }
                        if (NPC.downedBoss2 && Main.rand.NextBool())
                        {
                            caughtType = ModContent.ItemType<Depthscale>();
                            return;
                        }
                    }
                    if (player.ZoneCrimson && Main.rand.NextBool(4))
                    {
                        if (NPC.downedBoss2 && Main.rand.NextBool())
                        {
                            caughtType = ModContent.ItemType<Fleshscale>();
                            return;
                        }
                    }
                }
            }
            else if (liquidType == Tile.Liquid_Honey)
            {
                if (Main.rand.NextBool(4))
                {
                    caughtType = ModContent.ItemType<Combfish>();
                }
                else if (Main.rand.NextBool(6))
                {
                    caughtType = ModContent.ItemType<LarvaEel>();
                }
            }
        }
    }
}
