﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Game.Graphics;
using osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.CaretPosition;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.States;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.Components.Lyrics.Carets;

public partial class DrawableTypingCaret : DrawableCaret<TypingCaretPosition>
{
    private const float fading_time = 200;
    private const float caret_move_time = 60;
    private const float caret_width = 3;

    private readonly Box drawableCaret;
    private readonly TypingCaretEventHandler? typingCaretEventHandler;

    public DrawableTypingCaret(DrawableCaretType type)
        : base(type)
    {
        drawableCaret = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Color4.White,
            Alpha = GetAlpha(Type)
        };
        AddInternal(drawableCaret);

        if (Type != DrawableCaretType.Caret)
            return;

        var inputCaretTextBox = new InputCaretTextBox
        {
            Anchor = Anchor.TopRight,
            Origin = Anchor.TopLeft,
            Width = 50,
            Height = 20,
            ReleaseFocusOnCommit = false,
        };
        typingCaretEventHandler = new TypingCaretEventHandler(inputCaretTextBox);

        AddInternal(inputCaretTextBox);
        AddInternal(typingCaretEventHandler);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        drawableCaret
            .Loop(c => c.FadeTo(0.7f).FadeTo(0.4f, 500, Easing.InOutSine));
    }

    public override void Show() => this.FadeIn(fading_time);

    public override void Hide() => this.FadeOut(fading_time);

    protected override void ApplyCaretPosition(TypingCaretPosition caret)
    {
        typingCaretEventHandler?.ChangeLyric(caret.Lyric);
        typingCaretEventHandler?.ChangeCharGap(caret.CharGap);
        typingCaretEventHandler?.FocusInputCaretTextBox();

        var rect = LyricPositionProvider.GetRectByCharIndicator(caret.CharGap);
        var position = rect.TopLeft;

        this.MoveTo(new Vector2(position.X - caret_width / 2, position.Y), caret_move_time, Easing.Out);
        this.ResizeWidthTo(caret_width, caret_move_time, Easing.Out);
        Height = rect.Height;
    }

    protected override void TriggerDisallowEditEffect(OsuColour colour)
    {
        this.FlashColour(colour.Red, 200);
    }

    private partial class TypingCaretEventHandler : Component
    {
        private readonly InputCaretTextBox inputCaretTextBox;

        public TypingCaretEventHandler(InputCaretTextBox inputCaretTextBox)
        {
            this.inputCaretTextBox = inputCaretTextBox;
        }

        [BackgroundDependencyLoader]
        private void load(ILyricTextChangeHandler lyricTextChangeHandler, ILyricCaretState lyricCaretState, IEditableLyricState editableLyricState)
        {
            inputCaretTextBox.NewCommitText = text =>
            {
                if (lyricTextChangeHandler.IsSelectionsLocked())
                {
                    editableLyricState.TriggerDisallowEditEffect();
                    return;
                }

                lyricTextChangeHandler.InsertText(charGap ?? throw new ArgumentNullException(nameof(charGap)), text);

                moveCaret(text.Length);
            };
            inputCaretTextBox.DeleteText = () =>
            {
                if (lyricTextChangeHandler.IsSelectionsLocked())
                {
                    editableLyricState.TriggerDisallowEditEffect();
                    return;
                }

                if (charGap == 0)
                    return;

                lyricTextChangeHandler.DeleteLyricText(charGap ?? throw new ArgumentNullException(nameof(charGap)));

                moveCaret(-1);
            };

            void moveCaret(int offset)
            {
                // calculate new caret position.
                int index = (charGap ?? throw new ArgumentNullException(nameof(charGap))) + offset;
                lyricCaretState.MoveCaretToTargetPosition(lyric ?? throw new ArgumentNullException(nameof(lyric)), index);
            }
        }

        private Lyric? lyric;

        public void ChangeLyric(Lyric lyric)
        {
            this.lyric = lyric;
        }

        private int? charGap;

        public void ChangeCharGap(int gap)
        {
            charGap = gap;
        }

        public void FocusInputCaretTextBox()
        {
            // Should wait for a while after click event finished in the outside.
            Schedule(() =>
            {
                inputCaretTextBox.Text = string.Empty;
                GetContainingInputManager().ChangeFocus(inputCaretTextBox);
            });
        }
    }

    private partial class InputCaretTextBox : BasicTextBox
    {
        public Action<string>? NewCommitText;

        public Action? DeleteText;

        // should not accept tab event because all focus/unfocus should be controlled by caret.
        public override bool CanBeTabbedTo => false;

        // should not allow cursor index change because press left/right event is handled by parent caret.
        protected override bool AllowWordNavigation => false;

        public InputCaretTextBox()
        {
            OnCommit += (sender, newText) =>
            {
                if (!newText)
                    return;

                string text = sender.Text;

                if (string.IsNullOrEmpty(text))
                    return;

                NewCommitText?.Invoke(text);

                sender.Text = string.Empty;
            };
        }

        public override bool OnPressed(KeyBindingPressEvent<PlatformAction> e)
        {
            bool triggerDeleteText = processTriggerDeleteText(e.Action);
            bool triggerMoveTextCaretIndex = processTriggerMoveText(e.Action);

            // should trigger delete the main text in lyric if there's not pending text.
            if (triggerDeleteText && string.IsNullOrEmpty(Text))
            {
                DeleteText?.Invoke();
                return true;
            }

            // should not block the move left/right event if there's on text in the text box.
            if (triggerMoveTextCaretIndex && string.IsNullOrEmpty(Text))
            {
                return false;
            }

            return base.OnPressed(e);

            static bool processTriggerDeleteText(PlatformAction action) =>
                action switch
                {
                    // Deletion
                    PlatformAction.DeleteBackwardChar => true,
                    _ => false
                };

            static bool processTriggerMoveText(PlatformAction action) =>
                action switch
                {
                    // Move left/right actions.
                    PlatformAction.MoveBackwardChar => true,
                    PlatformAction.MoveForwardChar => true,
                    PlatformAction.MoveBackwardWord => true,
                    PlatformAction.MoveForwardWord => true,
                    PlatformAction.MoveBackwardLine => true,
                    PlatformAction.MoveForwardLine => true,
                    _ => false
                };
        }
    }
}
