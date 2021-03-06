// <copyright file="WavPackAudioUtility.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace Acoustics.Tools.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Acoustics.Shared;

    /// <summary>
    /// Audio utility implemented using wav(un)pack.
    /// </summary>
    public class WavPackAudioUtility : AbstractAudioUtility, IAudioUtility
    {
        /*
        // -i ignore the header and accept the actual length
        // -m = compute & store MD5 signature of raw audio data
        // -q = quiet (keep console output to a minimum)
        ////private readonly static string WavPackArgs = " -i -m -q ";

        // -m = calculate and display MD5 signature; verify if lossless
        // -q = quiet (keep console output to a minimum)
        // -ss = display super summary (including tags) to stdout (no decode)
        ////private readonly static string WavUnPackArgs = " -m -q -ss ";

        // --skip=[sample|hh:mm:ss.ss] = start decoding at specified sample/time
        // Specifies an alternate start position for decoding, as either an integer sample
        // index or as a time in hours, minutes, and seconds (with fraction). The WavPack
        // file must be seekable (i.e. not a pipe). This option can be used with the --until
        // option to decode a specific region of a track.

        // --until=[+|-][sample|hh:mm:ss.ss] = stop decoding at specified sample/time
        // Specifies an alternate stop position for decoding, as either an integer sample
        // index or as a time in hours, minutes, and seconds (with fraction).
        // If a plus ('+') or minus ('-') sign is inserted before the specified sample (or time)
        // then it becomes a relative amount, either from the position specified by a --start option
        // (if plus) or from the end of the file (if minus).

        // -w = regenerate .wav header (ignore RIFF data in file)
        */

        public const string MissingBinary = "Converting from WavPack is not supported because we cannot find a wvunpack binary.";

        private const string ArgsDefault = " -m -q -w ";
        private const string ArgsSkip = " --skip={0} ";
        private const string ArgsUtil = " --until={0}{1} ";
        private const string ArgsInputAndOutputFile = " \"{0}\" -o \"{1}\" ";

        /// <summary>
        /// Initializes a new instance of the <see cref="WavPackAudioUtility"/> class.
        /// </summary>
        /// <param name="wavUnpack">
        /// The wav Unpack.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// If the provided binary does not exist.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="wavUnpack"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="wavUnpack"/> does contain the string "wavUnpack".
        /// </exception>
        public WavPackAudioUtility(FileInfo wavUnpack)
        {
            this.CheckExe(wavUnpack, "wvunpack");
            this.ExecutableInfo = wavUnpack;
            this.ExecutableModify = wavUnpack;

            this.TemporaryFilesDirectory = TempFileHelper.TempDir();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WavPackAudioUtility"/> class.
        /// </summary>
        /// <param name="wavUnpack">
        /// The wav Unpack.
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// If the provided binary does not exist.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="wavUnpack"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="wavUnpack"/> does contain the string "wavUnpack".
        /// </exception>
        public WavPackAudioUtility(FileInfo wavUnpack, DirectoryInfo temporaryFilesDirectory)
        {
            this.CheckExe(wavUnpack, "wvunpack");
            this.ExecutableInfo = wavUnpack;
            this.ExecutableModify = wavUnpack;

            this.TemporaryFilesDirectory = temporaryFilesDirectory;
        }

        /// <summary>
        /// Gets the valid source media types.
        /// </summary>
        protected override IEnumerable<string> ValidSourceMediaTypes => new[] { MediaTypes.MediaTypeWavpack };

        /// <summary>
        /// Gets the invalid source media types.
        /// </summary>
        protected override IEnumerable<string> InvalidSourceMediaTypes => null;

        /// <summary>
        /// Gets the valid output media types.
        /// </summary>
        protected override IEnumerable<string> ValidOutputMediaTypes => new[] { MediaTypes.MediaTypeWav };

        /// <summary>
        /// Gets the invalid output media types.
        /// </summary>
        protected override IEnumerable<string> InvalidOutputMediaTypes => null;

        /// <summary>
        /// The construct modify args.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        protected override string ConstructModifyArgs(FileInfo source, FileInfo output, AudioUtilityRequest request)
        {
            var sb = new StringBuilder(ArgsDefault);

            // only deals with start and end, does not do anything with sampling, channels or bit rate.
            if (request.OffsetStart.HasValue || request.OffsetEnd.HasValue)
            {

                if (request.OffsetStart.HasValue && request.OffsetStart.Value > TimeSpan.Zero)
                {
                    sb.AppendFormat(ArgsSkip, FormatTimeSpan(request.OffsetStart.Value));
                }

                if (request.OffsetEnd.HasValue && request.OffsetEnd.Value > TimeSpan.Zero)
                {
                    if (request.OffsetStart.HasValue && request.OffsetStart.Value > TimeSpan.Zero)
                    {
                        sb.AppendFormat(ArgsUtil, "+",
                            FormatTimeSpan(request.OffsetEnd.Value - request.OffsetStart.Value));
                    }
                    else
                    {
                        sb.Append(string.Format(ArgsUtil, string.Empty, FormatTimeSpan(request.OffsetEnd.Value)));
                    }
                }
            }

            sb.AppendFormat(ArgsInputAndOutputFile, source.FullName, output.FullName);

            return sb.ToString();
        }

        /// <summary>
        /// The construct info args.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        protected override string ConstructInfoArgs(FileInfo source)
        {
            string args = $" -s \"{source.FullName}\" ";
            return args;
        }

        /// <summary>
        /// The get info.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="process">
        /// The process.
        /// </param>
        /// <returns>
        /// The Acoustics.Tools.AudioUtilityInfo.
        /// </returns>
        protected override AudioUtilityInfo GetInfo(FileInfo source, ProcessRunner process)
        {
            var lines = process.StandardOutput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var result = new AudioUtilityInfo();
            result.SourceFile = source;

            foreach (var line in lines)
            {
                var firstColon = line.IndexOf(':');

                if (firstColon > -1)
                {
                    var key = line.Substring(0, firstColon).Trim();
                    var value = line.Substring(firstColon + 1).Trim();

                    if (key == "source")
                    {
                        var values = value.Split(' ');

                        // precision
                        result.RawData.Add("precision", values[0].Trim());

                        // sample rate
                        result.RawData.Add("sample rate", values[3].Trim());
                    }
                    else
                    {
                        result.RawData.Add(key, value);
                    }
                }
            }

            // parse info into class
            const string KeyDuration = "duration";
            const string KeyBitRate = "ave bitrate";
            const string KeySampleRate = "sample rate";
            const string KeyChannels = "channels";
            const string KeyPrecision = "precision";

            if (result.RawData.ContainsKey(KeyDuration))
            {
                var stringDuration = result.RawData[KeyDuration];

                var formats = new[]
                        {
                            @"h\:mm\:ss\.ff", @"hh\:mm\:ss\.ff", @"h:mm:ss.ff",
                            @"hh:mm:ss.ff",
                        };

                if (TimeSpan.TryParseExact(stringDuration.Trim(), formats, CultureInfo.InvariantCulture, out var tsresult))
                {
                    result.Duration = tsresult;
                }
            }

            if (result.RawData.ContainsKey(KeyBitRate))
            {
                var stringValue = result.RawData[KeyBitRate];

                var hadK = false;
                if (stringValue.Contains("kbps"))
                {
                    stringValue = stringValue.Replace("kbps", string.Empty);
                    hadK = true;
                }

                var value = double.Parse(stringValue);

                if (hadK)
                {
                    value = value * 1000;
                }

                result.BitsPerSecond = Convert.ToInt32(value);
            }

            if (result.RawData.ContainsKey(KeySampleRate))
            {
                result.SampleRate = this.ParseIntStringWithException(result.RawData[KeySampleRate], "wavPack.SampleRate");
            }

            if (result.RawData.ContainsKey(KeyChannels))
            {
                // multi channel will attempt to print the channel names "4 (FL,FR,FC,LFE)"
                string channels = result.RawData[KeyChannels];
                channels = Regex.Replace(channels, "\\(.*\\).*", string.Empty).Trim();

                result.ChannelCount = this.ParseIntStringWithException(channels, "wavPack.Channels");
            }

            if (result.RawData.ContainsKey(KeyPrecision))
            {
                result.BitsPerSample = this.ParseIntStringWithException(result.RawData[KeyPrecision].Replace("-bit", string.Empty).Trim(), "wavPack.Precision");
            }

            result.MediaType = MediaTypes.MediaTypeWavpack;

            return result;
        }

        /// <summary>
        /// The check audioutility request.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="sourceMimeType">
        /// The source Mime Type.
        /// </param>
        /// <param name="output">
        /// The output.
        /// </param>
        /// <param name="outputMediaType">
        /// The output media type.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Wvunpack cannot perform this type of request.
        /// </exception>
        protected override void CheckRequestValid(FileInfo source, string sourceMimeType, FileInfo output, string outputMediaType, AudioUtilityRequest request)
        {

            if (request.BitDepth.NotNull())
            {
                const string message = "Haven't added support for changing bit depth in" + nameof(WavPackAudioUtility);
                throw new BitDepthOperationNotImplemented(message);
            }

            if (request.Channels.NotNull())
            {
                throw new ChannelSelectionOperationNotImplemented("Wvunpack cannot modify the channel.");
            }

            if (request.MixDownToMono.HasValue && request.MixDownToMono.Value)
            {
                throw new ChannelSelectionOperationNotImplemented("Wvunpack cannot mix down the channels to mono.");
            }

            if (request.TargetSampleRate.HasValue)
            {
                throw new ArgumentException("Wvunpack cannot modify the sample rate.", nameof(request));
            }
        }

        private static string FormatTimeSpan(TimeSpan value)
        {
            // "hh\\:mm\\:ss\\.ff"
            // hh:mm:ss.ss
            return Math.Floor(value.TotalHours).ToString("00") + ":" + value.Minutes.ToString("00") + ":" + value.Seconds.ToString("00") +
                   "." + (value.Milliseconds / 10).ToString("00");
        }
    }
}