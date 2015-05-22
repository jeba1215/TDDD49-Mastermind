using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MasterMindProject
{
    class CMP
    {
        private Engine engine;
        public String name = "Computer";

        public CMP(){}        

        public void setup(ref Engine _engine)
        {
            engine = _engine;
        }

        public void start()
        {
            while (!engine.isGameOver)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>{
                    while(!engine.isGameOver)
                    {
                        var row = getActiveRow();

                        engine.cmpCalc(row);

                        System.Threading.Thread.Sleep(500);
                    }                   
                }));        
            }
        }

        public SolidColorBrush[] solveRow()
        {
            SolidColorBrush[] row = getActiveRow();
            //Console.WriteLine(row[0]);
            return row;
        }

        private SolidColorBrush[] getActiveRow()
        {
            SolidColorBrush[] row = new SolidColorBrush[4];
            for (int i = 0; i < 4; ++i)
            {
                Ellipse e = (Ellipse) engine.GetGridElement(engine.activeRow, i);
                row[i] = (SolidColorBrush) e.Fill;
                Console.Write(i + ": ");
                Console.WriteLine(row[i]);
            }
            return row;
        }
    }
}
