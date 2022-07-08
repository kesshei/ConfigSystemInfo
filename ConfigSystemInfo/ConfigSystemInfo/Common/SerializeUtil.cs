using Newtonsoft.Json;
namespace ConfigSystemInfo.Common
{
    /// <summary>
    /// 序列化工具
    /// </summary>
    public static class SerializeUtil
    {
        /// <summary>
        /// 序列化json
        /// </summary>
        public static string JsonSerialize(object t)
        {
            return JsonConvert.SerializeObject(t);
        }

        /// <summary>
        /// 反序列化json
        /// </summary>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
