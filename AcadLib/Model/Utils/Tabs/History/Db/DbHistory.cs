namespace AcadLib.Utils.Tabs.History.Db
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using DynamicData;
    using Model.Utils.Tabs.History.Db;
    using NetLib;
    using Properties;

    public class DbHistory
    {
        private Entities db;

        public DbHistory()
        {
            var sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "vpp-sql04",
                InitialCatalog = "StatEvents",
                IntegratedSecurity = false,
                UserID = "CAD_AllUsers",
                Password = "qwerty!2345",
            };
            var conBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ToString(),
                Metadata = @"res://*/Model.Utils.Tabs.History.Db.DbEvents.csdl|res://*/Model.Utils.Tabs.History.Db.DbEvents.ssdl|res://*/Model.Utils.Tabs.History.Db.DbEvents.msl"
            };
            var con = conBuilder.ToString();
            db = new Entities(con);
        }

        public IQueryable<StatEvents> LoadHistoryFiles()
        {
            var login = Environment.UserName.ToLower();
            return db.StatEvents.Where(w => w.App == "AutoCAD" || w.App == "Civil")
                .Where(w => w.EventName == "Открытие")
                .Where(w => w.UserName.ToLower() == login)
                .GroupBy(g => g.DocPath).Select(s => s.OrderByDescending(o => o.Start).FirstOrDefault());
        }
    }
}
