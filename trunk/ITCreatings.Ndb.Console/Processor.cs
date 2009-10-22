using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ITCreatings.Ndb.Accessors;
using ITCreatings.Ndb.Core;
using ITCreatings.Ndb.Exceptions;
using ITCreatings.Ndb.Import;
using ITCreatings.Ndb.NdbConsole.Formatters;

namespace ITCreatings.Ndb.NdbConsole
{
    public class Processor
    {
        public Action Action { get; private set; }
        public DbProvider Provider { get; private set; }
        public DbAccessor Accessor { get; private set; }
        public DbStructureGateway StructureGateway { get; private set; }
        public ExitCode ExitCode = 0;

        private Type[] Process(Assembly assembly)
        {
            switch (Action)
            {
                case Action.Recreate:
                    StructureGateway.DropTables(assembly);
                    return StructureGateway.CreateTables(assembly);

                case Action.Create:
                    return StructureGateway.CreateTables(assembly);

                case Action.Drop:
                    return StructureGateway.DropTables(assembly);

                case Action.Alter:
                    return StructureGateway.AlterTables(assembly);

                default:
                    throw new Exception("Unknown Action: " + Action);
            }
        }

        public void Run(IEnumerable<string> assemblies, XmlFormatter xmlFormatter)
        {
            foreach (string _assembly in assemblies)
            {
                // using LoadFrom since: 
                // Assemblies can be loaded from multiple paths, not just from beneath the ApplicationBase.
                // Dependencies in the same dir as the requesting LoadFrom context assembly will automatically be found.
                // (thanks to Suzanne Cook http://blogs.msdn.com/suzcook/archive/2003/05/29/57143.aspx)
                Assembly file = Assembly.LoadFrom(_assembly);

                Console.WriteLine("\r\nProcessing {0} assembly", file.FullName);
                
                if (!ProcessEx(file, xmlFormatter))
                {
                    Type[] process = Process(file);
                    foreach (Type type in process)
                        Console.WriteLine("  - {0}", type);
                }
            }
        }

        private bool ProcessEx(Assembly assembly, XmlFormatter xmlFormatter)
        {
            if (Action == Action.Check)
            {
                Type[] types = DbAttributesManager.LoadDbRecordTypes(assembly);
                int errorNumber = 1;
                for (int i = 0; i < types.Length; i++)
                {
                    Type type = types[i];
                    bool isValid = StructureGateway.IsValid(type);
                    if (isValid)
                    {
                        xmlFormatter.AppendUnitTestResult("Mapping Test - " + type.FullName, Outcome.Passed, "");
                        string message = string.Format(
                            "{0} ({1}) - Ok"
                            , type, assembly.ManifestModule.Name);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        string message = string.Format(
                            "\r\n{3}. {0} ({1}) \r\n{4}\r\n{2}\r\n{4}"
                            , type, assembly.ManifestModule.Name, StructureGateway.LastError, errorNumber++,
                            "----------------------------------------------------------------------------"
                            );
                        Console.WriteLine(message);

                        ExitCode = ExitCode.Failure;
                        xmlFormatter.AppendUnitTestResult("Mapping Test - " + type.FullName, Outcome.Failed, StructureGateway.LastError);
                    }
                }
                return true;
            }
            return false;
        }

        public void SetProvider(string _provider)
        {
            try
            {
                Provider = (DbProvider)Enum.Parse(typeof(DbProvider), _provider, true);
            }
            catch
            {
                throw new ArgumentException(string.Format("Provider {0} doesn't supported", _provider));
            }
        }

        public void SetConnectionString(string connectionString)
        {
            Accessor = DbAccessor.Create(Provider, connectionString);
            StructureGateway = new DbStructureGateway(Accessor);

            if (!Accessor.CanConnect)
                throw new NdbException(string.Format(
                                           "Can't connect to \"{0}\" using {1} provider", connectionString, Provider));
        }

        public void SetAction(string _action)
        {
            try
            {
                Action = (Action) Enum.Parse(typeof(Action), _action, true);
            }
            catch
            {
                throw new ArgumentException(string.Format("Action {0} doesn't supported", _action));
            }
        }

        public void GenerateClasses(string [] args)
        {
            string path = args[3];
            string Namespace = (args.Length > 4) ? args[4] : null;
            
            var generator = new DbCodeGenerator(StructureGateway);
            if (!string.IsNullOrEmpty(Namespace))
                generator.Namespace = Namespace;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] Tables = StructureGateway.Accessor.LoadTables();
            foreach (string table in Tables)
            {
                string generatedClass = generator.GenerateClass(table);
                string filepath = Path.Combine(path, table + ".cs");

                if (File.Exists(filepath))
                {
                    Console.WriteLine("Error: File {0} already exists", filepath);
                    continue;
                }

                using (var sw = File.CreateText(filepath))
                {
                    sw.Write(generatedClass);
                    sw.Close();
                    
                }

                Console.WriteLine("Success: File {0} generated...", filepath);
            }
        }

        public void ImportFromExcel(string [] args)
        {
            if (args.Length > 4)
            {
                string SourceFileConnectionString = args[3];
                string assemblyName = args[4];

                var assembly = Assembly.LoadFrom(assemblyName);
                Type[] types = DbAttributesManager.LoadDbRecordTypes(assembly);

                var Source = (ExcelAccessor) DbAccessor.Create(DbProvider.Excel, SourceFileConnectionString);
                Accessor.ShareConnection = true;
                var Target = new DbGateway(Accessor);

                DbExcelExport export = new DbExcelExport(Source, Target);
                export.Export(types, true);
                ExitCode = ExitCode.Success;
            }
            else
            {
                ExitCode = ExitCode.Exception;    
            }
        }
    }
}