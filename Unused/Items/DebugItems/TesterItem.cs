using Aequus.Common.Effects;
using Aequus.Content.Biomes.Aether;
using Aequus.Content.Biomes.CrabCrevice.Tiles;
using Aequus.Content.Events.GlimmerEvent;
using Aequus.Content.World.Generation;
using Aequus.Items;
using Aequus.NPCs.Monsters.Underworld;
using Aequus.Tiles.Ambience;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Unused.Items.DebugItems {
    internal class TesterItem : ModItem {
        public override string Texture => AequusTextures.Gamestar.Path;

        public MethodInfo _debugMethod;

        public record struct TestParameters(int X, int Y, Player Player) {
            public Point TileCoordinates_Point => new(X, Y);
            public Vector2 TileCoordinates_Vector2 => new(X, Y);
            public Vector2 WorldCoordinates => new Vector2(X, Y).ToWorldCoordinates();
        }

        public override bool IsLoadingEnabled(Mod mod) {
#if DEBUG
            return true;
#endif
            return false;
        }

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.rare = ItemRarityID.Red;
            Item.width = 20;
            Item.height = 20;
            Item.color = Main.OurFavoriteColor;
        }

        public override bool? UseItem(Player player) {
            if (player.altFunctionUse == 2) {
                var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                MethodInfo firstFoundMethod = null;
                bool foundCurrentSelectedMethod = false;
                for (int i = 0; i < methods.Length; i++) {
                    var parameters = methods[i].GetParameters();
                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof(TestParameters)) {
                        continue;
                    }

                    if (firstFoundMethod == null) {
                        firstFoundMethod = methods[i];
                    }

                    if (!foundCurrentSelectedMethod && _debugMethod != null) {

                        if (methods[i].Name == _debugMethod.Name) {
                            foundCurrentSelectedMethod = true;
                        }
                        continue;
                    }
                    else {
                        _debugMethod = methods[i];
                        return true;
                    }
                }
                _debugMethod = firstFoundMethod;
                return true;
            }

            TestParameters testParameters = new(Helper.MouseTileX, Helper.MouseTileY, player);
            _debugMethod?.Invoke(this, new object[] { testParameters });
            return true;
        }

        public override bool AltFunctionUse(Player player) {
            return true;
        }

        public void GenerateAetherWaterfall(TestParameters parameters) {
            ModContent.GetInstance<AetherCavesGenerator>().GenerateWaterfall(parameters.X, parameters.Y);
        }

        public void GenerateAether(TestParameters parameters) {
            ModContent.GetInstance<AetherCavesGenerator>().Generate();
        }

        public void GenerateAetheriumSpike(TestParameters parameters) {
            ModContent.GetInstance<AetherCavesGenerator>().GenerateAetheriumSpike(parameters.X, parameters.Y);
        }

        public void ShimmerBiomeReworkTunnels(TestParameters parameters) {
            ModContent.GetInstance<AetherCavesGenerator>().GenerateTunnel(parameters.X, parameters.Y, parameters.X, parameters.Y, 100, 100);
            //ModContent.GetInstance<AetherCavesGenerator>().GenerateFill(parameters.X, parameters.Y, parameters.X, parameters.Y, 100, 100);
        }

        public void SetCam(TestParameters parameters) {
            var camera = ModContent.GetInstance<CameraFocus>();
            if (camera.hold <= 10) {
                foreach (var g in Main.gore) {
                    g.active = false;
                }
                camera.SetTarget("Test", Main.MouseWorld, CameraPriority.VeryImportant, hold: int.MaxValue - 100);
            }
            else {
                camera.hold = 2;
            }
        }

        public void SpitTileObjectData(TestParameters parameters) {
            int style = 0;
            int alt = 0;
            TileObjectData.GetTileInfo(Main.tile[parameters.X, parameters.Y], ref style, ref alt);
            Main.NewText(style, Color.Orange);
            Main.NewText(alt, Color.Yellow);
        }

        public void ReforgeItems(TestParameters parameters, int pre) {
            var player = parameters.Player;
            for (int i = 0; i < Main.InventorySlotsTotal; i++) {
                int stack = player.inventory[i].stack;
                player.inventory[i].SetDefaults(player.inventory[i].type);
                player.inventory[i].stack = stack;
                player.inventory[i].Prefix(pre);
            }
        }

        public void RadonCavesTest(TestParameters parameters) {
            int x = parameters.X;
            int y = parameters.Y;
            if (AequusWorldGenerator.RadonCaves.ValidSpotForCave(x, y)) {
                AequusWorldGenerator.RadonCaves.CreateCave(x, y);
            }
        }
        public void RadonGrowStalactite(TestParameters parameters) {
            int x = parameters.X;
            int y = parameters.Y;
            AequusWorldGenerator.RadonCaves.GrowStalactite(x, y, AequusWorldGenerator.RadonCaves.MaxWidth, AequusWorldGenerator.RadonCaves.MaxHeight);
        }

        public void PlacePollenExamples(TestParameters parameters) {
            int x = parameters.X;
            int y = parameters.Y;
            WorldGen.PlaceTile(x, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x, y - 1, ModContent.TileType<MorayTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y - 1, ModContent.TileType<MorayTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y, ModContent.TileType<SedimentaryRockTile>(), forced: true);
            WorldGen.PlaceTile(x + 4, y - 1, ModContent.TileType<MorayTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x, y + 2, ModContent.TileType<ManacleTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x + 2, y + 2, ModContent.TileType<ManacleTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 3, TileID.Ash, forced: true);
            WorldGen.PlaceTile(x + 4, y + 2, ModContent.TileType<ManacleTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x + 2, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 6, TileID.Meteorite, forced: true);
            WorldGen.PlaceTile(x + 4, y + 5, ModContent.TileType<MoonflowerTile>(), forced: true, style: 2);

            WorldGen.PlaceTile(x, y + 9, TileID.Cloud, forced: true);
            WorldGen.PlaceTile(x, y + 8, ModContent.TileType<MistralTile>(), forced: true);
            WorldGen.PlaceTile(x + 2, y + 9, TileID.RainCloud, forced: true);
            WorldGen.PlaceTile(x + 2, y + 8, ModContent.TileType<MistralTile>(), forced: true, style: 1);
            WorldGen.PlaceTile(x + 4, y + 9, TileID.SnowCloud, forced: true);
            WorldGen.PlaceTile(x + 4, y + 8, ModContent.TileType<MistralTile>(), forced: true, style: 2);
        }

        public void SpawnPhysicalTestDummies(TestParameters parameters, int npcId) {
            var m = parameters.WorldCoordinates;
            for (int i = 0; i < 5; i++) {
                var n = NPC.NewNPCDirect(null, m + new Vector2(90f * (i - 2), 0f), npcId);
                n.Aequus().noAI = true;
                n.knockBackResist = 0f;
            }
        }

        public void SpawnGlimmer(TestParameters parameters) {
            GlimmerBiomeManager.TileLocation = Point.Zero;
            GlimmerSystem.BeginEvent();
            GlimmerBiomeManager.TileLocation = parameters.TileCoordinates_Point;
        }

        public void KillOfType(TestParameters parameters, int npcId) {
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].active && Main.npc[i].type == npcId) {
                    Main.npc[i].StrikeInstantKill();
                }
            }
        }
        public NPC NoAINPC(TestParameters parameters, int npcId) {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld, npcId);
            n.Aequus().noAI = true;
            return n;
        }

        public void NoAITrapperImp(TestParameters parameters) {
            var n = NPC.NewNPCDirect(null, Main.MouseWorld.NumFloor(4), ModContent.NPCType<TrapperImp>());
            n.Aequus().noAI = true;

            foreach (var v in Helper.CircularVector(3, -MathHelper.PiOver2)) {
                var n2 = NPC.NewNPCDirect(n.GetSource_FromAI(), (Main.MouseWorld + v * 125f).NumFloor(4), ModContent.NPCType<Trapper>(), n.whoAmI, ai1: n.whoAmI + 1f);
                n2.Aequus().noAI = true;
                n2.rotation = v.ToRotation() + MathHelper.PiOver2;
            }
        }

        public void WriteNPCsInHashSet(TestParameters parameters, HashSet<int> hash) {
            foreach (var item in hash) {
                Main.NewText(Lang.GetNPCNameValue(item));
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            if (_debugMethod != null) {
                tooltips.Add(new(Mod, "DebugLine0", _debugMethod.Name));
            }

            if (Helper.DebugKeyPressed)
                return;

            tooltips.Add(new(Mod, "DebugLine1", string.Join(", ", Helper.GetListOfActiveDifficulties())));
            tooltips.Add(new TooltipLine(Mod, "DebugLine2", string.Join(", ", Main.LocalPlayer.GetStringListOfBiomes().ConvertAll(Language.GetTextValue))));
        }
    }
}