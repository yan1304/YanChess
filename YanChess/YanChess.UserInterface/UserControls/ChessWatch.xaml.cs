using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YanChess.GameLogic;

namespace YanChess.UserInterface
{
    /// <summary>
    /// Логика взаимодействия для ChessWatch.xaml
    /// </summary>
    public partial class ChessWatch : UserControl
    {
        public ColorFigur Color { get; set; }
        public ChessWatch()
        {
            InitializeComponent();
            h1.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\0.png", UriKind.Relative)));
            h2.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\0.png", UriKind.Relative)));
            m1.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\0.png", UriKind.Relative)));
            m2.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\0.png", UriKind.Relative)));
            s1.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\0.png", UriKind.Relative)));
            s2.Background = new ImageBrush(new BitmapImage(new Uri(@"Resourses\0.png", UriKind.Relative)));
        }
        //вывод времени
        public void UpdateTime(TimeSpan time)
        {
            int h = 0;
            int m = 0;
            int s = 0;
            h = time.Hours;
            m = time.Minutes;
            s = time.Seconds;
            if(h>9)
            {
                h1.Background = IntToImg(h - (h % 10));
                h2.Background = IntToImg(h % 10);
            }
            else if(h>0)
            {
                h1.Background = IntToImg(0);
                h2.Background = IntToImg(h);
            }
            else
            {
                h1.Background = IntToImg(0);
                h2.Background = IntToImg(0);
            }
            if (m > 9)
            {
                m1.Background = IntToImg(m-(m%10));
                m2.Background = IntToImg(m%10);
            }
            else if (m > 0)
            {
                m1.Background = IntToImg(0);
                m2.Background = IntToImg(m);
            }
            else
            {
                m1.Background = IntToImg(0);
                m2.Background = IntToImg(0);
            }
            if (s > 9)
            {
                s1.Background = IntToImg(s - (s % 10));
                s2.Background = IntToImg(s % 10);
            }
            else if (s > 0)
            {
                s1.Background = IntToImg(s - (s % 10));
                s2.Background = IntToImg(s % 10);
            }
            else
            {
                s1.Background = IntToImg(s - (s % 10));
                s2.Background = IntToImg(s % 10);
            }
        }

        private ImageBrush IntToImg(int i)
        {
            if (i >= 10) i = i / 10;
            StringBuilder sb = new StringBuilder(@"Resourses\");
            sb=sb.Append(i.ToString());
            sb = sb.Append(".png");
            return new ImageBrush(new BitmapImage(new Uri(sb.ToString(), UriKind.Relative)));
        }
    }
}
