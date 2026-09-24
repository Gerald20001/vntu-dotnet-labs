using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinqToObjects;

public record Student(string Name, string Group, int Score);
public record GroupCurator(string Group, string CuratorName, string Department);

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Початкова колекція студентів (25 записів, 4 різні групи)
        List<Student> students = new()
        {
            new("Іваненко Олександр", "ПЗ-21", 95),
            new("Петренко Марія", "ПЗ-21", 88),
            new("Сидоренко Дмитро", "ПЗ-21", 72),
            new("Ковальчук Анна", "ПЗ-21", 54),
            new("Мельник Богдан", "ПЗ-21", 67),
            new("Шевченко Вікторія", "ПЗ-21", 91),

            new("Бондар Максим", "ПЗ-22", 82),
            new("Ткаченко Олена", "ПЗ-22", 98),
            new("Кравченко Ігор", "ПЗ-22", 59),
            new("Олійник Софія", "ПЗ-22", 76),
            new("Поліщук Владислав", "ПЗ-22", 84),
            new("Лисенко Катерина", "ПЗ-22", 63),

            new("Григоренко Артем", "КН-21", 90),
            new("Мороз Назарій", "КН-21", 45),
            new("Павленко Юлія", "КН-21", 79),
            new("Марченко Денис", "КН-21", 85),
            new("Руденко Анастасія", "КН-21", 94),
            new("Савченко Ярослав", "КН-21", 68),

            new("Кузьменко Тарас", "ІПЗ-21", 73),
            new("Левченко Дарина", "ІПЗ-21", 89),
            new("Семенюк Роман", "ІПЗ-21", 52),
            new("Коваленко Сергій", "ІПЗ-21", 96),
            new("Федоренко Ірина", "ІПЗ-21", 81),
            new("Данилюк Вадим", "ІПЗ-21", 64),
            new("Захарченко Михайло", "ІПЗ-21", 77)
        };

        // Супутня колекція для демонстрації оператора Join
        List<GroupCurator> curators = new()
        {
            new("ПЗ-21", "доц. Коваленко О.І.", "Кафедра ПЗ"),
            new("ПЗ-22", "проф. Мельник Т.П.", "Кафедра ПЗ"),
            new("КН-21", "доц. Войтович В.С.", "Кафедра КН"),
            new("ІПЗ-21", "ст.вик. Захарова Н.В.", "Кафедра ПЗ")
        };

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 9);
            Console.WriteLine();

            isRunning = choice switch
            {
                1 => HandleQuery1_FilterAndSort(students),
                2 => HandleQuery2_AverageByGroup(students),
                3 => HandleQuery3_Top5(students),
                4 => HandleQuery4_GradeDistribution(students),
                5 => HandleQuery5_CheckAllAboveThreshold(students),
                6 => HandleQuery6_CheckAnyHonorStudent(students),
                7 => HandleQuery7_JoinCurators(students, curators),
                8 => HandleQuery8_AggregateStats(students),
                9 => HandleExportToCsv(students),
                0 => false,
                _ => true
            };

            if (isRunning)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Програму ПР №4 завершено. До зустрічі!");
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    Практична робота №4 | Варіант 1 (№17 у списку)   ");
        Console.WriteLine("        LINQ to Objects: Студенти та оцінки           ");
        Console.WriteLine("=======================================================");
        Console.WriteLine("1. Фільтрація та сортування (Where + OrderBy + Select)");
        Console.WriteLine("2. Середній бал за групами (GroupBy + Average) [Метод + Query]");
        Console.WriteLine("3. Топ-5 студентів за балом (OrderByDescending + Take)");
        Console.WriteLine("4. Розподіл за категоріями оцінок (обчислюваний GroupBy)");
        Console.WriteLine("5. Перевірка All: чи всі студенти групи мають бал >= поріг");
        Console.WriteLine("6. Перевірка Any: чи є в групі відмінники (бал >= 90)");
        Console.WriteLine("7. З'єднання з кураторами груп (Join) [Метод + Query]");
        Console.WriteLine("8. Загальна підсумкова статистика (Count, Min, Max, Average)");
        Console.WriteLine("9. Зберегти підсумковий звіт у файл CSV (students_report.csv)");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("-------------------------------------------------------");
    }

    // =========================================================================
    // Запити LINQ
    // =========================================================================

    // 1. Where, OrderByDescending, ThenBy, Select (проєкція в анонімний тип)
    private static bool HandleQuery1_FilterAndSort(List<Student> students)
    {
        Console.WriteLine("--- 1. Фільтрація та сортування студентів ---");
        int minScore = ReadInt("Введіть мінімальний бал для фільтру (0-100): ", 0, 100);

        var query = students
            .Where(s => s.Score >= minScore)
            .OrderByDescending(s => s.Score)
            .ThenBy(s => s.Name)
            .Select(s => new { FullName = s.Name, GroupName = s.Group, Points = s.Score })
            .ToList();

        Console.WriteLine($"\nЗнайдено студентів з балом >= {minScore}: {query.Count}");
        foreach (var s in query)
        {
            Console.WriteLine($"  • {s.FullName,-22} | Група: {s.GroupName,-7} | Бал: {s.Points,3}");
        }

        return true;
    }

    // 2. GroupBy + Average (порівняння method syntax та query syntax)
    private static bool HandleQuery2_AverageByGroup(List<Student> students)
    {
        Console.WriteLine("--- 2. Середній бал за групами (GroupBy + Average) ---");

        // Спосіб А: Method Syntax
        var methodSyntaxResult = students
            .GroupBy(s => s.Group)
            .Select(g => new
            {
                Group = g.Key,
                AvgScore = g.Average(s => s.Score),
                Count = g.Count()
            })
            .OrderByDescending(g => g.AvgScore)
            .ToList();

        // Спосіб Б: Query Syntax (SQL-подібний)
        var querySyntaxResult = (from s in students
                                 group s by s.Group into g
                                 orderby g.Average(x => x.Score) descending
                                 select new
                                 {
                                     Group = g.Key,
                                     AvgScore = g.Average(x => x.Score),
                                     Count = g.Count()
                                 }).ToList();

        Console.WriteLine("Результати (Method-синтаксис):");
        foreach (var item in methodSyntaxResult)
        {
            Console.WriteLine($"  Група: {item.Group,-7} | Середній бал: {item.AvgScore,5:F2} | Студентів: {item.Count,2}");
        }

        Console.WriteLine("\nПеревірка Query-синтаксису (ідентичний результат):");
        Console.WriteLine($"  Найкраща група за рейтингом: {querySyntaxResult.First().Group} ({querySyntaxResult.First().AvgScore:F2})");

        return true;
    }

    // 3. Топ-5 студентів за оцінкою
    private static bool HandleQuery3_Top5(List<Student> students)
    {
        Console.WriteLine("--- 3. Топ-5 студентів за успішністю (OrderByDescending + Take) ---");

        var top5 = students
            .OrderByDescending(s => s.Score)
            .Take(5)
            .ToList();

        int rank = 1;
        foreach (var s in top5)
        {
            Console.WriteLine($"  #{rank++,2}. {s.Name,-22} | Група: {s.Group,-7} | Бал: {s.Score}");
        }

        return true;
    }

    // 4. Розподіл за обчислюваним ключем
    private static bool HandleQuery4_GradeDistribution(List<Student> students)
    {
        Console.WriteLine("--- 4. Розподіл за академічними категоріями ---");

        var distribution = students
            .GroupBy(s => s.Score switch
            {
                >= 90 => "1. Відмінно (90–100)",
                >= 75 => "2. Добре (75–89)",
                >= 60 => "3. Задовільно (60–74)",
                _ => "4. Незадовільно (<60)"
            })
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                Category = g.Key,
                Count = g.Count(),
                Percentage = (double)g.Count() / students.Count * 100,
                Students = g.Select(s => s.Name).ToList()
            });

        foreach (var cat in distribution)
        {
            Console.WriteLine($"\n{cat.Category}: {cat.Count} студентів ({cat.Percentage:F1}%)");
            Console.WriteLine("  Студенти: " + string.Join(", ", cat.Students));
        }

        return true;
    }

    // 5. Перевірка All та обробка граничного випадку
    private static bool HandleQuery5_CheckAllAboveThreshold(List<Student> students)
    {
        Console.WriteLine("--- 5. Перевірка All (чи всі студенти групи мають бал >= порогу) ---");
        Console.Write("Введіть назву групи (наприклад, ПЗ-21, ПЗ-22, КН-21, ІПЗ-21): ");
        string? targetGroup = Console.ReadLine()?.Trim();

        // Граничний випадок: група не існує
        var groupStudents = students.Where(s => s.Group.Equals(targetGroup, StringComparison.OrdinalIgnoreCase)).ToList();
        if (groupStudents.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Граничний випадок]: Групу '{targetGroup}' не знайдено серед даних!");
            Console.ResetColor();
            return true;
        }

        int threshold = ReadInt("Введіть поріг балів (наприклад, 60): ", 0, 100);

        bool allPass = groupStudents.All(s => s.Score >= threshold);

        Console.WriteLine($"\nПеревірка для групи {groupStudents[0].Group} (усього {groupStudents.Count} студ.):");
        if (allPass)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"-> ТАК! Усі студенти групи мають бал >= {threshold}.");
        }
        else
        {
            var failed = groupStudents.Where(s => s.Score < threshold).Select(s => $"{s.Name} ({s.Score})");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"-> НІ. Не всі студенти досягли порогу {threshold}. Студенти з нижчим балом: {string.Join(", ", failed)}");
        }
        Console.ResetColor();

        return true;
    }

    // 6. Перевірка Any
    private static bool HandleQuery6_CheckAnyHonorStudent(List<Student> students)
    {
        Console.WriteLine("--- 6. Перевірка Any (наявність відмінників >= 90) ---");
        Console.Write("Введіть назву групи: ");
        string? targetGroup = Console.ReadLine()?.Trim();

        var groupStudents = students.Where(s => s.Group.Equals(targetGroup, StringComparison.OrdinalIgnoreCase)).ToList();
        if (groupStudents.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Граничний випадок]: Групу '{targetGroup}' не знайдено.");
            Console.ResetColor();
            return true;
        }

        bool hasExcellence = groupStudents.Any(s => s.Score >= 90);

        if (hasExcellence)
        {
            var honorNames = groupStudents.Where(s => s.Score >= 90).Select(s => $"{s.Name} ({s.Score})");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"У групі {groupStudents[0].Group} Є відмінники: {string.Join(", ", honorNames)}");
        }
        else
        {
            Console.WriteLine($"У групі {groupStudents[0].Group} немає жодного відмінника (бали < 90).");
        }
        Console.ResetColor();

        return true;
    }

    // 7. Join двох колекцій
    private static bool HandleQuery7_JoinCurators(List<Student> students, List<GroupCurator> curators)
    {
        Console.WriteLine("--- 7. З'єднання двох колекцій (Join Student + GroupCurator) ---");

        // Спосіб А: Method Syntax
        var joined = students.Join(
            curators,
            s => s.Group,
            c => c.Group,
            (s, c) => new
            {
                s.Name,
                s.Group,
                s.Score,
                c.CuratorName,
                c.Department
            })
            .OrderBy(x => x.Group)
            .ThenByDescending(x => x.Score)
            .ToList();

        // Спосіб Б: Query Syntax (для порівняння у звіті)
        var joinedQuery = (from s in students
                           join c in curators on s.Group equals c.Group
                           select new { s.Name, s.Group, c.CuratorName }).Take(3).ToList();

        Console.WriteLine($"З'єднано записів: {joined.Count}");
        foreach (var item in joined.Take(8))
        {
            Console.WriteLine($"  • {item.Name,-22} | {item.Group} | Бал: {item.Score,2} | Куратор: {item.CuratorName} ({item.Department})");
        }
        Console.WriteLine("  ... (показано перші 8 записів)");

        return true;
    }

    // 8. Агрегатні підсумки
    private static bool HandleQuery8_AggregateStats(List<Student> students)
    {
        Console.WriteLine("--- 8. Підсумкова агрегатна статистика (Aggregate Functions) ---");

        int totalCount = students.Count();
        int minScore = students.Min(s => s.Score);
        int maxScore = students.Max(s => s.Score);
        double avgScore = students.Average(s => s.Score);
        long totalSum = students.Sum(s => (long)s.Score);

        Console.WriteLine($"• Усього студентів  : {totalCount}");
        Console.WriteLine($"• Мінімальний бал   : {minScore}");
        Console.WriteLine($"• Максимальний бал  : {maxScore}");
        Console.WriteLine($"• Середній бал      : {avgScore:F2}");
        Console.WriteLine($"• Сума всіх балів   : {totalSum}");

        return true;
    }

    // 9. Збереження звіту в CSV
    private static bool HandleExportToCsv(List<Student> students)
    {
        Console.WriteLine("--- 9. Експорт агрегованого звіту у файл CSV ---");

        var statsByGroup = students
            .GroupBy(s => s.Group)
            .Select(g => new
            {
                Group = g.Key,
                AvgScore = g.Average(s => s.Score),
                MinScore = g.Min(s => s.Score),
                MaxScore = g.Max(s => s.Score),
                Count = g.Count()
            })
            .OrderBy(g => g.Group);

        var lines = new List<string>
        {
            "Група;Кількість студентів;Середній бал;Мін. бал;Макс. бал"
        };

        foreach (var item in statsByGroup)
        {
            lines.Add($"{item.Group};{item.Count};{item.AvgScore:F2};{item.MinScore};{item.MaxScore}");
        }

        string filePath = "students_report.csv";
        File.WriteAllLines(filePath, lines, Encoding.UTF8);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[Успішно] Підсумковий звіт збережено у файл '{Path.GetFullPath(filePath)}'.");
        Console.ResetColor();

        Console.WriteLine("\nВміст згенерованого файлу:");
        foreach (var line in lines)
        {
            Console.WriteLine("  " + line);
        }

        return true;
    }

    private static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min && value <= max)
            {
                return value;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Помилка: введіть ціле число від {min} до {max}.");
            Console.ResetColor();
        }
    }
}
