using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows;

namespace MasterMindProject
{

    class GameData
    {
        public int activeRow { get; set; }
        public int turn { get; set; }
        public List<List<string>> brushList = new List<List<string>>();
        public string[] keys = new string[4];
        static public string filePath = @"c:\Users\Jesper\Desktop\data.json";
        static public string folderPath = @"c:\Users\Jesper\Desktop\";

        public GameData() { }

        public GameData(int _activeRow, int _turn)
        {
            activeRow = _activeRow;
            turn = _turn;
        }
    }

    class Engine
    {
        Player player;
        CMP cmp;
        public int activeRow = 0;
        private int firstScore = 0;
        public int turn = 0;
        public int amountOfColors = 3;
        private bool correctGuess = false;
        private string[] key;
        public IGUI page;
        private Thread t;
        FileSystemWatcher watcher = new FileSystemWatcher();

        public bool isGameOver = false;
        public string gameOverString = " ";

        public Engine(IGUI p, ref Player _player, ref CMP _cmp)
        {
            player = _player;
            cmp = _cmp;         

            page = p;
            key = new string[4];

            setupWatcher();

            //prepareNewRow();
            loadData();
            Console.WriteLine("KEY");
            foreach (var x in key)
            {
                Console.WriteLine(x);
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void setupWatcher()
        {
            watcher.Path = GameData.folderPath;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Filter = "data.json";
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);

            Action action = () =>
            {
                loadData();
            };
            Application.Current.Dispatcher.BeginInvoke(action);
            watcher.EnableRaisingEvents = false;
        }

        private void prepareNewRow()
        {
            for (int i = 0; i < 4; ++i)
            {
                page.setColor(activeRow, i, page.getGameColor(0));
            }
        }

        private void switchPlayer()
        {
            firstScore = activeRow;
            correctGuess = false;
            page.clearBoard();
            activeRow = 0;
            ++turn;
            prepareNewRow();
            startCMP();
        }

        private void startCMP()
        {
            t = new Thread(cmp.start);
            t.Start();
        }

        public void cmpCalc(string[] row)
        {
            //Console.WriteLine("in cmpcalc");
            for (int i = 0; i < 4; ++i)
            {
                page.setColor(activeRow, i, row[i]);
            }
            guessClick();                
        }

        private void gameOver()
        {
            if (firstScore < activeRow)
            {
                Console.WriteLine("Player 1 won the game");
                gameOverString = player.name + " won the game";
            }
            else if (firstScore > activeRow)
            {
                Console.WriteLine("Player 2 won the game");
                gameOverString = cmp.name + " won the game";
            }
            else
            {
                Console.WriteLine("It's a draw");
                gameOverString = "It's a draw";
            }
            reset();
            saveData();
            page.gameOver(gameOverString);
        }

        public void reset()
        {
            resetData();
            setKey();
            cmp.reload();
            Console.WriteLine("KEY");
            foreach (var x in key)
            {
                Console.WriteLine(x);
            }
        }

        public void resetData()
        {
            page.clearBoard();
            correctGuess = false;
            turn = 0;
            activeRow = 0;
            isGameOver = false;
            prepareNewRow();
        }     

        public void guessClick()
        {
            calcGuesses();            

            if (correctGuess || activeRow == 11)
            {
                if (turn == 0)
                {
                    switchPlayer();
                }
                else
                {
                    isGameOver = true;
                    t.Abort();
                    gameOver();
                }
                
            }                
            else
            {
                activeRow++;
                prepareNewRow();
            }
        }

        public void gridClick(int row, int col)
        {
            //Console.WriteLine("Clicked: " + row + " " + col);
            //Console.WriteLine("Check: " + activeRow);
            if (row == activeRow && col < 4)
            {
                //Console.WriteLine("gridclick true");

                string next_color = page.findNextColor(page.getColor(row, col));
                page.setColor(row, col, next_color);
                saveData();
            }
        }

        private void calcGuesses()
        {
            //Console.WriteLine("in calcguesses");
            int correctPlacements = 0;
            int correctColors = 0;
            ArrayList vacantColorPlacements = new ArrayList();            
            vacantColorPlacements.Add(0);
            vacantColorPlacements.Add(1);
            vacantColorPlacements.Add(2);
            vacantColorPlacements.Add(3);

            List<int> keycopy = new List<int>();
            keycopy.Add(0);
            keycopy.Add(1);
            keycopy.Add(2);
            keycopy.Add(3);
            
            for (int i = 0; i < 4; ++i)
            {
                string cell = page.getColor(activeRow, i);
                if (cell == key[i])
                {
                    ++correctPlacements;
                    vacantColorPlacements.Remove(i);
                    keycopy.Remove(i);
                }
            }
            for (int i=0; i<4; ++i)
            {
                if (!keycopy.Contains(i))
                {
                    continue;
                }
                string cell = page.getColor(activeRow, i);
                foreach(int j in vacantColorPlacements)
                {
                    if (cell == key[j])
                    {
                        ++correctColors;
                        vacantColorPlacements.Remove(j);
                        keycopy.Remove(i);
                        break;
                    }          
                }
            }

            //Console.WriteLine("Color: " + correctColors);
            //Console.WriteLine("Placement: " + correctPlacements);

            for (int i = 0; i < correctPlacements; ++i)
            {
                page.setColor(activeRow, i + 4, page.getGameColor(1));
            }
            for (int i = correctPlacements; i < correctPlacements + correctColors; ++i)
            {              
                page.setColor(activeRow, i + 4, page.getGameColor(0));
            }

            if (correctPlacements >= 4)
                correctGuess = true;
        }

        public void setKey()
        {
            Random rnd = new Random();
            for (int i = 0; i < 4; ++i)
            {
                key[i] = page.getGameColor(rnd.Next(0, amountOfColors));
            }            
        }

        public void saveData()
        {
            var data = new GameData();
            data.activeRow = activeRow;
            data.turn = turn;
            //Console.WriteLine("Saving active row as: " + activeRow);
            for (int i = 0; i < 12; ++i)
            {
                data.brushList.Add(new List<string>() );
                for (int j = 0; j < 4; ++j)
                {
                    data.brushList[i].Add(page.getColor(i, j));
                }
            }
            for (int i = 0; i < 4; ++i)
            {
                data.keys[i] = key[i];
            }

            string output = JsonConvert.SerializeObject(data);
            //Console.WriteLine(output);
            File.WriteAllText(GameData.filePath, output);
            watcher.EnableRaisingEvents = true;
        }

        public void loadData()
        {
            JObject o = new JObject();
            GameData data = new GameData();
            if (!File.Exists(GameData.filePath))
            {
                Console.WriteLine("File doesnt exist");
                prepareNewRow();
                setKey();
                return;
            }
            using (var stream = File.Open(GameData.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                }
            }
            
            //Console.WriteLine(o);
            resetData();

            var guesses = from arr in o["brushList"]
                          select arr;

            var keys = from k in o["keys"]
                       select k;

            var _activeRow = o["activeRow"];

            int row = 0;
            foreach (var x in keys)
            {
                key[row] = x.ToString();
                ++row;
            }

            row = 0;            
            foreach (var x in guesses)
            {
                for (int i = 0; i < 4; ++i)
                {
                    page.setColor(row, i, x[i].ToString());
                }
                if (row.ToString() == _activeRow.ToString())
                {
                    break;
                }
                else
                {
                    guessClick();
                    ++row;
                    activeRow = row;
                }
            }
            //Console.WriteLine("Active row: " + _activeRow);        
            watcher.EnableRaisingEvents = true;
        }
    }
}
