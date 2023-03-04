using Aequus.Content.Town.CarpenterNPC.Misc;
using Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps;
using Aequus.Items;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties
{
    public class CarpenterBounty
    {
        public int Type { get; internal set; }
        public List<Step> steps;
        public Mod Mod { get; private set; }
        public string Name { get; private set; }
        public int ItemReward;
        public int ItemStack;
        public List<int> MiscUnlocks;
        private Func<bool> bountyAvailable;
        public int BuildingBuff;
        public int BountyNPC;
        public float Progression;

        public virtual string LanguageKey => $"Mods.{Mod.Name}.CarpenterBounty.{Name}";
        public virtual string DisplayName => Language.GetTextValue($"{LanguageKey}");
        public virtual string Description => Language.GetTextValue($"{LanguageKey}.Description");
        public virtual string Icon => $"{Mod.Name}/Content/Carpentery/Bounties/Icons/{Name}";

        public CarpenterBounty(Mod mod, string name)
        {
            Mod = mod;
            Name = name;
            MiscUnlocks = new List<int>();
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

        public CarpenterBounty SetBuff<T>() where T : ModBuff
        {
            BuildingBuff = ModContent.BuffType<T>();
            return this;
        }

        public CarpenterBounty SetNPC(int npc)
        {
            BountyNPC = npc;
            return this;
        }
        public CarpenterBounty SetNPC<T>() where T : ModNPC
        {
            return SetNPC(ModContent.NPCType<T>());
        }

        public CarpenterBounty SetProgression(float progression)
        {
            Progression = progression;
            return this;
        }

        public CarpenterBounty AddMiscUnlock(int itemID)
        {
            MiscUnlocks.Add(itemID);
            return this;
        }
        public CarpenterBounty AddMiscUnlock<T>() where T : ModItem
        {
            return AddMiscUnlock(ModContent.ItemType<T>());
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

        public List<Item> ProvideBountyRewardItems()
        {
            var item = new List<Item>() { AequusItem.SetDefaults(ItemReward) };
            foreach (var misc in MiscUnlocks)
            {
                item.Add(AequusItem.SetDefaults(misc));
            }
            return item;
        }

        public virtual void OnCompleteBounty(Player player, NPC npc)
        {
            var reward = ProvideBountyRewardItems()[0];
            player.QuickSpawnItem(npc.GetSource_GiftOrReward(), reward, reward.stack);
        }

        public bool IsNPCPresent()
        {
            if (BountyNPC > 0)
            {
                return NPC.AnyNPCs(BountyNPC);
            }
            return true;
        }

        public bool IsBountyAvailable()
        {
            return bountyAvailable?.Invoke() != false;
        }

        public int GetBountyNPCID()
        {
            return BountyNPC > 0 ? BountyNPC : ModContent.NPCType<Carpenter>();
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
            var result = new StepResult("");
            foreach (var step in steps)
            {
                step.Initialize(info);
            }
            foreach (var step in steps)
            {
                result.perStepResults.Add(step.GetResult(this, info));
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

        public List<string> StepsToString()
        {
            var l = new List<string>();
            foreach (var s in steps)
            {
                var text = s.GetStepText(this);
                if (string.IsNullOrEmpty(text))
                    continue;
                l.Add(Language.GetTextValue(text));
            }
            return l;
        }
    }
}