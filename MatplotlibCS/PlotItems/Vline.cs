using Newtonsoft.Json;
using System.Linq;

namespace MatplotlibCS.PlotItems
{
    /// <summary>
    /// Vertical line
    /// </summary>
    public class Vline : Line2D
    {
        [JsonProperty(PropertyName = "ymin")]
        public double YMin { get; set; }

        [JsonProperty(PropertyName = "ymax")]
        public double YMax { get; set; }

        public Vline(string name, object[] x, double ymin, double ymax) : base(name)
        {
            X = x.ToList();
            YMin = ymin;
            YMax = ymax;
            ShowLegend = false;
        }
    }
}
