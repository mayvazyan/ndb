using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.NdbConsole.Formatters;

namespace ITCreatings.Ndb.NdbConsole
{
    public class FormattersFactory
    {
        public static XmlFormatter GetFormatter(string key, string path)
        {
            //TODO: add NUnit formatter
            switch (key)
            {
                case "MsTest2008":
                    return new MsTest2008Formatter(path);
            }
            
            throw new NdbException("The following formatter isn't supported: {0}.\r\nSupported formatters: MsTest2008", key);
        }
    }
}
