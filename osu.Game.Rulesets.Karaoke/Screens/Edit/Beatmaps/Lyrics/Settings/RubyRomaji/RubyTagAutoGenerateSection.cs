﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.RubyRomaji;

public partial class RubyTagAutoGenerateSection : TextTagAutoGenerateSection
{
    protected override AutoGenerateSubsection CreateAutoGenerateSubsection()
        => new RubyTagAutoGenerateSubsection();

    private partial class RubyTagAutoGenerateSubsection : TextTagAutoGenerateSubsection<ILyricRubyTagsChangeHandler>
    {
        protected override ConfigButton CreateConfigButton()
            => new RubyTagAutoGenerateConfigButton();

        protected partial class RubyTagAutoGenerateConfigButton : MultiConfigButton
        {
            protected override IEnumerable<KaraokeRulesetEditGeneratorSetting> AvailableSettings => new[]
            {
                KaraokeRulesetEditGeneratorSetting.JaRubyTagGeneratorConfig,
            };

            protected override string GetDisplayName(KaraokeRulesetEditGeneratorSetting setting) =>
                setting switch
                {
                    KaraokeRulesetEditGeneratorSetting.JaRubyTagGeneratorConfig => "Japanese",
                    _ => throw new ArgumentOutOfRangeException(nameof(setting))
                };
        }
    }
}
