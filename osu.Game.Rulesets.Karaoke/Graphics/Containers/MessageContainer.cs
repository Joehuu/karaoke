﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osuTK;

namespace osu.Game.Rulesets.Karaoke.Graphics.Containers;

public partial class MessageContainer : OsuTextFlowContainer
{
    [Resolved]
    private OsuColour colours { get; set; }

    public MessageContainer(Action<SpriteText> defaultCreationParameters = null)
        : base(defaultCreationParameters)
    {
    }

    public void AddSuccessParagraph(string text, Action<SpriteText> creationParameters = null)
    {
        NewParagraph();
        AddIcon(FontAwesome.Solid.Check, icon =>
        {
            icon.Colour = colours.Green;
        });
        AddText($" {text}", creationParameters);
    }

    public void AddWarningParagraph(string text, Action<SpriteText> creationParameters = null)
    {
        NewParagraph();
        AddIcon(FontAwesome.Solid.ExclamationTriangle, icon =>
        {
            icon.Colour = colours.Yellow;
            icon.Scale = new Vector2(0.9f);
        });
        AddText($" {text}", creationParameters);
    }

    public void AddAlertParagraph(string text, Action<SpriteText> creationParameters = null)
    {
        NewParagraph();
        AddIcon(FontAwesome.Solid.TimesCircle, icon =>
        {
            icon.Colour = colours.Red;
        });
        AddText($" {text}", creationParameters);
    }

    public void AddHighlightText(string text, Action<SpriteText> creationParameters = null)
    {
        AddText($" {text}", c =>
        {
            c.Colour = colours.Yellow;
            creationParameters?.Invoke(c);
        });
    }
}
