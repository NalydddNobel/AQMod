using AQMod.Common;
using AQMod.Items.Accessories.Healing;
using AQMod.Items.Materials;
using AQMod.Items.Materials.Energies;
using AQMod.Items.Misc;
using AQMod.Items.Misc.QuestFish;
using AQMod.Items.Placeable.Nature;
using AQMod.Items.Potions;
using AQMod.Items.Tools.Fishing.Bait;
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
            if (Main.rand.NextBool(8))
            {
                return ModContent.ItemType<Blobfish>();
            }
            else if (Main.rand.NextBool(8))
            {
                return ModContent.ItemType<Nessie>();
            }
            else if (Main.rand.NextBool(9))
            {
                return ModContent.ItemType<UltraEel>();
            }
            else if (WorldDefeats.DownedStarite && Main.rand.NextBool(8))
            {
                return ModContent.ItemType<LightMatter>();
            }
            else if (WorldDefeats.DownedStarite && Main.rand.NextBool(8))
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
        private bool TryPool_BloodMoon(ref int newFish)
        {
            if (Main.rand.NextBool(25))
            {
                newFish = ModContent.ItemType<BloodPlasma>();
                return true;
            }
            if (Main.rand.NextBool(30))
            {
                newFish = ModContent.ItemType<PalePufferfish>();
                return true;
            }
            if (Main.rand.NextBool(30))
            {
                newFish = ModContent.ItemType<VampireSquid>();
                return true;
            }
            return false;
        }
        private bool TryPool_Corruption(ref int newFish)
        {
            if (!Main.dayTime && Main.rand.NextBool(4))
            {
                newFish = ModContent.ItemType<Fizzler>();
                return true;
            }
            if (NPC.downedBoss2 && Main.rand.NextBool(4))
            {
                newFish = ModContent.ItemType<Depthscale>();
                return true;
            }
            return false;
        }
        private bool TryPool_Crimson(ref int newFish)
        {
            if (NPC.downedBoss2 && Main.rand.NextBool(4))
            {
                newFish = ModContent.ItemType<Fleshscale>();
                return true;
            }
            return false;
        }
        private bool TryPool_Honey(ref int newFish)
        {
            if (Main.rand.NextBool(4))
            {
                newFish = ModContent.ItemType<Combfish>();
                return true;
            }
            if (Main.rand.NextBool(6))
            {
                newFish = ModContent.ItemType<LarvaEel>();
                return true;
            }
            return false;
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
                        if (biomes.zoneGlimmerEvent)
                        {
                            caughtType = Pool_GlimmerEvent();
                            junk = false;
                            return;
                        }
                        if (Main.bloodMoon && TryPool_BloodMoon(ref caughtType))
                        {
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
                    if (player.ZoneCorrupt && TryPool_Corruption(ref caughtType))
                    {
                        return;
                    }
                    if (player.ZoneCrimson && TryPool_Crimson(ref caughtType))
                    {
                        return;
                    }
                }
            }
            else if (liquidType == Tile.Liquid_Honey)
            {
                TryPool_Honey(ref caughtType);
            }
        }
    }
}
