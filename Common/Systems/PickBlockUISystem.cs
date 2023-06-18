using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PipetteMod.Common.UI.PickBlockUI;

namespace PipetteMod.Common.Systems;

internal class PickBlockUISystem : ModSystem
{
    internal UserInterface userInterface;
    internal PickBlockUIState pickBlockState;

    private GameTime lastUpdateUiGameTime;

    private int activeTime = -1;

    public override void Load()
    {
        if (Main.dedServ) return;

        userInterface = new UserInterface();
        pickBlockState = new PickBlockUIState();
        pickBlockState.Activate();
    }

    public override void UpdateUI(GameTime gameTime)
    {
        lastUpdateUiGameTime = gameTime;

        if (userInterface?.CurrentState != null)
        {
            userInterface.Update(gameTime);

            if (activeTime == 0)
            {
                userInterface.SetState(null);
                activeTime = -1;
            }
            else if(activeTime > 0) activeTime--;
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex == -1) return;

        layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
            "PipetteMod: PickBlockInterface",
            delegate
            {
                if (lastUpdateUiGameTime != null && userInterface?.CurrentState != null)
                {
                    userInterface.Draw(Main.spriteBatch, lastUpdateUiGameTime);
                }
                return true;
            },
            InterfaceScaleType.UI));
    }

    internal void DescribeItem(in Item item, int duration)
    {
        pickBlockState.PickDescription.SetText($"Picked {item.Name} ({item.stack})");
        activeTime = duration * 60;
        userInterface?.SetState(pickBlockState);
    }
}
