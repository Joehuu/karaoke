﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings
{
    public abstract partial class LyricEditorSettings : EditorSettings
    {
        public abstract SettingsDirection Direction { get; }

        public abstract float SettingsWidth { get; }

        protected void ReloadSections()
        {
            this.ChildrenOfType<FillFlowContainer>().First().Children = CreateSections();
        }

        [BackgroundDependencyLoader]
        private void load(ILyricEditorState state, LyricEditorColourProvider colourProvider)
        {
            // change the background colour to the lighter one.
            ChangeBackgroundColour(colourProvider.Background3(state.Mode));
        }
    }
}
