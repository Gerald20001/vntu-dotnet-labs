using System;
using OopExceptions.Exceptions;

namespace OopExceptions.Models;

/// <summary>
/// Похідний клас для геометричної фігури "Коло".
/// </summary>
public class Circle : Shape
{
    private double _radius;

    /// <summary>
    /// Радіус кола.
    /// Демонстрація інкапсуляції та генерації власного винятку InvalidShapeDimensionException
    /// у разі спроби встановити недодатне значення.
    /// </summary>
    public double Radius
    {
        get => _radius;
        set
        {
            if (value <= 0)
            {
                throw new InvalidShapeDimensionException("Радіус кола", value);
            }
            _radius = value;
        }
    }

    /// <summary>
    /// Конструктор кола з радіусом (використовує назву за замовчуванням "Коло").
    /// </summary>
    public Circle(double radius)
        : base("Коло")
    {
        Radius = radius;
    }

    /// <summary>
    /// Перевантажений конструктор із можливістю задати користувацьку назву фігури.
    /// </summary>
    public Circle(string name, double radius)
        : base(name)
    {
        Radius = radius;
    }

    /// <summary>
    /// Розрахунок площі кола: S = π * r²
    /// </summary>
    public override double GetArea() => Math.PI * Radius * Radius;

    /// <summary>
    /// Розрахунок довжини кола (периметра): P = 2 * π * r
    /// </summary>
    public override double GetPerimeter() => 2 * Math.PI * Radius;

    /// <summary>
    /// Перевизначення віртуального методу виведення інформації з розширенням специфічними даними кола.
    /// </summary>
    public override void PrintInfo()
    {
        Console.WriteLine($"[Коло]        Назва: {Name,-10} | Радіус: {Radius,6:F2} | Площа: {GetArea(),8:F2} | Периметр: {GetPerimeter(),8:F2}");
    }
}
