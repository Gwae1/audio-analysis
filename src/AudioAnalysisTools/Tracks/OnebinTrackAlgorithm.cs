// <copyright file="OnebinTrackAlgorithm.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace AudioAnalysisTools.Tracks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Acoustics.Shared;
    using AnalysisPrograms.Recognizers.Base;
    using AudioAnalysisTools.Events;
    using AudioAnalysisTools.Events.Tracks;
    using AudioAnalysisTools.StandardSpectrograms;
    using TowseyLibrary;
    using TrackType = AudioAnalysisTools.Events.Tracks.TrackType;

    public static class OnebinTrackAlgorithm
    {
        /// <summary>
        /// This method averages dB log values incorrectly but it is faster than doing many log conversions.
        /// This method is used to find acoustic events and is accurate enough for the purpose.
        /// </summary>
        public static (List<EventCommon> ListOfevents, double[] CombinedIntensityArray) GetOnebinTracks(
            SpectrogramStandard sonogram,
            OnebinTrackParameters parameters,
            TimeSpan segmentStartOffset)
        {
            var sonogramData = sonogram.Data;
            int frameCount = sonogramData.GetLength(0);
            int binCount = sonogramData.GetLength(1);
            int nyquist = sonogram.NyquistFrequency;
            double binWidth = nyquist / (double)binCount;
            int minBin = (int)Math.Round(parameters.MinHertz.Value / binWidth);
            int maxBin = (int)Math.Round(parameters.MaxHertz.Value / binWidth);
            double decibelThreshold = parameters.DecibelThreshold.Value;
            double minDuration = parameters.MinDuration.Value;
            double maxDuration = parameters.MaxDuration.Value;

            var converter = new UnitConverters(
                segmentStartOffset: segmentStartOffset.TotalSeconds,
                sampleRate: sonogram.SampleRate,
                frameSize: sonogram.Configuration.WindowSize,
                frameOverlap: sonogram.Configuration.WindowOverlap);

            //Find all bin peaks and place in peaks matrix
            var peaks = new double[frameCount, binCount];
            for (int tf = 0; tf < frameCount; tf++)
            {
                for (int bin = minBin + 1; bin < maxBin - 1; bin++)
                {
                    if (sonogramData[tf, bin] < decibelThreshold)
                    {
                        continue;
                    }

                    // here we define the amplitude profile of a whistle. The buffer zone around whistle is five bins wide.
                    var bandIntensity = ((sonogramData[tf, bin - 1] * 0.5) + sonogramData[tf, bin] + (sonogramData[tf, bin + 1] * 0.5)) / 2.0;
                    var topSidebandIntensity = (sonogramData[tf, bin + 3] + sonogramData[tf, bin + 4] + sonogramData[tf, bin + 5]) / 3.0;
                    var netAmplitude = 0.0;
                    if (bin < 4)
                    {
                        netAmplitude = bandIntensity - topSidebandIntensity;
                    }
                    else
                    {
                        var bottomSideBandIntensity = (sonogramData[tf, bin - 3] + sonogramData[tf, bin - 4] + sonogramData[tf, bin - 5]) / 3.0;
                        netAmplitude = bandIntensity - ((topSidebandIntensity + bottomSideBandIntensity) / 2.0);
                    }

                    if (netAmplitude >= decibelThreshold)
                    {
                        peaks[tf, bin] = sonogramData[tf, bin];
                    }
                }
            }

            var tracks = GetOnebinTracks(peaks, minDuration, maxDuration, decibelThreshold, converter);

            // Initialise tracks as events and get the combined intensity array.
            var events = new List<WhistleEvent>();
            var combinedIntensityArray = new double[frameCount];
            var scoreRange = new Interval<double>(0, decibelThreshold * 5);

            foreach (var track in tracks)
            {
                var ae = new WhistleEvent(track, scoreRange)
                {
                    SegmentStartSeconds = segmentStartOffset.TotalSeconds,
                    SegmentDurationSeconds = frameCount * converter.SecondsPerFrameStep,
                    Name = "Whistle",
                };

                events.Add(ae);

                // fill the intensity array
                var startRow = converter.FrameFromStartTime(track.StartTimeSeconds);
                var amplitudeTrack = track.GetAmplitudeOverTimeFrames();
                for (int i = 0; i < amplitudeTrack.Length; i++)
                {
                    combinedIntensityArray[startRow + i] = Math.Max(combinedIntensityArray[startRow + i], amplitudeTrack[i]);
                }
            }

            // This algorithm tends to produce temporally overlapped whistle events in adjacent channels.
            // Combine overlapping whistle events
            var hertzDifference = 4 * binWidth;
            var whistleEvents = WhistleEvent.CombineAdjacentWhistleEvents(events, hertzDifference);

            // Finally filter the whistles for presense of excess noise in buffer band just above the whistle.
            // Excess noise would suggest this is not a whistle event.
            var bufferHertz = 300;
            var bufferBins = (int)Math.Round(bufferHertz / binWidth);
            var filteredEvents = new List<EventCommon>();
            foreach (var ev in whistleEvents)
            {
                var avNhAmplitude = GetAverageAmplitudeInNeighbourhood((SpectralEvent)ev, sonogramData, bufferBins, converter);
                Console.WriteLine($"###################################Buffer Average decibels = {avNhAmplitude}");

                if (avNhAmplitude < decibelThreshold)
                {
                    // There is little acoustic activity in the buffer zone above the whistle. It is likely to be a whistle.
                    filteredEvents.Add(ev);
                }
            }

            return (filteredEvents, combinedIntensityArray);
        }

        /// <summary>
        /// This method finds whistle tracks, that is horizontal ridges in one frequency bin.
        /// </summary>
        /// <param name="peaks">Peaks matrix.</param>
        /// <param name="minDuration">Minimum duration of a whistle.</param>
        /// <param name="maxDuration">Maximum duration of a whistle.</param>
        /// <param name="threshold">Minimum amplitude threshold to be valid whistle.</param>
        /// <param name="converter">For the time/frequency scales.</param>
        /// <returns>A list of whistle tracks.</returns>
        public static List<Track> GetOnebinTracks(double[,] peaks, double minDuration, double maxDuration, double threshold, UnitConverters converter)
        {
            int frameCount = peaks.GetLength(0);
            int bandwidthBinCount = peaks.GetLength(1);

            var tracks = new List<Track>();

            // Look for possible track starts and initialise as track.
            // Cannot include edge rows & columns because of edge effects.
            // Each row is a time frame which is a spectrum. Each column is a frequency bin
            for (int row = 0; row < frameCount; row++)
            {
                for (int col = 3; col < bandwidthBinCount - 3; col++)
                {
                    if (peaks[row, col] < threshold)
                    {
                        continue;
                    }

                    // Visit each spectral peak in order. Each may be start of possible whistle track
                    var track = GetOnebinTrack(peaks, row, col, threshold, converter);

                    //If track has length within duration bounds, then add the track to list.
                    if (track.DurationSeconds >= minDuration && track.DurationSeconds <= maxDuration)
                    {
                        tracks.Add(track);
                    }
                }
            }

            return tracks;
        }

        public static Track GetOnebinTrack(double[,] peaks, int startRow, int bin, double threshold, UnitConverters converter)
        {
            var track = new Track(converter, TrackType.OneBinTrack);
            track.SetPoint(startRow, bin, peaks[startRow, bin]);

            // set the start point in peaks matrix to zero to prevent return to this point.
            peaks[startRow, bin] = 0.0;

            //Now move to next time frame.
            for (int row = startRow + 1; row < peaks.GetLength(0) - 2; row++)
            {
                // explore track in vicinity.
                int nhStart = Math.Max(row - 2, 0);
                int nhEnd = Math.Min(row + 3, peaks.GetLength(0));
                int nhWidth = nhEnd - nhStart + 1;
                double avIntensity = 0.0;
                for (int nh = nhStart; nh < nhEnd; nh++)
                {
                    avIntensity += peaks[nh, bin];
                }

                avIntensity /= (double)nhWidth;
                track.SetPoint(row, bin, peaks[row, bin]);

                // Set visited value to zero so as not to revisit.....
                peaks[row, bin] = 0.0;

                // next line is for debug purposes
                //var info = track.CheckPoint(row, bin);

                // Check if track has come to an end - average value is less than threshold.
                if (avIntensity < threshold)
                {
                    return track;
                }
            }

            return track;
        }

        /// <summary>
        /// Calculates the average amplitude in the frequency just above the whistle.
        /// If it contains above threshold acoustic content, this is unlikely to be a whistle.
        /// </summary>
        /// <param name="ev">The event.</param>
        /// <param name="sonogramData">The spectrogram data as matrix with origin top/left.</param>
        /// <param name="bufferBins">THe badnwidth of the buffer zone in bins.</param>
        /// <param name="converter">A converter to convert seconds/Hertz to frames/bins.</param>
        /// <returns>Average of the spectrogram amplitude in buffer band above whistler.</returns>
        public static double GetAverageAmplitudeInNeighbourhood(SpectralEvent ev, double[,] sonogramData, int bufferBins, UnitConverters converter)
        {
            var bottomBufferBin = converter.GetFreqBinFromHertz(ev.HighFrequencyHertz) + 5;
            var topBufferBin = bottomBufferBin + bufferBins;
            var frameStart = converter.FrameFromStartTime(ev.EventStartSeconds);
            var frameEnd = converter.FrameFromStartTime(ev.EventEndSeconds);
            var subMatrix = MatrixTools.Submatrix<double>(sonogramData, frameStart, bottomBufferBin, frameEnd, topBufferBin);
            var averageRowDecibels = MatrixTools.GetRowAverages(subMatrix);
            var av = averageRowDecibels.Average();
            return av;
        }
    }
}
