
/************************************************************************************************************
 * Copyright (C) 2020 Francis-Black EWANE
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
************************************************************************************************************/
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Xpandables.Net.QrCodes.Presenter
{
    /// <summary>
    /// The QrCode class.
    /// </summary>
    public sealed class QrCode : AbstractQrCode
    {
        /// <summary>
        /// Initializes a new instance of <see cref="QrCode" /> class with the QrCode data content.
        /// </summary>
        /// <param name="data">The QrCode data content.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="data" /> is null.</exception>
        public QrCode(QrCodeData data) : base(data) { }

        /// <summary>
        /// Returns a bitmap that contains the resulting QR code as image.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="drawQuietZones"></param>
        public Bitmap GetGraphic(int pixelsPerModule, Color darkColor, Color lightColor, bool drawQuietZones = true)
        {
            var size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Bitmap(size, size);
            using (var gfx = Graphics.FromImage(bmp))
            using (var lightBrush = new SolidBrush(lightColor))
            using (var darkBrush = new SolidBrush(darkColor))
            {
                for (var x = 0; x < size + offset; x += pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y += pixelsPerModule)
                    {
                        var module = QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                        if (module)
                        {
                            gfx.FillRectangle(darkBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                        else
                        {
                            gfx.FillRectangle(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                    }
                }

                gfx.Save();
            }

            return bmp;
        }

        /// <summary>
        /// Returns a bitmap that contains the resulting QR code as image.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        public Bitmap GetGraphic(int pixelsPerModule) => GetGraphic(pixelsPerModule, Color.Black, Color.White, true);

        /// <summary>
        /// Returns a bitmap that contains the resulting QR code as image.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColorHtmlHex"></param>
        /// <param name="lightColorHtmlHex"></param>
        /// <param name="drawQuietZones"></param>
        public Bitmap GetGraphic(int pixelsPerModule, string darkColorHtmlHex, string lightColorHtmlHex, bool drawQuietZones = true)
            => GetGraphic(pixelsPerModule, ColorTranslator.FromHtml(darkColorHtmlHex), ColorTranslator.FromHtml(lightColorHtmlHex), drawQuietZones);

        /// <summary>
        /// Returns a bitmap that contains the resulting QR code as image.
        /// </summary>
        /// <param name="pixelsPerModule"></param>
        /// <param name="darkColor"></param>
        /// <param name="lightColor"></param>
        /// <param name="icon"></param>
        /// <param name="iconSizePercent"></param>
        /// <param name="iconBorderWidth"></param>
        /// <param name="drawQuietZones"></param>
        public Bitmap GetGraphic(
            int pixelsPerModule,
            Color darkColor,
            Color lightColor,
            Bitmap? icon = null,
            int iconSizePercent = 15,
            int iconBorderWidth = 6,
            bool drawQuietZones = true)
        {
            var size = (QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (var gfx = Graphics.FromImage(bmp))
            using (var lightBrush = new SolidBrush(lightColor))
            using (var darkBrush = new SolidBrush(darkColor))
            {
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.Clear(lightColor);

                var drawIconFlag = icon != null && iconSizePercent > 0 && iconSizePercent <= 100;

                GraphicsPath? iconPath = null;
                float iconDestWidth = 0, iconDestHeight = 0, iconX = 0, iconY = 0;

                if (drawIconFlag)
                {
                    iconDestWidth = iconSizePercent * bmp.Width / 100f;
                    iconDestHeight = drawIconFlag ? iconDestWidth * icon!.Height / icon.Width : 0;
                    iconX = (bmp.Width - iconDestWidth) / 2;
                    iconY = (bmp.Height - iconDestHeight) / 2;

                    var centerDest = new RectangleF(iconX - iconBorderWidth, iconY - iconBorderWidth, iconDestWidth + iconBorderWidth * 2, iconDestHeight + iconBorderWidth * 2);
                    iconPath = CreateRoundedRectanglePath(centerDest, iconBorderWidth * 2);
                }

                for (var x = 0; x < size + offset; x += pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y += pixelsPerModule)
                    {
                        var module = QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];

                        if (module)
                        {
                            var r = new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule);

                            if (drawIconFlag)
                            {
                                var region = new Region(r);
                                region.Exclude(iconPath!);
                                gfx.FillRegion(darkBrush, region);
                            }
                            else
                            {
                                gfx.FillRectangle(darkBrush, r);
                            }
                        }
                        else
                        {
                            gfx.FillRectangle(lightBrush, new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule));
                        }
                    }
                }

                if (drawIconFlag)
                {
                    var iconDestRect = new RectangleF(iconX, iconY, iconDestWidth, iconDestHeight);
                    gfx.DrawImage(icon!, iconDestRect, new RectangleF(0, 0, icon!.Width, icon.Height), GraphicsUnit.Pixel);
                }

                gfx.Save();
            }

            return bmp;
        }

#pragma warning disable CA1822 // Mark members as static
        internal GraphicsPath CreateRoundedRectanglePath(RectangleF rect, int cornerRadius)
#pragma warning restore CA1822 // Mark members as static
        {
            var roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}
