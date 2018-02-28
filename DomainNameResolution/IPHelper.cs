/**************************************************************** 
 * 作    者：xiaor
 *
 * 创建时间：2018/2/27 11:27:23 
 *
 * 描述说明： 
 *
*****************************************************************/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DomainNameResolution
{
    public class IPHelper
    {
        public static async Task<string> GetIP_Sohu()
        {
            HttpClient client = new HttpClient();
            var responseMessage = await client.GetAsync("http://pv.sohu.com/cityjson");
            var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encodeList = Encoding.GetEncodings();
            var encode = Encoding.GetEncoding("gbk");
            var responseText = new StreamReader(responseStream, encode).ReadToEnd();

            var startIndex = 18;

            var ip = responseText.Substring(startIndex);

            ip = ip.Trim();
            ip = ip.Trim(';');

            var ipObj = JsonConvert.DeserializeObject<CitySN>(ip);
            return ipObj.cip;
        }

        public static async Task<string> GetIP_Amazon()
        {
            HttpClient client = new HttpClient();
            var responseMessage = await client.GetAsync("http://checkip.amazonaws.com");
            var responseString = (await responseMessage.Content.ReadAsStringAsync()).Trim('\n');
            return responseString;
        }
    }
}
