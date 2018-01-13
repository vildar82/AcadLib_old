using System;
using System.Drawing;
using JetBrains.Annotations;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

// ReSharper disable once CheckNamespace
namespace AcadLib.PaletteCommands
{
    /// <summary>
    /// Команда вставки блока из списка
    /// </summary>
    [PublicAPI]
    public class PaletteVisualInsertBlocks : PaletteCommand
    {
        private readonly string file;
        private readonly Predicate<string> filter;

        public PaletteVisualInsertBlocks(Predicate<string> filter, string file, string name, Bitmap image,
            string description, string group = "", bool isTest = false)
            : base(name, image, "", description, group, isTest)
        {
            this.file = file;
            this.filter = filter;
        }

        public override void Execute()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            using (doc.LockDocument())
            {
                Blocks.Visual.VisualInsertBlock.InsertBlock(file, filter);
            }
        }
    }
}