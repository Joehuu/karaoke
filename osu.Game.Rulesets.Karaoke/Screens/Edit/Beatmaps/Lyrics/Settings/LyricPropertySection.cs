﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Game.Rulesets.Karaoke.Edit.Utils;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings.Components;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Settings;

public abstract partial class LyricPropertySection : EditorSection
{
    private readonly IBindable<Lyric?> bindableFocusedLyric = new Bindable<Lyric?>();
    private readonly IBindable<int> bindablePropertyWritableVersion = new Bindable<int>();

    public override bool PropagateNonPositionalInputSubTree => base.PropagateNonPositionalInputSubTree && !Disabled;
    public override bool PropagatePositionalInputSubTree => base.PropagatePositionalInputSubTree && !Disabled;

    protected bool IsRebinding { get; private set; }

    protected bool Disabled { get; private set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bindableFocusedLyric.BindValueChanged(x =>
        {
            var lyric = x.NewValue;

            IsRebinding = true;

            OnLyricChanged(lyric);

            bindablePropertyWritableVersion.UnbindBindings();

            if (lyric != null)
            {
                bindablePropertyWritableVersion.BindTo(lyric.LyricPropertyWritableVersion);
                updateDisableStatus();
            }

            IsRebinding = false;
        }, true);

        bindablePropertyWritableVersion.BindValueChanged(x =>
        {
            updateDisableStatus();
        });

        updateDisableStatus();
    }

    private void updateDisableStatus()
    {
        var lyric = bindableFocusedLyric.Value;
        var propertyLocked = lyric != null ? IsWriteLyricPropertyLocked(lyric) : null;
        Disabled = propertyLocked != null;

        UpdateDisabledState(Disabled);

        // should show the block section and make the children looks not editable if disable edit.
        Content.FadeTo(Disabled ? 0.5f : 1, 300);
        updateBlockSectionMessage(propertyLocked);
    }

    private void updateBlockSectionMessage(LockLyricPropertyBy? propertyLocked)
    {
        var blockMaskingWrapper = InternalChildren.OfType<BlockSectionWrapper>().FirstOrDefault();

        if (blockMaskingWrapper == null && propertyLocked != null)
        {
            var icon = getWriteLyricPropertyLockedIcon(propertyLocked.Value);
            var title = getWriteLyricPropertyLockedDescriptionTitle(propertyLocked.Value);
            var description = GetWriteLyricPropertyLockedDescription(propertyLocked.Value);
            var tooltip = GetWriteLyricPropertyLockedTooltip(propertyLocked.Value);

            AddInternal(new BlockSectionWrapper(icon, title, description, tooltip));
        }
        else if (blockMaskingWrapper != null && propertyLocked == null)
        {
            RemoveInternal(blockMaskingWrapper, true);
        }

        static IconUsage getWriteLyricPropertyLockedIcon(LockLyricPropertyBy lockLyricPropertyBy) =>
            lockLyricPropertyBy switch
            {
                LockLyricPropertyBy.ReferenceLyricConfig => FontAwesome.Solid.Chair,
                LockLyricPropertyBy.LockState => FontAwesome.Solid.Lock,
                _ => throw new ArgumentOutOfRangeException(nameof(lockLyricPropertyBy), lockLyricPropertyBy, null)
            };

        static LocalisableString getWriteLyricPropertyLockedDescriptionTitle(LockLyricPropertyBy lockLyricPropertyBy) =>
            lockLyricPropertyBy switch
            {
                LockLyricPropertyBy.ReferenceLyricConfig => "Sync",
                LockLyricPropertyBy.LockState => "Locked",
                _ => throw new ArgumentOutOfRangeException(nameof(lockLyricPropertyBy), lockLyricPropertyBy, null)
            };
    }

    [BackgroundDependencyLoader]
    private void load(ILyricCaretState lyricCaretState)
    {
        bindableFocusedLyric.BindTo(lyricCaretState.BindableFocusedLyric);
    }

    protected abstract void OnLyricChanged(Lyric? lyric);

    protected virtual void UpdateDisabledState(bool disabled)
    {
    }

    protected abstract LockLyricPropertyBy? IsWriteLyricPropertyLocked(Lyric lyric);

    protected abstract LocalisableString GetWriteLyricPropertyLockedDescription(LockLyricPropertyBy lockLyricPropertyBy);

    protected abstract LocalisableString GetWriteLyricPropertyLockedTooltip(LockLyricPropertyBy lockLyricPropertyBy);
}
