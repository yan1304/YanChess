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
using YanChess.GameLogic;

namespace YanChess.UserInterface.Windows
{
    /// <summary>
    /// Логика взаимодействия для Option.xaml
    /// </summary>
    public partial class Option : Window
    {
        Position pos;
        public Option(Position p = null)
        {
            InitializeComponent();
            if (p == null) pos = new Position("start");
            else pos = (Position)p.DeepCopy();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            WindowBoard wb = new WindowBoard(pos,option.GetWhiteTime(),option.GetBlackTime(),option.isWhitePlayer(),option.IsComputer(),!option.isWhitePlayer());
            wb.Show();
            this.Close();
        }

        private void buttonOption_Click(object sender, RoutedEventArgs e)
        {
            WindowGameOption w = new WindowGameOption(true);
            w.Show();
        }
    }
}
