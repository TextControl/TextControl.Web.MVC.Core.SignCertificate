using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using tx_sign_cert.Models;
using TXTextControl;

namespace tx_sign_cert.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger) {
			_logger = logger;
		}

		public IActionResult Index() {
			return View();
		}

		[HttpPost]
		public string SignDocument([FromBody] TXTextControl.Web.MVC.DocumentViewer.Models.SignatureData data) {
	
			byte[] bPDF;

			// create temporary ServerTextControl
			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
				tx.Create();

				// load the document
				tx.Load(Convert.FromBase64String(data.SignedDocument.Document),
				  TXTextControl.BinaryStreamType.InternalUnicodeFormat);

				// create a certificate
				X509Certificate2 cert = new X509Certificate2("App_Data/textcontrolself.pfx", "123");

				// assign the certificate to the signature fields
				TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings() {
					CreatorApplication = "TX Text Control Sample Application",
					SignatureFields = new DigitalSignature[] {
					  new TXTextControl.DigitalSignature(cert, null, "txsign")
					}
				};

				// save the document as PDF
				tx.Save(out bPDF, TXTextControl.BinaryStreamType.AdobePDFA, saveSettings);
			}

			// return as Base64 encoded string
			return Convert.ToBase64String(bPDF);
		}

		public IActionResult Privacy() {
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}