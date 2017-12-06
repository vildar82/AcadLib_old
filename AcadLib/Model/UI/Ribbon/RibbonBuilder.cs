using AcadLib.PaletteCommands;
using AcadLib.UI.Ribbon.Elements;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Windows;
using JetBrains.Annotations;
using NetLib;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MicroMvvm;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace AcadLib.UI.Ribbon
{
    /// <summary>
    /// Создает ленту
    /// </summary>
    public static class RibbonBuilder
    {
        private static RibbonControl ribbon;
        private static bool isInitialized;

        public static void InitRibbon()
        {
            if (ribbon != null || isInitialized) return;
            isInitialized = true;
            ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
        }

        private static void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
        {
            ribbon = ComponentManager.Ribbon;
            if (ribbon == null) return;
            ComponentManager.ItemInitialized -= ComponentManager_ItemInitialized;
            ribbon.BackgroundRenderFinished += Ribbon_BackgroundRenderFinished;
        }

        private static void Ribbon_BackgroundRenderFinished(object sender, EventArgs e)
        {
            ribbon.BackgroundRenderFinished -= Ribbon_BackgroundRenderFinished;
            CreateRibbon();
            Application.SystemVariableChanged += Application_SystemVariableChanged;
        }

        private static void Application_SystemVariableChanged(object sender, [NotNull] SystemVariableChangedEventArgs e)
        {
            if (e.Name.Equals("WSCURRENT"))
            {
                CreateRibbon();
            }
        }

        private static void CreateRibbon()
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

        private static void CreateRibbon(List<IRibbonElement> elements)
        {
            try
            {
                // группировка элементов по вкладкам
                foreach (var tabElems in elements.GroupBy(g => g.Tab))
                {
                    var tab = CreateTab(tabElems.Key, tabElems.ToList());
                    ribbon.Tabs.Insert(0, tab);
                }
                ribbon.UpdateLayout();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex, "CreateRibbon");
            }
        }

        [NotNull]
        private static RibbonTab CreateTab(string tabName, [NotNull] List<IRibbonElement> elements)
        {
            var tab = new RibbonTab
            {
                Title = tabName,
                Name = tabName,
                Id = tabName
            };
            foreach (var panelElems in elements.GroupBy(g => g.Panel))
            {
                var panel = CreatePanel(panelElems.Key, panelElems.ToList());
                tab.Panels.Add(panel);
            }
            return tab;
        }

        [NotNull]
        private static RibbonPanel CreatePanel(string panelName, List<IRibbonElement> elements)
        {
            var name = panelName.IsNullOrEmpty() ? "Главная" : panelName;
            var panelSource = new RibbonPanelSource
            {
                Name = name,
                Id = panelName,
                Title = name
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
            var panel = new RibbonPanel { Source = panelSource };
            return panel;
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

        [NotNull]
        private static BitmapSource ResizeImage([NotNull] BitmapSource image, int size)
        {
            return new TransformedBitmap(image, new ScaleTransform(size / image.Width, size / image.Height));
        }
    }
}
