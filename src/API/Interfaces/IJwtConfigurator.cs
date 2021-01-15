
namespace API.Interfaces
{
    public interface IJwtConfigurator
    {
        public string GetToken(string userName);
    }
}
