namespace FC.Api.Controllers
{
    using System;
    using System.Web.Mvc;
    using FC.Domain.Version.Business;
    using FC.Domain.Version.Entity;
    using Ninject;
    using FC.Core;

    public class VersionDAOController : Controller
    {
        Versions _version = DI.Current.Get<Versions>();
        IEssAppRepository _app = DI.Current.Get<IEssAppRepository>();

        public VersionDAOController()
        {
        }

        [HttpGet]
        public ActionResult Update()
        {
            TempData["VersionNo"] = _app.AppVersionInfo;
            return View(_version.GetLatestVersionInfo());
        }

        [HttpPost]
        public ActionResult Update(VersionEntity model)
        {
            using (var scope = TransactionScopeBuilder.CreateReadCommitted())
            {
                try
                {
                    _version.Upgrade();

                    TempData["VersionNo"] = _app.AppVersionInfo;
                    TempData["NotifyMessage"] = "Version update has completed.";
                   
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    TempData["VersionNo"] = _app.AppVersionInfo;
                    ModelState.AddModelError("", ex.Message);

                    return View(model);
                }
            }
            return RedirectToAction("Update");
        }
    }
}