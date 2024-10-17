class ApiClient {
    apiKey;
    constructor(apiKey = undefined) {
        this.apiKey = apiKey;
    }
    setApiKey(apiKey) {
        this.apiKey = apiKey;
    }
    async getTemplate(templateName, data) {
        var request = {
            templateName,
            data
        };
        var res = await fetch('/template', {
            method: "POST",
            body: JSON.stringify(data),
            headers: {
                "Content-Type": 'application/json',
                "X-API-KEY": this.apiKey
            }
        }).then(x => x.json());
        if (res.success) {
            return res.data;
        }
    }
    async sendEmail(request) {
        return await fetch('/email', {
            method: "POST",
            body: JSON.stringify(request),
            headers: {
                "Content-Type": 'application/json',
                "X-API-KEY": this.apiKey
            }
        });
    }
    async sendSms(request) {
        return await fetch('/sms', {
            method: "POST",
            body: JSON.stringify(request),
            headers: {
                "Content-Type": 'application/json',
                "X-API-KEY": this.apiKey
            }
        });
    }
}

(async function () {
    var apiClientInput = document.getElementById("api-key");
    var apiClient = new ApiClient(apiClientInput.value);
    apiClientInput.onchange = function () {
        apiClient.setApiKey(apiClientInput.value);
    }

    var tabGroup = document.getElementById("tab-group");
    var tabHeader = document.getElementById("tab-header");
    var tabHeaderTemplate = document.getElementById("tab-header-template");
    var tabHeaderEmail = document.getElementById("tab-header-email");
    var tabHeaderSms = document.getElementById("tab-header-sms");
    var tabContent = document.getElementById("tab-content");
    var tabContentTemplate = document.getElementById("tab-content-template");
    var tabContentTemplateRequest = document.getElementById("template-request");
    var tabContentTemplateName = document.getElementById("t-template-name");
    var tabContentTemplateData = document.getElementById("t-template-data");
    var tabContentTemplateSubmit = document.getElementById("template-submit");
    var tabContentTemplateResponse = document.getElementById("template-response");
    var tabContentTemplateResultBody = document.getElementById("template-result-body");
    var tabContentTemplateResultSubject = document.getElementById("template-result-subject");
    var tabContentEmail = document.getElementById("tab-content-email");
    var tabContentSms = document.getElementById("tab-content-sms");

    tabContentTemplateSubmit.onclick = function () {
        var templateName = tabContentTemplateName.value;
        var templateDataString = tabContentTemplateData.value;
        var templateData = {};
        if (!templateName || !templateName.length) {
            return;
        }
        if (templateDataString && templateDataString.length) {
            templateData = JSON.parse(templateDataString);
        }
        apiClient.getTemplate(templateName, templateData).then(result => {
            var body = result.body;
            var subject = result.subject;
            tabContentTemplateResultBody.innerHTML = body;
            tabContentTemplateResultSubject.innerHTML = subject;
        });
    }

    function toggle(element, show) {
        if (show) {
            element.classlist = "";
        } else {
            element.classlist = "hidden";
        }
    }

    tabHeaderTemplate.onclick = function () {
        toggle(tabContentTemplate, true);
        toggle(tabContentEmail);
        toggle(tabContentSms);
    }
    tabHeaderEmail.onclick = function () {
        toggle(tabContentTemplate);
        toggle(tabContentEmail, true);
        toggle(tabContentSms);
    }
    tabHeaderSms.onclick = function () {
        toggle(tabContentTemplate);
        toggle(tabContentEmail);
        toggle(tabContentSms, true);
    }
}())