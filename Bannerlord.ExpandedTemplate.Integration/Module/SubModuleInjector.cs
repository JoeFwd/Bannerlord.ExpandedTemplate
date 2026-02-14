// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Xml;
// using Bannerlord.ExpandedTemplate.Domain.Logging.Port;
// using TaleWorlds.ModuleManager;
//
// namespace Bannerlord.ExpandedTemplate.Integration.Module;
//
// public class SubModuleInjector
// {
//     private static readonly FieldInfo? VersionListField =
//         typeof(List<SubModuleInfo>).GetField("_version", BindingFlags.NonPublic | BindingFlags.Instance);
//
//     private readonly ILogger _logger;
//     private readonly IServiceProvider _serviceProvider;
//
//     public SubModuleInjector(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
//     {
//         _logger = loggerFactory.CreateLogger<SubModuleInjector>();
//         _serviceProvider = serviceProvider;
//     }
//
//     public void Inject()
//     {
//         if (VersionListField == null)
//         {
//             _logger.Error(
//                 "Could not add dynamic Bannerlord.ExpandedTemplate module into Bannerlord. This is due to an unexpected dotnet implementation");
//             return;
//         }
//
//         var currentlyLoadedSubModuleTypeNames = GetCurrentlyLoadedSubModuleTypeNames();
//
//         var moduleInfoBeingLoaded = GetAllNonOfficialModuleInfoWithSubModuleTypeName()
//             .Where(tuple =>
//                 !currentlyLoadedSubModuleTypeNames.Contains(tuple.subModuleTypeName))
//             .Select(tuple => tuple.moduleInfo).FirstOrDefault();
//
//         if (moduleInfoBeingLoaded == null)
//         {
//             _logger.Warn("No module info being loaded was found. Skipping SubModule injection.");
//             return;
//         }
//
//         var subModuleInfo = new SubModuleInfo();
//         subModuleInfo.LoadFrom(GetSubModuleInfo(), moduleInfoBeingLoaded.FolderPath, false);
//
//         AddSubModule(moduleInfoBeingLoaded, subModuleInfo);
//     }
//
//     private List<(ModuleInfo moduleInfo, string subModuleTypeName)>
//         GetAllNonOfficialModuleInfoWithSubModuleTypeName()
//     {
//         return ModuleHelper.GetActiveModules()
//             .Where(moduleInfo => !moduleInfo.IsOfficial)
//             .SelectMany(info => info.SubModules.Select(sub => (info, sub.SubModuleClassTypeName))).ToList();
//     }
//
//     private List<string> GetCurrentlyLoadedSubModuleTypeNames()
//     {
//         return TaleWorlds.MountAndBlade.Module.CurrentModule.CollectSubModules()
//             .Select(subModule => subModule.GetType().FullName).ToList();
//     }
//
//     private XmlNode GetSubModuleInfo()
//     {
//         string subModuleXml = $@"
//             <SubModule>
//                 <Name value=""{typeof(SubModule).Name}""/>
//                 <DLLName value=""{Assembly.GetExecutingAssembly().GetName().Name}.dll""/>
//                 <SubModuleClassType value=""{typeof(SubModule).FullName}""/>
//                 <Tags />
//             </SubModule>";
//         var doc = new XmlDocument();
//         doc.LoadXml(subModuleXml);
//         return doc.DocumentElement;
//     }
//
//     private void AddSubModule(ModuleInfo moduleInfo, SubModuleInfo subModuleInfo)
//     {
//         int currentVersion = (int)(VersionListField?.GetValue(moduleInfo.SubModules) ?? 0);
//
//         moduleInfo.SubModules.Add(subModuleInfo);
//
//         if (VersionListField != null)
//         {
//             VersionListField.SetValue(moduleInfo.SubModules, currentVersion);
//         }
//     }
// }