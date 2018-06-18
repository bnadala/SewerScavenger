using SewerScavenger.ViewModels;
using System;
using System.Collections.Generic;

namespace SewerScavenger.Models
{
    public class BattleGrid
    {
        // Number of rows and columns that make up the grid
        private int RowSize;
        private int ColumnSize;

        // List of potential targets to move closer towards. 
        private List<BattleTile> TargetList = new List<BattleTile>();

        // The actual grid that holds all the fighters as tiles
        public List<List<BattleTile>> Grid { get; set; }

        // Remaining state after an attack.
        public BattleTile recentlyDeceased = new BattleTile();      // Tile of the Fighter that was defeated by an attack
        public string DamageStatus = "";                            // Damage status indicates type of attack(miss, normal, crit, crit miss, focused)
        
        // Grid Constructor with indicated size.
        public BattleGrid(int rows, int columns)
        {
            // Initializes the grid.
            RowSize = rows;
            ColumnSize = columns;
            Grid = new List<List<BattleTile>>();

            for (var i = 0; i < ColumnSize; i++)
            {
                Grid.Add(new List<BattleTile>());
                for (var j = 0; j < RowSize; j++)
                {
                    Grid[i].Add(new BattleTile(j, i));
                }
            }
        }

        // Recursive algorithm that paints the grid in red to indicate range to attack.
        // Exclusively used by Characters to indicate their range.
        public void PaintWarPath(BattleTile current, int row, int col, int range)
        {
            // Limits path range based on number of moves taken
            if (range < 0)
            {
                return;
            }

            // Maximum range in the grid from anywhere in the grid.
            // Without this limit, the algorithm would continue to run needlessly
            if(range > 9)
            {
                range = 9;
            }

            // If the current tile is passed, paint it red so characters can skip turn.
            if (row == 0 && col == 0)
            {
                Grid[current.Column][current.Row].Highlight = "Red";
            }

            // Updates the new position
            current.Row += row;
            current.Column += col;

            // Tries to continue the path on all directions
            PaintWarPath(new BattleTile(current), 1, 0, range - 1);
            PaintWarPath(new BattleTile(current), 0, 1, range - 1);
            PaintWarPath(new BattleTile(current), -1, 0, range - 1);
            PaintWarPath(new BattleTile(current), 0, -1, range - 1);

            // Ensures path is within bounds
            if (current.Row >= 0 && current.Row < Grid[0].Count && current.Column >= 0 && current.Column < Grid.Count)
            {
                // No friendly fire
                if (Grid[current.Column][current.Row].Type == "Character")
                {
                    return;
                }

                // Mark empty tiles to skip attack and Monsters to direct attack.
                if (Grid[current.Column][current.Row].Type == "Tile" || Grid[current.Column][current.Row].Type == "Monster")
                {
                    Grid[current.Column][current.Row].Highlight = "Red";
                }
            }
        }

        // Recursive algorithm that paints the tiles the character may move to. BLUE
        public void PaintPath(BattleTile current, int row, int col, int moves)
        {
            // Limits path range based on number of moves taken
            if (moves < 0)
            {
                return;
            }

            // Maximum range in the grid from anywhere in the grid.
            // Without this limit, the algorithm would continue to run needlessly
            if (moves > 9)
            {
                moves = 9;
            }

            // If the current tile is passed, paint it red so characters can skip turn.
            if (row == 0 && col == 0)
            {
                Grid[current.Column][current.Row].Highlight = "Blue";
            }

            // Updates the new position
            current.Row += row;
            current.Column += col;

            // Tries to continue the path on all directions
            PaintPath(new BattleTile(current), 1, 0, moves - 1);
            PaintPath(new BattleTile(current), 0, 1, moves - 1);
            PaintPath(new BattleTile(current), -1, 0, moves - 1);
            PaintPath(new BattleTile(current), 0, -1, moves - 1);

            // Ensures path is within bounds
            if(current.Row >= 0 && current.Row < Grid[0].Count && current.Column >= 0 && current.Column < Grid.Count)
            {
                // Cannot pass by monsters but may pass by tiles and characters
                if (Grid[current.Column][current.Row].Type == "Monster")
                {
                    return;
                }

                //Mark path
                if (Grid[current.Column][current.Row].Type == "Tile")
                {
                    Grid[current.Column][current.Row].Highlight = "Blue";
                }
            }
        }

