using System;
using System.Collections.Generic;
using GenericsDelegatesEvents.Events;
using GenericsDelegatesEvents.Exceptions;

namespace GenericsDelegatesEvents.Collections;

/// <summary>
/// Власний узагальнений клас стека (LIFO) з обмеженою місткістю та подією переповнення.
/// </summary>
/// <typeparam name="T">Тип елементів стека</typeparam>
public class MyStack<T>
{
    private readonly List<T> _items;

    /// <summary>
    /// Максимальна місткість стека.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Поточна кількість елементів у стеку.
    /// </summary>
    public int Count => _items.Count;

    /// <summary>
    /// Ознака того, чи стек порожній.
    /// </summary>
    public bool IsEmpty => _items.Count == 0;

    /// <summary>
    /// Ознака того, чи стек повністю заповнений.
    /// </summary>
    public bool IsFull => _items.Count >= Capacity;

    /// <summary>
    /// Подія, що виникає при спробі додати елемент до вже заповненого стека.
    /// </summary>
    public event EventHandler<OverflowEventArgs<T>>? Overflow;

    public MyStack(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Місткість стека має бути додатною (> 0).");
        }

        Capacity = capacity;
        _items = new List<T>(capacity);
    }

    /// <summary>
    /// Додає новий елемент на вершину стека.
    /// Якщо стек заповнений, викликає подію Overflow та повертає false.
    /// </summary>
    public bool Push(T item)
    {
        if (IsFull)
        {
            OnOverflow(new OverflowEventArgs<T>(item, Capacity));
            return false;
        }

        _items.Add(item);
        return true;
    }

    /// <summary>
    /// Вилучає та повертає верхній елемент стека.
    /// Якщо стек порожній, викидає виняток EmptyStackException.
    /// </summary>
    public T Pop()
    {
        if (IsEmpty)
        {
            throw new EmptyStackException("Неможливо виконати Pop: стек порожній.");
        }

        int lastIndex = _items.Count - 1;
        T item = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        return item;
    }

    /// <summary>
    /// Повертає верхній елемент стека без його вилучення.
    /// Якщо стек порожній, викидає виняток EmptyStackException.
    /// </summary>
    public T Peek()
    {
        if (IsEmpty)
        {
            throw new EmptyStackException("Неможливо виконати Peek: стек порожній.");
        }

        return _items[^1];
    }

    /// <summary>
    /// Очищає всі елементи стека.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }

    /// <summary>
    /// Повертає елементи стека від вершини до дна у вигляді списку.
    /// </summary>
    public List<T> ToList()
    {
        var copy = new List<T>(_items);
        copy.Reverse();
        return copy;
    }

    // =========================================================================
    // Методи вищого порядку (демонстрація вбудованих делегатів Func, Action, Predicate)
    // =========================================================================

    /// <summary>
    /// Фільтрує елементи стека за допомогою делегата Predicate&lt;T&gt;.
    /// </summary>
    public List<T> FindAll(Predicate<T> match)
    {
        ArgumentNullException.ThrowIfNull(match);

        var result = new List<T>();
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (match(_items[i]))
            {
                result.Add(_items[i]);
            }
        }
        return result;
    }

    /// <summary>
    /// Підраховує кількість елементів, що задовольняють умову делегата Func&lt;T, bool&gt;.
    /// </summary>
    public int CountWhere(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        int count = 0;
        foreach (T item in _items)
        {
            if (predicate(item))
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Виконує дію Action&lt;T&gt; для кожного елемента стека.
    /// </summary>
    public void ForEach(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            action(_items[i]);
        }
    }

    /// <summary>
    /// Трансформує елементи стека за допомогою делегата Func&lt;T, TResult&gt;.
    /// </summary>
    public List<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var result = new List<TResult>(_items.Count);
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            result.Add(selector(_items[i]));
        }
        return result;
    }

    protected virtual void OnOverflow(OverflowEventArgs<T> e)
    {
        Overflow?.Invoke(this, e);
    }
}
