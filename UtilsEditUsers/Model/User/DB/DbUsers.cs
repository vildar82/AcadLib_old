using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
#if Utils
using UtilsEditUsers.Model.User.DB;
#else
using AcadLib.Model.User.DB;
#endif
using JetBrains.Annotations;


namespace AcadLib.User.DB
{
    [PublicAPI]
    public class DbUsers : IDisposable
    {
        private readonly CAD_AutoCADEntities entities;

        public DbUsers()
        {
            var sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = "vpp-sql04",
                InitialCatalog = "CAD_AutoCAD",
                IntegratedSecurity = false,
                UserID = "CAD_AllUsers",
                Password = "qwerty!2345",
            };
            var conBuilder = new EntityConnectionStringBuilder 
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ToString(),
                Metadata =  @"res://*/Model.User.DB.Users.csdl|res://*/Model.User.DB.Users.ssdl|res://*/Model.User.DB.Users.msl"
            };
            //res://*/source.DB.Users.csdl|res://*/source.DB.Users.ssdl|res://*/source.DB.Users.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=vpp-sql04;initial catalog=CAD_AutoCAD;persist security info=True;user id=CAD_AllUsers;password=qwerty!2345;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
            var conStr = conBuilder.ToString();
            entities = new CAD_AutoCADEntities(new EntityConnection(conStr));
        }

        public List<AutocadUsers> GetUsers()
        {
            return entities.AutocadUsers.ToList();
        }

        public void Save()
        {
            entities.SaveChanges();
        }

        public void Dispose()
        {
            entities?.Dispose();
        }
    }
}
