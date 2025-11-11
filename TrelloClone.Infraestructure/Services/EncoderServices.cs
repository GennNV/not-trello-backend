namespace TrelloClone.Infraestructure.Services
{
    public interface IEncoderService
    {
        string Encode(string value);

        bool Verify(string value, string hash);
    }

    public class EncoderService : IEncoderService
    {
        public string Encode(string value)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(13);
            return BCrypt.Net.BCrypt.HashPassword(value, salt);
        }

        public bool Verify(string value, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(value, hash);
        }
    }
}
