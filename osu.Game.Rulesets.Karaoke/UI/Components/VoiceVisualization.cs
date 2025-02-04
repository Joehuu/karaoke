﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Caching;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Lines;
using osu.Framework.Graphics.Performance;
using osu.Framework.Layout;
using osu.Framework.Threading;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.UI.Components;

public abstract partial class VoiceVisualization<T> : LifetimeManagementContainer
{
    private const float safe_lifetime_end_multiplier = 1;

    private readonly IBindable<double> timeRange = new BindableDouble();
    private readonly IBindable<ScrollingDirection> direction = new Bindable<ScrollingDirection>();

    [Resolved]
    private IScrollingInfo scrollingInfo { get; set; } = null!;

    private readonly LayoutValue initialStateCache = new(Invalidation.RequiredParentSizeToFit | Invalidation.DrawInfo);

    private readonly IDictionary<ScoringPath, IList<T>> frames = new Dictionary<ScoringPath, IList<T>>();
    private readonly IDictionary<ScoringPath, Cached> pathInitialStateCache = new Dictionary<ScoringPath, Cached>();

    protected IEnumerable<ScoringPath> Paths => InternalChildren.OfType<ScoringPath>();
    protected IEnumerable<ScoringPath> AlivePaths => AliveInternalChildren.OfType<ScoringPath>();

    protected virtual float PathRadius => 2;

    protected VoiceVisualization()
    {
        AddLayout(initialStateCache);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        direction.BindTo(scrollingInfo.Direction);
        timeRange.BindTo(scrollingInfo.TimeRange);

        direction.ValueChanged += _ => initialStateCache.Invalidate();
        timeRange.ValueChanged += _ => initialStateCache.Invalidate();
    }

    protected abstract double GetTime(T frame);

    protected abstract float GetPosition(T frame);

    protected void CreateNew(T point)
    {
        var path = new ScoringPath
        {
            PathRadius = PathRadius,
        };
        frames.Add(path, new List<T> { point });
        pathInitialStateCache.Add(path, new Cached());

        AddInternal(path);
    }

    protected void Append(T point)
    {
        frames.LastOrDefault().Value.Add(point);
    }

    public void Clear()
    {
        frames.Clear();
        pathInitialStateCache.Clear();
        ClearInternal();
    }

    private float scrollLength;

    protected override void Update()
    {
        base.Update();

        scrollLength = DrawSize.X;

        // If change the speed or direction, mark all the cache is invalid and re-calculate life time
        if (!initialStateCache.IsValid)
        {
            // Reset scroll info
            scrollingInfo.Algorithm.Reset();

            foreach (var cached in pathInitialStateCache.Values)
                cached.Invalidate();

            foreach (var path in Paths)
                computeLifetime(path);

            // Mark all the state is valid
            initialStateCache.Validate();
        }

        // Re-calculate alive path
        AlivePaths.ForEach(computePath);
    }

    protected void MarkAsInvalid(ScoringPath path) => pathInitialStateCache[path].Invalidate();

    protected void Invalid() => initialStateCache.Invalidate();

    private void computeLifetime(ScoringPath path)
    {
        var firstFrameInPath = frames[path].FirstOrDefault();
        var lastFrameInPath = frames[path].LastOrDefault();

        if (firstFrameInPath == null || lastFrameInPath == null)
            return;

        double startTime = GetTime(firstFrameInPath);
        double endTime = GetTime(lastFrameInPath);

        float originAdjustment = direction.Value switch
        {
            ScrollingDirection.Left => path.OriginPosition.X,
            ScrollingDirection.Right => path.DrawWidth - path.OriginPosition.X,
            _ => 0.0f
        };

        path.LifetimeStart = scrollingInfo.Algorithm.GetDisplayStartTime(startTime, originAdjustment, timeRange.Value, scrollLength);
        path.LifetimeEnd = scrollingInfo.Algorithm.TimeAt(scrollLength * safe_lifetime_end_multiplier, endTime, timeRange.Value, scrollLength);
    }

    // Cant use AddOnce() since the delegate is re-constructed every invocation
    private void computePath(ScoringPath path) => path.Schedule(() =>
    {
        var firstFrameInPath = frames[path].FirstOrDefault();
        if (firstFrameInPath == null)
            return;

        double startTime = GetTime(firstFrameInPath);
        if (pathInitialStateCache[path].IsValid)
            return;

        pathInitialStateCache[path].Validate();

        // Calculate path
        var frameList = frames[path];
        if (frameList.Count <= 1)
            return;

        path.ClearVertices();

        bool left = direction.Value == ScrollingDirection.Left;
        path.Anchor = path.Origin = left ? Anchor.TopLeft : Anchor.TopRight;

        foreach (var frame in frameList)
        {
            float x = scrollingInfo.Algorithm.GetLength(startTime, GetTime(frame), timeRange.Value, scrollLength);
            path.AddVertex(new Vector2(left ? x : -x, GetPosition(frame)));
        }
    });

    protected override void UpdateAfterChildrenLife()
    {
        base.UpdateAfterChildrenLife();

        // We need to calculate hit object positions as soon as possible after lifetimes so that hitobjects get the final say in their positions
        foreach (var path in AlivePaths)
            updatePosition(path, Time.Current);
    }

    protected override void OnChildLifetimeBoundaryCrossed(LifetimeBoundaryCrossedEvent e)
    {
        // Recalculate path if appear
        if (e.Kind == LifetimeBoundaryKind.Start && e.Child is ScoringPath path)
            computePath(path);

        base.OnChildLifetimeBoundaryCrossed(e);
    }

    protected virtual float Offset => 0;

    private void updatePosition(ScoringPath path, double currentTime)
    {
        var firstFrameInPath = frames[path].FirstOrDefault();
        if (firstFrameInPath == null)
            return;

        double startTime = GetTime(firstFrameInPath);
        int multiple = direction.Value == ScrollingDirection.Left ? 1 : -1;
        float x = scrollingInfo.Algorithm.PositionAt(startTime, currentTime, timeRange.Value, scrollLength);
        path.X = (x + Offset) * multiple;
    }

    protected partial class ScoringPath : Path
    {
        public override bool RemoveWhenNotAlive => false;

        /// <summary>
        /// Schedules an <see cref="Action"/> to this <see cref="ScoringPath"/>.
        /// todo : might move this?
        /// </summary>
        protected internal new ScheduledDelegate Schedule(Action action) => base.Schedule(action);
    }
}
