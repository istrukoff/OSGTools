using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSGTools
{
    public static class TextOnPicture
    {
        // нарисовать кривой текст
        private static string drawText(string inputdirpath, string inputfilename, string text)
        {
            return drawText(inputdirpath, inputfilename, text, Color.OrangeRed);
        }

        // нарисовать кривой текст указанного цвета
        private static string drawText(string inputdirpath, string inputfilename, string text, Color color)
        {
            string outputfilename = Functions.generateFileName(6, 12);

            FontFamily ffam = new FontFamily(GenericFontFamilies.SansSerif);
            int fsize = 40;
            FontStyle fstyle = FontStyle.Bold | FontStyle.Italic;
            Font font = new Font(ffam, fsize, fstyle);

            Image inputImage = Image.FromFile(inputfilename);

            Brush brush = new SolidBrush(color);

            List<string> listText = new List<string>();

            foreach (char ch in text)
                listText.Add(ch.ToString());

            int height = inputImage.Height;
            int width = inputImage.Width;

            Random rnd_xy = new Random();

            int x = rnd_xy.Next(0, 100);
            int y = rnd_xy.Next(height - 200, height - 50);
            int h = 100;
            int w = width / 2;

            StringFormat frmtText = new StringFormat();
            frmtText.LineAlignment = StringAlignment.Center;
            frmtText.Alignment = StringAlignment.Center;

            int x1 = x;
            int y1 = y;
            int w1 = 128;
            int step_x = 96;
            int step_y = 64;

            Random rnd_step_x = new Random();
            Random rnd_step_y = new Random();

            using (Graphics g = Graphics.FromImage(inputImage))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingMode = CompositingMode.SourceOver;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                foreach (string s in listText)
                {
                    if (step_y >= 8)
                        step_y -= 4;
                    x1 += step_x;
                    y1 -= rnd_step_y.Next(4, step_y);
                    Rectangle r = new Rectangle(x1, y1, w1, h);
                    g.DrawString(s, font, brush, r, frmtText);
                }
                g.Save();
            }

            inputImage.Save(string.Format(@"{0}\output\{1}", inputdirpath, outputfilename), ImageFormat.Jpeg);

            return outputfilename;
        }

        // нарисовать кривой текст на картинках из папки
        private static string drawText(string inputdirpath, string text, bool indir)
        {
            string outputfilename = Functions.generateFileName(6, 12);

            List<string> listOfPictures = new List<string>();

            foreach (string f in Directory.GetFiles(inputdirpath, "*.*", SearchOption.TopDirectoryOnly).Where(i => i.EndsWith(".jpg") || i.EndsWith(".jpeg") || i.EndsWith(".png")))
                listOfPictures.Add(f);

            FontFamily ffam = new FontFamily(GenericFontFamilies.SansSerif);
            int fsize = 40;
            FontStyle fstyle = FontStyle.Bold | FontStyle.Italic;
            Font font = new Font(ffam, fsize, fstyle);

            for (int i = 0; i < listOfPictures.Count; i++)
            {
                Image inputImage = Image.FromFile(listOfPictures[i]);

                Color color = Color.DarkOrange;
                Brush brush = new SolidBrush(color);

                List<string> listText = new List<string>();

                foreach (char ch in text)
                    listText.Add(ch.ToString());

                int height = inputImage.Height;
                int width = inputImage.Width;

                Random rnd_xy = new Random();

                int x = rnd_xy.Next(0, 100);
                int y = rnd_xy.Next(height - 200, height - 50);
                int h = 100;
                int w = width / 2;

                StringFormat frmtText = new StringFormat();
                frmtText.LineAlignment = StringAlignment.Center;
                frmtText.Alignment = StringAlignment.Center;

                int x1 = x;
                int y1 = y;
                int w1 = 128;
                int step_x = 96;
                int step_y = 64;

                Random rnd_step_x = new Random();
                Random rnd_step_y = new Random();

                using (Graphics g = Graphics.FromImage(inputImage))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingMode = CompositingMode.SourceOver;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                    foreach (string s in listText)
                    {
                        if (step_y >= 8)
                            step_y -= 4;
                        x1 += step_x;
                        y1 -= rnd_step_y.Next(4, step_y);
                        Rectangle r = new Rectangle(x1, y1, w1, h);
                        g.DrawString(s, font, brush, r, frmtText);
                    }
                    g.Save();
                }

                inputImage.Save(string.Format(@"{0}\output\{1}", inputdirpath, outputfilename), ImageFormat.Jpeg);
            }

            return outputfilename;
        }

        // нарисовать прямой текст
        public static void drawLineText(string inputfilename, string text, string outputfilename = null)
        {
            outputfilename = outputfilename ?? Functions.generateFileName(6, 12);

            Image inputImage = Image.FromFile(inputfilename);

            FontFamily ffam = new FontFamily(GenericFontFamilies.SansSerif);
            int fsize = 40;
            FontStyle fstyle = FontStyle.Bold | FontStyle.Italic;
            Font font = new Font(ffam, fsize, fstyle);

            Brush brush = new SolidBrush(Color.DarkOrange);

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

            inputImage.Save(outputfilename, ImageFormat.Jpeg);
        }
    }
}