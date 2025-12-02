using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using DataEntity;
using System.Collections.ObjectModel;

namespace Sklad.ViewModels
{
    public abstract class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Material> materialyCol;
        public ObservableCollection<Material> MaterialyCol
        {
            get
            {
                if (materialyCol == null)
                {
                    materialyCol = new ObservableCollection<Material>(Sklad.Globals.context.Materialy);
                }
                return materialyCol;
            }
        }
    }
}
