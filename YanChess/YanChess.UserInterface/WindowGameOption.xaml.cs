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

namespace YanChess.UserInterface.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowGameOption.xaml
    /// </summary>
    public partial class WindowGameOption : Window
    {
        private bool isMomentalChange;
        public WindowGameOption(bool isOpenInTheGame = false)
        {
            InitializeComponent();
            isMomentalChange = isOpenInTheGame;
            strongOfPlay.Value = Engine.EngineOptions.MaxDepth;
            checkBoxDictionary.IsChecked = Engine.EngineOptions.IsUsePositionDictionary;
            checkBoxMultithreading.IsChecked = Engine.EngineOptions.IsMultithread;
            checkBoxStrongScore.IsChecked = !Engine.EngineOptions.IsUseEasyScoreOfPosition;
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(isMomentalChange)
            {
                Engine.EngineOptions.IsMultithread = (bool)checkBoxMultithreading.IsChecked;
                Engine.EngineOptions.IsUseEasyScoreOfPosition = (bool)!checkBoxStrongScore.IsChecked;
                Engine.EngineOptions.MaxDepth = (uint)strongOfPlay.Value;
                Engine.EngineOptions.IsUsePositionDictionary = (bool)checkBoxDictionary.IsChecked;
                this.Close();
                return;
            }
            MainWindow w = new MainWindow((uint)strongOfPlay.Value, (bool)checkBoxMultithreading.IsChecked, (bool)!checkBoxStrongScore.IsChecked, (bool)checkBoxDictionary.IsChecked);
            w.Show();
            this.Close();
        }
    }
}
