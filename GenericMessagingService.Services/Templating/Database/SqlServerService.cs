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
        private readonly DatabaseTemplateSettings settings;

        public SqlServerService(DatabaseTemplateSettings settings) 
        {
            this.settings = settings;
        }

        public async Task<(string Template, string Subject)?> GetTemplate(string templateName)
        {
            try
            {
                var query = !string.IsNullOrEmpty(settings.SubjectColumn)
                    ? $@"SELECT {settings.TemplateColumn}, {settings.SubjectColumn}
                    FROM {settings.Table}
                    WHERE {settings.LookupColumn} = '{templateName}'"
                    : $@"SELECT {settings.TemplateColumn}
                    FROM {settings.Table}
                    WHERE {settings.LookupColumn} = '{templateName}'";
                string template = null;
                string subject = null;
                using (var connection = new SqlConnection(settings.ConnectionString))
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        template = reader[settings.TemplateColumn].ToString();
                        var subjectColumn = reader[settings.SubjectColumn];
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
