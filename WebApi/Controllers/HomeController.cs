

namespace WebApi.Controllers
{

    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        
        public HomeController(IWebHostEnvironment webHostEnvironment)
            : base()
        {
            this.WebHostEnvironment = webHostEnvironment;
        }


        private IWebHostEnvironment WebHostEnvironment { get; }


        [HttpGet("test")]
        public IActionResult Index()
        {
            return this.Ok();
        }


        [HttpGet("home")]
        public IActionResult Home()
        {
            return new PhysicalFileResult(
                Path.Combine(this.WebHostEnvironment.ContentRootPath, "Views/Home.html"),
                new MediaTypeHeaderValue("text/html").ToString());
        }

        [HttpGet("error")]
        public IActionResult Error()
        {
            return new PhysicalFileResult(
                Path.Combine(this.WebHostEnvironment.ContentRootPath, "Views/Error.html"),
                new MediaTypeHeaderValue("text/html").ToString());
        }
    }
}
