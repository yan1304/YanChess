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

namespace YanChess.UserInterface.UserControls
{
    /// <summary>
    /// Логика взаимодействия для GameOption.xaml
    /// </summary>
    public partial class GameOption : UserControl
    {
        public GameOption()
        {
            InitializeComponent();
        }

        private void hWhite_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i = 0;
            bool b=Int32.TryParse(((TextBox)e.OriginalSource).Text,out i);
            if(!b)
            {
                ((TextBox)e.OriginalSource).Text="";
            }
            else
            {
                if(i>23)
                {
                    ((TextBox)e.OriginalSource).Text = "23";
                }
                else if (i<0)
                {
                    ((TextBox)e.OriginalSource).Text = "";
                }
            }
        }

        private void mWhite_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i = 0;
            bool b = Int32.TryParse(((TextBox)e.OriginalSource).Text, out i);
            if (!b)
            {
                ((TextBox)e.OriginalSource).Text = "";
            }
            else
            {
                if (i > 60)
                {
                    ((TextBox)e.OriginalSource).Text = "59";
                }
                else if (i < 0)
                {
                    ((TextBox)e.OriginalSource).Text = "";
                }
            }
        }

        public TimeSpan GetWhiteTime()
        {
            if (hWhite.Text == "") hWhite.Text = "0";
            if (mWhite.Text == "") mWhite.Text = "0";
            if (sWhite.Text == "") sWhite.Text = "0";
            if (sWhite.Text == "0" && mWhite.Text == "0" && hWhite.Text == "0") mWhite.Text = "5";
            return new TimeSpan(Convert.ToInt32(hWhite.Text),Convert.ToInt32(mWhite.Text),Convert.ToInt32(sWhite.Text));
        }

        public TimeSpan GetBlackTime()
        {
            if (hBlack.Text == "") hBlack.Text = "0";
            if (mBlack.Text == "") mBlack.Text = "0";
            if (sBlack.Text == "") sBlack.Text = "0";
            if (sBlack.Text == "0" && mBlack.Text == "0" && hBlack.Text == "0") mBlack.Text = "5";
            return new TimeSpan(Convert.ToInt32(hBlack.Text), Convert.ToInt32(mBlack.Text), Convert.ToInt32(sBlack.Text));
        }

        public bool IsComputer()
        {
            return (bool)isPC.IsChecked;
        }

        public bool isWhitePlayer()
        {
            bool b;
            if (isWhite.SelectedIndex==0)
            {
                b = true;
            }
            else b = false;
            return b;
        }
    }
}
