using System.Windows;
using System.Windows.Controls;

using Sklad.ViewModels;

namespace Sklad.Views
{
    /// <summary>
    /// Interaction logic for PaletyView.xaml
    /// </summary>
    public partial class PaletyView : Window
    {
        public PaletyView()
        {
            InitializeComponent();
            DataContext = new PaletyViewVM();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is PaletyViewVM vm)
            {
                if (!vm.OnClosing())
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }
    }
}
