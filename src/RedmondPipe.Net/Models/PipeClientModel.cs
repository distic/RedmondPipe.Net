using Newtonsoft.Json;
using System.Drawing;

namespace RedmondPipe.Models
{
    public class PipeClientModel
    {
        public string Title { get; set; }

        public string ConsoleBackColorString { get; set; }

        public string ConsoleTextColorString { get; set; }

        [JsonIgnore]
        public Color ConsoleBackColor
        {
            get
            {
                if (ConsoleBackColorString == null)
                    return Color.Black;
                return Color.FromName(ConsoleBackColorString);
            }
        }

        [JsonIgnore]
        public Color ConsoleTextColor
        {
            get
            {
                if (ConsoleTextColorString == null)
                    return Color.Gainsboro;
                return Color.FromName(ConsoleTextColorString);
            }
        }
    }
}