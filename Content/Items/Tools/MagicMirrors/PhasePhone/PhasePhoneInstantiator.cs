using Aequus.Common.Items;
using Aequus.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Tools.MagicMirrors.PhasePhone;

public class PhasePhoneInstantiator : ModSystem {
    public ModItem PhasePhone { get; private set; }
    public ModItem PhasePhoneHome { get; private set; }
    public ModItem PhasePhoneSpawn { get; private set; }
    public ModItem PhasePhoneOcean { get; private set; }
    public ModItem PhasePhoneUnderworld { get; private set; }

    public override void Load() {
        var phasePhone = new InstancedPhasePhone("", ItemID.ShellphoneDummy);

        var phasePhoneHome = new InstancedPhasePhone("Home", ItemID.Shellphone);

        var phasePhoneSpawn = new InstancedPhasePhone("Spawn", ItemID.ShellphoneSpawn)
            .WithTeleportLocation((player) => player.Shellphone_Spawn())
            .WithDust(DustID.RainbowMk2)
            .WithDustColors(() => Main.rand.Next(4) switch {
                2 => Color.Yellow,
                3 => Color.White,
                _ => new(100, 255, 100),
            });

        var phasePhoneOcean = new InstancedPhasePhone("Ocean", ItemID.ShellphoneOcean)
            .WithTeleportLocation((player) => player.MagicConch())
            .WithDynamicDust(Dust.dustWater);

        var phasePhoneUnderworld = new InstancedPhasePhone("Underworld", ItemID.ShellphoneHell)
            .WithTeleportLocation((player) => player.DemonConch())
            .WithDust(DustID.Lava);

        phasePhone.ConvertInto(phasePhoneHome);
        phasePhoneHome.ConvertInto(phasePhoneSpawn);
        phasePhoneSpawn.ConvertInto(phasePhoneOcean);
        phasePhoneOcean.ConvertInto(phasePhoneUnderworld);
        phasePhoneUnderworld.ConvertInto(phasePhoneHome);

        PhasePhone = phasePhone;
        PhasePhoneHome = phasePhoneHome;
        PhasePhoneSpawn = phasePhoneSpawn;
        PhasePhoneOcean = phasePhoneOcean;
        PhasePhoneUnderworld = phasePhoneUnderworld;

        Mod.AddContent(PhasePhone);
        Mod.AddContent(PhasePhoneHome);
        Mod.AddContent(PhasePhoneSpawn);
        Mod.AddContent(PhasePhoneOcean);
        Mod.AddContent(PhasePhoneUnderworld);
    }

    public override void AddRecipes() {
        Recipe.Create(PhasePhone.Type)
            .AddRecipeGroup(AequusRecipes.Shellphone)
            .AddIngredient<PhaseMirror.PhaseMirror>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}