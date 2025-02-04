﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Localisation;
using osu.Game.Graphics;
using osu.Game.Overlays;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States.Modes;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Components.Markdown;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.TimeTags;

public partial class TimeTagEditModeSection : LyricEditorEditModeSection<ITimeTagModeState, TimeTagEditMode>
{
    protected override OverlayColourScheme CreateColourScheme()
        => OverlayColourScheme.Orange;

    protected override Selection CreateSelection(TimeTagEditMode mode) =>
        mode switch
        {
            TimeTagEditMode.Create => new Selection(),
            TimeTagEditMode.Recording => new Selection(),
            TimeTagEditMode.Adjust => new TimeTagVerifySelection(),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    protected override LocalisableString GetSelectionText(TimeTagEditMode mode) =>
        mode switch
        {
            TimeTagEditMode.Create => "Create",
            TimeTagEditMode.Recording => "Recording",
            TimeTagEditMode.Adjust => "Adjust",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    protected override Color4 GetSelectionColour(OsuColour colours, TimeTagEditMode mode, bool active) =>
        mode switch
        {
            TimeTagEditMode.Create => active ? colours.Blue : colours.BlueDarker,
            TimeTagEditMode.Recording => active ? colours.Red : colours.RedDarker,
            TimeTagEditMode.Adjust => active ? colours.Yellow : colours.YellowDarker,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    protected override DescriptionFormat GetSelectionDescription(TimeTagEditMode mode) =>
        mode switch
        {
            TimeTagEditMode.Create => "Create the time-tag or adjust the position.",
            TimeTagEditMode.Recording => new DescriptionFormat
            {
                Text =
                    $"Press [{DescriptionFormat.LINK_KEY_ACTION}](set_time_tag_time) at the right time to set current time to time-tag. Press [{DescriptionFormat.LINK_KEY_ACTION}](clear_time_tag_time) to clear the time-tag time.",
                Actions = new Dictionary<string, IDescriptionAction>
                {
                    {
                        "set_time_tag_time", new InputKeyDescriptionAction
                        {
                            AdjustableActions = new List<KaraokeEditAction> { KaraokeEditAction.SetTime }
                        }
                    },
                    {
                        "clear_time_tag_time", new InputKeyDescriptionAction
                        {
                            AdjustableActions = new List<KaraokeEditAction> { KaraokeEditAction.ClearTime }
                        }
                    }
                }
            },
            TimeTagEditMode.Adjust => "Drag to adjust time-tag time precisely.",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

    private partial class TimeTagVerifySelection : LyricEditorVerifySelection
    {
        protected override LyricEditorMode EditMode => LyricEditorMode.EditTimeTag;
    }
}
