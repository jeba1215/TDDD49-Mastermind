using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MasterMindProject
{
    class Player
    {
        public String name = "Player";
        Engine engine;

        public Player(){}

        public void setup(ref Engine _engine)
        {
            engine = _engine;
        }

        public void click(ref Grid grid, ref Point point)
        {
            if (engine.turn != 0)
                return;
            int row = 0;
            int col = 0;
            double accumulatedHeight = 0.0;
            double accumulatedWidth = 0.0;

            // calc row mouse was over
            foreach (var rowDefinition in grid.RowDefinitions)
            {
                accumulatedHeight += rowDefinition.ActualHeight;
                if (accumulatedHeight >= point.Y)
                    break;
                row++;
            }

            // calc col mouse was over
            foreach (var columnDefinition in grid.ColumnDefinitions)
            {
                accumulatedWidth += columnDefinition.ActualWidth;
                if (accumulatedWidth >= point.X)
                    break;
                col++;
            }            
            engine.gridClick(row, col);
        }

        public void getKey()
        {

        }
    }
}
