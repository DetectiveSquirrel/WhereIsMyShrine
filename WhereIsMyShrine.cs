﻿using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Shrine = ExileCore.PoEMemory.Components.Shrine;
using Vector2N = System.Numerics.Vector2;
using Vector3N = System.Numerics.Vector3;

namespace WhereIsMyShrine;

public class WhereIsMyShrine : BaseSettingsPlugin<Settings>
{
    private readonly List<Entity> shrineList = [];

    public WhereIsMyShrine()
    {
        Name = "Where Is My Shrine";
    }

    public Vector3N PlayerPos { get; set; }
    public IngameData IngameData { get; set; }

    public override bool Initialise() => true;

    public override Job Tick()
    {
        IngameData = GameController.IngameState.Data;
        var Player = GameController?.Player;

        if (Player == null)
        {
            return null;
        }

        PlayerPos = IngameData.ToWorldWithTerrainHeight(Player.PosNum);
        return null;
    }

    public override void AreaChange(AreaInstance area)
    {
        shrineList.Clear();
    }

    public override void EntityAdded(Entity entity)
    {
        entity.TryGetComponent<Shrine>(out var shrineComp);

        if (shrineComp != null)
        {
            shrineList.Add(entity);
        }
    }

    public override void EntityRemoved(Entity entity)
    {
        var entityToRemove = shrineList.FirstOrDefault(shrine => shrine.Id == entity.Id);

        if (entityToRemove != null)
        {
            shrineList.Remove(entityToRemove);
        }
    }

    public override void Render()
    {
        if (!Settings.Enable.Value || !GameController.InGame || IngameData == null || PlayerPos == Vector3N.Zero)
        {
            return;
        }

        var ingameUi = GameController.Game.IngameState.IngameUi;

        if (Settings.DisableDrawOnLeftOrRightPanelsOpen && (ingameUi.OpenLeftPanel.IsVisible || ingameUi.OpenRightPanel.IsVisible))
        {
            return;
        }

        if (!Settings.IgnoreFullscreenPanels && ingameUi.FullscreenPanels.Any(x => x.IsVisible))
        {
            return;
        }

        if (!Settings.IgnoreLargePanels && ingameUi.LargePanels.Any(x => x.IsVisible))
        {
            return;
        }

        var screenSize = new RectangleF
        {
            X = 0,
            Y = 0,
            Width = GameController.Window.GetWindowRectangleTimeCache.Size.Width,
            Height = GameController.Window.GetWindowRectangleTimeCache.Size.Height
        };

        foreach (var shrineEntity in shrineList)
        {
            shrineEntity.TryGetComponent<Shrine>(out var shrineComp);

            if (shrineComp is {IsAvailable: true})
            {
                var entityPosScreen = RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen(shrineEntity.PosNum);
                if (IsEntityWithinScreen(entityPosScreen, screenSize, 50))
                {
                    Graphics.DrawFilledCircleInWorld(shrineEntity.PosNum, Settings.ShrineSize, Settings.ShrineColor, Settings.ShrineSegments);
                }
            }
        }
    }

    private static bool IsEntityWithinScreen(Vector2N entityPos, RectangleF screensize, float allowancePX)
    {
        // Check if the entity position is within the screen bounds with allowance
        var leftBound = screensize.Left - allowancePX;
        var rightBound = screensize.Right + allowancePX;
        var topBound = screensize.Top - allowancePX;
        var bottomBound = screensize.Bottom + allowancePX;

        return entityPos.X >= leftBound && entityPos.X <= rightBound && entityPos.Y >= topBound && entityPos.Y <= bottomBound;
    }
}