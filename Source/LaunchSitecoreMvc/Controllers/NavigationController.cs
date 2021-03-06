﻿using LaunchSitecore.Configuration;
using LaunchSitecore.Configuration.SiteUI.Base;
using LaunchSitecore.Configuration.SiteUI;
using LaunchSitecore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace LaunchSitecore.Controllers
{
  public class NavigationController : LaunchSitecoreBaseController
  {
    public ActionResult Breadcrumbs()
    {
      if (Sitecore.Context.Item.ID != SiteConfiguration.GetHomeItem().ID)
      {
        List<SimpleItem> items = new List<SimpleItem>();
        Item temp = Sitecore.Context.Item;

        while (temp.ID != SiteConfiguration.GetHomeItem().ParentID)
        {
          items.Add(new SimpleItem(temp));
          temp = temp.Parent;
        }

        items.Reverse();
        return View(items);
      }
      return null;
    }

    public ActionResult FooterNavigation()
    {
      Item baseItem = SiteConfiguration.GetFooterLinksItem();
      List<SimpleNavigationItem> items = new List<SimpleNavigationItem>();
      foreach (Item footerLink in baseItem.Children)
      {
        Item i = Sitecore.Context.Database.GetItem(footerLink["Top Level Item"]);
        if (i != null && SiteConfiguration.DoesItemExistInCurrentLanguage(i)) { items.Add(new SimpleNavigationItem(i)); }
      }

      return View(items);
    }

    public ActionResult NavigationBar()
    {      
      Item presentationSettings = SiteConfiguration.GetPresentationSettingsItem();
      return (presentationSettings != null) ? View(presentationSettings) : null;      
    }

    public ActionResult SecondaryNavigation()
    {
      Item home = SiteConfiguration.GetHomeItem();
      Item dataSource = Sitecore.Context.Item;
      if (home.ID != dataSource.ID)  // if on the home node, just use it
      {
        while (dataSource.ParentID != home.ID)
          dataSource = dataSource.Parent;
      }

      MenuItem ds = new MenuItem(dataSource);
      return (dataSource != null && ds.HasChildrenToShowInSecondaryMenu) ? View(ds) : ShowListIsEmptyPageEditorAlert();
    }

    public ActionResult TertiaryNavigation()
    {
      // microsites are small so we are limiting the top area.
      if (SiteConfiguration.IsMicrosite())
      {
        return View("TertiaryNavigationMicrosite");
      }
      else
      {
        return (Sitecore.Context.IsLoggedIn && Sitecore.Context.Domain.Name.ToLower() == "extranet") ? View("TertiaryNavigationAuthenticated") : View("TertiaryNavigationAnonymous");
      }
    }
    
    public ActionResult SitesNavigation()
    {
      Item contentNode = SiteConfiguration.GetHomeItem().Parent;
      List<GenericLink> sites = new List<GenericLink>();

      foreach (Item site in contentNode.Children.ToArray().Where(item => SiteConfiguration.DoesItemExistInCurrentLanguage(item)))
      {
        if (site["Show in Sites Menu"] == "1") { sites.Add(new GenericLink(site["Site Name"], LinkManager.GetItemUrl(site), false)); }
      }

      if (SiteConfiguration.GetExternalSitesItem() != null)
      {
        foreach (Item externalsite in SiteConfiguration.GetExternalSitesItem().Children)
        {
          if (SiteConfiguration.DoesItemExistInCurrentLanguage(externalsite))
          {
            Sitecore.Data.Fields.LinkField lf = externalsite.Fields["Site Link"];
            sites.Add(new GenericLink(lf.Text, lf.Url, true));
          }
        }
      }

      // Don't show the drop down unless there are multiple sites
      return (sites.Count > 1) ? View("TertiaryNavigationPartialSites", sites as IEnumerable<GenericLink>) : null;
    }

    public ActionResult FavoritesNavigation()
    {
      List<GenericLink> favs = new List<GenericLink>();

      Sitecore.Security.Accounts.User user = Sitecore.Context.User;
      Sitecore.Security.UserProfile profile = user.Profile;
      string ItemIds = profile.GetCustomProperty("Favorites");

      foreach (string itemId in ItemIds.Split('|'))
      {
        Item item = Sitecore.Context.Database.GetItem(itemId);
        if (item != null) favs.Add(new GenericLink(item["Menu Title"], LinkManager.GetItemUrl(item), false));
      }

      return (favs.Count > 0) ? View("TertiaryNavigationPartialFavorites", favs as IEnumerable<GenericLink>) : null;
    }
    
    private ActionResult ShowListIsEmptyPageEditorAlert()
    {
      return IsPageEditorEditing ? View("ShowPageEditorAlert", new PageEditorAlert(PageEditorAlert.Alerts.ListIsEmpty)) : null;
    }
  }
}
