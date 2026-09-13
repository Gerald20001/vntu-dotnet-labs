using System;

namespace OopExceptions.Models;

/// <summary>
/// Базовий абстрактний клас геометричної фігури.
/// Реалізує інтерфейс IComparable&lt;Shape&gt; для природного впорядкування (сортування за площею).
/// </summary>
public abstract class Shape : IComparable<Shape>
{
    private string _name = string.Empty;

    /// <summary>
    /// Назва фігури.
    /// Демонстрація інкапсуляції та валідації у set-аксесорі: порожнє або пробільне ім'я заборонене.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Назва фігури не може бути порожньою або складатись лише з пробілів.", nameof(value));
            }
            _name = value.Trim();
        }
    }

    /// <summary>
    /// Захищений конструктор для ініціалізації базового класу.
    /// </summary>
    /// <param name="name">Назва фігури</param>
    protected Shape(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Абстрактний метод для розрахунку площі фігури.
    /// Обов'язковий для реалізації в кожному похідному класі.
    /// </summary>
    public abstract double GetArea();

    /// <summary>
    /// Абстрактний метод для розрахунку периметра фігури.
    /// Обов'язковий для реалізації в кожному похідному класі.
    /// </summary>
    public abstract double GetPerimeter();

    /// <summary>
    /// Віртуальний метод для виведення інформації про фігуру.
    /// Може бути перевизначений (override) та доповнений у похідних класах.
    /// </summary>
    public virtual void PrintInfo()
    {
        Console.WriteLine($"Фігура: {Name,-15} | Площа: {GetArea(),8:F2} | Периметр: {GetPerimeter(),8:F2}");
    }

    /// <summary>
    /// Реалізація інтерфейсу IComparable&lt;Shape&gt;.
    /// Дозволяє сортувати колекцію об'єктів Shape (наприклад, через List&lt;Shape&gt;.Sort())
    /// за зростанням їхньої площі.
    /// </summary>
    public int CompareTo(Shape? other)
    {
        if (other is null)
        {
            return 1;
        }

        return GetArea().CompareTo(other.GetArea());
    }

    public override string ToString()
    {
        return $"{Name} (S = {GetArea():F2}, P = {GetPerimeter():F2})";
    }
}
