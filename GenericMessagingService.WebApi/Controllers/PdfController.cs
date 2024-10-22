using GenericMessagingService.Services.Pdf;
using GenericMessagingService.Types.Pdf;
using GenericMessagingService.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace GenericMessagingService.WebApi.Controllers
{
    [ApiKeyFilter]
    [ActiveFeatureFilter("pdf")]
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IPdfGenerationService pdfGenerationService;

        public PdfController(IPdfGenerationService pdfGenerationService)
        {
            this.pdfGenerationService = pdfGenerationService;
        }

        [HttpPost]
        public async Task<FileResult> GetPdf(PdfRequest pdfRequest)
        {
            var pdfBytes = await pdfGenerationService.GetPdf(pdfRequest);
            var filename = string.IsNullOrEmpty(pdfRequest.Filename)
                ? pdfRequest.TemplateName.Replace("\\", "_").Replace("/", "_") + ".pdf"
                : pdfRequest.Filename;
            return File(pdfBytes, "application/pdf", filename);
        }
    }
}
