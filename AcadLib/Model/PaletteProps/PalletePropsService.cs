namespace AcadLib.PaletteProps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms.Integration;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.DatabaseServices;
    using Autodesk.AutoCAD.EditorInput;
    using Autodesk.AutoCAD.Windows;
    using Errors;
    using JetBrains.Annotations;
    using UI;
    using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

    /// <summary>
    /// Палитра свойств
    /// </summary>
    public static class PalletePropsService
    {
        public static readonly PalettePropsVM propsVM = new PalettePropsVM();
        private static readonly List<PalettePropsProvider> providers = new List<PalettePropsProvider>();
        private static PaletteSet palette;
        private static bool stop;

        /// <summary>
        /// Добавление провайдера
        /// </summary>
        /// <param name="name">Название</param>
        /// <param name="getTypes">Для выделенных объектов вернуть группы типов свойств. Транзакция уже запущена.</param>
        public static void Registry(string name, Func<ObjectId[], Document, List<PalettePropsType>> getTypes)
        {
            if (providers.Any(p => p.Name == name))
                throw new Exception($"Такой провайдер свойств палитры уже есть - '{name}'");
            providers.Add(new PalettePropsProvider(name, getTypes));
        }

        public static void Start()
        {
            stop = false;
            if (palette == null)
            {
                Application.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
                foreach (var doc in Application.DocumentManager)
                {
                    DocumentSelectionChangeSubscribe(doc as Document);
                }

                palette = new PaletteSet("ПИК Свойства", nameof(Commands.PIK_PaletteProperties),
                    new Guid("F1FFECA8-A9AE-47D6-8682-752D6AF1A15B"));
                palette.StateChanged += Palette_StateChanged;
                var propsView = new PalettePropsView(propsVM);
                var host = new ElementHost { Child = propsView };
                palette.Add("Свойства", host);

                // palette.AddVisual("Свойства", propsView);
            }

            palette.Visible = true;
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            DocumentSelectionChangeSubscribe(e.Document);
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
            if (doc == null)
                return;
            doc.ImpliedSelectionChanged += Document_ImpliedSelectionChanged;
        }

        private static void Document_ImpliedSelectionChanged(object sender, EventArgs e)
        {
            if (stop || !providers.Any())
                return;
            try
            {
                ShowSelection();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex);
            }
        }

        private static void ShowSelection()
        {
            var doc = AcadHelper.Doc;
            var sel = doc.Editor.SelectImplied();
            if (providers.Any() && sel.Status != PromptStatus.OK || sel.Value.Count == 0)
            {
                // Очистить палитру свойств
                propsVM.Clear();
                return;
            }

            var ids = sel.Value.GetObjectIds();

            // группы по типу объектов
            var groups = new List<PalettePropsType>();
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                foreach (var provider in providers)
                {
                    try
                    {
                        var types = provider.GetTypes(ids, doc)
                            .Where(w => w?.Groups?.Any(g => g?.Properties?.Any() == true) == true);
                        groups.AddRange(types);
                    }
                    catch (Exception ex)
                    {
                        Inspector.AddError($"Ошибка обработки группы свойств '{provider.Name}' - {ex}");
                    }
                }

                t.Commit();
            }

            if (groups.Count == 0)
            {
                propsVM.Clear();
            }
            else
            {
                if (groups.Count > 1)
                {
                    // Добавить тип "Все"
                    var allType = new PalettePropsType
                    {
                        Name = "Все",
                        Groups = groups.Where(w => w.Groups?.Any() == true).SelectMany(s => s.Groups).ToList()
                    };
                    groups.Insert(0, allType);
                }

                propsVM.Types = groups;
                propsVM.SelectedType = groups[0];
            }

            Inspector.Show();
        }
    }
}