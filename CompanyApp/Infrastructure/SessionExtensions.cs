using Newtonsoft.Json;
namespace CompanyApp.Infrastructure
{
    public static class SessionExtensions
    {
        public static void SetList<T>(this ISession session, string key, IEnumerable<T> value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
