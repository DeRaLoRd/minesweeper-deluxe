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

namespace kursach
{
    /// <summary>
    /// Главное окно
    /// </summary>
    public partial class MainWindow : Window
    {
        MenuPage menuPage = null;

        // Выделение памяти под окно с выбором сложности и под страницу с главным меню
        public MainWindow()
        {
            InitializeComponent();
            menuPage = new MenuPage(this);

            MainFrame.Content = menuPage;
        }
    }
}
