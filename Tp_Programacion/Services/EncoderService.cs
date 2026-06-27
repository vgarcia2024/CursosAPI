
namespace Tp_Programacion.Services
{
    public interface IEncoderService
    {
        string Encrypt(string text);
        bool Verify(string encrypted, string text);
    }

    public class EncoderService : IEncoderService
    {
        public string Encrypt(string text)
        {
            string salt = BC.GenerateSalt(13);
            string hashed = BC.HashPassword(text, salt);
            return hashed;
        }

        public bool Verify(string encrypted, string text)
        {
            bool result = BC.Verify(text, encrypted);
            return result;
        }
    }
}
