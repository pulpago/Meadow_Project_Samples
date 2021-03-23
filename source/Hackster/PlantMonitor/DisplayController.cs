﻿using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using SimpleJpegDecoder;
using System.IO;
using System.Reflection;

namespace PlantMonitor
{
    public class DisplayController
    {
        GraphicsLibrary graphics;

        public DisplayController(St7789 display)
        { 
            graphics = new GraphicsLibrary(display);
            graphics.CurrentFont = new Font12x20();
            graphics.Stroke = 3;

            graphics.Clear();

            graphics.DrawRectangle(0, 0, 240, 240, Color.White, true);

            string plant = "Plant";
            string monitor = "Monitor";

            graphics.CurrentFont = new Font12x16();
            graphics.DrawText((240 - (plant.Length * 24)) / 2, 80, plant, Color.Black, GraphicsLibrary.ScaleFactor.X2);
            graphics.DrawText((240 - (monitor.Length * 24)) / 2, 130, monitor, Color.Black, GraphicsLibrary.ScaleFactor.X2);

            graphics.Show();
        }

        void UpdateImage(int index, int xOffSet, int yOffSet)
        {
            var jpgData = LoadResource($"level_{index}.jpg");
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            graphics.DrawRectangle(0, 0, 240, 208, Color.White, true);

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x + xOffSet, y + yOffSet, Color.FromRgb(r, g, b));

                x++;
                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }

            graphics.Show();
        }

        byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"PlantMonitor.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public void UpdateMoistureImage(FloatChangeResult moistureReadings) 
        {
            float moisture = moistureReadings.New;

            if (moisture > 1) moisture = 1f;
            else if (moisture < 0) moisture = 0f;
            
            if (moisture > 0 && moisture <= 0.25) {
                UpdateImage(0, 42, 10);
            }
            else if (moisture > 0.25 && moisture <= 0.50) {
                UpdateImage(1, 28, 4);
            }
            else if (moisture > 0.50 && moisture <= 0.75) {
                UpdateImage(2, 31, 5);
            }
            else if (moisture > 0.75 && moisture <= 1.0) {
                UpdateImage(3, 35, 5);
            }

            graphics.Show();
        }

        public void UpdateMoisturePercentage(float newValue, float oldValue) 
        {
            if (newValue > 1) newValue = 1f;
            else if (newValue < 0) newValue = 0f;

            graphics.DrawText(0, 208, $"{(int)(oldValue * 100)}%", Color.White, GraphicsLibrary.ScaleFactor.X2);
            graphics.DrawText(0, 208, $"{(int)(newValue * 100)}%", Color.Black, GraphicsLibrary.ScaleFactor.X2);
            graphics.Show();
        }

        public void UpdateTemperatureValue(float newValue, float oldValue) 
        {
            string t = $"{(int)oldValue}C";           
            graphics.DrawText(240 - t.Length * 24, 208, t, Color.White, GraphicsLibrary.ScaleFactor.X2);

            t = $"{(int)newValue}C";
            graphics.DrawText(240 - t.Length * 24, 208, t, Color.Black, GraphicsLibrary.ScaleFactor.X2);

            graphics.Show();
        }
    }
}