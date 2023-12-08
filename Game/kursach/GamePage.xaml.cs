﻿using System;
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

    struct TimePassed
    {
        public TimePassed(int min, int sec)
        {
            minutes = min;
            seconds = sec;
        }
        public int minutes;
        public int seconds;
        public override string ToString()
        {
            return (minutes >= 10 ? "" : "0") + $"{minutes}:" + (seconds >= 10 ? "" : "0") + $"{seconds}";
        }
    }

    /// <summary>
    /// Логика взаимодействия для GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {
        MenuPage menuPage = null;            // Ссылка на страницу с главным меню для навигации назад
        public int Difficulty { get; set; }  // Сложность переданная из MenuPage
        public int MineAmount { get; set; }  // Количество непомеченных мин на поле
        private int mineOverallAmount = 0;      // Количество мин в принципе
        TimePassed timePassed;               // Время решения уровня
        DispatcherTimer timer = null;       // Для реализации секундомера
        internal List<Tile> tileList = null;  // Список мин

        public GamePage(MenuPage menu, int diff)
        {
            InitializeComponent();
            menuPage = menu;
            Difficulty = diff;
            MineAmount = 0;
            timePassed = new TimePassed(0, 0);          // Установка времени
            timer = new DispatcherTimer();              // Создание таймера
            timer.Interval = TimeSpan.FromSeconds(1);   // Настройка на срабатывание каждую секунду
            timer.Tick += Timer_Tick;                   // Обработчик события срабатывания - Timer_Tick()

            // Настройка margin игрового поля, чтобы было более-менее красиво
            // Значения подобраны экспериментально и должны удовлетворять формуле:
            // Thickness(a, 20, a, 20);
            // где а = ((600 - (Difficulty * 25)) / 2) - 6
            // для 20 другое
            switch (Difficulty)
            {
                case 9:
                    GameField.Margin = new Thickness(181.5, 20, 181.5, 20);
                    break;
                case 12:
                    GameField.Margin = new Thickness(144, 20, 144, 20);
                    break;
                case 20:
                    GameField.Margin = new Thickness(44, 20, 44, 20);
                    break;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timePassed.seconds < 59)
                timePassed.seconds++;
            else
            {
                timePassed.minutes++;
                timePassed.seconds = 0;
            }
            TimeLabel.Content = timePassed.ToString();
        }

        /// <summary>
        /// Заполняет игровое поле минами в зависимости от сложности
        /// </summary>
        /// <param name="field">Ссылка на список плиток Tile</param>
        /// <param name="difficulty">Сложность (9, 12, 20)</param>
        /// <returns>Список Tile, проинициализированный минами</returns>
        private void FillField(List<Tile> field, int difficulty)
        {
            Random rng = new Random();
            double chance;
            foreach (var item in field)
            {
                chance = rng.NextDouble();
                if (difficulty == 9 && chance <= 0.1 ||
                    difficulty == 12 && chance <= 0.2 ||
                    difficulty == 20 && chance <= 0.25)
                {
                    MineAmount++;
                    mineOverallAmount++;
                    item.Planted = true;
                    item.TileUpdate();
                }
            }
        }

        /// <summary>
        /// Выставляет значения клеточкам
        /// </summary>
        /// <param name="field">Ссылка на поле</param>
        /// <param name="difficulty">Сложность</param>
        private void SetAroundCounts(List<Tile> field, int difficulty)
        {
            int rows = difficulty == 20 ? 12 : difficulty;

            if (field[1].Planted) field[0].AroundCount++;
            if (field[difficulty].Planted) field[0].AroundCount++;
            if (field[difficulty + 1].Planted) field[0].AroundCount++;
            field[0].TileUpdate();

            for (int i = 1; i < difficulty - 1; i++)
            {
                if (field[i - 1].Planted) field[i].AroundCount++;
                if (field[i + 1].Planted) field[i].AroundCount++;
                if (field[i + difficulty - 1].Planted) field[i].AroundCount++;
                if (field[i + difficulty].Planted) field[i].AroundCount++;
                if (field[i + difficulty + 1].Planted) field[i].AroundCount++;
                field[i].TileUpdate();
            }

            if (field[difficulty - 2].Planted) field[difficulty - 1].AroundCount++;
            if (field[2 * difficulty - 2].Planted) field[difficulty - 1].AroundCount++;
            if (field[2 * difficulty - 1].Planted) field[difficulty - 1].AroundCount++;
            field[difficulty - 1].TileUpdate();

            for (int i = 1; i < rows - 1; i++)
            {
                int d = difficulty * i;

                if (field[d - difficulty].Planted) field[d].AroundCount++;
                if (field[d - difficulty + 1].Planted) field[d].AroundCount++;
                if (field[d + 1].Planted) field[d].AroundCount++;
                if (field[d + difficulty].Planted) field[d].AroundCount++;
                if (field[d + difficulty + 1].Planted) field[d].AroundCount++;
                field[d].TileUpdate();

                for (int j = 1; j < difficulty - 1; j++)
                {
                    if (field[d + j - difficulty - 1].Planted) field[d + j].AroundCount++;
                    if (field[d + j - difficulty].Planted) field[d + j].AroundCount++;
                    if (field[d + j - difficulty + 1].Planted) field[d + j].AroundCount++;
                    if (field[d + j - 1].Planted) field[d + j].AroundCount++;
                    if (field[d + j + 1].Planted) field[d + j].AroundCount++;
                    if (field[d + j + difficulty - 1].Planted) field[d + j].AroundCount++;
                    if (field[d + j + difficulty].Planted) field[d + j].AroundCount++;
                    if (field[d + j + difficulty + 1].Planted) field[d + j].AroundCount++;
                    field[d + j].TileUpdate();
                }

                if (field[d - 2].Planted) field[d + difficulty - 1].AroundCount++;
                if (field[d - 1].Planted) field[d + difficulty - 1].AroundCount++;
                if (field[d + difficulty - 2].Planted) field[d + difficulty - 1].AroundCount++;
                if (field[d + 2 * difficulty - 2].Planted) field[d + difficulty - 1].AroundCount++;
                if (field[d + 2 * difficulty - 1].Planted) field[d + difficulty - 1].AroundCount++;
                field[d + difficulty - 1].TileUpdate();
            }

            if (field[difficulty * (rows - 2)].Planted) field[difficulty * (rows - 1)].AroundCount++;
            if (field[difficulty * (rows - 2) + 1].Planted) field[difficulty * (rows - 1)].AroundCount++;
            if (field[difficulty * (rows - 1) + 1].Planted) field[difficulty * (rows - 1)].AroundCount++;
            field[difficulty * (rows - 1)].TileUpdate();

            for (int i = difficulty * (rows - 1) + 1; i < difficulty * rows - 1; i++)
            {
                if (field[i - difficulty - 1].Planted) field[i].AroundCount++;
                if (field[i - difficulty].Planted) field[i].AroundCount++;
                if (field[i - difficulty + 1].Planted) field[i].AroundCount++;
                if (field[i - 1].Planted) field[i].AroundCount++;
                if (field[i + 1].Planted) field[i].AroundCount++;
                field[i].TileUpdate();
            }

            if (field[difficulty * rows - 2].Planted) field[difficulty * rows - 1].AroundCount++;
            if (field[difficulty * (rows - 1) - 2].Planted) field[difficulty * rows - 1].AroundCount++;
            if (field[difficulty * (rows - 1) - 1].Planted) field[difficulty * rows - 1].AroundCount++;
            field[difficulty * rows - 1].TileUpdate();
        }

        internal void OpenAround(Tile tile)
        {
            int i = 0;
            while (tileList[i] != tile)
                i++;

            OpenAround(tile, i);
        }

        internal void OpenAround(Tile tile, int cur/*, int prev*/)
        {
            int rows = Difficulty == 20 ? 12 : Difficulty;

            tile.tileStatus = TileStatus.Open;
            tile.TileUpdate();

            // Базис рекурсии
            if (tile.AroundCount != 0 || tile.Planted) return;

            if (cur == 0)
            {
                if (tileList[1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[1], 1);
                if (tileList[Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[Difficulty], Difficulty);
                if (tileList[Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[Difficulty + 1], Difficulty + 1);
            }
            else if (cur < Difficulty - 1)
            {
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1], cur - 1);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + 1], cur + 1);
                if (tileList[cur - 1 + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1 + Difficulty], cur - 1 + Difficulty);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty], cur + Difficulty);
                if (tileList[cur + 1 + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + 1 + Difficulty], cur + 1 + Difficulty);
            }
            else if (cur == Difficulty - 1)
            {
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1], cur - 1);
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty - 1], cur + Difficulty - 1);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty], cur + Difficulty);
            }
            else if ((cur % Difficulty == 0) && (cur != Difficulty * (rows - 1)))
            {
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty], cur - Difficulty);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty + 1], cur - Difficulty + 1);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + 1], cur + 1);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty], cur + Difficulty);
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty + 1], cur + Difficulty + 1);
            }
            else if ((cur + 1) % Difficulty == 0 && cur != (Difficulty * rows - 1))
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty - 1], cur - Difficulty - 1);
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty], cur - Difficulty);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1], cur - 1);
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty - 1], cur + Difficulty - 1);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty], cur + Difficulty);
            }
            else if (cur == Difficulty * (rows - 1))
            {
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty], cur - Difficulty);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty + 1], cur - Difficulty + 1);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur+ 1], Difficulty + 1);
            }
            else if (cur > Difficulty * (rows - 1) && cur < Difficulty * rows - 1)
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty - 1], cur - Difficulty - 1);
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty], cur - Difficulty);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty + 1], cur - Difficulty + 1);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1], cur - 1);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + 1], cur + 1);
            }
            else if (cur == Difficulty * rows - 1)
            {
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty], cur - Difficulty);
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty - 1], cur - Difficulty - 1);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1], cur - 1);
            }
            else
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty - 1], cur - Difficulty - 1);
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty], cur - Difficulty);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - Difficulty + 1], cur - Difficulty + 1);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur - 1], cur - 1);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + 1], cur + 1);
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty - 1], cur + Difficulty - 1);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty], cur + Difficulty);
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Closed)
                    OpenAround(tileList[cur + Difficulty + 1], cur + Difficulty + 1);
            }
        }

        internal void DoAccord(Tile tile)
        {
            int cur = 0;
            while (tileList[cur] != tile)
                cur++;

            int rows = Difficulty == 20 ? 12 : Difficulty;
            int markedAmount = 0;

            List<Tile> list = new List<Tile>();

            if (cur == 0)
            {
                if (tileList[cur + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + 1]);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty]);
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty + 1]);
            }

            else if (cur > 0 && cur < Difficulty - 1)
            {
                if (tileList[cur - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - 1]);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + 1]);
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty - 1]);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty]);
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty + 1]);
            }

            else if (cur == Difficulty - 1)
            {
                if (tileList[cur - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - 1]);
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty - 1]);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty]);
            }

            else if (cur % Difficulty == 0 && cur != Difficulty * (rows - 1))
            {
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty]);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty + 1]);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + 1]);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty]);
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty + 1]);
            }

            else if ((cur + 1) % Difficulty == 0 && cur != Difficulty * rows - 1)
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty - 1]);
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty]);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - 1]);
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty - 1]);
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty]);
            }

            else if (cur == Difficulty * (rows - 1))
            {
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty]);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty + 1]);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + 1]);
            }

            else if (cur > Difficulty * (rows - 1) && cur < Difficulty * rows - 1)
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty - 1]);
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty]);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty + 1]);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur  - 1]);
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + 1]);
            }

            else if (cur == Difficulty * rows - 1)
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty - 1]);
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty]);
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - 1]);
            }

            else
            {
                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Marked)
                    markedAmount++;
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Marked)
                    markedAmount++;

                if (markedAmount != tile.AroundCount)
                    return;

                if (tileList[cur - Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty - 1]); 
                if (tileList[cur - Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty]);
                if (tileList[cur - Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - Difficulty + 1]); 
                if (tileList[cur - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur - 1]); 
                if (tileList[cur + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + 1]); 
                if (tileList[cur + Difficulty - 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty - 1]); 
                if (tileList[cur + Difficulty].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty]); 
                if (tileList[cur + Difficulty + 1].tileStatus == TileStatus.Closed)
                    list.Add(tileList[cur + Difficulty + 1]);
            }

            foreach (var item in list)
            {
                item.tileStatus = TileStatus.Open;
                item.TileUpdate();
                if (item.Planted)
                    DisplayLose();
            }
        }

        // Генерация игрового поля
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Start();

            int tileAmount = Difficulty == 9 ? 81 : Difficulty == 12 ? 144 : 240;
            tileList = new List<Tile>();
            for (int i = 0; i < tileAmount; i++)
            {
                Tile temp = new Tile(this);
                GameField.Children.Add(temp.TileButton);
                tileList.Add(temp);
            }

            FillField(tileList, Difficulty);
            SetAroundCounts(tileList, Difficulty);
            MineLabel.Content = MineAmount.ToString();
        }

        public void CheckWin(object sender, EventArgs e)
        {
            int rows = Difficulty == 20 ? 12 : Difficulty;
            for (int i = 0; i < Difficulty * rows - 1; i++)
                if (tileList[i].tileStatus != TileStatus.Open && !tileList[i].Planted)
                    return;

            DisplayWin();
        }

        private void DisplayWin()
        {
            MessageBox.Show("Изи победа");
        }

        public void DisplayLose()
        {
            MessageBox.Show("Лошара");
        }

        // Обновление значения мин
        public void MineAmountUpdate(int delta)
        {
            MineAmount += delta;
            MineLabel.Content = MineAmount.ToString();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            MessageBox.Show("Поле было открыто " + timePassed.ToString() + " <- вот столько времени");
            NavigationService.Navigate(menuPage);
        }
    }
}
