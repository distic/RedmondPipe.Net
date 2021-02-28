using System.Collections.Generic;

namespace RedmondPipe.Models
{
    public class AppSettingsModel
    {
        public string NamedPipe { get; set; }

        public List<PipeClientModel> PipeClients { get; set; }
    }
}