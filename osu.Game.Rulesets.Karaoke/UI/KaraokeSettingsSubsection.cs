﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Screens;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Karaoke.Configuration;
using osu.Game.Rulesets.Karaoke.Extensions;
using osu.Game.Rulesets.Karaoke.Localisation;
using osu.Game.Rulesets.Karaoke.Overlays;
using osu.Game.Rulesets.Karaoke.Screens.Settings;
using osu.Game.Screens;

namespace osu.Game.Rulesets.Karaoke.UI;

public partial class KaraokeSettingsSubsection : RulesetSettingsSubsection
{
    protected override LocalisableString Header => CommonStrings.RulesetName;

    public KaraokeSettingsSubsection(Ruleset ruleset)
        : base(ruleset)
    {
    }

    [Resolved]
    protected OsuGame Game { get; private set; } = null!;

    private KaraokeChangelogOverlay? changelogOverlay;

    [BackgroundDependencyLoader]
    private void load(IPerformFromScreenRunner performer)
    {
        var config = (KaraokeRulesetConfigManager)Config;

        Children = new Drawable[]
        {
            // Scrolling
            new SettingsEnumDropdown<KaraokeScrollingDirection>
            {
                ClassicDefault = KaraokeScrollingDirection.Left,
                LabelText = KaraokeSettingsSubsectionStrings.ScrollingDirection,
                TooltipText = KaraokeSettingsSubsectionStrings.ScrollingDirectionTooltip,
                Current = config.GetBindable<KaraokeScrollingDirection>(KaraokeRulesetSetting.ScrollDirection)
            },
            new SettingsSlider<double, TimeSlider>
            {
                LabelText = KaraokeSettingsSubsectionStrings.ScrollSpeed,
                Current = config.GetBindable<double>(KaraokeRulesetSetting.ScrollTime)
            },
            // Gameplay
            new SettingsCheckbox
            {
                LabelText = KaraokeSettingsSubsectionStrings.ShowCursorWhilePlaying,
                TooltipText = KaraokeSettingsSubsectionStrings.ShowCursorWhilePlayingTooltip,
                Current = config.GetBindable<bool>(KaraokeRulesetSetting.ShowCursor)
            },
            // Translate
            new SettingsCheckbox
            {
                LabelText = KaraokeSettingsSubsectionStrings.Translate,
                TooltipText = KaraokeSettingsSubsectionStrings.TranslateTooltip,
                Current = config.GetBindable<bool>(KaraokeRulesetSetting.UseTranslate)
            },
            // Device
            new SettingsMicrophoneDeviceDropdown
            {
                ClassicDefault = string.Empty,
                LabelText = KaraokeSettingsSubsectionStrings.MicrophoneDevice,
                Current = config.GetBindable<string>(KaraokeRulesetSetting.MicrophoneDevice)
            },
            // Practice
            new DangerousSettingsButton
            {
                Text = KaraokeSettingsSubsectionStrings.OpenRulesetSettings,
                TooltipText = KaraokeSettingsSubsectionStrings.OpenRulesetSettingsTooltip,
                Action = () => performer.PerformFromScreen(menu => menu.Push(new KaraokeSettings()))
            },
            new SettingsButton
            {
                Text = KaraokeSettingsSubsectionStrings.ChangeLog,
                TooltipText = KaraokeSettingsSubsectionStrings.ChangeLogTooltip,
                Action = () =>
                {
                    try
                    {
                        var displayContainer = Game.GetChangelogPlacementContainer();
                        var settingOverlay = Game.GetSettingsOverlay();
                        if (displayContainer == null)
                            return;

                        if (changelogOverlay == null && !displayContainer.Children.OfType<KaraokeChangelogOverlay>().Any())
                            displayContainer.Add(changelogOverlay = new KaraokeChangelogOverlay("karaoke-dev"));

                        changelogOverlay?.Show();
                        settingOverlay?.Hide();
                    }
                    catch
                    {
                        // maybe this overlay has been moved into internal.
                    }
                }
            }
        };
    }

    private partial class TimeSlider : RoundedSliderBar<double>
    {
        public override LocalisableString TooltipText => Current.Value.ToString("N0") + "ms";
    }
}
