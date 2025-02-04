﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using Newtonsoft.Json;
using osu.Game.IO.Serialization;

namespace osu.Game.Rulesets.Karaoke.Tests.IO.Serialization.Converters;

public abstract class BaseSingleConverterTest<TConverter> where TConverter : JsonConverter, new()
{
    protected JsonSerializerSettings CreateSettings()
    {
        var globalSetting = JsonSerializableExtensions.CreateGlobalSettings();
        globalSetting.Formatting = Formatting.None; // do not change new line in testing.
        globalSetting.Converters.Add(new TConverter());

        var converters = CreateExtraConverts();

        foreach (var converter in converters)
        {
            globalSetting.Converters.Add(converter);
        }

        return globalSetting;
    }

    protected virtual JsonConverter[] CreateExtraConverts() => Array.Empty<JsonConverter>();
}
