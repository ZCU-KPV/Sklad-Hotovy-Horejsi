using System.Windows.Input;
using Sklad;
using Sklad.Views;
using PropertyChanged;

namespace Sklad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowVM : BaseVM
    {
        // --- Vlastnosti pro Commandy ---
        // Na tyto vlastnosti se budeme bindovat v XAML
        public ICommand OpenMaterialCommand { get; }
        public ICommand OpenPaletyCommand { get; }
        public ICommand OpenPrehledMaterialuCommand { get; }
        public ICommand OpenPrehledPaletCommand { get; }

        public bool TabulkyPovoleny { get; set; } = true;

        public MainWindowVM()
        {
            // V konstruktoru inicializujeme commandy a propojíme je s metodami
            OpenMaterialCommand = new RelayCommand(ExecuteOpenMaterial);
            OpenPaletyCommand = new RelayCommand(ExecuteOpenPalety);
            OpenPrehledMaterialuCommand = new RelayCommand(ExecuteOpenPrehledMaterialu);
            OpenPrehledPaletCommand = new RelayCommand(ExecuteOpenPrehledPalet);
        }

  
        private void ExecuteOpenMaterial(object parameter)
        {
            //var materialWindow = new MaterialView(); 
            //materialWindow.Show();
        }

        private void ExecuteOpenPalety(object parameter)
        {
            var paletyWindow = new PaletyView();
            paletyWindow.Show();
        }

        private void ExecuteOpenPrehledMaterialu(object parameter)
        {
            var prehMatWindow = new PrehledMaterialuView();
            prehMatWindow.Show();
        }

        private void ExecuteOpenPrehledPalet(object parameter)
        {
            //var prehPalWindow = new winPrehledPaletView();
            //prehPalWindow.Show();
        }
    }
}