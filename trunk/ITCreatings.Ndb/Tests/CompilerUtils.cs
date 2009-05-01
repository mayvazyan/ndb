using System.CodeDom.Compiler;
using System.Reflection;
using ITCreatings.Ndb.Exceptions;
using Microsoft.CSharp;

namespace ITCreatings.Ndb.Tests
{
    internal class CompilerUtils
    {
        public static Assembly Compile(params string [] sources)
        {
            CompilerParameters compilerParameters = new CompilerParameters
                                                        {
                                                            GenerateInMemory = true,
                                                            GenerateExecutable = false,
                                                            TreatWarningsAsErrors = false,
                                                            CompilerOptions = "/optimize"
                                                        };
            string ndbAssembly = typeof (DbStructureGateway).Assembly.Location;
            compilerParameters.ReferencedAssemblies.Add(ndbAssembly);
            CSharpCodeProvider provider = new CSharpCodeProvider();
            
            CompilerResults results = provider.CompileAssemblyFromSource(compilerParameters, sources);

            if (results.Errors.HasErrors)
                throw new NdbException("Compile error: %r" + string.Concat(results.Errors[0].ErrorText));

            return results.CompiledAssembly;
        }
    }
}
