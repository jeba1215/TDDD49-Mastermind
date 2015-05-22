using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterMindProject
{
    interface IGUI
    {
        string getGameColor(int i);
        string getColor(int row, int col);
        void setColor(int row, int col, string color);
        void clearBoard();
        string findNextColor(string color);
        void gameOver(string name);
    }
}
