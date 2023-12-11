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
    public struct Score
    {
        public int difficulty;
        public TimePassed time;

        public Score(int diff, TimePassed time)
        {
            difficulty = diff;
            this.time = time;
        }
    }

    /// <summary>
    /// Страница с меню
    /// </summary>
    public partial class MenuPage : Page
    {
        private DifficultyWindow difficultyWindow = null;
        public List<Score> scoreList;

        public MenuPage(MainWindow mainWin)
        {
            InitializeComponent();

            scoreList = new List<Score>();
            ScoreHandler.ReadAll(scoreList);
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
            NavigationService.Navigate(new RecordsPage(this, scoreList));
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            ScoreHandler.WriteAll(scoreList);
            Application.Current.Shutdown();
        }
    }
}
