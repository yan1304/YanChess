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

namespace YanChess.UserInterface
{
    /// <summary>
    /// Логика взаимодействия для WindowConvertPeen.xaml
    /// </summary>
    public partial class WindowConvertPeen : Window
    {
        public Square Figure { get; set; }
        
        public WindowConvertPeen()
        {
            
            InitializeComponent();
            if(!GameLogic.GameLogic.GamePosition.IsWhiteMove)
            {
                Q.Background= new ImageBrush(new BitmapImage(new Uri(@"Resourses\WQ.png", UriKind.Relative)));
                R.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\WR.png", UriKind.Relative)));
                B.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\WB.png", UriKind.Relative)));
                N.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\WN.png", UriKind.Relative)));
            }
            else
            {
                Q.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\BQ.png", UriKind.Relative)));
                R.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\BR.png", UriKind.Relative)));
                B.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\BB.png", UriKind.Relative)));
                N.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\BN.png", UriKind.Relative)));
            }
        }

        private void K_Click(object sender, RoutedEventArgs e)
        {
            if (!GameLogic.GameLogic.GamePosition.IsWhiteMove)
            {
                switch (((Button)e.OriginalSource).Name)
                {
                    case "Q": Figure = new Square(new Queen(ColorFigur.white));break;
                    case "R": Figure = new Square(new Rock(ColorFigur.white)); break;
                    case "B": Figure = new Square(new Bishop(ColorFigur.white)); break;
                    case "N": Figure = new Square(new Knight(ColorFigur.white)); break;
                    case "P": Figure = new Square(new Peen(ColorFigur.white)); break;
                }
            }
            else
            {
                switch (((Button)e.OriginalSource).Name)
                {
                    case "Q": Figure = new Square(new Queen(ColorFigur.black)); break;
                    case "R": Figure = new Square(new Rock(ColorFigur.black)); break;
                    case "B": Figure = new Square(new Bishop(ColorFigur.black)); break;
                    case "N": Figure = new Square(new Knight(ColorFigur.black)); break;
                    case "P": Figure = new Square(new Peen(ColorFigur.black)); break;
                }
            }
            DialogResult = true;
        }
    }
}
