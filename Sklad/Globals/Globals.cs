using DataEntity;
using Microsoft.EntityFrameworkCore;
using Sklad.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Sklad
{
    public static class Globals
    {

        public static SkladContext context { get; set; }

        public static MainWindow parent { get; set; }


   
        public static void Vratit()
        {

            foreach (var entity in context.ChangeTracker.Entries())
            {
                if (entity.State == EntityState.Modified) entity.Reload();
                if (entity.State == EntityState.Added) entity.State = EntityState.Detached;
            }

        }


        public static bool IsValid(DependencyObject parent)
        {
            if (System.Windows.Controls.Validation.GetHasError(parent))
                return false;

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool HasUnsavedChanges()
        {
            return Globals.context.ChangeTracker.Entries().Any(e => e.State == EntityState.Added
                                                      || e.State == EntityState.Modified
                                                      || e.State == EntityState.Deleted);
        }


        public static void UlozitData()
        {
            // 1. Najdeme všechny změněné nebo přidané entity
            var entitiesToValidate = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            var validationErrors = new List<ValidationResult>();

            // 2. Ručně spustíme validaci pro každou entitu
            foreach (var entity in entitiesToValidate)
            {
                var validationContext = new ValidationContext(entity);
                // Třetí parametr 'true' zajistí validaci všech vlastností
                Validator.TryValidateObject(entity, validationContext, validationErrors, validateAllProperties: true);
            }

            // 3. Pokud se našly chyby, zobrazíme je a neukládáme
            if (validationErrors.Any())
            {
                String text = "";
                foreach (var error in validationErrors)
                {
                    // Náhrada za starou strukturu chyby
                    // error.MemberNames obsahuje názvy vlastností, kterých se chyba týká
                    string propertyName = error.MemberNames.Any() ? string.Join(", ", error.MemberNames) : "Neznámá vlastnost";

                    text += $"- Vlastnost: {propertyName}, Chyba: {error.ErrorMessage}" + Environment.NewLine;
                }

                MessageBox.Show("Entita obsahuje tyto chyby ověření:" + Environment.NewLine + text, "CHYBA VALIDACE", MessageBoxButton.OK, MessageBoxImage.Error);

                // Důležité: Nechceme pokračovat k SaveChanges()
                return;
            }

            // 4. Pokud je vše v pořádku, pokusíme se uložit
            try
            {
                context.SaveChanges();
            }
            // 5. odchytáváme DbUpdateException
            catch (DbUpdateException e)
            {
                // Tato chyba teď znamená problém na úrovni DB (constraint, atd.)
                // Pro detailnější info se často hodí InnerException
                string errorDetails = e.InnerException?.Message ?? e.Message;
                MessageBox.Show("Nastala chyba při ukládání do databáze (např. porušení omezení).\n" + errorDetails, "CHYBA DATABÁZE", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nastala neočekávaná chyba při ukládání.\n" + ex.ToString(), "CHYBA", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }


        }
    }
}

