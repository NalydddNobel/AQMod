namespace Aequus.Content.Systems.Commands;

internal static class ModCommandExtensions {
    public static bool TryGetBoolean(this ModCommand c, string argument, out bool value) {
        switch (argument) {
            case "yes":
            case "y":
            case "1":
                value = true;
                return true;
            case "no":
            case "n":
            case "0":
                value = false;
                return true;
        }

        value = false;
        return false;
    }
}