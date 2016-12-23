using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadLib.Extensions
{
    public static class EntityExt
    {
        /// <summary>
        /// Установка аннотативности объекту и масштаба с удалением текущего масштаба чертежа.
        /// </summary>
        /// <param name="ent">Объект поддерживающий аннотативность (текст, размер и т.п.)</param>
        /// <param name="scale">Масштаб в виде 100, 25 и т.п.</param>
        public static void SetAnnotativeScale(this Entity ent, int scale)
        {
            // Проверка, есть ли нужный масштаб в чертеже
            string nameScale = string.Format("1:{0}", scale);
            ObjectContextManager ocm = ent.Database.ObjectContextManager;
            ObjectContextCollection occ = ocm.GetContextCollection("ACDB_ANNOTATIONSCALES");
            ObjectContext contextAnnoScale;
            if (!occ.HasContext(nameScale))
            {
                AnnotationScale annoScale = new AnnotationScale();
                annoScale.Name = nameScale;
                annoScale.PaperUnits = 1;
                annoScale.DrawingUnits = scale;
                occ.AddContext(annoScale);
                contextAnnoScale = annoScale;
            }
            else
            {
                contextAnnoScale = occ.GetContext(nameScale);
            }
            ent.Annotative = AnnotativeStates.True;
            ent.AddContext(contextAnnoScale);
            ent.RemoveContext(ent.Database.Cannoscale);
        }
    }
}
