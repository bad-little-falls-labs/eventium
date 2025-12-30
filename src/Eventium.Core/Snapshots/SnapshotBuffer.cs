// <copyright file="SnapshotBuffer.cs" company="bad-little-falls-labs">
// Copyright © 2025 bad-little-falls-labs. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;

namespace Eventium.Core.Snapshots;

/// <summary>
/// Ring buffer for storing simulation snapshots with bounded memory.
/// Maintains snapshots keyed by simulation time, automatically overwriting oldest snapshots when full.
/// </summary>
public sealed class SnapshotBuffer
{
    private readonly ISimulationSnapshot?[] _buffer;
    private readonly Dictionary<double, int> _timeIndex = new();
    private int _count;
    private int _writeIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnapshotBuffer"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of snapshots to retain.</param>
    /// <exception cref="ArgumentException">Thrown if capacity is less than 1.</exception>
    public SnapshotBuffer(int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentException("Capacity must be at least 1.", nameof(capacity));
        }

        _buffer = new ISimulationSnapshot?[capacity];
    }

    /// <summary>
    /// Gets the maximum capacity of the buffer.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Gets the number of snapshots currently in the buffer.
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Adds a snapshot to the buffer. If the buffer is full, overwrites the oldest snapshot.
    /// </summary>
    /// <param name="snapshot">The snapshot to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if snapshot is null.</exception>
    public void Add(ISimulationSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        // Remove old index entry if overwriting
        var oldSnapshot = _buffer[_writeIndex];
        if (oldSnapshot is not null)
        {
            _timeIndex.Remove(oldSnapshot.Time);
        }

        // Add new snapshot
        _buffer[_writeIndex] = snapshot;
        _timeIndex[snapshot.Time] = _writeIndex;

        _writeIndex = (_writeIndex + 1) % _buffer.Length;
        if (_count < _buffer.Length)
        {
            _count++;
        }
    }

    /// <summary>
    /// Clears all snapshots from the buffer.
    /// </summary>
    public void Clear()
    {
        Array.Clear(_buffer, 0, _buffer.Length);
        _timeIndex.Clear();
        _writeIndex = 0;
        _count = 0;
    }

    /// <summary>
    /// Gets all snapshots currently in the buffer in chronological order.
    /// </summary>
    /// <returns>A list of snapshots ordered by simulation time.</returns>
    public IReadOnlyList<ISimulationSnapshot> GetAll()
    {
        var result = new List<ISimulationSnapshot>(_count);

        for (int i = 0; i < _buffer.Length; i++)
        {
            if (_buffer[i] is not null)
            {
                result.Add(_buffer[i]!);
            }
        }

        result.Sort((a, b) => a.Time.CompareTo(b.Time));
        return result;
    }

    /// <summary>
    /// Attempts to retrieve a snapshot by simulation time.
    /// </summary>
    /// <param name="time">The simulation time to search for.</param>
    /// <param name="snapshot">The snapshot at the requested time, or null if not found.</param>
    /// <returns>True if a snapshot at the exact time was found; false otherwise.</returns>
    public bool TryGetByTime(double time, out ISimulationSnapshot? snapshot)
    {
        snapshot = null;

        if (_timeIndex.TryGetValue(time, out var index))
        {
            snapshot = _buffer[index];
            return snapshot is not null;
        }

        return false;
    }
}
