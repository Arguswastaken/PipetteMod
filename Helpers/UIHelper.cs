using Terraria.GameContent.UI.Elements;

namespace PipetteMod.Helpers;

internal static class UIHelper
{
    internal static UIPanel NewPanel(int width, int height)
    {
        UIPanel panel = new();

        panel.Width.Set(width, 0);
        panel.Height.Set(height, 0);

        return panel;
    }

    internal static UIText NewCenteredText(ref UIPanel container, string content)
    {
        UIText text = new(content)
        {
            HAlign = 0.5f,
            VAlign = 0.5f
        };
        container.Append(text);

        return text;
    }
}
