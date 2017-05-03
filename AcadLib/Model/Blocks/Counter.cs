using System.Collections.Generic;
using System.Text;

namespace AcadLib.Blocks
{
   public static class Counter
   {
      private static Dictionary<string, int> _counter;

      static Counter()
      {
         Clear();
      }

      public static void Clear ()
      {
         _counter = new Dictionary<string, int>();
      }

      public static void AddCount (string key)
      {
         if (_counter.ContainsKey(key) )
         {
            _counter[key]++;
         }
         else
         {
            _counter.Add(key, 1);
         }
      }

      public static int GetCount(string key)
      {
         int count;
         _counter.TryGetValue(key, out count);
         return count;
      }

      public static string Report()
      {
         var report = new StringBuilder("Обработано блоков:");
         foreach (var counter in _counter)
         {
            report.AppendLine(string.Format("\n{0} - {1} блоков.", counter.Key, counter.Value));
         }
         return report.ToString(); 
      }
   }
}
