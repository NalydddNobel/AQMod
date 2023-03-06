using Aequus.Common.Recipes;
using Aequus.Content.ItemPrefixes;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        public void Load_Shimmer()
        {
            On_Item.GetShimmered += On_Item_GetShimmered;
            On_ShimmerTransforms.IsItemTransformLocked += On_ShimmerTransforms_IsItemTransformLocked;
        }

        private static bool On_ShimmerTransforms_IsItemTransformLocked(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            bool value = orig(type);
            if (!value)
                return false;

            if (AequusRecipes.ShimmerConditionOverride.TryGetValue(type, out var condition))
                return condition();

            return true;
        }

        private static void ShimmerThisMan(Item item)
        {
            if (item.stack > 0)
            {
                item.shimmerTime = 1f;
            }
            else
            {
                item.shimmerTime = 0f;
            }

            item.shimmerWet = true;
            item.wet = true;
            item.velocity *= 0.1f;
            if (Main.netMode == 0)
            {
                Item.ShimmerEffect(item.Center);
            }
            else
            {
                NetMessage.SendData(146, -1, -1, null, 0, (int)item.Center.X, (int)item.Center.Y);
                NetMessage.SendData(145, -1, -1, null, item.whoAmI, 1f);
            }
            AchievementsHelper.NotifyProgressionEvent(27);
        }

        private static void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item item)
        {
            if (item.prefix >= PrefixID.Count && PrefixLoader.GetPrefix(item.prefix) is AequusPrefix prefix)
            {
                if (prefix.Shimmerable)
                {
                    int oldStack = item.stack;
                    item.SetDefaults(item.netID);
                    item.stack = oldStack;
                    item.prefix = 0;
                    item.shimmered = true;

                    ShimmerThisMan(item);
                    return;
                }
            }
            orig(item);
        }
    }
}