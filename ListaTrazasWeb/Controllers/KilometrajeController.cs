using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ListaTrazasWeb.Models;
using Microsoft.SharePoint.Client;

namespace ListaTrazasWeb.Controllers
{
    public class KilometrajeController : Controller
    {
        // GET: Kilometraje
        public PartialViewResult Index()
        {
            List<Kilometraje> claimsToDisplay =new List<Kilometraje>();

            var spContext = Session["sp"] as SharePointContext;

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())

            {
                Web web = clientContext.Web;
                clientContext.Load(web);
                clientContext.ExecuteQuery();
                ListCollection lists = web.Lists;
                clientContext.Load<ListCollection>(lists);
                clientContext.ExecuteQuery();
                var kilometros = lists.GetByTitle("Kilometros");
                clientContext.Load(kilometros);
                clientContext.ExecuteQuery();
                CamlQuery query = new CamlQuery();
                ListItemCollection kilometrosItems = kilometros.GetItems(query);
                clientContext.Load(kilometrosItems);
                clientContext.ExecuteQuery();

                foreach (var ki in kilometrosItems)
                {
                    Kilometraje currentClaim = new Kilometraje();
                    currentClaim.Destino = ki["Destino"].ToString();
                    currentClaim.Kilometros = ki["Distancia"] != null ? Convert.ToInt32(ki["Distancia"]) : 0;
                    claimsToDisplay.Add(currentClaim);
                }
            }

            return PartialView("Index", claimsToDisplay);

        }

        public ActionResult Create()
        {
            Kilometraje newClaim = new Kilometraje();
            return View("Create", newClaim);
        }

        [HttpPost]
        public ActionResult Create(Kilometraje claim)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", claim);
            }

            else
            {
                var spContext = Session["sp"] as SharePointContext;
                using (var context = spContext.CreateUserClientContextForSPAppWeb())
                {
                    List claimsList = context.Web.Lists.GetByTitle("Kilometros");
                    context.Load(claimsList);
                    ListItemCreationInformation creationInfo = new ListItemCreationInformation();
                    ListItem newClaim = claimsList.AddItem(creationInfo);
                    newClaim["Destino"] = claim.Destino;
                    var d = Convert.ToInt32(claim.Kilometros);
                    newClaim["Distancia"] = d;
                    newClaim.Update();
                    context.ExecuteQuery();
                }
                return RedirectToAction("Index", "Home");
            }
        }


    }
}