﻿namespace AcadLib.User.DB
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.SqlClient;
    using System.Linq;
    using JetBrains.Annotations;
#if Utils
    using UtilsEditUsers.Model.User.DB;
#else
    using AcadLib.Model.User.DB;
#endif

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
                Metadata = @"res://*/Model.User.DB.Users.csdl|res://*/Model.User.DB.Users.ssdl|res://*/Model.User.DB.Users.msl"
            };
            var conStr = conBuilder.ToString();
            entities = new CAD_AutoCADEntities(new EntityConnection(conStr));
        }

        public List<AutocadUsers> GetUsers()
        {
            return entities.AutocadUsers.ToList();
        }

        public void DeleteUser(AutocadUsers user)
        {
            entities.AutocadUsers.Remove(user);
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