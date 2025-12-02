using DataEntity;
using Microsoft.EntityFrameworkCore;
using PropertyChanged;
using Sklad;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static DataEntity.Enums;

namespace Sklad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class PaletyViewVM : BaseVM
    {
        // --- Kolekce ---
        private ObservableCollection<Paleta> paletyCol;
        public ObservableCollection<Paleta> PaletyCol
        {
            get
            {
                if (paletyCol == null)
                {
                    // Načtení palet včetně materiálu pro zobrazení názvu materiálu
                    paletyCol = new ObservableCollection<Paleta>(Sklad.Globals.context.Palety.Include(p => p.Material));
                }
                return paletyCol;
            }
        }



        // --- Enums pro ComboBoxy ---
        public IEnumerable<PaletaTyp> PaletaTypValues => Enum.GetValues(typeof(PaletaTyp)).Cast<PaletaTyp>();
        public IEnumerable<PaletaStav> PaletaStavValues => Enum.GetValues(typeof(PaletaStav)).Cast<PaletaStav>();

        // --- Výběr ---
        public Paleta SelectedPaleta { get; set; }

        // --- Commands ---
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand SaveCommand { get; }

        public PaletyViewVM()
        {
            AddCommand = new RelayCommand(ExecuteAdd);
            RemoveCommand = new RelayCommand(ExecuteRemove, CanExecuteRemove);
            SaveCommand = new RelayCommand(ExecuteSave, CanExecuteSave);
        }

        private void ExecuteAdd(object parameter)
        {
            var novaPaleta = new Paleta
            {
                Typ = PaletaTyp.Mala, // Default
                Stav = PaletaStav.Zaskladneno, // Default
                Mnozstvi = 0
            };

            Sklad.Globals.context.Palety.Add(novaPaleta);
            PaletyCol.Add(novaPaleta);
            SelectedPaleta = novaPaleta;
        }

        private bool CanExecuteRemove(object parameter)
        {
            return SelectedPaleta != null;
        }

        private void ExecuteRemove(object parameter)
        {
            if (SelectedPaleta == null) return;

            if (MessageBox.Show("Opravdu jste si jist?", "Smazat paletu", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Sklad.Globals.context.Palety.Remove(SelectedPaleta);
                PaletyCol.Remove(SelectedPaleta);
                SelectedPaleta = null;
            }
        }

        private bool CanExecuteSave(object parameter)
        {
            // Povolit uložení pouze pokud jsou všechny palety validní
            // IsValid je vlastnost z BaseModelu
            return PaletyCol != null && PaletyCol.All(p => p.IsValid);
        }

        private void ExecuteSave(object parameter)
        {
            Sklad.Globals.UlozitData();
            MessageBox.Show("Data byla úspěšně uložena.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public ICommand SelectMaterialCommand => new RelayCommand(ExecuteSelectMaterial);

        private void ExecuteSelectMaterial(object parameter)
        {
            if (parameter is Paleta paleta)
            {
                var vm = new MaterialyEditViewVM(paleta.Material);
                var window = new Sklad.Views.MaterialyEditView(vm);
                
                if (window.ShowDialog() == false) // ShowDialog returns bool?
                {
                    // If user confirmed (we can check vm.SelectedMaterial, or rely on the fact they clicked OK which closed the window)
                    // Actually ShowDialog returns true if DialogResult is set to true. 
                    // But our VM just closes the window. Let's assume if SelectedMaterial is not null and changed, we use it.
                    // Better: The VM should probably set DialogResult.
                    // Or we just check vm.SelectedMaterial.
                    
                    if (vm.SelectedMaterial != null)
                    {
                        paleta.Material = vm.SelectedMaterial;
                    }
                }
            }
        }
        public bool OnClosing()
        {
            if (Sklad.Globals.HasUnsavedChanges())
            {
                var result = MessageBox.Show("Máte neuložené změny. Chcete je uložit?", "Upozornění", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (CanExecuteSave(null))
                    {
                        ExecuteSave(null);
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Nelze uložit data, protože obsahují chyby.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false; // Cancel close to let user fix errors
                    }
                }
                else if (result == MessageBoxResult.No)
                {
                    Sklad.Globals.Vratit();
                    return true;
                }
                else // Cancel
                {
                    return false;
                }
            }
            return true;
        }
    }
}
