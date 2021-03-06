﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LitoriaFallax_OBSOLETE.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace AnalysisPrograms
{

    /*
     * 
     * 
     * 
     * 
     *    _____   ____    _   _  ____ _______   _    _  _____ ______             ____  ____   _____  ____  _     ______ _______ ______ 
     *   |  __ \ / __ \  | \ | |/ __ \__   __| | |  | |/ ____|  ____|           / __ \|  _ \ / ____|/ __ \| |   |  ____|__   __|  ____|
     *   | |  | | |  | | |  \| | |  | | | |    | |  | | (___ | |__     ______  | |  | | |_) | (___ | |  | | |   | |__     | |  | |__   
     *   | |  | | |  | | | . ` | |  | | | |    | |  | |\___ \|  __|   |______| | |  | |  _ < \___ \| |  | | |   |  __|    | |  |  __|  
     *   | |__| | |__| | | |\  | |__| | | |    | |__| |____) | |____           | |__| | |_) |____) | |__| | |___| |____   | |  | |____ 
     *   |_____/ \____/  |_| \_|\____/  |_|     \____/|_____/|______|           \____/|____/|_____/ \____/|_____|______|  |_|  |______|
     *   
     *   
     *   
     *                                                                                                                                        
     */

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Acoustics.Shared;
    using Acoustics.Shared.Contracts;
    using Acoustics.Shared.Csv;
    using Acoustics.Tools;
    using AnalysisBase;
    using AnalysisBase.ResultBases;
    using Production;
    using AudioAnalysisTools;
    using AudioAnalysisTools.DSP;
    using AudioAnalysisTools.StandardSpectrograms;
    using AudioAnalysisTools.WavTools;
    using TowseyLibrary;
    using ProcessRunner = TowseyLibrary.ProcessRunner;

    /// <summary>
    /// NOTE: This recogniser is for the frog Litoria fallax.
    /// It was built using two recordings:
    /// 1. One from the david Steart CD with high SNR and usual cleaned up recording
    /// 2. One from JCU, Lin and Kiyomi.
    /// Recording 2. also contains canetoad and Limnodynastes convex-something-or-another.
    /// So I have combined the three recognisers into one analysis.
    ///
    /// </summary>
    [Obsolete]
    public class LitoriaFallax_OBSOLETE : AbstractStrongAnalyser
    {
        #region Constants

        public const string AnalysisName = "LitoriaFallax-OLDNONOTUSE";
        public const string AbbreviatedName = "L.fallax";

        public static readonly int ResampleRate = AppConfigHelper.DefaultTargetSampleRate;

        #endregion

        #region Public Properties

        public override AnalysisSettings DefaultSettings => new AnalysisSettings
            {
                AnalysisMaxSegmentDuration = TimeSpan.FromMinutes(1),
                AnalysisMinSegmentDuration = TimeSpan.FromSeconds(30),
                SegmentMediaType = MediaTypes.MediaTypeWav,
                SegmentOverlapDuration = TimeSpan.Zero,
                AnalysisTargetSampleRate = ResampleRate,
            };

        public override string DisplayName => "Litoria fallax";

        public override string Identifier => "Towsey." + AnalysisName;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// A WRAPPER AROUND THE analyzer.Analyze(analysisSettings) METHOD
        ///     To be called as an executable with command line arguments.
        /// </summary>
        /// <param name="arguments">
        /// The command line arguments.
        /// </param>
        public static void Execute(Arguments arguments)
        {
            Contract.Requires(arguments != null);

            var (analysisSettings, segmentSettings) = arguments.ToAnalysisSettings();
            TimeSpan start = TimeSpan.FromSeconds(arguments.Start ?? 0);
            TimeSpan duration = TimeSpan.FromSeconds(arguments.Duration ?? 0);

            // EXTRACT THE REQUIRED RECORDING SEGMENT
            FileInfo tempF = segmentSettings.SegmentAudioFile;
            if (duration == TimeSpan.Zero)
            {
                // Process entire file
                AudioFilePreparer.PrepareFile(
                    arguments.Source,
                    tempF,
                    new AudioUtilityRequest { TargetSampleRate = ResampleRate },
                    analysisSettings.AnalysisTempDirectoryFallback);
            }
            else
            {
                AudioFilePreparer.PrepareFile(
                    arguments.Source,
                    tempF,
                    new AudioUtilityRequest
                        {
                            TargetSampleRate = ResampleRate,
                            OffsetStart = start,
                            OffsetEnd = start.Add(duration),
                        },
                    analysisSettings.AnalysisTempDirectoryFallback);
            }

            // DO THE ANALYSIS
            /* ############################################################################################################################################# */
            IAnalyser2 analyser = new LitoriaFallax_OBSOLETE();
            //IAnalyser2 analyser = new Canetoad();
            analyser.BeforeAnalyze(analysisSettings);
            AnalysisResult2 result = analyser.Analyze(analysisSettings, segmentSettings);
            /* ############################################################################################################################################# */

            if (result.Events.Length > 0)
            {
                LoggedConsole.WriteLine("{0} events found", result.Events.Length);
            }
            else
            {
                LoggedConsole.WriteLine("No events found");
            }
        }

        public override AnalysisResult2 Analyze<T>(AnalysisSettings analysisSettings, SegmentSettings<T> segmentSettings)
        {
            FileInfo audioFile = segmentSettings.SegmentAudioFile;

            // execute actual analysis
            Dictionary<string, string> configuration = analysisSettings.Configuration;
            LitoriaFallaxResults results = Analysis(audioFile, configuration, segmentSettings.SegmentStartOffset);

            var analysisResults = new AnalysisResult2(analysisSettings, segmentSettings, results.RecordingDuration);

            BaseSonogram sonogram = results.Sonogram;
            double[,] hits = results.Hits;
            Plot scores = results.Plot;
            List<AcousticEvent> predictedEvents = results.Events;

            analysisResults.Events = predictedEvents.ToArray();

            if (analysisSettings.AnalysisDataSaveBehavior)
            {
                this.WriteEventsFile(segmentSettings.SegmentEventsFile, analysisResults.Events);
                analysisResults.EventsFile = segmentSettings.SegmentEventsFile;
            }

            if (analysisSettings.AnalysisDataSaveBehavior)
            {
                var unitTime = TimeSpan.FromMinutes(1.0);
                analysisResults.SummaryIndices = this.ConvertEventsToSummaryIndices(analysisResults.Events, unitTime, analysisResults.SegmentAudioDuration, 0);

                this.WriteSummaryIndicesFile(segmentSettings.SegmentSummaryIndicesFile, analysisResults.SummaryIndices);
            }

            if (analysisSettings.AnalysisImageSaveBehavior.ShouldSave(analysisResults.Events.Length))
            {
                string imagePath = segmentSettings.SegmentImageFile.FullName;
                const double EventThreshold = 0.1;
                Image image = DrawSonogram(sonogram, hits, scores, predictedEvents, EventThreshold);
                image.Save(imagePath, ImageFormat.Png);
                analysisResults.ImageFile = segmentSettings.SegmentImageFile;
            }

            return analysisResults;
        }

        public override void SummariseResults(
            AnalysisSettings settings,
            FileSegment inputFileSegment,
            EventBase[] events,
            SummaryIndexBase[] indices,
            SpectralIndexBase[] spectralIndices,
            AnalysisResult2[] results)
        {
            // noop
        }

        public override void WriteEventsFile(FileInfo destination, IEnumerable<EventBase> results)
        {
            Csv.WriteToCsv(destination, results);
        }

        public override List<FileInfo> WriteSpectrumIndicesFiles(DirectoryInfo destination, string fileNameBase, IEnumerable<SpectralIndexBase> results)
        {
            throw new NotImplementedException();
        }

        public override void WriteSummaryIndicesFile(FileInfo destination, IEnumerable<SummaryIndexBase> results)
        {
            Csv.WriteToCsv(destination, results);
        }

        #endregion

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="segmentOfSourceFile"></param>
        /// <param name="configDict"></param>
        /// <param name="segmentStartOffset"></param>
        /// <returns></returns>
        internal static LitoriaFallaxResults Analysis(FileInfo segmentOfSourceFile, Dictionary<string, string> configDict, TimeSpan segmentStartOffset)
        {
            var recording = new AudioRecording(segmentOfSourceFile.FullName);
            return Analysis(recording, configDict, segmentStartOffset);
        }

        /// <summary>
        /// THE KEY ANALYSIS METHOD
        /// </summary>
        /// <param name="recording">
        ///     The segment Of Source File.
        /// </param>
        /// <param name="configDict">
        ///     The config Dict.
        /// </param>
        /// <param name="value"></param>
        /// <returns>
        /// The <see cref="LitoriaFallaxResults"/>.
        /// </returns>
        internal static LitoriaFallaxResults Analysis(
            AudioRecording recording,
            Dictionary<string, string> configDict,
            TimeSpan segmentStartOffset)
        {
            // WARNING: TODO TODO TODO = this method simply duplicates the CANETOAD analyser!!!!!!!!!!!!!!!!!!!!! ###################

            int minHz = int.Parse(configDict[AnalysisKeys.MinHz]);
            int maxHz = int.Parse(configDict[AnalysisKeys.MaxHz]);

            // BETTER TO CALCULATE THIS. IGNORE USER!
            // double frameOverlap = Double.Parse(configDict[Keys.FRAME_OVERLAP]);

            // duration of DCT in seconds
            double dctDuration = double.Parse(configDict[AnalysisKeys.DctDuration]);

            // minimum acceptable value of a DCT coefficient
            double dctThreshold = double.Parse(configDict[AnalysisKeys.DctThreshold]);

            // ignore oscillations below this threshold freq
            int minOscilFreq = int.Parse(configDict[AnalysisKeys.MinOscilFreq]);

            // ignore oscillations above this threshold freq
            int maxOscilFreq = int.Parse(configDict[AnalysisKeys.MaxOscilFreq]);

            // min duration of event in seconds
            double minDuration = double.Parse(configDict[AnalysisKeys.MinDuration]);

            // max duration of event in seconds
            double maxDuration = double.Parse(configDict[AnalysisKeys.MaxDuration]);

            // min score for an acceptable event
            double eventThreshold = double.Parse(configDict[AnalysisKeys.EventThreshold]);

            // The default was 512 for Canetoad.
            // Framesize = 128 seems to work for Littoria fallax.
            const int FrameSize = 128;
            double windowOverlap = Oscillations2012.CalculateRequiredFrameOverlap(
                recording.SampleRate,
                FrameSize,
                maxOscilFreq);
            //windowOverlap = 0.75; // previous default

            // i: MAKE SONOGRAM
            var sonoConfig = new SonogramConfig
                                 {
                                     SourceFName = recording.BaseName,
                                     WindowSize = FrameSize,
                                     WindowOverlap = windowOverlap,
                                     NoiseReductionType = NoiseReductionType.None,
                                 };

            // sonoConfig.NoiseReductionType = SNR.Key2NoiseReductionType("STANDARD");
            TimeSpan recordingDuration = recording.Duration;
            int sr = recording.SampleRate;
            double freqBinWidth = sr / (double)sonoConfig.WindowSize;

            /* #############################################################################################################################################
             * window    sr          frameDuration   frames/sec  hz/bin  64frameDuration hz/64bins       hz/128bins
             * 1024     22050       46.4ms          21.5        21.5    2944ms          1376hz          2752hz
             * 1024     17640       58.0ms          17.2        17.2    3715ms          1100hz          2200hz
             * 2048     17640       116.1ms          8.6         8.6    7430ms           551hz          1100hz
             */

            // int minBin = (int)Math.Round(minHz / freqBinWidth) + 1;
            // int maxbin = minBin + numberOfBins - 1;
            BaseSonogram sonogram = new SpectrogramStandard(sonoConfig, recording.WavReader);
            int rowCount = sonogram.Data.GetLength(0);
            int colCount = sonogram.Data.GetLength(1);

            // double[,] subMatrix = MatrixTools.Submatrix(sonogram.Data, 0, minBin, (rowCount - 1), maxbin);

            // ######################################################################
            // ii: DO THE ANALYSIS AND RECOVER SCORES OR WHATEVER
            //minDuration = 1.0;
            double[] scores; // predefinition of score array
            List<AcousticEvent> acousticEvents;
            double[,] hits;
            Oscillations2012.Execute(
                (SpectrogramStandard)sonogram,
                minHz,
                maxHz,
                dctDuration,
                minOscilFreq,
                maxOscilFreq,
                dctThreshold,
                eventThreshold,
                minDuration,
                maxDuration,
                out scores,
                out acousticEvents,
                out hits,
                segmentStartOffset);

            acousticEvents.ForEach(ae =>
                    {
                        ae.SpeciesName = configDict[AnalysisKeys.SpeciesName];
                        ae.SegmentStartSeconds = segmentStartOffset.TotalSeconds;
                        ae.SegmentDurationSeconds = recordingDuration.TotalSeconds;
                        ae.Name = AbbreviatedName;
                    });

            var plot = new Plot(AnalysisName, scores, eventThreshold);

            // DEBUG ONLY ################################ TEMPORARY ################################
            // Draw a standard spectrogram and mark of hites etc.
            bool createStandardDebugSpectrogram = true;
            if (createStandardDebugSpectrogram)
            {
                string fileName = "LittoriaFallaxDEBUG";
                throw new NotSupportedException("YOU NEED TO FIX THIS FOR PRODUCTION");
                string path = @"G:\SensorNetworks\Output\Frogs\TestOfHiResIndices-2016July\Test\Towsey.HiResIndices\SpectrogramImages";
                var imageDir = new DirectoryInfo(path);
                if (!imageDir.Exists) imageDir.Create();
                string filePath2 = Path.Combine(imageDir.FullName, fileName + ".png");
                Image sonoBmp = DrawSonogram(sonogram, hits, plot, acousticEvents, eventThreshold);
                sonoBmp.Save(filePath2);
            }
            // END DEBUG ################################ TEMPORARY ################################

            return new LitoriaFallaxResults
            {
                           Sonogram = sonogram,
                           Hits = hits,
                           Plot = plot,
                           Events = acousticEvents,
                           RecordingDuration = recordingDuration,
                       };
        } // Analysis()

        private static Image DrawSonogram(
            BaseSonogram sonogram,
            double[,] hits,
            Plot scores,
            List<AcousticEvent> predictedEvents,
            double eventThreshold)
        {
            const bool DoHighlightSubband = false;
            const bool Add1KHzLines = true;
            var image = new Image_MultiTrack(sonogram.GetImage(DoHighlightSubband, Add1KHzLines));

            ////System.Drawing.Image img = sonogram.GetImage(doHighlightSubband, add1kHzLines);
            ////img.Save(@"C:\SensorNetworks\temp\testimage1.png", System.Drawing.Imaging.ImageFormat.Png);

            ////Image_MultiTrack image = new Image_MultiTrack(img);
            image.AddTrack(ImageTrack.GetTimeTrack(sonogram.Duration, sonogram.FramesPerSecond));
            image.AddTrack(ImageTrack.GetSegmentationTrack(sonogram));
            if (scores != null)
            {
                image.AddTrack(ImageTrack.GetNamedScoreTrack(scores.data, 0.0, 1.0, scores.threshold, scores.title));
            }

            if (hits != null)
            {
                image.OverlayRedTransparency(hits);
            }

            if ((predictedEvents != null) && (predictedEvents.Count > 0))
            {
                image.AddEvents(
                    predictedEvents,
                    sonogram.NyquistFrequency,
                    sonogram.Configuration.FreqBinCount,
                    sonogram.FramesPerSecond);
            }

            return image.GetImage();
        }

        #endregion

        public class Arguments : AnalyserArguments
        {
        }

        public class LitoriaFallaxResults
        {
            #region Public Properties

            public List<AcousticEvent> Events { get; set; }

            public double[,] Hits { get; set; }

            public Plot Plot { get; set; }

            public TimeSpan RecordingDuration { get; set; }

            public BaseSonogram Sonogram { get; set; }

            #endregion
        }
    }
}