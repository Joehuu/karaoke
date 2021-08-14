﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Game.Rulesets.Karaoke.Skinning.Fonts;

namespace osu.Game.Rulesets.Karaoke.IO.Stores
{
    public class KaraokeLocalFontStore : FontStore
    {
        private readonly Dictionary<FontInfo, IGlyphStore> fontInfos = new Dictionary<FontInfo, IGlyphStore>();
        private readonly IResourceStore<TextureUpload> store;
        private readonly FontManager fontManager;

        /// <summary>
        /// Construct a font store to be added to a parent font store via <see cref="AddFont"/>.
        /// </summary>
        /// <param name="fontManager">font manager.</param>
        /// <param name="store">The texture source.</param>
        /// <param name="scaleAdjust">The raw pixel height of the font. Can be used to apply a global scale or metric to font usages.</param>
        public KaraokeLocalFontStore(FontManager fontManager, IResourceStore<TextureUpload> store = null, float scaleAdjust = 100)
            : base(store, scaleAdjust)
        {
            this.store = store;
            this.fontManager = fontManager;
        }

        public void AddFont(FontInfo fontInfo)
        {
            var hasFont = fontInfos.Keys.Contains(fontInfo);
            if (hasFont)
                return;

            var glyphStore = fontManager.GetGlyphStore(fontInfo);
            if (glyphStore == null)
                return;

            AddStore(glyphStore);
            fontInfos.Add(fontInfo, glyphStore as IGlyphStore);
        }

        public void RemoveFont(FontInfo fontInfo)
        {
            var glyphStore = fontInfos[fontInfo];
            if (glyphStore == null)
                return;

            RemoveStore(store);
            fontInfos.Remove(fontInfo);
        }

        public void ClearFont()
        {
            foreach (var (fontInfo, _) in fontInfos)
            {
                RemoveFont(fontInfo);
            }
        }
    }
}
