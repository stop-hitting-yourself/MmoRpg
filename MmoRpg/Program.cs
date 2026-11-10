using System;
using MmoRpg.Units;
using MmoRpg.Field;
using MmoRpg.Factories;

namespace MmoRpg
{
    class Program
    {
        static GameField? field = null;
        static GameField? copiedField = null;
        static int maxUnits = 0;
        static int copiedMaxUnits = 0;
        static bool usingCopy = false;

        static void Main()
        {
            int choice;
            do
            {
                Console.Clear();
                if (field == null)
                    Console.WriteLine("Игровое поле не создано.");
                else
                {
                    Console.WriteLine(usingCopy ? "Текущее поле - копия" : "Текущее поле - оригинал");
                    PrintField(field);
                }

                Console.WriteLine("1. Создать игровое поле");
                Console.WriteLine("2. Показать список юнитов");
                Console.WriteLine("3. Добавить юнита");
                Console.WriteLine("4. Переместить юнита");
                Console.WriteLine("5. Атаковать юнита");
                Console.WriteLine("6. Специальные способности");
                Console.WriteLine("7. Информация о юните");
                Console.WriteLine("8. Удалить юнита");
                Console.WriteLine("9. Скопировать поле");
                Console.WriteLine("10. Переключить поле");
                Console.WriteLine("11. Обход поля итератором");
                Console.WriteLine("12. Очистить поле");
                Console.WriteLine("13. Изменить характеристики юнита");
                Console.WriteLine("0. Выход");

                choice = ReadInt("\nВыберите действие: ");
                Console.Clear();
                switch (choice)
                {
                    case 1: CreateField(); break;
                    case 2: ShowUnits(); break;
                    case 3: AddUnit(); break;
                    case 4: MoveUnit(); break;
                    case 5: AttackUnit(); break;
                    case 6: SpecialAbility(); break;
                    case 7: ShowUnitInfo(); break;
                    case 8: RemoveUnit(); break;
                    case 9: CopyField(); break;
                    case 10: SwitchField(); break;
                    case 11: ShowIterator(); break;
                    case 12: ClearField(); break;
                    case 13: ChangeUnitAttributes(); break;
                    case 0: Console.WriteLine("Завершение программы."); break;
                    default: Console.WriteLine("Неверный пункт меню."); break;
                }
                if (choice != 0)
                {
                    Console.ReadLine();
                }
            } while (choice != 0);
        }

