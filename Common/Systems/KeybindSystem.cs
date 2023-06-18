using Terraria.ModLoader;

namespace PipetteMod.Common.Systems;

public class KeybindSystem : ModSystem
{
    public static ModKeybind PickBlockKeybind { get; private set; }

    public override void Load()
    {
        PickBlockKeybind = KeybindLoader.RegisterKeybind(Mod, "PickBlock", "Mouse2");
    }

    public override void Unload()
    {
        PickBlockKeybind = null;
    }
}
