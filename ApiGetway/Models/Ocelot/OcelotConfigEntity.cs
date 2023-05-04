using Newtonsoft.Json;
using Ocelot.Configuration.File;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gss.ApiGateway.Models.Ocelot
{
    public class OcelotConfigEntity
    {

        public int Id { get; set; }

        public Version Version { get; set; }

        [NotMapped]
        public FileConfiguration Payload
        {
            get =>
                JsonConvert.DeserializeObject<FileConfiguration>(PayloadAsString);
            set => PayloadAsString = JsonConvert.SerializeObject(value);
        }

        public string PayloadAsString { get; private set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }
    }
}
