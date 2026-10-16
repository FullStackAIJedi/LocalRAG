namespace RagApp.Api.Controllers
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using MediatR;

    using RagApp.Application.Features.Documents;

    [ApiController]
    [Route("api/documents")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;
         
        private const long MaxFileSize = 50 * 1024 * 1024;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(claim ?? throw new UnauthorizedAccessException());
        }

        // GET /api/documents 
        [HttpGet]
        public async Task<IActionResult> GetMyDocuments()
        {
            var docs = await _mediator.Send(new GetUserDocumentsQuery(GetUserId()));
            return Ok(docs);
        }

        // POST /api/documents/upload 
        [HttpPost("upload")]
        [RequestSizeLimit(MaxFileSize)]
        public async Task<IActionResult> Upload(IFormFile file)
        { 
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file selected." });

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only PDF files are allowed." });

            if (file.Length > MaxFileSize)
                return BadRequest(new { error = "The file is too large (maximum 50 MB)." });

            try
            { 
                await using var stream = file.OpenReadStream();

                var docId = await _mediator.Send(new UploadDocumentCommand(
                    GetUserId(),
                    file.FileName,
                    stream,
                    file.Length));
                 
                return Ok(new { documentId = docId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
