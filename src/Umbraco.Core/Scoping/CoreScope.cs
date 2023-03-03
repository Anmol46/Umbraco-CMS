﻿using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.IO;

namespace Umbraco.Cms.Core.Scoping;

public class CoreScope : ICoreScope
{
    private ICompletable? _scopedFileSystem;
    private IScopedNotificationPublisher? _notificationPublisher;
    private IsolatedCaches? _isolatedCaches;

    private readonly RepositoryCacheMode _repositoryCacheMode;
    private readonly bool? _shouldScopeFileSystems;
    private readonly IEventAggregator _eventAggregator;

    private bool? _completed;
    private bool _disposed;

    public CoreScope(
        IDistributedLockingMechanismFactory distributedLockingMechanismFactory,
        FileSystems scopedFileSystem,
        IEventAggregator eventAggregator,
        RepositoryCacheMode repositoryCacheMode = RepositoryCacheMode.Unspecified,
        bool? shouldScopeFileSystems = null)
    {
        _eventAggregator = eventAggregator;
        InstanceId = Guid.NewGuid();
        CreatedThreadId = Environment.CurrentManagedThreadId;
        Locks = ParentScope is null
            ? new LockingMechanism(distributedLockingMechanismFactory)
            : ResolveLockingMechanism();
        _repositoryCacheMode = repositoryCacheMode;
        _shouldScopeFileSystems = shouldScopeFileSystems;
        if (_shouldScopeFileSystems is true)
        {
            _scopedFileSystem = scopedFileSystem.Shadow();
        }
    }

    private CoreScope? ParentScope { get; }

    public int Depth
    {
        get
        {
            if (ParentScope == null)
            {
                return 0;
            }

            return ParentScope.Depth + 1;
        }
    }

    public Guid InstanceId { get; }

    public int CreatedThreadId { get; }

    public ILockingMechanism Locks { get; }

    public IScopedNotificationPublisher Notifications
    {
        get
        {
            EnsureNotDisposed();
            if (ParentScope != null)
            {
                return ParentScope.Notifications;
            }

            return _notificationPublisher ??= new ScopedNotificationPublisher(_eventAggregator);
        }
    }

    public RepositoryCacheMode RepositoryCacheMode
    {
        get
        {
            if (_repositoryCacheMode != RepositoryCacheMode.Unspecified)
            {
                return _repositoryCacheMode;
            }

            return ParentScope?.RepositoryCacheMode ?? RepositoryCacheMode.Default;
        }
    }

    public IsolatedCaches IsolatedCaches
    {
        get
        {
            if (ParentScope != null)
            {
                return ParentScope.IsolatedCaches;
            }

            return _isolatedCaches ??= new IsolatedCaches(_ => new DeepCloneAppCache(new ObjectCacheAppCache()));
        }
    }

    /// <summary>
    /// Completes a scope
    /// </summary>
    /// <returns>A value indicating whether the scope is completed or not.</returns>
    public bool Complete()
    {
        if (_completed.HasValue == false)
        {
            _completed = true;
        }

        return _completed.Value;
    }

    public void ReadLock(params int[] lockIds) => Locks.ReadLock(InstanceId, lockIds);

    public void WriteLock(params int[] lockIds) => Locks.WriteLock(InstanceId, lockIds);

    public void WriteLock(TimeSpan timeout, int lockId) => Locks.ReadLock(InstanceId, lockId);

    public void ReadLock(TimeSpan timeout, int lockId) => Locks.WriteLock(InstanceId, lockId);

    public void EagerWriteLock(params int[] lockIds) => Locks.EagerWriteLock(InstanceId, lockIds);

    public void EagerWriteLock(TimeSpan timeout, int lockId) => Locks.EagerWriteLock(InstanceId, lockId);

    public void EagerReadLock(TimeSpan timeout, int lockId) => Locks.EagerReadLock(InstanceId, lockId);

    public void EagerReadLock(params int[] lockIds) => Locks.EagerReadLock(InstanceId, lockIds);

    public void Dispose()
    {
        if (ParentScope is null)
        {
            HandleScopedFileSystems();
        }
        else
        {
            ParentScope.ChildCompleted(_completed);
        }

        // Decrement the lock counters on the parent if any.
        Locks.ClearLocks(InstanceId);

        _disposed = true;
    }

    private void ChildCompleted(bool? completed)
    {
        // if child did not complete we cannot complete
        if (completed.HasValue == false || completed.Value == false)
        {
            _completed = false;
        }
    }

    private void HandleScopedFileSystems()
    {
        if (_shouldScopeFileSystems == true)
        {
            if (_completed.HasValue && _completed.Value)
            {
                _scopedFileSystem?.Complete();
            }

            _scopedFileSystem?.Dispose();
            _scopedFileSystem = null;
        }
    }

    private void EnsureNotDisposed()
    {
        // We can't be disposed
        if (_disposed)
        {
            throw new ObjectDisposedException($"The {nameof(CoreScope)} with ID ({InstanceId}) is already disposed");
        }

        // And neither can our ancestors if we're trying to be disposed since
        // a child must always be disposed before it's parent.
        // This is a safety check, it's actually not entirely possible that a parent can be
        // disposed before the child since that will end up with a "not the Ambient" exception.
        ParentScope?.EnsureNotDisposed();
    }

    private ILockingMechanism ResolveLockingMechanism() =>
        ParentScope is not null ? ParentScope.ResolveLockingMechanism() : Locks;
}
