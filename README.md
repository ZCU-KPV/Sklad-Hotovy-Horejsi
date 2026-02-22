# Skladový systém — Kompletní průvodce tvorbou aplikace

> **Školní projekt** pro výuku C#, WPF, Entity Framework Core a návrhového vzoru MVVM.
> Aplikace simuluje jednoduchý skladový systém — evidenci materiálů, měrných jednotek a palet.

---

## Obsah

1. [O čem aplikace je](#1-o-čem-aplikace-je)
2. [Požadavky na počítač](#2-požadavky-na-počítač)
3. [Struktura řešení (Solution)](#3-struktura-řešení-solution)
4. [KROK 1 — Vytvoření Solution a projektů ve Visual Studiu](#4-krok-1--vytvoření-solution-a-projektů-ve-visual-studiu)
5. [KROK 2 — Projekt DataEntity: instalace NuGet balíčků](#5-krok-2--projekt-dataentity-instalace-nuget-balíčků)
6. [KROK 3 — Vytvoření adresářové struktury DataEntity](#6-krok-3--vytvoření-adresářové-struktury-dataentity)
7. [KROK 4 — Tabulka Materiál (první entita)](#7-krok-4--tabulka-materiál-první-entita)
8. [KROK 5 — SkladContext (první verze)](#8-krok-5--skladcontext-první-verze)
9. [KROK 6 — První migrace a aktualizace databáze](#9-krok-6--první-migrace-a-aktualizace-databáze)
10. [KROK 7 — Rozšíření tabulky Materiál o další sloupce](#10-krok-7--rozšíření-tabulky-materiál-o-další-sloupce)
11. [KROK 8 — Tabulka MěrnáJednotka a propojení s Materiálem](#11-krok-8--tabulka-měrnájednotka-a-propojení-s-materiálem)
12. [KROK 9 — Přejmenování tabulky + úprava Komentáře na nullable](#12-krok-9--přejmenování-tabulky--úprava-komentáře-na-nullable)
13. [KROK 10 — BaseModel (dědičnost a validace)](#13-krok-10--basemodel-dědičnost-a-validace)
14. [KROK 11 — Tabulka Paleta](#14-krok-11--tabulka-paleta)
15. [KROK 12 — Rozšíření Palety o Komentář](#15-krok-12--rozšíření-palety-o-komentář)
16. [KROK 13 — Enumy (výčtové typy)](#16-krok-13--enumy-výčtové-typy)
17. [KROK 14 — Seed data (počáteční naplnění databáze)](#17-krok-14--seed-data-počáteční-naplnění-databáze)
18. [KROK 15 — Projekt Sklad (WPF aplikace): instalace NuGet balíčků](#18-krok-15--projekt-sklad-wpf-aplikace-instalace-nuget-balíčků)
19. [KROK 16 — Struktura WPF projektu (MVVM)](#19-krok-16--struktura-wpf-projektu-mvvm)
20. [KROK 17 — Helpers a Globals](#20-krok-17--helpers-a-globals)
21. [KROK 18 — BaseVM (základ pro všechny ViewModely)](#21-krok-18--basevm-základ-pro-všechny-viewmodely)
22. [KROK 19 — MainWindow (hlavní okno)](#22-krok-19--mainwindow-hlavní-okno)
23. [KROK 20 — Styly a Konvertory](#23-krok-20--styly-a-konvertory)
24. [KROK 21 — Přehled materiálu (View + ViewModel)](#24-krok-21--přehled-materiálu-view--viewmodel)
25. [KROK 22 — Palety (View + ViewModel)](#25-krok-22--palety-view--viewmodel)
26. [KROK 23 — Výběr materiálu (dialogové okno)](#26-krok-23--výběr-materiálu-dialogové-okno)
27. [Shrnutí příkazů pro migraci](#27-shrnutí-příkazů-pro-migraci)
28. [Přehled relací (vztahů) mezi tabulkami](#28-přehled-relací-vztahů-mezi-tabulkami)
29. [Časté chyby a řešení](#29-časté-chyby-a-řešení)

---

## 1. O čem aplikace je

Aplikace **Skladový systém** slouží k evidenci materiálů ve skladu. Umožňuje:

- **Evidovat materiály** — každý materiál má název, měrnou jednotku, pojistné množství, maximální množství do palety a komentář.
- **Evidovat palety** — každá paleta obsahuje určitý materiál, má typ (malá / velká / dělená), stav (zaskladněno / vyskladněno / v dopravě), adresu uložení ve skladu a množství.
- **Evidovat měrné jednotky** — kusy (ks), kilogramy (kg), metry krychlové (m³) apod.
- **Přehledy** — zobrazení materiálů s filtrováním, přidáváním, editací a mazáním. U každého materiálu je vidět seznam jeho palet a celkové množství na skladě.

Aplikace je **desktopová** (WPF — Windows Presentation Foundation) a data ukládá do **SQL Server LocalDB** (lokální databáze, která je součástí Visual Studia).

---

## 2. Požadavky na počítač

| Software | Verze |
|---|---|
| **Visual Studio** | 2022 nebo novější (Community edice stačí) |
| **.NET SDK** | 9.0 |
| **SQL Server LocalDB** | Součástí instalace Visual Studia (zaškrtněte workload *Data storage and processing*) |

Při instalaci Visual Studia zaškrtněte:
- **.NET desktop development** (pro WPF)
- **Data storage and processing** (pro LocalDB)

---

## 3. Struktura řešení (Solution)

Celé řešení se skládá ze **dvou projektů**:

```
Sklad.sln                          ← Solution soubor
│
├── DataEntity/                    ← Projekt: DATOVÁ VRSTVA (Class Library)
│   ├── DataEntity.csproj
│   ├── SkladContext.cs            ← DbContext — spojení s databází
│   ├── Data/
│   │   ├── Base/
│   │   │   └── BaseModel.cs       ← Abstraktní předek všech entit (validace, RowVersion)
│   │   ├── Enum/
│   │   │   └── Enums.cs           ← Výčtové typy (PaletaTyp, PaletaStav)
│   │   ├── Material.cs            ← Entita: tabulka Materialy
│   │   ├── MernaJednotka.cs       ← Entita: tabulka MerneJednotky
│   │   └── Paleta.cs              ← Entita: tabulka Palety
│   └── Migrations/                ← Automaticky generované migrace
│       ├── 20250929094916_001.cs
│       ├── 20250929101630_002.cs
│       ├── 20251006081422_003.cs
│       ├── 20251006083448_004.cs
│       ├── 20251006093510_005.cs
│       ├── 20251013083420_006.cs
│       └── 20251128073214_007.cs
│
└── Sklad/                         ← Projekt: WPF APLIKACE
    ├── Sklad.csproj
    ├── App.xaml / App.xaml.cs      ← Vstupní bod aplikace, definice stylů
    ├── Converters/
    │   └── EnumDescriptionConverter.cs  ← Konvertor enum → čitelný text
    ├── Globals/
    │   └── Globals.cs              ← Sdílený kontext, ukládání, validace, vracení změn
    ├── Helpers/
    │   └── RelayCommand.cs         ← Implementace ICommand pro MVVM
    ├── Images/                     ← Ikony a obrázky
    ├── Styles/
    │   ├── Styles.xaml             ← Globální styly (Margin apod.)
    │   └── Converters.xaml         ← Registrace konvertorů jako XAML resource
    ├── ViewModels/
    │   ├── BaseVM.cs               ← Základ pro všechny ViewModely
    │   ├── MainWindowVM.cs         ← ViewModel hlavního okna
    │   ├── PrehledMaterialuVM.cs   ← ViewModel přehledu materiálu (CRUD + filtrování)
    │   ├── PaletyViewVM.cs         ← ViewModel palet (CRUD)
    │   └── MaterialyEditViewVM.cs  ← ViewModel dialogu pro výběr materiálu
    └── Views/
        ├── MainWindow.xaml / .cs   ← Hlavní okno (navigace)
        ├── PrehledMaterialuView.xaml / .cs  ← Okno přehledu materiálu
        ├── PaletyView.xaml / .cs   ← Okno palet
        └── MaterialyEditView.xaml / .cs     ← Dialogové okno výběru materiálu
```

### Proč dva projekty?

- **DataEntity** — obsahuje pouze definice tabulek (entit), databázový kontext a migrace. Nemá žádné UI. Dá se znovu použít i v jiné aplikaci (webové, mobilní...).
- **Sklad** — WPF aplikace, která DataEntity používá jako **referenci** (závislost). Obsahuje veškeré UI (okna, tlačítka, tabulky) a logiku zobrazení (ViewModely).

Toto oddělení se nazývá **Separation of Concerns** — oddělení zodpovědností. Datová vrstva neví nic o UI a UI neví nic o databázi (komunikuje přes Entity Framework).

---

## 4. KROK 1 — Vytvoření Solution a projektů ve Visual Studiu

### 4.1 Vytvoření Solution s prvním projektem

1. Otevřete **Visual Studio**.
2. Zvolte **Create a new project**.
3. Vyhledejte šablonu **WPF Application** (C#) a zvolte ji.
4. **Project name:** `Sklad`
5. **Solution name:** `Sklad` (nebo libovolný název)
6. **Framework:** `.NET 9.0`
7. Klikněte **Create**.

### 4.2 Přidání projektu DataEntity

1. V **Solution Explorer** klikněte pravým tlačítkem na **Solution 'Sklad'** (úplně nahoře).
2. Zvolte **Add → New Project...**
3. Vyhledejte šablonu **Class Library** (C#) a zvolte ji.
4. **Project name:** `DataEntity`
5. **Framework:** `.NET 9.0`
6. Klikněte **Create**.
7. Smažte automaticky vytvořený soubor `Class1.cs` — nebude potřeba.

### 4.3 Přidání reference DataEntity do projektu Sklad

1. V **Solution Explorer** klikněte pravým tlačítkem na projekt **Sklad**.
2. Zvolte **Add → Project Reference...**
3. Zaškrtněte **DataEntity** a potvrďte **OK**.

> **Proč?** Projekt `Sklad` potřebuje znát třídy z projektu `DataEntity` (entity, kontext). Bez reference by nešel zkompilovat.

---

## 5. KROK 2 — Projekt DataEntity: instalace NuGet balíčků

Klikněte pravým tlačítkem na projekt **DataEntity** → **Manage NuGet Packages...** a nainstalujte:

| Balíček | Verze | K čemu slouží |
|---|---|---|
| `Microsoft.EntityFrameworkCore` | 9.0.9 | Jádro Entity Frameworku — ORM (mapování objektů na tabulky) |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.9 | Poskytovatel (provider) pro SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.9 | Nástroje pro migrace (příkazy v Package Manager Console) |
| `Microsoft.EntityFrameworkCore.Proxies` | 9.0.9 | Lazy loading proxy — automatické donačítání souvisejících dat |
| `PropertyChanged.Fody` | 4.1.0 | Automatické generování `INotifyPropertyChanged` (stačí přidat atribut) |

> **Tip:** Balíčky se dají nainstalovat i přes **Package Manager Console**:
> ```
> Install-Package Microsoft.EntityFrameworkCore -Version 9.0.9 -ProjectName DataEntity
> ```

---

## 6. KROK 3 — Vytvoření adresářové struktury DataEntity

V projektu **DataEntity** vytvořte následující složky (pravý klik → **Add → New Folder**):

```
DataEntity/
├── Data/
│   ├── Base/      ← sem přijde BaseModel.cs
│   └── Enum/      ← sem přijdou Enums.cs
```

Složka `Migrations/` se **vytvoří automaticky** při prvním spuštění migrace — **nevytvářejte ji ručně**.

> **K čemu složky slouží:**
> - `Data/` — hlavní složka pro všechny entity (třídy reprezentující tabulky v DB).
> - `Data/Base/` — abstraktní předek `BaseModel`, ze kterého budou dědit všechny entity. Obsahuje validaci a společné vlastnosti.
> - `Data/Enum/` — výčtové typy (enumy), které slouží jako předem definované seznamy hodnot (typ palety, stav palety).

---

## 7. KROK 4 — Tabulka Materiál (první entita)

Vytvořte soubor `Material.cs` ve složce `Data/` projektu DataEntity.

**První verze** — zatím jen se dvěma vlastnostmi:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntity
{
    [Table("Materialy")]
    public class Material
    {
        [Key]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Název materiálu je povinné pole")]
        [StringLength(150, ErrorMessage = "Maximální délka je 150")]
        public string Nazev { get; set; }
    }
}
```

### Co znamenají jednotlivé části?

| Prvek | Vysvětlení |
|---|---|
| `[Table("Materialy")]` | Atribut, který říká Entity Frameworku, že tato třída odpovídá tabulce `Materialy` v databázi. |
| `[Key]` | Označuje primární klíč tabulky. EF automaticky vytvoří auto-increment (IDENTITY). |
| `[Required]` | Validační atribut — pole je povinné, nesmí být prázdné. |
| `[StringLength(150)]` | Validační atribut — maximální délka textového pole. V DB se vytvoří `nvarchar(150)`. |
| `public int MaterialId { get; set; }` | Vlastnost (property) = sloupec v tabulce. Typ `int` = celočíselný sloupec. |
| `public string Nazev { get; set; }` | Textový sloupec pro název materiálu. |

---

## 8. KROK 5 — SkladContext (první verze)

Vytvořte soubor `SkladContext.cs` v kořeni projektu DataEntity.

```csharp
using Microsoft.EntityFrameworkCore;

namespace DataEntity
{
    public class SkladContext : DbContext
    {
        public DbSet<Material> Materialy { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Data Source=(localdb)\\MSSQLLocalDB;" +
                    "Initial Catalog=Sklad2025;" +
                    "Integrated Security=True;" +
                    "TrustServerCertificate=True");
            }
        }
    }
}
```

### Co znamenají jednotlivé části?

| Prvek | Vysvětlení |
|---|---|
| `DbContext` | Základní třída Entity Frameworku — představuje „sezení" s databází. Přes ni čteme a zapisujeme data. |
| `DbSet<Material> Materialy` | Reprezentuje tabulku `Materialy` v databázi. Přes tuto vlastnost přistupujeme k záznamům (SELECT, INSERT, UPDATE, DELETE). |
| `OnConfiguring` | Metoda, kde nastavíme **connection string** — připojovací řetězec k databázi. |
| `(localdb)\\MSSQLLocalDB` | LocalDB je lehká verze SQL Serveru instalovaná s Visual Studiem. Žádné další nastavení není potřeba. |
| `Initial Catalog=Sklad2025` | Název databáze, kterou EF vytvoří. |
| `Integrated Security=True` | Přihlášení vaším Windows účtem (bez hesla). |

---

## 9. KROK 6 — První migrace a aktualizace databáze

Nyní máme připravenou první entitu a kontext. Je čas vytvořit databázi.

### 9.1 Otevřete Package Manager Console

V menu Visual Studia: **Tools → NuGet Package Manager → Package Manager Console**

### 9.2 Nastavte Default project

V rozbalovacím menu **Default project** v Package Manager Console vyberte **DataEntity**.

> **DŮLEŽITÉ:** Migrace se vždy spouští nad projektem, kde je `DbContext` a entity — tedy **DataEntity**.

### 9.3 Vytvoření migrace

```
Add-Migration 001 -Project DataEntity
```

> **Co se stane:** Entity Framework porovná váš kód (třídy) s aktuálním stavem databáze (zatím prázdná) a vygeneruje soubor migrace do složky `Migrations/`. Tento soubor obsahuje C# kód pro vytvoření tabulky `Materialy`.

### 9.4 Aplikování migrace na databázi

```
Update-Database -Project DataEntity
```

> **Co se stane:** Entity Framework spustí migraci a vytvoří databázi `Sklad2025` na LocalDB s tabulkou `Materialy` (sloupce `MaterialId`, `Nazev`).

### 9.5 Co vzniklo v migraci 001?

Soubor `20250929094916_001.cs` obsahuje:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.CreateTable(
        name: "Materialy",
        columns: table => new
        {
            MaterialId = table.Column<int>(type: "int", nullable: false)
                .Annotation("SqlServer:Identity", "1, 1"),
            Nazev = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
        },
        constraints: table =>
        {
            table.PrimaryKey("PK_Materialy", x => x.MaterialId);
        });
}
```

- **`Up`** — co se provede při aplikaci migrace (vytvoření tabulky).
- **`Down`** — co se provede při vrácení migrace (smazání tabulky).
- `SqlServer:Identity` — automatické číslování (1, 2, 3...).

---

## 10. KROK 7 — Rozšíření tabulky Materiál o další sloupce

Nyní rozšíříme třídu `Material` o další vlastnosti. Upravte soubor `Material.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntity
{
    [Table("Materialy")]
    public class Material
    {
        [Key]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "Název materiálu je povinné pole")]
        [StringLength(150, ErrorMessage = "Maximální délka je 150")]
        public string Nazev { get; set; }

        [Required(ErrorMessage = "Pojistné množství je povinné pole")]
        [Range(0, int.MaxValue, ErrorMessage = "Pojistné množství musí být kladné")]
        public int MnozPoj { get; set; } = 0;

        [Required(ErrorMessage = "Množství do palety je povinné pole")]
        [Range(0, int.MaxValue, ErrorMessage = "Množství do palety musí být kladné")]
        public int MnozDoPal { get; set; } = 0;

        [Column(TypeName = "date")]
        public DateTime Datum { get; set; } = DateTime.Now;

        public string Komentar { get; set; }
    }
}
```

### Nové vlastnosti:

| Vlastnost | Typ v DB | Vysvětlení |
|---|---|---|
| `MnozPoj` | `int` | Pojistné množství — minimální zásoba, pod kterou by materiál neměl klesnout. |
| `MnozDoPal` | `int` | Maximální množství materiálu, které se vejde do jedné palety. |
| `Datum` | `date` | Datum záznamu. Atribut `[Column(TypeName = "date")]` říká SQL Serveru, aby uložil jen datum (bez času). |
| `Komentar` | `nvarchar(max)` | Volitelný komentář k materiálu. |
| `= 0` / `= DateTime.Now` | — | Výchozí hodnoty v C# — když vytvoříme nový objekt, automaticky se nastaví. |

### Migrace:

```
Add-Migration 002 -Project DataEntity
Update-Database -Project DataEntity
```

> **Co se stane v migraci 002:** EF zjistí, že tabulka `Materialy` už existuje, ale přibyly 4 nové sloupce. Vygeneruje příkazy `AddColumn` pro každý nový sloupec.

---

## 11. KROK 8 — Tabulka MěrnáJednotka a propojení s Materiálem

### 11.1 Vytvoření entity MernaJednotka

Vytvořte soubor `MernaJednotka.cs` ve složce `Data/`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntity
{
    [Table("MerneJednotky")]
    public class MernaJednotka
    {
        [Key]
        public int MernaJednotkaId { get; set; }

        [StringLength(50, ErrorMessage = "Popis maximálně na 50 znaků.")]
        public string Popis { get; set; }
    }
}
```

### 11.2 Propojení: Materiál → MěrnáJednotka (cizí klíč)

Do třídy `Material` přidejte:

```csharp
public int MernaJednotkaId { get; set; }

[ForeignKey(nameof(MernaJednotkaId))]
public virtual MernaJednotka MernaJednotka { get; set; }
```

### 11.3 Přidání DbSet do SkladContext

```csharp
public DbSet<MernaJednotka> MerneJednotky { get; set; }
```

### 11.4 Co je to cizí klíč (Foreign Key)?

- `MernaJednotkaId` — celočíselný sloupec v tabulce `Materialy`, který obsahuje ID měrné jednotky.
- `[ForeignKey(nameof(MernaJednotkaId))]` — atribut, který říká EF: *„Tato navigační vlastnost se váže přes sloupec MernaJednotkaId."*
- `virtual` — klíčové slovo nutné pro **lazy loading** (automatické donačítání souvisejících dat, až když je potřebujete).
- Vztah je **N : 1** — mnoho materiálů může mít stejnou měrnou jednotku, ale jeden materiál má právě jednu měrnou jednotku.

### Migrace:

```
Add-Migration 003 -Project DataEntity
Update-Database -Project DataEntity
```

> **Co se stane v migraci 003:** Vytvoří se nová tabulka `MernaJednotka`, do tabulky `Materialy` se přidá sloupec `MernaJednotkaId` a vytvoří se cizí klíč (FK) + index.

---

## 12. KROK 9 — Přejmenování tabulky + úprava Komentáře na nullable

Při vývoji jsme zjistili, že:
1. Tabulka v DB se jmenuje `MernaJednotka` (jednotné číslo), ale chceme `MerneJednotky` (množné číslo) — opravíme to přidáním atributu `[Table("MerneJednotky")]`.
2. Vlastnost `Komentar` v materiálu by měla být **nepovinná** (nullable) — změníme `string` na `string?`.

### Úpravy v kódu:

V `Material.cs` změníme:
```csharp
public string? Komentar { get; set; }
```

V `MernaJednotka.cs` přidáme/ověříme:
```csharp
[Table("MerneJednotky")]
```

### Migrace:

```
Add-Migration 004 -Project DataEntity
Update-Database -Project DataEntity
```

> **Co se stane v migraci 004:** EF přejmenuje tabulku z `MernaJednotka` na `MerneJednotky` a změní sloupec `Komentar` z `NOT NULL` na `NULL`.

---

## 13. KROK 10 — BaseModel (dědičnost a validace)

Nyní vytvoříme **abstraktní bázovou třídu**, ze které budou všechny entity dědit. Díky tomu nemusíme opakovat společný kód v každé entitě.

### 13.1 Vytvoření BaseModel.cs

Vytvořte soubor `BaseModel.cs` ve složce `Data/Base/`:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataEntity
{
    public abstract class BaseModel : IDataErrorInfo
    {
        #region "validace"

        string IDataErrorInfo.Error
        {
            get
            {
                throw new NotSupportedException(
                    "IDataErrorInfo.Error is not supported, use IDataErrorInfo.this[propertyName] instead.");
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return OnValidate(propertyName);
            }
        }

        protected virtual string OnValidate(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("Invalid property name", propertyName);

            string error = string.Empty;
            var value = this.GetType().GetProperty(propertyName).GetValue(this, null);
            var results = new List<ValidationResult>(1);
            var context = new ValidationContext(this, null, null) { MemberName = propertyName };
            var result = Validator.TryValidateProperty(value, context, results);

            if (!result)
            {
                var validationResult = results.First();
                error = validationResult.ErrorMessage;
            }

            return error;
        }

        protected IReadOnlyList<ValidationResult> ValidateAll()
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);
            Validator.TryValidateObject(this, context, results, validateAllProperties: true);
            return results;
        }

        public int ErrorsCount => ValidateAll().Count;

        public bool IsValid => ErrorsCount == 0;

        #endregion

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public DateTime DatumVytvoreni { get; set; } = DateTime.Now;
    }
}
```

### Co znamenají jednotlivé části BaseModelu?

| Prvek | Vysvětlení |
|---|---|
| `abstract class` | Třídu nelze přímo vytvořit (`new BaseModel()` nepůjde). Slouží jen jako předek pro dědění. |
| `IDataErrorInfo` | Rozhraní, které WPF používá pro validaci v reálném čase. Když uživatel napíše do TextBoxu neplatnou hodnotu, okamžitě se zobrazí červený rámeček s chybou. |
| `OnValidate` | Metoda, která přečte validační atributy (`[Required]`, `[StringLength]`, `[Range]`) na dané vlastnosti a ověří hodnotu. |
| `ValidateAll()` | Ověří **všechny** vlastnosti najednou. |
| `IsValid` | Vrátí `true`, pokud entita nemá žádné validační chyby. |
| `ErrorsCount` | Počet chyb. |
| `[Timestamp] RowVersion` | Slouží pro **optimistickou souběžnost** (concurrency). Pokud dva uživatelé upraví stejný záznam současně, EF detekuje konflikt. |
| `DatumVytvoreni` | Automaticky se nastaví na aktuální datum při vytvoření záznamu. |

### 13.2 Nastavení dědičnosti na všech entitách

Nyní změníme entity tak, aby dědily z `BaseModel`:

**Material.cs** — změníme `public class Material` na:
```csharp
public class Material : BaseModel
```

**MernaJednotka.cs** — změníme `public class MernaJednotka` na:
```csharp
public class MernaJednotka : BaseModel
```

> **Co se stane:** Všechny vlastnosti a metody z `BaseModel` se automaticky přenesou do `Material` i `MernaJednotka`. To znamená, že obě entity budou mít `RowVersion`, `DatumVytvoreni`, `IsValid`, validaci atd.

### Migrace:

```
Add-Migration 005 -Project DataEntity
Update-Database -Project DataEntity
```

> **Co se stane v migraci 005:** Do tabulek `Materialy` a `MerneJednotky` se přidají sloupce `RowVersion` (typ `rowversion`) a `DatumVytvoreni` (typ `datetime2`).

---

## 14. KROK 11 — Tabulka Paleta

### 14.1 Vytvoření entity Paleta

Vytvořte soubor `Paleta.cs` ve složce `Data/`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataEntity
{
    [Table("Palety")]
    public class Paleta : BaseModel
    {
        [Key]
        public int PaletaId { get; set; }

        [Required(ErrorMessage = "Typ palety je povinné pole")]
        public Enums.PaletaTyp Typ { get; set; }

        [Required(ErrorMessage = "Stav palety je povinné pole")]
        public Enums.PaletaStav Stav { get; set; }

        [StringLength(10, ErrorMessage = "Maximální délka adresy je 10 znaků")]
        public string? AdresaUlozeni { get; set; }

        [Required(ErrorMessage = "Množství je povinné pole")]
        [Range(0, int.MaxValue, ErrorMessage = "Množství musí být nezáporné číslo")]
        public int Mnozstvi { get; set; } = 0;

        public int? MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public virtual Material? Material { get; set; }
    }
}
```

### Propojení tabulek:

| Vztah | Vysvětlení |
|---|---|
| **Paleta → Materiál** (N : 1) | Každá paleta obsahuje nanejvýš jeden materiál. Více palet může obsahovat stejný materiál. Cizí klíč je `MaterialId` (nullable — paleta nemusí mít materiál přiřazený). |

### 14.2 Přidání navigační kolekce do Materiálu

Do třídy `Material` přidejte:

```csharp
public virtual ObservableCollection<Paleta> Palety { get; set; }
```

> **Proč `ObservableCollection`?** Protože WPF potřebuje kolekci, která automaticky upozorní UI na změny (přidání/odebrání palety). Běžný `List<>` by tuto notifikaci neposkytl.

### 14.3 Přidání navigační kolekce do MěrnéJednotky

Do třídy `MernaJednotka` přidejte:

```csharp
public virtual ObservableCollection<Material> Materialy { get; set; }
```

### 14.4 Přidání DbSet do SkladContext

```csharp
public DbSet<Paleta> Palety { get; set; }
```

### Migrace:

```
Add-Migration 006 -Project DataEntity
Update-Database -Project DataEntity
```

> **Co se stane v migraci 006:** Vytvoří se nová tabulka `Palety` se všemi sloupci a cizím klíčem na tabulku `Materialy`.

---

## 15. KROK 12 — Rozšíření Palety o Komentář

Přidáme do `Paleta.cs` novou vlastnost:

```csharp
[StringLength(255, ErrorMessage = "Maximální délka komentáře je 255 znaků")]
public string? Komentar { get; set; }
```

### Migrace:

```
Add-Migration 007 -Project DataEntity
Update-Database -Project DataEntity
```

> **Co se stane v migraci 007:** Do tabulky `Palety` se přidá sloupec `Komentar` typu `nvarchar(255)`, nullable.

---

## 16. KROK 13 — Enumy (výčtové typy)

Vytvořte soubor `Enums.cs` ve složce `Data/Enum/`:

```csharp
using System.ComponentModel;

namespace DataEntity
{
    public class Enums
    {
        public enum PaletaTyp
        {
            [Description("Malá")]
            Mala = 0,

            [Description("Velká")]
            Velka = 1,

            [Description("Dělená (pro tyčový materiál)")]
            Delena = 2
        }

        public enum PaletaStav
        {
            [Description("Zaskladněno")]
            Zaskladneno = 0,

            [Description("Vyskladněno")]
            Vyskladneno = 1,

            [Description("V dopravě")]
            Doprava = 2
        }
    }
}
```

### Co je enum a proč ho používáme?

- **Enum** (výčtový typ) je sada předem definovaných hodnot. V databázi se uloží jako `int` (číslo), ale v kódu pracujeme s pojmenovanými hodnotami (`PaletaTyp.Velka`).
- **`[Description("Velká")]`** — atribut, který přidá čitelný český popis. V UI pak místo `Velka` zobrazíme `Velká` (pomocí konvertoru).
- **Proč ne jen text v DB?** Enum zabrání překlepům a zajistí, že hodnota bude vždy jedna z povolených.

### Jak se enum propojuje s Paletou?

V `Paleta.cs` jsou vlastnosti:
```csharp
public Enums.PaletaTyp Typ { get; set; }
public Enums.PaletaStav Stav { get; set; }
```

EF uloží hodnotu enumu jako `int` do databáze. Při čtení zpět automaticky převede číslo na enum.

---

## 17. KROK 14 — Seed data (počáteční naplnění databáze)

Aby aplikace po prvním spuštění nebyla prázdná, přidáme do `SkladContext.cs` metodu `Seed()`, která naplní databázi vzorovými daty:

```csharp
public void Seed()
{
    if (MerneJednotky.Any())
    {
        return; // Databáze již byla naplněna.
    }

    var mjKs = new MernaJednotka { Popis = "ks" };
    var mjKg = new MernaJednotka { Popis = "kg" };
    var mjM3 = new MernaJednotka { Popis = "m3" };

    var materialSroub = new Material
    {
        Nazev = "Šroub",
        MnozPoj = 1000,
        MnozDoPal = 2000,
        Datum = DateTime.Now,
        MernaJednotka = mjKs
    };

    var materialHrebik = new Material
    {
        Nazev = "Hřebík",
        MnozPoj = 1500,
        MnozDoPal = 1000,
        Datum = DateTime.Now,
        MernaJednotka = mjKs
    };

    var palety = new[]
    {
        new Paleta
        {
            Stav = Enums.PaletaStav.Vyskladneno,
            Typ = Enums.PaletaTyp.Velka,
            Material = materialSroub,
            AdresaUlozeni = "M10",
            Mnozstvi = 500
        },
        new Paleta
        {
            Stav = Enums.PaletaStav.Vyskladneno,
            Typ = Enums.PaletaTyp.Velka,
            Material = materialSroub,
            AdresaUlozeni = "M2",
            Mnozstvi = 750
        },
        new Paleta
        {
            Stav = Enums.PaletaStav.Vyskladneno,
            Typ = Enums.PaletaTyp.Velka,
            Material = materialHrebik,
            AdresaUlozeni = "M2/5",
            Mnozstvi = 1000
        }
    };

    MerneJednotky.AddRange(mjKs, mjKg, mjM3);
    Materialy.AddRange(materialSroub, materialHrebik);
    Palety.AddRange(palety);

    SaveChanges();
}
```

### Jak Seed funguje?

1. Nejdříve zkontroluje, zda v DB už existují měrné jednotky — pokud ano, skončí (zabránění duplicitám).
2. Vytvoří 3 měrné jednotky, 2 materiály a 3 palety.
3. Přidá je do kontextu pomocí `AddRange`.
4. Uloží vše najednou pomocí `SaveChanges()`.

> **Kdy se Seed zavolá?** Při startu aplikace v `MainWindow.xaml.cs` (viz krok 19).

---

## 18. KROK 15 — Projekt Sklad (WPF aplikace): instalace NuGet balíčků

Klikněte pravým tlačítkem na projekt **Sklad** → **Manage NuGet Packages...** a nainstalujte:

| Balíček | Verze | K čemu slouží |
|---|---|---|
| `MahApps.Metro` | 2.4.11 | Moderní vzhled WPF oken (Metro styl) |
| `Microsoft.EntityFrameworkCore` | 9.0.9 | Potřebné pro práci s kontextem |
| `Microsoft.EntityFrameworkCore.Proxies` | 9.0.9 | Lazy loading |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.9 | SQL Server provider |
| `Microsoft.EntityFrameworkCore.Tools` | 9.0.9 | Nástroje pro migraci |
| `PropertyChanged.Fody` | 4.1.0 | Automatické `INotifyPropertyChanged` |

> **Nezapomeňte:** V projektu `Sklad` už máte **referenci na DataEntity** (krok 1). NuGet balíčky EF jsou potřeba i zde, protože Sklad přímo pracuje s kontextem.

---

## 19. KROK 16 — Struktura WPF projektu (MVVM)

V projektu **Sklad** vytvořte následující složky:

```
Sklad/
├── Converters/     ← konvertory pro XAML binding
├── Globals/        ← sdílená statická třída (kontext, ukládání)
├── Helpers/        ← pomocné třídy (RelayCommand)
├── Images/         ← ikony a obrázky
│   ├── Icons/      ← ikony tlačítek (Add.png, Delete.png, Edit.png...)
│   └── MainMenu/   ← ikony hlavního menu
├── Styles/         ← XAML soubory se styly a konvertory
├── ViewModels/     ← logika UI (ViewModely)
└── Views/          ← XAML okna a code-behind
```

### Co je MVVM?

**MVVM** (Model-View-ViewModel) je návrhový vzor pro WPF aplikace:

| Vrstva | Co obsahuje | Příklad souboru |
|---|---|---|
| **Model** | Data a business logika. V našem případě entity z projektu DataEntity. | `Material.cs`, `Paleta.cs` |
| **View** | Grafické rozhraní — XAML soubory s okny, tlačítky, tabulkami. | `MainWindow.xaml` |
| **ViewModel** | Propojení mezi View a Model. Obsahuje kolekce dat, příkazy (Commands) a logiku UI. | `MainWindowVM.cs` |

### Proč MVVM?

- **View** nezná logiku — jen zobrazuje data.
- **ViewModel** nezná UI — jen připravuje data a reaguje na příkazy.
- Propojení probíhá přes **Data Binding** (XAML `{Binding ...}`).
- Výhoda: Kód je přehledný, testovatelný a snadno rozšiřitelný.

---

## 20. KROK 17 — Helpers a Globals

### 20.1 RelayCommand (Helpers/RelayCommand.cs)

Tato třída umožňuje ve ViewModelech vytvářet příkazy (Commands), na které se bindují tlačítka v XAML.

```csharp
using System;
using System.Windows.Input;

namespace Sklad
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
```

### Co je RelayCommand?

| Prvek | Vysvětlení |
|---|---|
| `ICommand` | Rozhraní, které WPF vyžaduje pro tlačítka (`Button.Command`). |
| `_execute` | Akce, která se provede po kliknutí. |
| `_canExecute` | Podmínka, zda je tlačítko povolené (šedé / aktivní). Pokud není zadaná, tlačítko je vždy aktivní. |
| `CommandManager.RequerySuggested` | WPF automaticky periodicky přehodnocuje `CanExecute`. |

### 20.2 Globals (Globals/Globals.cs)

Statická třída, která drží sdílený databázový kontext a společné metody:

```csharp
using DataEntity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;

namespace Sklad
{
    public static class Globals
    {
        public static SkladContext context { get; set; }

        public static void Vratit()
        {
            foreach (var entity in context.ChangeTracker.Entries())
            {
                if (entity.State == EntityState.Modified) entity.Reload();
                if (entity.State == EntityState.Added) entity.State = EntityState.Detached;
            }
        }

        public static bool HasUnsavedChanges()
        {
            return context.ChangeTracker.Entries().Any(
                e => e.State == EntityState.Added
                  || e.State == EntityState.Modified
                  || e.State == EntityState.Deleted);
        }

        public static void UlozitData()
        {
            var entitiesToValidate = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);

            var validationErrors = new List<ValidationResult>();

            foreach (var entity in entitiesToValidate)
            {
                var validationContext = new ValidationContext(entity);
                Validator.TryValidateObject(entity, validationContext, validationErrors,
                    validateAllProperties: true);
            }

            if (validationErrors.Any())
            {
                string text = "";
                foreach (var error in validationErrors)
                {
                    string propertyName = error.MemberNames.Any()
                        ? string.Join(", ", error.MemberNames)
                        : "Neznámá vlastnost";
                    text += $"- Vlastnost: {propertyName}, Chyba: {error.ErrorMessage}"
                         + Environment.NewLine;
                }

                MessageBox.Show(
                    "Entita obsahuje tyto chyby ověření:" + Environment.NewLine + text,
                    "CHYBA VALIDACE", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                string errorDetails = e.InnerException?.Message ?? e.Message;
                MessageBox.Show(
                    "Nastala chyba při ukládání do databáze.\n" + errorDetails,
                    "CHYBA DATABÁZE", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }
    }
}
```

### Co dělají jednotlivé metody?

| Metoda | Vysvětlení |
|---|---|
| `Vratit()` | Vrátí všechny neuložené změny. Upravené záznamy znovu načte z DB (`Reload`), nové záznamy odpojí (`Detached`). |
| `HasUnsavedChanges()` | Vrátí `true`, pokud kontext obsahuje jakékoli neuložené změny. Používá se při zavírání oken. |
| `UlozitData()` | Provede **DataAnnotations validaci** všech změněných entit. Pokud najde chyby, zobrazí je v MessageBoxu a neukládá. Pokud je vše v pořádku, zavolá `SaveChanges()`. Zachytává chyby databáze (porušení omezení apod.). |

---

## 21. KROK 18 — BaseVM (základ pro všechny ViewModely)

Vytvořte soubor `BaseVM.cs` ve složce `ViewModels/`:

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
                    materialyCol = new ObservableCollection<Material>(Globals.context.Materialy);
                }
                return materialyCol;
            }
        }
    }
}
```

### Co je BaseVM?

| Prvek | Vysvětlení |
|---|---|
| `INotifyPropertyChanged` | Rozhraní, které WPF potřebuje, aby mohl automaticky aktualizovat UI při změně dat. |
| `OnPropertyChanged` | Metoda, která upozorní UI, že se hodnota vlastnosti změnila. Díky `[CallerMemberName]` se jméno vlastnosti doplní automaticky. |
| `MaterialyCol` | Kolekce materiálů sdílená mezi ViewModely. Načte se jednou z DB a pak je dostupná všude. |

> **Poznámka:** Díky balíčku **PropertyChanged.Fody** stačí na ViewModel přidat atribut `[AddINotifyPropertyChangedInterface]` a Fody automaticky vygeneruje `OnPropertyChanged` volání pro všechny vlastnosti. V `BaseVM` máme manuální implementaci jako základ.

---

## 22. KROK 19 — MainWindow (hlavní okno)

### 22.1 App.xaml — vstupní bod aplikace

Soubor `App.xaml` definuje, které okno se otevře jako první, a registruje globální styly:

```xml
<Application x:Class="Sklad.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:Sklad"
             xmlns:converters="clr-namespace:Sklad.Converters"
             StartupUri="Views/MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" />
                <ResourceDictionary Source="pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml" />
                <ResourceDictionary Source="Styles/Styles.xaml"/>
                <ResourceDictionary Source="Styles/Converters.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

- **`StartupUri`** — cesta k hlavnímu oknu.
- **MahApps.Metro** — 3 ResourceDictionary pro moderní Metro vzhled (ovládací prvky, fonty, světle-modrý motiv).
- **Styles.xaml** — naše vlastní styly.
- **Converters.xaml** — registrace konvertorů (EnumToStringConverter).

### 22.2 MainWindow.xaml.cs — inicializace kontextu

```csharp
public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();

        var s = new SkladContext();
        s.Seed();

        Globals.context = s;
    }
}
```

- Při startu se vytvoří `SkladContext`, zavolá se `Seed()` (naplní DB ukázkovými daty, pokud jsou prázdná) a kontext se uloží do `Globals.context`.
- Okno dědí z `MetroWindow` (MahApps) místo běžného `Window` — díky tomu má moderní vzhled.

### 22.3 MainWindowVM.cs — ViewModel hlavního okna

```csharp
[AddINotifyPropertyChangedInterface]
public class MainWindowVM : BaseVM
{
    public ICommand OpenMaterialCommand { get; }
    public ICommand OpenPaletyCommand { get; }
    public ICommand OpenPrehledMaterialuCommand { get; }
    public ICommand OpenPrehledPaletCommand { get; }

    public bool TabulkyPovoleny { get; set; } = true;

    public MainWindowVM()
    {
        OpenMaterialCommand = new RelayCommand(ExecuteOpenMaterial);
        OpenPaletyCommand = new RelayCommand(ExecuteOpenPalety);
        OpenPrehledMaterialuCommand = new RelayCommand(ExecuteOpenPrehledMaterialu);
        OpenPrehledPaletCommand = new RelayCommand(ExecuteOpenPrehledPalet);
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

    // ... další metody pro otevírání oken
}
```

### Jak to funguje?

1. V **XAML** je tlačítko s `Command="{Binding OpenPaletyCommand}"`.
2. WPF díky `DataContext` najde vlastnost `OpenPaletyCommand` ve ViewModelu.
3. Při kliknutí WPF zavolá `Execute` na příkazu → provede se `ExecuteOpenPalety` → otevře se nové okno `PaletyView`.

### 22.4 MainWindow.xaml — XAML hlavního okna

Hlavní okno obsahuje:
- **Logo** a nadpis „Skladový systém".
- **Dvě skupiny tlačítek:** Tabulky (Materiál, Palety) a Přehledy (Přehled materiálu, Přehled palet).
- **CheckBox** „Povolit tabulky" — přepínač, který zapíná/vypíná skupinu Tabulky.
- **Copyright** dole.

Každé tlačítko má ikonu a text, a je nabindované na Command ve ViewModelu:
```xml
<Button Command="{Binding OpenPaletyCommand}">
    <StackPanel>
        <Image Source="../Images/MainMenu/paleta.png" Width="48" Height="48"/>
        <TextBlock Text="Palety" HorizontalAlignment="Center"/>
    </StackPanel>
</Button>
```

---

## 23. KROK 20 — Styly a Konvertory

### 23.1 Styles.xaml (Styles/Styles.xaml)

Globální styly, které se automaticky aplikují na všechny prvky daného typu:

```xml
<ResourceDictionary>
    <Style TargetType="TextBlock" BasedOn="{StaticResource {x:Type TextBlock}}">
        <Setter Property="Margin" Value="3" />
    </Style>
    <Style TargetType="TextBox" BasedOn="{StaticResource {x:Type TextBox}}">
        <Setter Property="Margin" Value="3" />
    </Style>
    <Style TargetType="ComboBox" BasedOn="{StaticResource {x:Type ComboBox}}">
        <Setter Property="Margin" Value="3" />
    </Style>
</ResourceDictionary>
```

- `BasedOn` — dědí styl z MahApps.Metro (zachovává Metro vzhled) a přidává `Margin`.

### 23.2 Converters.xaml (Styles/Converters.xaml)

Registrace konvertorů jako XAML zdroj:

```xml
<ResourceDictionary xmlns:converters="clr-namespace:Sklad.Converters">
    <converters:EnumDescriptionConverter x:Key="EnumToStringConverter"/>
</ResourceDictionary>
```

### 23.3 EnumDescriptionConverter (Converters/EnumDescriptionConverter.cs)

Konvertor, který převede hodnotu enumu na český popis z atributu `[Description]`:

```csharp
public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return null;
        Enum myEnum = (Enum)value;
        string description = GetEnumDescription(myEnum);
        return description;
    }

    private string GetEnumDescription(Enum enumObj)
    {
        FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
        if (fieldInfo == null) return enumObj.ToString();

        object[] attribArray = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attribArray.Length == 0) return enumObj.ToString();

        DescriptionAttribute attrib = (DescriptionAttribute)attribArray[0];
        return attrib.Description;
    }
}
```

Použití v XAML:
```xml
<TextBlock Text="{Binding Typ, Converter={StaticResource EnumToStringConverter}}"/>
```
→ Místo `Velka` se zobrazí `Velká`.

---

## 24. KROK 21 — Přehled materiálu (View + ViewModel)

### 24.1 PrehledMaterialuVM.cs

Toto je nejkomplexnější ViewModel — obsahuje:

- **Kolekce materiálů** (`MaterialyCol` z BaseVM) a **měrných jednotek** (`MerneJednotkyCol`).
- **Filtrovaný pohled** (`MaterialyView`) — `ICollectionView` umožňuje filtrovat zobrazená data bez změny zdrojové kolekce.
- **Filtry** (`FiltrNazev`, `FiltrKod`) — při každé změně se automaticky obnoví filtr.
- **Režimy UI** — enum `Rezimy` (Prohlížení / Přidávání / Editace) řídí, co je v UI povolené.
- **Příkazy** — Přidat, Opravit, Vymazat, Potvrdit, Storno.

### Režimy a jejich vliv na UI:

| Režim | Formulář vlevo | DataGrid vpravo | Toolbar |
|---|---|---|---|
| **Prohlížení** | Zamknutý | Aktivní (lze vybrat řádek) | Přidat / Smazat / Opravit |
| **Přidávání** | Odemknutý (nový záznam) | — | OK / Storno |
| **Editace** | Odemknutý (existující záznam) | — | OK / Storno |

### Klíčové metody:

- **`ExecutePridat`** — Vytvoří nový `Material`, přidá ho do kontextu a kolekce, přepne do režimu Přidávání.
- **`ExecuteOpravit`** — Přepne do režimu Editace (formulář se odemkne).
- **`ExecuteVymazat`** — Zkontroluje, zda materiál nemá palety (pokud ano, nelze smazat). Po potvrzení odstraní z DB.
- **`ExecutePotvrdit`** — Uloží změny přes `Globals.UlozitData()`, vrátí se do Prohlížení.
- **`ExecuteStorno`** — V režimu Přidávání odpojí nový záznam z kontextu. V režimu Editace vrátí změny přes `Globals.Vratit()`.

### 24.2 PrehledMaterialuView.xaml

Okno je rozděleno na **3 sloupce** s `GridSplitter` (přetažitelný oddělovač):

1. **Levý panel** — Toolbar (ikony Přidat/Smazat/Opravit) + editační formulář (TextBoxy, ComboBox měrné jednotky, tlačítka OK/Storno).
2. **Střední panel** — Filtry (Kód, Název) + DataGrid se seznamem materiálů.
3. **Pravý panel** — DataGrid s paletami vybraného materiálu + zobrazení celkového množství (červené, pokud je pod pojistnou zásobou).

### Vypočítané vlastnosti v Material.cs:

```csharp
public int CelkoveMnozstvi
{
    get { return Palety == null ? 0 : Palety.Sum(x => x.Mnozstvi); }
}

public bool PodPojistnymMnozstvim
{
    get { return CelkoveMnozstvi < MnozPoj; }
}
```

- `CelkoveMnozstvi` — sečte množství ze všech palet daného materiálu.
- `PodPojistnymMnozstvim` — `true`, pokud je celkové množství menší než pojistné. V XAML je na to navázán `DataTrigger`, který změní barvu textu na červenou.

---

## 25. KROK 22 — Palety (View + ViewModel)

### 25.1 PaletyViewVM.cs

ViewModel pro okno palet obsahuje:

- **Kolekci palet** (`PaletyCol`) — načtená přes `Include(p => p.Material)` pro eager loading názvu materiálu.
- **Hodnoty enumů** (`PaletaTypValues`, `PaletaStavValues`) — pro naplnění ComboBoxů.
- **Příkazy** — Přidat, Odebrat, Uložit, Vybrat materiál.
- **Metodu `OnClosing`** — při zavření okna zkontroluje neuložené změny a nabídne uložení.

### 25.2 PaletyView.xaml

Okno obsahuje:
- **Toolbar** — tlačítka Přidat (ikona +), Odebrat (ikona koš), Uložit.
- **DataGrid** — editovatelná tabulka palet s:
  - ID (readonly)
  - Typ palety (ComboBox s enumy)
  - Stav palety (ComboBox s enumy)
  - Uložení (textové pole)
  - Materiál (ComboBox + tlačítko „..." pro dialogový výběr)
  - Množství
  - Komentář

### Zajímavé prvky v XAML:

**ComboBox pro enum s českým popisem:**
```xml
<ComboBox ItemsSource="{Binding DataContext.PaletaTypValues, RelativeSource={RelativeSource AncestorType=Window}}"
          SelectedItem="{Binding Typ, UpdateSourceTrigger=PropertyChanged}">
    <ComboBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Converter={StaticResource EnumToStringConverter}}"/>
        </DataTemplate>
    </ComboBox.ItemTemplate>
</ComboBox>
```

**Tlačítko „..." pro výběr materiálu z dialogu:**
```xml
<Button Content="..." Width="30"
        Command="{Binding DataContext.SelectMaterialCommand, RelativeSource={RelativeSource AncestorType=Window}}"
        CommandParameter="{Binding}"/>
```
- `CommandParameter="{Binding}"` — předá aktuální paletu jako parametr příkazu.

---

## 26. KROK 23 — Výběr materiálu (dialogové okno)

### 26.1 MaterialyEditViewVM.cs

Jednoduchý ViewModel s:
- `SelectedMaterial` — vybraný materiál.
- `ConfirmCommand` — zavře okno.
- `CloseAction` — akce pro zavření okna (nastaví se v code-behind).

### 26.2 MaterialyEditView.xaml

Dialogové okno s DataGridem materiálů a tlačítkem „Vybrat":
```xml
<DataGrid ItemsSource="{Binding MaterialyCol}"
          SelectedItem="{Binding SelectedMaterial}"
          AutoGenerateColumns="False" IsReadOnly="True" SelectionMode="Single">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding MaterialId}"/>
        <DataGridTextColumn Header="Název" Binding="{Binding Nazev}"/>
        <DataGridTextColumn Header="Měrná jednotka" Binding="{Binding MernaJednotka.Popis}"/>
    </DataGrid.Columns>
</DataGrid>
```

---

## 27. Shrnutí příkazů pro migraci

Všechny příkazy se zadávají v **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console).

| # | Kdy | Příkaz | Co se stane |
|---|---|---|---|
| 1 | Po vytvoření `Material` + `SkladContext` | `Add-Migration 001 -Project DataEntity` | Migrace: vytvoření tabulky `Materialy` (MaterialId, Nazev) |
| 1 | | `Update-Database -Project DataEntity` | Aplikuje migraci, vytvoří DB |
| 2 | Po přidání sloupců do `Material` | `Add-Migration 002 -Project DataEntity` | Migrace: přidání Datum, Komentar, MnozDoPal, MnozPoj |
| 2 | | `Update-Database -Project DataEntity` | Aplikuje migraci |
| 3 | Po vytvoření `MernaJednotka` + FK | `Add-Migration 003 -Project DataEntity` | Migrace: tabulka MernaJednotka + FK v Materialy |
| 3 | | `Update-Database -Project DataEntity` | Aplikuje migraci |
| 4 | Po přejmenování tabulky + nullable | `Add-Migration 004 -Project DataEntity` | Migrace: přejmenování na MerneJednotky, Komentar nullable |
| 4 | | `Update-Database -Project DataEntity` | Aplikuje migraci |
| 5 | Po vytvoření BaseModel + dědičnosti | `Add-Migration 005 -Project DataEntity` | Migrace: přidání RowVersion, DatumVytvoreni |
| 5 | | `Update-Database -Project DataEntity` | Aplikuje migraci |
| 6 | Po vytvoření Paleta | `Add-Migration 006 -Project DataEntity` | Migrace: tabulka Palety + FK na Materialy |
| 6 | | `Update-Database -Project DataEntity` | Aplikuje migraci |
| 7 | Po přidání Komentar do Palety | `Add-Migration 007 -Project DataEntity` | Migrace: přidání sloupce Komentar do Palety |
| 7 | | `Update-Database -Project DataEntity` | Aplikuje migraci |

> **Pravidlo:** Po každé změně v entitách → `Add-Migration` → `Update-Database`. Vždy v tomto pořadí.

---

## 28. Přehled relací (vztahů) mezi tabulkami

```
┌──────────────────┐       1 : N       ┌──────────────────┐       N : 1       ┌──────────────────┐
│  MerneJednotky   │◄──────────────────│    Materialy     │──────────────────►│     Palety       │
│──────────────────│                    │──────────────────│                    │──────────────────│
│ MernaJednotkaId  │ PK                │ MaterialId       │ PK                │ PaletaId         │ PK
│ Popis            │                    │ Nazev            │                    │ Typ              │
│ RowVersion       │                    │ MnozPoj          │                    │ Stav             │
│ DatumVytvoreni   │                    │ MnozDoPal        │                    │ AdresaUlozeni    │
│                  │                    │ Datum            │                    │ Mnozstvi         │
│                  │                    │ Komentar         │                    │ MaterialId       │ FK
│                  │                    │ MernaJednotkaId  │ FK                │ Komentar         │
│                  │                    │ RowVersion       │                    │ RowVersion       │
│                  │                    │ DatumVytvoreni   │                    │ DatumVytvoreni   │
└──────────────────┘                    └──────────────────┘                    └──────────────────┘
```

- **MěrnáJednotka → Materiál** (1 : N) — jedna měrná jednotka může patřit mnoha materiálům.
- **Materiál → Paleta** (1 : N) — jeden materiál může být na mnoha paletách.

---

## 29. Časté chyby a řešení

| Chyba | Příčina | Řešení |
|---|---|---|
| *„No DbContext was found"* | V PMC není vybraný správný Default project | Nastavte **Default project** na **DataEntity** |
| *„Build failed"* | Kód se nezkompiluje | Opravte chyby v kódu (Error List), pak znovu spusťte migraci |
| *„The name '...' does not exist"* | Chybí `using` nebo reference | Přidejte správný `using` na začátek souboru |
| *„Unable to connect to SQL Server"* | LocalDB není nainstalovaná | Ve Visual Studio Installer přidejte workload **Data storage and processing** |
| *„Pending model changes"* | Zapomněli jste vytvořit migraci po změně entity | Spusťte `Add-Migration` a `Update-Database` |
| Červený rámeček v TextBoxu | Validační chyba (atributy `[Required]`, `[Range]`...) | Opravte hodnotu podle chybové zprávy |
| Tlačítko je šedé / neaktivní | `CanExecute` vrací `false` | Zkontrolujte podmínku (např. není vybraný záznam) |

---

## Licence

(c) 2025 TUCNAK — Školní projekt pro výuku programování.
