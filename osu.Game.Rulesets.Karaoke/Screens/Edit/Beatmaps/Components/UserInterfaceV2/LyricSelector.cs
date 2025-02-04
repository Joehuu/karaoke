// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Screens.Edit;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Components.UserInterfaceV2;

public partial class LyricSelector : CompositeDrawable, IHasCurrentValue<Lyric?>
{
    private readonly LyricSelectionSearchTextBox filter;

    private readonly BindableWithCurrent<Lyric?> current = new();

    public Bindable<Lyric?> Current
    {
        get => current.Current;
        set => current.Current = value;
    }

    public override bool AcceptsFocus => true;

    public override bool RequestsFocus => true;

    private readonly RearrangeableLyricListContainer lyricList;

    public LyricSelector()
    {
        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.Absolute, 40),
                new Dimension()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    filter = new LyricSelectionSearchTextBox
                    {
                        RelativeSizeAxes = Axes.X,
                    }
                },
                new Drawable[]
                {
                    lyricList = CreateRearrangeableLyricListContainer().With(x =>
                    {
                        x.RelativeSizeAxes = Axes.Both;
                        x.RequestSelection = item =>
                        {
                            Current.Value = item;
                        };
                    })
                }
            }
        };

        filter.Current.BindValueChanged(e => lyricList.Filter(e.NewValue));
        Current.BindValueChanged(e => lyricList.SelectedSet.Value = e.NewValue);
    }

    protected virtual RearrangeableLyricListContainer CreateRearrangeableLyricListContainer() => new();

    [BackgroundDependencyLoader]
    private void load(EditorBeatmap editorBeatmap)
    {
        lyricList.Items.AddRange(editorBeatmap.HitObjects.OfType<Lyric>());
    }

    protected override void OnFocus(FocusEvent e)
    {
        base.OnFocus(e);

        GetContainingInputManager().ChangeFocus(filter);
    }

    private partial class LyricSelectionSearchTextBox : SearchTextBox
    {
        protected override Color4 SelectionColour => Color4.Gray;

        public LyricSelectionSearchTextBox()
        {
            PlaceholderText = @"type in keywords...";
        }
    }

    protected partial class RearrangeableLyricListContainer : RearrangeableTextFlowListContainer<Lyric?>
    {
        protected override DrawableTextListItem CreateDrawable(Lyric? item)
            => new DrawableLyricListItem(item);

        protected partial class DrawableLyricListItem : DrawableTextListItem
        {
            [Resolved]
            private OsuColour colours { get; set; } = null!;

            public DrawableLyricListItem(Lyric? item)
                : base(item)
            {
            }

            public override IEnumerable<LocalisableString> FilterTerms => new[]
            {
                new LocalisableString(Model?.Text ?? ""),
            };

            protected override void CreateDisplayContent(OsuTextFlowContainer textFlowContainer, Lyric? model)
            {
                if (model == null)
                {
                    textFlowContainer.AddText("<Empty>");
                }
                else
                {
                    // display the lyric order.
                    textFlowContainer.AddText($"#{model.Order}", x => x.Colour = colours.Yellow);
                    textFlowContainer.AddText("  ");

                    // main text
                    textFlowContainer.AddText(model.Text);
                    textFlowContainer.AddText(" ");
                }
            }
        }
    }
}
