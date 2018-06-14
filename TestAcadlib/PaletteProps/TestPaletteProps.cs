using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using AcadLib;
using AcadLib.PaletteProps;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using ReactiveUI;
using MathExt = NetLib.MathExt;
using TransactionManager = Autodesk.AutoCAD.ApplicationServices.TransactionManager;

namespace TestAcadlib.PaletteProps
{
    public class TestPaletteProps
    {
        private static Random rnd = new Random();

        [CommandMethod(nameof(TestPalettePropsCom))]
        public void TestPalettePropsCom()
        {
            PalletePropsService.Registry("Test", GetTypes);
        }

        [NotNull]
        public static List<PalettePropsType> GetTypes([NotNull] ObjectId[] ids, Document doc)
        {
            var types = new List<PalettePropsType>();
            foreach (var typeEnts in ids.GetObjects<Circle>().GroupBy(g=>g.GetType()))
            {
                var ents = typeEnts.ToList();
                var typeProps = new PalettePropsType
                {
                    Name = "Круг",
                    Groups = new List<PalettePropsGroup>
                    {
                        new PalettePropsGroup
                        {
                            Name = "Entity",
                            Ids = ents.Select(s => s.Id).ToList(),
                            Properties = GetProperties(ents)
                        }
                    }
                };
                types.Add(typeProps);
            }
            //types.AddRange(Enumerable.Range(0,1).Select(s=> new PalettePropsType
            //{
            //    Name = $"Type{s}",
            //    Groups = Enumerable.Range(0,5).Select(g=>new PalettePropsGroup
            //    {
            //        Name = $"Group{g}",
            //        Properties = Enumerable.Range(0,15).Select(p=> new PalettePropVM
            //        {
            //            Name = $"Prop{p}",
            //            ValueControl = GetRandomValueControl(p, ids.ToList()),
            //            Tooltip = $"Hello {s} {g} {p}"
            //        }).ToList()
            //    }).ToList()
            //}));
            return types;
        }

        private static Control GetRandomValueControl(int i, List<ObjectId> ids)
        {
            var ci = MathExt.IsEven(i);// rnd.Next(0, 1);
            if (ci)
            {
                var ivm = new IntListValueVM {Value = i, AllowCustomValue = DateTime.Now.Ticks % 2 == 0};
                ivm.WhenAnyValue(v => v.Value).Skip(1).Subscribe(s => UpdateValue(s, ids));
                return new IntListValueView(ivm);
            }
            var ilvm = new IntValueVM {Value = i, Min = 1, Max = 10};
            ilvm.WhenAnyValue(v => v.Value).Skip(1).Subscribe(s => UpdateValue(s, ids));
            return new IntValueView(ilvm);
        }

        [NotNull]
        private static List<PalettePropVM> GetProperties(List<Circle> ents)
        {
            return new List<PalettePropVM>
            {
                GetIntProp(ents, "Целое", false),
                GetIntProp(ents, "Целое чтение", true),
                GetIntListProp(ents, "Список целых", false, true),
                GetIntListProp(ents, "Список целых", false, false),
                GetIntListProp(ents, "Список целых", true, true),
                GetIntListProp(ents, "Список целых", true, false)
            };
        }

        [NotNull]
        private static PalettePropVM GetIntProp(List<Circle> ents, string propName, bool isReadObly)
        {
            var vm = new IntValueVM
            {
                Value = GetValue(ents.GroupBy(g => (int) g.Radius).Select(s => s.Key)),
                IsReadOnly = isReadObly,
                Min = 1, Max = 1000
            };
            vm.WhenAnyValue(v => v.Value).Skip(1).Subscribe(s => UpdateValue(s, ents.Select(e => e.Id).ToList()));
            return new PalettePropVM
            {
                Name = propName,
                ValueControl = new IntValueView(vm),
                Tooltip = $"Help IntValueVM isReadObly={isReadObly}.",
            };
        }
        [NotNull]
        private static PalettePropVM GetIntListProp(List<Circle> ents, string propName, bool isReadObly, bool allowCustomValue)
        {
            var vm = new IntListValueVM
            {
                Values = new List<int> { 1, 10, 50, 100, 500, 1000 },
                Value = GetValue(ents.GroupBy(g => (int) g.Radius).Select(s => s.Key)), 
                AllowCustomValue = allowCustomValue,
                IsReadOnly = isReadObly,
                Min = 1, Max = 1000
            };
            vm.WhenAnyValue(v => v.Value).Skip(1).Subscribe(s => UpdateValue(s, ents.Select(e => e.Id).ToList()));
            return new PalettePropVM
            {
                Name = propName,
                ValueControl = new IntListValueView(vm),
                Tooltip = $"Help IntListValueVM isReadObly={isReadObly}, allowCustomValue={allowCustomValue}"
            };
        }

        private static int? GetValue(IEnumerable<int> values)
        {
            if (values.Skip(1).Any()) return null;
            return values.First();
        }

        private static void UpdateValue(int? value, List<ObjectId> ids)
        {
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                foreach (var circle in ids.GetObjects<Circle>(OpenMode.ForWrite))
                {
                    circle.Radius = value ?? 100;
                }
                t.Commit();
                Utils.FlushGraphics();
            }
        }
    }
}
