using AcadLib.IO;
using AcadLib.PaletteCommands;
using AcadLib.UI.Ribbon.Elements;
using AcadLib.UI.Ribbon.Options;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Private.Windows;
using Autodesk.Windows;
using JetBrains.Annotations;
using MicroMvvm;
using NetLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.UI.Ribbon
{
    /// <summary>
    /// Создает ленту
    /// </summary>
    public static class RibbonBuilder
    {
        private static readonly RibbonOptions ribbonOptions;
        private static readonly JsonLocalData<RibbonOptions> ribbonOptionsData;
        private static bool isInitialized;
        private static RibbonControl ribbon;

        static RibbonBuilder()
        {
            // Загрузка настроек ленты
            ribbonOptionsData = new JsonLocalData<RibbonOptions>("Ribbon", "RibbonOptions");
            ribbonOptions = ribbonOptionsData.TryLoad() ?? new RibbonOptions();
        }

        public static void InitRibbon()
        {
            if (ribbon != null || isInitialized) return;
            isInitialized = true;
            ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
        }

        internal static void CreateRibbon()
        {
            try
            {
                foreach (var palette in PaletteSetCommands._paletteSets)
                {
                    CreateRibbon(palette.Commands.Where(w => PaletteSetCommands.IsAccess(w.Access))
                        .Select(c => new RibbonElement
                        {
                            Command = new RelayCommand(c.Execute),
                            Image = c.Image,
                            LargeImage = c.Image,
                            Name = c.Name,
                            Tab = palette.Name,
                            Panel = c.Group,
                            Description = c.Description,
                        }).ToList<IRibbonElement>());
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CreateRibbon");
            }
        }

        private static void AddItem<T>(int index, [NotNull] T item, [NotNull] IList<T> items) where T : IRibbonContentUid
        {
            if (index > items.Count) index = ribbon.Tabs.Count;
            else if (index < 0)
            {
                items.Add(item);
                return;
            }
            items.Insert(index, item);
        }

        private static void Application_SystemVariableChanged(object sender, [NotNull] SystemVariableChangedEventArgs e)
        {
            if (e.Name.Equals("WSCURRENT"))
            {
                CreateRibbon();
            }
        }

        private static void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            ribbon = ComponentManager.Ribbon;
            if (ribbon == null) return;
            ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;
            CreateRibbon();
            Application.SystemVariableChanged += Application_SystemVariableChanged;
        }

        [NotNull]
        private static RibbonButton CreateButton([NotNull] IRibbonElement element)
        {
            var button = new RibbonButton
            {
                CommandHandler = element.Command,
                Text = element.Name, // Текст рядом с кнопкой, если ShowText = true
                Name = element.Name, // Тест на всплявающем окошке (заголовов)
                Description = element.Description, // Описание на всплывающем окошке
                LargeImage = ResizeImage(element.LargeImage as BitmapSource, 32),
                Image = ResizeImage(element.Image as BitmapSource, 16),
                ToolTip = GetToolTip(element),
                IsToolTipEnabled = true,
                ShowImage = element.LargeImage != null || element.Image != null,
                ShowText = false,
                Size = RibbonItemSize.Large
            };
            return button;
        }

        [NotNull]
        private static ItemOptions CreatePanel(string panelName, IEnumerable<IRibbonElement> elements,
            [NotNull] ItemOptions tabOptions)
        {
            var name = panelName.IsNullOrEmpty() ? "Главная" : panelName;
            var panelSource = new RibbonPanelSource
            {
                Name = name,
                Id = panelName,
                Title = name,
                UID = name
            };
            foreach (var part in elements.SplitParts(2))
            {
                var row = new RibbonRowPanel();
                foreach (var element in part)
                {
                    var button = CreateButton(element);
                    row.Items.Add(button);
                }
                panelSource.Items.Add(row);
                panelSource.Items.Add(new RibbonRowBreak());
            }
            var panel = new RibbonPanel { Source = panelSource, UID = panelSource.UID };
            var panelOpt = GetItemOptions(panel, tabOptions.Items);
            panel.IsVisible = panelOpt.IsVisible;
            panel.PropertyChanged += Panel_PropertyChanged;
            return panelOpt;
        }

        private static void CreateRibbon(IEnumerable<IRibbonElement> elements)
        {
            try
            {
                if (ribbon == null) ribbon = ComponentManager.Ribbon;
                ribbon.Tabs.CollectionChanged -= Tabs_CollectionChanged;
                // группировка элементов по вкладкам
                var tabsOpt = elements.GroupBy(g => g.Tab).Select(t => CreateTab(t.Key, t.ToList()));
                foreach (var tabOpt in tabsOpt)
                {
                    var tab = (RibbonTab)tabOpt.Item;
                    AddItem(tabOpt.Index, tab, ribbon.Tabs);
                    tab.Panels.CollectionChanged += Panels_CollectionChanged;
                    tab.PropertyChanged += Tab_PropertyChanged;
                }
                ribbon.Tabs.CollectionChanged += Tabs_CollectionChanged;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CreateRibbon");
            }
        }

        [NotNull]
        private static ItemOptions CreateTab(string tabName, [NotNull] IEnumerable<IRibbonElement> elements)
        {
            var tab = new RibbonTab
            {
                Title = tabName,
                Name = tabName,
                Id = tabName,
                UID = tabName
            };
            var tabOptions = GetItemOptions(tab, ribbonOptions.Tabs);
            tab.IsVisible = tabOptions.IsVisible;
            tabOptions.Items = elements.GroupBy(g => g.Panel).Select(p => CreatePanel(p.Key, p.ToList(), tabOptions))
                .OrderBy(o => o.Index).ToList();
            foreach (var panelOpt in tabOptions.Items)
            {
                tab.Panels.Add((RibbonPanel)panelOpt.Item);
            }
            return tabOptions;
        }

        [NotNull]
        private static ItemOptions GetItemOptions<T>([NotNull] T item, [NotNull] List<ItemOptions> itemOptions)
            where T : IRibbonContentUid
        {
            var tabOption = itemOptions.FirstOrDefault(t => t.UID.Equals(item.UID));
            if (tabOption == null)
            {
                tabOption = new ItemOptions
                {
                    UID = item.UID,
                    Index = 0,
                    Item = item
                };
                itemOptions.Add(tabOption);
            }
            else
            {
                tabOption.Item = item;
            }
            return tabOption;
        }

        [NotNull]
        private static RibbonToolTip GetToolTip([NotNull] IRibbonElement element)
        {
            return new RibbonToolTip
            {
                Title = element.Name,
                Content = element.Description,
                IsHelpEnabled = false,
                Image = element.LargeImage,
            };
        }

        private static void Panel_PropertyChanged(object sender, [NotNull] System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                var panel = (RibbonPanel)sender;
                var tab = panel.Tab;
                if (tab == null) return;
                var tabOpt = ribbonOptions.Tabs.FirstOrDefault(t => t.UID == tab.UID);
                if (tabOpt == null) return;
                panel.IsVisible = panel.IsVisible;
                SaveOptions();
            }
        }

        private static void Panels_CollectionChanged(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                var ribbonPanelCol = sender as RibbonPanelCollection;
                var tab = ribbonPanelCol?.FirstOrDefault()?.Tab;
                if (tab == null) return;
                var tabOptions = ribbonOptions.Tabs.FirstOrDefault(t => t.UID == tab.UID);
                if (tabOptions == null) return;
                for (var index = 0; index < ribbonPanelCol.Count; index++)
                {
                    var panel = ribbonPanelCol[index];
                    var panelOpt = tabOptions.Items.FirstOrDefault(p => p.UID == panel.UID);
                    if (panelOpt != null)
                    {
                        panelOpt.Index = index;
                    }
                }
                SaveOptions();
            }
        }

        [CanBeNull]
        private static BitmapSource ResizeImage([CanBeNull] BitmapSource image, int size)
        {
            return image == null ? null
                : new TransformedBitmap(image, new ScaleTransform(size / image.Width, size / image.Height));
        }

        private static void SaveOptions()
        {
            foreach (var tabOpt in ribbonOptions.Tabs)
            {
                var tab = (RibbonTab)tabOpt.Item;
                tabOpt.IsVisible = tab.IsVisible;
                foreach (var panelOpt in tabOpt.Items)
                {
                    var panel = (RibbonPanel)panelOpt.Item;
                    panelOpt.IsVisible = panel.IsVisible;
                }
            }
            Debug.WriteLine("RibbonBuilder SaveOptions");
            ribbonOptionsData.TrySave(ribbonOptions);
        }

        private static void Tab_PropertyChanged(object sender, [NotNull] System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                var tab = (RibbonTab)sender;
                var tabOpt = ribbonOptions.Tabs.FirstOrDefault(t => t.UID == tab.UID);
                if (tabOpt == null) return;
                tabOpt.IsVisible = tab.IsVisible;
                SaveOptions();
            }
        }

        private static void Tabs_CollectionChanged(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var tab in ribbonOptions.Tabs)
                {
                    var index = ribbon.Tabs.IndexOf((RibbonTab)tab.Item);
                    if (index == -1)
                    {
                        return;
                    }
                    tab.Index = index;
                }
                SaveOptions();
            }
        }
    }
}