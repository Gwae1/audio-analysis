// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointOfInterest.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AudioAnalysisTools
{
    using System;
    using System.Collections.Generic;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;

    /// <summary>
    /// The point of interest.
    /// </summary>
    public class PointOfInterest
    {
        /// <summary>
        /// The anchor color.
        /// </summary>
        public static readonly Color TemplateColor = Color.Chartreuse;

        /// <summary>
        /// The default border color.
        /// </summary>
        public static readonly Color DefaultBorderColor = Color.Crimson;

        /// <summary>
        /// The hits color.
        /// </summary>
        public static readonly Color HitsColor = Color.Blue;

        private Color? drawColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointOfInterest"/> class.
        /// </summary>
        /// <param name="point">
        /// The point to represent.
        /// </param>
        public PointOfInterest(Point point)
        {
            this.Point = point;
        }

        public PointOfInterest(TimeSpan time, double hertz)
        {
            this.TimeLocation = time;
            this.Hertz = hertz;
        }

        /// <summary>
        /// Gets or sets the point.
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Gets or sets the X-axis timescale seconds per pixel.
        /// </summary>

        /// <summary>
        /// Gets or sets the time of the point of interest from beginning of recording.
        /// </summary>
        public TimeSpan TimeLocation { get; set; }

        /// <summary>
        /// Gets or sets the frequency location of point of interest.
        /// </summary>
        public double Hertz { get; set; }

        /// <summary>
        /// Gets or sets the X-axis timescale seconds per pixel.
        /// </summary>
        public TimeSpan TimeScale { get; set; }

        /// <summary>
        /// Gets or sets the Y-axis scale herz per pixel.
        /// </summary>
        public double HerzScale { get; set; }

        /// <summary>
        /// Gets or sets the matrix of fft.
        /// </summary>
        public double[,] fftMatrix { get; set; }
        /// <summary>
        /// Gets or sets the draw color.
        /// </summary>
        public Color DrawColor
        {
            get
            {
                //return (this.drawColor.HasValue ? this.drawColor.Value : DefaultBorderColor);
                return this.drawColor ?? DefaultBorderColor;
            }

            set
            {
                this.drawColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the spectral intensity at the given point.
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Gets or sets the Ridge Magnitude at the given point.
        /// </summary>
        public double RidgeMagnitude { get; set; }

        /// <summary>
        /// Gets or sets the Local Ridge Orientation.
        /// </summary>
        public double RidgeOrientation { get; set; }

        /// <summary>
        /// Gets or sets the Local Ridge Orientation.
        /// </summary>
        public int OrientationCategory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets boolean - is POI a local maximum?.
        /// </summary>
        public bool IsLocalMaximum { get; set; }

        /// <summary>
        /// Draw a box from a point at top left with radius width and radius length.
        /// </summary>
        public void DrawBox(IImageProcessingContext context, IEnumerable<PointOfInterest> pointsOfInterest, int radius)
        {
            foreach (PointOfInterest poi in pointsOfInterest)
            {
                var pen = new Pen(Color.Crimson, 1);

                //graphics.DrawRectangle(pen, poi.Point.X, height - poi.Point.Y - 1, radius, radius);
                context.DrawRectangle(pen, poi.Point.X, poi.Point.Y, radius, radius);
            }
        }

        public void DrawLocalMax(Image<Rgb24> bmp, int spectrogramHeight)
        {
            if (this.IsLocalMaximum)
            {
                int x = (int)Math.Round(this.TimeLocation.TotalSeconds / this.TimeScale.TotalSeconds);
                int y = spectrogramHeight - (int)Math.Round(this.Hertz / this.HerzScale) - 1;
                Color color = this.DrawColor;
                bmp[x, y] = color;

                //bmp[x, y-1] = color;
                //bmp[x, y+1] = color;
                //bmp[x-1, y] = color;
                //bmp[x+1, y] = color;
            }
        }

        public void DrawPoint(Image<Rgb24> bmp, int spectrogramHeight, bool multiPixel)
        {
            //int x = this.Point.X;
            //int y = this.Point.Y;
            int x = (int)Math.Round(this.TimeLocation.TotalSeconds / this.TimeScale.TotalSeconds);
            int y = spectrogramHeight - (int)Math.Round(this.Hertz / this.HerzScale) - 1;
            int orientationCategory = (int)Math.Round(this.RidgeOrientation * 8 / Math.PI);

            //orientation = indexMax * Math.PI / (double)8;

            Color color = this.DrawColor;
            bmp[x, y] = color;
            if (!multiPixel)
            {
                return;
            }

            if (orientationCategory == 0)
            {
                bmp[x - 1, y] = color;
                bmp[x + 1, y] = color;
                bmp[x + 2, y] = color;
            }
            else
            {
                if (orientationCategory == 1)
                {
                    bmp[x + 2, y] = color;
                    bmp[x + 1, y] = color;
                    bmp[x - 1, y] = color;
                }
                else
                {
                    if (orientationCategory == 2)
                    {
                        bmp[x - 1, y + 1] = color;
                        bmp[x + 1, y - 1] = color;
                        bmp[x + 2, y - 2] = color;
                    }
                    else
                        if (orientationCategory == 3)
                    {
                        bmp[x, y - 1] = color;
                        bmp[x, y + 1] = color;
                        bmp[x, y + 2] = color;
                    }
                    else
                            if (orientationCategory == 4)
                    {
                        bmp[x, y - 1] = color;
                        bmp[x, y + 1] = color;
                        bmp[x, y + 2] = color;
                    }
                    else if (orientationCategory == 5)
                    {
                        bmp[x, y - 1] = color;
                        bmp[x, y + 1] = color;
                        bmp[x, y + 2] = color;
                    }
                    else if (orientationCategory == 6)
                    {
                        bmp[x + 2, y + 2] = color;
                        bmp[x + 1, y + 1] = color;
                        bmp[x - 1, y - 1] = color;
                    }
                    else if (orientationCategory == 7)
                    {
                        bmp[x + 2, y] = color;
                        bmp[x + 1, y] = color;
                        bmp[x - 1, y] = color;
                    }
                }
            }
        }

        /// <summary>
        /// Currently, I can only refine the ridge orientation up to 12 possibilities.
        /// </summary>
        public void DrawRefinedOrientationPoint(Image<Rgb24> bmp, int spectrogramHeight)
        {
            int x = (int)Math.Round(this.TimeLocation.TotalSeconds / this.TimeScale.TotalSeconds);
            int y = spectrogramHeight - (int)Math.Round(this.Hertz / this.HerzScale) - 1;
            double orientation = this.RidgeOrientation;
            Color color = this.DrawColor;

            if (orientation == 0 * Math.PI / 12)
            {
                color = Color.Red;
            }
            else if (orientation == 1 * Math.PI / 12)
            {
                color = Color.Pink;
            }
            else if (orientation == 2 * Math.PI / 12)
            {
                color = Color.LightGreen;
            }
            else if (orientation == 3 * Math.PI / 12)
            {
                color = Color.Green;
            }
            else if (orientation == 4 * Math.PI / 12)
            {
                color = Color.LightGreen;
            }
            else if (orientation == 5 * Math.PI / 12)
            {
                color = Color.LightSkyBlue;
            }
            else if (orientation == 6 * Math.PI / 12)
            {
                color = Color.Blue;
            }
            else if (orientation == 7 * Math.PI / 12)
            {
                color = Color.LightSkyBlue;
            }
            else if (orientation == 8 * Math.PI / 12)
            {
                color = Color.Orange;
            }
            else if (orientation == 9 * Math.PI / 12)
            {
                color = Color.DarkOrange;
            }
            else if (orientation == 10 * Math.PI / 12)
            {
                color = Color.Orange;
            }
            else
            {
                color = Color.Pink;
            }

            bmp[x, y] = color;
        } // DrawOrientationPoint

        public void DrawOrientationPoint(Image<Rgb24> bmp, int spectrogramHeight)
        {
            // This ones for structure tensor
            //int x = this.Point.Y;
            //int y = this.Point.X;
            // THis one for decibel ridges
            int x = this.Point.X;
            int y = this.Point.Y;

            // this one for amplitude ridges.
            //int x = this.Point.X;
            //int y = this.Point.Y + 36;
            //int x = (int)Math.Round(this.TimeLocation.TotalSeconds / this.TimeScale.TotalSeconds);
            //int y = spectrogramHeight - (int)Math.Round(this.Herz / this.HerzScale) - 1;
            //int orientationCategory = (int)Math.Round((this.RidgeOrientation * 8) / Math.PI);
            int orientationCategory = this.OrientationCategory;

            //orientation = indexMax * Math.PI / (double)8;
            Color color = this.DrawColor;

            if (orientationCategory == 0)
            {
                color = Color.Red;
            }
            else
            {
                if (orientationCategory == 1)
                {
                    color = Color.Orange;
                }
                else
                {
                    if (orientationCategory == 2)
                    {
                        color = Color.Green;
                    }
                    else
                    {
                        if (orientationCategory == 3)
                        {
                            color = Color.Cyan;
                        }
                        else if (orientationCategory == 4)
                        {
                            color = Color.Blue;
                        }
                        else if (orientationCategory == 5)
                        {
                            color = Color.LightBlue;
                        }
                        else if (orientationCategory == 6)
                        {
                            color = Color.Purple;
                        }
                        else if (orientationCategory == 7)
                        {
                            color = Color.Magenta;
                        }
                        else if (orientationCategory == 8)
                        {
                            color = Color.BlueViolet;
                        }
                        else
                        {
                            color = Color.White;
                        }
                    }
                }
            } // if (orientationCategory == 0) else

            bmp[x, y] = color;
        } // DrawOrientationPoint

        public static void PruneSingletons(List<PointOfInterest> poiList, int rows, int cols)
        {
            double[,] m = TransferPOIsToDoublesMatrix(poiList, rows, cols);
            TowseyLibrary.MatrixTools.SetSingletonsToZero(m);
            RemovePOIsFromList(poiList, m);
        }

        public static void PruneDoublets(List<PointOfInterest> poiList, int rows, int cols)
        {
            double[,] m = TransferPOIsToDoublesMatrix(poiList, rows, cols);
            TowseyLibrary.MatrixTools.SetDoubletsToZero(m);
            RemovePOIsFromList(poiList, m);
        }

        public static List<PointOfInterest> PruneAdjacentTracks(List<PointOfInterest> poiList, int rows, int cols)
        {
            var M = TransferPOIsToMatrix(poiList, rows, cols);
            for (int r = 1; r < rows - 1; r++)
            {
                for (int c = 1; c < cols - 1; c++)
                {
                    if (M[r, c] == null)
                    {
                        continue;
                    }

                    if (M[r, c].OrientationCategory == 0) // horizontal line
                    {
                        if (M[r - 1, c] != null && M[r - 1, c].OrientationCategory == 0)
                        {
                            if (M[r - 1, c].RidgeMagnitude < M[r, c].RidgeMagnitude)
                            {
                                M[r - 1, c] = null;
                            }
                        }

                        if (M[r + 1, c] != null && M[r + 1, c].OrientationCategory == 0)
                        {
                            if (M[r + 1, c].RidgeMagnitude < M[r, c].RidgeMagnitude)
                            {
                                M[r + 1, c] = null;
                            }
                        }
                    }
                    else if (M[r, c].OrientationCategory == 4) // vertical line
                    {
                        if (M[r, c - 1] != null && M[r, c - 1].OrientationCategory == 4)
                        {
                            if (M[r, c - 1].RidgeMagnitude < M[r, c].RidgeMagnitude)
                            {
                                M[r, c - 1] = null;
                            }
                        }

                        if (M[r, c + 1] != null && M[r, c + 1].OrientationCategory == 4)
                        {
                            if (M[r, c + 1].RidgeMagnitude < M[r, c].RidgeMagnitude)
                            {
                                M[r, c + 1] = null;
                            }
                        }
                    } // if (OrientationCategory)
                } // c
            } // for r loop

            return TransferPOIMatrix2List(M);
        } // PruneAdjacentTracks()

        public static PointOfInterest[,] TransferPOIsToMatrix(List<PointOfInterest> list, int rows, int cols)
        {
            PointOfInterest[,] m = new PointOfInterest[rows, cols];
            foreach (PointOfInterest poi in list)
            {
                m[poi.Point.Y, poi.Point.X] = poi;
            }

            return m;
        }

        public static List<PointOfInterest> TransferPOIMatrix2List(PointOfInterest[,] m)
        {
            List<PointOfInterest> list = new List<PointOfInterest>();
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (m[r, c] != null)
                    {
                        list.Add(m[r, c]);
                    }
                }
            }

            return list;
        }

        public static double[,] TransferPOIsToDoublesMatrix(List<PointOfInterest> list, int rows, int cols)
        {
            double[,] m = new double[rows, cols];
            foreach (PointOfInterest poi in list)
            {
                m[poi.Point.Y, poi.Point.X] = poi.RidgeMagnitude;
            }

            return m;
        }

        public static int[,] TransferPOIsToOrientationMatrix(List<PointOfInterest> list, int rows, int cols)
        {
            int[,] m = new int[rows, cols];
            foreach (PointOfInterest poi in list)
            {
                int orientation = poi.OrientationCategory;
                int r = poi.Point.Y;
                int c = poi.Point.X;
                m[r, c] = orientation + 1; // do not want a zero category
                if (orientation == 0)
                {
                    m[r, c - 1] = orientation + 1;
                    m[r, c + 1] = orientation + 1;

                    //m[r, c + 2] = orientation + 1;
                }
                else
                {
                    if (orientation == 1)
                    {
                        m[r, c - 1] = orientation + 1;
                        m[r, c + 1] = orientation + 1;

                        //m[r, c + 2] = orientation + 1;
                    }
                    else
                    {
                        if (orientation == 2)
                        {
                            m[r + 1, c - 1] = orientation + 1;
                            m[r - 1, c + 1] = orientation + 1;

                            //m[r - 2, c + 2] = orientation + 1;
                        }
                        else
                            if (orientation == 3)
                        {
                            m[r - 1, c] = orientation + 1;
                            m[r + 1, c] = orientation + 1;

                            //m[x + 2, y] = orientation + 1;
                            //m[x, y - 1] = orientation + 1;
                            //m[x, y + 1] = orientation + 1;
                            //m[x, y + 2] = orientation + 1;
                        }
                        else
                                if (orientation == 4)
                        {
                            m[r - 1, c] = orientation + 1;
                            m[r + 1, c] = orientation + 1;

                            //m[x + 2, y] = orientation + 1;
                        }
                        else if (orientation == 5)
                        {
                            m[r - 1, c] = orientation + 1;
                            m[r + 1, c] = orientation + 1;

                            //m[r + 2, c] = orientation + 1;
                        }
                        else if (orientation == 6)
                        {
                            //m[r + 2, c + 2] = orientation + 1;
                            m[r + 1, c + 1] = orientation + 1;
                            m[r - 1, c - 1] = orientation + 1;
                        }
                        else if (orientation == 7)
                        {
                            m[r, c - 1] = orientation + 1;
                            m[r, c + 1] = orientation + 1;

                            //m[r, c + 2] = orientation + 1;
                            //m[x + 2, y] = orientation + 1;
                            //m[x + 1, y] = orientation + 1;
                            //m[x - 1, y] = orientation + 1;
                        }
                    }
                }
            } // foreach

            return m;
        } // TransferPOIsToOrientationMatrix()

        public static void RemovePOIsFromList(List<PointOfInterest> list, double[,] m)
        {
            // each (PointOfInterest poi in list)
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (m[list[i].Point.Y, list[i].Point.X] == 0.0)
                {
                    list.Remove(list[i]);
                }
            }
        } // RemovePOIsFromList

        public static void RemoveLowIntensityPOIs(List<PointOfInterest> list, double threshold)
        {
            // each (PointOfInterest poi in list)
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Intensity < threshold)
                {
                    list.Remove(list[i]);
                }
            }
        } // RemovePOIsFromList

        public static void CountPOIsInMatrix(int[,] m, out int poiCount, out double fraction)
        {
            int rows = m.GetLength(0);
            int cols = m.GetLength(1);
            poiCount = 0;
            int cellCount = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (m[r, c] > 0.5)
                    {
                        poiCount++;
                    }

                    cellCount++;
                }
            }

            fraction = poiCount / (double)cellCount;
        } // CountPOIsInMatrix()
    }
}