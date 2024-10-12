using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Database
{
    internal class SqlServerService : IDatabaseService
    {
        private readonly EmailSettings settings;

        public SqlServerService(EmailSettings settings) 
        {
            this.settings = settings;
        }

        public async Task<(string Template, string Subject)?> GetTemplate(string templateName)
        {
            try
            {
                var dbSettings = settings.DatabaseTemplates;
                var query = !string.IsNullOrEmpty(dbSettings.SubjectColumn)
                    ? $@"SELECT {dbSettings.TemplateColumn}, {dbSettings.SubjectColumn}
                    FROM {dbSettings.Table}
                    WHERE {dbSettings.LookupColumn} = '{templateName}'"
                    : $@"SELECT {dbSettings.TemplateColumn}
                    FROM {dbSettings.Table}
                    WHERE {dbSettings.LookupColumn} = '{templateName}'";
                string template = null;
                string subject = null;
                using (var connection = new SqlConnection(dbSettings.ConnectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        template = reader[dbSettings.TemplateColumn].ToString();
                        var subjectColumn = reader[dbSettings.SubjectColumn];
                        if (subjectColumn != null)
                        {
                            subject = subjectColumn.ToString();
                        }
                    }
                }
                if (template != null) { return null; }
                return (template, subject);
            }
            catch (Exception ex) 
            { 
                return null; 
            }
        }
    }
}
