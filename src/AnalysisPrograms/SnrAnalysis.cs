// <copyright file="SnrAnalysis.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace AnalysisPrograms
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Acoustics.Shared.ConfigFile;
    using Acoustics.Tools;
    using AnalysisPrograms.Production.Arguments;
    using AudioAnalysisTools;
    using AudioAnalysisTools.DSP;
    using AudioAnalysisTools.StandardSpectrograms;
    using AudioAnalysisTools.WavTools;
    using McMaster.Extensions.CommandLineUtils;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using TowseyLibrary;
    using Path = System.IO.Path;

    public class SnrAnalysis
    {
        public const string CommandName = "SnrAnalysis";

        [Command(
            CommandName,
            Description = "[UNMAINTAINED] Calculates signal to noise ratio for short files.")]
        public class Arguments : SourceConfigOutputDirArguments
        {
            public override Task<int> Execute(CommandLineApplication app)
            {
                SnrAnalysis.Execute(this);
                return this.Ok();
            }
        }

        public static void Execute(Arguments arguments)
        {
            const string Title = "# DETERMINING SIGNAL TO NOISE RATIO IN RECORDING";
            string date = "# DATE AND TIME: " + DateTime.Now;
            Log.WriteLine(Title);
            Log.WriteLine(date);
            Log.Verbosity = 1;

            var input = arguments.Source;
            var sourceFileName = input.Name;
            var outputDir = arguments.Output;
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(input.FullName);
            var outputTxtPath = Path.Combine(outputDir.FullName, fileNameWithoutExtension + ".txt").ToFileInfo();

            Log.WriteIfVerbose("# Recording file: " + input.FullName);
            Log.WriteIfVerbose("# Config file:    " + arguments.Config);
            Log.WriteIfVerbose("# Output folder =" + outputDir.FullName);
            FileTools.WriteTextFile(outputTxtPath.FullName, date + "\n# Recording file: " + input.FullName);

            //READ PARAMETER VALUES FROM INI FILE
            // load YAML configuration
            Config configuration = ConfigFile.Deserialize(arguments.Config);

            //ii: SET SONOGRAM CONFIGURATION
            SonogramConfig sonoConfig = new SonogramConfig(); //default values config
            sonoConfig.SourceFName = input.FullName;
            sonoConfig.WindowSize = configuration.GetIntOrNull(AnalysisKeys.KeyFrameSize) ?? 512;
            sonoConfig.WindowOverlap = configuration.GetDoubleOrNull(AnalysisKeys.FrameOverlap) ?? 0.5;
            sonoConfig.WindowFunction = configuration[AnalysisKeys.KeyWindowFunction];
            sonoConfig.NPointSmoothFFT = configuration.GetIntOrNull(AnalysisKeys.KeyNPointSmoothFft) ?? 256;
            sonoConfig.NoiseReductionType = SNR.KeyToNoiseReductionType(configuration[AnalysisKeys.NoiseReductionType]);

            int minHz = configuration.GetIntOrNull("MIN_HZ") ?? 0;
            int maxHz = configuration.GetIntOrNull("MAX_HZ") ?? 11050;
            double segK1 = configuration.GetDoubleOrNull("SEGMENTATION_THRESHOLD_K1") ?? 0;
            double segK2 = configuration.GetDoubleOrNull("SEGMENTATION_THRESHOLD_K2") ?? 0;
            double latency = configuration.GetDoubleOrNull("K1_K2_LATENCY") ?? 0;
            double vocalGap = configuration.GetDoubleOrNull("VOCAL_GAP") ?? 0;
            double minVocalLength = configuration.GetDoubleOrNull("MIN_VOCAL_DURATION") ?? 0;

            //bool DRAW_SONOGRAMS = (bool?)configuration.DrawSonograms ?? true;    //options to draw sonogram

            //double intensityThreshold = Acoustics.AED.Default.intensityThreshold;
            //if (dict.ContainsKey(key_AED_INTENSITY_THRESHOLD)) intensityThreshold = Double.Parse(dict[key_AED_INTENSITY_THRESHOLD]);
            //int smallAreaThreshold = Acoustics.AED.Default.smallAreaThreshold;
            //if( dict.ContainsKey(key_AED_SMALL_AREA_THRESHOLD))   smallAreaThreshold = Int32.Parse(dict[key_AED_SMALL_AREA_THRESHOLD]);

            // COnvert input recording into wav
            var convertParameters = new AudioUtilityRequest { TargetSampleRate = 17640 };
            var fileToAnalyse = new FileInfo(Path.Combine(outputDir.FullName, "temp.wav"));

            if (File.Exists(fileToAnalyse.FullName))
            {
                File.Delete(fileToAnalyse.FullName);
            }

            var convertedFileInfo = AudioFilePreparer.PrepareFile(
                input,
                fileToAnalyse,
                convertParameters,
                outputDir);

            // (A) ##########################################################################################################################
            AudioRecording recording = new AudioRecording(fileToAnalyse.FullName);
            int signalLength = recording.WavReader.Samples.Length;
            TimeSpan wavDuration = TimeSpan.FromSeconds(recording.WavReader.Time.TotalSeconds);
            double frameDurationInSeconds = sonoConfig.WindowSize / (double)recording.SampleRate;
            TimeSpan frameDuration = TimeSpan.FromTicks((long)(frameDurationInSeconds * TimeSpan.TicksPerSecond));
            int stepSize = (int)Math.Floor(sonoConfig.WindowSize * (1 - sonoConfig.WindowOverlap));
            double stepDurationInSeconds = sonoConfig.WindowSize * (1 - sonoConfig.WindowOverlap)
                                           / recording.SampleRate;
            TimeSpan stepDuration = TimeSpan.FromTicks((long)(stepDurationInSeconds * TimeSpan.TicksPerSecond));
            double framesPerSecond = 1 / stepDuration.TotalSeconds;
            int frameCount = signalLength / stepSize;

            // (B) ################################## EXTRACT ENVELOPE and SPECTROGRAM ##################################
            var dspOutput = DSP_Frames.ExtractEnvelopeAndFfts(
                recording,
                sonoConfig.WindowSize,
                sonoConfig.WindowOverlap);

            //double[] avAbsolute = dspOutput.Average; //average absolute value over the minute recording

            // (C) ################################## GET SIGNAL WAVEFORM ##################################
            double[] signalEnvelope = dspOutput.Envelope;
            double avSignalEnvelope = signalEnvelope.Average();

            // (D) ################################## GET Amplitude Spectrogram ##################################
            double[,] amplitudeSpectrogram = dspOutput.AmplitudeSpectrogram; // get amplitude spectrogram.

            // (E) ################################## Generate deciBel spectrogram from amplitude spectrogram
            double epsilon = Math.Pow(0.5, recording.BitsPerSample - 1);
            double[,] deciBelSpectrogram = MFCCStuff.DecibelSpectra(
                dspOutput.AmplitudeSpectrogram,
                dspOutput.WindowPower,
                recording.SampleRate,
                epsilon);

            LoggedConsole.WriteLine("# Finished calculating decibel spectrogram.");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\nSIGNAL PARAMETERS");
            sb.AppendLine("Signal Duration     =" + wavDuration);
            sb.AppendLine("Sample Rate         =" + recording.SampleRate);
            sb.AppendLine("Min Signal Value    =" + dspOutput.MinSignalValue);
            sb.AppendLine("Max Signal Value    =" + dspOutput.MaxSignalValue);
            sb.AppendLine("Max Absolute Ampl   =" + signalEnvelope.Max().ToString("F3") + "  (See Note 1)");
            sb.AppendLine("Epsilon Ampl (1 bit)=" + epsilon);

            sb.AppendLine("\nFRAME PARAMETERS");
            sb.AppendLine("Window Size    =" + sonoConfig.WindowSize);
            sb.AppendLine("Frame Count    =" + frameCount);
            sb.AppendLine("Envelope length=" + signalEnvelope.Length);
            sb.AppendLine("Frame Duration =" + frameDuration.TotalMilliseconds.ToString("F3") + " ms");
            sb.AppendLine("Frame overlap  =" + sonoConfig.WindowOverlap);
            sb.AppendLine("Step Size      =" + stepSize);
            sb.AppendLine("Step duration  =" + stepDuration.TotalMilliseconds.ToString("F3") + " ms");
            sb.AppendLine("Frames Per Sec =" + framesPerSecond.ToString("F1"));

            sb.AppendLine("\nFREQUENCY PARAMETERS");
            sb.AppendLine("Nyquist Freq    =" + dspOutput.NyquistFreq + " Hz");
            sb.AppendLine("Freq Bin Width  =" + dspOutput.FreqBinWidth.ToString("F2") + " Hz");
            sb.AppendLine("Nyquist Bin     =" + dspOutput.NyquistBin);

            sb.AppendLine("\nENERGY PARAMETERS");
            double val = dspOutput.FrameEnergy.Min();
            sb.AppendLine(
                "Minimum dB / frame       =" + (10 * Math.Log10(val)).ToString("F2") + "  (See Notes 2, 3 & 4)");
            val = dspOutput.FrameEnergy.Max();
            sb.AppendLine("Maximum dB / frame       =" + (10 * Math.Log10(val)).ToString("F2"));

            sb.AppendLine("\ndB NOISE SUBTRACTION");
            double noiseRange = 2.0;

            //sb.AppendLine("Noise (estimate of mode) =" + sonogram.SnrData.NoiseSubtracted.ToString("F3") + " dB   (See Note 5)");
            //double noiseSpan = sonogram.SnrData.NoiseRange;
            //sb.AppendLine("Noise range              =" + noiseSpan.ToString("F2") + " to +" + (noiseSpan * -1).ToString("F2") + " dB   (See Note 6)");
            //sb.AppendLine("SNR (max frame-noise)    =" + sonogram.SnrData.Snr.ToString("F2") + " dB   (See Note 7)");

            //sb.Append("\nSEGMENTATION PARAMETERS");
            //sb.Append("Segment Thresholds K1: {0:f2}.  K2: {1:f2}  (See Note 8)", segK1, segK2);
            //sb.Append("# Event Count = " + predictedEvents.Count());

            FileTools.Append2TextFile(outputTxtPath.FullName, sb.ToString());
            FileTools.Append2TextFile(outputTxtPath.FullName, GetSNRNotes(noiseRange).ToString());

            // (F) ################################## DRAW IMAGE 1: original spectorgram
            Log.WriteLine("# Start drawing noise reduced sonograms.");
            TimeSpan X_AxisInterval = TimeSpan.FromSeconds(1);

            //int Y_AxisInterval = (int)Math.Round(1000 / dspOutput.FreqBinWidth);
            int nyquist = recording.SampleRate / 2;
            int hzInterval = 1000;

            var image1 = DrawSonogram(deciBelSpectrogram, wavDuration, X_AxisInterval, stepDuration, nyquist, hzInterval);

            // (G) ################################## Calculate modal background noise spectrum in decibels
            //double SD_COUNT = -0.5; // number of SDs above the mean for noise removal
            //NoiseReductionType nrt = NoiseReductionType.MODAL;
            //System.Tuple<double[,], double[]> tuple = SNR.NoiseReduce(deciBelSpectrogram, nrt, SD_COUNT);

            //double upperPercentileBound = 0.2;    // lowest percentile for noise removal
            //NoiseReductionType nrt = NoiseReductionType.LOWEST_PERCENTILE;
            //System.Tuple<double[,], double[]> tuple = SNR.NoiseReduce(deciBelSpectrogram, nrt, upperPercentileBound);

            // (H) ################################## Calculate BRIGGS noise removal from amplitude spectrum
            int percentileBound = 20; // low energy percentile for noise removal

            //double binaryThreshold   = 0.6;   //works for higher SNR recordings
            double binaryThreshold = 0.4; //works for lower SNR recordings

            //double binaryThreshold = 0.3;   //works for lower SNR recordings
            double[,] m = NoiseRemoval_Briggs.BriggsNoiseFilterAndGetMask(
                amplitudeSpectrogram,
                percentileBound,
                binaryThreshold);

            string title = "TITLE";
            var image2 = NoiseRemoval_Briggs.DrawSonogram(
                m,
                wavDuration,
                X_AxisInterval,
                stepDuration,
                nyquist, hzInterval,
                title);

            //Image image2 = NoiseRemoval_Briggs.BriggsNoiseFilterAndGetSonograms(amplitudeSpectrogram, upperPercentileBound, binaryThreshold,
            //                                                                          wavDuration, X_AxisInterval, stepDuration, Y_AxisInterval);

            // (I) ################################## Calculate MEDIAN noise removal from amplitude spectrum

            //double upperPercentileBound = 0.8;    // lowest percentile for noise removal
            //NoiseReductionType nrt = NoiseReductionType.MEDIAN;
            //System.Tuple<double[,], double[]> tuple = SNR.NoiseReduce(deciBelSpectrogram, nrt, upperPercentileBound);

            //double[,] noiseReducedSpectrogram1 = tuple.Item1;  //
            //double[] noiseProfile              = tuple.Item2;  // smoothed modal profile

            //SNR.NoiseProfile dBProfile = SNR.CalculateNoiseProfile(deciBelSpectrogram, SD_COUNT);       // calculate noise value for each freq bin.
            //double[] noiseProfile = DataTools.filterMovingAverage(dBProfile.noiseThresholds, 7);        // smooth modal profile
            //double[,] noiseReducedSpectrogram1 = SNR.TruncateBgNoiseFromSpectrogram(deciBelSpectrogram, dBProfile.noiseThresholds);
            //Image image2 = DrawSonogram(noiseReducedSpectrogram1, wavDuration, X_AxisInterval, stepDuration, Y_AxisInterval);

            var combinedImage = ImageTools.CombineImagesVertically(image1, image2);

            string imagePath = Path.Combine(outputDir.FullName, fileNameWithoutExtension + ".png");
            combinedImage.Save(imagePath);

            // (G) ################################## Calculate modal background noise spectrum in decibels

            Log.WriteLine("# Finished recording:- " + input.Name);
        }

        private static Image<Rgb24> DrawSonogram(
            double[,] data,
            TimeSpan recordingDuration,
            TimeSpan xInterval,
            TimeSpan xAxisPixelDuration,
            int nyquist,
            int hzInterval)
        {
            var image = ImageTools.GetMatrixImage(data);
            string title = string.Format("TITLE");
            var titleBar = BaseSonogram.DrawTitleBarOfGrayScaleSpectrogram(title, image.Width);
            var minuteOffset = TimeSpan.Zero;
            var labelInterval = TimeSpan.FromSeconds(5);
            image = BaseSonogram.FrameSonogram(
                image,
                titleBar,
                minuteOffset,
                xInterval,
                xAxisPixelDuration,
                labelInterval,
                nyquist,
                hzInterval);

            return image;

            //USE THIS CODE TO RETURN COMPRESSED SONOGRAM
            //int factor = 10;  //compression factor
            //using (var image3 = new Image_MultiTrack(sonogram.GetImage_ReducedSonogram(factor)))
            //{
            //image3.AddTrack(ImageTrack.GetTimeTrack(sonogram.Duration));
            //image3.AddTrack(ImageTrack.GetWavEnvelopeTrack(recording, image3.Image.Width));
            //image3.AddTrack(ImageTrack.GetDecibelTrack(sonogram));
            //image3.AddTrack(ImageTrack.GetSegmentationTrack(sonogram));
            //path = outputFolder + wavFileName + "_reduced.png"
            //image3.Save(path);
            //}

            //DISPLAY IMAGE SUB BAND HIGHLIGHT and SNR DATA
            //doHighlightSubband = true;
            //var image4 = new Image_MultiTrack(sonogram.GetImage(doHighlightSubband, add1kHzLines));
            //image4.AddTrack(ImageTrack.GetTimeTrack(sonogram.Duration));
            ////image4.AddTrack(ImageTrack.GetWavEnvelopeTrack(recording, image4.SonoImage.Width));
            //image4.AddTrack(ImageTrack.GetSegmentationTrack(sonogram));
            ////path = outputFolder + wavFileName + "_subband.png"
            //image4.Save(path);
        }

        private static void DrawWaveforms(AudioRecording recording, string path)
        {
            int imageWidth = 284;
            int imageHeight = 60;
            var image2 = new Image_MultiTrack(recording.GetWaveForm(imageWidth, imageHeight));

            //path = outputFolder + wavFileName + "_waveform.png";
            image2.Save(path);

            double dBMin = -25.0; //-25 dB appear to be good value
            var image6 = new Image_MultiTrack(recording.GetWaveFormInDecibels(imageWidth, imageHeight, dBMin));

            //path = outputFolder + wavFileName + "_waveformDB.png"
            image6.Save(path);
        }

        public static StringBuilder GetSNRNotes(double noiseRange)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\nSIGNAL PARAMETERS");
            sb.AppendLine("\n\tNote 1:      Signal samples take values between -1.0 and +1.0");
            sb.AppendLine(
                "\n\tNote 2:      The acoustic power per frame is calculated in decibels: dB = 10 * log(Frame Energy)");
            sb.AppendLine(
                "\t             where Frame Energy = average of the amplitude squared of all 512 values in a frame.");
            sb.AppendLine(
                "\n\tNote 3:      At this stage all dB values are <= 0.0. A dB value = 0.0 could only occur if the average frame amplitude = 1.0");
            sb.AppendLine("\t             Typically, highest amplitudes occur in gusting wind.");
            sb.AppendLine(
                "\t             Highest av. frame amplitude we have encountered due to animal source was 0.55 due to nearby cicada.");
            sb.AppendLine(
                "\n\tNote 4:        A minimum value for dB is truncated at -80 dB, which allows for very quiet background noise.");
            sb.AppendLine(
                "\t             A typical background noise dB value for Brisbane Airport (BAC2) recordings is -45 dB.");
            sb.AppendLine("\t             Log energy values are converted to decibels by multiplying by 10.");
            sb.AppendLine(
                "\n\tNote 5:      The modal background noise per frame is calculated using an algorithm of Lamel et al, 1981, called 'Adaptive Level Equalisatsion'.");
            sb.AppendLine(
                "\t             Subtracting this value from each frame dB value sets the modal background noise level to 0 dB. Values < 0.0 are clipped to 0.0 dB.");
            sb.AppendLine(
                "\n\tNote 6:      The modal noise level is now 0 dB but the noise ranges " + noiseRange.ToString("F2")
                + " dB either side of zero.");
            sb.AppendLine(
                "\n\tNote 7:      Here are some dB comparisons. NOTE! They are with reference to the auditory threshold at 1 kHz.");
            sb.AppendLine(
                "\t             Our estimates of SNR are with respect to background environmental noise which is typically much higher than hearing threshold!");
            sb.AppendLine("\t             Leaves rustling, calm breathing:  10 dB");
            sb.AppendLine("\t             Very calm room:                   20 - 30 dB");
            sb.AppendLine("\t             Normal talking at 1 m:            40 - 60 dB");
            sb.AppendLine("\t             Major road at 10 m:               80 - 90 dB");
            sb.AppendLine("\t             Jet at 100 m:                    110 -140 dB");
            sb.AppendLine(
                "\n\tNote 8:      dB above the background (modal) noise, which has been set to zero dB. These thresholds are used to segment acoustic events.");
            sb.AppendLine("\n");
            return sb;
        }
    } //end class
}