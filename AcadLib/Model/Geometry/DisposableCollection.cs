using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadLib
{
    public interface IDisposableCollection<T> : ICollection<T>, IDisposable
      where T : IDisposable
    {
        void AddRange (IEnumerable<T> items);
        IEnumerable<T> RemoveRange (IEnumerable<T> items);
    }

    public class DisposableSet<T> : HashSet<T>, IDisposableCollection<T>
       where T : IDisposable
    {
        public DisposableSet ()
        {
        }

        public DisposableSet (IEnumerable<T> items)
        {
            AddRange(items);
        }

        public void Dispose ()
        {
            if (Count > 0)
            {
                Exception last = null;
                var list = this.ToList();
                Clear();
                foreach (T item in list)
                {
                    if (item != null)
                    {
                        try
                        {
                            item.Dispose();
                        }
                        catch (Exception ex)
                        {
                            last = last ?? ex;
                        }
                    }
                }
                if (last != null)
                    throw last;
            }
        }

        public void AddRange (IEnumerable<T> items)
        {
            if (items == null)
                return;
            base.UnionWith(items);
        }

        public IEnumerable<T> RemoveRange (IEnumerable<T> items)
        {
            if (items == null) return null;
            base.ExceptWith(items);
            return items;
        }
    }
}
