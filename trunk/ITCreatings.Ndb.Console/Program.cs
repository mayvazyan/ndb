using System;
using System.IO;
using System.Text;

namespace ITCreatings.Ndb.NdbConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 4)
            {
                printUsage();
                return;
            }

            string action = args[0];
            string provider = args[1];
            string connectionString = args[2];

            string[] assemblies = new string[args.Length - 3];
            for (int i = 3; i < args.Length; i++)
            {
                string path = args[i];
                assemblies[i - 3] = path;
            }

            Processor processor = new Processor();

            try
            {
                processor.SetProvider(provider);
                processor.SetConnectionString(connectionString);
                processor.SetAction(action);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                printUsage();
                return;
            }

            Console.WriteLine("Action: {0}", action);

            try
            {
                processor.Run(assemblies);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Internal exception occured: " + ex.Message);
            }
        }

       
        private static void printUsage()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Usage: NdbConsole [action] [inputfiles]");
            sb.AppendLine();
            sb.AppendLine("Actions:");
            sb.AppendFormat("\\{0} \tCreates database structure based on specifyed  assemblies\r\n", Action.Create);
            sb.AppendFormat("\\{0} \t\tRemoves tables related to the objects in specifyed  assemblies\r\n", Action.Drop);
            sb.AppendFormat("\\{0} \t\tUpdates database structure to match specifyed assemblies\r\n", Action.Alter);

            Console.WriteLine(sb.ToString());
        }
    }
}