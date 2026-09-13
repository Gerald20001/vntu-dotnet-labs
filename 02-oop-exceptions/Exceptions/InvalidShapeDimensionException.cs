using System;

namespace OopExceptions.Exceptions;

/// <summary>
/// Власний клас винятку для позначення неприпустимих (недодатних) геометричних розмірів фігури.
/// Забезпечено три стандартні конструктори згідно з рекомендаціями Microsoft .NET.
/// </summary>
public class InvalidShapeDimensionException : Exception
{
    /// <summary>
    /// Назва виміру (наприклад, "Радіус", "Ширина", "Висота"), що спричинив помилку.
    /// </summary>
    public string? DimensionName { get; }

    /// <summary>
    /// Некоректне значення виміру.
    /// </summary>
    public double InvalidValue { get; }

    public InvalidShapeDimensionException()
        : base("Розмір геометричної фігури має бути строго додатним числом (> 0).")
    {
    }

    public InvalidShapeDimensionException(string message)
        : base(message)
    {
    }

    public InvalidShapeDimensionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public InvalidShapeDimensionException(string dimensionName, double invalidValue)
        : base($"Неприпустимий розмір '{dimensionName}': значення {invalidValue} має бути строго більше нуля (> 0).")
    {
        DimensionName = dimensionName;
        InvalidValue = invalidValue;
    }
}
