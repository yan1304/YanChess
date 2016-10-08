using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YanChess.UserInterface.Windows;
using YanChess.Engine;

namespace YanChess.UserInterface
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EngineOptions.MaxDepth = 4;
            EngineOptions.IsMultithread = true;
            EngineOptions.IsUseEasyScoreOfPosition = false;
            EngineOptions.IsUsePositionDictionary = true;
        }

        public MainWindow(uint maxDepth,bool isMultithread, bool isEasyScore, bool isUseDictionary)
        {
            InitializeComponent();
            EngineOptions.MaxDepth = maxDepth;
            if (maxDepth > 5) EngineOptions.MaxDepth = 99;
            EngineOptions.IsMultithread = isMultithread;
            EngineOptions.IsUseEasyScoreOfPosition = isEasyScore;
            EngineOptions.IsUsePositionDictionary = isUseDictionary;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            Option b = new Option();
            b.Show();
            this.Close();
        }

        private void buttonInsertPosition_Click(object sender, RoutedEventArgs e)
        {
            WindowEditPosition w = new WindowEditPosition();
            w.Show();
            this.Close();
        }

        private void buttonOption_Click(object sender, RoutedEventArgs e)
        {
            WindowGameOption w = new WindowGameOption();
            w.Show();
            this.Close();
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
