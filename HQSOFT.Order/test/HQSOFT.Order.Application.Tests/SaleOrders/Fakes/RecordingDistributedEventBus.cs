using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace HQSOFT.Order.SaleOrders;

public class RecordingDistributedEventBus : IDistributedEventBus
{
    public ConcurrentBag<object> PublishedEvents { get; } = [];

    public Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class
    {
        PublishedEvents.Add(eventData);
        return Task.CompletedTask;
    }

    public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete) where TEvent : class
    {
        PublishedEvents.Add(eventData);
        return Task.CompletedTask;
    }

    public Task PublishAsync<TEvent>(TEvent eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true) where TEvent : class
    {
        PublishedEvents.Add(eventData);
        return Task.CompletedTask;
    }

    public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete)
    {
        PublishedEvents.Add(eventData);
        return Task.CompletedTask;
    }

    public Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
    {
        PublishedEvents.Add(eventData);
        return Task.CompletedTask;
    }

    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
        return NullDisposable.Instance;
    }

    public IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
    {
        return NullDisposable.Instance;
    }

    public IDisposable Subscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler, new()
    {
        return NullDisposable.Instance;
    }

    public IDisposable Subscribe(Type eventType, IEventHandler handler)
    {
        return NullDisposable.Instance;
    }

    public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
    {
        return NullDisposable.Instance;
    }

    public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
    {
        return NullDisposable.Instance;
    }

    public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
    {
    }

    public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
    {
    }

    public void Unsubscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
    {
    }

    public void Unsubscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler
    {
    }

    public void Unsubscribe(Type eventType, IEventHandler handler)
    {
    }

    public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
    {
    }

    public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
    {
    }

    public void UnsubscribeAll<TEvent>() where TEvent : class
    {
    }

    public void UnsubscribeAll(Type eventType)
    {
    }

    private sealed class NullDisposable : IDisposable
    {
        public static NullDisposable Instance { get; } = new();
        public void Dispose() { }
    }
}
