using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.NdbConsole.Formatters;

namespace ITCreatings.Ndb.NdbConsole
{
    public class FormattersFactory
    {
        const string MsTest2008Filename = @"results.trx";

        public static XmlFormatter GetFormatter(string key)
        {
            //TODO: add NUnit formatter
            switch (key)
            {
                case "MsTest2008":
                    return new MsTest2008Formatter(MsTest2008Filename);
            }
            
            throw new NdbException("The following formatter isn't supported: {0}.\r\nSupported formatters: MsTest2008", key);
        }
    }
}
