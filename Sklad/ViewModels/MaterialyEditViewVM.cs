using DataEntity;
using Sklad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using PropertyChanged;

namespace Sklad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MaterialyEditViewVM : BaseVM
    {
        public Material SelectedMaterial { get; set; }

        public ICommand ConfirmCommand { get; }
        public Action CloseAction { get; set; }

        public MaterialyEditViewVM(Material currentMaterial)
        {
            SelectedMaterial = currentMaterial;
            ConfirmCommand = new RelayCommand(ExecuteConfirm);
        }

        private void ExecuteConfirm(object parameter)
        {
            CloseAction?.Invoke();
        }
    }
}
