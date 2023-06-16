# Pipette Mod
Pick Block Mod for Terraria (currently unreleased).

This is a simple mod to make building faster and more convenient. Hover the mouse over a tile in the world and hit the keybind to select that block in your hotbar.

A slightly more detailed explanation of how it works: if you have the item in your hotbar, you will switch to it automatically. Otherwise it will be pulled from your inventory. If you have space in your hotbar it will be moved there and you will switch to it, otherwise it will swap with the block you are currently holding.

---

I have done the best I can with the knowledge I have, but there are definitely some improvements to be made. A few known issues:

## Hotbar Switching
Switching items on the hotbar is currently done with `player.changeItem = hotbarItemIndex`. This is fine unless the inventory happens to be open.
It seems that Terraria does not register the item switch via this method until the inventory is closed.
Even though the point of the mod is to remove the need to have the inventory open all the time for reaching the rest of your blocks, it is still an issue to be fixed.
I can't really figure out any other way of doing this though, so if anyone more knowledgable knows, a bit of help would be greatly appreciated.

## FrameImportants and placeStyles
A few tiles in vanilla Terraria (and perhaps some mods?) have a vast amount of different styles/varieties.
These are all grouped into a single FrameImportant tile for that block, having a single ID to avoid having an enormous number of unique tiles for every single chair, workbench, chest etc.
Torches, for instance, *are* placed by unique items, which all have their own item ID. But they all place the same tile, just using a different `placeStyle` so it displays the correct frame from its one big sprite.

Here are torches (1x1 FrameImportants I presume), which go vertically:

![](https://cdn.discordapp.com/attachments/854799209428680747/1119374066940383384/Tiles_4.png)

And bookcases, which go horizontally with a second row:

![](https://cdn.discordapp.com/attachments/854799209428680747/1119374407459143821/Tiles_101.png)

This makes sense, but it complicates things for this mod since we can't always rely on `tile.TileType` alone.

I did figure out this hacky tentative way of resolving this, although I hate it:
```cs
private static readonly GroupedTileInfo[] groupedTilesWithFrameLengths = new GroupedTileInfo[]
{
  new GroupedTileInfo(TileID.Torches, 22, 'y'),
  new GroupedTileInfo(TileID.Containers, 36, 'x'),
  new GroupedTileInfo(TileID.Containers2, 36, 'x'),
  new GroupedTileInfo(TileID.FakeContainers, 36, 'x'),
  new GroupedTileInfo(TileID.FakeContainers2, 36, 'x'),
  new GroupedTileInfo(TileID.GemSaplings, 54, 'x'),
  new GroupedTileInfo(TileID.WorkBenches, 36, 'x'),
  new GroupedTileInfo(TileID.Tables, 54, 'x'),
  new GroupedTileInfo(TileID.Bookcases, 54, 'x'),
  new GroupedTileInfo(TileID.BeachPiles, 18, 'y')
};
```

Right now these random structs are how these tiles are represented. Every time the mod is used it checks whether the selected tile matches one of these IDs.
If it does, it divides the tile's `TileFrameX`/`Y` by the *pre-defined* number of pixels (on the *pre-defined* axis since it's not always the same one),
and finds the item in the player's inventory which has a `placeStyle` that matches this number.

This technically works for the ones that have been correctly put in, but it's very flawed for obvious reasons.
I had to manually figure out the numbers for all of them, and I'm sure I missed some too which is another reason this is bad.
They also don't support any ModTiles that also work this way, if anyone has made them.

### Fixing This Thing

I put this repository up here as the first step to publishing the mod itself, but after writing this I decided I should probably fix it first.
I do have a plan involving `Main.tileFrameImportant[tile.TileType]` and `TileObjectData`, assuming I can use them in the way I imagine I can for this.
I don't like having that horrible inefficient monstrosity in my project, so fixing this is my main priority for now.

---

## Credit
GasGrass: Pre-release testing
