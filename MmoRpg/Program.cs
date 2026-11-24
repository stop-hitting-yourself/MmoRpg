
using System;
using MmoRpg.Attributes;
using MmoRpg.Units;
using MmoRpg.Factories;
using MmoRpg.Field;
using MmoRpg.Bases;
using MmoRpg.Terrains;
using MmoRpg.NeutralObjects;

namespace MmoRpg
{
    internal class Program
    {
        private static GameField? mainField;
        private static GameField? copiedField;
        private static GameField? currentField;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();

                if (currentField != null)
                {
                    ShowField(false);
                }
                else
                {
                    Console.WriteLine("Игровое поле не создано");
                }

                Console.WriteLine();
                Console.WriteLine("1. Управление игровым полем");
                Console.WriteLine("2. Управление юнитами");
                Console.WriteLine("3. Управление базами");
                Console.WriteLine("4. Управление ландшафтом");
                Console.WriteLine("5. Нейтральные объекты");
                Console.WriteLine("6. Следующий ход");
                Console.WriteLine("0. Выход");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 0)
                {
                    return;
                }

                if (choice >= 2 && choice <= 6 && !CheckField())
                {
                    Pause();
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            FieldMenu();
                            break;

                        case 2:
                            UnitMenu();
                            break;

                        case 3:
                            BaseMenu();
                            break;

                        case 4:
                            TerrainMenu();
                            break;

                        case 5:
                            NeutralObjectMenu();
                            break;

                        case 6:
                            NextTurn();
                            Pause();
                            break;

                        default:
                            Console.WriteLine("Неверный пункт меню");
                            Pause();
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Pause();
                }
            }
        }

        private static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    return value;
                }

                Console.WriteLine("Введите целое число");
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для продолжения");
            Console.ReadKey(true);
        }

        private static bool CheckField()
        {
            if (currentField == null)
            {
                Console.WriteLine("Сначала создайте игровое поле");
                return false;
            }

            return true;
        }

        private static string GetCurrentFieldName()
        {
            if (currentField == null)
            {
                return "Отсутствует";
            }

            if (currentField == mainField)
            {
                return "Основное";
            }

            return "Копия";
        }

        private static bool ReadCoordinates(out int x, out int y)
        {
            x = -1;
            y = -1;

            if (currentField == null)
            {
                return false;
            }

            Console.WriteLine($"Допустимые X: 0 - {currentField.Width - 1}");
            Console.WriteLine($"Допустимые Y: 0 - {currentField.Height - 1}");

            x = ReadInt("Введите X: ");
            y = ReadInt("Введите Y: ");

            if (x < 0 || x >= currentField.Width || y < 0 || y >= currentField.Height)
            {
                Console.WriteLine("Координаты выходят за границы поля");
                return false;
            }

            return true;
        }

        private static void ShowResult(bool result, string successMessage, string failureMessage)
        {
            if (result)
            {
                Console.WriteLine(successMessage);
            }
            else
            {
                Console.WriteLine(failureMessage);
            }
        }

        private static Unit? SelectUnit()
        {
            if (currentField == null)
            {
                return null;
            }

            if (currentField.Empty)
            {
                Console.WriteLine("На поле нет юнитов");
                return null;
            }

            ShowUnitPositions();

            if (!ReadCoordinates(out int x, out int y))
            {
                return null;
            }

            Unit? unit = currentField.GetUnit(x, y);

            if (unit == null)
            {
                Console.WriteLine("В указанной клетке нет юнита");
            }

            return unit;
        }

        private static Base? SelectBase()
        {
            if (currentField == null)
            {
                return null;
            }

            if (!ShowBasePositions())
            {
                return null;
            }

            if (!ReadCoordinates(out int x, out int y))
            {
                return null;
            }

            Base? gameBase = currentField.GetBase(x, y);

            if (gameBase == null)
            {
                Console.WriteLine("В указанной клетке нет базы");
            }

            return gameBase;
        }

        private static UnitFactory? SelectFactory()
        {
            Console.WriteLine("1. Мечник");
            Console.WriteLine("2. Копейщик");
            Console.WriteLine("3. Лёгкий лучник");
            Console.WriteLine("4. Тяжёлый лучник");
            Console.WriteLine("5. Катафрактарий");
            Console.WriteLine("6. Конный лучник");
            Console.WriteLine("0. Отмена");

            int choice = ReadInt("Выберите тип юнита: ");

            switch (choice)
            {
                case 1:
                    return new InfantryFactory("Swordsman");

                case 2:
                    return new InfantryFactory("Spearman");

                case 3:
                    return new ArcherFactory("LightArcher");

                case 4:
                    return new ArcherFactory("HeavyArcher");

                case 5:
                    return new CavalryFactory("Cataphract");

                case 6:
                    return new CavalryFactory("HorseArcher");

                case 0:
                    return null;

                default:
                    Console.WriteLine("Неверный тип юнита");
                    return null;
            }
        }

        private static string GetUnitName(Unit unit)
        {
            switch (unit.GetUnitType())
            {
                case "Swordsman":
                    return "Мечник";

                case "Spearman":
                    return "Копейщик";

                case "LightArcher":
                    return "Лёгкий лучник";

                case "HeavyArcher":
                    return "Тяжёлый лучник";

                case "Cataphract":
                    return "Катафрактарий";

                case "HorseArcher":
                    return "Конный лучник";

                default:
                    return unit.GetUnitType();
            }
        }

        private static string GetNeutralObjectName(NeutralObject neutralObject)
        {
            switch (neutralObject.GetObjectType())
            {
                case "TreasureChest":
                    return "Сундук с сокровищами";

                case "Trap":
                    return "Ловушка";

                case "HealingPotion":
                    return "Лечебное зелье";

                case "PowerUpAltar":
                    return "Усиливающий алтарь";

                default:
                    return neutralObject.GetObjectType();
            }
        }

        private static string GetTerrainName(ITerrain terrain)
        {
            switch (terrain.GetTerrainType())
            {
                case "Plain":
                    return "Равнина";

                case "Forest":
                    return "Лес";

                case "Swamp":
                    return "Болото";

                default:
                    return terrain.GetTerrainType();
            }
        }

        private static char GetUnitSymbol(Unit unit)
        {
            switch (unit.GetUnitType())
            {
                case "Swordsman":
                    return 'S';

                case "Spearman":
                    return 'P';

                case "LightArcher":
                    return 'L';

                case "HeavyArcher":
                    return 'H';

                case "Cataphract":
                    return 'C';

                case "HorseArcher":
                    return 'R';

                default:
                    return '?';
            }
        }

        private static char GetNeutralObjectSymbol(NeutralObject neutralObject)
        {
            switch (neutralObject.GetObjectType())
            {
                case "TreasureChest":
                    return 'T';

                case "Trap":
                    return 'X';

                case "HealingPotion":
                    return '+';

                case "PowerUpAltar":
                    return 'A';

                default:
                    return '?';
            }
        }

        private static char GetTerrainSymbol(ITerrain terrain)
        {
            switch (terrain.GetTerrainType())
            {
                case "Plain":
                    return '.';

                case "Forest":
                    return 'F';

                case "Swamp":
                    return '~';

                default:
                    return '?';
            }
        }

        private static void FieldMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("Управление игровым полем");
                Console.WriteLine("1. Создать игровое поле");
                Console.WriteLine("2. Показать игровое поле");
                Console.WriteLine("3. Показать список юнитов");
                Console.WriteLine("4. Скопировать игровое поле");
                Console.WriteLine("5. Переключить игровое поле");
                Console.WriteLine("6. Очистить игровое поле");
                Console.WriteLine("0. Назад");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 0)
                {
                    return;
                }

                if (choice != 1 && !CheckField())
                {
                    Pause();
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            CreateField();
                            break;

                        case 2:
                            ShowField();
                            break;

                        case 3:
                            ShowUnits();
                            break;

                        case 4:
                            CopyField();
                            break;

                        case 5:
                            SwitchField();
                            break;

                        case 6:
                            ClearField();
                            break;

                        default:
                            Console.WriteLine("Неверный пункт меню");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Pause();
            }
        }

        private static void CreateField()
        {
            int width = ReadInt("Введите ширину поля: ");
            int height = ReadInt("Введите высоту поля: ");
            int maxUnits = ReadInt("Введите максимальное число юнитов: ");

            GameField field = new GameField(width, height, maxUnits);

            mainField = field;
            copiedField = null;
            currentField = mainField;

            Console.WriteLine("Игровое поле создано");
        }

        private static void ShowField(bool showLegend = true)
        {
            if (currentField == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine($"Поле: {GetCurrentFieldName()}");
            Console.WriteLine($"Ход: {currentField.TurnNumber}");
            Console.WriteLine();

            for (int y = 0; y < currentField.Height; y++)
            {
                for (int x = 0; x < currentField.Width; x++)
                {
                    Unit? unit = currentField.GetUnit(x, y);
                    Base? gameBase = currentField.GetBase(x, y);
                    NeutralObject? neutralObject = currentField.GetNeutralObject(x, y);
                    ITerrain? terrain = currentField.GetTerrain(x, y);

                    char symbol = '.';

                    if (unit != null)
                    {
                        symbol = GetUnitSymbol(unit);
                    }
                    else if (gameBase != null)
                    {
                        symbol = 'B';
                    }
                    else if (neutralObject != null)
                    {
                        symbol = GetNeutralObjectSymbol(neutralObject);
                    }
                    else if (terrain != null)
                    {
                        symbol = GetTerrainSymbol(terrain);
                    }

                    Console.Write($"{symbol} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine($"Количество юнитов: {currentField.Size}");

            if (showLegend)
            {
                Console.WriteLine();
                Console.WriteLine("S - мечник, P - копейщик");
                Console.WriteLine("L - лёгкий лучник, H - тяжёлый лучник");
                Console.WriteLine("C - катафрактарий, R - конный лучник");
                Console.WriteLine("B - база");
                Console.WriteLine("T - сундук, X - ловушка");
                Console.WriteLine("+ - зелье, A - алтарь");
                Console.WriteLine(". - равнина, F - лес, ~ - болото");
            }
        }

        private static void ShowUnitInfo(Unit unit)
        {
            Console.WriteLine($"Тип: {GetUnitName(unit)}");
            Console.WriteLine($"Координаты: ({unit.X}, {unit.Y})");
            Console.WriteLine($"Здоровье: {unit.CurrentHealth}/{unit.MaxHealth}");
            Console.WriteLine($"Броня: {unit.UnitArmor.Value}");
            Console.WriteLine($"Эффективная броня: {unit.EffectiveArmor}");
            Console.WriteLine($"Атака: {unit.UnitAttack.Value}");

            string damageType = unit.UnitAttack.Type == DamageType.Physical ? "Физический" : "Магический";

            Console.WriteLine($"Тип атаки: {damageType}");
            Console.WriteLine($"Магическое сопротивление: {unit.UnitMagicResistance.Value}%");
            Console.WriteLine($"Очки перемещения: {unit.MovementPoints:F2}/{unit.MaxMovementPoints}");
            Console.WriteLine($"Множитель скорости: {unit.SpeedMultiplier:F2}");

            if (unit.OwnerBase != null)
            {
                Console.WriteLine($"База: ({unit.OwnerBase.X}, {unit.OwnerBase.Y})");
            }
            else
            {
                Console.WriteLine("База: отсутствует");
            }

            if (unit is Spearman spearman)
            {
                Console.WriteLine($"Щит поднят: {(spearman.ShieldRaised ? "Да" : "Нет")}");
            }

            Console.WriteLine("------------------------------");
        }

        private static void ShowUnits()
        {
            if (currentField == null)
            {
                return;
            }

            if (currentField.Empty)
            {
                Console.WriteLine("На поле нет юнитов");
                return;
            }

            Console.WriteLine("Список юнитов");

            foreach (Unit unit in currentField)
            {
                ShowUnitInfo(unit);
            }

            Console.WriteLine($"Всего юнитов: {currentField.Size}");
        }

        private static void ShowUnitPositions()
        {
            if (currentField == null)
            {
                return;
            }

            Console.WriteLine("Юниты на поле:");

            foreach (Unit unit in currentField)
            {
                Console.WriteLine($"{GetUnitName(unit)} ({unit.X}, {unit.Y}), HP: {unit.CurrentHealth}/{unit.MaxHealth}");
            }

            Console.WriteLine();
        }

        private static void CopyField()
        {
            if (currentField == null)
            {
                return;
            }

            if (currentField != mainField)
            {
                Console.WriteLine("Для копирования переключитесь на основное поле");
                return;
            }

            copiedField = new GameField(currentField);

            Console.WriteLine("Копия игрового поля создана");
        }

        private static void SwitchField()
        {
            if (copiedField == null)
            {
                Console.WriteLine("Копия игрового поля отсутствует");
                return;
            }

            if (currentField == mainField)
            {
                currentField = copiedField;
            }
            else
            {
                currentField = mainField;
            }

            Console.WriteLine($"Активное поле: {GetCurrentFieldName()}");
        }

        private static void ClearField()
        {
            if (currentField == null)
            {
                return;
            }

            currentField.Clear();

            Console.WriteLine("Игровое поле очищено");
            Console.WriteLine("Ландшафт сохранён");
        }

        private static void UnitMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("=== Управление юнитами ===");
                Console.WriteLine("1. Создать независимого юнита");
                Console.WriteLine("2. Удалить юнита");
                Console.WriteLine("3. Переместить юнита");
                Console.WriteLine("4. Атаковать другого юнита");
                Console.WriteLine("5. Использовать способность");
                Console.WriteLine("6. Изменить характеристики");
                Console.WriteLine("7. Показать информацию о юните");
                Console.WriteLine("8. Показать всех юнитов");
                Console.WriteLine("0. Назад");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 0)
                {
                    return;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            CreateUnit();
                            break;

                        case 2:
                            RemoveUnit();
                            break;

                        case 3:
                            MoveUnit();
                            break;

                        case 4:
                            AttackUnit();
                            break;

                        case 5:
                            UseAbility();
                            break;

                        case 6:
                            ChangeUnitAttributes();
                            break;

                        case 7:
                            ShowSelectedUnit();
                            break;

                        case 8:
                            ShowUnits();
                            break;

                        default:
                            Console.WriteLine("Неверный пункт меню");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Pause();
            }
        }

        private static void CreateUnit()
        {
            if (currentField == null)
            {
                return;
            }

            UnitFactory? factory = SelectFactory();

            if (factory == null)
            {
                return;
            }

            ShowField();

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            ShowResult(currentField.CreateUnit(factory, x, y), "Юнит создан", "Не удалось создать юнита");
        }

        private static void RemoveUnit()
        {
            if (currentField == null)
            {
                return;
            }

            Unit? unit = SelectUnit();

            if (unit == null)
            {
                return;
            }

            ShowResult(currentField.RemoveUnit(unit), "Юнит удалён", "Не удалось удалить юнита");
        }

        private static void MoveUnit()
        {
            if (currentField == null)
            {
                return;
            }

            Unit? unit = SelectUnit();

            if (unit == null)
            {
                return;
            }

            Console.WriteLine($"Осталось очков перемещения: {unit.MovementPoints:F2}");

            ShowField();

            Console.WriteLine("Выберите соседнюю клетку");

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            bool result = currentField.MoveUnit(unit, x, y);

            ShowResult(result, "Юнит перемещён", "Перемещение невозможно");

            Console.WriteLine($"Положение: ({unit.X}, {unit.Y})");
            Console.WriteLine($"Осталось очков перемещения: {unit.MovementPoints:F2}");
        }

        private static void AttackUnit()
        {
            if (currentField == null)
            {
                return;
            }

            if (currentField.Size < 2)
            {
                Console.WriteLine("На поле должно быть не менее двух юнитов");
                return;
            }

            Console.WriteLine("Выберите атакующего юнита");

            Unit? attacker = SelectUnit();

            if (attacker == null)
            {
                return;
            }

            Console.WriteLine("Выберите цель");

            Unit? target = SelectUnit();

            if (target == null)
            {
                return;
            }

            if (attacker == target)
            {
                Console.WriteLine("Юнит не может атаковать самого себя");
                return;
            }

            int previousHealth = target.CurrentHealth;
            int x = target.X;
            int y = target.Y;

            attacker.AttackUnit(target);

            Console.WriteLine($"Нанесено урона: {previousHealth - target.CurrentHealth}");

            if (target.IsAlive)
            {
                Console.WriteLine($"Здоровье цели: {target.CurrentHealth}/{target.MaxHealth}");
            }
            else
            {
                Console.WriteLine("Юнит уничтожен");

                Unit? replacement = currentField.GetUnit(x, y);

                if (replacement != null && replacement != target)
                {
                    Console.WriteLine($"На его месте появился {GetUnitName(replacement)}");
                }
            }
        }

        private static void UseAbility()
        {
            Unit? unit = SelectUnit();

            if (unit == null)
            {
                return;
            }

            if (unit is Spearman spearman)
            {
                if (spearman.ShieldRaised)
                {
                    spearman.LowerShield();
                    Console.WriteLine("Щит опущен");
                }
                else
                {
                    spearman.RaiseShield();
                    Console.WriteLine("Щит поднят");
                }

                Console.WriteLine($"Броня: {unit.UnitArmor.Value}");
            }
            else if (unit is LightArcher lightArcher)
            {
                lightArcher.ChangeDamageType();

                Console.WriteLine("Тип атаки изменён");
                Console.WriteLine($"Тип атаки: {unit.UnitAttack.Type}");
                Console.WriteLine($"Атака: {unit.UnitAttack.Value}");
            }
            else if (unit is HeavyArcher heavyArcher)
            {
                Console.WriteLine("1. Изменить тип атаки");
                Console.WriteLine("2. Восстановить здоровье");
                Console.WriteLine("0. Отмена");

                int choice = ReadInt("Выберите способность: ");

                switch (choice)
                {
                    case 1:
                        heavyArcher.ChangeDamageType();
                        Console.WriteLine($"Тип атаки: {unit.UnitAttack.Type}");
                        break;

                    case 2:
                        int previousHealth = unit.CurrentHealth;
                        heavyArcher.HealSelf();
                        Console.WriteLine($"Восстановлено здоровья: {unit.CurrentHealth - previousHealth}");
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Неверный пункт меню");
                        break;
                }
            }
            else if (unit is Cavalry)
            {
                Console.WriteLine("Способность кавалерии срабатывает автоматически");
            }
            else
            {
                Console.WriteLine("У юнита нет активных способностей");
            }
        }

        private static void ChangeUnitAttributes()
        {
            Unit? unit = SelectUnit();

            if (unit == null)
            {
                return;
            }

            ShowUnitInfo(unit);

            Console.WriteLine("1. Восстановить здоровье");
            Console.WriteLine("2. Уменьшить здоровье");
            Console.WriteLine("3. Увеличить броню");
            Console.WriteLine("4. Уменьшить броню");
            Console.WriteLine("5. Увеличить атаку");
            Console.WriteLine("6. Уменьшить атаку");
            Console.WriteLine("7. Увеличить магическое сопротивление");
            Console.WriteLine("8. Уменьшить магическое сопротивление");
            Console.WriteLine("0. Отмена");

            int choice = ReadInt("Выберите действие: ");

            if (choice == 0)
            {
                return;
            }

            if (choice < 1 || choice > 8)
            {
                Console.WriteLine("Неверный пункт меню");
                return;
            }

            int amount = ReadInt("Введите величину изменения: ");

            if (amount < 0)
            {
                Console.WriteLine("Величина не может быть отрицательной");
                return;
            }

            switch (choice)
            {
                case 1:
                    unit.TakeHeal(amount);
                    break;

                case 2:
                    unit.TakeDamage(amount);
                    break;

                case 3:
                    unit.IncreaseArmor(amount);
                    break;

                case 4:
                    unit.DecreaseArmor(amount);
                    break;

                case 5:
                    unit.IncreaseAttack(amount);
                    break;

                case 6:
                    unit.DecreaseAttack(amount);
                    break;

                case 7:
                    unit.IncreaseMagicResistance(amount);
                    break;

                case 8:
                    unit.DecreaseMagicResistance(amount);
                    break;
            }

            if (unit.IsAlive)
            {
                Console.WriteLine("Текущие характеристики:");
                ShowUnitInfo(unit);
            }
            else
            {
                Console.WriteLine("Юнит уничтожен");
            }
        }

        private static void ShowSelectedUnit()
        {
            Unit? unit = SelectUnit();

            if (unit != null)
            {
                ShowUnitInfo(unit);
            }
        }

        private static void BaseMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("=== Управление базами ===");
                Console.WriteLine("1. Создать базу");
                Console.WriteLine("2. Удалить базу");
                Console.WriteLine("3. Создать юнита на базе");
                Console.WriteLine("4. Добавить ресурсы");
                Console.WriteLine("5. Показать информацию о базе");
                Console.WriteLine("6. Показать все базы");
                Console.WriteLine("7. Уменьшить здоровье базы");
                Console.WriteLine("8. Восстановить здоровье базы");
                Console.WriteLine("0. Назад");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 0)
                {
                    return;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            CreateBase();
                            break;

                        case 2:
                            RemoveBase();
                            break;

                        case 3:
                            CreateUnitOnBase();
                            break;

                        case 4:
                            AddBaseResources();
                            break;

                        case 5:
                            ShowSelectedBase();
                            break;

                        case 6:
                            ShowBases();
                            break;

                        case 7:
                            DamageBase();
                            break;

                        case 8:
                            HealBase();
                            break;

                        default:
                            Console.WriteLine("Неверный пункт меню");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Pause();
            }
        }

        private static void CreateBase()
        {
            if (currentField == null)
            {
                return;
            }

            int health = ReadInt("Введите здоровье базы: ");
            int armor = ReadInt("Введите броню базы: ");
            int resources = ReadInt("Введите количество ресурсов: ");
            int maxUnits = ReadInt("Введите лимит юнитов базы: ");

            Base gameBase = new Base(health, armor, resources, maxUnits);

            ShowField();

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            ShowResult(currentField.PlaceBase(gameBase, x, y), "База создана", "Не удалось разместить базу");
        }

        private static void RemoveBase()
        {
            if (currentField == null)
            {
                return;
            }

            Base? gameBase = SelectBase();

            if (gameBase == null)
            {
                return;
            }

            ShowResult(currentField.RemoveBase(gameBase), "База удалена. Её юниты стали независимыми", "Не удалось удалить базу");
        }

        private static void CreateUnitOnBase()
        {
            if (currentField == null)
            {
                return;
            }

            Base? gameBase = SelectBase();

            if (gameBase == null)
            {
                return;
            }

            if (gameBase.UnitCount >= gameBase.MaxUnits)
            {
                Console.WriteLine("Достигнут лимит юнитов базы");
                return;
            }

            Console.WriteLine($"Ресурсы базы: {gameBase.Resources}");
            Console.WriteLine($"Юниты: {gameBase.UnitCount}/{gameBase.MaxUnits}");

            UnitFactory? factory = SelectFactory();

            if (factory == null)
            {
                return;
            }

            int cost = 50;

            Console.WriteLine($"Стоимость создания: {cost}");

            if (gameBase.Resources < cost)
            {
                Console.WriteLine("Недостаточно ресурсов");
                return;
            }

            ShowField();

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            ShowResult(gameBase.CreateUnit(factory, x, y, cost), "Юнит успешно создан на базе", "Не удалось создать юнита");

            Console.WriteLine($"Осталось ресурсов: {gameBase.Resources}");
            Console.WriteLine($"Юниты: {gameBase.UnitCount}/{gameBase.MaxUnits}");
        }

        private static void AddBaseResources()
        {
            Base? gameBase = SelectBase();

            if (gameBase == null)
            {
                return;
            }

            int amount = ReadInt("Введите количество ресурсов: ");

            gameBase.AddResources(amount);

            Console.WriteLine($"Ресурсы базы: {gameBase.Resources}");
        }

        private static void ShowBaseInfo(Base gameBase)
        {
            Console.WriteLine($"Координаты: ({gameBase.X}, {gameBase.Y})");
            Console.WriteLine($"Здоровье: {gameBase.CurrentHealth}/{gameBase.MaxHealth}");
            Console.WriteLine($"Броня: {gameBase.ArmorValue}");
            Console.WriteLine($"Ресурсы: {gameBase.Resources}");
            Console.WriteLine($"Юниты: {gameBase.UnitCount}/{gameBase.MaxUnits}");
            Console.WriteLine("Принадлежащие базе юниты:");

            if (gameBase.UnitCount == 0)
            {
                Console.WriteLine("Отсутствуют");
            }
            else
            {
                foreach (Unit unit in gameBase.GetUnits())
                {
                    Console.WriteLine($"{GetUnitName(unit)} ({unit.X}, {unit.Y}), HP: {unit.CurrentHealth}/{unit.MaxHealth}");
                }
            }

            Console.WriteLine("------------------------------");
        }

        private static bool ShowBasePositions()
        {
            if (currentField == null)
            {
                return false;
            }

            bool found = false;

            for (int y = 0; y < currentField.Height; y++)
            {
                for (int x = 0; x < currentField.Width; x++)
                {
                    Base? gameBase = currentField.GetBase(x, y);

                    if (gameBase != null)
                    {
                        Console.WriteLine($"База ({x}, {y}), HP: {gameBase.CurrentHealth}/{gameBase.MaxHealth}, ресурсы: {gameBase.Resources}, юниты: {gameBase.UnitCount}/{gameBase.MaxUnits}");

                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("На поле нет баз");
            }

            return found;
        }

        private static void ShowSelectedBase()
        {
            Base? gameBase = SelectBase();

            if (gameBase != null)
            {
                ShowBaseInfo(gameBase);
            }
        }

        private static void ShowBases()
        {
            if (currentField == null)
            {
                return;
            }

            bool found = false;

            for (int y = 0; y < currentField.Height; y++)
            {
                for (int x = 0; x < currentField.Width; x++)
                {
                    Base? gameBase = currentField.GetBase(x, y);

                    if (gameBase != null)
                    {
                        ShowBaseInfo(gameBase);
                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("На поле нет баз");
            }
        }

        private static void DamageBase()
        {
            Base? gameBase = SelectBase();

            if (gameBase == null)
            {
                return;
            }

            int damage = ReadInt("Введите величину урона: ");
            int previousHealth = gameBase.CurrentHealth;

            gameBase.TakeDamage(damage);

            Console.WriteLine($"Получено урона: {previousHealth - gameBase.CurrentHealth}");

            if (gameBase.IsAlive)
            {
                Console.WriteLine($"Здоровье: {gameBase.CurrentHealth}/{gameBase.MaxHealth}");
            }
            else
            {
                Console.WriteLine("База уничтожена");
                Console.WriteLine("Юниты базы стали независимыми");
            }
        }

        private static void HealBase()
        {
            Base? gameBase = SelectBase();

            if (gameBase == null)
            {
                return;
            }

            int amount = ReadInt("Введите количество здоровья: ");

            gameBase.TakeHeal(amount);

            Console.WriteLine($"Здоровье: {gameBase.CurrentHealth}/{gameBase.MaxHealth}");
        }

        private static void TerrainMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("Управление ландшафтом");
                Console.WriteLine("1. Установить ландшафт");
                Console.WriteLine("2. Показать карту ландшафта");
                Console.WriteLine("3. Показать информацию о клетке");
                Console.WriteLine("0. Назад");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 0)
                {
                    return;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            SetTerrain();
                            break;

                        case 2:
                            ShowTerrainMap();
                            break;

                        case 3:
                            ShowCellInfo();
                            break;

                        default:
                            Console.WriteLine("Неверный пункт меню");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Pause();
            }
        }

        private static void SetTerrain()
        {
            if (currentField == null)
            {
                return;
            }

            ShowField();

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            Console.WriteLine("1. Равнина");
            Console.WriteLine("2. Лес");
            Console.WriteLine("3. Болото");
            Console.WriteLine("0. Отмена");

            int choice = ReadInt("Выберите тип ландшафта: ");

            ITerrain terrain;

            switch (choice)
            {
                case 1:
                    terrain = new Plain();
                    break;

                case 2:
                    terrain = new Forest();
                    break;

                case 3:
                    terrain = new Swamp();
                    break;

                case 0:
                    return;

                default:
                    Console.WriteLine("Неверный тип ландшафта");
                    return;
            }

            ShowResult(currentField.SetTerrain(x, y, terrain), "Ландшафт изменён", "Не удалось изменить ландшафт");
        }

        private static void ShowTerrainMap()
        {
            if (currentField == null)
            {
                return;
            }

            Console.WriteLine("Карта ландшафта");
            Console.WriteLine();

            for (int y = 0; y < currentField.Height; y++)
            {
                for (int x = 0; x < currentField.Width; x++)
                {
                    ITerrain? terrain = currentField.TerrainMap.GetTerrainAt(x, y);

                    if (terrain != null)
                    {
                        Console.Write($"{GetTerrainSymbol(terrain)} ");
                    }
                    else
                    {
                        Console.Write("? ");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(". - равнина");
            Console.WriteLine("F - лес");
            Console.WriteLine("~ - болото");
        }

        private static void ShowCellInfo()
        {
            if (currentField == null)
            {
                return;
            }

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            ITerrain? terrain = currentField.TerrainMap.GetTerrainAt(x, y);
            Unit? unit = currentField.GetUnit(x, y);
            Base? gameBase = currentField.GetBase(x, y);
            NeutralObject? neutralObject = currentField.TerrainMap.GetNeutralObjectAt(x, y);

            Console.WriteLine($"Клетка ({x}, {y})");

            if (terrain != null)
            {
                Console.WriteLine($"Ландшафт: {GetTerrainName(terrain)}");
                Console.WriteLine($"Класс объекта ландшафта: {terrain.GetType().Name}");
            }

            if (unit != null)
            {
                Console.WriteLine($"Юнит: {GetUnitName(unit)}");

                if (terrain != null)
                {
                    Console.WriteLine($"Бонус брони: {terrain.GetDefenseBonus(unit)}");
                    Console.WriteLine($"Множитель скорости: {terrain.GetSpeedMultiplier(unit):F2}");
                }
            }
            else
            {
                Console.WriteLine("Юнит: отсутствует");
            }

            Console.WriteLine($"База: {(gameBase != null ? "Присутствует" : "Отсутствует")}");

            if (neutralObject != null)
            {
                Console.WriteLine($"Нейтральный объект: {GetNeutralObjectName(neutralObject)}");
            }
            else
            {
                Console.WriteLine("Нейтральный объект: отсутствует");
            }
        }

        private static void NeutralObjectMenu()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("=== Нейтральные объекты ===");
                Console.WriteLine("1. Разместить нейтральный объект");
                Console.WriteLine("2. Удалить нейтральный объект");
                Console.WriteLine("3. Взаимодействовать с объектом");
                Console.WriteLine("4. Показать все нейтральные объекты");
                Console.WriteLine("0. Назад");

                int choice = ReadInt("Выберите действие: ");

                if (choice == 0)
                {
                    return;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            PlaceNeutralObject();
                            break;

                        case 2:
                            RemoveNeutralObject();
                            break;

                        case 3:
                            InteractWithNeutralObject();
                            break;

                        case 4:
                            ShowNeutralObjects();
                            break;

                        default:
                            Console.WriteLine("Неверный пункт меню");
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                Pause();
            }
        }

        private static void PlaceNeutralObject()
        {
            if (currentField == null)
            {
                return;
            }

            Console.WriteLine("1. Сундук с сокровищами");
            Console.WriteLine("2. Ловушка");
            Console.WriteLine("3. Лечебное зелье");
            Console.WriteLine("4. Усиливающий алтарь");
            Console.WriteLine("0. Отмена");

            int choice = ReadInt("Выберите объект: ");

            NeutralObject neutralObject;

            switch (choice)
            {
                case 1:
                    neutralObject = new TreasureChest();
                    break;

                case 2:
                    neutralObject = new Trap();
                    break;

                case 3:
                    neutralObject = new HealingPotion();
                    break;

                case 4:
                    neutralObject = new PowerUpAltar();
                    break;

                case 0:
                    return;

                default:
                    Console.WriteLine("Неверный тип объекта");
                    return;
            }

            ShowField();

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            ShowResult(currentField.PlaceNeutralObject(neutralObject, x, y), "Нейтральный объект размещён", "Не удалось разместить нейтральный объект");
        }

        private static void RemoveNeutralObject()
        {
            if (currentField == null)
            {
                return;
            }

            if (!ShowNeutralObjects())
            {
                return;
            }

            if (!ReadCoordinates(out int x, out int y))
            {
                return;
            }

            ShowResult(currentField.RemoveNeutralObject(x, y), "Нейтральный объект удалён", "В указанной клетке нет нейтрального объекта");
        }

        private static void InteractWithNeutralObject()
        {
            if (currentField == null)
            {
                return;
            }

            Unit? unit = SelectUnit();

            if (unit == null)
            {
                return;
            }

            int x = unit.X;
            int y = unit.Y;

            NeutralObject? neutralObject = currentField.GetNeutralObject(x, y);

            if (neutralObject == null)
            {
                Console.WriteLine("В клетке юнита нет нейтрального объекта");
                return;
            }

            if (neutralObject is PowerUpAltar altar && altar.WasUsedBy(unit))
            {
                Console.WriteLine("Этот юнит уже использовал данный алтарь");
                return;
            }

            Console.WriteLine($"Объект: {GetNeutralObjectName(neutralObject)}");

            int previousHealth = unit.CurrentHealth;
            int previousArmor = unit.UnitArmor.Value;
            int previousAttack = unit.UnitAttack.Value;
            int previousResistance = unit.UnitMagicResistance.Value;

            Base? owner = unit.OwnerBase;
            int previousResources = owner != null ? owner.Resources : 0;

            if (!currentField.InteractWithNeutralObject(unit))
            {
                Console.WriteLine("Взаимодействие невозможно");
                return;
            }

            Console.WriteLine("Взаимодействие выполнено");

            if (previousHealth != unit.CurrentHealth)
            {
                Console.WriteLine($"Здоровье: {previousHealth} -> {unit.CurrentHealth}");
            }

            if (previousArmor != unit.UnitArmor.Value)
            {
                Console.WriteLine($"Броня: {previousArmor} -> {unit.UnitArmor.Value}");
            }

            if (previousAttack != unit.UnitAttack.Value)
            {
                Console.WriteLine($"Атака: {previousAttack} -> {unit.UnitAttack.Value}");
            }

            if (previousResistance != unit.UnitMagicResistance.Value)
            {
                Console.WriteLine($"Магическое сопротивление: {previousResistance}% -> {unit.UnitMagicResistance.Value}%");
            }

            if (owner != null && previousResources != owner.Resources)
            {
                Console.WriteLine($"Ресурсы базы: {previousResources} -> {owner.Resources}");
            }

            if (neutralObject.IsUsed)
            {
                Console.WriteLine("Объект израсходован и удалён с поля");
            }
            else
            {
                Console.WriteLine("Объект остался на поле");
            }

            if (!unit.IsAlive)
            {
                Console.WriteLine("Юнит уничтожен");

                Unit? replacement = currentField.GetUnit(x, y);

                if (replacement != null && replacement != unit)
                {
                    Console.WriteLine($"На его месте появился {GetUnitName(replacement)}");
                }
            }
        }

        private static bool ShowNeutralObjects()
        {
            if (currentField == null)
            {
                return false;
            }

            bool found = false;

            Console.WriteLine("=== Нейтральные объекты ===");

            for (int y = 0; y < currentField.Height; y++)
            {
                for (int x = 0; x < currentField.Width; x++)
                {
                    NeutralObject? neutralObject = currentField.GetNeutralObject(x, y);

                    if (neutralObject != null)
                    {
                        Console.WriteLine($"{GetNeutralObjectName(neutralObject)} ({x}, {y})");
                        Console.WriteLine($"Многоразовый: {(neutralObject.IsReusable ? "Да" : "Нет")}");
                        Console.WriteLine("------------------------------");

                        found = true;
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("На поле нет нейтральных объектов");
            }

            return found;
        }

        private static void NextTurn()
        {
            if (currentField == null)
            {
                return;
            }

            currentField.NextTurn();

            Console.WriteLine($"Начался ход {currentField.TurnNumber}");
            Console.WriteLine("Очки перемещения всех юнитов восстановлены");
        }
    }
}
