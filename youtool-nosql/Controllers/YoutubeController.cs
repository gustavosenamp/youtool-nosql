using Microsoft.AspNetCore.Mvc;
using youtool_nosql.Services;
using youtool_nosql.Models;
using Google.Apis.YouTube.v3;

namespace youtool_nosql.Controllers
{
    public class YoutubeController : Controller
    {
        private readonly ServiceYoutube _serviceYoutube;

        public YoutubeController(ServiceYoutube youtubeService)
        {
            _serviceYoutube = youtubeService;
        }

        public async Task<IActionResult> Index()
        {
            var comentarios = await _serviceYoutube.ListarComentarios();
            return View(comentarios);
        }

        [HttpPost]
        public async Task<IActionResult> Coletar(string videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId))
            {
                TempData["Erro"] = "Informe o ID do vídeo.";
                return RedirectToAction("Index");
            }

            await _serviceYoutube.ObterComentarios(videoId);
            TempData["Mensagem"] = "Comentários coletados com sucesso!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Canal(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
            {
                TempData["Erro"] = "Informe o nome ou ID do canal.";
                return RedirectToAction("Index");
            }

            var canal = await _serviceYoutube.BuscarCanal(termo);
            if (canal == null)
            {
                TempData["Erro"] = "Canal não encontrado.";
                return RedirectToAction("Index");
            }

            return View("Canal", canal);
        }

    }
}