        // AI for a Monster to make an attack.
        // Includes identifying the closest character
        // Moving closer to the character. Teleporting more like
        // Then attacking the target if within range. 
        public string EnemyTakeTurn(int row, int column)
        {
            // Find target in the grid 
            BattleTile target = new BattleTile();
            for (int i = 0; i < Grid.Count; i++)
            {
                for (int j = 0; j < Grid[i].Count; j++)
                {
                    // If a Character occupies the current grid access to determine if it is closer than current target.
                    if (Grid[i][j].Type == "Character")
                    {
                        // If the current target is just a tile or empty then assign the new target.
                        if (target.Type == "Tile")
                        {
                            target = Grid[i][j];
                        }
                        else
                        {
                            // If the current target and locations are both Characters, use ManhattanDistance to determine the closest one. 
                            if (ManhattanDistance(target.Row, target.Column, row, column) > ManhattanDistance(j, i, row, column))
                            {
                                target = Grid[i][j];
                            }
                        }

                    }
                }
            }
            
            string battleText = "";
            // Move to target in the grid
            BattleTile current = new BattleTile(Grid[column][row]);
            BattleTile newLocation = new BattleTile(MoveCloser(target, current));
            Grid[column][row] = new BattleTile(row, column);
            current.Row = newLocation.Row;
            current.Column = newLocation.Column;
            Grid[newLocation.Column][newLocation.Row] = current;

            // Cannot move any closer to the target so the Monster stays put
            if(row == newLocation.Row && column == newLocation.Column)
            {
                battleText += current.Name + " does not move.\n";
            }

            // Has successfully moved closer to the target. 
            else
            {
                battleText += current.Name + " moves to Row " + newLocation.Row + " and Column " + newLocation.Column + ".\n";
            }

            // Try to attack target.
            if (ManhattanDistance(target.Row, target.Column, newLocation.Row, newLocation.Column) <= current.GetRange())
            {
                // Damage is attempted against the Character target.
                int damage = target.DealtDamage(current);
                string status = target.DamageStatus;

                // Monster misses the attack.
                if(status == "Miss")
                {
                    battleText += current.Name + " misses attack against " + target.Name + ".\n";
                }

                // Monster hits the target.
                else if(status == "Hit")
                {
                    battleText += current.Name + " deals " + damage + " damage to " + target.Name + ".\n";
                }

                // Monster critically misses.
                else if (status == "Critical Miss")
                {
                    battleText += current.Name + " CRITICALLY misses attack against " + target.Name + ".\n";
                    DamageStatus = "Critical Miss";

                // Monster Criticaly hits.
                }else if (status == "Critical Hit")
                {
                    battleText += current.Name + " deals " + damage + " CRITICAL damage to " + target.Name + ".\n";
                    DamageStatus = "Critical Hit";
                }
                target.DamageStatus = "";

                // If the target dies
                if (target.GetHealthCurr() <= 0)
                {
                    // Automatically resurect if the MostlyDead option is turned on.
                    if (BattleSystemViewModel.Instance.MostlyDead && target.MostlyDead)
                    {
                        target.MostlyDead = false;
                        battleText += "Miracle Max pops out of a drainage pipe steps on " + target.Name + " and brings him back to life.\n";
                        target.Hero.HealthCurr = target.Hero.Health;
                        target.Life = "life" + (int)(((double)(target.Hero.HealthCurr + 1) / (double)(target.Hero.Health + 1)) * 10) + ".png";
                    }

                    // Otherwise the character dies and the state is saved. 
                    else
                    {
                        battleText += target.Name + " dies from the damage.\n";
                        Grid[target.Column][target.Row] = new BattleTile(target.Row, target.Column);
                        recentlyDeceased = target;
                    }
                }
            }

            // Monster is not within range of its target. 
            else
            {
                battleText += current.Name + " does not attack. \n";
            }

            // Returns the messages saved during combat.
            return battleText;
        }
        
