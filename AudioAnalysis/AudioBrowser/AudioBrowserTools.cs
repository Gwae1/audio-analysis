﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using System.Linq;
using System.Text;

using System.Threading;
using System.Threading.Tasks;

//using Acoustics.Shared;
//using Acoustics.Tools;
//using Acoustics.Tools.Audio;
using AnalysisBase;

using AnalysisPrograms;
using AudioAnalysisTools;
using TowseyLib;
using Acoustics.Shared;
using Acoustics.Tools.Audio;
using AnalysisRunner;


namespace AudioBrowser
{
    class AudioBrowserTools
    {
        public const int DEFAULT_TRACK_HEIGHT = 10; 

        //TASK IDENTIFIERS
        public const string task_EXTRACT_SEGMENT  = "SOURCE.audio2SEGMENT.wav";
        public const string task_GETSONOGRAM      = "SEGMENT.wav2SONOGRAM.png";
        public const string task_LOAD_CSV         = "INDICES.csv2TRACKSIMAGE.png";

        public const string BROWSER_TITLE_TEXT = "AUDIO-BROWSER: An application for exploring bio-acoustic recordings.  (c) Queensland University of Technology.";
        public const string IMAGE_TITLE_TEXT   = "Image produced by AUDIO-BROWSER, Queensland University of Technology (QUT).";


        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: AudioBrowserTools.Main() must be called with command line arguments.");
            }
            else
            {
                string[] restOfArgs = args.Skip(1).ToArray();
                switch (args[0])
                {
                    case task_EXTRACT_SEGMENT: // extraqcts segment from long audio source file
                        ExtractSegmentFromLongSourceAudioFile(restOfArgs);
                        break;
                    case task_GETSONOGRAM:     // converts segment into sonogram
                        GetSonogramFromAudioFile(restOfArgs);
                        break;
                    case task_LOAD_CSV:        // loads a csv file for visualisation
                        LoadIndicesCsvFileAndDisplayTracksImage(restOfArgs);
                        break;
                    default:
                        Console.WriteLine("Unrecognised task>>> " + args[0]);
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadLine();
                        break;
                }
            }
        } //Main()

        public static void ExtractSegmentFromLongSourceAudioFile(string[] args)
        {
            FileInfo fiSource = new FileInfo(args[0]);
            TimeSpan start    = new TimeSpan(0, 0, Int32.Parse(args[1]));
            TimeSpan end      = new TimeSpan(0, 0, Int32.Parse(args[2]));
            TimeSpan buffer   = new TimeSpan(0, 0, 0);
            int resampleRate  = Int32.Parse(args[3]);
            FileInfo fiOutputSegment = new FileInfo(args[4]);
            ExtractSegment(fiSource, start, end, buffer, resampleRate, fiOutputSegment);
        }
        public static void GetSonogramFromAudioFile(string[] args)
        {
            FileInfo fiAudio  = new FileInfo(args[0]); 
            FileInfo fiConfig = new FileInfo(args[1]);
            FileInfo fiImage  = new FileInfo(args[2]);
            MakeSonogram(fiAudio, fiConfig, fiImage);
        }
        public static int LoadIndicesCsvFileAndDisplayTracksImage(string[] args)
        {
            int status = 0;
            string analyisName = args[0];
            string csvPath     = args[1];
            string configPath  = args[2];
            string imagePath   = args[3]; //Path.Combine(browserSettings.diOutputDir.FullName, (Path.GetFileNameWithoutExtension(csvPath) + ".png"));

            var fiAnalysisConfig = new FileInfo(configPath);
            IAnalysis analyser = AudioBrowserTools.GetAcousticAnalyser(analyisName);
            if (analyser == null)
            {
                //Console.WriteLine("\nWARNING: Could not construct image from CSV file. Analysis name not recognized: " + analyisName);
                return 1;
            }

            var output = analyser.ProcessCsvFile(new FileInfo(csvPath), fiAnalysisConfig);
            if (output == null) return 3;
            //DataTable dtRaw = output.Item1;
            DataTable dt2Display = output.Item2;
            analyser = null;
            bool normalisedDisplay = true;
            Bitmap tracksImage = AudioBrowserTools.ConstructVisualIndexImage(dt2Display, DEFAULT_TRACK_HEIGHT, normalisedDisplay, imagePath);
            return status;
        }

        public static IAnalysis GetAcousticAnalyser(string analysisName)
        {
            IAnalysis analyser = null;
            if ((analysisName==null)||(analysisName.Length == 0))
            {
                Console.WriteLine("ERROR: INVALID ANALYSIS NAME.");
                return null;
            }
            else
            {
                switch (analysisName)
                {
                    case Acoustic.ANALYSIS_NAME:      // 
                        analyser = new Acoustic();
                        break;
                    case LSKiwi2.ANALYSIS_NAME:     // 
                        analyser = new LSKiwi2();
                        break;
                    case "none":     // 
                        break;
                    default:
                        Console.WriteLine("Unrecognised analyis name>>> " + analysisName);
                        break;
                }
            }
            return analyser;
        } //GetAcousticAnalyser()

        public static System.Tuple<DataTable, DataTable> ProcessRecording(FileInfo fiSourceRecording, DirectoryInfo diOutputDir, FileInfo fiConfig)
        {
            var dict = ConfigDictionary.ReadPropertiesFile(fiConfig.FullName);
            string analysisName = dict[Keys.ANALYSIS_NAME];
            string sourceRecordingPath = fiSourceRecording.FullName;
            string outputDir = diOutputDir.FullName;

            double segmentDuration_mins = ConfigDictionary.GetDouble(AudioBrowserSettings.key_SEGMENT_DURATION, dict);
            int segmentOverlap = ConfigDictionary.GetInt(AudioBrowserSettings.key_SEGMENT_OVERLAP, dict);
            int resampleRate = ConfigDictionary.GetInt(AudioBrowserSettings.key_RESAMPLE_RATE, dict);
            var tsSegmentOffset  = new TimeSpan(0, 0, (int)(segmentDuration_mins*60));
            var tsSegmentOverlap = new TimeSpan(0, 0, segmentOverlap);
            var tsSegmentDuration = tsSegmentOffset.Add(tsSegmentOverlap);
            //int segmentDuration_ms = (int)(segmentDuration_mins * 60000) + (segmentOverlap * 1000);


            // CREATE RUN ANALYSIS CLASS HERE
            IAnalysis analyser = GetAcousticAnalyser(analysisName);
            if (analyser == null) 
            {
                Console.WriteLine("#######  CANNOT ANALYSE RECORDING - ANALYSIS TYPE UNKNOWN OR = \"none\".");
                return null;
            }

            //IAudioUtility audioUtility = new MasterAudioUtility(resampleRate); //creates AudioUtility and
            var audioUtility = new MasterAudioUtility(resampleRate, SoxAudioUtility.SoxResampleQuality.VeryHigh);


            var mimeType = MediaTypes.GetMediaType(fiSourceRecording.Extension);
            var sourceDuration = audioUtility.Duration(fiSourceRecording, mimeType);
            int segmentCount = (int)Math.Round(sourceDuration.TotalMinutes / tsSegmentDuration.TotalMinutes); //convert length to minute chunks

            Console.WriteLine("# Source audio - duration: {0}hr:{1}min:{2}s:{3}ms", sourceDuration.Hours, sourceDuration.Minutes, sourceDuration.Seconds, sourceDuration.Milliseconds);
            Console.WriteLine("# Source audio - segments: {0}", segmentCount);

            //##### DO THE ANALYSIS ############ 
            AnalysisSettings settings = new AnalysisSettings();
            settings.AnalysisRunDirectory = diOutputDir;
            settings.AudioFile   = null;
            settings.ConfigFile  = fiConfig;
            settings.ImageFile   = null;
            settings.EventsFile  = null;
            settings.IndicesFile = null;

            //var fSeg = new FileSegment { OriginalFile = fiSourceRecording };
            //IEnumerable<FileSegment> fSegments = LocalSourcePreparer.CalculateSegments(IEnumerable < FileSegment > fileSegments, settings);
            //var results = coord.Run(fSeg, matchingPlugin, settings);

            //SET UP THE OUTPUT REPORT DATATABLE
            DataTable outputDataTable = null;

            // LOOP THROUGH THE FILE
            //initialise timers for diagnostics - ONLY IF IN SEQUENTIAL MODE
            //DateTime tStart = DateTime.Now;
            //DateTime tPrevious = tStart;

            segmentCount = 3;   //for testing and debugging

            for (int s = 0; s < segmentCount; s++)
            // Parallelize the loop to partition the source file by segments.
            //Parallel.For(0, 570, s =>              //USE FOR FIRST HALF OF RECORDING
            //Parallel.For(569, segmentCount, s =>   //USE FOR SECOND HALF OF RECORDING
            //Parallel.For(847, 848, s =>
            //Parallel.For(0, segmentCount, s =>
            {
                //Console.WriteLine(string.Format("Worker threads in use: {0}", GetThreadsInUse()));
                var startTime         = new TimeSpan(0, 0, (int)(s * tsSegmentOffset.TotalSeconds));
                int startMilliseconds = (int)startTime.TotalMilliseconds;
                int endMilliseconds   = startMilliseconds + (int)tsSegmentDuration.TotalMilliseconds;

                #region time diagnostics - used only in sequential loop - no use for parallel loop
                //DateTime tNow = DateTime.Now;
                //TimeSpan elapsedTime = tNow - tStart;
                //string timeDuration = DataTools.Time_ConvertSecs2Mins(elapsedTime.TotalSeconds);
                //double avIterTime = elapsedTime.TotalSeconds / s;
                //if (s == 0) avIterTime = 0.0; //correct for division by zero
                //double t2End = avIterTime * (segmentCount - s) / (double)60;
                //TimeSpan iterTimeSpan = tNow - tPrevious;
                //double iterTime = iterTimeSpan.TotalSeconds;
                //if (s == 0) iterTime = 0.0;
                //tPrevious = tNow;
                //Console.WriteLine("\n");
                //Console.WriteLine("## SEQUENTIAL SAMPLE {0}:  Starts@{1} min.  Elpased time:{2:f1}    E[t2End]:{3:f1} min.   Sec/iteration:{4:f2} (av={5:f2})",
                //                           s, startMinutes, timeDuration, t2End, iterTime, avIterTime);
                #endregion

                //set up the temporary audio segment output file
                string tempFname = "temp" + s + ".wav";
                string tempSegmentPath = Path.Combine(outputDir, tempFname); //path name of the temporary segment files extracted from long recording
                FileInfo fiOutputSegment = new FileInfo(tempSegmentPath);
                //MasterAudioUtility.Segment(resampleRate, fiSourceRecording, fiOutputSegment, startMilliseconds, endMilliseconds);
                MasterAudioUtility.Segment(audioUtility, fiSourceRecording, fiOutputSegment, startMilliseconds, endMilliseconds);

                AudioRecording recordingSegment = new AudioRecording(fiOutputSegment.FullName);

                //double check that recording is over minimum length
                TimeSpan segmentDuration = recordingSegment.GetWavReader().Time;
                int sampleCount = recordingSegment.GetWavReader().Samples.Length; //get recording length to determine if long enough
                int minimumDuration = 30; //seconds
                int minimumSamples = minimumDuration * resampleRate; //ignore recordings shorter than 100 frame
                if (sampleCount <= minimumSamples)
                {
                    Console.WriteLine("# WARNING: Segment @{0}minutes is only {1} samples long (i.e. less than {2} seconds). Will ignore.", startTime.TotalMinutes, sampleCount, minimumDuration);
                    //break;
                }
                else //do analysis
                {
                    //#############################################################################################################################################
                    settings.AudioFile = fiOutputSegment;
                    AnalysisResult result = analyser.Analyse(settings);
                    DataTable dt = result.Data;
                    //#############################################################################################################################################

                    if (dt != null)
                    {
                        if (outputDataTable == null) //create the data table
                        {
                            outputDataTable = dt.Clone();
                        }
                        var headers = new List<string>();
                        foreach (DataColumn col in dt.Columns) headers.Add(col.ColumnName);

                        foreach (DataRow row in dt.Rows)
                        {
                            if (headers.Contains(Keys.SEGMENT_TIMESPAN)) row[Keys.SEGMENT_TIMESPAN] = segmentDuration.TotalSeconds;
                            if (headers.Contains(Keys.EVENT_START_ABS))  row[Keys.EVENT_START_ABS]  = startTime.TotalSeconds + (double)row[Keys.EVENT_START_ABS];
                            if (headers.Contains(Keys.START_MIN))        row[Keys.START_MIN]        = startTime.TotalMinutes;
                            if (headers.Contains(Keys.EVENT_COUNT))      row[Keys.EVENT_COUNT]      = s;
                            if (headers.Contains(Keys.INDICES_COUNT))    row[Keys.INDICES_COUNT]    = s;
                            outputDataTable.ImportRow(row);
                        }
                    } //if (dt != null)
                } // if (sampleCount <= minimumSamples)

                recordingSegment.Dispose();
                File.Delete(tempSegmentPath); //deleted the temp file
                //startTime.Add(tsSegmentOffset);
            } //end of for loop
//            ); // Parallel.For


            //AT THE END OF ANALYSIS NEED TO CONSTRUCT EVENTS AND INDICES DATATABLES
            //different things happen depending on the content of the analysis data table
            if (outputDataTable.Columns.Contains(AudioAnalysisTools.Keys.INDICES_COUNT)) //outputdata consists of rows of one minute indices 
            {
                DataTable eventsDatatable = null;
                return System.Tuple.Create(eventsDatatable, outputDataTable);
            }

            //must have an events data table. Thereofre also create an indices data table
            var unitTime = new TimeSpan(0, 0, 60);
            double scoreThreshold = 0.2;
            DataTable indicesDataTable = analyser.ConvertEvents2Indices(outputDataTable, unitTime, sourceDuration, scoreThreshold); //convert to datatable of indices

            return System.Tuple.Create(outputDataTable, outputDataTable);
        }


        /// <summary>
        /// Analyse multiple files using the same settings.
        /// </summary>
        /// <param name="fileSegments">
        /// The file Segments.
        /// </param>
        /// <param name="analysis">
        /// The analysis.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The results from multiple analyses.
        /// </returns>
        //public IEnumerable<AnalysisResult> Run(IEnumerable<FileSegment> fileSegments, IAnalysis analysis, AnalysisSettings settings)
        //{
        //    var analysisSegments = LocalSourcePreparer.CalculateSegments(fileSegments, settings).ToList();
        //    var analysisSegmentsCount = analysisSegments.Count();

        //    bool DoParallel = true;
        //    if (DoParallel)
        //    {
        //        var results = new AnalysisResult[analysisSegmentsCount];

        //        Parallel.ForEach(
        //            analysisSegments,
        //            (item, state, index) =>
        //            {
        //                var sourceFile = LocalSourcePreparer.PrepareFile(
        //                    settings.AnalysisBaseDirectory,
        //                    item.OriginalFile,
        //                    settings.SegmentMediaType,
        //                    item.SegmentStartOffset.Value,
        //                    item.SegmentEndOffset.Value,
        //                    settings.SegmentTargetSampleRate);

        //                settings.AudioFile = sourceFile;
        //                var result = this.Analyse(analysis, settings);
        //                results[index] = result;
        //            });

        //        return results;
        //    }
        //    else
        //    {
        //        var results = new List<AnalysisResult>();
        //        foreach (var item in analysisSegments)
        //        {
        //            var sourceFile = this.SourcePreparer.PrepareFile(
        //                    settings.AnalysisBaseDirectory,
        //                    item.OriginalFile,
        //                    settings.SegmentMediaType,
        //                    item.SegmentStartOffset.Value,
        //                    item.SegmentEndOffset.Value,
        //                    settings.SegmentTargetSampleRate);

        //            settings.AudioFile = sourceFile;
        //            var result = this.Analyse(analysis, settings);
        //            results.Add(result);
        //        }

        //        return results;
        //    }
        //}


        private static int GetThreadsInUse()
        {
            int availableWorkerThreads, availableCompletionPortThreads, maxWorkerThreads, maxCompletionPortThreads;
            ThreadPool.GetAvailableThreads(out  availableWorkerThreads, out availableCompletionPortThreads);
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
            int threadsInUse = maxWorkerThreads - availableWorkerThreads;

            return threadsInUse;
        }

        public static void ExtractSegment(FileInfo fiSource, TimeSpan start, TimeSpan end, TimeSpan buffer, int resampleRate, FileInfo fiOutputSegment)
        {
            //EXTRACT RECORDING SEGMENT
            int startMilliseconds = (int)(start.TotalMilliseconds - buffer.TotalMilliseconds);
            int endMilliseconds   = (int)(end.TotalMilliseconds   + buffer.TotalMilliseconds);
            if (startMilliseconds < 0) startMilliseconds = 0;
            //if (endMilliseconds <= 0) endMilliseconds = (int)(segmentDuration * 60000) - 1;//no need to worry about end

            //string outputSegmentPath = Path.Combine(browserSettings.diOutputDir.FullName, segmentFName); //path name of the segment file extracted from long recording
            MasterAudioUtility.Segment(resampleRate, fiSource, fiOutputSegment, startMilliseconds, endMilliseconds);
        }


        /// <summary>
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="title"></param>
        /// <param name="timeScale"></param>
        /// <param name="order"></param>
        /// <param name="trackHeight"></param>
        /// <param name="doNormalise"></param>
        /// <returns></returns>
        public static Bitmap ConstructVisualIndexImage(DataTable dt, string title, int timeScale, double[] order, int trackHeight, bool doNormalise)
        {
            List<string> headers = (from DataColumn col in dt.Columns select col.ColumnName).ToList();
            List<double[]> values = DataTableTools.ListOfColumnValues(dt);

            //set up the array of tracks to display
            //var dodisplay = new bool[values.Count];
            //for (int i = 0; i < values.Count; i++) dodisplay[i] = true;
            //if (!(tracks2Display == null))
            //{
            //    for (int i = 0; i < values.Count; i++)
            //        if (i < tracks2Display.Length) dodisplay[i] = tracks2Display[i];
            //}

            // accumulate the individual tracks
            int duration = values[0].Length;    //time in minutes - 1 value = 1 pixel
            int endPanelwidth = 150;
            int imageWidth = duration + endPanelwidth;

            var bitmaps = new List<Bitmap>();
            double threshold = 0.0;
            double[] array;
            for (int i = 0; i < values.Count - 1; i++) //for pixels in the line
            {
                //if ((!dodisplay[i]) || (values[i].Length == 0)) continue;
                if (values[i].Length == 0) continue;
                array = values[i];
                if (doNormalise) array = DataTools.normalise(values[i]);
                bitmaps.Add(Image_Track.DrawBarScoreTrack(order, array, imageWidth, trackHeight, threshold, headers[i]));
            }
            int x = values.Count - 1;
            array = values[x];
            if (doNormalise) array = DataTools.normalise(values[x]);
            //if ((dodisplay[x]) || (values[x].Length > 0))
            if (values[x].Length > 0)
                bitmaps.Add(Image_Track.DrawColourScoreTrack(order, array, imageWidth, trackHeight, threshold, headers[x])); //assumed to be weighted index

            //set up the composite image parameters
            int imageHt = trackHeight * (bitmaps.Count + 3);  //+3 for title and top and bottom time tracks
            Bitmap titleBmp = Image_Track.DrawTitleTrack(imageWidth, trackHeight, title);
            Bitmap timeBmp = Image_Track.DrawTimeTrack(duration, timeScale, imageWidth, trackHeight, "Time (hours)");

            //draw the composite bitmap
            Bitmap compositeBmp = new Bitmap(imageWidth, imageHt); //get canvas for entire image
            Graphics gr = Graphics.FromImage(compositeBmp);
            gr.Clear(Color.Black);

            int offset = 0;
            gr.DrawImage(titleBmp, 0, offset); //draw in the top title
            offset += trackHeight;
            gr.DrawImage(timeBmp, 0, offset); //draw in the top time scale
            offset += trackHeight;
            for (int i = 0; i < bitmaps.Count; i++)
            {
                gr.DrawImage(bitmaps[i], 0, offset);
                offset += trackHeight;
            }
            gr.DrawImage(timeBmp, 0, offset); //draw in bottom time scale
            return compositeBmp;
        }



        public static Bitmap ConstructVisualIndexImage(DataTable dt, int trackHeight, bool doNormalisation, string imagePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(imagePath); 
            string title = String.Format(AudioBrowserTools.IMAGE_TITLE_TEXT + "   SOURCE:{0};  ", fileName);
            int timeScale = 60; //put a tik every 60 pixels = 1 hour
            //construct an order array - this assumes that the table is properly ordered.
            int length = dt.Rows.Count;
            double[] order = new double[length];
            for (int i = 0; i < length; i++) order[i] = i;
            Bitmap tracksImage = ConstructVisualIndexImage(dt, title, timeScale, order, trackHeight, doNormalisation);

            //SAVE THE IMAGE
            //string imagePath = Path.Combine(browserSettings.diOutputDir.FullName, (Path.GetFileNameWithoutExtension(csvPath) + ".png"));
            tracksImage.Save(imagePath);
            Console.WriteLine("\n\tSaved csv data tracks to image file: " + imagePath);
            return tracksImage;
        }

        public static FileInfo InferSourceFileFromCSVFileName(FileInfo csvFile, DirectoryInfo diSourceDir)
        {
            string csvFname = Path.GetFileNameWithoutExtension(csvFile.FullName);
            string[] parts = csvFname.Split('_'); //assume that underscore plus analysis type has been added to source name
            string sourceName = parts[0];
            for (int i = 1; i < parts.Length - 1; i++) sourceName += ("_" + parts[i]);

            var fiSourceMP3 = new FileInfo(Path.Combine(diSourceDir.FullName, sourceName + ".mp3"));
            var fiSourceWAV = new FileInfo(Path.Combine(diSourceDir.FullName, sourceName + ".wav"));
            if (fiSourceMP3.Exists) return fiSourceMP3;
            else
                if (fiSourceWAV.Exists) return fiSourceWAV;
                else
                    return null;
        }


        public static Image GetImageFromAudioSegment(FileInfo fiAudio, FileInfo fiConfig, FileInfo fiImage)
        {
            var config = new ConfigDictionary(fiConfig.FullName); //read in config file

            string analyisName = config.GetString(Keys.ANALYSIS_NAME);
            bool doAnnotate = config.GetBoolean(Keys.ANNOTATE_SONOGRAM);
            bool doNoiseReduction = config.GetBoolean(Keys.NOISE_DO_REDUCTION);
            double bgNoiseThreshold = config.GetDouble(Keys.NOISE_BG_REDUCTION);

            var diOutputDir = new DirectoryInfo(Path.GetDirectoryName(fiImage.FullName));
            Image image = null;

            if (doAnnotate)
            {
                IAnalysis analyser = AudioBrowserTools.GetAcousticAnalyser(analyisName);
                if (analyser == null)
                {
                    Console.WriteLine("\nWARNING: Could not construct image.");
                    Console.WriteLine("\t Analysis name not recognized: " + analyisName);
                    return null;
                }
                AnalysisSettings settings = new AnalysisSettings();
                settings.AudioFile = fiAudio;
                settings.ConfigFile = fiConfig;
                settings.ImageFile = fiImage;
                settings.AnalysisRunDirectory = diOutputDir;
                var results = analyser.Analyse(settings);
                if (results.ImageFile == null) image = null;
                else                           image = Image.FromFile(results.ImageFile.FullName);
                analyser = null;
            }
            else
            {
                image = AudioBrowserTools.MakeSonogram(fiAudio, fiConfig, fiImage);
            }
            return image;
        }



        public static Image MakeSonogram(FileInfo fiAudio, FileInfo fiConfig, FileInfo fiImage)
        {
            var config = new ConfigDictionary(fiConfig.FullName);
            //Dictionary<string, string> configDict = configuration.GetTable();
            bool doNoiseReduction = config.GetBoolean(Keys.NOISE_DO_REDUCTION);
            double bgThreshold = config.GetDouble(Keys.NOISE_BG_REDUCTION);


            AudioRecording recordingSegment = new AudioRecording(fiAudio.FullName);
            SonogramConfig sonoConfig = new SonogramConfig(); //default values config
            sonoConfig.SourceFName = recordingSegment.FileName;
            sonoConfig.WindowSize = 1024;
            sonoConfig.WindowOverlap = 0.0;
            BaseSonogram sonogram = new SpectralSonogram(sonoConfig, recordingSegment.GetWavReader());


            // (iii) NOISE REDUCTION
            if (doNoiseReduction)
            {
                Console.WriteLine("PERFORMING NOISE REDUCTION");
                var tuple = SNR.NoiseReduce(sonogram.Data, NoiseReductionType.STANDARD, bgThreshold);
                sonogram.Data = tuple.Item1;   // store data matrix
            }

            //prepare the image
            bool doHighlightSubband = false;
            bool add1kHzLines = true;
            System.Drawing.Image img = sonogram.GetImage(doHighlightSubband, add1kHzLines);
            Image_MultiTrack mti = new Image_MultiTrack(img);
            mti.AddTrack(Image_Track.GetTimeTrack(sonogram.Duration, sonogram.FramesPerSecond)); //add time scale
            var image = mti.GetImage();

            if (image != null)
                image.Save(fiImage.FullName, ImageFormat.Png);

            return image;
        }//MakeSonogram()


        public static void RunAudacity(string audacityPath, string recordingPath, string dir)
        {
            ProcessRunner process = new ProcessRunner(audacityPath);
            process.Run(recordingPath, dir);
        }// RunAudacity()


    } //AudioBrowserTools
}
