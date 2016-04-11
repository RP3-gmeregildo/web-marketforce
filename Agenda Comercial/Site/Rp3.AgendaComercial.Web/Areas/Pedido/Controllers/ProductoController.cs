using DevExpress.Data;
using DevExpress.Web.Mvc;
using DevExpress.Data.Linq.Helpers;
using Rp3.AgendaComercial.Models;
using Rp3.AgendaComercial.Models.Pedido;
using Rp3.AgendaComercial.Web.Areas.Pedido.Models;
using Rp3.Web.Mvc;
using Rp3.Web.Mvc.Html;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThoughtWorks.QRCode.Codec;
using ZXing;
using ZXing.Common;
using Rp3.Web.Mvc.Utility;
using Rp3.Data.Common;

namespace Rp3.AgendaComercial.Web.Pedido.Controllers
{
    public static class ProductoBindingHandlers
    {
        static IQueryable Model { get { return ProductoController.GetListIndex(); } }

        public static void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {
            e.DataRowCount = Model.ApplyFilter(e.State.FilterExpression).Count();
        }
        public static void GetData(GridViewCustomBindingGetDataArgs e)
        {
            e.Data = Model.ApplyFilter(e.State.FilterExpression).ApplySorting(e.State.SortedColumns).Skip(e.StartDataRowIndex).Take(e.DataRowCount);
        }
    }
    public class ProductoController : AgendaComercial.Web.Controllers.AgendaComercialController
    {
        // GET: Pedido/Producto
        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "QUERY", "AGENDACOMERCIAL")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridViewIndex()
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            if (viewModel == null)
                viewModel = CreateGridViewModel();
            return GridCustomActionCore(viewModel);
            //return PartialView("_GridViewIndex", GetListIndex().ToList());
        }

        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "EDIT", "AGENDACOMERCIAL")]
        public ActionResult Edit(int id)
        {
            InicializarTab();
            ViewBag.ReadOnly = false;
            Producto model = DataBase.Productos.GetSingleOrDefault(p => p.IdProducto == id);
            model.QRCode = ToQRCode(model);
            model.URLFoto = Url.Content(AgendaComercial.Models.Constantes.ProductoMedia.FilePath + @"/" + model.URLFoto);
            ViewBag.SubCategorias = DataBase.SubCategorias.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "DETAIL", "AGENDACOMERCIAL")]
        public ActionResult Detail(int id)
        {
            InicializarTab();
            ViewBag.ReadOnly = true;
            Producto model = DataBase.Productos.GetSingleOrDefault(p => p.IdProducto == id);
            model.QRCode = ToQRCode(model);
            model.URLFoto = Url.Content(AgendaComercial.Models.Constantes.ProductoMedia.FilePath + @"/" + model.URLFoto);
            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "NEW", "AGENDACOMERCIAL")]
        public ActionResult Create()
        {
            InicializarTab();
            ViewBag.ReadOnly = false;
            Producto producto = new Producto();
            producto.Descuentos = new List<Descuento>();
            producto.TipoDescuento = Models.Constantes.TipoDescuento.Valor;
            producto.AsignarId();
            ViewBag.SubCategorias = DataBase.SubCategorias.Get(p => p.Estado == Models.Constantes.Estado.Activo).ToSelectList(includeNullItem: true);
            return View(producto);
        }

        public static IQueryable<Producto> GetListIndex()
        {
            ContextService db = new ContextService();
            return db.Productos.GetQueryable(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado, includeProperties: "EstadoGeneralValue");
        }

        #region Pagination & Sorting

        public ActionResult GridViewFilteringAction(GridViewColumnState column)
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.ApplyFilteringState(column);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridViewSortingAction(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.SortBy(column, reset);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridViewPagingAction(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("gridViewIndex");
            viewModel.Pager.Assign(pager);
            return GridCustomActionCore(viewModel);
        }

        public ActionResult GridCustomActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                ProductoBindingHandlers.GetDataRowCount,
                ProductoBindingHandlers.GetData
            );

            return PartialView("_GridViewIndex", gridViewModel);
        }

        static GridViewModel CreateGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "IdProducto";

            var col = viewModel.Columns.Add("Descripcion");
            col.SortOrder = ColumnSortOrder.Ascending;
            col.SortIndex = 0;

            viewModel.Pager.PageSize = 15;
            return viewModel;
        }

        #endregion Pagination & Sorting

        private void InicializarTab()
        {
            TabCollection tabCollection = new TabCollection();

            tabCollection.Add("tabproducto", TabTarget.HtmlElement, "#tabproducto", Rp3.AgendaComercial.Resources.TitleFor.Productos, true);
            //tabCollection.Add("tabdescuentos", TabTarget.HtmlElement, "#tabdescuentos", Rp3.AgendaComercial.Resources.TitleFor.Descuento, false);

            ViewBag.TabCollection = tabCollection;
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "NEW", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Create(Producto model)
        {
            try
            {
                model.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                model.FecIng = GetCurrentDateTime();
                model.UsrIng = this.UserLogonName;
                model.FecMod = GetCurrentDateTime();
                model.UsrMod = this.UserLogonName;
                DataBase.Productos.Insert(model);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", model);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return View(model);
        }

        public ActionResult Preview()
        {
            var data = DataBase.Productos.Get(p => p.Estado != Rp3.AgendaComercial.Models.Constantes.Estado.Eliminado).ToList();
            ViewBag.User = this.UserLogonName;
            ViewBag.Fecha = this.GetCurrentDateTime();

            foreach(Producto prod in data)
            {
                prod.QRCode = ToQRCode(prod);
                if(prod.URLFoto != null)
                    prod.URLFoto = Path.Combine(HttpRuntime.AppDomainAppPath, AgendaComercial.Models.Constantes.ProductoMedia.NewFilePath, prod.URLFoto);
            }

            Rp3.AgendaComercial.Web.Areas.Pedido.Reports.Productos report = new Areas.Pedido.Reports.Productos(ViewBag, data);
            System.IO.MemoryStream reportStream = new System.IO.MemoryStream();
            report.ExportToPdf(reportStream);
            reportStream.Position = 0;
            //return Json(new { buffer = Convert.ToBase64String(reportStream.ToArray()) } , JsonRequestBehavior.AllowGet);
            return new Rp3.AgendaComercial.Web.BinaryResult { Data = reportStream.ToArray(), ContentType = "application/pdf", FileName = string.Format("Producto.{0:yyyy.MM.dd}.{1:HH.mm.ss}.pdf", DateTime.Now, DateTime.Now) };

        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Producto model)
        {
            try
            {
                Producto modelToUpdate = DataBase.Productos.GetSingleOrDefault(p => p.IdProducto == model.IdProducto);
                modelToUpdate.Descripcion = model.Descripcion;
                modelToUpdate.Precio = model.Precio;
                modelToUpdate.Estado = model.Estado;
                modelToUpdate.IdExterno = model.IdExterno;
                modelToUpdate.EstadoTabla = Rp3.AgendaComercial.Models.Constantes.Estado.Tabla;
                modelToUpdate.FecMod = GetCurrentDateTime();
                modelToUpdate.UsrMod = this.UserLogonName;
                modelToUpdate.IdSubCategoria = model.IdSubCategoria;
                DataBase.Productos.Update(modelToUpdate);
                DataBase.Save();

                this.AddDefaultSuccessMessage();
                return RedirectToAction("Index", modelToUpdate);
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }
            return View(model);
        }

        public Producto GetModel(int id)
        {
            return DataBase.Productos.GetSingleOrDefault(p => p.IdProducto == id);
        }

        #region Foto
        [HttpPost]
        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "EDIT", "AGENDACOMERCIAL")]
        public JsonResult SaveFoto(int id, string fileName, double x, double y, double width, double height)
        {
            try
            {
                fileName = Path.Combine(Server.MapPath(Url.Content(Constantes.ProductoMedia.FilePath)), Path.GetFileName(fileName));
                Bitmap src = System.Drawing.Image.FromFile(fileName) as Bitmap;

                if (width > src.Width)
                    width = src.Width;

                if (height > src.Height)
                    height = src.Height;

                Rectangle cropRect = new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(width), Convert.ToInt32(height));
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                     cropRect,
                                     GraphicsUnit.Pixel);
                }

                FileInfo fileDelete = new FileInfo(fileName);

                var newfileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(fileDelete.Name));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProductoMedia.FilePath)), newfileName);

                try
                {
                    fileDelete.Delete();
                }
                catch { }

                target.Save(imagePath);
                string minFileName = Thumbnail.SaveThumbnail(imagePath, Constantes.ProfileFotosSize.ProfilePictuteMinWidth, Constantes.ProfileFotosSize.ProfilePictuteMinHeight, "min");
                Thumbnail.SaveThumbnail(imagePath, Constantes.ProfileFotosSize.ProfilePictuteMedWidth, Constantes.ProfileFotosSize.ProfilePictuteMedHeight, "med");
                Thumbnail.SaveThumbnail(imagePath, Constantes.ProfileFotosSize.ProfilePictuteSmaWidth, Constantes.ProfileFotosSize.ProfilePictuteSmaHeight, "sma");

                var modelUpdate = GetModel(id);

                if (modelUpdate != null)
                {
                    if (!string.IsNullOrEmpty(modelUpdate.URLFoto))
                    {

                        string deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProductoMedia.FilePath)), Path.GetFileName(modelUpdate.URLFoto));
                        FileInfo FileDetele = new FileInfo(deleteImagePath);
                        if (FileDetele.Exists)
                            FileDetele.Delete();
                    }

                    //modelUpdate.URLFoto = Path.Combine(Constantes.ProfileFotosSize.ClienteImagePath, newfileName);
                    modelUpdate.URLFoto = newfileName;
                    DataBase.Productos.Update(modelUpdate);
                    DataBase.Save();
                }


                string fotoFileName = Path.Combine(Constantes.ProductoMedia.FileSavePath + newfileName);
                string fotoMin = Thumbnail.GetPictureMinFromOriginal(fotoFileName);

                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.ArchivoSubidoCorrectamente,
                        MessageType = MessageType.Success,
                        ImageNamePath = "~" + Url.Content(fotoFileName),
                        ImagePath = Url.Content(fotoFileName),
                        ThumbnailImagePath = Url.Content(fotoMin)
                    }
                };
            }
            catch (Exception)
            {
                return new JsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.NoPudoCargarArchivo,
                        MessageType = MessageType.Error
                    }
                };
            }
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "EDIT", "AGENDACOMERCIAL")]
        public WrappedJsonResult DeleteFoto(int id)
        {
            string message = Rp3.AgendaComercial.Resources.MessageFor.EliminarImageOk;
            bool valid = false;


            try
            {
                var modelUpdate = GetModel(id);

                if (modelUpdate != null)
                {
                    string deleteImagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProfileFotosSize.ClienteImagePath)), Path.GetFileName(modelUpdate.URLFoto));
                    FileInfo FileDetele = new FileInfo(deleteImagePath);
                    if (FileDetele.Exists)
                        FileDetele.Delete();

                    modelUpdate.URLFoto = null;

                    DataBase.Productos.Update(modelUpdate);
                    DataBase.Save();

                    valid = true;
                }
                else
                    message = Rp3.AgendaComercial.Resources.MessageFor.NoExisteFotoEliminar;

            }
            catch (Exception)
            {
                message = Rp3.AgendaComercial.Resources.MessageFor.ErrorEliminarFoto;
            }

            return new WrappedJsonResult() { Data = new { Message = message, IsValid = valid } };
        }

        [HttpPost]
        [Rp3.Web.Mvc.Authorize("PRODUCTOS", "EDIT", "AGENDACOMERCIAL")]
        public WrappedJsonResult UploadFoto(HttpPostedFileWrapper file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = Rp3.AgendaComercial.Resources.MessageFor.NoPudoCargarFoto,
                            ImagePath = string.Empty
                        }
                    };
                }

                if (file.ContentLength > Constantes.ProfileFotosSize.MaxSizeUploadFile)
                {
                    return new WrappedJsonResult
                    {
                        Data = new
                        {
                            IsValid = false,
                            Message = String.Format(Rp3.AgendaComercial.Resources.MessageFor.TamanoFotoExcedido, (Constantes.ProfileFotosSize.MaxSizeUploadFile / 1048576)),
                            ImagePath = string.Empty
                        }
                    };
                }

                var fileName = String.Format("{0}{1}", Guid.NewGuid(), System.IO.Path.GetExtension(file.FileName));
                var imagePath = Path.Combine(Server.MapPath(Url.Content(Constantes.ProductoMedia.FilePath)), fileName);

                file.SaveAs(imagePath);
                //SaveThumbnail(imagePath);              

                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = true,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.ArchivoSubidoCorrectamente,
                        ImagePath = Url.Content(Constantes.ProductoMedia.FileSavePath + fileName),
                        Width = Constantes.ProfileFotosSize.ProfilePictureWidth,
                        Height = Constantes.ProfileFotosSize.ProfilePictureHeight
                    }
                };
            }
            catch
            {
                return new WrappedJsonResult
                {
                    Data = new
                    {
                        IsValid = false,
                        Message = Rp3.AgendaComercial.Resources.MessageFor.NoPudoCargarArchivo
                    }
                };
            }
        }
        #endregion

        #region QRCode
        private byte[] ToQRCode(Producto prod)
        {
            ProductoQR qr = new ProductoQR();
            qr.id = prod.IdProducto;
            qr.p = prod.Precio;
            qr.d = prod.Descripcion;
            qr.f = prod.URLFoto;
            string jCode = new JavaScriptSerializer().Serialize(qr); ;

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 115,
                    Width = 115,
                    Margin = 0
                }
            };
            return imageToByteArray(barcodeWriter.Write(jCode));
            
        }

        public byte[] imageToByteArray(System.Drawing.Bitmap imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        #endregion
    }
}