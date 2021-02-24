namespace OpenControls.Wpf.SurfacePlot.TextRenderer
{
    internal class TextureSettings
    {
        public TextureSettings(string fontName, int fontSize)
        {
            ++Count;
            FontName = fontName;
            FontSize = fontSize;
            GlyphInfoArray = new GlyphInfo[256];
        }

        private static string _tempFolder;
        private static string TempFolder
        {
            get
            {
                if (_tempFolder == null)
                {
                    _tempFolder = System.IO.Path.GetTempPath();
                }
                return _tempFolder;
            }
        }
        private static int Count = 0;

        public string FontBitmapFilename
        {
            get
            {
                return System.IO.Path.Combine(TempFolder, "Font_" + Count + ".png");
            }
        }

        public int GlyphsPerLine = 16;
        public int GlyphLineCount = 16;

        public class GlyphInfo
        {
            public float X;
            public float Y;
            public float Width;
            public float Height;
        }
        public GlyphInfo[] GlyphInfoArray;

        public readonly int FontSize;
        public bool BitmapFont = false;
        public string FromFile;
        public readonly string FontName;
    }
}
