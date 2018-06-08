using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AcadLib;
using AcadLib.PaletteProps;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;
using ReactiveUI;
using MathExt = NetLib.MathExt;

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
            foreach (var typeEnts in ids.GetObjects<Entity>().GroupBy(g=>g.GetType()))
            {
                var ents = typeEnts.ToList();
                var typeProps = new PalettePropsType
                {
                    Name = typeEnts.Key.Name,
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
            types.AddRange(Enumerable.Range(0,1).Select(s=> new PalettePropsType
            {
                Name = $"Type{s}",
                Groups = Enumerable.Range(0,5).Select(g=>new PalettePropsGroup
                {
                    Name = $"Group{g}",
                    Properties = Enumerable.Range(0,15).Select(p=> new PalettePropVM
                    {
                        Name = $"Prop{p}",
                        ValueControl = GetRandomValueControl(p, ids.ToList()),
                        Tooltip = $"Hello {s} {g} {p}"
                    }).ToList()
                }).ToList()
            }));
            return types;
        }

        private static Control GetRandomValueControl(int i, List<ObjectId> ids)
        {
            var ci = MathExt.IsEven(i);// rnd.Next(0, 1);
            if (ci)
            {
                var ivm = new IntListValueVM {Value = i, AllowCustomValue = DateTime.Now.Ticks % 2 == 0};
                ivm.WhenAnyValue(v => v.Value).Subscribe(s => UpdateValue(s, ids));
                return new IntListValueView(ivm);
            }
            var ilvm = new IntValueVM {Value = i, Min = 1, Max = 10};
            ilvm.WhenAnyValue(v => v.Value).Subscribe(s => UpdateValue(s, ids));
            return new IntValueView(ilvm);
        }

        [NotNull]
        private static List<PalettePropVM> GetProperties(List<Entity> ents)
        {
            return new List<PalettePropVM>
            {
                GetProp(ents, nameof(Entity.Color)),
            };
        }

        [NotNull]
        private static PalettePropVM GetProp(List<Entity> ents, string propName)
        {
            return new PalettePropVM
            {
                Name = propName,
                ValueControl =GetRandomValueControl(ents.Count, ents.Select(s=>s.Id).ToList())
            };
        }

        private static void UpdateValue(int value, List<ObjectId> ids)
        {
            var doc = AcadHelper.Doc;
            using (doc.LockDocument())
            using (var t = doc.TransactionManager.StartTransaction())
            {
                foreach (var circle in ids.GetObjects<Circle>(OpenMode.ForWrite))
                {
                    circle.Radius = value;
                }
                t.Commit();
            }
        }
    }
}
