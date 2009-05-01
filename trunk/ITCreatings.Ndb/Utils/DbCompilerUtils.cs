#if DEBUG
using System.CodeDom.Compiler;
using System.Reflection;
using ITCreatings.Ndb.Exceptions;
using Microsoft.CSharp;

namespace ITCreatings.Ndb.Utils
{
    /// <summary>
    /// Compiler Utils (just for Unit Tests purposes... at least now)
    /// </summary>
    public class DbCompilerUtils
    {
        private static string ndbAssembly { get { return typeof (DbStructureGateway).Assembly.Location; } }

        /// <summary>
        /// Compiles the specified sources.
        /// </summary>
        /// <param name="sources">The sources.</param>
        /// <returns></returns>
        public static Assembly Compile(params string [] sources)
        {
            CompilerParameters compilerParameters = new CompilerParameters
                                                        {
                                                            GenerateInMemory = true,
                                                            GenerateExecutable = false,
                                                            TreatWarningsAsErrors = false,
                                                            CompilerOptions = "/optimize"
                                                        };
            
            compilerParameters.ReferencedAssemblies.Add(ndbAssembly);
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParameters, sources);

            if (results.Errors.HasErrors)
                throw new NdbException("Compile error: %r" + string.Concat(results.Errors[0].ErrorText));

            return results.CompiledAssembly;
        }
    }
}

#endif