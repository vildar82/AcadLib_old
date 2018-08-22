namespace AcadLib.UI.Ribbon
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using AcadLib.PaletteCommands;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.Private.Windows;
    using Autodesk.Windows;
    using Elements;
    using Files;
    using JetBrains.Annotations;
    using MicroMvvm;
    using NetLib;
    using Options;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    /// <summary>
    ///     Создает ленту
    /// </summary>
    public static class RibbonBuilder
    {
        private static readonly LocalFileData<RibbonOptions> ribbonOptions;
        private static bool isInitialized;
        private static RibbonControl ribbon;

        static RibbonBuilder()
        {
            // Загрузка настроек ленты
            ribbonOptions = FileDataExt.GetLocalFileData<RibbonOptions>("Ribbon", "RibbonOptions", false);
            ribbonOptions.TryLoad();
            if (ribbonOptions.Data == null)
            {
                ribbonOptions.Data = new RibbonOptions();
            }
        }

        public static void InitRibbon()
        {
            if (ribbon != null || isInitialized)
                return;
            isInitialized = true;
            ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
        }

        internal static void CreateRibbon()
        {
            try
            {
                foreach (var palette in PaletteSetCommands._paletteSets)
                {
                    var elems = palette.Commands.Where(w => PaletteSetCommands.IsAccess(w.Access))
                        .Select(c => ConvertToRibbonElement(c, palette.Name));
                    CreateRibbon(elems, palette.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CreateRibbon");
            }
        }

        private static void AddItem<T>(int index, [NotNull] T item, [NotNull] IList<T> items)
            where T : IRibbonContentUid
        {
            if (index > items.Count)
            {
                index = ribbon.Tabs.Count;
            }
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
                CreateRibbon();
        }

        private static void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            ribbon = ComponentManager.Ribbon;
            if (ribbon == null)
                return;
            ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;
            CreateRibbon();
            Application.SystemVariableChanged += Application_SystemVariableChanged;
        }

        [NotNull]
        private static RibbonElement ConvertPaletteCommand([NotNull] IPaletteCommand c, string paletteName)
        {
            return new RibbonElement
            {
                Command = new RelayCommand(c.Execute),
                Image = c.Image,
                LargeImage = c.Image,
                Name = c.Name,
                Tab = paletteName,
                Panel = c.Group,
                Description = c.Description
            };
        }

        [NotNull]
        private static IRibbonElement ConvertToRibbonElement([NotNull] IPaletteCommand c, string paletteName)
        {
            if (c is SplitCommand splitCommand)
            {
                return new SplitElement
                {
                    Items = splitCommand.Commands.Select(s => ConvertPaletteCommand(s, paletteName)).ToList(),
                    Tab = paletteName,
                    Panel = c.Group
                };
            }

            return ConvertPaletteCommand(c, paletteName);
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
        private static ItemOptions CreatePanel(
            string panelName,
            [NotNull] IEnumerable<IRibbonElement> elements,
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
                    RibbonItem item;
                    if (element is SplitElement splitElem)
                        item = CreateSplitButton(splitElem);
                    else
                        item = CreateButton(element);
                    row.Items.Add(item);
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

        private static void CreateRibbon(IEnumerable<IRibbonElement> elements, string tabName)
        {
            try
            {
                if (ribbon == null)
                    ribbon = ComponentManager.Ribbon;
                ribbon.Tabs.CollectionChanged -= Tabs_CollectionChanged;
                if (ribbon.FindTab(tabName) != null)
                    return;

                // группировка элементов по вкладкам
                var tabsOpt = elements.GroupBy(g => g.Tab).Select(t => CreateTab(t.Key, t.ToList())).ToList();
                foreach (var tabOpt in tabsOpt)
                {
                    var tab = (RibbonTab)tabOpt.Item;
                    if (tab == null)
                        continue;
                    AddItem(tabOpt.Index, tab, ribbon.Tabs);
                    tab.Panels.CollectionChanged += Panels_CollectionChanged;
                    tab.PropertyChanged += Tab_PropertyChanged;
                }

                var activeTab = (RibbonTab)ribbonOptions.Data.Tabs.FirstOrDefault(t => t.UID == ribbonOptions.Data.ActiveTab)?.Item;
                if (activeTab != null)
                {
                    ribbon.ActiveTab = activeTab;
                }

                ribbon.Tabs.CollectionChanged += Tabs_CollectionChanged;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CreateRibbon");
            }
        }

        private static void Tab_PropertyChanged(object sender, [NotNull] PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RibbonTab.IsVisible):
                    var tab = (RibbonTab)sender;
                    var tabOpt = ribbonOptions.Data.Tabs.FirstOrDefault(t => t.UID == tab.UID);
                    if (tabOpt == null)
                        return;
                    tabOpt.IsVisible = tab.IsVisible;
                    SaveOptions();
                    break;
                case nameof(RibbonTab.IsActive):
                    SaveActiveTab();
                    break;
            }
        }

        public static void SaveActiveTab()
        {
            if (ribbon?.ActiveTab != null)
            {
                ribbonOptions.Data.ActiveTab = ribbon.ActiveTab.UID;
                ribbonOptions.TrySave();
            }
        }

        [NotNull]
        private static RibbonSplitButton CreateSplitButton([NotNull] SplitElement splitElem)
        {
            var splitB = new RibbonSplitButton();
            foreach (var elem in splitElem.Items)
            {
                var button = CreateButton(elem);
                splitB.Items.Add(button);
            }

            return splitB;
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
            var tabOptions = GetItemOptions(tab, ribbonOptions.Data.Tabs);
            ribbonOptions.Data.Tabs = ribbonOptions.Data.Tabs.Where(w => w.Item != null).ToList();
            tab.IsVisible = tabOptions.IsVisible;
            tabOptions.Items = elements.GroupBy(g => g.Panel).Select(p => CreatePanel(p.Key, p.ToList(), tabOptions))
                .OrderBy(o => o.Index).ToList();
            foreach (var panelOpt in tabOptions.Items)
            {
                var panel = (RibbonPanel)panelOpt.Item;
                if (panel == null)
                    continue;
                tab.Panels.Add(panel);
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
                Image = element.LargeImage
            };
        }

        private static void Panel_PropertyChanged(object sender, [NotNull] PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible")
            {
                var panel = (RibbonPanel)sender;
                var tab = panel.Tab;
                if (tab == null)
                    return;
                var tabOpt = ribbonOptions.Data.Tabs.FirstOrDefault(t => t.UID == tab.UID);
                if (tabOpt == null)
                    return;
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
                if (tab == null)
                    return;
                var tabOptions = ribbonOptions.Data.Tabs.FirstOrDefault(t => t.UID == tab.UID);
                if (tabOptions == null)
                    return;
                for (var index = 0; index < ribbonPanelCol.Count; index++)
                {
                    var panel = ribbonPanelCol[index];
                    var panelOpt = tabOptions.Items.FirstOrDefault(p => p.UID == panel.UID);
                    if (panelOpt != null)
                        panelOpt.Index = index;
                }

                SaveOptions();
            }
        }

        [CanBeNull]
        private static BitmapSource ResizeImage([CanBeNull] BitmapSource image, int size)
        {
            return image == null
                ? null
                : new TransformedBitmap(image, new ScaleTransform(size / image.Width, size / image.Height));
        }

        private static void SaveOptions()
        {
            foreach (var tabOpt in ribbonOptions.Data.Tabs)
            {
                var tab = (RibbonTab)tabOpt.Item;
                if (tab == null)
                    continue;
                tabOpt.IsVisible = tab.IsVisible;
                foreach (var panelOpt in tabOpt.Items)
                {
                    var panel = (RibbonPanel)panelOpt.Item;
                    if (panel == null)
                        continue;
                    panelOpt.IsVisible = panel.IsVisible;
                }
            }

            Debug.WriteLine("RibbonBuilder SaveOptions");
            ribbonOptions.TrySave();
        }

        private static void Tabs_CollectionChanged(object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var tab in ribbonOptions.Data.Tabs)
                {
                    var index = ribbon.Tabs.IndexOf((RibbonTab)tab.Item);
                    if (index == -1)
                        continue;
                    tab.Index = index;
                }

                SaveOptions();
            }
        }
    }
}