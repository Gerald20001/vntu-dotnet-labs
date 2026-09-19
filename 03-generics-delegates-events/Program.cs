using System;
using System.Collections.Generic;
using System.Text;
using GenericsDelegatesEvents.Collections;
using GenericsDelegatesEvents.Events;
using GenericsDelegatesEvents.Exceptions;

namespace GenericsDelegatesEvents;

internal class Program
{
    // Глобальний лічильник відхилених через переповнення елементів (для підписника-лямбди)
    private static int _overflowRejectionsCount = 0;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        // Створюємо стек цілих чисел з обмеженою місткістю 5
        var numberStack = new MyStack<int>(capacity: 5);

        // =====================================================================
        // Підписка щонайменше 2 обробників на подію Overflow
        // =====================================================================
        // Підписник 1: класичний іменований метод
        numberStack.Overflow += LogOverflowToConsole;

        // Підписник 2: лямбда-вираз із замиканням (closures) на лічильник відхилень
        numberStack.Overflow += (sender, e) =>
        {
            _overflowRejectionsCount++;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[Підписник 2 (Лямбда)] Зафіксовано спробу переповнення! Загальна кількість відхилень: {_overflowRejectionsCount}");
            Console.ResetColor();
        };

        // Попереднє заповнення стека початковими даними
        numberStack.Push(10);
        numberStack.Push(25);
        numberStack.Push(42);

        bool isRunning = true;

        while (isRunning)
        {
            PrintMenu();
            int choice = ReadInt("Ваш вибір: ", 0, 7);
            Console.WriteLine();

            isRunning = choice switch
            {
                1 => HandlePush(numberStack),
                2 => HandlePop(numberStack),
                3 => HandlePeek(numberStack),
                4 => HandleShowStack(numberStack),
                5 => HandleFilterStack(numberStack),
                6 => HandleStringStackDemo(),
                7 => HandleOverflowStressTest(numberStack),
                0 => false,
                _ => true
            };

            if (isRunning)
            {
                Console.WriteLine("\nНатисніть Enter для повернення в меню...");
                Console.ReadLine();
            }
        }

