using DataEntity;
using Microsoft.EntityFrameworkCore;
using PropertyChanged; 
using Sklad;
using Sklad.ViewModels; 
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows; 
using System.Windows.Data;
using System.Windows.Input;

namespace Sklad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class PrehledMaterialuVM : BaseVM
    {

        // --- Kolekce dat ---

  



        private ObservableCollection<MernaJednotka> merneJednotkyCol;
        public ObservableCollection<MernaJednotka> MerneJednotkyCol
        {
            get
            {
                if (merneJednotkyCol == null)
                    merneJednotkyCol = new ObservableCollection<MernaJednotka>(Globals.context.MerneJednotky);
                return merneJednotkyCol;
            }
        }


        // 3. Filtrovaný POHLED pro DataGrid materiálu
        public ICollectionView MaterialyView { get; }

        // --- Filtry ---

        private string filtrNazev;
        public string FiltrNazev 
        {
            get
            {
                return filtrNazev;
            }
            set 
            { 
                filtrNazev = value;
                MaterialyView.Refresh();
            }
        
        }
        public string filtrKod;

        public string FiltrKod
        {
            get
            {
                return filtrKod;
            }
            set
            {
                filtrKod = value;
                MaterialyView.Refresh();
            }

        }

        // --- STAV UI (režimy jako ve staré stránce pgPrehMat) ---
        public enum Rezimy { Prohlizeni, Pridavani, Editace }

        public Rezimy AktualniRezim { get; set; } = Rezimy.Prohlizeni;

        public string AktualniRezimString =>
            AktualniRezim == Rezimy.Pridavani ? "Přidávání" :
            AktualniRezim == Rezimy.Editace ? "Editace" : "Prohlížení";

        public bool RezimPridavaniNeboEditace => AktualniRezim != Rezimy.Prohlizeni;
        public bool JeProhlizeni => AktualniRezim == Rezimy.Prohlizeni;


        // --- Výběr ---
        // Přejmenováno přesně podle zadání
        public Material MaterialSelected { get; set; }

        // --- ŘÍZENÍ STAVU UI ---
        // Nová vlastnost pro řízení tlačítek
        public bool JeMaterialVybran => (MaterialSelected != null);

        // --- Commandy ---
        public ICommand PridatMaterialCommand { get; }
        public ICommand OpravitMaterialCommand { get; }
        public ICommand VymazatMaterialCommand { get; }

        public ICommand PotvrditCommand { get; }
        public ICommand StornoCommand { get; }

        // Konstruktor
        public PrehledMaterialuVM()
        {
            // Vytvoření filtrovatelného pohledu
            MaterialyView = CollectionViewSource.GetDefaultView(this.MaterialyCol);
            MaterialyView.Filter = FilterMaterialu;

            // Inicializace Commandů (už bez CanExecute)
            PridatMaterialCommand = new RelayCommand(ExecutePridat);
            OpravitMaterialCommand = new RelayCommand(ExecuteOpravit);
            VymazatMaterialCommand = new RelayCommand(ExecuteVymazat);
            PotvrditCommand = new RelayCommand(ExecutePotvrdit, x => (MaterialSelected != null && MaterialSelected.IsValid));
            StornoCommand = new RelayCommand(ExecuteStorno);
        }

        // --- Metody pro filtrování ---
        private bool FilterMaterialu(object item)
        {
            if (item is not Material material) return false;

            // Filtr Kód (MaterialId)
            if (!string.IsNullOrEmpty(FiltrKod))
            {
                if (!int.TryParse(FiltrKod, out int kodId) || material.MaterialId != kodId)
                {
                    return false;
                }
            }

            // Filtr Název
            if (!string.IsNullOrEmpty(FiltrNazev))
            {
                // Bez Include() se spoléháme, že MernaJednotka bude donačtena
                // (což by díky lazy loadingu měla)
                if (!material.Nazev.Contains(FiltrNazev, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }

        // --- Metody pro Commandy ---
        // CanExecute už není potřeba, řídí to IsEnabled v XAML

        // --- LOGIKA ---
        private void ExecutePridat(object parameter)
        {
            var mat = new Material
            {
                // Datum/Komentář mají v modelu defaulty; naznačím datum kvůli UX
                Datum = DateTime.Now
            };

            // Vezmi první měrnou jednotku z kolekce, která má neprázdný popis
            var defaultMj = MerneJednotkyCol?
                .FirstOrDefault(mj => mj != null && !string.IsNullOrWhiteSpace(mj.Popis));


            if (defaultMj != null)
            {
                mat.MernaJednotka = defaultMj;
              //  mat.MernaJednotkaId = defaultMj.MernaJednotkaId;
            }


            Globals.context.Materialy.Add(mat);
            MaterialyCol.Add(mat);
            MaterialSelected = mat;

            AktualniRezim = Rezimy.Pridavani;
        }

        private void ExecuteOpravit(object parameter)
        {
            if (!JeMaterialVybran) return; //není nutné - řídí IsEnabled
            AktualniRezim = Rezimy.Editace;
        }

        private void ExecuteVymazat(object parameter)
        {
            if (!JeMaterialVybran) return; //není nutné - řídí IsEnabled

            // Nelze smazat, pokud materiál má palety
            if (MaterialSelected.Palety != null && MaterialSelected.Palety.Any())
            {
                MessageBox.Show(
                    $"Materiál je obsažen v paletách ({MaterialSelected.Palety.Count}). Nelze smazat!",
                    "CHYBA", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show(
                    $"Opravdu chcete vymazat materiál „{MaterialSelected.Nazev}“?",
                    "UPOZORNĚNÍ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Globals.context.Materialy.Remove(MaterialSelected);
                MaterialyCol.Remove(MaterialSelected);
                MaterialSelected = null;

                // okamžitý persist
                Globals.UlozitData();
            }
        }


        private void ExecutePotvrdit(object parameter)
        {
            // Validace + Save (Globals.UlozitData dělá DataAnnotations validaci a zobrazí chyby)
            Globals.UlozitData();

            // návrat do prohlížení
            AktualniRezim = Rezimy.Prohlizeni;

            // refresh view (kvůli filtrům)
            MaterialyView.Refresh();
        }

        private void ExecuteStorno(object parameter)
        {
            if (AktualniRezim == Rezimy.Pridavani)
            {
                // Nový záznam zrušit (detache řeší Globals.Vratit; zároveň odebereme z kolekce)
                if (MaterialSelected != null)
                {
                    // pokud ještě nemá ID -> nový
                    if (MaterialSelected.MaterialId == 0)
                    {
                        var entry = Globals.context.Entry(MaterialSelected);
                        MaterialyCol.Remove(MaterialSelected);
                        // odpojit z contextu
                   
                        if (entry != null) entry.State = EntityState.Detached;
                    }
                }
                MaterialSelected = null;
            }
            else if (AktualniRezim == Rezimy.Editace)
            {
                // vrátit změny modifikovaných entit (viz Globals.Vratit)
                Globals.Vratit();

                // >>> přidej tyto 2 řádky:
                var sel = MaterialSelected;
                MaterialSelected = null;      // vynutí PropertyChanged
                MaterialSelected = sel;       // rebind textboxů na nová data

            }

            AktualniRezim = Rezimy.Prohlizeni;
            MaterialyView.Refresh();
        }
    }

}
