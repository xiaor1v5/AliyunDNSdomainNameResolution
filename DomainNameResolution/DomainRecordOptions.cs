/**************************************************************** 
 * 作    者：xiaor
 *
 * 创建时间：2018/2/27 17:31:13 
 *
 * 描述说明： 
 *
*****************************************************************/  
namespace DomainNameResolution
{
    public class DomainRecordOptions
    {
        /// <summary>
        /// 必填固定值，必须为"cn-hangzhou"
        /// </summary>
        public string RegionId { get; set; }

        /// <summary>
        /// your accessKey
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// your accessSecret
        /// </summary>
        public string AccessKeySecret { get; set; }
        public string DomainName { get; set; }
        public string DomainType { get; set; }
    }
}
