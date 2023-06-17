using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace PipetteMod.Common.Configs
{
    public class PickBlockClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Functionality")]
        [DefaultValue(true)]
        public bool EnablePickBlock;

        [DefaultValue(true)]
        public bool PickWalls;

        [DefaultValue(false)]
        public bool OnlyPickFromHotbar;

        [DefaultValue(false)]
        public bool AlwaysSwapWithHeldItem;

        [DefaultValue(false)]
        public bool DisableWithToolsEquipped;

        [DefaultValue(false)]
        public bool DisablePickWhileInteracting;

        [Header("Feedback")]
        [DefaultValue(true)]
        public bool PlaySoundOnPickBlock;

        [DefaultValue(true)]
        public bool DisplayUIMessageOnPickBlock;

        [DefaultValue(2)]
        [Range(1, 4)]
        public int MessageLengthSeconds;
    }
}
