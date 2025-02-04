// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.CaretPosition;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.CaretPosition.Algorithms;

namespace osu.Game.Rulesets.Karaoke.Tests.Screens.Edit.Beatmap.Lyrics.CaretPosition.Algorithms;

public abstract class BaseCharIndexCaretPositionAlgorithmTest<TAlgorithm, TCaret> : BaseIndexCaretPositionAlgorithmTest<TAlgorithm, TCaret>
    where TAlgorithm : CharIndexCaretPositionAlgorithm<TCaret> where TCaret : struct, ICharIndexCaretPosition
{
    #region Lyric

    [TestCase(nameof(singleLyric), 0, 0, true)]
    [TestCase(nameof(singleLyric), 0, 3, true)]
    [TestCase(nameof(singleLyric), 0, 4, false)]
    [TestCase(nameof(singleLyric), 0, -1, false)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0, true)]
    [TestCase(nameof(singleLyricWithNoText), 0, 0, false)] // Should have at least one char in the lyric.
    [TestCase(nameof(singleLyricWithNoText), 0, 1, false)]
    public void TestPositionMovable(string sourceName, int lyricIndex, int index, bool movable)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var caret = createCaretPosition(lyrics, lyricIndex, index);

        // Check is movable
        TestPositionMovable(lyrics, caret, movable);
    }

    [TestCase(nameof(singleLyric), 0, 0, null, null)] // cannot move up if at top index.
    [TestCase(nameof(singleLyricWithOneText), 0, 0, null, null)]
    [TestCase(nameof(twoLyricsWithText), 1, 0, 0, 0)]
    [TestCase(nameof(threeLyricsWithSpacing), 2, 0, 0, 0)]
    [TestCase(nameof(threeLyricsWithSpacing), 2, 2, 0, 2)]
    public void TestMoveToPreviousLyric(string sourceName, int lyricIndex, int index, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var caret = createCaretPosition(lyrics, lyricIndex, index);
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check is movable
        TestMoveToPreviousLyric(lyrics, caret, expected);
    }

    [TestCase(nameof(singleLyric), 0, 0, null, null)] // cannot move down if at bottom index.
    [TestCase(nameof(singleLyricWithOneText), 0, 0, null, null)]
    [TestCase(nameof(twoLyricsWithText), 0, 0, 1, 0)]
    [TestCase(nameof(threeLyricsWithSpacing), 0, 0, 2, 0)]
    [TestCase(nameof(threeLyricsWithSpacing), 0, 3, 2, 2)]
    public void TestMoveToNextLyric(string sourceName, int lyricIndex, int index, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var caret = createCaretPosition(lyrics, lyricIndex, index);
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check is movable
        TestMoveToNextLyric(lyrics, caret, expected);
    }

    [TestCase(nameof(singleLyric), 0, 0)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0)]
    [TestCase(nameof(twoLyricsWithText), 0, 0)]
    [TestCase(nameof(threeLyricsWithSpacing), 0, 0)]
    public void TestMoveToFirstLyric(string sourceName, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check first position
        TestMoveToFirstLyric(lyrics, expected);
    }

    [TestCase(nameof(singleLyric), 0, 3)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0)]
    [TestCase(nameof(twoLyricsWithText), 1, 2)]
    [TestCase(nameof(threeLyricsWithSpacing), 2, 2)]
    public void TestMoveToLastLyric(string sourceName, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check last position
        TestMoveToLastLyric(lyrics, expected);
    }

    [TestCase(nameof(singleLyric), 0, 0)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0)]
    public void TestMoveToTargetLyric(string sourceName, int lyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var lyric = lyrics[lyricIndex];
        var expected = createExpectedCaretPosition(lyrics, lyricIndex, expectedIndex);

        // Check move to target position.
        TestMoveToTargetLyric(lyrics, lyric, expected);
    }

    #endregion

    #region Lyric index

    [TestCase(nameof(singleLyric), 0, 0, null, null)]
    [TestCase(nameof(singleLyric), 0, 1, 0, 0)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0, null, null)]
    public void TestMoveToPreviousIndex(string sourceName, int lyricIndex, int index, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var caret = createCaretPosition(lyrics, lyricIndex, index);
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check is movable
        TestMoveToPreviousIndex(lyrics, caret, expected);
    }

    [TestCase(nameof(singleLyric), 0, 3, null, null)]
    [TestCase(nameof(singleLyric), 0, 2, 0, 3)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0, null, null)]
    public void TestMoveToNextIndex(string sourceName, int lyricIndex, int index, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var caret = createCaretPosition(lyrics, lyricIndex, index);
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check is movable
        TestMoveToNextIndex(lyrics, caret, expected);
    }

    [TestCase(nameof(singleLyric), 0, 0, 0)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0, 0)]
    public void TestMoveToFirstIndex(string sourceName, int lyricIndex, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var lyric = lyrics[lyricIndex];
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check is movable
        TestMoveToFirstIndex(lyrics, lyric, expected);
    }

    [TestCase(nameof(singleLyric), 0, 0, 3)]
    [TestCase(nameof(singleLyricWithOneText), 0, 0, 0)]
    public void TestMoveToLastIndex(string sourceName, int lyricIndex, int? expectedLyricIndex, int? expectedIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var lyric = lyrics[lyricIndex];
        var expected = createExpectedCaretPosition(lyrics, expectedLyricIndex, expectedIndex);

        // Check is movable
        TestMoveToLastIndex(lyrics, lyric, expected);
    }

    [TestCase(nameof(singleLyric), 0, 0, 0)]
    [TestCase(nameof(singleLyric), 0, 3, 3)]
    [TestCase(nameof(singleLyric), 0, -1, null)] // will check the invalid case.
    [TestCase(nameof(singleLyric), 0, 5, null)]
    public void TestMoveToTargetLyric(string sourceName, int lyricIndex, int textIndex, int? expectedTextIndex)
    {
        var lyrics = GetLyricsByMethodName(sourceName);
        var lyric = lyrics[lyricIndex];
        var expected = createExpectedCaretPosition(lyrics, lyricIndex, expectedTextIndex);

        // Check move to target position.
        TestMoveToTargetLyric(lyrics, lyric, textIndex, expected);
    }

    #endregion

    protected abstract TCaret CreateCaret(Lyric lyric, int index);

    private TCaret createCaretPosition(IEnumerable<Lyric> lyrics, int lyricIndex, int index)
    {
        var lyric = lyrics.ElementAtOrDefault(lyricIndex);
        if (lyric == null)
            throw new ArgumentNullException();

        return CreateCaret(lyric, index);
    }

    private TCaret? createExpectedCaretPosition(IEnumerable<Lyric> lyrics, int? lyricIndex, int? index)
    {
        if (lyricIndex == null || index == null)
            return null;

        return createCaretPosition(lyrics, lyricIndex.Value, index.Value);
    }

    #region source

    private static Lyric[] singleLyric => new[]
    {
        new Lyric
        {
            Text = "カラオケ"
        }
    };

    private static Lyric[] singleLyricWithOneText => new[]
    {
        new Lyric
        {
            Text = "A"
        }
    };

    private static Lyric[] singleLyricWithNoText => new[]
    {
        new Lyric()
    };

    private static Lyric[] twoLyricsWithText => new[]
    {
        new Lyric
        {
            Text = "カラオケ"
        },
        new Lyric
        {
            Text = "大好き"
        }
    };

    private static Lyric[] threeLyricsWithSpacing => new[]
    {
        new Lyric
        {
            Text = "カラオケ"
        },
        new Lyric(),
        new Lyric
        {
            Text = "大好き"
        }
    };

    #endregion
}
