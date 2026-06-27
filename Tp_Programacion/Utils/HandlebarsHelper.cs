using HandlebarsDotNet;
using System.Text;

namespace Tp_Programacion.Utils
{
    public static class HandlebarsHelper
    {
        public static string Render(string source, object data)
        {
            var template = HB.Compile(source);
            var result = template(data);
            return result;
        }

        public static string RenderFile(string filePath, object data)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }
            var source = File.ReadAllText(filePath, Encoding.UTF8);
            return Render(source, data);
        }

        public static string GenerateResetPwdTemplate(object data)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Templates", "reset-password.html");
            if (filePath == null)
            {
                throw new FileNotFoundException("No se encontro la ruta");
            }
            return RenderFile(filePath, data);
        }
    }
}
