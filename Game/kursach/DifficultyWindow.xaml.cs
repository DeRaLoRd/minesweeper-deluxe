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
using System.Windows.Shapes;

namespace kursach
{
    /// <summary>
    /// Окно с выбором сложности игры
    /// </summary>
    public partial class DifficultyWindow : Window
    {
        public int Difficulty { get; set; } // Публичное свойство со сложностью, устанавливается непосредственно перед "игрой"
        private int difBuf;                 // "Буфер" сложности, обновляется при каждом нажатии какой нибудь RadioButton

        public DifficultyWindow()
        {
            InitializeComponent();
            Difficulty = 0;
            difBuf = 0;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            Difficulty = difBuf;
            if (Difficulty != 0)
            {
                Close();
            }
        }

        // Вызывается при нажатии на какой нибудь RadioButton
        private void Difficulty_Check(object sender, RoutedEventArgs e)
        {
            RadioButton button = (RadioButton)sender;
            if (button.Content.ToString() == "Новичок")
                difBuf = 9;
            else if (button.Content.ToString() == "Среднячок")
                difBuf = 12;
            else
                difBuf = 20;
        }
    }
}
