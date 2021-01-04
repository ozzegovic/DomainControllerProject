
namespace DomainController
{
    public class UserRequest
    {
        public string Username { get; set; }
        public short Challenge { get; set; }
        public string RequestedService { get; set; }
        public byte[] SessionKey { get; set; }

        public UserRequest() { }
    }
}