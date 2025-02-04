// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers;
using osu.Game.Rulesets.Karaoke.Edit.Export;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Components.Menus;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Pages;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Singers;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Translate;
using osu.Game.Rulesets.Karaoke.Skinning.Fonts;
using osu.Game.Screens.Edit.Components.Menus;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps;

public partial class KaraokeBeatmapEditor : GenericEditor<KaraokeBeatmapEditorScreenMode>
{
    [Cached]
    private readonly OverlayColourProvider colourProvider = new(OverlayColourScheme.Blue);

    [Cached]
    private readonly KaraokeRulesetEditConfigManager editConfigManager;

    [Cached]
    private readonly KaraokeRulesetLyricEditorConfigManager lyricEditorConfigManager;

    [Cached]
    private readonly KaraokeRulesetEditGeneratorConfigManager generatorConfigManager;

    [Cached]
    private readonly KaraokeRulesetEditCheckerConfigManager checkerConfigManager;

    [Cached]
    private readonly FontManager fontManager;

    [Cached(typeof(IKaraokeBeatmapResourcesProvider))]
    private KaraokeBeatmapResourcesProvider karaokeBeatmapResourcesProvider;

    [Cached(typeof(ILyricsProvider))]
    private readonly LyricsProvider lyricsProvider;

    [Cached]
    private readonly ExportLyricManager exportLyricManager;

    [Cached(typeof(IImportBeatmapChangeHandler))]
    private readonly ImportBeatmapChangeHandler importBeatmapChangeHandler;

    [Cached]
    private readonly Bindable<LyricEditorMode> bindableLyricEditorMode = new();

    public KaraokeBeatmapEditor()
    {
        editConfigManager = new KaraokeRulesetEditConfigManager();
        lyricEditorConfigManager = new KaraokeRulesetLyricEditorConfigManager();
        generatorConfigManager = new KaraokeRulesetEditGeneratorConfigManager();
        checkerConfigManager = new KaraokeRulesetEditCheckerConfigManager();

        // Duplicated registration because selection handler need to use it.
        AddInternal(fontManager = new FontManager());
        AddInternal(karaokeBeatmapResourcesProvider = new KaraokeBeatmapResourcesProvider());

        AddInternal(exportLyricManager = new ExportLyricManager());
        AddInternal(lyricsProvider = new LyricsProvider());

        AddInternal(importBeatmapChangeHandler = new ImportBeatmapChangeHandler());
    }

    protected override GenericEditorScreen<KaraokeBeatmapEditorScreenMode> GenerateScreen(KaraokeBeatmapEditorScreenMode screenMode) =>
        screenMode switch
        {
            KaraokeBeatmapEditorScreenMode.Lyric => new LyricEditorScreen(),
            KaraokeBeatmapEditorScreenMode.Singer => new SingerScreen(),
            KaraokeBeatmapEditorScreenMode.Translate => new TranslateScreen(),
            KaraokeBeatmapEditorScreenMode.Page => new PageScreen(),
            _ => throw new InvalidOperationException("Editor menu bar switched to an unsupported mode")
        };

    protected override MenuItem[] GenerateMenuItems(KaraokeBeatmapEditorScreenMode screenMode)
    {
        return screenMode switch
        {
            KaraokeBeatmapEditorScreenMode.Lyric => new MenuItem[]
            {
                new("File")
                {
                    Items = new MenuItem[]
                    {
                        new ImportLyricMenu(this, "Import from text", importBeatmapChangeHandler),
                        new ImportLyricMenu(this, "Import from .lrc file", importBeatmapChangeHandler),
                        new EditorMenuItemSpacer(),
                        new EditorMenuItem("Export to .lrc", MenuItemType.Standard, () => exportLyricManager.ExportToLrc()),
                        new EditorMenuItem("Export to text", MenuItemType.Standard, () => exportLyricManager.ExportToText()),
                        new EditorMenuItem("Export to json", MenuItemType.Destructive, () => exportLyricManager.ExportToJson()),
                        new EditorMenuItem("Export to json beatmap", MenuItemType.Destructive, () => exportLyricManager.ExportToJsonBeatmap()),
                    }
                },
                new LyricEditorModeMenu(bindableLyricEditorMode, "Mode"),
                new("View")
                {
                    Items = new MenuItem[]
                    {
                        new LyricEditorPreferLayoutMenu(lyricEditorConfigManager, "Layout"),
                        new LyricEditorTextSizeMenu(lyricEditorConfigManager, "Text size"),
                        new AutoFocusToEditLyricMenu(lyricEditorConfigManager, "Auto focus to edit lyric"),
                    }
                },
                new("Config")
                {
                    Items = new MenuItem[] { new EditorMenuItem("Lyric editor"), new GeneratorConfigMenu("Auto-generator"), new LockStateMenu(lyricEditorConfigManager, "Lock") }
                },
            },
            _ => Array.Empty<MenuItem>()
        };
    }
}
