using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MasterMindProject
{
    class CMP
    {
        private Engine engine;
        private List<string[]> possibleGuesses;
        private string[] lastGuess;
        private int correctPlacement;
        private int correctColor;
        public String name = "Computer";


        public CMP(){}        

        public void setup(ref Engine _engine)
        {
            engine = _engine;
            reload();
        }

        public void reload()
        {
            possibleGuesses = new List<string[]>();
            generatePowerset();
        }

        public void start()
        {
            Action initialAction = () =>
            {
                initialGuess();                
                System.Threading.Thread.Sleep(1000);
            };

            Application.Current.Dispatcher.BeginInvoke(initialAction);
            System.Threading.Thread.Sleep(1000);

            while (!engine.isGameOver)
            {
                Action action = () =>
                    {
                        solveRow();
                        Random r = new Random();
                        int index = r.Next(possibleGuesses.Count());
                        try
                        {
                            string[] guess = possibleGuesses[index];
                            lastGuess = guess;
                            engine.cmpCalc(guess);
                        }
                        catch(System.ArgumentOutOfRangeException e)
                        {
                            Console.WriteLine("Index: " + index);
                            Console.WriteLine("Count: " + possibleGuesses.Count());
                        }
                    };  
                Application.Current.Dispatcher.BeginInvoke(action);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void initialGuess()
        {
            string[] row = new string[4];
            row[0] = engine.page.getGameColor(0);
            row[1] = engine.page.getGameColor(0);
            row[2] = engine.page.getGameColor(1);
            row[3] = engine.page.getGameColor(1);

            engine.cmpCalc(row);
            possibleGuesses.Remove(row);
            lastGuess = row;
        }

        public string[] solveRow()
        {
            string[] row = getActiveRow();
            getGuessResult();
            //Console.WriteLine(possibleGuesses.Count);
            for(int i=possibleGuesses.Count-1; i>=0; --i)
                excludeImpossibleGuesses(possibleGuesses[i]);
            return row;
        }

        private string[] getActiveRow()
        {
            string[] row = new string[4];
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("Getting color: " + i + " " + engine.activeRow);
                row[i] = engine.page.getColor(engine.activeRow, i);
            }
            return row;
        }

        private void getGuessResult()
        {
            string[] row = new string[4];
            int tempCorrectPlacement = 0;
            int tempCorrectColor = 0;
            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine("Getting color: " + i + 4 + " " + (engine.activeRow -1));
                row[i] = engine.page.getColor(engine.activeRow - 1, i + 4);
            }
            for (int i = 0; i < row.Length; ++i)
            {
                //Console.WriteLine(row[i]);
                if (row[i] == engine.page.getGameColor(1))
                {
                    tempCorrectPlacement++;
                }
                else if (row[i] == engine.page.getGameColor(0))
                {
                    tempCorrectColor++;
                }
            }
            correctPlacement = tempCorrectPlacement;
            correctColor = tempCorrectColor;

            //Console.WriteLine(correctPlacement + " Gröna. " + correctColor + " Röda.");
        }

        private void generatePowerset()
        {
            string[] guess = new string[4];           
            foreach (var c in CombinationsWithRepition(new int[] { 0, 1, 2 }, 4))
            {
                int j = 0;
                //Console.WriteLine(int.Parse(c));
                foreach (char i in c)
                {
                    guess[j] = engine.page.getGameColor(int.Parse(i.ToString()));
                    ++j;
                }
                //Console.WriteLine(guess[0] + " " + guess[1] + " " + guess[2] + " " + guess[3]);
                possibleGuesses.Add(new string[] {guess[0], guess[1], guess[2], guess[3]} );
            }            
        }

        private void excludeImpossibleGuesses(string[] potentialGuess)
        {
            int placement = 0, color = 0;
            ArrayList vacantColorPlacements = new ArrayList();
            vacantColorPlacements.Add(0);
            vacantColorPlacements.Add(1);
            vacantColorPlacements.Add(2);
            vacantColorPlacements.Add(3);

            for (int i = 0; i < 4; i++)
            {
                if (potentialGuess[i] == lastGuess[i])
                    placement++;
            }
            for (int i = 0; i < 4; ++i)
            {
                foreach (int j in vacantColorPlacements)
                {
                    if (potentialGuess[i] == lastGuess[j])
                    {
                        ++color;
                        vacantColorPlacements.Remove(j);
                        break;
                    }
                }
            }
            if (placement != correctPlacement && color != correctColor)
                possibleGuesses.Remove(potentialGuess);
        }

        private IEnumerable<String> CombinationsWithRepition(IEnumerable<int> input, int length)
        {
            if (length <= 0)
                yield return "";
            else
            {
                foreach (var i in input)
                    foreach (var c in CombinationsWithRepition(input, length - 1))
                        yield return i + c;
            }
        }        
    }
}
