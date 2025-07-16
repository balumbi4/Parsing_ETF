using log4net.Config;
using System.IO;


namespace Parsing
{
    internal class Program
    {
        static void Main()
        {
            XmlConfigurator.Configure(new FileInfo("../../../loggerConfig.xml"));
            Parser parser = new Parser();
            parser.Parsing();        
        }
    }
}
