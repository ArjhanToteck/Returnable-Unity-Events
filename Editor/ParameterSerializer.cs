using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ReturnableUnityEvents.Editor
{
    [Serializable]
    public static class ParameterSerializer
    {
        private static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        // we have to handle parameter serialization ourselves because unity's system is stupid asf
        public static List<object> DeserializeParameters(string parametersJSON)
        {
            return JsonConvert.DeserializeObject<List<object>>(parametersJSON, jsonSettings);
        }

        public static string SerializeParameters(List<object> value)
        {
            return JsonConvert.SerializeObject(value, jsonSettings);
        }
    }
}