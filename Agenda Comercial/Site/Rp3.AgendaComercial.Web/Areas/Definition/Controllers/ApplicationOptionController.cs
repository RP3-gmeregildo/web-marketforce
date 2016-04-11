using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rp3.Data.Models.Security;
using Rp3.Web.Mvc.Application.Security.Models;

namespace Rp3.Web.Mvc.Application.Definition.Controllers
{
    public class ApplicationOptionController : Rp3.Web.Mvc.Controllers.BaseController
    {
        [Authorize("APPOPTION", "QUERY", "SECURITY")]
        public ActionResult Index()
        {
            List<Data.Models.Definition.ApplicationOption> options = DataBase.ApplicationOptions.Get().Where(p=>p.ApplicationOptionColumns.Count > 0).ToList();

            return View(options);
        }

        private Data.Models.Definition.ApplicationOption GetModel(string applicationId, string optionId)
        {
            Data.Models.Definition.ApplicationOption result = DataBase.ApplicationOptions.Get(p => p.ApplicationId == applicationId && p.OptionId == optionId, includeProperties: "ApplicationOptionColumns").SingleOrDefault();

            return result;
        }

        [Rp3.Web.Mvc.Authorize("APPOPTION", "EDIT", "SECURITY")]
        public ActionResult Edit(string applicationId, string optionId)
        {
            var model = GetModel(applicationId, optionId);

            return View(model);
        }

        [Rp3.Web.Mvc.Authorize("APPOPTION", "DETAIL", "SECURITY")]
        public ActionResult Detail(string applicationId, string optionId)
        {
            var model = GetModel(applicationId, optionId);
            model.ReadOnly = true;
            return View("Edit", model);
        }

        [HttpPost]
        [PreventSpam(Order = 0)]
        [Rp3.Web.Mvc.Authorize("PROGRAMATAREA", "EDIT", "AGENDACOMERCIAL", Order = 1)]
        public ActionResult Edit(Data.Models.Definition.ApplicationOption model, string[] columns)
        {
            Data.Models.Definition.ApplicationOption modelUpdate = GetModel(model.ApplicationId, model.OptionId);

            try
            {
                if (ModelState.IsValid)
                {
                    model.ApplicationOptionColumns = modelUpdate.ApplicationOptionColumns;
                    SetColumns(model, columns);
                    DataBase.ApplicationOptionColumns.Update(model.ApplicationOptionColumns, modelUpdate.ApplicationOptionColumns);

                    DataBase.ApplicationOptions.Update(modelUpdate);
                    DataBase.Save();

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                this.AddDefaultErrorMessage();
            }

            model.ApplicationOptionColumns = modelUpdate.ApplicationOptionColumns;
            SetColumns(model, columns);

            return View(model);
        }

        public void SetColumns(Data.Models.Definition.ApplicationOption model, string[] columns)
        {
            foreach (var col in model.ApplicationOptionColumns)
                col.Visible = false;

            if (columns != null)
            {
                foreach (var insert in columns.Where(p => p != "false"))
                {
                    string[] keyParts = insert.Split('-');

                    string columnName = Convert.ToString(keyParts[2]);

                    var det = model.ApplicationOptionColumns.Where(p => p.ColumnName == columnName).FirstOrDefault();

                    if (det != null)
                        det.Visible = true;
                }
            }
        }
    }
}
