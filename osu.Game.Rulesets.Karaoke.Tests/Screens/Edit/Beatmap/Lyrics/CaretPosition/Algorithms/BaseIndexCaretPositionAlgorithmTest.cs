﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Karaoke.Objects;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.CaretPosition;
using osu.Game.Rulesets.Karaoke.Screens.Edit.Beatmaps.Lyrics.CaretPosition.Algorithms;

namespace osu.Game.Rulesets.Karaoke.Tests.Screens.Edit.Beatmap.Lyrics.CaretPosition.Algorithms;

public abstract class BaseIndexCaretPositionAlgorithmTest<TAlgorithm, TCaret> : BaseCaretPositionAlgorithmTest<TAlgorithm, TCaret>
    where TAlgorithm : IIndexCaretPositionAlgorithm where TCaret : struct, IIndexCaretPosition
{
    protected static void TestMoveToPreviousIndex(Lyric[] lyrics, TCaret caret, TCaret? expected, Action<TAlgorithm>? invokeAlgorithm = null)
    {
        var algorithm = CreateAlgorithm(lyrics);

        invokeAlgorithm?.Invoke(algorithm);

        var actual = algorithm.MoveToPreviousIndex(caret) as TCaret?;
        AssertEqual(expected, actual);
    }

    protected static void TestMoveToNextIndex(Lyric[] lyrics, TCaret caret, TCaret? expected, Action<TAlgorithm>? invokeAlgorithm = null)
    {
        var algorithm = CreateAlgorithm(lyrics);

        invokeAlgorithm?.Invoke(algorithm);

        var actual = algorithm.MoveToNextIndex(caret) as TCaret?;
        AssertEqual(expected, actual);
    }

    protected static void TestMoveToFirstIndex(Lyric[] lyrics, Lyric lyric, TCaret? expected, Action<TAlgorithm>? invokeAlgorithm = null)
    {
        var algorithm = CreateAlgorithm(lyrics);

        invokeAlgorithm?.Invoke(algorithm);

        var actual = algorithm.MoveToFirstIndex(lyric) as TCaret?;
        AssertEqual(expected, actual);
    }

    protected static void TestMoveToLastIndex(Lyric[] lyrics, Lyric lyric, TCaret? expected, Action<TAlgorithm>? invokeAlgorithm = null)
    {
        var algorithm = CreateAlgorithm(lyrics);

        invokeAlgorithm?.Invoke(algorithm);

        var actual = algorithm.MoveToLastIndex(lyric) as TCaret?;
        AssertEqual(expected, actual);
    }

    protected static void TestMoveToTargetLyric<TIndex>(Lyric[] lyrics, Lyric lyric, TIndex index, TCaret? expected, Action<TAlgorithm>? invokeAlgorithm = null)
        where TIndex : notnull
    {
        var algorithm = CreateAlgorithm(lyrics);

        invokeAlgorithm?.Invoke(algorithm);

        var actual = algorithm.MoveToTargetLyric(lyric, index) as TCaret?;
        AssertEqual(expected, actual);
    }
}
