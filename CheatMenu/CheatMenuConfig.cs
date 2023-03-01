using SpaceWarp.API.Configuration;
using Newtonsoft.Json;

namespace CheatMenu
{
    [JsonObject(MemberSerialization.OptOut)]
    [ModConfig]
    public class CheatMenuConfig
    {
         [ConfigField("pi")] [ConfigDefaultValue(3.14159)] public double pi;
    }
}