using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using PipetteMod.Helpers;

namespace PipetteMod.Common.UI.PickBlockUI
{
    internal class PickBlockUIState : UIState
    {
        internal UIText PickDescription;

        private UIPanel container;

        public override void OnInitialize()
        {
            container = UIHelper.NewPanel(500, 50);
            container.HAlign = 0.5f;
            container.VAlign = 0.7f;

            PickDescription = UIHelper.NewCenteredText(ref container, "");

            this.Append(container);
        }
    }
}
