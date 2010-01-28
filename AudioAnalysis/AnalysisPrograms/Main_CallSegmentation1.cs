﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TowseyLib;
using AudioTools;
using AudioAnalysisTools;


namespace AnalysisPrograms
{
    class Main_CallSegmentation1
    {


        public static void Main(string[] args)
        {
            Console.WriteLine("DATE AND TIME:" + DateTime.Now);
            Console.WriteLine("");


            //#######################################################################################################
            // KEY PARAMETERS TO CHANGE
            //Log.Verbosity = 1;
            //#######################################################################################################

            //string wavDirName = @"C:\SensorNetworks\Templates\Template_3\TrainingSet1";
            string wavDirName = @"C:\SensorNetworks\Templates\Template_CURLEW1\data\train";
            if (args.Length == 0) 
            {
                if (!Directory.Exists(wavDirName))
                {
                    Console.WriteLine("YOU NEED A COMMAND LINE ARGUEMENT!");
                    Usage();
                    throw new Exception("DIRECTORY DOES NOT EXIST:" + wavDirName + "  FATAL ERROR!");
                }
            }
            else
                if (args.Length > 0)
                {
                    wavDirName = args[0];
                    if (!Directory.Exists(wavDirName))
                    {
                        Usage();
                        Console.WriteLine("DIRECTORY DOES NOT EXIST:" + wavDirName + "  FATAL ERROR!");
                        throw new Exception("DIRECTORY DOES NOT EXIST:" + wavDirName + "  FATAL ERROR!");
                    }
                }
            string outputDir      = wavDirName;  //put output into same dir as vocalisations

            string segmentIniPath = wavDirName + "\\segmentation.ini";
            if (!File.Exists(segmentIniPath))
            {
                Usage();
                throw new Exception(".INI FILE DOES NOT EXIST:" + segmentIniPath + "  FATAL ERROR!");
            }

            //A: SET UP THE CONFIG VALUES.
            SonogramConfig config1 = null; 
            Console.WriteLine("INIT SONOGRAM CONFIG: " + segmentIniPath);
            try
            {
                config1 = new SonogramConfig(new Configuration(segmentIniPath));
            }
            catch(Exception e)
            {
                Console.WriteLine("ERROR: Error initialising the SonogramConfig() class");
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("SEEKING SILENCE FILE: " + config1.SilenceRecordingPath);
            if (!File.Exists(config1.SilenceRecordingPath))
            {
                throw new Exception("FILE FOR SILENCE MODEL DOES NOT EXIST:<" + config1.SilenceRecordingPath + ">  FATAL ERROR!");
            }

            //B: set up file name to store noise model 
            Console.WriteLine("Noise/silence model to be derived from a .wav file");
            string noiseFileDir = Path.GetDirectoryName(config1.SilenceRecordingPath);
            if (noiseFileDir == wavDirName)
            {
                Console.WriteLine("WARNING! DO NOT PLACE THE NOISE/SILENCE FILE IN SAME DIRECTORY AS VOCALISATIONS.");
                throw new Exception("  SHIFT NOISE FILE TO DIFFERENT DIRECTORY. FATAL ERROR!");
            }

            string noiseModelExt = ".noiseModel";
            string noiseModelPath = noiseFileDir + "\\" + Path.GetFileNameWithoutExtension(config1.SilenceRecordingPath) + noiseModelExt;
            Console.WriteLine("segmentIniPath  =" + segmentIniPath);
            Console.WriteLine("wav Dir         =" + wavDirName);
            Console.WriteLine("outputDir       =" + outputDir);
            Console.WriteLine("noise Model File=" + config1.SilenceRecordingPath);


            //C: EXTRACT THE NOISE MODEL FROM SILENCE AUDIO FILE IF .noisemodel file does not exist.
            if (!File.Exists(noiseModelPath))
            {
                ConfigKeys.NoiseReductionType requestedNoiseReductionType = config1.NoiseReductionType;//store requested noise reduction type
                config1.NoiseReductionType = ConfigKeys.NoiseReductionType.STANDARD;

                Console.WriteLine("READING WAV FILE");
                var wr = new WavReader(config1.SilenceRecordingPath);
                Console.WriteLine("MAKING SONOGRAM");
                var s = new SpectralSonogram(config1, wr);

                //C1: Make noise/silence model, write to file and store in config
                FileInfo f1 = new FileInfo(config1.SilenceRecordingPath);
                var noiseModel = s.SnrFrames.ModalNoiseProfile;
                //DataTools.writeBarGraph(noiseModel);
                Console.WriteLine("SILENCE MODEL IN " + noiseModelPath);
                FileTools.WriteArray2File_Formatted(noiseModel, noiseModelPath, "F6");

                //C2: change CONFIGURATION to the SILENCE MODEL
                config1.NoiseReductionType = requestedNoiseReductionType;

                //show consequence of the noise model on file from which model derived
                config1.SilenceModel = FileTools.ReadDoubles2Vector(noiseModelPath);
                Segment(f1, config1, noiseFileDir);
            }

            //D: READ FILE CONTAINING NOISE PROFILE
            config1.SilenceModel = FileTools.ReadDoubles2Vector(noiseModelPath);

            //E: NOISE REDUCE ALL RECORDINGS IN DIR
            //Get List of Vocalisation Recordings - either paths or URIs
            string ext = ".wav";
            FileInfo[] recordingFiles = FileTools.GetFilesInDirectory(wavDirName, ext);
            if (recordingFiles.Length == 0)
            {
                throw new Exception("THERE ARE NO WAV FILES IN DESIGNATED DIRECTORY:" + wavDirName + "  FATAL ERROR!");
            }

            Console.WriteLine("Number of recordings = " + recordingFiles.Length);
            Log.Verbosity = 0;
            int count = 0;
            //double avDuration = 0.0; //to determine average duration test vocalisations
            foreach (FileInfo f in recordingFiles)
            {
                Log.Verbosity = 1;
                count++;
                Log.WriteIfVerbose("\n" + count + " ######  RECORDING= " + f.Name);
                Segment(f, config1, outputDir);
            } //end of all training vocalisations

            //Console.WriteLine("\nAv Duration = " + (avDuration / count));
            Console.WriteLine("\nFINISHED AUDIO SEGMENTATION!");
            //Console.ReadLine();
        } //end method Main()



        public static void Segment(FileInfo f, SonogramConfig config, string outputDir)
        {
            //Make sonogram of each recording
            AudioRecording recording = new AudioRecording(f.FullName);
            var wr = recording.GetWavReader();
            var ss = new SpectralSonogram(config, wr);
            var image = new Image_MultiTrack(ss.GetImage(false, false));
            image.AddTrack(Image_Track.GetTimeTrack(ss.Duration));
            image.AddTrack(Image_Track.GetSegmentationTrack(ss));
            string path1 = outputDir + "\\" + Path.GetFileNameWithoutExtension(f.Name) + ".png";
            image.Save(path1);

            string path2 = outputDir + "\\" + Path.GetFileNameWithoutExtension(f.Name) + ".segmentation.txt";
            StringBuilder sb = ss.GetSegmentationText();
            FileTools.WriteTextFile(path2, sb.ToString());
        }


        public static void Usage()
        {
            Console.WriteLine("USAGE: VocalSegmentation.exe VocalisationDirectory");
            Console.WriteLine("\t where VocalisationDirectory is the directory containing the vocalisations to be segmented.");
            Console.WriteLine("\t The VocalisationDirectory must ALSO contain a file called 'segmentation.ini' ");
            Console.WriteLine("\n\t The segmentation.ini file must contain all the parameters for segmentation, as shown in example file given.");
            Console.WriteLine("\t In particular, it must contain the path of a .WAV file that is to be used to extract a SILENCE/NOISE model.");
            Console.WriteLine("\t NOTE: The .wav silence file MUST NOT be in same directory as the vocalisations.");
            Console.WriteLine("\t OUTPUT 1: The directory containing the silence .wav file will contain a .png file, a segmentation.txt file ...");
            Console.WriteLine("\t           and a .noiseModel file, all of which have been obtained from the .wav silence file.");
            Console.WriteLine("\t           These may be used to help you check the effects of the noise reduction and the silence model extracted.");
            Console.WriteLine("\t OUTPUT 2: The directory containing the vocalisation files will contain a .png file and a .segmentation.txt file ...");
            Console.WriteLine("\t           for each vocalisation .wav file.");
            Console.WriteLine("\t           The .segmentation.txt file is to be used to build the HMM model.");
            Console.WriteLine("\t           The .png files offer a visual check on the effect of the noise removal and segmentation.");
            Console.WriteLine();
        }


    }
}
