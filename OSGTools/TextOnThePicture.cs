using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public static class TextOnThePicture
    {
        private static string inputFileName;
        private static Image inputImage = Image.FromFile(inputFileName);
        private static string inputText;
        private static FontFamily ff = new FontFamily(GenericFontFamilies.SansSerif);
        private static int fs = 48;
        private static Font font = new Font(ff, fs, FontStyle.Italic);
        private static Brush brush = new SolidBrush(Color.Violet);
        private static string outputFileName;

        public static void drawText(string inputfilename, string outputfilename = null, string text = null)
        {
            inputFileName = inputfilename;
            outputFileName = outputfilename ?? (inputfilename.Split('.')[0] + ".png");
            inputText = text ?? "sport-show-room.ru";

            int x = 50;
            int y = inputImage.Height - 50;
            int h = 100;
            int w = inputImage.Width - 50;
            Rectangle rect = new Rectangle(x, y, w, h);

            using (Graphics g = Graphics.FromImage(inputImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingMode = CompositingMode.SourceOver;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.DrawString(text, font, brush, rect);
                g.Save();
            }

            inputImage.Save(outputFileName, ImageFormat.Png);
        }
    }
}