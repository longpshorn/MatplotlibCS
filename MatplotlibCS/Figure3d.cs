using Newtonsoft.Json;
using System.Collections.Generic;

namespace MatplotlibCS
{
    /// <summary>
    /// Class desribing a figure to be build
    /// </summary>
    [JsonObject(Title = "figure")]
    public class Figure3d : Figure
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="rows">Number of rows in subplots grid</param>
        /// <param name="columns">Number of columns in subplots grid</param>
        public Figure3d(int rows = 1, int columns = 1)
            : base(rows, columns)
        {
            Rows = rows;
            Columns = columns;
            Subplots = new List<Axes3d>();
        }

        /// <summary>
        /// Figuree subplots
        /// </summary>
        [JsonProperty(PropertyName = "__subplots__")]
        public new List<Axes3d> Subplots { get; set; }
    }
}
