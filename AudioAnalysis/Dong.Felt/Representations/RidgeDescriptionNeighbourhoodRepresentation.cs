﻿using AudioAnalysisTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Dong.Felt.Representations
{
    public class RidgeDescriptionNeighbourhoodRepresentation : NeighbourhoodRepresentation
    {
        #region Properties

        /// <summary>
        /// Gets or sets the dominant orientation type of the neighbourhood.
        /// </summary>
        public int dominantOrientationType { get; set; }

        /// <summary>
        /// Gets or sets the count of points of interest (pois) in the neighbourhood.
        /// </summary>
        public int dominantPOICount { get; set; }

        /// <summary>
        /// Gets or sets the sum of the magnitude of pois with dominant orientation in the neighbourhood.
        /// </summary>
        public double dominantMagnitudeSum { get; set; }

        /// <summary>
        /// Gets or sets the orientation type 1 of the neighbourhood.
        /// </summary>
        public int orientationType1 { get; set; }

        /// <summary>
        /// Gets or sets the orientation type 2 of the neighbourhood.
        /// </summary>
        public int orientationType2 { get; set; }

        /// <summary>
        /// Gets or sets the orientation type 3 of the neighbourhood.
        /// </summary>
        public int orientationType3 { get; set; }

        /// <summary>
        /// Gets or sets the orientation type 4 of the neighbourhood.
        /// </summary>
        public int orientationType4 { get; set; }

        /// <summary>
        /// Gets or sets the count of points of interest (pois) with orentation type 1 in the neighbourhood.
        /// </summary>
        public int orientationType1POICount { get; set; }

        /// <summary>
        /// Gets or sets the count of points of interest (pois) with orentation type 2 in the neighbourhood.
        /// </summary>
        public int orientationType2POICount { get; set; }

        /// <summary>
        /// Gets or sets the count of points of interest (pois) with orentation type 3 in the neighbourhood.
        /// </summary>
        public int orientationType3POICount { get; set; }

        /// <summary>
        /// Gets or sets the count of points of interest (pois) with orentation type 4 in the neighbourhood.
        /// </summary>
        public int orientationType4POICount { get; set; }

        /// <summary>
        /// Gets or sets the sum of the magnitude of pois with the orientation type 1 in the neighbourhood.
        /// </summary>
        public double orentationType1MagnitudeSum { get; set; }

        /// <summary>
        /// Gets or sets the sum of the magnitude of pois with the orientation type 2 in the neighbourhood.
        /// </summary>
        public double orentationType2MagnitudeSum { get; set; }

        /// <summary>
        /// Gets or sets the sum of the magnitude of pois with the orientation type 3 in the neighbourhood.
        /// </summary>
        public double orentationType3MagnitudeSum { get; set; }

        /// <summary>
        /// Gets or sets the sum of the magnitude of pois with the orientation type 4 in the neighbourhood.
        /// </summary>
        public double orentationType4MagnitudeSum { get; set; }

        #endregion

        #region public method

        /// <summary>
        /// This method is used to get the dominantOrientationType, dominantPOICount, and  dominantMagnitudeSum of the neighbourhood, the neighbourhood is composed
        /// a matrix of PointOfInterest.
        /// </summary>
        /// <param name="neighbourhood">This is a fix neighbourhood which contains a list of points of interest.</param>
        /// <param name="PointX">This value is the X coordinate of centroid of neighbourhood.</param>
        /// <param name="PointY">This value is the Y coordinate of centroid of neighbourhood.</param>
        public void SetDominantNeighbourhoodRepresentation(PointOfInterest[,] neighbourhood, int pointX, int pointY)
        {
            var timeScale = 11.6; // ms
            var frequencyScale = 43.0; // hz           

            var ridgeNeighbourhoodFeatureVector = RectangularRepresentation.SliceRidgeRepresentation(neighbourhood, pointX, pointY);
            var ridgeDominantOrientationRepresentation = RectangularRepresentation.SliceMainSlopeRepresentation(ridgeNeighbourhoodFeatureVector);
            dominantOrientationType = ridgeDominantOrientationRepresentation.Item1;
            dominantPOICount = ridgeDominantOrientationRepresentation.Item2;

            int maximumRowIndex = neighbourhood.GetLength(0);
            int maximumColIndex = neighbourhood.GetLength(1);

            for (int rowIndex = 0; rowIndex < maximumRowIndex; rowIndex++)
            {
                for (int colIndex = 0; colIndex < maximumColIndex; colIndex++)
                {
                    if (neighbourhood[rowIndex, colIndex].OrientationCategory == dominantOrientationType)
                    {
                        dominantMagnitudeSum += neighbourhood[rowIndex, colIndex].RidgeMagnitude;
                    }
                }
            }

            // baseclass properties
            RowIndex = (int)(pointY * timeScale);
            ColIndex = (int)(pointX * frequencyScale);
            WidthPx = ridgeNeighbourhoodFeatureVector.neighbourhoodWidth;
            HeightPx = ridgeNeighbourhoodFeatureVector.neighbourhoodHeight;
            Duration = TimeSpan.FromMilliseconds(neighbourhood.GetLength(1) * timeScale);
            FrequencyRange = neighbourhood.GetLength(0) * frequencyScale;
        }

        /// <summary>
        /// This method is used for obtaining the general representation based on different orientations. 
        /// </summary>
        /// <param name="neighbourhood"></param>
        public void SetGeneralNeighbourhoodRepresentation(PointOfInterest[,] neighbourhood)
        {
            int maximumRowIndex = neighbourhood.GetLength(1);
            int maximumColIndex = neighbourhood.GetLength(2);

            for (int rowIndex = 0; rowIndex < maximumColIndex; rowIndex++)
            {
                for (int colIndex = 0; colIndex < maximumColIndex; colIndex++)
                {
                    if (neighbourhood[rowIndex, colIndex].OrientationCategory == (int)Direction.East)
                    {
                        orientationType1POICount++;
                        orentationType1MagnitudeSum += neighbourhood[rowIndex, colIndex].RidgeMagnitude;
                    }
                    if (neighbourhood[rowIndex, colIndex].OrientationCategory == (int)Direction.NorthEast)
                    {
                        orientationType2POICount++;
                        orentationType2MagnitudeSum += neighbourhood[rowIndex, colIndex].RidgeMagnitude;
                    }
                    if (neighbourhood[rowIndex, colIndex].OrientationCategory == (int)Direction.North)
                    {
                        orientationType3POICount++;
                        orentationType3MagnitudeSum += neighbourhood[rowIndex, colIndex].RidgeMagnitude;
                    }
                    if (neighbourhood[rowIndex, colIndex].OrientationCategory == (int)Direction.NorthWest)
                    {
                        orientationType4POICount++;
                        orentationType4MagnitudeSum += neighbourhood[rowIndex, colIndex].RidgeMagnitude;
                    }
                }
            }
        }

        public static RidgeDescriptionNeighbourhoodRepresentation FromFeatureVector(PointOfInterest[,] matrix, int rowIndex, int colIndex)
        {
            var ridgeNeighbourhoodRepresentation = new RidgeDescriptionNeighbourhoodRepresentation();
            ridgeNeighbourhoodRepresentation.SetDominantNeighbourhoodRepresentation(matrix, rowIndex, colIndex);
            return ridgeNeighbourhoodRepresentation;
        }

        public static RidgeNeighbourhoodFeatureVector ToFeatureVector(IEnumerable<string[]> lines)
        {
            return null;
        }

        public static RidgeDescriptionNeighbourhoodRepresentation FromNeighbourhoodCsv(IEnumerable<string> lines)
        {
            // assume csv file is laid out as we expect it to be.
            var listLines = lines.ToList();
            var nh = new RidgeDescriptionNeighbourhoodRepresentation()
            {
                ColIndex = int.Parse(listLines[0]),
                RowIndex = int.Parse(listLines[1]),
                WidthPx = int.Parse(listLines[2]),
                HeightPx = int.Parse(listLines[3]),
                Duration = TimeSpan.FromMilliseconds(double.Parse(listLines[4])),
                FrequencyRange = double.Parse(listLines[5]),
                dominantOrientationType = int.Parse(listLines[6]),
                dominantPOICount = int.Parse(listLines[7]),
            };
            return nh;
        }

        public static RidgeDescriptionNeighbourhoodRepresentation FromRegionCsv(IEnumerable<string> lines)
        {
            // assume csv file is laid out as we expect it to be.
            var listLines = lines.ToList();

            var nh = new RidgeDescriptionNeighbourhoodRepresentation()
            {
                ColIndex = (int)double.Parse(listLines[1]),
                RowIndex = int.Parse(listLines[2]),
                dominantOrientationType = int.Parse(listLines[3]),
                dominantPOICount = int.Parse(listLines[4]),
            };
            return nh;
        }
 
        /// <summary>
        /// This method is used for reconstruct the spectrogram with ridge neighbourhood representation, it can be done by show ridge neighbourhood representation on image. 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="nhRepresentation"></param>
        public static void RidgeNeighbourhoodRepresentationToImage(Graphics graphics, RidgeDescriptionNeighbourhoodRepresentation nhRepresentation)
        {
            int neighbourhoodLength = 13;
            int nhRadius = neighbourhoodLength / 2;
            int maxFrequencyBand = 257;
            int x = StatisticalAnalysis.MilliSecondsToFrameIndex(nhRepresentation.ColIndex);
            int y = maxFrequencyBand - StatisticalAnalysis.FrequencyToFruencyBandIndex(nhRepresentation.RowIndex);
            int dominantOrientationCategory = nhRepresentation.dominantOrientationType;
            int dominantPOICount = nhRepresentation.dominantPOICount;
            int score = dominantOrientationCategory * dominantPOICount;
            int times = score / neighbourhoodLength;
            var brush = new SolidBrush(Color.Black);
            var pen = new Pen(Color.Black, 1);
            FillNeighbourhood(graphics, brush, pen, dominantOrientationCategory, times, score, x, y, neighbourhoodLength);          
        }

        /// <summary>
        /// This method is used to fill the neighbourhood by drawing lines. The lines can be horizontal, vertical, diagonal. 
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="orientationType"></param>
        /// <param name="times"></param>
        /// <param name="scores"></param>
        /// <param name="startPointX"></param>
        /// <param name="startPointY"></param>
        /// <param name="neighbourhoodLength"></param>
        public static void FillNeighbourhood(Graphics graphics, SolidBrush brush, Pen pen, int orientationType, int times, int scores, int startPointX, int startPointY, int neighbourhoodLength)
        {
            var nhRadius = neighbourhoodLength / 2;
            var modValue = scores % neighbourhoodLength;
            var maxIntegerIndex = times;
            if (times > 0)
            {                
                if (orientationType == 1)  // fill the neighbourhood with horizontal lines. 
                {
                    for (int index = 1; index <= maxIntegerIndex; index++)
                    {
                        var offset = index / 2;
                        if (index % 2 == 0)
                        {
                            //fill in the line above the centroid of nh.
                            graphics.FillRectangle(brush, startPointX, startPointY - nhRadius - offset, neighbourhoodLength, 1);
                        }
                        else
                        {
                            //fill in the line below the centroid line of nh.
                            graphics.FillRectangle(brush, startPointX, startPointY - nhRadius + offset, neighbourhoodLength, 1);
                        }
                    } // end for
                    graphics.FillRectangle(brush, startPointX, startPointY - nhRadius, modValue, 1);
                }//end if orientation  
                if (orientationType == 2)  // fill the neighbourhood with horizontal lines. 
                {
                    for (int index = 1; index <= maxIntegerIndex; index++)
                    {
                        var offset = index / 2;
                        if (index % 2 == 0)
                        {
                            //fill in the line above the diagonal centroid of nh.
                            var startPoint = new Point(startPointX, startPointY);
                            var endPoint = new Point(startPointX + neighbourhoodLength - offset - 1, startPointY - neighbourhoodLength + 1);
                            graphics.DrawLine(pen, startPoint, endPoint);
                        }
                        else
                        {
                            //fill in the line below the diagonal centroid line of nh.
                            var startPoint = new Point(startPointX + offset, startPointY);
                            var endPoint = new Point(startPointX + neighbourhoodLength - 1, startPointY - neighbourhoodLength + offset + 1);
                            graphics.DrawLine(pen, startPoint, endPoint);
                        }
                    } // end for
                    var offset1 = maxIntegerIndex;
                    var lastStartPoint1 = new Point(startPointX, startPointY - offset1);
                    var lastEndPoint1 = new Point(startPointX + modValue, startPointY + modValue);
                    graphics.DrawLine(pen, lastStartPoint1, lastEndPoint1);
                }//end if orientation  
                else if (orientationType == 3) // fill the neighbourhood with vertical lines. 
                {
                    for (int index = 1; index <= maxIntegerIndex; index++)
                    {
                        var offset = index / 2;
                        if (index % 2 == 0)
                        {
                            //fill in the line on the left of the centroid of nh.
                            graphics.FillRectangle(brush, startPointX + nhRadius - offset, startPointY - neighbourhoodLength, 1, neighbourhoodLength);
                        }
                        else
                        {
                            //fill in the line on the right of the centroid line of nh.
                            graphics.FillRectangle(brush, startPointX + nhRadius + offset, startPointY - neighbourhoodLength, 1, neighbourhoodLength);
                        }
                    } // end for
                    graphics.FillRectangle(brush, startPointX + nhRadius + maxIntegerIndex, startPointY - neighbourhoodLength, 1, modValue);
                }
                
                if (orientationType == 4)  // fill the neighbourhood with horizontal lines. 
                {
                    for (int index = 1; index <= maxIntegerIndex; index++)
                    {
                        var offset = index / 2;
                        if (index % 2 == 0)
                        {
                            //fill in the line above the diagonal centroid of nh.
                            var startPoint = new Point(startPointX + offset, startPointY - neighbourhoodLength);
                            var endPoint = new Point(startPointX + neighbourhoodLength + offset, startPointY - offset);
                            graphics.DrawLine(pen, startPoint, endPoint);
                        }
                        else
                        {
                            //fill in the line below the diagonal centroid line of nh.
                            var startPoint = new Point(startPointX, startPointY - offset);
                            var endPoint = new Point(startPointX + neighbourhoodLength - offset, startPointY);
                            graphics.DrawLine(pen, startPoint, endPoint);
                        }
                    } // end for
                    var offset1 = maxIntegerIndex;
                    var lastStartPoint1 = new Point(startPointX, startPointY - offset1);
                    var lastEndPoint1 = new Point(startPointX + neighbourhoodLength + modValue, startPointY - modValue);
                    graphics.DrawLine(pen, lastStartPoint1, lastEndPoint1);
                }//end if orientation  
            }// end if times > 0
            else
            {
                if (orientationType == 1)  // fill the neighbourhood with horizontal lines. 
                {
                    graphics.FillRectangle(brush, startPointX, startPointY - nhRadius, modValue, 1);
                }
                else if (orientationType == 2)
                {
                    var lastStartPoint1 = new Point(startPointX, startPointY);
                    var lastEndPoint1 = new Point(startPointX + modValue - 1, startPointY - modValue + 1);
                    graphics.DrawLine(pen, lastStartPoint1, lastEndPoint1);
                }
                else if (orientationType == 3)
                {
                    graphics.FillRectangle(brush, startPointX + nhRadius, startPointY - neighbourhoodLength, 1, modValue);
                }             
                else if (orientationType == 4)
                {
                    var lastStartPoint1 = new Point(startPointX, startPointY - neighbourhoodLength);
                    var lastEndPoint1 = new Point(startPointX + modValue - 1, startPointY - modValue + 1);
                    graphics.DrawLine(pen, lastStartPoint1, lastEndPoint1);
                }
            }
        }

        #endregion
    }
}