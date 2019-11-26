using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace Checkers.UI
{
    public class Program
    {
        public static void Main()
        {
            GameSettings start = new GameSettings();
            start.ShowDialog();
        }
    }
}
