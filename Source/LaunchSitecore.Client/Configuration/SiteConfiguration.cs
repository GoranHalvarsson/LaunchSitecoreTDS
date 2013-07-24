﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace LaunchSitecore.Configuration
{
    /// <summary>
    /// A collection of methods to centralize common tasks throughout the site.  This allows them to be managed 
    /// in one place and be easily reused.  Many of these methods look to Sitecore items for values.
    /// </summary>
    public static class SiteConfiguration
    {
        /// <summary>
        /// Wraps the standard sitecore dictionary call.  In order to have all site labels and standard text managed within the CMS, we use the distionary.
        /// These items simply hold the value for each label.    
        /// </summary>
        /// <param name="key">The dictionary key you are requesting.</param>
        /// <returns>The phrase value for the requested key.</returns>
        public static string GetDictionaryText(string key)
        {
            return Sitecore.Globalization.Translate.Text(key);
        }

        /// <summary>
        /// Launch Sitecore stores the prerquisite articles for each article.  In order to get a list of items that 
        /// you have fulfilled the prerequisites for, we need to run a query.  This method returns the query to use.        
        /// </summary>
        /// <param name="ItemId">The Item Id of the current article.</param>
        /// <returns>A string holding the query to use.</returns>
        public static string GetFurtherReadingArticlesQuery(string ItemId)
        {
            // Use Fast Query
            return String.Format("fast://sitecore/content//*[@Prerequisite Articles = '%{0}%']", ItemId);            
        }

        /// <summary>
        /// Since each Article has one or more Contributors, we need the ability to retreive the articles by a specific 
        /// author.  This method returns the query to use.       
        /// </summary>
        /// <param name="ContributorItemId">The Item Id of the Contributor.</param>
        /// <returns>A string holding the query to use.</returns>
        public static string GetArticlesByContributor(string ContributorItemId)
        {
            return String.Format(".//*[@@templatekey='article' and contains(@Contributors, '{0}')]", ContributorItemId);
        }

        /// <summary>
        /// Retreives all of the the articles in the site.  This method returns the query to use.        
        /// </summary>       
        /// <returns>A string holding the query to use.</returns>
        public static string GetArticles()
        {
            return ".//*[@@templatekey='article']";
        }

        /// <summary>
        /// All of the site settings items are stored beneath the Configuration item for the current site.        
        /// </summary>       
        /// <returns>The root configuration item.</returns>
        public static Item GetSiteSettingsItem()
        {
            return Sitecore.Context.Database.GetItem(String.Format("{0}/Configuration/Site Settings", GetHomeItem().Paths.FullPath));
        }

        /// <summary>
        /// All of the site settings items are stored beneath the Configuration item for the current site. This gets the presentation item below it.
        /// </summary>       
        /// <returns>The root configuration item.</returns>
        public static Item GetPresentationSettingsItem()
        {
            return Sitecore.Context.Database.GetItem(String.Format("{0}/Configuration/Presentation Settings", GetHomeItem().Paths.FullPath));
        }

        /// <summary>
        /// All of the site settings items are stored beneath the Configuration item for the current site.  This get the version info item beneath it.
        /// </summary>       
        /// <returns>The root configuration item.</returns>
        public static Item GetVersionInformationItem()
        {
            return Sitecore.Context.Database.GetItem(String.Format("{0}/Configuration/Version Information", GetHomeItem().Paths.FullPath));
        }

        /// <summary>
        /// All of the site settings items are stored beneath the Configuration item for the current site.  This gets the footer links item beneath it.
        /// </summary>       
        /// <returns>The root configuration item.</returns>
        public static Item GetFooterLinksItem()
        {
            return Sitecore.Context.Database.GetItem(String.Format("{0}/Configuration/Footer Links", GetHomeItem().Paths.FullPath));
        }

        /// <summary>
        /// All of the site settings items are stored beneath the Configuration item for the current site.  This gets the external sites item beneath it.
        /// </summary>       
        /// <returns>The root configuration item.</returns>
        public static Item GetExternalSitesItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/content/Global/External Sites");
        }

        /// <summary>
        /// All of the global settings items are stored beneath the Global item.
        /// </summary>       
        /// <returns>The root configuration item.</returns>
        public static Item GetGlobalItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/content/Global");
        }

        /// <summary>
        /// Quick access to the home node for the site.        
        /// </summary>       
        /// <returns>The home item.</returns>
        public static Item GetHomeItem()
        {
            // Since we want to support multi-site for evaluation purposes and do not create site nodes in the site section of 
            // the web.config, we will just go up the tree until we get to the content node.
            Item temp = Sitecore.Context.Item;
            Item contentNode = Sitecore.Context.Database.GetItem("/sitecore/content");
            while (temp.Parent != null && temp.ParentID != contentNode.ID) 
            { 
                temp = temp.Parent; 
            }
            return temp;
            
            // This is the best way to get the home node, but it only works if there is a site definition in the web.config
            //return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);

            // These options are also ways to get tot he home node.
            //return Sitecore.Context.Item.Axes.SelectSingleItem("ancestor-or-self::*[@@templatekey='home']");
            //return Sitecore.Context.Database.GetItem("/sitecore/content/home");
            
            // There are multiple ways to retrieve the home node.  You can reference the path, but this is bad for multisite installs.  
            // You can also check by template if all items share the same template type, but my favorite way is to use the StartPath 
            // which is on the site definition node in the config file.
        }

        /// <summary>
        /// Quick access to the articles node for the site.  Much of the content 
        /// is beneath this item, so it makes sense to have a quick way to access it.
        /// </summary>       
        /// <returns>The articles item.</returns>
        public static Item GetArticlesRootItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/content/home/articles");
        }

        /// <summary>
        /// Quick access to the glossary node for the site.
        /// </summary>       
        /// <returns>The glossary item.</returns>
        public static Item GetGlossaryItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/content/home/glossary");
        }
                      
        /// <summary>
        /// Quick access to the job function node for the site.
        /// </summary>       
        /// <returns>The glossary item.</returns>
        public static Item GetJobFunctionItem()
        {
            return Sitecore.Context.Database.GetItem("/sitecore/content/home/job function");
        }

        /// <summary>
        /// Quick access to the peelback profile node for the site.
        /// </summary>       
        /// <returns>The glossary item.</returns>
        public static Item GetPeelBackProfilePathItem()
        {
            Item config = SiteConfiguration.GetSiteSettingsItem();
            return Sitecore.Context.Database.GetItem(config["Profile Path"]);
        }       
    }
}