using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OPTech
{
    /// <summary>
    /// Logique d'interaction pour StatusBarDialog.xaml
    /// </summary>
    public partial class StatusBarDialog : Window
    {
        public StatusBarDialog(Window owner)
        {
            InitializeComponent();

            this.Owner = owner;
        }
    }
}
