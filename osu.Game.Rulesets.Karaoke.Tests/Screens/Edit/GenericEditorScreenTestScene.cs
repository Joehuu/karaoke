// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Overlays;
using osu.Game.Rulesets.Edit;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Screens.Edit;
using osu.Game.Rulesets.Karaoke.Tests.Beatmaps;
using osu.Game.Screens.Edit;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Karaoke.Tests.Screens.Edit;

public abstract partial class GenericEditorScreenTestScene<TScreen, TType> : EditorClockTestScene
    where TScreen : GenericEditorScreen<TType>
{
    [Cached(typeof(EditorBeatmap))]
    [Cached(typeof(IBeatSnapProvider))]
    private readonly EditorBeatmap editorBeatmap;

    [Cached]
    private readonly OverlayColourProvider colourProvider = new(OverlayColourScheme.Blue);

    protected GenericEditorScreenTestScene()
    {
        editorBeatmap = new EditorBeatmap(CreateBeatmap());
    }

    protected override void LoadComplete()
    {
        editorBeatmap.BeatmapInfo.Ruleset = new KaraokeRuleset().RulesetInfo;

        Beatmap.Value = CreateWorkingBeatmap(editorBeatmap.PlayableBeatmap);

        Children = new Drawable[]
        {
            editorBeatmap,
            CreateEditorScreen().With(x =>
            {
                x.State.Value = Visibility.Visible;
            })
        };
    }

    protected abstract GenericEditorScreen<TType> CreateEditorScreen();

    protected virtual KaraokeBeatmap CreateBeatmap()
    {
        var beatmap = new TestKaraokeBeatmap(new KaraokeRuleset().RulesetInfo);
        if (new KaraokeBeatmapConverter(beatmap, new KaraokeRuleset()).Convert() is not KaraokeBeatmap karaokeBeatmap)
            throw new ArgumentNullException(nameof(karaokeBeatmap));

        return karaokeBeatmap;
    }
}
