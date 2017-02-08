using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Views
{
    public static class Utils
    {
        public static string GetLink(string url, string description)
            => $"<a href=\"#\" onclick=\"return popitup('{url}')\">{description}</a>";

        public static string GetPopItUpJsFunction() =>
           @"function popitup(url) {

                window.location.href = url;
                return false;

                /* newwindow = window.open(url, 'name', 'height=800,width=1200');
                if (window.focus) { newwindow.focus()}
                return false; */
            }";

        #region "Links"

        public static string GetShowDownwardTreeLink(int personId) => GetLink(GetShowDownardTreeUrl() + personId, "🡓 show downward tree");

        public static string GetShowUpwardTreeLink(int personId) => GetLink(GetShowUpwardTreeUrl() + personId, "🡑 show upward tree");

        public static string GetEditMarriageLink(int personId) => GetLink(GetEditMarriageUrl() + personId, "✎ edit marriage");

        public static string GetEditPersonLink(int personId) => GetLink(GetEditPersonUrl() + personId, "✎ edit person");

        #endregion

        #region "URLs"

        public static string GetShowDownardTreeUrl() => $"/data/persondownwardtree?personId=";

        public static string GetShowUpwardTreeUrl() => $"/data/personupwardtree?personId=";

        public static string GetEditMarriageUrl() => $"/data/marriageByPersonId?personId=";

        public static string GetEditPersonUrl() => $"/data/personById?personId=";

        #endregion

    }
}
