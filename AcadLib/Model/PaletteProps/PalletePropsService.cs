using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AcadLib.Errors;
using AcadLib.PaletteProps.UI;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using JetBrains.Annotations;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = System.Exception;

namespace AcadLib.PaletteProps
{
    /// <summary>
    /// Палитра свойств
    /// </summary>
    public static class PalletePropsService
    {
        private static PaletteSet palette;
        private static readonly PalettePropsVM propsVM = new PalettePropsVM();
        private static bool stop;
        private static readonly List<PalettePropsProvider> providers = new List<PalettePropsProvider>();

        /// <summary>
        /// Добавление провайдера
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="getTypes">Для выделенных объектов вернуть группы типов свойств</param>
        public static void Registry(string name, Func<ObjectId[], Document, List<PalettePropsType>> getTypes)
        {
            if (providers.Any(p => p.Name == name))
                throw new Exception($"Такой провайдер свойств палитры уже есть - '{name}'");
            providers.Add(new PalettePropsProvider(name, getTypes));
        }

        [CommandMethod("PIK_PaletteProperties")]
        public static void Start()
        {
            Application.DocumentManager.DocumentCreated +=
                (sender, args) => DocumentSelectionChangeSubscribe(args.Document);
            foreach (var doc in Application.DocumentManager)
            {
                DocumentSelectionChangeSubscribe(doc as Document);
            }

            stop = false;
            if (palette == null)
            {
                palette = new PaletteSet("ПИК Свойства", "PIK_PaletteProperties", new Guid("F1FFECA8-A9AE-47D6-8682-752D6AF1A15B"));
                palette.StateChanged += Palette_StateChanged;
                var propsView = new PalettePropsView(propsVM);
                palette.AddVisual("Свойства", propsView);
            }
            palette.Visible = true;
        }

        private static void Palette_StateChanged(object sender, [NotNull] PaletteSetStateEventArgs e)
        {
            switch (e.NewState)
            {
                case StateEventIndex.Hide:
                    stop = true;
                    break;
                case StateEventIndex.Show:
                    stop = false;
                    break;
            }
        }

        private static void DocumentSelectionChangeSubscribe([CanBeNull] Document doc)
        {
            if (doc == null) return;
            doc.ImpliedSelectionChanged += Document_ImpliedSelectionChanged;
        }

        private static void Document_ImpliedSelectionChanged(object sender, EventArgs e)
        {
            if (stop || !providers.Any()) return;
            Debug.WriteLine("Document_ImpliedSelectionChanged");
            ShowSelection();
        }

        private static void ShowSelection()
        {
            var doc = AcadHelper.Doc;
            var sel = doc.Editor.SelectImplied();
            if (sel.Status != PromptStatus.OK || sel.Value.Count == 0)
            {
                // Очистить палитру свойств
                propsVM.Clear();
                return;
            }
            var ids = sel.Value.GetObjectIds();
            // группы по типу объектов
            var groups = new List<PalettePropsType>();
            foreach (var provider in providers)
            {
                try
                {
                    groups.AddRange(provider.GetTypes(ids, doc));
                }
                catch (Exception ex)
                {
                    Inspector.AddError($"Ошибка обработки группы свойств '{provider.Name}' - {ex}");
                }
            }
            propsVM.Types = groups;
        }
    }
}