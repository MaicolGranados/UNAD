using FUNSAR.AccesoDatos.Data.Repository;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.Data;
using FUNSAR.Models;
using FUNSAR.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FUNSAR.Areas.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("Admin")]
    public class ArticulosController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                ArticuloVM artivm = new ArticuloVM()
                {
                    Articulo = new FUNSAR.Models.Articulo(),
                    ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias(),
                    ListaServicio = _contenedorTrabajo.Servicio.GetListaServicio(),
                    ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(new List<int> { 1, 26, 27, 28, 29, 30, 31 })
                };

                return View(artivm);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ArticulosController.Create()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticuloVM artivm)
        {
            try
            {
                string rutaPrincipal = _hostingEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;
                string nombreArchivo = Guid.NewGuid().ToString();

                if (artivm.Articulo.Id == 0)
                {
                    if (archivos[0] != null)
                    {
                        var imagenes = Path.Combine(rutaPrincipal, @"img\articulos");
                        var extensionImagen = Path.GetExtension(archivos[0].FileName);

                        using (var fileStreams = new FileStream(Path.Combine(imagenes, nombreArchivo + extensionImagen), FileMode.Create))
                        {
                            archivos[0].CopyTo(fileStreams);
                        }

                        artivm.Articulo.UrlImagen = @"\img\articulos\" + nombreArchivo + extensionImagen;
                    }
                    if (archivos[1] != null)
                    {
                        var documentos = Path.Combine(rutaPrincipal, @"Files\autorizaciones");
                        var extensiondoc = Path.GetExtension(archivos[1].FileName);

                        using (var fileStreams = new FileStream(Path.Combine(documentos, nombreArchivo + extensiondoc), FileMode.Create))
                        {
                            archivos[1].CopyTo(fileStreams);
                        }

                        artivm.Articulo.UrlDocumento = @"\Files\autorizaciones\" + nombreArchivo + extensiondoc;
                    }

                    artivm.Articulo.FechaCreacion = DateTime.Now.ToString();
                    artivm.Articulo.Activo = true;
                    _contenedorTrabajo.Articulo.Add(artivm.Articulo);
                    _contenedorTrabajo.save();
                    return RedirectToAction(nameof(Index));
                }
                artivm.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
                artivm.ListaServicio= _contenedorTrabajo.Servicio.GetListaServicio();
                return View(artivm);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ArticulosController.Create(Model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Edit (int? id)
        {
            try
            {
                ArticuloVM artivm = new ArticuloVM()
                {
                    Articulo = new FUNSAR.Models.Articulo(),
                    ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias(),
                    ListaServicio = _contenedorTrabajo.Servicio.GetListaServicio(),
                    ListaBrigadas = _contenedorTrabajo.Brigada.GetListaBrigadas(new List<int> {1,26,27,28,29,30,31})
                };

                if (id != null)
                {
                    artivm.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());
                }

                return View(artivm);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ArticulosController.Edit()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticuloVM artivm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string rutaPrincipal = _hostingEnviroment.WebRootPath;
                    var archivos = HttpContext.Request.Form.Files;
                    var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(artivm.Articulo.Id);

                    if (archivos.Count() > 0)
                    {
                        if (archivos[0] != null)
                        {
                            //Nueva Imagen para el articulo
                            string nombreArchivo = Guid.NewGuid().ToString();
                            var subidas = Path.Combine(rutaPrincipal, @"img\articulos");
                            var extension = Path.GetExtension(archivos[0].FileName);
                            var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                            var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));
                            if (System.IO.File.Exists(rutaImagen))
                            {
                                System.IO.File.Delete(rutaImagen);
                            }

                            //Nuevamente subimos el archivo

                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivos[0].CopyTo(fileStreams);
                            }

                            artivm.Articulo.UrlImagen = @"\img\articulos\" + nombreArchivo + extension;
                            artivm.Articulo.FechaCreacion = DateTime.Now.ToString();
                            _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                            _contenedorTrabajo.save();
                        }
                        if (archivos[1] != null)
                        {
                            //Nueva Imagen para el articulo
                            string nombreArchivo = Guid.NewGuid().ToString();
                            var subidas = Path.Combine(rutaPrincipal, @"\Files\autorizaciones\");
                            var extension = Path.GetExtension(archivos[0].FileName);
                            var nuevaExtension = Path.GetExtension(archivos[0].FileName);

                            var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));
                            if (System.IO.File.Exists(rutaImagen))
                            {
                                System.IO.File.Delete(rutaImagen);
                            }

                            //Nuevamente subimos el archivo

                            using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                            {
                                archivos[0].CopyTo(fileStreams);
                            }

                            artivm.Articulo.UrlImagen = @"\Files\autorizaciones\" + nombreArchivo + extension;
                            artivm.Articulo.FechaCreacion = DateTime.Now.ToString();
                            _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                            _contenedorTrabajo.save();
                        }
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        artivm.Articulo.UrlImagen = articuloDesdeDb.UrlImagen;
                        artivm.Articulo.UrlDocumento = articuloDesdeDb.UrlDocumento;
                        _contenedorTrabajo.Articulo.Update(artivm.Articulo);
                        _contenedorTrabajo.save();
                        return RedirectToAction(nameof(Index));
                    }

                }
                return View(artivm);
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                log.escribirLog("ERROR", ex.ToString(), "ArticulosController.Edit(Model)");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }

        #region Llamadas a la API
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Opcion1
            try
            {
                return Json(new { data = _contenedorTrabajo.Articulo.GetALL(includeProperties: "Categoria") });

            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ArticulosController.GetAll()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
                
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var articuloDesdeDb = _contenedorTrabajo.Articulo.Get(id);
                string rutaDirectorioPrincipal = _hostingEnviroment.WebRootPath;
                var rutaImagen = Path.Combine(rutaDirectorioPrincipal, articuloDesdeDb.UrlImagen.TrimStart('\\'));

                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);
                }

                if (articuloDesdeDb == null)
                {
                    return Json(new { success = false, message = "Error Borrando articulo" });
                }

                _contenedorTrabajo.Articulo.Remove(articuloDesdeDb);
                _contenedorTrabajo.save();
                return Json(new { success = true, message = "Articulo borrado correctamente" });
            }
            catch (Exception ex)
            {
                HerramientasComunes.Log log = new HerramientasComunes.Log();
                await log.escribirLog("ERROR", ex.ToString(), "ArticulosController.Delete()");
                ErrorViewModel errorVM = new ErrorViewModel();
                return View(errorVM);
            }
            
        }
        #endregion
    }
}
