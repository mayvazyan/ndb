using System;
using System.Configuration;
using System.Text;
using ITCreatings.Ndb.NdbConsole.Formatters;

namespace ITCreatings.Ndb.NdbConsole
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return (int) Run(args);
        }

        private static ExitCode Run(string[] args)
        {
            var parameters = new InputParameters();
            if (!parameters.Init(args))
            {
                printUsage();
                return ExitCode.Exception;
            }
            
            var processor = new Processor();
            try
            {
                parameters.Apply(processor);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                printUsage();
                return ExitCode.Exception;
            }

            try
            {
                switch (processor.Action)
                {
                    case Action.Generate:
                        processor.GenerateClasses(args);
                        break;

                    case Action.ImportExcel:
                        processor.ImportFromExcel(args);
                        break;

                    default:
                        string outputFormatter = ConfigurationManager.AppSettings["OutputFormatter"];
                        using (XmlFormatter xmlFormatter = FormattersFactory.GetFormatter(outputFormatter))
                        {
                            processor.Run(parameters.Assemblies, xmlFormatter);
                        }
                        break;
                }
                
                if (processor.ExitCode != ExitCode.Success)
                    printUsage();

                return processor.ExitCode;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Internal exception occured: " + ex.Message);
                return ExitCode.Exception;
            }
        }
        
        private static void printUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Usage: NdbConsole [Action] [Provider] [ConnectionString] [InputFiles]");
            sb.AppendLine();
            sb.AppendLine("Actions:");
            sb.AppendFormat("\\{0} \tCreates database structure based on specifyed  assemblies\r\n", Action.Create);
            sb.AppendFormat("\\{0} \t\tRemoves tables related to the objects in specifyed  assemblies\r\n", Action.Drop);
            sb.AppendFormat("\\{0} \t\tUpdates database structure to match specifyed assemblies\r\n", Action.Alter);

            sb.AppendLine("Providers:");
            sb.AppendFormat("{0},", DbProvider.MySql);
            sb.AppendFormat("{0},", DbProvider.MsSql);
            sb.AppendFormat("{0}, etc.", DbProvider.SqLite);

            Console.WriteLine(sb.ToString());
        }
    }
}