        // Algorithm that tries to move the monster closer to the target. 
        public BattleTile MoveCloser(BattleTile target, BattleTile current)
        {
            TargetList = new List<BattleTile>();
            BattleTile newLocation = new BattleTile(current);
            MoveCloser(target, new BattleTile(current), 0, 0, current.GetMove());
            foreach (BattleTile t in TargetList)
            {
                // Manhattand Distance is used to select the closest tile to the target whether or not it is within range. 
                if (ManhattanDistance(t.Row, t.Column, target.Row, target.Column) < ManhattanDistance(newLocation.Row, newLocation.Column, target.Row, target.Column))
                {
                    newLocation = t;
                }
            }

            // Returns the new monster location closest to the target. Might be the current location in which case the monster did not move. 
            return Grid[newLocation.Column][newLocation.Row];
        }

        // Recursive algorithm for pathfinding. 
        public void MoveCloser(BattleTile target, BattleTile current, int row, int col, int moves)
        {
            // Limits path range based on number of moves taken
            if (moves < 0)
            {
                return;
            }

            // Moves to the next avaialable tile
            current.Row += row;
            current.Column += col;
            MoveCloser(target, new BattleTile(current), 1, 0, moves - 1);
            MoveCloser(target, new BattleTile(current), 0, 1, moves - 1);
            MoveCloser(target, new BattleTile(current), -1, 0, moves - 1);
            MoveCloser(target, new BattleTile(current), 0, -1, moves - 1);

            // Adds the target to the taget list for comparison after the recursion.
            if (current.Row >= 0 && current.Row < Grid[0].Count && current.Column >= 0 && current.Column < Grid.Count && Grid[current.Column][current.Row].Type == "Tile")
            {
                TargetList.Add(Grid[current.Column][current.Row]);
            }
        }

        // Simple ManhattanDistance calculator
        public int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        // Generates a turnlist by grabbing all monsters and characters in the grid then arranging them by speed. 
        public List<BattleTile> ComputeTurnList()
        {
            List<BattleTile> turnList = new List<BattleTile>();
            for (int i = 0; i < Grid.Count; i++)
            {
                for (int j = 0; j < Grid[i].Count; j++)
                {
                    BattleTile tempTile = Grid[i][j];
                    if(tempTile.Type != "Tile")
                    {
                        bool insterted = false;
                        for (var k = 0; k < turnList.Count && !insterted; k++)
                        {
                            if (tempTile.GetSpeed() > turnList[k].GetSpeed())
                            {
                                insterted = true;
                                turnList.Insert(k, tempTile);
                            }else if (tempTile.GetSpeed() == turnList[k].GetSpeed())
                            {
                                if(tempTile.Type == "Character" && turnList[k].Type == "Monster")
                                {
                                    insterted = true;
                                    turnList.Insert(k, tempTile);
                                }
                            }
                        }
                        if (!insterted)
                        {
                            turnList.Add(tempTile);
                        }
                    }
                }
            }
            return turnList;
        }

        // Checks the grid if there are still characters left. 
        public bool NoCharactersLeft()
        {
            for (var i = 0; i < Grid.Count; i++)
            {
                for (var j = 0; j < Grid[i].Count; j++)
                {
                    if (Grid[i][j].Type == "Character")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Checks the grid if there are still any monsters left
        public bool NoMonstersLeft()
        {
            for (var i = 0; i < Grid.Count; i++)
            {
                for (var j = 0; j < Grid[i].Count; j++)
                {
                    if (Grid[i][j].Type == "Monster")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
