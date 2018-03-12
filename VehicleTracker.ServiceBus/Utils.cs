using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VehicleTracker.ServiceBus
{
    public static class Utils
    {
        public static string Serialize(this object obj)
            => JsonConvert.SerializeObject(obj);

        public static byte[] SerializeBytes(this object obj)
        {
            var serialized = obj.Serialize();
            var body = Encoding.UTF8.GetBytes(serialized);
            return body;
        }

        public static string Stringify(this byte[] array)
            => Encoding.UTF8.GetString(array);

        public static T Deserialize<T>(this byte[] array)
        {
            var s = array.Stringify();
            var result = JsonConvert.DeserializeObject<T>(s);
            return result;
        }
    }
}
