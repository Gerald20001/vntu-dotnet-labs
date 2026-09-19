using System;

namespace GenericsDelegatesEvents.Exceptions;

/// <summary>
/// Виняток, що викидається при спробі отримати або вилучити елемент із порожнього стека.
/// </summary>
public class EmptyStackException : Exception
{
    public EmptyStackException()
        : base("Помилка стека: операція неможлива, оскільки стек порожній.")
    {
    }

    public EmptyStackException(string message)
        : base(message)
    {
    }

    public EmptyStackException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
