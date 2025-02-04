// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Karaoke.Objects.Types;

namespace osu.Game.Rulesets.Karaoke.Edit.ChangeHandlers.Lyrics;

public interface ILyricTextTagsChangeHandler<in TTextTag> : ILyricListPropertyChangeHandler<TTextTag> where TTextTag : ITextTag
{
    void SetIndex(TTextTag textTag, int? startIndex, int? endIndex);

    void ShiftingIndex(IEnumerable<TTextTag> textTags, int offset);

    void SetText(TTextTag textTag, string text);
}
