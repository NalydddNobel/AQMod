using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;

namespace Aequus.Content.UI.PyramidOffering {
    public record struct OfferingInfo {
        public int ItemID;
        public string DescriptionKey;
        public Color AuraColor;
        private Func<Player, int, int, bool> acceptOffer;

        public OfferingInfo(int itemID, string descriptionKey, Color auraColor, Func<Player, int, int, bool> AcceptOffer) {
            ItemID = itemID;
            DescriptionKey = descriptionKey;
            AuraColor = auraColor;
            acceptOffer = AcceptOffer;
            Language.GetOrRegister(descriptionKey);
        }

        public bool AcceptOffer(Player player, int i, int j) {
            return acceptOffer(player, i, j);
        }
    }
}