﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Graphics.UserInterfaceV2;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Beatmaps.Metadatas.Types;
using osu.Game.Rulesets.Karaoke.Edit.Components.Containers;
using osu.Game.Screens.Edit;

namespace osu.Game.Rulesets.Karaoke.Edit.Lyrics.Extends.Singers
{
    public class SingerEditSection : Section
    {
        private readonly Bindable<int[]> singerIndexes = new Bindable<int[]>();
        protected override string Title => "Singer";

        public SingerEditSection()
        {
            singerIndexes.BindValueChanged(e =>
            {
                foreach (var singerLabel in Content.OfType<LabelledSingerSwitchButton>())
                {
                    // should mark singer as selected/unselected.
                    var singerId = singerLabel.Singer.ID;
                    var selected = singerIndexes.Value?.Contains(singerId) ?? false;

                    // update singer label selection.
                    singerLabel.Current.Value = selected;
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(EditorBeatmap beatmap, ILyricEditorState state)
        {
            // update singer
            if (beatmap?.PlayableBeatmap is KaraokeBeatmap karaokeBeatmap)
            {
                var singers = karaokeBeatmap.Singers;
                Content.AddRange(singers.Select(x =>
                {
                    var singerName = x.Name;
                    var description = x.Description;
                    return new LabelledSingerSwitchButton(x)
                    {
                        Label = singerName,
                        Description = description,
                    };
                }));
            }

            // update lyric.
            state.BindableCaretPosition.BindValueChanged(e =>
            {
                e.OldValue?.Lyric?.SingersBindable.UnbindFrom(singerIndexes);
                e.NewValue?.Lyric?.SingersBindable.BindTo(singerIndexes);
            });
        }

        public class LabelledSingerSwitchButton : LabelledSwitchButton
        {
            public ISinger Singer { get; }

            public LabelledSingerSwitchButton(ISinger singer)
            {
                Singer = singer;
            }
        }
    }
}
