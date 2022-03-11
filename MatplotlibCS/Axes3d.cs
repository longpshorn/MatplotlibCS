using MatplotlibCS.PlotItems;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MatplotlibCS
{
    /// <summary></summary>
    [JsonObject(Title = "axes")]
    public class Axes3d : Axes
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="index"></param>
        /// <param name="xtitle">Заголовок оси x</param>
        /// <param name="ytitle">Заголовок оси y</param>
        public Axes3d(int index = 1, string xtitle = "", string ytitle = "")
            : base(index, xtitle, ytitle)
        {
            this.Index = index;
            this.XTitle = xtitle;
            this.YTitle = ytitle;
            Grid = new Grid3d();
            PlotItems = new List<PlotItem>();
        }

        [JsonProperty(PropertyName = "is3d")]
        public bool Is3d { get; set; } = true;

        /// <summary>
        /// Подпись к оси Z
        /// </summary>
        [JsonProperty(PropertyName = "ztitle")]
        public string ZTitle { get; set; }

        /// <summary>
        /// Plot grid settings
        /// </summary>
        [JsonProperty(PropertyName = "grid")]
        public new Grid3d Grid { get; set; }
    }
}

