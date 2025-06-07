

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{

    [AllowAnonymous]
    [ApiController]
    public class HomeController : ControllerBase
    {

        public HomeController(IWebHostEnvironment webHostEnvironment, ReportingContext reportContext)
            : base()
        {
            this._webHostEnvironment = webHostEnvironment;
            this._dbContext = reportContext;
        }


        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ReportingContext _dbContext;

        [HttpGet("test")]
        public IActionResult Index()
        {
            return this.Ok();
        }


        [HttpGet("home")]
        public IActionResult Home()
        {
            return new PhysicalFileResult(
                Path.Combine(this._webHostEnvironment.ContentRootPath, "Views/Home.html"),
                new MediaTypeHeaderValue("text/html").ToString());
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            return new PhysicalFileResult(
                Path.Combine(this._webHostEnvironment.ContentRootPath, "Views/Error.html"),
                new MediaTypeHeaderValue("text/html").ToString());
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            // This endpoint can be used for health checks
            return this.Ok(new { status = "Healthy" });
        }

        [HttpGet("getExcelData")]
        public async Task<IActionResult> GetExcelData()
        {
            
            var filePath = Path.Combine(this._webHostEnvironment.ContentRootPath, "Files", "Countries_Leasing_Stats.xlsx");
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    FormattableString sql = $"SELECT * FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 'Excel 12.0;Database={filePath};HDR=YES;' ,Sheet1$);";
                    var result = await _dbContext.CountriesTurnoverReport.FromSql(sql).AsNoTracking().ToListAsync();
                    return this.Ok(new RequestResult<CountriesTurnoverReport> { Items = result, Successful = true, Error = null, Item = null });
                }
                catch (Exception ex)
                {
                    return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = ex.Message, Items = null });
                }

            }
            return this.BadRequest(new RequestResult<UserResponse> { Item = null, Successful = false, Error = "Data Not Found...", Items = null });
        }



    }
}
