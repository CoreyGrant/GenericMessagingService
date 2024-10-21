using GenericMessagingService.Types.Config;

namespace GenericMessagingService.WebApi.Setup
{
    public class ConfigValidator
    {
        private readonly ILogger<ConfigValidator> logger;

        public ConfigValidator(ILogger<ConfigValidator> logger) 
        {
            this.logger = logger;
        }

        public void Validate(AppSettings? appSettings)
        {
            if (appSettings == null)
            {
                throw new ConfigValidationException("App settings cannot be null");
            }
            var email = appSettings.Email;
            var sms = appSettings.Sms;
            var template = appSettings.Template;
            var comboTemplate = appSettings.ComboTemplate;

            if (email == null && sms == null && template == null && comboTemplate == null)
            {
                throw new ConfigValidationException("No features configured");
            }

            ValidateEmail(appSettings);
            ValidateSms(appSettings);
            
            if(template != null && comboTemplate != null)
            {
                throw new ConfigValidationException("Both template and comboTemplate has been configured");
            }
            ValidateTemplate(template);
        }

        private void ValidateEmail(AppSettings settings)
        {
            var email = settings.Email;
            var template = settings.Template;
            var comboTemplate = settings.ComboTemplate;
            if (email != null)
            {
                if (template == null && comboTemplate == null)
                {
                    logger.LogInformation("Email configured without templates, only plain emails will be possible");
                }

                var sendGrid = email.SendGrid;
                var mailChimp = email.MailChimp;
                var folder = email.Folder;
                var smtp = email.Smtp;
                var emailSections = new object?[] { sendGrid, mailChimp, folder, smtp }.Where(x => x != null);

                if (!string.IsNullOrEmpty(email.OverrideToAddress))
                {
                    logger.LogInformation("Override to address configured. All emails will be sent to " + email.OverrideToAddress);
                }
                if (!string.IsNullOrEmpty(email.DefaultFromAddress))
                {
                    logger.LogInformation("Default from address configured. Any emails without an explicit from address will be from " + email.DefaultFromAddress);
                }

                if (!emailSections.Any())
                {
                    throw new ConfigValidationException("Email section included with no sender configured");
                }
                else if (emailSections.Count() > 1)
                {
                    throw new ConfigValidationException("Email section included with multiple senders configured");
                }
                if (sendGrid != null)
                {
                    if (string.IsNullOrEmpty(sendGrid.ApiKey))
                    {
                        throw new ConfigValidationException("Email SendGrid ApiKey is required");
                    }
                }
                if (mailChimp != null) { }
                if (folder != null)
                {
                    if (string.IsNullOrEmpty(folder.FolderPath))
                    {
                        throw new ConfigValidationException("Email Folder FolderPath is required");
                    }
                }
                if (smtp != null) { }
            }
        }

        private void ValidateSms(AppSettings settings)
        {
            var sms = settings.Sms;
            var template = settings.Template;
            var comboTemplate = settings.ComboTemplate;

            if (sms != null)
            {
                if (template == null && comboTemplate == null)
                {
                    logger.LogInformation("Sms configured without templates, only plain sms will be possible");
                }

                var twilio = sms.Twilio;
                var folder = sms.Folder;
                var smses = new object[] { twilio, folder }.Where(x => x != null);
                if (!smses.Any())
                {
                    throw new ConfigValidationException("Sms section included with no sender configured");
                }
                if(smses.Count() > 1)
                {
                    throw new ConfigValidationException("Sms section included with multiple sender configured");
                }

                if (!string.IsNullOrEmpty(sms.OverrideToNumber))
                {
                    logger.LogInformation("Override to number configured. All sms will be sent to " + sms.OverrideToNumber);
                }
                if (!string.IsNullOrEmpty(sms.DefaultFromNumber))
                {
                    logger.LogInformation("Default from number configured. Any sms without an explicit from number will be from " + sms.DefaultFromNumber);
                }

                if (twilio != null)
                {
                    if (string.IsNullOrEmpty(twilio.AuthToken))
                    {
                        throw new ConfigValidationException("Sms Twilio AuthToken is required");
                    }
                    if (string.IsNullOrEmpty(twilio.AccountSid))
                    {
                        throw new ConfigValidationException("Sms Twilio AccountSid is required");
                    }
                }
                if(folder != null)
                {
                    if (string.IsNullOrEmpty(folder.FolderPath))
                    {
                        throw new ConfigValidationException("Sms Folder FolderPath is required");
                    }
                }
            }
        }

