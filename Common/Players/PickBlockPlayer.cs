using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using PipetteMod.Common.Systems;
using PipetteMod.Common.Configs;
using PipetteMod.Common.UI.PickBlockUI;
using PipetteMod.Helpers;
using Slot = PipetteMod.Helpers.InventoryHelper.Slot;

namespace PipetteMod.Common.Players
{
    public class PickBlockPlayer : ModPlayer
    {
        private static SoundStyle pickSoundStyle = new("PipetteMod/Assets/Sounds/pick");
        private static ModKeybind PickBlockKeybind => KeybindSystem.PickBlockKeybind;
        private static readonly PickBlockClientConfig config = ModContent.GetInstance<PickBlockClientConfig>();
        private static readonly PickBlockUISystem uiSystem = ModContent.GetInstance<PickBlockUISystem>();

        private Player Me => this.Entity;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Me.HeldItem.damage > 0 && config.DisableWithToolsEquipped) return;
            if (config.EnablePickBlock && Me == Main.LocalPlayer && PickBlockKeybind.JustPressed)
            {
                if (InventoryHelper.TryGetTileAtMousePosition(Me, Main.MouseWorld, config.PickWalls, config.DisablePickWhileInteracting, out Tile tileAtMousePosition))
                {
                    PickBlock(tileAtMousePosition);
                }
            }
        }

        private void PickBlock(Tile tileAtMousePosition)
        {
            if (!InventoryHelper.TryGetMatchingItem(Me, tileAtMousePosition, InventoryHelper.IsTileWallOnly(tileAtMousePosition), out Slot matchingItemSlot))
                return;

            if (InventoryHelper.TryGetIndexOfItemInHotbar(Me, matchingItemSlot.Item, out int hotbarItemIndex))
            {
                Me.changeItem = hotbarItemIndex;
                NotifyPick(matchingItemSlot.Item);
                return;
            }

            if (config.OnlyPickFromHotbar) return;

            if (InventoryHelper.TryFindEmptySlotInHotbar(Me, out Slot emptyHotbarSlot) && !config.AlwaysSwapWithHeldItem)
            {
                InventoryHelper.SwapTwoItems(Me, emptyHotbarSlot, matchingItemSlot);
                Me.changeItem = emptyHotbarSlot.Index;
                NotifyPick(matchingItemSlot.Item);
            }
            else if (InventoryHelper.TryGetHeldItemSlot(Me, out Slot heldItemSlot))
            {
                InventoryHelper.SwapTwoItems(Me, heldItemSlot, matchingItemSlot);
                NotifyPick(matchingItemSlot.Item);
            }
        }

        private void NotifyPick(in Item item)
        {
            if (item is null) return;

            if (config.PlaySoundOnPickBlock)
                SoundEngine.PlaySound(pickSoundStyle);

            if (config.DisplayUIMessageOnPickBlock)
                uiSystem.DescribeItem(in item, config.MessageLengthSeconds);
        }
    }
}