        static int ReadInt(string message)
        {
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out int value))
                    return value;
                Console.WriteLine("Ошибка! Введите целое число.");
            }
        }

        static bool CheckUnits()
        {
            if (field == null)
            {
                Console.WriteLine("Сначала создайте игровое поле.");
                return false;
            }
            if (field.Empty)
            {
                Console.WriteLine("Поле пусто. Сначала добавьте юнитов.");
                return false;
            }
            return true;
        }

        static void CreateField()
        {
            int width = ReadInt("Ширина: ");
            int height = ReadInt("Высота: ");
            int limit = ReadInt("Максимум юнитов: ");
            if (width <= 0 || height <= 0)
            {
                Console.WriteLine("Размеры поля должны быть положительными.");
                return;
            }
            if (limit < 0 || limit > (long)width * height)
            {
                Console.WriteLine("Недопустимое количество юнитов.");
                return;
            }
            field = new GameField(width, height, limit);
            maxUnits = limit;
            copiedField = null;
            copiedMaxUnits = 0;
            usingCopy = false;
            Console.WriteLine("\nПоле успешно создано");
        }

        static char GetUnitSymbol(Unit unit)
        {
            if (unit is Swordsman) return 'S';
            if (unit is Spearman) return 'P';
            if (unit is LightArcher) return 'L';
            if (unit is HeavyArcher) return 'H';
            if (unit is Cataphract) return 'C';
            if (unit is HorseArcher) return 'R';
            return 'U';
        }

        static void PrintField(GameField gameField)
        {
            for (int x = 0; x < gameField.Width + 2; x++)
                Console.Write(" X ");
            Console.WriteLine();
            for (int y = 0; y < gameField.Height; y++)
            {
                Console.Write(" X ");
                for (int x = 0; x < gameField.Width; x++)
                {
                    Unit? unit = gameField.GetUnit(x, y);
                    char symbol = unit == null ? '.' : GetUnitSymbol(unit);
                    Console.Write($" {symbol} ");
                }
                Console.WriteLine(" X ");
            }
            for (int x = 0; x < gameField.Width + 2; x++)
                Console.Write(" X ");
            Console.WriteLine();
            Console.WriteLine($"\nЗанято мест: {gameField.Size} / {maxUnits}");
        }

        static void PrintUnitInfo(Unit unit)
        {
            Console.WriteLine($"Тип: {unit.GetUnitType()}");
            Console.WriteLine($"Позиция: ({unit.X}, {unit.Y})");
            Console.WriteLine($"Здоровье: {unit.UnitHealth.Value} / {unit.UnitHealth.MaxValue}");
            Console.WriteLine($"Броня: {unit.UnitArmor.Value}");
            Console.WriteLine($"Атака: {unit.UnitAttack.Value}");
            Console.WriteLine($"Тип урона: {unit.UnitAttack.Type}");
            Console.WriteLine($"Магическое сопротивление: {unit.UnitMagicResistance.Value}%");
            if (unit is Spearman spearman)
                Console.WriteLine($"Щит поднят: {(spearman.ShieldRaised ? "Да" : "Нет")}");
            Console.WriteLine($"Состояние: {(unit.UnitHealth.IsZero() ? "Погиб" : "Жив")}");
        }

        static void ShowUnits()
        {
            if (!CheckUnits()) return;
            int number = 1;
            foreach (Unit unit in field!)
            {
                Console.WriteLine($"Юнит {number++}");
                PrintUnitInfo(unit);
                Console.WriteLine();
            }
            Console.WriteLine($"Всего юнитов: {field!.Size}");
        }

        static void ShowOccupiedCells(GameField gameField, Unit? excludedUnit = null, bool onlyLiving = false)
        {
            Console.WriteLine("\nДоступные юниты:");
            int count = 0;
            foreach (Unit unit in gameField)
            {
                if (ReferenceEquals(unit, excludedUnit)) continue;
                if (onlyLiving && unit.UnitHealth.IsZero()) continue;
                Console.WriteLine($"({unit.X}, {unit.Y}) - {unit.GetUnitType()}");
                count++;
            }
            if (count == 0)
                Console.WriteLine("Нет доступных юнитов.");
        }

        static void ShowCoordinateRange(GameField gameField)
        {
            Console.WriteLine("\nДопустимые координаты:");
            Console.WriteLine($"X: от 0 до {gameField.Width - 1}");
            Console.WriteLine($"Y: от 0 до {gameField.Height - 1}");
        }

        static Unit? SelectUnit(Unit? excludedUnit = null, bool onlyLiving = false)
        {
            if (!CheckUnits()) return null;
            int available = 0;
            foreach (Unit unit in field!)
            {
                if (ReferenceEquals(unit, excludedUnit)) continue;
                if (onlyLiving && unit.UnitHealth.IsZero()) continue;
                available++;
            }
            if (available == 0)
            {
                Console.WriteLine("Нет доступных юнитов для выбора.");
                return null;
            }
            ShowCoordinateRange(field!);
            ShowOccupiedCells(field!, excludedUnit, onlyLiving);
            int x = ReadInt("\nКоордината X: ");
            int y = ReadInt("Координата Y: ");
            Unit? selected = field!.GetUnit(x, y);
            if (selected == null || ReferenceEquals(selected, excludedUnit) ||
                (onlyLiving && selected.UnitHealth.IsZero()))
            {
                Console.WriteLine("По указанным координатам нет доступного юнита.");
                return null;
            }
            return selected;
        }

        static void AddUnit()
        {
            if (field == null)
            {
                Console.WriteLine("Сначала создайте поле.");
                return;
            }
            if (field.Size >= maxUnits)
            {
                Console.WriteLine("Достигнут лимит юнитов.");
                return;
            }
            Console.WriteLine("1. Мечник");
            Console.WriteLine("2. Копейщик");
            Console.WriteLine("3. Лёгкий лучник");
            Console.WriteLine("4. Тяжёлый лучник");
            Console.WriteLine("5. Катафрактарий");
            Console.WriteLine("6. Конный лучник");
            int choice = ReadInt("\nВыберите тип: ");
            UnitFactory factory;
            switch (choice)
            {
                case 1: factory = new InfantryFactory("Swordsman"); break;
                case 2: factory = new InfantryFactory("Spearman"); break;
                case 3: factory = new ArcherFactory("LightArcher"); break;
                case 4: factory = new ArcherFactory("HeavyArcher"); break;
                case 5: factory = new CavalryFactory("Cataphract"); break;
                case 6: factory = new CavalryFactory("HorseArcher"); break;
                default:
                    Console.WriteLine("Неверный тип юнита.");
                    return;
            }
            ShowCoordinateRange(field);
            Console.WriteLine("\nЗанятые клетки:");
            if (field.Empty) Console.WriteLine("Нет занятых клеток.");
            else
                foreach (Unit unit in field)
                    Console.WriteLine($"({unit.X}, {unit.Y}) - {unit.GetUnitType()}");
            int x = ReadInt("\nКоордината X: ");
            int y = ReadInt("Координата Y: ");
            if (field.CreateUnit(factory, x, y))
                Console.WriteLine("\nЮнит успешно добавлен!");
            else
                Console.WriteLine("\nНе удалось добавить юнита: клетка занята или координаты недопустимы.");
        }

        static void MoveUnit()
        {
            if (!CheckUnits()) return;
            Unit? unit = SelectUnit();
            if (unit == null) return;
            Console.WriteLine($"\nВыбран: {unit.GetUnitType()}");
            ShowCoordinateRange(field!);
            Console.WriteLine("\nЗанятые клетки:");
            foreach (Unit other in field!)
                Console.WriteLine($"({other.X}, {other.Y}) - {other.GetUnitType()}");
            int x = ReadInt("\nНовая координата X: ");
            int y = ReadInt("Новая координата Y: ");
            Console.WriteLine(unit.Move(x, y) ? "\nЮнит перемещён!" : "\nПеремещение невозможно.");
        }

        static void AttackUnit()
        {
            if (!CheckUnits()) return;
            Console.WriteLine("Выберите атакующего:");
            Unit? attacker = SelectUnit(onlyLiving: true);
            if (attacker == null) return;
            Console.WriteLine("\nВыберите цель:");
            Unit? target = SelectUnit(excludedUnit: attacker, onlyLiving: true);
            if (target == null) return;
            if (ReferenceEquals(attacker, target))
            {
                Console.WriteLine("Нельзя атаковать самого себя.");
                return;
            }
            int targetX = target.X;
            int targetY = target.Y;
            int oldHealth = target.UnitHealth.Value;
            Console.WriteLine($"\n{attacker.GetUnitType()} атакует {target.GetUnitType()}!");
            attacker.AttackUnit(target);
            Console.WriteLine($"Здоровье цели до атаки: {oldHealth}");
            Console.WriteLine($"Здоровье цели после атаки: {target.UnitHealth.Value}");
            if (target.UnitHealth.IsZero())
            {
                Console.WriteLine("Цель погибла.");
                Unit? replacement = field!.GetUnit(targetX, targetY);
                if (replacement != null && !ReferenceEquals(replacement, target))
                    Console.WriteLine($"На её месте появился: {replacement.GetUnitType()}");
            }
        }

        static void SpecialAbility()
        {
            if (!CheckUnits()) return;
            Unit? unit = SelectUnit(onlyLiving: true);
            if (unit == null) return;
            if (unit is Spearman spearman)
            {
                Console.WriteLine("\n1. Поднять щит");
                Console.WriteLine("2. Опустить щит");
                int choice = ReadInt("\nДействие: ");
                if (choice == 1)
                {
                    if (spearman.ShieldRaised) Console.WriteLine("Щит уже поднят.");
                    else { spearman.RaiseShield(); Console.WriteLine("Щит поднят."); }
                }
                else if (choice == 2)
                {
                    if (!spearman.ShieldRaised) Console.WriteLine("Щит уже опущен.");
                    else { spearman.LowerShield(); Console.WriteLine("Щит опущен."); }
                }
                else Console.WriteLine("Неверное действие.");
            }
            else if (unit is LightArcher lightArcher)
            {
                lightArcher.ChangeDamageType();
                Console.WriteLine($"\nТип урона: {lightArcher.UnitAttack.Type}");
                Console.WriteLine($"Атака: {lightArcher.UnitAttack.Value}");
            }
            else if (unit is HeavyArcher heavyArcher)
            {
                Console.WriteLine("\n1. Сменить тип урона");
                Console.WriteLine("2. Вылечиться на 50 HP");
                int choice = ReadInt("\nДействие: ");
                if (choice == 1)
                {
                    heavyArcher.ChangeDamageType();
                    Console.WriteLine($"Тип урона: {heavyArcher.UnitAttack.Type}");
                }
                else if (choice == 2)
                {
                    heavyArcher.HealSelf();
                    Console.WriteLine($"Здоровье: {heavyArcher.UnitHealth.Value} / {heavyArcher.UnitHealth.MaxValue}");
                }
                else Console.WriteLine("Неверное действие.");
            }
            else if (unit is Cataphract || unit is HorseArcher)
                Console.WriteLine("\nСпособность срабатывает автоматически после гибели.");
            else
                Console.WriteLine("\nУ этого юнита нет специальной способности.");
        }

        static void ShowUnitInfo()
        {
            if (!CheckUnits()) return;
            Unit? unit = SelectUnit();
            if (unit == null) return;
            Console.WriteLine();
            PrintUnitInfo(unit);
        }

        static void RemoveUnit()
        {
            if (!CheckUnits()) return;
            Unit? unit = SelectUnit();
            if (unit == null) return;
            Console.WriteLine(field!.RemoveUnit(unit) ? "\nЮнит удалён." : "\nНе удалось удалить юнита.");
        }

        static void ChangeUnitAttributes()
        {
            if (!CheckUnits()) return;
            Unit? unit = SelectUnit();
            if (unit == null) return;
            Console.WriteLine("\nВыбранный юнит:");
            PrintUnitInfo(unit);
            Console.WriteLine("\n1. Здоровье");
            Console.WriteLine("2. Броня");
            Console.WriteLine("3. Атака");
            Console.WriteLine("4. Магическое сопротивление");
            Console.WriteLine("0. Отмена");
            int choice = ReadInt("\nЧто изменить: ");
            if (choice == 0) return;
            if (choice < 1 || choice > 4)
            {
                Console.WriteLine("Неверный пункт меню.");
                return;
            }
            Console.WriteLine("\nПоложительное число увеличивает значение, отрицательное уменьшает.");
            int change = ReadInt("Изменение: ");
            if (change == 0)
            {
                Console.WriteLine("Характеристика не изменилась.");
                return;
            }
            if (change == int.MinValue)
            {
                Console.WriteLine("Слишком большое изменение.");
                return;
            }
            int amount = Math.Abs(change);
            int oldX = unit.X;
            int oldY = unit.Y;

            switch (choice)
            {
                case 1:
                    if (change > 0)
                    {
                        if (unit.UnitHealth.IsZero())
                        {
                            Console.WriteLine("Нельзя лечить погибшего юнита.");
                            return;
                        }
                        amount = Math.Min(amount, unit.UnitHealth.MaxValue - unit.UnitHealth.Value);
                        unit.TakeHeal(amount);
                    }
                    else
                    {
                        amount = Math.Min(amount, unit.UnitHealth.Value);
                        unit.TakeDamage(amount);
                    }
                    break;
                case 2:
                    if (change > 0)
                    {
                        if ((long)unit.UnitArmor.Value + amount > int.MaxValue)
                        {
                            Console.WriteLine("Значение брони слишком велико.");
                            return;
                        }
                        unit.IncreaseArmor(amount);
                    }
                    else
                    {
                        if ((long)unit.UnitArmor.Value - amount < int.MinValue)
                        {
                            Console.WriteLine("Значение брони слишком мало.");
                            return;
                        }
                        unit.DecreaseArmor(amount);
                    }
                    break;
                case 3:
                    if (change > 0)
                    {
                        if ((long)unit.UnitAttack.Value + amount > int.MaxValue)
                        {
                            Console.WriteLine("Значение атаки слишком велико.");
                            return;
                        }
                        unit.IncreaseAttack(amount);
                    }
                    else
                    {
                        amount = Math.Min(amount, unit.UnitAttack.Value);
                        unit.DecreaseAttack(amount);
                    }
                    break;
                case 4:
                    if (change > 0)
                    {
                        amount = Math.Min(amount, 100 - unit.UnitMagicResistance.Value);
                        unit.IncreaseMagicResistance(amount);
                    }
                    else
                    {
                        amount = Math.Min(amount, unit.UnitMagicResistance.Value);
                        unit.DecreaseMagicResistance(amount);
                    }
                    break;
            }
            Console.WriteLine("\nХарактеристики после изменения:");
            Unit? currentUnit = field!.GetUnit(oldX, oldY);
            if (currentUnit != null) PrintUnitInfo(currentUnit);
            else Console.WriteLine("Юнит больше не находится на поле.");
        }

        static void CopyField()
        {
            if (field == null)
            {
                Console.WriteLine("Сначала создайте поле.");
                return;
            }
            copiedField = new GameField(field);
            copiedMaxUnits = maxUnits;
            Console.WriteLine("Независимая копия текущего поля создана!");
            Console.WriteLine("Пункт 10 переключает между двумя полями.");
        }

        static void SwitchField()
        {
            if (field == null || copiedField == null)
            {
                Console.WriteLine("Сначала создайте поле и его копию.");
                return;
            }
            GameField temp = field;
            field = copiedField;
            copiedField = temp;
            int tempLimit = maxUnits;
            maxUnits = copiedMaxUnits;
            copiedMaxUnits = tempLimit;
            usingCopy = !usingCopy;
            Console.WriteLine(usingCopy ? "Переключено на копию." : "Переключено на оригинал.");
        }

        static void ShowIterator()
        {
            if (!CheckUnits()) return;
            int number = 1;
            foreach (Unit unit in field!)
                Console.WriteLine($"{number++}. {unit.GetUnitType()} ({unit.X}, {unit.Y})");
        }

        static void ClearField()
        {
            if (!CheckUnits()) return;
            Console.WriteLine("1. Удалить всех юнитов");
            Console.WriteLine("2. Отмена");
            int choice = ReadInt("\nВыбор: ");
            if (choice == 1)
            {
                field!.Clear();
                Console.WriteLine("\nПоле очищено!");
            }
            else Console.WriteLine("\nОчистка отменена.");
        }
    }
}