        private void ValidateTemplate(TemplateSettings? template, string? subName = null)
        {
            if (template != null)
            {
                var subString = (subName != null ? " for " + subName : "");
                var location = template.Location;
                var formatting = template.Formatting;
                if(location == null)
                {
                    throw new ConfigValidationException("Template Location is required" + subString);
                }

                var folder = location.Folder;
                var remote = location.Remote;
                var database = location.Database;
                var locations = new object?[] { folder, remote, database }.Where(x => x != null);

                if (!locations.Any())
                {
                    throw new ConfigValidationException("No Template locations configured" + subString);
                }
                if(locations.Count() > 1 && !string.IsNullOrEmpty(location.Strategy)) 
                {
                    throw new ConfigValidationException("Template strategy is required for multiple locations" + subString);
                }

                if(folder != null)
                {
                    if (string.IsNullOrEmpty(folder.BaseFolder))
                    {
                        throw new ConfigValidationException("Template Folder BaseFolder is required" + subString);
                    }
                    var fixedValid = folder.Fixed != null && folder.Fixed.Any();
                    var regexValid = folder.Regex != null && folder.Regex.Any();
                    if(!fixedValid && !regexValid)
                    {
                        throw new ConfigValidationException("Template Folder requires either Fixed or Regex paths" + subString);
                    }
                }
                if(database != null)
                {
                    if (string.IsNullOrEmpty(database.ConnectionString))
                    {
                        throw new ConfigValidationException("Template Database ConnectionString is required" + subString);
                    }
                    if (string.IsNullOrEmpty(database.Table))
                    {
                        throw new ConfigValidationException("Template Database Table is required" + subString);
                    }
                    if (string.IsNullOrEmpty(database.LookupColumn))
                    {
                        throw new ConfigValidationException("Template Database LookupColumn is required" + subString);
                    }
                    if (string.IsNullOrEmpty(database.TemplateColumn))
                    {
                        throw new ConfigValidationException("Template Databsae TemplateColumn is required" + subString);
                    }
                }

                if(formatting == null)
                {
                    throw new ConfigValidationException("Template Formatting is required" + subString);
                }

                var razor = formatting.Razor;
                var handlebars = formatting.Handlebars;
                var basic = formatting.Basic;

                var formatters = new object?[] { razor, handlebars, basic }.Where(x => x != null);
                if (!formatters.Any())
                {
                    throw new ConfigValidationException("No Template formating configured" + subString);
                }
                if(formatters.Count() > 1)
                {
                    throw new ConfigValidationException("Multiple Template formating configured" + subString);
                }
            }
        }

        private void ValidateComboTemplate(ComboTemplateSettings? comboTemplate)
        {
            if(comboTemplate != null)
            {
                var combo = comboTemplate.Combo;

                if(combo.Count == 0)
                {
                    throw new ConfigValidationException("ComboTemplate has no configured templates");
                }
                if (combo.Count == 1)
                {
                    throw new ConfigValidationException("ComboTemplate has a single configured template, please use Template if this is intended");
                }

                if (!string.IsNullOrEmpty(comboTemplate.Strategy))
                {
                    throw new ConfigValidationException("ComboTemplate strategy is required");
                }
                var strategyParts = comboTemplate.Strategy.Split("|");
                var comboKeys = combo.Keys;
                if (!strategyParts.SequenceEqual(comboKeys))
                {
                    throw new ConfigValidationException("ComboTemplate strategy doesn't match keys in combo");
                }
                foreach(var (key, template) in combo)
                {
                    ValidateTemplate(template, key);
                }
            }
        }
    }

    public class ConfigValidationException : Exception
    {
        public ConfigValidationException() : base() { }
        public ConfigValidationException(string message) : base(message) { }
    }
}
