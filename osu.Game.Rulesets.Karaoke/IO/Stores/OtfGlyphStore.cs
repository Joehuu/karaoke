﻿// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace osu.Game.Rulesets.Karaoke.IO.Stores
{
    public class OtfGlyphStore : IResourceStore<TextureUpload>, IGlyphStore
    {
        protected readonly string AssetName;

        public readonly string FontName;

        protected readonly ResourceStore<byte[]> Store;

        [CanBeNull]
        public Font Font => completionSource.Task.Result;

        private IFontInstance fontInstance => Font?.Instance;

        private readonly TaskCompletionSource<Font> completionSource = new TaskCompletionSource<Font>();

        /// <summary>
        /// Create a new glyph store.
        /// </summary>
        /// <param name="store">The store to provide font resources.</param>
        /// <param name="assetName">The base name of thße font.</param>
        /// <param name="textureLoader">An optional platform-specific store for loading textures. Should load for the store provided in <param ref="param"/>.</param>
        public OtfGlyphStore(ResourceStore<byte[]> store, string assetName = null)
        {
            Store = new ResourceStore<byte[]>(store);

            Store.AddExtension("ttf");

            AssetName = assetName;

            FontName = assetName?.Split('/').Last();
        }

        private Task fontLoadTask;

        public Task LoadFontAsync() => fontLoadTask ??= Task.Factory.StartNew(() =>
        {
            try
            {
                Font font;

                using (var s = Store.GetStream($@"{AssetName}"))
                {
                    var fonts = new FontCollection();
                    var fontFamily = fonts.Install(s);
                    font = new Font(fontFamily, 12);
                }

                completionSource.SetResult(font);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Couldn't load font asset from {AssetName}.");
                completionSource.SetResult(null);
                throw;
            }
        }, TaskCreationOptions.PreferFairness);

        public bool HasGlyph(char c) => fontInstance?.GetGlyph(c).GlyphType != GlyphType.Fallback;

        public int GetBaseHeight() => fontInstance?.LineHeight ?? 0;

        public int? GetBaseHeight(string name)
        {
            if (name != FontName)
                return null;

            return GetBaseHeight();
        }

        [CanBeNull]
        public CharacterGlyph Get(char character)
        {
            if (fontInstance == null)
                return null;

            var glyphInstance = fontInstance.GetGlyph(character);
            if (glyphInstance.GlyphType == GlyphType.Fallback)
                return null;

            // todo : get x and y offset.
            return new CharacterGlyph(character, 0, 0, glyphInstance.AdvanceWidth, this);
        }

        public int GetKerning(char left, char right)
        {
            if (fontInstance == null)
                return 0;

            var leftGlyphInstance = fontInstance.GetGlyph(left);
            var rightGlyphInstance = fontInstance.GetGlyph(right);

            return (int)fontInstance.GetOffset(leftGlyphInstance, rightGlyphInstance).X;
        }

        Task<CharacterGlyph> IResourceStore<CharacterGlyph>.GetAsync(string name) => Task.Run(() => ((IGlyphStore)this).Get(name[0]));

        CharacterGlyph IResourceStore<CharacterGlyph>.Get(string name) => Get(name[0]);

        public TextureUpload Get(string name)
        {
            if (fontInstance == null) return null;

            if (name.Length > 1 && !name.StartsWith($@"{FontName}/", StringComparison.Ordinal))
                return null;

            return !HasGlyph(name.Last()) ? null : LoadCharacter(name.Last());
        }

        public virtual async Task<TextureUpload> GetAsync(string name)
        {
            if (name.Length > 1 && !name.StartsWith($@"{FontName}/", StringComparison.Ordinal))
                return null;

            await completionSource.Task.ConfigureAwait(false);

            return LoadCharacter(name.Last());
        }

        protected int LoadedGlyphCount;

        protected virtual TextureUpload LoadCharacter(char c)
        {
            LoadedGlyphCount++;

            // see: https://stackoverflow.com/a/53023454/4105113

            const int font_size = 24;

            var style = new RendererOptions(Font);
            var text = new string(new[] { c });
            var bounds = TextMeasurer.MeasureBounds(text, style);
            var targetSize = new
            {
                Width = (int)(bounds.Width * font_size),
                Height = (int)(bounds.Height * font_size),
            };

            // this is the important line, where we render the glyphs to a vector instead of directly to the image
            // this allows further vector manipulation (scaling, translating) etc without the expensive pixel operations.
            var glyphs = SixLabors.ImageSharp.Drawing.TextBuilder.GenerateGlyphs(text, style);

            // adjust scale
            var widthScale = targetSize.Width / glyphs.Bounds.Width;
            var heightScale = targetSize.Height / glyphs.Bounds.Height;
            var minScale = Math.Min(widthScale, heightScale);

            // scale so that it will fit exactly in image shape once rendered
            glyphs = glyphs.Scale(minScale);

            // move the vectorised glyph so that it touch top and left edges
            // could be tweeked to center horizontally & vertically here
            glyphs = glyphs.Translate(-glyphs.Bounds.Location);

            // create image with char.
            using var img = new Image<Rgba32>(targetSize.Width, targetSize.Height, new Rgba32(255, 255, 255, 0));
            img.Mutate(i => i.Fill(Color.White, glyphs));
            return new TextureUpload(img);
        }

        public Stream GetStream(string name) => throw new NotSupportedException();

        public IEnumerable<string> GetAvailableResources() => throw new NotSupportedException();

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
