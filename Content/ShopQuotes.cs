using Aequus;
using Aequus.Content.CrossMod;
using Aequus.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content
{
    public class ShopQuotes : GlobalItem
    {
        public class NPCQuotes
        {
            public static Func<Color> DefaultColor => () => Colors.RarityBlue;

            private readonly int NPC;
            private Func<Color> getColor;
            public readonly Dictionary<int, string> ItemToQuote;

            internal NPCQuotes(int npc)
            {
                NPC = npc;
                getColor = DefaultColor;
                ItemToQuote = new Dictionary<int, string>();
            }

            public NPCQuotes AddQuote(string key, int item)
            {
                ItemToQuote.Add(item, key);
                return this;
            }
            public NPCQuotes AddQuote<T>(string key) where T : ModItem
            {
                return AddQuote(key, ModContent.ItemType<T>());
            }

            public NPCQuotes AddQuote(Mod mod, int item)
            {
                ItemToQuote.Add(item, KeyFromIDs(mod.Name, NPC, item));
                return this;
            }
            public NPCQuotes AddQuote<T>(Mod mod) where T : ModItem
            {
                return AddQuote(mod, ModContent.ItemType<T>());
            }

            internal NPCQuotes AddQuote(int item)
            {
                ItemToQuote.Add(item, KeyFromIDs("Aequus", NPC, item));
                return this;
            }
            internal NPCQuotes AddQuote<T>() where T : ModItem
            {
                return AddQuote(ModContent.ItemType<T>());
            }

            public NPCQuotes WithColor(Func<Color> color)
            {
                getColor = color;
                return this;
            }

            public NPCQuotes WithColor(Color color)
            {
                return WithColor(() => color);
            }

            public Color GetColor()
            {
                return getColor();
            }
        }

        public class QuoteDatabase : IModCallable
        {
            private Dictionary<int, NPCQuotes> database;

            public QuoteDatabase(bool init = true)
            {
                database = new Dictionary<int, NPCQuotes>();
                if (init)
                {
                    Initalize();
                }
            }

            internal void Initalize()
            {
                database = new Dictionary<int, NPCQuotes>()
                {
                    [NPCID.ArmsDealer] = new NPCQuotes(NPCID.ArmsDealer)
                    .WithColor(Color.Gray * 1.45f)
                    .AddQuote("LegacyDialog.66", ItemID.Minishark),

                    [NPCID.Cyborg] = new NPCQuotes(NPCID.Cyborg)
                    .WithColor(Color.Cyan * 1.5f)
                    .AddQuote(ItemID.RocketI)
                    .AddQuote(ItemID.RocketII)
                    .AddQuote(ItemID.RocketIII)
                    .AddQuote(ItemID.RocketIV)
                    .AddQuote(ItemID.DryRocket)
                    .AddQuote(ItemID.ProximityMineLauncher)
                    .AddQuote(ItemID.Nanites)
                    .AddQuote(ItemID.ClusterRocketI)
                    .AddQuote(ItemID.ClusterRocketII)
                    .AddQuote(ItemID.HiTekSunglasses)
                    .AddQuote(ItemID.NightVisionHelmet)
                    .AddQuote(ItemID.PortalGunStation)
                    .AddQuote(ItemID.EchoBlock)
                    .AddQuote(ItemID.SpectreGoggles),

                    [NPCID.Pirate] = new NPCQuotes(NPCID.Pirate)
                    .WithColor(Color.Orange * 1.2f)
                    .AddQuote(ItemID.Cannon)
                    .AddQuote(ItemID.Cannonball)
                    .AddQuote(ItemID.PirateHat)
                    .AddQuote(ItemID.PirateShirt)
                    .AddQuote(ItemID.PiratePants)
                    .AddQuote(ItemID.Sail)
                    .AddQuote(ItemID.ParrotCracker)
                    .AddQuote(ItemID.BunnyCannon)
                    .AddQuote(ItemID.ExplosiveBunny)
                    ,
                };
            }

            public NPCQuotes this[int npc]
            {
                get => database[npc];
            }

            public bool TryGetValue(int npc, out NPCQuotes quote)
            {
                return database.TryGetValue(npc, out quote);
            }

            /// <summary>
            /// Adds an NPC entry if one doesn't exist already
            /// </summary>
            /// <param name="npc"></param>
            /// <returns></returns>
            public NPCQuotes AddNPC(int npc)
            {
                if (TryGetValue(npc, out var quote))
                {
                    return quote;
                }
                database.Add(npc, new NPCQuotes(npc));
                return this[npc];
            }

            /// <summary>
            /// Adds shop quote data for an npc
            /// <para>Parameter 1: NPC Type (<see cref="int"/>)</para>
            /// <para>Parameter 2: Key (<see cref="string"/>)</para>
            /// <para>Parameter 2 (Alternative): Mod (<see cref="Mod"/>) Would generate a key which looks like: <code>Mods.MYMODNAME.Chat.(NPCKEY).ShopQuotes.(ITEMKEY)</code></para>
            /// <para>Parameter 3: Item Type (<see cref="int"/>) OR (<see cref="int"/>[])</para>
            /// <para>To make a town npc use a specific quote color:</para>
            /// <para>Parameter 1: NPC Type (<see cref="int"/>)</para>
            /// <para>Parameter 2: Color (<see cref="Color"/>)</para>
            /// <para>A successful mod call would look like:</para>
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, <see cref="Color"/>);</code> OR
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, this, <see cref="ItemID"/>);</code> OR
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, "This is text. I can also use language keys here.", <see cref="ItemID"/>);</code>
            /// <code>aequus.Call("AddShopQuote", <see cref="NPCID"/>, "This is text. I can also use language keys here.", new int[] { <see cref="ItemID"/>, });</code>
            /// <para>Please handle these mod calls in <see cref="Mod.PostSetupContent"/>.</para>
            /// </summary>
            /// <param name="aequus"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object HandleModCall(Aequus aequus, object[] args)
            {
                int npc = (int)args[1];

                AddNPC(npc);
                if (args[2] is Color color)
                {
                    this[npc].WithColor(color);
                    return IModCallable.Success;
                }
                int[] items = new int[1];
                if (args[3] is int[] arr)
                {
                    items = arr;
                }
                else
                {
                    items[0] = (int)args[3];
                }

                foreach (var item in items)
                {
                    if (args[2] is string quoteText)
                    {
                        this[npc].AddQuote(quoteText, item);
                    }
                    else if (args[2] is Mod mod)
                    {
                        this[npc].AddQuote(mod, item);
                    }
                }

                return IModCallable.Success;
            }

            void ILoadable.Load(Mod mod)
            {
            }

            void ILoadable.Unload()
            {
            }
        }

        public static QuoteDatabase Database { get; private set; }

        public override void Load()
        {
            Database = new QuoteDatabase(init: true);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!ClientConfig.Instance.NPCShopQuotes)
            {
                return;
            }

            if (item.isAShopItem && item.buy && item.tooltipContext == ItemSlot.Context.ShopItem && Main.LocalPlayer.talkNPC != -1)
            {
                try
                {
                    AddShopQuote(item, tooltips);
                }
                catch
                {
                }
            }
        }

        public void AddShopQuote(Item item, List<TooltipLine> tooltips)
        {
            var talkNPC = Main.npc[Main.LocalPlayer.talkNPC];

            if (talkNPC.type < Main.maxNPCTypes && !ClientConfig.Instance.OtherNPCShopQuotes)
            {
                return;
            }

            if (TryGetQuote(talkNPC, item, out string text, out var textColor))
            {
                string lineText = "         ";
                lineText += Language.GetTextValue(text);
                int index = tooltips.GetIndex("JourneyResearch");
                tooltips.Insert(index, new TooltipLine(Mod, "Fake", "_"));
                tooltips.Insert(index, new TooltipLine(Mod, "ShopQuote", lineText) { OverrideColor = textColor, });
            }
        }
        public bool TryGetQuote(NPC talkNPC, Item item, out string text, out Color color)
        {
            text = "";
            color = default(Color);
            if (Database.TryGetValue(talkNPC.type, out var value))
            {
                if (value.ItemToQuote.TryGetValue(item.type, out var itemQuote))
                {
                    text = itemQuote;
                    color = value.GetColor();
                    return true;
                }
            }
            return false;
        }

        public static void DrawShopQuote(string text, int x, int y, float rotation, Vector2 origin, Vector2 baseScale, Color color, NPC npc)
        {
            var chatBubbleFrame = Images.StatusBubble.Value.Frame(horizontalFrames: Images.StatusBubbleFramesX, frameX: 2);
            var chatBubbleScale = baseScale * 0.9f;
            chatBubbleScale.X *= 1.1f;
            var chatBubblePosition = new Vector2(x + chatBubbleFrame.Width / 2f * chatBubbleScale.X, y + chatBubbleFrame.Height / 2f * chatBubbleScale.Y);

            Main.spriteBatch.Draw(Images.StatusBubble.Value, chatBubblePosition + new Vector2(2f) * chatBubbleScale,
                chatBubbleFrame, Color.Black * 0.4f, 0f, chatBubbleFrame.Size() / 2f, chatBubbleScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Images.StatusBubble.Value, chatBubblePosition,
                chatBubbleFrame, Color.White, 0f, chatBubbleFrame.Size() / 2f, chatBubbleScale, SpriteEffects.None, 0f);

            var headTexture = TextureAssets.NpcHead[NPC.TypeToDefaultHeadIndex(npc.type)].Value;

            Main.spriteBatch.Draw(headTexture, chatBubblePosition,
                headTexture.Frame(), Color.White, 0f, headTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            var font = FontAssets.MouseText.Value;
            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, new Vector2(x, chatBubblePosition.Y - font.MeasureString(text).Y / 2f), color, rotation, origin, baseScale);
        }
        public static void DrawShopQuote(string text, int x, int y, Color color, NPC npc)
        {
            DrawShopQuote(text, x, y, 0f, Vector2.Zero, Vector2.One, color, npc);
        }
        public static void DrawShopQuote(DrawableTooltipLine line, NPC npc)
        {
            DrawShopQuote(line.Text, line.X, line.Y, line.Rotation, line.Origin, line.BaseScale, line.OverrideColor.GetValueOrDefault(line.Color), npc);
        }

        public static string KeyFromIDs(string mod, int npc, int item)
        {
            return $"Mods.{mod}.Chat.{AequusText.CreateSearchNameFromNPC(npc)}.ShopQuote.{AequusText.SearchNameToAequusKey(ItemID.Search.GetName(item))}";
        }
    }
}