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

namespace kursach
{
    /// <summary>
    /// Страница с меню
    /// </summary>
    public partial class MenuPage : Page
    {
        // Ссылка на главное окно (для вызова у него метода закрытия) 
        private MainWindow mainWindow = null;
        private DifficultyWindow difficultyWindow = null;

        public MenuPage(MainWindow mainWin)
        {
            InitializeComponent();
            mainWindow = mainWin;
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            difficultyWindow = new DifficultyWindow();
            difficultyWindow.ShowDialog();
            if (difficultyWindow.Difficulty == 0)
                return;
            NavigationService.Navigate(new GamePage(this, difficultyWindow.Difficulty));
        }

        private void Scores_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("В разработке");
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
