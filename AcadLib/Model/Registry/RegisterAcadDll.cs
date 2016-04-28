using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Microsoft.Win32;

namespace AcadLib
{
   public static class RegisterAcadDll
   {
      /// <summary>
      /// Controls how and when the .NET assembly is loaded.
      /// </summary>
      [Flags]
      public enum LOADCTRLS
      {
         DetectProxy = 1,
         Startup = 2,
         Command = 4,
         Request = 8,
         NotLoad = 16,
         Transparently = 32
      }

      /// <summary>
      /// Регистрация в реестре автозагрузки текущей сборки в текущем автокаде. 
      /// </summary>
      /// <param name="loadctrls">Управление загрузкой сборки</param>
      /// <param name="UserOrMachine">True - User (HKCU); False - Local machine (HKLM)</param>
      /// <returns></returns>
      public static bool Registration(LOADCTRLS loadctrls = LOADCTRLS.Command | LOADCTRLS.Request, bool UserOrMachine = true)
      {
         try
         {
            string sProdKey = UserOrMachine ? HostApplicationServices.Current.UserRegistryProductRootKey :
                                                HostApplicationServices.Current.MachineRegistryProductRootKey;
            Assembly curAssembly = Assembly.GetExecutingAssembly();
            string sAppName = curAssembly.GetName().Name;

            using (Microsoft.Win32.RegistryKey regAcadProdKey = UserOrMachine ? Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sProdKey) :
                                                         Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sProdKey))
            {
               using (Microsoft.Win32.RegistryKey regAcadAppKey = regAcadProdKey.OpenSubKey("Applications", true))
               {
                  // Check to see if the "MyApp" key exists
                  string[] subKeys = regAcadAppKey.GetSubKeyNames();
                  foreach (string subKey in subKeys)
                  {
                     if (subKey.Equals(sAppName))
                     {
                        return true;
                     }
                  }

                  // Register the application
                  using (Microsoft.Win32.RegistryKey regAppAddInKey = regAcadAppKey.CreateSubKey(sAppName))
                  {
                     string desc = curAssembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
                     if (desc == "") desc = sAppName;
                     regAppAddInKey.SetValue("DESCRIPTION", desc, Microsoft.Win32.RegistryValueKind.String);
                     regAppAddInKey.SetValue("LOADCTRLS", loadctrls, Microsoft.Win32.RegistryValueKind.DWord);
                     regAppAddInKey.SetValue("LOADER", curAssembly.Location, Microsoft.Win32.RegistryValueKind.String);
                     regAppAddInKey.SetValue("MANAGED", 1, Microsoft.Win32.RegistryValueKind.DWord);

                     // Запись раздела Commands
                     SetCommands(regAppAddInKey, curAssembly);

                     return true;
                  }
               }
            }
         }
         catch
         {
            return false;
         }
      }

      private static void SetCommands(Microsoft.Win32.RegistryKey regAppAddInKey, Assembly curAssembly)
      {
         // Создание раздела Commands в переданной ветке реестра и создание записей команд в этом разделе.
         // Команды определяются по атрибутам переданной сборки, в которой должен быть определен атрибут класса команд
         // из которого получаются методы с атрибутами CommandMethod.
         using (regAppAddInKey = regAppAddInKey.CreateSubKey("Commands"))
         {
            var attClass = curAssembly.GetCustomAttribute<CommandClassAttribute>();
            var members = attClass.Type.GetMembers();
            foreach (var member in members)
            {
               if (member.MemberType == MemberTypes.Method)
               {
                  var att = member.GetCustomAttribute<CommandMethodAttribute>();
                  if (att != null)
                     regAppAddInKey.SetValue(att.GlobalName, att.GlobalName);
               }
            }
         }
      }

      /// <summary>
      /// Удаление записи из реестра, автозагрузки текущего приложения из текущей версии автокада.
      /// </summary>
      public static void Unregistration()
      {
         // Get the AutoCAD Applications key
         string sProdKey = HostApplicationServices.Current.UserRegistryProductRootKey;
         Assembly curAssembly = Assembly.GetExecutingAssembly();
         string sAppName = curAssembly.GetName().Name;

         // HKCU
         DeleteApp(sProdKey, sAppName, true);
         // HKLM         
         DeleteApp(sProdKey, sAppName, false);
      }

      private static void DeleteApp(string sProdKey, string sAppName, bool UserOrMachine)
      {
         using (Microsoft.Win32.RegistryKey regAcadProdKey = UserOrMachine ? Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sProdKey) :
                                                   Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sProdKey))
         {
            using (Microsoft.Win32.RegistryKey regAcadAppKey = regAcadProdKey.OpenSubKey("Applications", true))
            {
               // Delete the key for the application
               try
               {
                  regAcadAppKey.DeleteSubKeyTree(sAppName);
               }
               catch { }
            }
         }
      }
   }
}

