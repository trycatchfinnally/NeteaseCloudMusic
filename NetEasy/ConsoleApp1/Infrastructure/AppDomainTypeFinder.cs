using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Infrastructure
{
  public   class AppDomainTypeFinder:ITypeFinder
    {
        private bool ignoreReflectionErrors = true;
        private bool loadAppDomainAssemblies = true;
        private bool _binFolderAssembliesLoaded = false;
        private string assemblySkipLoadingPattern = "^System|^mscorlib|^Microsoft|^AjaxControlToolkit|^Antlr3|^Autofac|^AutoMapper|^Castle|^ComponentArt|^CppCodeProvider|^DotNetOpenAuth|^EntityFramework|^EPPlus|^FluentValidation|^ImageResizer|^itextsharp|^log4net|^MaxMind|^MbUnit|^MiniProfiler|^Mono.Math|^MvcContrib|^Newtonsoft|^NHibernate|^nunit|^Org.Mentalis|^PerlRegex|^QuickGraph|^Recaptcha|^Remotion|^RestSharp|^Rhino|^Telerik|^Iesi|^TestDriven|^TestFu|^UserAgentStringLibrary|^VJSharpCodeProvider|^WebActivator|^WebDev|^WebGrease";
        private string assemblyRestrictToLoadingPattern = ".*";
        private IList<string> assemblyNames = new List<string>();
       
        public   AppDomain App => AppDomain.CurrentDomain;
        public bool LoadAppDomainAssemblies
        {
            get { return loadAppDomainAssemblies; }
            set { loadAppDomainAssemblies = value; }
        }
        public IList<string> AssemblyNames
        {
            get
            {
                return assemblyNames;
            }

            set
            {
                assemblyNames = value;
            }
        }
        public string AssemblySkipLoadingPattern
        {
            get
            {
                return assemblySkipLoadingPattern;
            }

            set
            {
                assemblySkipLoadingPattern = value;
            }
        }
        public string AssemblyRestrictToLoadingPattern
        {
            get
            {
                return assemblyRestrictToLoadingPattern;
            }

            set
            {
                assemblyRestrictToLoadingPattern = value;
            }
        }
       private  bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
        {
            try
            {
                var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
                foreach (var implementedInterface in type.FindInterfaces((objType, objCriteria) => true, null))
                {
                    if (!implementedInterface.IsGenericType)
                        continue;

                    return genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition());

                }
                return false;
            }
            catch
            {
                return false;
            }
        }
       private  void  LoadMatchingAssemblies(string directoryPath)
        {
            var loadedAssemblyNames = new List<string>();
            foreach (Assembly a in GetAssemblies())
            {
                loadedAssemblyNames.Add(a.FullName);
            }

            if (!Directory.Exists(directoryPath))
            {
                return;
            }

            foreach (string dllPath in Directory.GetFiles(directoryPath, "*.dll"))
            {
                try
                {
                    var an = AssemblyName.GetAssemblyName(dllPath);
                    if (!Regex.IsMatch(an.FullName, assemblySkipLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
                        && Regex.IsMatch(an.FullName, assemblyRestrictToLoadingPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled)
                        && !loadedAssemblyNames.Contains(an.FullName))
                    {
                        App.Load(an);
                    }

                    //old loading stuff
                    //Assembly a = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    //if (Matches(a.FullName) && !loadedAssemblyNames.Contains(a.FullName))
                    //{
                    //    App.Load(a.FullName);
                    //}
                }
                catch (BadImageFormatException ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }
        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true) => FindClassesOfType(typeof(T), onlyConcreteClasses);
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true) =>
          FindClassesOfType(assignTypeFrom, GetAssemblies(), onlyConcreteClasses);
        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
            => FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            
            var result = new List<Type>();
            try
            {
                assemblies.Each(x =>
                {
                    Type[] types = null;
                    try { types = x.GetTypes(); }
                    catch { if (!ignoreReflectionErrors) throw; }
                    if (types != null)
                    {
                        foreach (var type in types)
                        {
                            if (assignTypeFrom.IsAssignableFrom(type) || (assignTypeFrom.IsGenericTypeDefinition && DoesTypeImplementOpenGeneric(type, assignTypeFrom)))
                            {
                                if (!type.IsInterface)
                                {
                                    if (onlyConcreteClasses)
                                    {
                                        if (type.IsClass && !type.IsAbstract)
                                        {
                                            result.Add(type);
                                        }
                                    }
                                    else
                                    {
                                        result.Add(type);
                                    }
                                }
                            }
                        }

                    }
                });
            }
            catch (ReflectionTypeLoadException ex)
            {

                StringBuilder sb = new StringBuilder();
                ex.LoaderExceptions.Each(x => sb.AppendLine(ex.Message));
                var fail = new Exception(sb.ToString(), ex);
                System.Diagnostics.Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
            return result;

        }
        public   IList<Assembly> GetAssemblies()
        {
            if ( !_binFolderAssembliesLoaded)
            {
                _binFolderAssembliesLoaded = true;
                LoadMatchingAssemblies(GetBinDirectory());
            }
            
            var addedAssemblyNames = new List<string>();
            var assemblies = new List<Assembly>();
            if (loadAppDomainAssemblies)
            {

                AppDomain.CurrentDomain.GetAssemblies().Where(x => !Regex.IsMatch(x.FullName, assemblySkipLoadingPattern,
                                                                     RegexOptions.IgnoreCase | RegexOptions.Compiled)
                                                                  && Regex.IsMatch(x.FullName, assemblyRestrictToLoadingPattern,
                                                                     RegexOptions.IgnoreCase | RegexOptions.Compiled))
                                                                     .Each(x =>
                                                                     {
                                                                         if (!addedAssemblyNames.Contains(x.FullName))
                                                                         {
                                                                             assemblies.Add(x);
                                                                             addedAssemblyNames.Add(x.FullName);
                                                                         }
                                                                     });
            }
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);
                if (!addedAssemblyNames.Contains(assembly.FullName))
                {
                    assemblies.Add(assembly);
                    addedAssemblyNames.Add(assembly.FullName);
                }
            }
            return assemblies;

        }
        private string GetBinDirectory() => AppDomain.CurrentDomain.BaseDirectory;
    }
}
