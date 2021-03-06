// <copyright file="OscillationParameters.cs" company="QutEcoacoustics">
// All code in this file and all associated files are the copyright and property of the QUT Ecoacoustics Research Group (formerly MQUTeR, and formerly QUT Bioacoustics Research Group).
// </copyright>

namespace AnalysisPrograms.Recognizers.Base
{
    using Acoustics.Shared;

    /// <summary>
    /// Parameters needed from a config file to detect oscillation components.
    /// </summary>
    [YamlTypeTag(typeof(OscillationParameters))]
    public class OscillationParameters : DctParameters
    {
    }
}