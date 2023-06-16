# Pipette Mod
Pick Block Mod for Terraria
Workshop Link: https://steamcommunity.com/sharedfiles/filedetails/?id=2990356011

This is a simple mod to make building faster and more convenient. Hover the mouse over a tile in the world and hit the keybind to select that block in your hotbar.

A slightly more detailed explanation of how it works: if you have the item in your hotbar, you will switch to it automatically. Otherwise it will be pulled from your inventory. If you have space in your hotbar it will be moved there and you will switch to it, otherwise it will swap with the block you are currently holding.

---

Known issues:

## Hotbar Switching
Switching items on the hotbar is currently done with `player.changeItem = hotbarItemIndex`. This is fine unless the inventory happens to be open.
It seems that Terraria does not register the item switch via this method until the inventory is closed.
Even though the point of the mod is to remove the need to have the inventory open all the time for reaching the rest of your blocks, it is still an issue to be fixed.
I can't really figure out any other way of doing this though, so if anyone more knowledgable knows, a bit of help would be greatly appreciated.

---

## Credit
GasGrass: Pre-release testing
