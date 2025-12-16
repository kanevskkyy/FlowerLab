using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PickupStore
    {
        [EnumMember(Value = "м. Чернівці, вул. Герцена 2а")]
        Hertsena2A,

        [EnumMember(Value = "м. Чернівці, вул. Васіле Александрі, 1")]
        VasileAlexandri1
    }
}
