using Sklad.ViewModels;
using System.Windows;

namespace Sklad.Views
{
    public partial class MaterialyEditView : Window
    {
        public MaterialyEditView(MaterialyEditViewVM vm)
        {
            InitializeComponent();
            DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new System.Action(this.Close);
        }
    }
}
