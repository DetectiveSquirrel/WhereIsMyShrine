using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using SharpDX;
using System.Collections.Generic;

namespace WhereIsMyShrine
{
    public class Settings : ISettings
    {
        public ToggleNode Enable { get; set; } = new ToggleNode(false);
        public ToggleNode DisableDrawOnLeftOrRightPanelsOpen { get; set; } = new ToggleNode(false);
        public ToggleNode IgnoreFullscreenPanels { get; set; } = new ToggleNode(false);
        public ToggleNode IgnoreLargePanels { get; set; } = new ToggleNode(false);
        public ColorNode ShrineColor { get; set; } = new ColorNode(Color.Yellow);
        public RangeNode<int> ShrineSize { get; set; } = new RangeNode<int>(80, 1, 500);
        public RangeNode<int> ShrineSegments { get; set; } = new RangeNode<int>(3, 3, 50);
    }
}