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
    /// Interaction logic for gameOverPage.xaml
    /// </summary>
    public partial class gameOverPage : Page
    {

        public gameOverPage(String msg)
        {
            InitializeComponent();
            textBlock.Text = msg;
        }

        private void resetClick(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new gameStatePage());
            this.NavigationService.GoBack();
        }
    }
}
