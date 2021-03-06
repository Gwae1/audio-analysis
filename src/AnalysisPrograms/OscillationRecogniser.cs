// <copyright file="OscillationRecogniser.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>
// default options for dev
//od  "C:\SensorNetworks\WavFiles\Canetoad\DM420010_128m_00s__130m_00s - Toads.mp3" C:\SensorNetworks\Output\OD_CaneToad\CaneToad_DetectionParams.txt events.txt

namespace AnalysisPrograms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Acoustics.Shared.Csv;
    using AnalysisPrograms.Production.Arguments;
    using AudioAnalysisTools;
    using AudioAnalysisTools.StandardSpectrograms;
    using AudioAnalysisTools.WavTools;
    using McMaster.Extensions.CommandLineUtils;
    using TowseyLibrary;

    public class OscillationRecogniser
    {
        //Following lines are used for the debug command line.
        //CANETOAD
        //od  "C:\SensorNetworks\WavFiles\Canetoad\DM420011\DM420011_00m_00s__02m_00s.wav" C:\SensorNetworks\Output\OD_CaneToad2_DM420011_NoFilter\OD_CaneToad2_DM420011_NoFilter_Params.txt events.txt
        //od  "C:\SensorNetworks\WavFiles\Canetoad\\FromPaulRoe\canetoad_CubberlaCreek_100529.WAV"  C:\SensorNetworks\Output\OD_CaneToad_PRoe\OD_CaneToad_Params.txt events.txt
        //GECKO
        //od "C:\SensorNetworks\WavFiles\Gecko\Suburban_March2010\geckos_suburban_104.mp3" C:\SensorNetworks\Output\OD_GeckoTrial\OD_GeckoTrial_Params.txt events.txt
        //od "C:\SensorNetworks\WavFiles\Gecko\Gecko05012010\DM420008_26m_00s__28m_00s - Gecko.mp3" C:\SensorNetworks\Output\OD_GeckoTrial\OD_GeckoTrial_Params.txt events.txt
        //KOALA MALE EXHALE
        //od "C:\SensorNetworks\WavFiles\Koala_Male\Recordings\KoalaMale\LargeTestSet\WestKnoll_Bees_20091103-190000.wav" C:\SensorNetworks\Output\OD_KoalaMaleExhale\KoalaMaleExhale_Params.txt events.txt
        //od "C:\SensorNetworks\WavFiles\Koala_Male\SmallTestSet\HoneymoonBay_StBees_20080905-001000.wav" C:\SensorNetworks\Output\OD_KoalaMaleExhale\KoalaMaleExhale_Params.txt events.txt
        //KOALA MALE FOREPLAY
        //od "C:\SensorNetworks\WavFiles\Koala_Male\SmallTestSet\HoneymoonBay_StBees_20080905-001000.wav" C:\SensorNetworks\Output\OD_KoalaMaleForeplay_LargeTestSet\KoalaMaleForeplay_Params.txt events.txt
        //BRIDGE CREEK
        //od "C:\SensorNetworks\WavFiles\Length1_2_4_8_16mins\BridgeCreek_1min.wav" C:\SensorNetworks\Output\TestWavDuration\DurationTest_Params.txt events.txt

        //Keys to recognise identifiers in PARAMETERS - INI file.
        //public static string key_FILE_EXT        = "FILE_EXT";
        public static string Key_DO_SEGMENTATION = "DO_SEGMENTATION";
        public static string Key_MIN_HZ = "MIN_HZ";
        public static string Key_MAX_HZ = "MAX_HZ";
        public static string Key_FRAME_OVERLAP = "FRAME_OVERLAP";
        public static string Key_DCT_DURATION = "DCT_DURATION";
        public static string Key_DCT_THRESHOLD = "DCT_THRESHOLD";
        public static string Key_MIN_OSCIL_FREQ = "MIN_OSCIL_FREQ";
        public static string Key_MAX_OSCIL_FREQ = "MAX_OSCIL_FREQ";
        public static string Key_MIN_DURATION = "MIN_DURATION";
        public static string Key_MAX_DURATION = "MAX_DURATION";
        public static string Key_EVENT_THRESHOLD = "EVENT_THRESHOLD";
        public static string Key_DRAW_SONOGRAMS = "DRAW_SONOGRAMS";

        public static string EventsFile = "events.txt";

        public const string CommandName = "Od";

        [Command(
            CommandName,
            Description = "[UNMAINTAINED] Oscillation Detection. On short files only.")]
        public class Arguments : SourceConfigOutputDirArguments
        {
            public override Task<int> Execute(CommandLineApplication app)
            {
                OscillationRecogniser.Execute(this);
                return this.Ok();
            }
        }

        public static void Execute(Arguments arguments)
        {
            string date = "# DATE AND TIME: " + DateTime.Now;
            Log.WriteLine("# DETECTING LOW FREQUENCY AMPLITUDE OSCILLATIONS");
            Log.WriteLine(date);

            Log.Verbosity = 1;

            FileInfo recordingFile = arguments.Source;
            FileInfo iniPath = arguments.Config.ToFileInfo();
            DirectoryInfo outputDir = arguments.Output;
            string opFName = "OcillationReconiserResults.cav";
            string opPath = outputDir + opFName;
            Log.WriteIfVerbose("# Output folder =" + outputDir);

            //READ PARAMETER VALUES FROM INI FILE
            var config = new ConfigDictionary(iniPath);
            Dictionary<string, string> dict = config.GetTable();
            Dictionary<string, string>.KeyCollection keys = dict.Keys;

            bool doSegmentation = bool.Parse(dict[Key_DO_SEGMENTATION]);
            int minHz = int.Parse(dict[Key_MIN_HZ]);
            int maxHz = int.Parse(dict[Key_MAX_HZ]);
            double frameOverlap = double.Parse(dict[Key_FRAME_OVERLAP]);
            double dctDuration = double.Parse(dict[Key_DCT_DURATION]);       //duration of DCT in seconds
            double dctThreshold = double.Parse(dict[Key_DCT_THRESHOLD]);      //minimum acceptable value of a DCT coefficient
            int minOscilFreq = int.Parse(dict[Key_MIN_OSCIL_FREQ]);      //ignore oscillations below this threshold freq
            int maxOscilFreq = int.Parse(dict[Key_MAX_OSCIL_FREQ]);      //ignore oscillations above this threshold freq
            double minDuration = double.Parse(dict[Key_MIN_DURATION]);       //min duration of event in seconds
            double maxDuration = double.Parse(dict[Key_MAX_DURATION]);       //max duration of event in seconds
            double eventThreshold = double.Parse(dict[Key_EVENT_THRESHOLD]);  //min score for an acceptable event
            int DRAW_SONOGRAMS = int.Parse(dict[Key_DRAW_SONOGRAMS]);      //options to draw sonogram

            Log.WriteIfVerbose("Freq band: {0} Hz - {1} Hz.)", minHz, maxHz);
            Log.WriteIfVerbose("Oscill bounds: " + minOscilFreq + " - " + maxOscilFreq + " Hz");
            Log.WriteIfVerbose("minAmplitude = " + dctThreshold);
            Log.WriteIfVerbose("Duration bounds: " + minDuration + " - " + maxDuration + " seconds");

            //#############################################################################################################################################
            var results = Execute_ODDetect(
                recordingFile,
                doSegmentation,
                minHz,
                maxHz,
                frameOverlap,
                dctDuration,
                dctThreshold,
                minOscilFreq,
                maxOscilFreq,
                eventThreshold,
                minDuration,
                maxDuration);
            Log.WriteLine("# Finished detecting oscillation events.");

            //#############################################################################################################################################

            var sonogram = results.Item1;
            var hits = results.Item2;
            var scores = results.Item3;
            var predictedEvents = results.Item4;
            var intensity = results.Item5;
            var analysisDuration = results.Item6;
            Log.WriteLine("# Event Count = " + predictedEvents.Count());
            int pcHIF = 100;
            if (intensity != null)
            {
                int hifCount = intensity.Count(p => p >= 0.001); //count of high intensity frames
                pcHIF = 100 * hifCount / sonogram.FrameCount;
            }

            // write event count to results file.
            string fname = recordingFile.BaseName();

            Csv.WriteToCsv(opPath.ToFileInfo(), predictedEvents);

            //draw images of sonograms
            string imagePath = outputDir + fname + ".png";
            if (DRAW_SONOGRAMS == 2)
            {
                DrawSonogram(sonogram, imagePath, hits, scores, predictedEvents, eventThreshold, intensity);
            }
            else
            if (DRAW_SONOGRAMS == 1 && predictedEvents.Count > 0)
            {
                DrawSonogram(sonogram, imagePath, hits, scores, predictedEvents, eventThreshold, intensity);
            }

            Log.WriteLine("# Finished recording:- " + recordingFile.Name);
        }

        public static Tuple<BaseSonogram, double[,], double[], List<AcousticEvent>, double[], TimeSpan> Execute_ODDetect(
            FileInfo wavPath,
            bool doSegmentation, int minHz, int maxHz, double frameOverlap, double dctDuration, double dctThreshold, int minOscilFreq, int maxOscilFreq,
            double eventThreshold, double minDuration, double maxDuration)
        {
            //i: GET RECORDING
            AudioRecording recording = new AudioRecording(wavPath.FullName);

            //if (recording.SampleRate != 22050) recording.ConvertSampleRate22kHz(); // THIS METHOD CALL IS OBSOLETE
            int sr = recording.SampleRate;

            //ii: MAKE SONOGRAM
            Log.WriteLine("Start sonogram.");
            SonogramConfig sonoConfig = new SonogramConfig(); //default values config
            sonoConfig.WindowOverlap = frameOverlap;
            sonoConfig.SourceFName = recording.BaseName;
            BaseSonogram sonogram = new SpectrogramStandard(sonoConfig, recording.WavReader);

            Log.WriteLine("Signal: Duration={0}, Sample Rate={1}", sonogram.Duration, sr);
            Log.WriteLine(
                "Frames: Size={0}, Count={1}, Duration={2:f1}ms, Overlap={5:f0}%, Offset={3:f1}ms, Frames/s={4:f1}",
                sonogram.Configuration.WindowSize,
                sonogram.FrameCount,
                sonogram.FrameDuration * 1000,
                sonogram.FrameStep * 1000,
                sonogram.FramesPerSecond,
                frameOverlap);
            int binCount = (int)(maxHz / sonogram.FBinWidth) - (int)(minHz / sonogram.FBinWidth) + 1;
            Log.WriteIfVerbose("Freq band: {0} Hz - {1} Hz. (Freq bin count = {2})", minHz, maxHz, binCount);

            Log.WriteIfVerbose("DctDuration=" + dctDuration + "sec.  (# frames=" + (int)Math.Round(dctDuration * sonogram.FramesPerSecond) + ")");
            Log.WriteIfVerbose("Score threshold for oscil events=" + eventThreshold);
            Log.WriteLine("Start OD event detection");

            //iii: DETECT OSCILLATIONS
            bool normaliseDCT = true;
            Oscillations2010.Execute((SpectrogramStandard)sonogram, doSegmentation, minHz, maxHz, dctDuration, dctThreshold, normaliseDCT,
                                         minOscilFreq, maxOscilFreq, eventThreshold, minDuration, maxDuration,
                                         out var scores, out var predictedEvents, out var hits, out var segments, out var analysisTime);

            return Tuple.Create(sonogram, hits, scores, predictedEvents, segments, analysisTime);
        }

        public static void DrawSonogram(BaseSonogram sonogram, string path, double[,] hits, double[] scores,
                                        List<AcousticEvent> predictedEvents, double eventThreshold, double[] intensity)
        {
            Log.WriteLine("# Start to draw image of sonogram.");
            bool doHighlightSubband = false;
            bool add1kHzLines = true;

            //double maxScore = 50.0; //assumed max posisble oscillations per second

            using (var img = sonogram.GetImage(doHighlightSubband, add1kHzLines, doMelScale: false))
            using (Image_MultiTrack image = new Image_MultiTrack(img))
            {
                //img.Save(@"C:\SensorNetworks\WavFiles\temp1\testimage1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                image.AddTrack(ImageTrack.GetTimeTrack(sonogram.Duration, sonogram.FramesPerSecond));
                image.AddTrack(ImageTrack.GetSegmentationTrack(sonogram));
                image.AddTrack(ImageTrack.GetScoreTrack(scores, 0.0, 1.0, eventThreshold));

                //double maxScore = 100.0;
                //image.AddSuperimposedMatrix(hits, maxScore);
                if (intensity != null)
                {
                    DataTools.MinMax(intensity, out var min, out var max);
                    double threshold_norm = eventThreshold / max; //min = 0.0;
                    intensity = DataTools.normalise(intensity);
                    image.AddTrack(ImageTrack.GetScoreTrack(intensity, 0.0, 1.0, eventThreshold));
                }

                image.AddEvents(predictedEvents, sonogram.NyquistFrequency, sonogram.Configuration.FreqBinCount, sonogram.FramesPerSecond);
                image.Save(path);
            }
        }
    }
}