//
// QRCodeRenderer.cs
//
// Copyright (c) 2019 HODL Wallet
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;

using SkiaSharp;

using QRCoder;

namespace HodlWallet2.Core.Utils
{
    public class QRCodeRenderer : IDisposable
    {
        /// <summary>
        /// Gets the paint.
        /// </summary>
        /// <value>The paint.</value>
        public SKPaint Paint { get; } = new SKPaint();

        /// <summary>
        /// Render the specified data into the given area of the target canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="area">The area.</param>
        /// <param name="data">The data.</param>
        public void Render(SKCanvas canvas, SKRect area, QRCodeData data)
        {
            if (data != null)
            {

                var rows = data.ModuleMatrix.Count;
                var columns = data.ModuleMatrix.Select(x => x.Count).Max();
                var cellHeight = area.Height / rows;
                var cellWidth = area.Width / columns;

                for (int y = 0; y < rows; y++)
                {
                    var row = data.ModuleMatrix.ElementAt(y);
                    for (int x = 0; x < row.Count; x++)
                    {
                        if (row[x])
                        {
                            var rect = SKRect.Create(area.Left + x * cellWidth, area.Top + y * cellHeight, cellWidth, cellHeight);
                            canvas.DrawRect(rect, this.Paint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="T:SkiaSharp.QRCodeGeneration.QRCodeRenderer"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:SkiaSharp.QRCodeGeneration.QRCodeRenderer"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="T:SkiaSharp.QRCodeGeneration.QRCodeRenderer"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:SkiaSharp.QRCodeGeneration.QRCodeRenderer"/> so the garbage collector can reclaim the memory
        /// that the <see cref="T:SkiaSharp.QRCodeGeneration.QRCodeRenderer"/> was occupying.</remarks>
        public void Dispose()
        {
            this.Paint.Dispose();
        }
    }
}
