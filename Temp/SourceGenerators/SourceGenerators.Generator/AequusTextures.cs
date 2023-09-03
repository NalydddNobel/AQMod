using System.Runtime.CompilerServices;
using Terraria.ModLoader;
using Aequus.Common;

namespace Aequus {
    /// <summary>(Amt Textures: 22)</summary>
    [CompilerGenerated]
    public partial class AequusTextures : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var f in GetType().GetFields()) {
                ((TextureAsset)f.GetValue(this))?.Unload();
            }
        }

        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/Shimmer/Balloon</summary>
        public static readonly TextureAsset Balloon_Shimmer = new("Aequus/TownNPCs/SkyMerchant/Shimmer/Balloon");
        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/Balloon</summary>
        public static readonly TextureAsset Balloon_SkyMerchant = new("Aequus/TownNPCs/SkyMerchant/Balloon");
        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/Shimmer/Basket</summary>
        public static readonly TextureAsset Basket_Shimmer = new("Aequus/TownNPCs/SkyMerchant/Shimmer/Basket");
        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/Basket</summary>
        public static readonly TextureAsset Basket_SkyMerchant = new("Aequus/TownNPCs/SkyMerchant/Basket");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Informational/Calendar/Calendar</summary>
        public static readonly TextureAsset Calendar = new("Aequus/Items/Equipment/Accessories/Informational/Calendar/Calendar");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Informational/Calendar/CalendarInfoDisplay</summary>
        public static readonly TextureAsset CalendarInfoDisplay = new("Aequus/Items/Equipment/Accessories/Informational/Calendar/CalendarInfoDisplay");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Informational/Calendar/CalendarTile</summary>
        public static readonly TextureAsset CalendarTile = new("Aequus/Items/Equipment/Accessories/Informational/Calendar/CalendarTile");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/Chain</summary>
        public static readonly TextureAsset Chain = new("Aequus/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/Chain");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield</summary>
        public static readonly TextureAsset FlashwayShield = new("Aequus/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield_Shield</summary>
        public static readonly TextureAsset FlashwayShield_Shield = new("Aequus/Items/Equipment/Accessories/Movement/FlashwayShield/FlashwayShield_Shield");
        /// <summary>Full Path: Aequus/icon</summary>
        public static readonly TextureAsset icon = new("Aequus/icon");
        /// <summary>Full Path: Aequus/icon_small</summary>
        public static readonly TextureAsset icon_small = new("Aequus/icon_small");
        /// <summary>Full Path: Aequus/icon_workshop</summary>
        public static readonly TextureAsset icon_workshop = new("Aequus/icon_workshop");
        /// <summary>Full Path: Aequus/Items/Potions/NeutronYogurt/NeutronYogurt</summary>
        public static readonly TextureAsset NeutronYogurt = new("Aequus/Items/Potions/NeutronYogurt/NeutronYogurt");
        /// <summary>Full Path: Aequus/Items/Potions/NeutronYogurt/NeutronYogurtBuff</summary>
        public static readonly TextureAsset NeutronYogurtBuff = new("Aequus/Items/Potions/NeutronYogurt/NeutronYogurtBuff");
        /// <summary>Full Path: Aequus/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/SkyHunterCrossbow</summary>
        public static readonly TextureAsset SkyHunterCrossbow = new("Aequus/Items/Weapons/Ranged/Bows/SkyHunterCrossbow/SkyHunterCrossbow");
        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/SkyMerchant</summary>
        public static readonly TextureAsset SkyMerchant = new("Aequus/TownNPCs/SkyMerchant/SkyMerchant");
        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/SkyMerchant_Aiming</summary>
        public static readonly TextureAsset SkyMerchant_Aiming = new("Aequus/TownNPCs/SkyMerchant/SkyMerchant_Aiming");
        /// <summary>Full Path: Aequus/TownNPCs/SkyMerchant/SkyMerchant_Head</summary>
        public static readonly TextureAsset SkyMerchant_Head = new("Aequus/TownNPCs/SkyMerchant/SkyMerchant_Head");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon</summary>
        public static readonly TextureAsset SlimyBlueBalloon = new("Aequus/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon_Balloon</summary>
        public static readonly TextureAsset SlimyBlueBalloon_Balloon = new("Aequus/Items/Equipment/Accessories/Movement/SlimyBlueBalloon/SlimyBlueBalloon_Balloon");
        /// <summary>Full Path: Aequus/Items/Equipment/Accessories/Movement/WeightedHorseshoe/WeightedHorseshoe</summary>
        public static readonly TextureAsset WeightedHorseshoe = new("Aequus/Items/Equipment/Accessories/Movement/WeightedHorseshoe/WeightedHorseshoe");
    }
}