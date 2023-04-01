using Terraria.ModLoader;

namespace Aequus.Content.Commands {
    public abstract class AequusCommand : ModCommand {
        public bool TryGetBoolean(string argument, out bool value) {

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
}