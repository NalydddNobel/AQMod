using Aequus.Items;
using Aequus.Items.Misc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.CarpenterBounties
{
    public class CarpenterBounty
    {
        public int Type { get; internal set; }
        public List<Step> steps;
        public Mod Mod { get; private set; }
        public string Name { get; private set; }
        public int ItemReward;
        public int ItemStack;
        private Func<bool> bountyAvailable;

        public virtual string LanguageKey => $"Mods.{Mod.Name}.CarpenterBounty.{Name}";

        public CarpenterBounty(Mod mod, string name)
        {
            Mod = mod;
            Name = name;
            steps = new List<Step>();
        }

        internal CarpenterBounty(string name) : this(Aequus.Instance, name)
        {
        }

        public string FullName => $"{Mod.Name}/{Name}";

        public CarpenterBounty SetReward(int itemID, int stack = 1)
        {
            ItemReward = itemID;
            ItemStack = stack;
            return this;
        }
        public CarpenterBounty SetReward<T>(int stack = 1) where T : ModItem
        {
            return SetReward(ModContent.ItemType<T>(), stack);
        }

        public CarpenterBounty AddStep(Step step)
        {
            steps.Add(step);
            return this;
        }

        public CarpenterBounty SetAvailability(Func<bool> isAvailable)
        {
            bountyAvailable = isAvailable;
            return this;
        }

        public int Register()
        {
            return CarpenterSystem.RegisterBounty(this);
        }

        public Item ProvideBountyRewardItem()
        {
            var item = AequusItem.SetDefaults(ItemReward);
            item.stack = ItemStack;
            return item;
        }

        public virtual void OnCompleteBounty(Player player, NPC npc)
        {
            var reward = ProvideBountyRewardItem();
            player.QuickSpawnClonedItem(npc.GetSource_GiftOrReward(), reward, reward.stack);
        }

        public bool IsBountyAvailable()
        {
            return bountyAvailable?.Invoke() != false;
        }

        public CarpenterBountyItem ProvidePortableBounty()
        {
            var item = AequusItem.SetDefaults<CarpenterBountyItem>();
            var bounty = item.ModItem<CarpenterBountyItem>();
            bounty.BountyFullName = FullName;
            return bounty;
        }

        public StepResult CheckConditions(StepInfo info)
        {
            var result = new StepResult();
            foreach (var step in steps)
            {
                step.Initialize(info);
            }
            foreach (var step in steps)
            {
                result = step.GetResult(this, info);
                if (!result.success)
                    break;
            }
            return result;
        }

        public static Rectangle GetSurroundings(StepInfo info, IEnumerable<List<Point>> pointsList)
        {
            var resultRectangle = new Rectangle(info.Width, info.Height, 1, 1);
            foreach (var points in pointsList)
            {
                foreach (var w in points)
                {
                    resultRectangle.X = Math.Min(resultRectangle.X, w.X);
                    resultRectangle.Y = Math.Min(resultRectangle.Y, w.Y);
                }
                foreach (var w in points)
                {
                    resultRectangle.Width = Math.Max(w.X - resultRectangle.X, resultRectangle.Width);
                    resultRectangle.Height = Math.Max(w.Y - resultRectangle.Y, resultRectangle.Height);
                }
            }
            return resultRectangle;
        }
        public static Rectangle GetSurroundings(StepInfo info, List<Point> points)
        {
            var resultRectangle = new Rectangle(info.Width, info.Height, 1, 1);
            foreach (var w in points)
            {
                resultRectangle.X = Math.Min(resultRectangle.X, w.X);
                resultRectangle.Y = Math.Min(resultRectangle.Y, w.Y);
            }
            foreach (var w in points)
            {
                resultRectangle.Width = Math.Max(w.X - resultRectangle.X, resultRectangle.Width);
                resultRectangle.Height = Math.Max(w.Y - resultRectangle.Y, resultRectangle.Height);
            }
            return resultRectangle;
        }
    }
}