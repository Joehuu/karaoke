﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Graphics.UserInterface;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Karaoke.Edit.Lyrics;

namespace osu.Game.Rulesets.Karaoke.Edit.Components.Menu
{
    public class LyricEditorEditModeMenu : MenuItem
    {
        public LyricEditorEditModeMenu()
           : base("Lyric editor mode")
        {
            Items = createMenuItems();
        }

        private ToggleMenuItem[] createMenuItems()
        {
            var enums = (Mode[])Enum.GetValues(typeof(Mode));
            return enums.Select(e =>
            {
                var item = new ToggleMenuItem(getName(e), MenuItemType.Standard, _ => updateMode(e));
                return item;
            }).ToArray();
        }

        private string getName(Mode mode)
        {
            switch (mode)
            {
                case Mode.ViewMode:
                    return "View";
                case Mode.EditMode:
                    return "Edit";
                case Mode.TypingMode:
                    return "Typing";
                case Mode.RecordMode:
                    return "Record";
                case Mode.TimeTagEditMode:
                    return "Edit time tag";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private void updateMode(Mode mode)
        {
            // todo : implementation
        }
    }
}
