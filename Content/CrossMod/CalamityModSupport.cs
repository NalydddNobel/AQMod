using Aequus.Common;
using Aequus.Common.Utilities;
using Aequus.Content.Necromancy;
using Aequus.Items;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class CalamityModSupport : IAddRecipes, ILoadBefore
    {
        private static Mod calamityMod;
        public static Mod CalamityMod => calamityMod;
        public static Mod CatalystMod { get; private set; }

        Type ILoadBefore.LoadBefore => typeof(NecromancyDatabase);

        void ILoadable.Load(Mod mod)
        {
        }

        void IAddRecipes.AddRecipes(Aequus aequus)
        {
            calamityMod = null;
            CatalystMod = null;
            if (ModLoader.TryGetMod("CalamityMod", out calamityMod))
            {
                new DatabaseBuilder<GhostInfo>(NecromancyDatabase.NPCs, calamityMod, NPCID.Search)
                    .TryAddModEntry("AeroSlime", GhostInfo.One)
                    .TryAddModEntry("CrawlerAmber", GhostInfo.One)
                    .TryAddModEntry("CrawlerAmethyst", GhostInfo.One)
                    .TryAddModEntry("CrawlerDiamond", GhostInfo.One)
                    .TryAddModEntry("CrawlerEmerald", GhostInfo.One)
                    .TryAddModEntry("CrawlerRuby", GhostInfo.One)
                    .TryAddModEntry("CrawlerSapphire", GhostInfo.One)
                    .TryAddModEntry("CrawlerTopaz", GhostInfo.One)
                    .TryAddModEntry("AquaticUrchin", GhostInfo.One)
                    .TryAddModEntry("BoxJellyfish", GhostInfo.One)
                    .TryAddModEntry("CalamityEye", GhostInfo.Two)
                    .TryAddModEntry("Catfish", GhostInfo.One)
                    .TryAddModEntry("Clam", GhostInfo.One)
                    .TryAddModEntry("Cnidrion", GhostInfo.Two)
                    .TryAddModEntry("CosmicElemental", GhostInfo.Two)
                    .TryAddModEntry("CrimulanBlightSlime", GhostInfo.One)
                    .TryAddModEntry("EbonianBlightSlime", GhostInfo.One)
                    .TryAddModEntry("Cuttlefish", GhostInfo.One)
                    .TryAddModEntry("DespairStone", GhostInfo.Two)
                    .TryAddModEntry("DevilFish", GhostInfo.Three)
                    .TryAddModEntry("DevilFishAlt", GhostInfo.Three)
                    .TryAddModEntry("EutrophicRay", GhostInfo.One)
                    .TryAddModEntry("Flounder", GhostInfo.One)
                    .TryAddModEntry("Frogfish", GhostInfo.One)
                    .TryAddModEntry("GhostBell", GhostInfo.One)
                    .TryAddModEntry("GiantSquid", GhostInfo.Two)
                    .TryAddModEntry("Gnasher", GhostInfo.One)
                    .TryAddModEntry("HeatSpirit", GhostInfo.Two)
                    .TryAddModEntry("MorayEel", GhostInfo.One)
                    .TryAddModEntry("PrismBack", GhostInfo.One)
                    .TryAddModEntry("Rotdog", GhostInfo.One)
                    .TryAddModEntry("Scryllar", GhostInfo.Two)
                    .TryAddModEntry("SeaFloaty", GhostInfo.One)
                    .TryAddModEntry("SeaUrchin", GhostInfo.One)
                    .TryAddModEntry("Stormlion", GhostInfo.One)
                    .TryAddModEntry("SoulSlurper", GhostInfo.One)
                    .TryAddModEntry("Sunskater", GhostInfo.One)
                    .TryAddModEntry("ToxicMinnow", GhostInfo.One)
                    .TryAddModEntry("Trasher", GhostInfo.Two)
                    .TryAddModEntry("Viperfish", GhostInfo.One)
                    .TryAddModEntry("WulfrumDrone", GhostInfo.One)
                    .TryAddModEntry("WulfrumGyrator", GhostInfo.One)
                    .TryAddModEntry("WulfrumHovercraft", GhostInfo.One)
                    .TryAddModEntry("WulfrumRover", GhostInfo.One);

                AddPylonColors();
                AddRarityNames();
            }
            if (ModLoader.TryGetMod("CatalystMod", out var catalystMod))
            {
                CatalystMod = catalystMod;
                if (catalystMod.TryFind<ModRarity>("SuperbossRarity", out var rarity))
                {
                    AequusItem.RarityNames.Add(rarity.Type, "Mods.Aequus.ItemRarity.Catalyst_Crystal");
                }
            }
        }

        public static void AddPylonColors()
        {
            ModPylon pylon;
            if (CalamityMod.TryFind("AstralPylonTile", out pylon))
            {
                AequusTile.PylonColors.Add(new Point(pylon.Type, 0), Color.Violet);
            }
            if (CalamityMod.TryFind("CragsPylonTile", out pylon))
            {
                AequusTile.PylonColors.Add(new Point(pylon.Type, 0), Color.Orange);
            }
            if (CalamityMod.TryFind("SunkenPylonTile", out pylon))
            {
                AequusTile.PylonColors.Add(new Point(pylon.Type, 0), Color.Blue);
            }
            if (CalamityMod.TryFind("SulphurPylonTile", out pylon))
            {
                AequusTile.PylonColors.Add(new Point(pylon.Type, 0), Color.LimeGreen);
            }
        }
        public static void AddRarityNames()
        {
            ModRarity rarity;
        }

        void ILoadable.Unload()
        {
            calamityMod = null;
            CatalystMod = null;
        }
    }
}