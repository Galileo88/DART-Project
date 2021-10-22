using System.Reflection;
using Kopernicus.RuntimeUtility;

[assembly: AssemblyTitle ("Kopernicus")]
[assembly: AssemblyDescription ("Planetary System Modifier for Kerbal Space Program")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("Kopernicus Project")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyCopyright ("Copyright (C) Kopernicus Project")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
[assembly: KSPAssembly("Kopernicus", 2, 0)]
[assembly: KSPAssemblyDependency("Kopernicus.Parser", 2, 0)]
[assembly: KSPAssemblyDependency("ModularFlightIntegrator", 1, 0)]
[assembly: AssemblyVersion("2.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: LogAggregator("GameData/ModuleManager.ConfigCache")]
[assembly: LogAggregator("Logs/DART/")]
[assembly: LogAggregator("KSP.log")]