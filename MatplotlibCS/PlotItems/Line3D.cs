using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MatplotlibCS.PlotItems
{
    public class Line3D : Line2D
    {
        public Line3D(string name)
            : base(name)
        {
            Z = new List<double>();
        }

        /// <summary>
        /// Данные для графика, значение
        /// </summary>
        [JsonProperty(PropertyName = "z")]
        public List<double> Z { get; set; }
    }
}
