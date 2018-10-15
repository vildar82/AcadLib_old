namespace AcadLib.DB
{
    using Autodesk.AutoCAD.DatabaseServices;

    public static class SummaryInfoExt
    {
        public static object GetDwgCustomPropertyValue(this Database db, string prop)
        {
            var dictProps = db.SummaryInfo.CustomProperties;
            while (dictProps.MoveNext())
            {
                var entry = dictProps.Entry;
                if (entry.Key == prop)
                {
                    return entry.Value;
                }
            }

            return null;
        }
    }
}
