﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Karaoke.Edit.Components.Cursor;
using osu.Game.Rulesets.Karaoke.Edit.Components.Sprites;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Timeline;
using osu.Game.Rulesets.Karaoke.Utils;
using osu.Game.Screens.Edit;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Compose.BottomEditor.AdjustTimeTags;

public partial class AdjustTimeTagSelectionBlueprint : EditableTimelineSelectionBlueprint<TimeTag>, IHasCustomTooltip<TimeTag>
{
    private const float time_tag_triangle_size = 10;

    [UsedImplicitly]
    private readonly Bindable<double?> startTime;

    private readonly TimeTagPiece timeTagPiece;
    private readonly TimeTagWithNoTimePiece timeTagWithNoTimePiece;
    private readonly OsuSpriteText timeTagText;

    public AdjustTimeTagSelectionBlueprint(TimeTag item)
        : base(item)
    {
        startTime = item.TimeBindable.GetBoundCopy();
        RelativeSizeAxes = Axes.None;
        AutoSizeAxes = Axes.X;

        // todo: not really sure why it fix the issue. should have more checks about this.
        Height = AdjustTimeTagScrollContainer.TIMELINE_HEIGHT - 1;

        AddRangeInternal(new Drawable[]
        {
            timeTagPiece = new TimeTagPiece(item)
            {
                Anchor = Anchor.CentreLeft,
                Origin = TextIndexUtils.GetValueByState(item.Index, Anchor.CentreLeft, Anchor.CentreRight)
            },
            timeTagWithNoTimePiece = new TimeTagWithNoTimePiece(item)
            {
                Anchor = Anchor.BottomLeft,
                Origin = TextIndexUtils.GetValueByState(item.Index, Anchor.BottomLeft, Anchor.BottomRight)
            },
            timeTagText = new OsuSpriteText
            {
                Text = "Text",
                Anchor = Anchor.BottomLeft,
                Origin = TextIndexUtils.GetValueByState(item.Index, Anchor.TopLeft, Anchor.TopRight),
                Y = 10,
            }
        });
    }

    [BackgroundDependencyLoader]
    private void load(EditorClock clock, AdjustTimeTagScrollContainer timeline, ILyricCaretState lyricCaretState, OsuColour colours)
    {
        // todo : should be able to let user able to select show from ruby or main text.
        timeTagText.Text = LyricUtils.GetTimeTagDisplayRubyText(lyricCaretState.BindableFocusedLyric.Value!, Item);

        timeTagPiece.Clock = clock;
        timeTagPiece.Colour = colours.BlueLight;

        timeTagWithNoTimePiece.Colour = colours.Red;
        startTime.BindValueChanged(_ =>
        {
            bool hasValue = hasTime();

            // update show time-tag style.
            switch (hasValue)
            {
                case true:
                    timeTagPiece.Show();
                    timeTagWithNoTimePiece.Hide();
                    break;

                case false:
                    timeTagPiece.Hide();
                    timeTagWithNoTimePiece.Show();
                    break;
            }

            // should wait until all time-tag time has been modified.
            Schedule(() =>
            {
                double previewTime = timeline.GetPreviewTime(Item);

                // adjust position.
                X = (float)previewTime;

                // make tickle effect.
                timeTagPiece.ClearTransforms();

                using (timeTagPiece.BeginAbsoluteSequence(previewTime))
                {
                    timeTagPiece.Colour = colours.BlueLight;
                    timeTagPiece.FlashColour(colours.PurpleDark, 750, Easing.OutQuint);
                }
            });
        }, true);
    }

    protected override Drawable GetInteractDrawable() => hasTime() ? timeTagPiece : timeTagWithNoTimePiece;

    public ITooltip<TimeTag> GetCustomTooltip() => new TimeTagTooltip();

    public TimeTag TooltipContent => Item;

    private bool hasTime() => startTime.Value.HasValue;

    public partial class TimeTagPiece : CompositeDrawable
    {
        public TimeTagPiece(TimeTag timeTag)
        {
            RelativeSizeAxes = Axes.Y;
            Width = time_tag_triangle_size;

            var textIndex = timeTag.Index;
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 1.5f,
                    Anchor = TextIndexUtils.GetValueByState(textIndex, Anchor.CentreLeft, Anchor.CentreRight),
                    Origin = TextIndexUtils.GetValueByState(textIndex, Anchor.CentreLeft, Anchor.CentreRight),
                },
                new DrawableTextIndex
                {
                    Size = new Vector2(time_tag_triangle_size),
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    State = textIndex.State
                }
            };
        }

        public override bool RemoveCompletedTransforms => false;
    }

    public partial class TimeTagWithNoTimePiece : CompositeDrawable
    {
        public TimeTagWithNoTimePiece(TimeTag timeTag)
        {
            AutoSizeAxes = Axes.Y;
            Width = time_tag_triangle_size;

            var state = timeTag.Index.State;
            InternalChildren = new Drawable[]
            {
                new DrawableTextIndex
                {
                    Size = new Vector2(time_tag_triangle_size),
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    State = state
                }
            };
        }
    }
}
