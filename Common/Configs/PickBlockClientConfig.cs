using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace PipetteMod.Common.Configs
{
    public class PickBlockClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Client Settings")]
        [DefaultValue(true)]
        [Label("Enable Pick Block")]
        public bool EnablePickBlock;

        [DefaultValue(true)]
        [Label("Pick Walls")]
        public bool PickWalls;

        [DefaultValue(false)]
        [Label("Only pick blocks already in the hotbar")]
        public bool OnlyPickFromHotbar;

        [DefaultValue(false)]
        [Label("Always swap with the currently held block instead of filling hotbar")]
        public bool AlwaysSwapWithHeldItem;

        [DefaultValue(false)]
        [Label("Disable Pick Block when weapons or tools are equipped")]
        public bool DisableWithToolsEquipped;

        [DefaultValue(false)]
        [Label("Disable Pick Block when close enough to open chests (useful when bound to RMB)")]
        public bool DisablePickWhileInteracting;

        [Header("Feedback")]
        [DefaultValue(true)]
        [Label("Play a sound effect on pick")]
        public bool PlaySoundOnPickBlock;

        [DefaultValue(true)]
        [Label("Display small UI message on pick")]
        public bool DisplayUIMessageOnPickBlock;

        [DefaultValue(2)]
        [Label("Message display time (seconds)")]
        [Range(1, 4)]
        public int MessageLengthSeconds;
    }
}
