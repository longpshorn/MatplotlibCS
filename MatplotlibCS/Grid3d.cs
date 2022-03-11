using Newtonsoft.Json;

namespace MatplotlibCS
{
    /// <summary>
    /// Class describes a grid settings on a plot
    /// </summary>
    public class Grid3d : Grid
    {
        [JsonProperty(PropertyName = "z_lim")]
        public double[] ZLim { get; set; }

        [JsonProperty(PropertyName = "z_major_ticks")]
        public double[] ZMajorTicks { get; set; }

        [JsonProperty(PropertyName = "z_minor_ticks")]
        public double[] ZMinorTicks { get; set; }
    }
}
