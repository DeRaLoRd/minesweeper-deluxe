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
using System.Windows.Threading;

namespace kursach
{
    /// <summary>
    /// Описывает состояние плитки на текущий момент времени
    /// </summary>
    enum TileStatus
    {
        Open,
        Closed,
        Marked
    }

    /// <summary>
    /// Описывает плитку
    /// </summary>
    class Tile
    {
        public GamePage gamePage = null;        // Ссылка на страницу с игрой
        public Button TileButton { get; set; }  // Кнопка вокруг которой построен класс
        public bool Planted { get; set; }       // Заложена ли мина
        public int AroundCount { get; set; }    // Сколько мин вокруг клетки
        public TileStatus tileStatus;          // Состояние конкретной плитки

        public Tile(GamePage gamePage)
        {
            this.gamePage = gamePage;
            TileButton = new Button();
            TileButton.Click += TileOpen;
            TileButton.MouseRightButtonDown += TileMarked;
            tileStatus = TileStatus.Closed;                 // Изначальное состояние плиток
            AroundCount = 0;
            TileUpdate();
        }

        /// <summary>
        /// Инициализирует стиль для плиток (цвета, шрифты и прочее)
        /// </summary>
        /// <returns>Стиль</returns>
        private Style GetStyle(TileStatus status)
        {
            Style tileStyle = new Style();

            // Настройка цветов для стиля плиток
            Color buttonBackground = new Color();
            Color buttonForeground = new Color();
            Color buttonBorder = new Color();

            buttonBackground.A = 255;
            switch (status)
            {
                case TileStatus.Closed:
                    buttonBackground.R = 78;
                    buttonBackground.G = 5;
                    buttonBackground.B = 173;
                    break;

                case TileStatus.Marked:
                    buttonBackground.R = 255;
                    buttonBackground.G = 255;
                    buttonBackground.B = 0;
                    break;

                case TileStatus.Open:
                    buttonBackground.R = 16;
                    buttonBackground.G = 16;
                    buttonBackground.B = 16;
                    break;
            }

            buttonForeground.A = 255;
            switch (status)
            {
                case TileStatus.Closed:
                    buttonForeground.R = 0;
                    buttonForeground.G = 0;
                    buttonForeground.B = 0;
                    break;

                case TileStatus.Marked:
                    buttonForeground.R = 0;
                    buttonForeground.G = 0;
                    buttonForeground.B = 0;
                    break;

                case TileStatus.Open:
                    if (!Planted)
                    {
                        buttonForeground.R = 255;
                        buttonForeground.G = 255;
                        buttonForeground.B = 255;
                    }
                    else
                    {
                        buttonForeground.R = 255;
                        buttonForeground.G = 0;
                        buttonForeground.B = 0;
                    }
                    break;
            }

            buttonBorder.A = 255;
            buttonBorder.R = 16;
            buttonBorder.G = 16;
            buttonBorder.B = 16;

            // Настройка стиля плиток
            tileStyle.Setters.Add(new Setter { Property = Button.BackgroundProperty, Value = new SolidColorBrush(buttonBackground) });
            tileStyle.Setters.Add(new Setter { Property = Button.BorderBrushProperty, Value = new SolidColorBrush(buttonBorder) });
            tileStyle.Setters.Add(new Setter { Property = Button.ForegroundProperty, Value = new SolidColorBrush(buttonForeground) });
            tileStyle.Setters.Add(new Setter { Property = Button.BorderThicknessProperty, Value = new Thickness(1) });
            tileStyle.Setters.Add(new Setter { Property = Button.HorizontalContentAlignmentProperty, Value = HorizontalAlignment.Center });
            tileStyle.Setters.Add(new Setter { Property = Button.VerticalContentAlignmentProperty, Value = VerticalAlignment.Center });
            tileStyle.Setters.Add(new Setter { Property = Button.FontFamilyProperty, Value = new FontFamily("Courier New Bold") });
            tileStyle.Setters.Add(new Setter { Property = Button.FontSizeProperty, Value = 23.0 });

            Trigger temp = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            temp.Setters.Add(new Setter { Property = Button.ForegroundProperty, Value = new SolidColorBrush(Colors.Black) });
            tileStyle.Triggers.Add(temp);

            return tileStyle;
        }

        public void TileUpdate()
        {
            switch (tileStatus)
            {
                case TileStatus.Open:
                    TileButton.Content = Planted ? "*" : 
                                       AroundCount == 0 ? "" : AroundCount.ToString();
                    break;

                case TileStatus.Closed:
                    TileButton.Content = "";
                    break;

                case TileStatus.Marked:
                    TileButton.Content = "!";
                    break;
            }
            TileButton.Style = GetStyle(tileStatus);
        }

        public void TileOpen(object sender, RoutedEventArgs e)
        {
            if (tileStatus == TileStatus.Open && AroundCount != 0)
            {
                gamePage.DoAccord(this);
            }

            else if (tileStatus == TileStatus.Closed)
            {
                tileStatus = TileStatus.Open;
                TileUpdate();

                if (Planted)
                {
                    TileButton.Click -= gamePage.CheckWin;
                    gamePage.DisplayLose();
                }

                if (AroundCount == 0 && !Planted)
                {
                    gamePage.OpenAround(this);
                }
            }
        }

        public void TileMarked(object sender, RoutedEventArgs e)
        {
            if (tileStatus == TileStatus.Open)
                return;
            if (tileStatus == TileStatus.Closed && gamePage.MineAmount > 0)
            {
                tileStatus = TileStatus.Marked;
                gamePage.MineAmountUpdate(-1);
            }
            else if (tileStatus == TileStatus.Marked)
            {
                tileStatus = TileStatus.Closed;
                gamePage.MineAmountUpdate(1);
            }

            TileUpdate();
        }
    }
}
