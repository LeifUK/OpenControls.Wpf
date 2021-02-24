using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace OpenControls.Wpf.SurfacePlot.TextRenderer
{
    internal class TextRenderer
    {
        public TextRenderer(string fontName, int fontSize, Color textColour, Color backgroundColour)
        {
            _textureSettings = new TextureSettings(fontName, fontSize);
            _textColour = textColour;
            _backgroundColour = backgroundColour;
            SetTiltInRadians(0);
        }

        TextureSettings _textureSettings;

        /*
         * Purpose: Creates a bitmap image of the character set and saves it as a file => [APP_DATA]\local\Temp\font_1.png
         * 
         * We do this so that the bitmap can be loaded 
         */
        private void GenerateFontImage()
        {
            System.Drawing.Font font;
            if (!string.IsNullOrWhiteSpace(_textureSettings.FromFile))
            {
                var collection = new System.Drawing.Text.PrivateFontCollection();
                collection.AddFontFile(_textureSettings.FromFile);
                var fontFamily = new System.Drawing.FontFamily(System.IO.Path.GetFileNameWithoutExtension(_textureSettings.FromFile), collection);
                font = new System.Drawing.Font(fontFamily, _textureSettings.FontSize);
            }
            else
            {
                font = new System.Drawing.Font(new System.Drawing.FontFamily(_textureSettings.FontName), _textureSettings.FontSize, FontStyle.Bold);
            }

            // Measure the size of each character when written to a bitmap
            using (System.Drawing.Bitmap bitmap = new Bitmap(100, 100, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    char c = (char)0;
                    float previousY = 0;
                    float lineHeight = 0;
                    for (int p = 0; p < _textureSettings.GlyphLineCount; p++)
                    {
                        float previousX = 0;
                        for (int n = 0; n < _textureSettings.GlyphsPerLine; n++, ++c)
                        {
                            SizeF size = graphics.MeasureString(c.ToString(), font, 0);
                            // The next step is purely empirical => remove superfluous padding
                            size = new SizeF(size.Width - _textureSettings.FontSize / 6, size.Height - _textureSettings.FontSize / 6);

                            if ((p == 0) && (n == 0))
                            {
                                lineHeight = size.Height;
                            }

                            TextureSettings.GlyphInfo glyphoInfo = new TextureSettings.GlyphInfo();
                            _textureSettings.GlyphInfoArray[c] = glyphoInfo;
                            glyphoInfo.X = previousX;
                            glyphoInfo.Y = previousY;
                            glyphoInfo.Width = size.Width;
                            glyphoInfo.Height = size.Height;
                            previousX += size.Width;

                            TextureWidth = System.Math.Max(TextureWidth, (int)(previousX + 1));
                        }
                        previousY += lineHeight;
                        TextureHeight = System.Math.Max(TextureHeight, (int)(previousY + 1));
                    }
                }
            }

            // Now create the character bitmap
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(TextureWidth, TextureHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                {
                    graphics.Clear(_backgroundColour);

                    if (_textureSettings.BitmapFont)
                    {
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
                    }
                    else
                    {
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                    }

                    char c = (char)0;
                    for (int i = 0; i < 256; ++i, ++c)
                    {
                        TextureSettings.GlyphInfo glyphoInfo = _textureSettings.GlyphInfoArray[i];
                        graphics.DrawString(c.ToString(), font, new SolidBrush(_textColour), glyphoInfo.X, glyphoInfo.Y);
                    }
                }
                bitmap.Save(_textureSettings.FontBitmapFilename);
            }
        }

        int FontTextureID;
        int TextureWidth;
        int TextureHeight;
        Color _textColour;
        Color _backgroundColour;

        private int LoadTexture(string filename)
        {
            using (var bitmap = new Bitmap(filename))
            {
                GL.GenTextures(1, out FontTextureID);
                GL.BindTexture(TextureTarget.Texture2D, FontTextureID);
                System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                bitmap.UnlockBits(data);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                return FontTextureID;
            }
        }

        public void Load()
        {
            GenerateFontImage();

            FontTextureID = LoadTexture(_textureSettings.FontBitmapFilename);
        }

        public float MeasureTextLength(string text)
        {
            float xScreen = 0;
            for (int n = 0; n < text.Length; n++)
            {
                char idx = text[n];
                xScreen += _textureSettings.GlyphInfoArray[idx].Width;
            }
            return xScreen;
        }

        public float MeasureTextHeight()
        {
            return _textureSettings.GlyphInfoArray['H'].Width;
        }

        private double _cosineTilt;
        private double _sineTilt;

        public void SetTiltInRadians(double tiltInRadians)
        {
            _cosineTilt = System.Math.Cos(tiltInRadians);
            _sineTilt = System.Math.Sin(tiltInRadians);
        }

        /*
         * Draw text: the text by default lies in the x,y plane and along the x axis in the positive direction. 
         * It can be tilted about the x-axis using the SetTiltInRadians method. 
         * Returns the string length in pixels. 
         */
        public void DrawText(float x, float y, float z, string text)
        {
            // Make this texture active
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BindTexture(TextureTarget.Texture2D, FontTextureID);

            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Transparent);
            float xScreen = x;
            float yScreen = y;
            foreach (var idx in text)
            {
                TextureSettings.GlyphInfo glyphoInfo = _textureSettings.GlyphInfoArray[idx];
                float xTexture = (float)glyphoInfo.X / (float)TextureWidth;
                float yTexture = (float)glyphoInfo.Y / (float)TextureHeight;
                float widthTexture = (float)glyphoInfo.Width / (float)TextureWidth;
                float heightTexture = (float)glyphoInfo.Height / (float)TextureHeight;

                float dy = (float)(glyphoInfo.Height * _cosineTilt);
                float dz = -(float)(glyphoInfo.Height * _sineTilt);

                GL.TexCoord3(xTexture, yTexture, z);
                GL.Vertex3(xScreen, yScreen + dy, z + dz);

                GL.TexCoord3(xTexture + widthTexture, yTexture, z);
                GL.Vertex3(xScreen + glyphoInfo.Width, yScreen + dy, z + dz);

                GL.TexCoord3(xTexture + widthTexture, yTexture + heightTexture, z);
                GL.Vertex3(xScreen + glyphoInfo.Width, yScreen, z);

                GL.TexCoord3(xTexture, yTexture + heightTexture, z);
                GL.Vertex3(xScreen, yScreen, z);

                xScreen += glyphoInfo.Width;
            }

            GL.End();
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }
    }

}
