using System;
using System.Collections.Generic;
using System.Linq;
using AcadLib.PaletteProps;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using JetBrains.Annotations;

namespace TestAcadlib.PaletteProps
{
    public class TestPaletteProps
    {
        [CommandMethod(nameof(TestPalettePropsCom))]
        public void TestPalettePropsCom()
        {
            PalletePropsService.Registry("Test", GetTypes);
        }

        [NotNull]
        private List<PalettePropsType> GetTypes([NotNull] ObjectId[] ids, Document doc)
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
            types.AddRange(Enumerable.Range(0,7).Select(s=> new PalettePropsType
            {
                Name = $"Type{s}",
                Groups = Enumerable.Range(0,3).Select(g=>new PalettePropsGroup
                {
                    Name = $"Group{g}",
                    Properties = Enumerable.Range(0,10).Select(p=> new PalettePropVM{ Name = $"Prop{p}"}).ToList()
                }).ToList()
            }));
            return types;
        }

        [NotNull]
        private List<PalettePropVM> GetProperties(List<Entity> ents)
        {
            return new List<PalettePropVM>
            {
                GetProp(ents, nameof(Entity.Color)),
            };
        }

        [NotNull]
        private PalettePropVM GetProp(List<Entity> ents, string propName)
        {
            return new PalettePropVM
            {
                Name = "Цвет"
            };
        }
    }
}
