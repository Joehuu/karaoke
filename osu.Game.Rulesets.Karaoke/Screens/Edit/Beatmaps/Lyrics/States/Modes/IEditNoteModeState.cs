// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.Notes;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States.Modes;

public interface IEditNoteModeState : IHasEditModeState<NoteEditMode>, IHasSpecialAction<NoteEditModeSpecialAction>, IHasBlueprintSelection<Note>
{
    Bindable<NoteEditPropertyMode> NoteEditPropertyMode { get; }
}
