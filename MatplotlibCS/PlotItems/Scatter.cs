using Newtonsoft.Json;
using System.Collections.Generic;

namespace MatplotlibCS.PlotItems
{
    public class Scatter : Line3D
    {
        public Scatter(string name)
            : base(name)
        {
            MarkerSize = 5;
            Marker = Marker.Circle;
            LineStyle = LineStyle.Solid;
            LineWidth = new List<float> { 1 };
        }

        /// <summary>
        /// Point colors
        /// </summary>
        [JsonProperty(PropertyName = "color")]
        public new List<Color> Color { get; set; } = new List<Color> { PlotItems.Color.Black };

        /// <summary>
        /// Width of line
        /// </summary>
        [JsonProperty(PropertyName = "lineWidth")]
        public new List<float> LineWidth { get; set; } = new List<float> { 1 };

        [JsonProperty(PropertyName = "plotnonfinite")]
        public bool PlotNonFinite { get; set; } = false;
    }
}
