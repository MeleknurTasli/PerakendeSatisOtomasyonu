using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class VKNProcessor
    {
        public static async Task<Models.VKNModel> LoadVKNinfo(string vkn)
        {
            string url = "http://service.cpm.com.tr/api/VknSorgu/?vkn=" + vkn + "&securitycode=palamutcpm1334";
            
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    Models.VKNModel vknmodel = await response.Content.ReadAsAsync<Models.VKNModel>();

                    return vknmodel;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
