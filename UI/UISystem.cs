using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Aequus.UI
{
    public sealed class UISystem : ModSystem
    {
        public static UserInterface InventoryInterface { get; private set; }
        public static UserInterface NPCTalkInterface { get; private set; }

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                InventoryInterface = new UserInterface();
                NPCTalkInterface = new UserInterface();
            }
        }

        public override void Unload()
        {
            InventoryInterface = null;
            NPCTalkInterface = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            InventoryInterface.Update(gameTime);
            NPCTalkInterface.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            IntoLayer(layers, "Vanilla: Inventory", "Aequus: Inventory", () => 
            {
                InventoryInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                return true;
            });
        }
        private void IntoLayer(List<GameInterfaceLayer> layers, string name, string yourName, GameInterfaceDrawMethod method, InterfaceScaleType scaleType = InterfaceScaleType.UI)
        {
            int index = layers.FindIndex((l) => l.Name.Equals(name));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(yourName, method, scaleType));
            }
        }
    }
}