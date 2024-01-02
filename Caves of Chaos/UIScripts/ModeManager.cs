using Caves_of_Chaos.CreatureScripts;
using Caves_of_Chaos.GridScripts;
using Caves_of_Chaos.ItemScripts;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caves_of_Chaos.UIScripts
{
    public static class ModeManager
    {
        public enum modes {Grid, Examine, Inventory, Decision, Message};
        public static modes mode = modes.Grid;
        public static bool mustMakeDecision = false;
        public static bool lockedMessage = false;

        public static void Draw()
        {
            if (mode == modes.Inventory)
            {
                InventoryMode.Render();
            } else if (mode == modes.Decision)
            {
                DecisionConsole.Render();
            }
        }

        public static void HandleInput(Keyboard keyboard)
        {
            if (mode == modes.Grid)
            {
                // If key is a mode selector, change mode:
                if (keyboard.IsKeyPressed(Keys.X))
                {
                    mode = modes.Examine;
                    ExamineMode.pos = PlayerManager.player.GetPosition();
                }
                else if (keyboard.IsKeyPressed(Keys.I))
                {
                    mode = modes.Inventory;
                    InventoryMode.selection = 0;
                    InventoryMode.Render();
                }
                else if (keyboard.IsKeyPressed(Keys.G)) // Get item:
                {
                    mode = modes.Decision;
                    DecisionConsole.selection = 0;
                    List<String> items = new List<String>();
                    Point playerPos = PlayerManager.player.GetPosition();
                    List<Item> itemsHere = GridManager.activeGrid.tiles[playerPos.X, playerPos.Y].items;
                    for (int i = 0; i < itemsHere.Count; i++)
                    {
                        items.Add(itemsHere[i].DisplayName());
                    }
                    DecisionConsole.list = items;
                    DecisionConsole.onSelection = i => { PlayerManager.player.GetItem(itemsHere[i]); };
                    DecisionConsole.Render();
                }
                else if (keyboard.IsKeyPressed(Keys.E)) // Equip item:
                {
                    mode = modes.Decision;
                    DecisionConsole.selection = 0;
                    List<String> items = new List<String>();
                    List<Item> inventoryItems = PlayerManager.player.inventory;
                    for (int i = 0; i < inventoryItems.Count; i++)
                    {
                        items.Add(inventoryItems[i].DisplayName());
                    }
                    DecisionConsole.list = items;
                    DecisionConsole.onSelection = i => { PlayerManager.player.EquipItem(inventoryItems[i]); };
                    DecisionConsole.Render();
                }
                else // Send to PlayerManager
                {
                    PlayerManager.HandleInput(keyboard);
                }
            } 
            else if (mode == modes.Examine)
            {
                if (keyboard.IsKeyPressed(Keys.Escape))
                {
                    mode = modes.Grid;
                }
                else
                {
                    ExamineMode.HandleInput(keyboard);
                }
            } else if (mode == modes.Inventory)
            {
                if (keyboard.IsKeyPressed(Keys.Escape))
                {
                    mode = modes.Grid;
                    Program.container.largeScreenConsole.IsVisible = false;
                } else
                {
                    InventoryMode.HandleInput(keyboard);
                }
            }
            else if (mode == modes.Decision)
            {
                if (keyboard.IsKeyPressed(Keys.Escape) && !mustMakeDecision)
                {
                    mode = modes.Grid;
                    Program.container.smallScreenConsole.IsVisible = false;
                }
                else if (keyboard.IsKeyPressed(Keys.Enter))
                {
                    mode = modes.Grid;
                    mustMakeDecision = false;
                    Program.container.smallScreenConsole.IsVisible = false;
                    DecisionConsole.onSelection(DecisionConsole.selection);
                }
                else
                {
                    DecisionConsole.HandleInput(keyboard);
                }
            } else if (mode == modes.Message)
            {
                if (keyboard.IsKeyPressed(Keys.Escape) && !lockedMessage)
                {
                    mode = modes.Grid;
                    Program.container.smallScreenConsole.IsVisible = false;
                    Program.container.largeScreenConsole.IsVisible = false;
                }
            }
        }

        public static void IncreaseStats()
        {
            mode = modes.Decision;
            mustMakeDecision = true;
            DecisionConsole.selection = 0;
            List<String> stats = new List<String>();
            stats.Add("Strength");
            stats.Add("Dexterity");
            DecisionConsole.list = stats;
            DecisionConsole.onSelection = i => { 
                if (i == 0) { PlayerManager.player.strength++; }
                else if (i == 1) { PlayerManager.player.dexterity++; }
            };
            DecisionConsole.Render();
        }
    }
}
