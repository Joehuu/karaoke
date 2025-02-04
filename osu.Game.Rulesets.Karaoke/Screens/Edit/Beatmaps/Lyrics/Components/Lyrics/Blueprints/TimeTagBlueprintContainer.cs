﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input.Events;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States.Modes;
using osu.Game.Screens.Edit.Compose.Components;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Components.Lyrics.Blueprints;

public partial class TimeTagBlueprintContainer : BindableBlueprintContainer<TimeTag>
{
    [Resolved]
    private ILyricCaretState lyricCaretState { get; set; } = null!;

    [UsedImplicitly]
    private readonly BindableList<TimeTag> timeTags;

    protected readonly Lyric Lyric;

    public TimeTagBlueprintContainer(Lyric lyric)
    {
        Lyric = lyric;
        timeTags = lyric.TimeTagsBindable.GetBoundCopy();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        lyricCaretState.MoveCaretToTargetPosition(Lyric);
        return base.OnMouseDown(e);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        // Add time tag into blueprint container
        RegisterBindable(timeTags);
    }

    protected override SelectionHandler<TimeTag> CreateSelectionHandler()
        => new TimeTagSelectionHandler();

    protected override SelectionBlueprint<TimeTag> CreateBlueprintFor(TimeTag item)
        => new TimeTagSelectionBlueprint(item);

    protected partial class TimeTagSelectionHandler : BindableSelectionHandler
    {
        [Resolved]
        private ILyricTimeTagsChangeHandler lyricTimeTagsChangeHandler { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load(ITimeTagModeState timeTagModeState)
        {
            SelectedItems.BindTo(timeTagModeState.SelectedItems);
        }

        // for now we always allow movement. snapping is provided by the Timeline's "distance" snap implementation
        public override bool HandleMovement(MoveSelectionEvent<TimeTag> moveEvent) => true;

        protected override void DeleteItems(IEnumerable<TimeTag> items)
        {
            lyricTimeTagsChangeHandler.RemoveRange(items);
        }
    }
}