        Console.WriteLine("Програму ПР №3 завершено. До зустрічі!");
    }

    // =========================================================================
    // Меню та обробники
    // =========================================================================
    private static void PrintMenu()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("    Практична робота №3 | Варіант 1 (№17 у списку)   ");
        Console.WriteLine("       Узагальнений стек MyStack<T> з подіями          ");
        Console.WriteLine("=======================================================");
        Console.WriteLine("1. Додати число в стек (Push)");
        Console.WriteLine("2. Вилучити число зі стека (Pop) [LIFO]");
        Console.WriteLine("3. Переглянути вершину стека (Peek)");
        Console.WriteLine("4. Показати вміст стека (з використанням Action<T>)");
        Console.WriteLine("5. Фільтрувати елементи за критерієм (Predicate<T>)");
        Console.WriteLine("6. Демонстрація узагальненого стека рядків MyStack<string>");
        Console.WriteLine("7. Спровокувати подію переповнення (Push понад місткість)");
        Console.WriteLine("0. Вихід");
        Console.WriteLine("-------------------------------------------------------");
    }

    private static bool HandlePush(MyStack<int> stack)
    {
        Console.WriteLine("--- Додавання елемента (Push) ---");
        int value = ReadInt("Введіть ціле число: ");

        bool added = stack.Push(value);
        if (added)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Успішно] Елемент {value} додано на вершину стека. (Заповнено: {stack.Count}/{stack.Capacity})");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"[Інфо] Елемент {value} не додано, оскільки ліміт місткості вичерпано.");
        }

        return true;
    }

    private static bool HandlePop(MyStack<int> stack)
    {
        Console.WriteLine("--- Вилучення елемента з вершини (Pop) ---");

        try
        {
            int popped = stack.Pop();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[Успішно] З вершини вилучено число: {popped}. (Залишилось: {stack.Count}/{stack.Capacity})");
            Console.ResetColor();
        }
        catch (EmptyStackException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Граничний випадок]: {ex.Message}");
            Console.ResetColor();
        }

        return true;
    }

    private static bool HandlePeek(MyStack<int> stack)
    {
        Console.WriteLine("--- Перегляд вершини (Peek) ---");

        try
        {
            int top = stack.Peek();
            Console.WriteLine($"Верхній елемент стека: {top} (без вилучення).");
        }
        catch (EmptyStackException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Граничний випадок]: {ex.Message}");
            Console.ResetColor();
        }

        return true;
    }

    private static bool HandleShowStack(MyStack<int> stack)
    {
        Console.WriteLine($"--- Вміст стека (Заповнено: {stack.Count}/{stack.Capacity}) ---");

        if (stack.IsEmpty)
        {
            Console.WriteLine("Стек порожній.");
            return true;
        }

        Console.WriteLine("Елементи від вершини до дна (обхід через делегат Action<T>):");
        int level = 1;

        // Демонстрація 1 використання делегата Action<T> через лямбда-вираз
        stack.ForEach(item =>
        {
            string marker = level == 1 ? "<- [ВЕРШИНА]" : "";
            Console.WriteLine($"  Рівень {level++}: {item,5} {marker}");
        });

        // Демонстрація 2 використання Func<T, TResult> для трансформації
        List<string> representations = stack.Select(x => $"[Число #{x}]");
        Console.WriteLine($"\nФорматований вигляд: {string.Join(" -> ", representations)}");

        return true;
    }

    private static bool HandleFilterStack(MyStack<int> stack)
    {
        Console.WriteLine("--- Фільтрація елементів через Predicate<T> ---");

        if (stack.IsEmpty)
        {
            Console.WriteLine("Стек порожній. Немає даних для фільтрації.");
            return true;
        }

        Console.WriteLine("Оберіть критерій фільтрації:");
        Console.WriteLine("1. Тільки парні числа (x % 2 == 0)");
        Console.WriteLine("2. Числа більші за поріг (x > threshold)");
        int filterType = ReadInt("Ваш вибір (1 або 2): ", 1, 2);

        Predicate<int> predicate;

        if (filterType == 1)
        {
            // Демонстрація Predicate<T> з лямбдою
            predicate = x => x % 2 == 0;
            Console.WriteLine("\nКритерій: Парні числа");
        }
        else
        {
            int threshold = ReadInt("Введіть порогове значення: ");
            // Демонстрація замикання (closure): лямбда захоплює локальну змінну threshold
            predicate = x => x > threshold;
            Console.WriteLine($"\nКритерій: Числа > {threshold}");
        }

        List<int> matched = stack.FindAll(predicate);

        // Демонстрація 3 Func<T, bool> для підрахунку
        int totalMatches = stack.CountWhere(x => predicate(x));

        Console.WriteLine($"Знайдено елементів: {matched.Count} (перевірка CountWhere: {totalMatches})");
        if (matched.Count > 0)
        {
            Console.WriteLine($"Результати: {string.Join(", ", matched)}");
        }

        return true;
    }

    private static bool HandleStringStackDemo()
    {
        Console.WriteLine("--- Демонстрація роботи узагальненого стека на рядках MyStack<string> ---");

        var stringStack = new MyStack<string>(capacity: 3);

        // Підписка на подію для рядкового стека
        stringStack.Overflow += (sender, e) =>
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[Overflow для String-стека]: Текст \"{e.RejectedItem}\" не помістився в ліміт {e.Capacity}!");
            Console.ResetColor();
        };

        Console.WriteLine("Додаємо рядки: \"Київ\", \"Вінниця\", \"Львів\"...");
        stringStack.Push("Київ");
        stringStack.Push("Вінниця");
        stringStack.Push("Львів");

        Console.WriteLine($"Вершина рядкового стека: {stringStack.Peek()}");

        Console.WriteLine("Спроба додати 4-й елемент (\"Одеса\") понад ліміт 3:");
        stringStack.Push("Одеса"); // Викличе Overflow

        Console.WriteLine("\nВилучаємо верхній елемент (Pop): " + stringStack.Pop());
        Console.WriteLine("Нова вершина: " + stringStack.Peek());

        return true;
    }

    private static bool HandleOverflowStressTest(MyStack<int> stack)
    {
        Console.WriteLine("--- Тестування переповнення стека ---");
        Console.WriteLine($"Поточний стан: {stack.Count}/{stack.Capacity}. Додаємо числа від 100 до заповнення і далі:");

        for (int i = 1; i <= 6; i++)
        {
            int val = 100 + i;
            Console.Write($"\nСпроба Push({val})... ");
            bool ok = stack.Push(val);
            if (ok)
            {
                Console.WriteLine($"Успішно! (Заповнено {stack.Count}/{stack.Capacity})");
            }
        }

        return true;
    }

    // =========================================================================
    // Класичний метод-обробник події (Підписник 1)
    // =========================================================================
    private static void LogOverflowToConsole<T>(object? sender, OverflowEventArgs<T> e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n[Підписник 1 (Іменований метод)] ПОДІЯ ПЕРЕПОВНЕННЯ!");
        Console.WriteLine($"  Час: {e.AttemptedAt:HH:mm:ss.fff}");
        Console.WriteLine($"  Відхилений елемент: {e.RejectedItem}");
        Console.WriteLine($"  Максимальна місткість: {e.Capacity}");
        Console.ResetColor();
    }

    // =========================================================================
    // Безпечне введення цілого числа
    // =========================================================================
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
