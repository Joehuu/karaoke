﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Karaoke.Beatmaps;
using osu.Game.Rulesets.Karaoke.Beatmaps.Metadatas;
using osu.Game.Rulesets.Karaoke.Graphics.Cursor;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Karaoke.Statistics;

public partial class BeatmapMetadataGraph : Container
{
    private const float spacing = 10;
    private const float transition_duration = 250;

    public BeatmapMetadataGraph(IBeatmap beatmap)
    {
        Masking = true;
        CornerRadius = 5;

        var beatmapInfo = beatmap.BeatmapInfo;
        var karaokeBeatmap = beatmap as KaraokeBeatmap;
        InternalChildren = new Drawable[]
        {
            new Box
            {
                Name = "Background",
                RelativeSizeAxes = Axes.Both,
                Colour = Color4.Black.Opacity(0.5f),
            },
            new OsuScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false,
                Padding = new MarginPadding { Left = spacing / 2, Top = spacing / 2 },
                Child = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    LayoutDuration = transition_duration,
                    LayoutEasing = Easing.OutQuad,
                    Spacing = new Vector2(spacing),
                    Children = new MetadataSection[]
                    {
                        new TextMetadataSection("Description")
                        {
                            Text = beatmapInfo?.DifficultyName ?? string.Empty
                        },
                        new TextMetadataSection("Source")
                        {
                            Text = beatmapInfo?.Metadata.Source ?? string.Empty
                        },
                        new TextMetadataSection("Tags")
                        {
                            Text = beatmapInfo?.Metadata.Tags ?? string.Empty
                        },
                        new SingerMetadataSection("Singer")
                        {
                            Singers = karaokeBeatmap?.SingerInfo.GetAllSingers().ToArray() ?? Array.Empty<Singer>()
                        }
                    },
                },
            },
        };
    }

    private abstract partial class MetadataSection : Container
    {
        protected FillFlowContainer TextContainer { get; }

        protected MetadataSection(string title)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChild = TextContainer = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(spacing / 2),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Child = new OsuSpriteText
                        {
                            Text = title,
                            Font = OsuFont.GetFont(weight: FontWeight.Bold, size: 14),
                        },
                    },
                },
            };
        }
    }

    private partial class TextMetadataSection : MetadataSection
    {
        private TextFlowContainer? textFlow;

        public TextMetadataSection(string title)
            : base(title)
        {
        }

        public string Text
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.FadeOut(transition_duration);
                    return;
                }

                this.FadeIn(transition_duration);

                setTextAsync(value);
            }
        }

        private void setTextAsync(string text)
        {
            textFlow?.Expire();
            TextContainer.Add(textFlow = new OsuTextFlowContainer(s => s.Font = s.Font.With(size: 14))
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Colour = Color4.White.Opacity(0.75f),
                Text = text
            });
        }
    }

    private partial class SingerMetadataSection : MetadataSection
    {
        private FillFlowContainer<SingerSpriteText>? textFlow;

        public SingerMetadataSection(string title)
            : base(title)
        {
        }

        public Singer[] Singers
        {
            set
            {
                if (!value.Any())
                {
                    this.FadeOut(transition_duration);
                    return;
                }

                this.FadeIn(transition_duration);

                setSingerAsync(value);
            }
        }

        private void setSingerAsync(IEnumerable<Singer> singers)
        {
            textFlow?.Expire();
            TextContainer.Add(textFlow = new FillFlowContainer<SingerSpriteText>
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Spacing = new Vector2(10),
                Colour = Color4.White.Opacity(0.75f),
            });

            foreach (var singer in singers)
            {
                textFlow.Add(new SingerSpriteText
                {
                    Singer = singer
                });
            }
        }

        private partial class SingerSpriteText : CompositeDrawable, IHasCustomTooltip<Singer>
        {
            private readonly OsuSpriteText osuSpriteText;

            public SingerSpriteText()
            {
                AutoSizeAxes = Axes.Both;
                InternalChildren = new[]
                {
                    osuSpriteText = new OsuSpriteText
                    {
                        Font = OsuFont.GetFont(size: 14)
                    }
                };
            }

            private Singer? singer;

            public Singer? Singer
            {
                get => singer;
                set
                {
                    singer = value;
                    osuSpriteText.Text = singer?.Name ?? "Known singer";
                }
            }

            public ITooltip<Singer> GetCustomTooltip() => new SingerToolTip();

            public Singer? TooltipContent => Singer;
        }
    }
}
