//    megridview v0.4.1
//    Developed By Mesut Talebi(mesut.talebi @yahoo.com)
//    Open Source And no licence :) free to use

using System;
using System.Web.Mvc;

namespace MT.GridView.HtmlHelpers
{
    public static class GridviewHelper
    {
        public static MvcHtmlString GridView(this HtmlHelper html, string readUrl, string gridName, 
            Func<MvcHtmlString> filterPartial = null)
        {
            var gridview = new TagBuilder("div");
            var gridviewHeader = new TagBuilder("div");
            var gridviewSearch = new TagBuilder("div");
            var gridviewViewport = new TagBuilder("div");

            gridview.InnerHtml = "<!--GRIDVIEW-->";
            gridview.AddCssClass("gridview " + gridName);

            //Grid Header
            gridviewHeader.AddCssClass("gridview-header");
            gridviewHeader.InnerHtml += @"<div class=""row""><div class=""col-sm-6"">";

            gridviewSearch.AddCssClass("gridview-search");
            gridviewSearch.InnerHtml = $@"<div class=""search input-group"">
                                <input class=""form-control"" placeholder=""Ara..."" type=""search"">
                                <span class=""input-group-btn"">
                                    <button class=""btn btn-default"" type=""button"">
                                        <span class=""fa fa-search""></span>
                                        <span class=""sr-only"">Ara</span>
                                    </button>
                                </span>
                            </div>";
           
            gridviewHeader.InnerHtml+= gridviewSearch.ToString();

            // Close col-sm-6
            gridviewHeader.InnerHtml+= "</div>";

            if (filterPartial != null)
            {
                gridviewHeader.InnerHtml += filterPartial.Invoke();
            }
            else
            {
                gridviewHeader.InnerHtml += @"<div class=""col-sm-6 text-right""></div>";
            }

            gridviewHeader.InnerHtml+="</div><!--End Of Row-->";
            //End of GridView Header
            gridview.InnerHtml += gridviewHeader;

            //Gridview ViewPort
            gridviewViewport.AddCssClass("gridview-viewport");
            gridviewViewport.Attributes.Add("id", "viewport");
            gridviewViewport.Attributes.Add("data-getdata-function", readUrl);
            gridviewViewport.InnerHtml+= @"<div class=""gridview-canvas table-responsive"" style=""min-height:400px;"">                        
                    </div>
                    <div class=""loading gridview-loader"">
                        <img src = ""/images/loading.gif"" />
                    </div>";
            //End of GridView ViewPort
            gridview.InnerHtml+= gridviewViewport.ToString();

            //Gridview Footer
            gridview.InnerHtml+= @"<div class=""gridview-footer"">
                </div>";

            gridview.InnerHtml += "<!-- /END OF GRIDVIEW-->";

            return MvcHtmlString.Create(gridview.ToString());
        }
    }
}