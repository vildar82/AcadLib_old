namespace WpfApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using NetLib.WPF;

    public class Model : BaseViewModel
    {
        public Model()
        {
            //var events = EventManager.GetRoutedEvents();
            //foreach (var routedEvent in events)
            //{
            //    EventManager.RegisterClassHandler(typeof(Window), 
            //        routedEvent, 
            //        new RoutedEventHandler(handler));
            //}

            Items = Enumerable.Range(1, 5).Select(s => new Item {  }).ToList();
        }

        private void handler(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("RoutedEvent: " + e.OriginalSource + "=>" + e.RoutedEvent + "; " + e.Source);
        }

        public List<Item> Items { get; set; }
    }

    public class Item : BaseModel
    {
        public bool Restore { get; set; }
    }
}
