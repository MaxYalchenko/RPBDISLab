namespace CompanyApp.Services
{
    public interface ICached<T>
    {
        public IEnumerable<T> GetList();
        public void AddList(string cacheKey);
        public IEnumerable<T> GetList(string cacheKey);
    }
}
