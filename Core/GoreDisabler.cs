namespace Aequus.Core;

public class GoreDisabler : ILoadable {
    public static System.Boolean DisableGores { get; private set; }

    public void Load(Mod mod) {
        On_Gore.NewGore_IEntitySource_Vector2_Vector2_int_float += On_Gore_NewGore_IEntitySource_Vector2_Vector2_int_float;
        DisableGores = false;
    }

    private System.Int32 On_Gore_NewGore_IEntitySource_Vector2_Vector2_int_float(On_Gore.orig_NewGore_IEntitySource_Vector2_Vector2_int_float orig, Terraria.DataStructures.IEntitySource source, Microsoft.Xna.Framework.Vector2 Position, Microsoft.Xna.Framework.Vector2 Velocity, System.Int32 Type, System.Single Scale) {
        if (DisableGores) {
            return Main.maxGore;
        }

        return orig(source, Position, Velocity, Type, Scale);
    }

    public void Unload() {
    }

    public static void Begin() {
        DisableGores = true;
    }

    public static void End() {
        DisableGores = false;
    }
}
