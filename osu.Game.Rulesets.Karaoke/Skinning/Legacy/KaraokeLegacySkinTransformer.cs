﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Beatmaps;
using osu.Game.IO;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Beatmaps.Formats;
using osu.Game.Rulesets.Karaoke.Skinning.Metadatas.Fonts;
using osu.Game.Rulesets.Karaoke.Skinning.Metadatas.Layouts;
using osu.Game.Rulesets.Karaoke.Skinning.Metadatas.Notes;
using osu.Game.Rulesets.Karaoke.UI.Components;
using osu.Game.Rulesets.Karaoke.UI.HUD;
using osu.Game.Rulesets.Karaoke.UI.Scrolling;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Karaoke.Skinning.Legacy
{
    public class KaraokeLegacySkinTransformer : LegacySkinTransformer
    {
        private readonly KaraokeBeatmap beatmap;
        private readonly Lazy<bool> isLegacySkin;

        private readonly IDictionary<int, Bindable<LyricFont>> bindableFonts = new Dictionary<int, Bindable<LyricFont>>();
        private readonly IDictionary<int, Bindable<LyricLayout>> bindableLayouts = new Dictionary<int, Bindable<LyricLayout>>();
        private readonly IDictionary<int, Bindable<NoteSkin>> bindableNotes = new Dictionary<int, Bindable<NoteSkin>>();

        private readonly Bindable<IDictionary<int, string>> bindableFontsLookup = new();
        private readonly Bindable<IDictionary<int, string>> bindableLayoutsLookup = new();
        private readonly Bindable<IDictionary<int, string>> bindableNotesLookup = new();

        private readonly Bindable<float> bindableColumnHeight = new(DefaultColumnBackground.COLUMN_HEIGHT);
        private readonly Bindable<float> bindableColumnSpacing = new(ScrollingNotePlayfield.COLUMN_SPACING);

        public KaraokeLegacySkinTransformer(ISkin source, IBeatmap beatmap)
            : base(source)
        {
            this.beatmap = (KaraokeBeatmap)beatmap;
            isLegacySkin = new Lazy<bool>(() => GetConfig<SkinConfiguration.LegacySetting, decimal>(SkinConfiguration.LegacySetting.Version) != null);

            // TODO : need a better way to load resource
            var assembly = Assembly.GetExecutingAssembly();
            const string resource_name = @"osu.Game.Rulesets.Karaoke.Resources.Skin.default.skin";

            using (var stream = assembly.GetManifestResourceStream(resource_name))
            using (var reader = new LineBufferedReader(stream))
            {
                var skin = new KaraokeSkinDecoder().Decode(reader);

                // Create bindable
                for (int i = 0; i < skin.Fonts.Count; i++)
                    bindableFonts.Add(i, new Bindable<LyricFont>(skin.Fonts[i]));
                for (int i = 0; i < skin.Layouts.Count; i++)
                    bindableLayouts.Add(i, new Bindable<LyricLayout>(skin.Layouts[i]));
                for (int i = 0; i < skin.NoteSkins.Count; i++)
                    bindableNotes.Add(i, new Bindable<NoteSkin>(skin.NoteSkins[i]));

                // Create lookups
                bindableFontsLookup.Value = skin.Fonts.ToDictionary(k => skin.Fonts.IndexOf(k), y => y.Name);
                bindableLayoutsLookup.Value = skin.Layouts.ToDictionary(k => skin.Layouts.IndexOf(k), y => y.Name);
                bindableNotesLookup.Value = skin.NoteSkins.ToDictionary(k => skin.NoteSkins.IndexOf(k), y => y.Name);
            }
        }

        public override Drawable GetDrawableComponent(ISkinComponent component)
        {
            switch (component)
            {
                case SkinnableTargetComponent targetComponent:
                    switch (targetComponent.Target)
                    {
                        case SkinnableTarget.MainHUDComponents:
                            var components = base.GetDrawableComponent(component) as SkinnableTargetComponentsContainer ?? getTargetComponentsContainerFromOtherPlace();
                            components.Add(new SettingButtonsDisplay
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                            });
                            return components;

                        default:
                            return base.GetDrawableComponent(component);
                    }

                case GameplaySkinComponent<HitResult> resultComponent:
                    return getResult(resultComponent.Component);

                case KaraokeSkinComponent karaokeComponent:
                    if (!isLegacySkin.Value)
                        return null;

                    return karaokeComponent.Component switch
                    {
                        KaraokeSkinComponents.ColumnBackground => new LegacyColumnBackground(),
                        KaraokeSkinComponents.StageBackground => new LegacyStageBackground(),
                        KaraokeSkinComponents.JudgementLine => new LegacyJudgementLine(),
                        KaraokeSkinComponents.Note => new LegacyNotePiece(),
                        KaraokeSkinComponents.HitExplosion => new LegacyHitExplosion(),
                        _ => throw new InvalidEnumArgumentException(nameof(karaokeComponent.Component))
                    };

                default:
                    return base.GetDrawableComponent(component);
            }

            SkinnableTargetComponentsContainer getTargetComponentsContainerFromOtherPlace()
            {
                switch (Skin)
                {
                    case LegacyBeatmapSkin legacyBeatmapSkin:
                        return new TempLegacySkin(legacyBeatmapSkin.SkinInfo).GetDrawableComponent(component) as SkinnableTargetComponentsContainer;

                    default:
                        throw new InvalidCastException();
                }
            }
        }

        private Drawable getResult(HitResult result)
        {
            // todo : get real component
            return null;
        }

        public override IBindable<TValue> GetConfig<TLookup, TValue>(TLookup lookup)
        {
            switch (lookup)
            {
                // Lookup skin by type and index
                case KaraokeSkinLookup skinLookup:
                {
                    var config = skinLookup.Config;
                    var lookupNumber = skinLookup.Lookup;

                    return config switch
                    {
                        KaraokeSkinConfiguration.LyricStyle => SkinUtils.As<TValue>(bindableFonts[lookupNumber]),
                        KaraokeSkinConfiguration.LyricLayout => SkinUtils.As<TValue>(bindableLayouts[lookupNumber]),
                        KaraokeSkinConfiguration.NoteStyle => SkinUtils.As<TValue>(bindableNotes[lookupNumber]),
                        _ => throw new InvalidEnumArgumentException(nameof(config))
                    };
                }

                // Lookup list of name by type
                case KaraokeIndexLookup indexLookup:
                    return indexLookup switch
                    {
                        KaraokeIndexLookup.Layout => SkinUtils.As<TValue>(bindableLayoutsLookup),
                        KaraokeIndexLookup.Style => SkinUtils.As<TValue>(bindableFontsLookup),
                        KaraokeIndexLookup.Note => SkinUtils.As<TValue>(bindableNotesLookup),
                        _ => throw new InvalidEnumArgumentException(nameof(indexLookup))
                    };

                case KaraokeSkinConfigurationLookup skinConfigurationLookup:
                    switch (skinConfigurationLookup.Lookup)
                    {
                        // should use customize height for note playfield in lyric editor.
                        case LegacyKaraokeSkinConfigurationLookups.ColumnHeight:
                            return SkinUtils.As<TValue>(bindableColumnHeight);

                        // not have note playfield judgement spacing in lyric editor.
                        case LegacyKaraokeSkinConfigurationLookups.ColumnSpacing:
                            return SkinUtils.As<TValue>(bindableColumnSpacing);
                    }

                    break;
            }

            return base.GetConfig<TLookup, TValue>(lookup);
        }

        // it's a temp class for just getting SkinnableTarget.MainHUDComponents
        private class TempLegacySkin : LegacySkin
        {
            public TempLegacySkin(SkinInfo skin)
                : base(skin, null, null, default(string))
            {
            }
        }
    }
}
