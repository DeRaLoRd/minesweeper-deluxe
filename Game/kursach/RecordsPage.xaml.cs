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
    /// Логика взаимодействия для RecordsPage.xaml
    /// </summary>
    public partial class RecordsPage : Page
    {
        MenuPage menuPage = null;
        public RecordsPage(MenuPage menu, List<Score> scores)
        {
            InitializeComponent();
            menuPage = menu;
            for (int i = 0; i < scores.Count; i++)
            {
                Label temp = new Label
                {
                    Content = scores[i].difficulty == 9 ? "Легко" : scores[i].difficulty == 12 ? "Нормально" : scores[i].difficulty == 20 ? "Сложно" : "--"
                };

                temp.Foreground = new SolidColorBrush(Colors.White);
                temp.FontSize = 36.0;
                temp.FontFamily = new FontFamily("Courier New Bold");
                temp.HorizontalAlignment = HorizontalAlignment.Center;
                temp.VerticalAlignment = VerticalAlignment.Center;
                RecordsDif.Children.Add(temp);

                Label temp2 = new Label
                {
                    Content = scores[i].time.minutes.ToString() + "m " + scores[i].time.seconds.ToString() + "s"
                }; 
                temp2.Foreground = new SolidColorBrush(Colors.White);
                temp2.FontSize = 36.0;
                temp2.FontFamily = new FontFamily("Courier New Bold");
                temp2.HorizontalAlignment = HorizontalAlignment.Center;
                temp2.VerticalAlignment = VerticalAlignment.Center;
                RecordsTime.Children.Add(temp2);
            }
        }

        public void BackButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(menuPage);
        }
    }
}
