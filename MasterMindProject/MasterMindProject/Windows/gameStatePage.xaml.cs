using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MasterMindProject
{
    /// <summary>
    /// Interaction logic for gameStatePage.xaml
    /// </summary>
    public partial class gameStatePage : Page, IGUI
    {
        Engine engine;
        Player player;
        CMP cmp;

        public SolidColorBrush[] colors = new SolidColorBrush[3];

        public gameStatePage()
        {
            InitializeComponent();

            colors[0] = new SolidColorBrush();
            colors[0].Color = Color.FromArgb(255, 255, 0, 0);
            colors[1] = new SolidColorBrush();
            colors[1].Color = Color.FromArgb(255, 0, 255, 0);
            colors[2] = new SolidColorBrush();
            colors[2].Color = Color.FromArgb(255, 0, 0, 255);

            player = new Player();
            player.name = "Player";

            cmp = new CMP();

            engine = new Engine(this, ref player, ref cmp);

            player.setup(ref engine);
            cmp.setup(ref engine);            
        }

        private void pauseClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new pauseMenuPage());
        }

        private void guessClick(object sender, RoutedEventArgs e)
        {
            engine.guessClick();
            if(engine.turn == 0) engine.saveData();
        }

        public void gameOver(String str)
        {
            this.NavigationService.Navigate(new gameOverPage(str));
        }

        private void resetClick(object sender, RoutedEventArgs e)
        {
            engine.reset();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            engine.saveData();
        }

        private void loadClick(object sender, RoutedEventArgs e)
        {
            engine.loadData();
        }

        private void gridClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var point = Mouse.GetPosition(grid);            
            player.click(ref grid, ref point);            
        }

        public string getGameColor(int i)
        {
            return colors[i].ToString();
        }

        public string findNextColor(string c)
        {
            for (int i = 0; i < colors.Length; ++i)
            {
                //This should always be true
                if (c == colors[i].ToString())
                {
                    //Console.WriteLine("Found color: "+i);
                    return colors[(i + 1) % colors.Length].ToString();
                }
            }
            Console.WriteLine("Couldn't find color");
            return c;
        }

        public void setColor(int row, int col, string color)
        {
            Ellipse e = (Ellipse)GetGridElement(row, col);
            e.Fill = (SolidColorBrush) new BrushConverter().ConvertFromString(color);
        }

        public string getColor(int row, int col)
        {
            var e = (Ellipse) GetGridElement(row, col);
            Console.WriteLine("" + row + " " + col);
            return e.Fill.ToString();
        }
        
        private UIElement GetGridElement(int r, int c)
        {
            for (int i = 0; i < grid.Children.Count; i++)
            {
                UIElement e = grid.Children[i];
                if (Grid.GetRow(e) == r && Grid.GetColumn(e) == c)
                    return e;
            }
            return null;
        }        

        public void clearBoard()
        {
            for (int i = 0; i < 12; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    Ellipse ellipse = (Ellipse)GetGridElement(i, j);
                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Color.FromArgb(255, 255, 255, 255);
                    ellipse.Fill = brush;
                }
            }
        }
    }
}