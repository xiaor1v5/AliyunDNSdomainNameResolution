/**************************************************************** 
 * 作    者：xiaor
 *
 * 创建时间：2018/2/27 17:22:17 
 *
 * 描述说明： 
 *
*****************************************************************/
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Aliyun.Acs.Alidns.Model.V20150109;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;

namespace DomainNameResolution
{
    public class DomainRecord
    {
        DomainRecordOptions Options { get; }
        
        IAcsClient client = null;

        ILogger logger = new LoggerFactory().AddConsole().CreateLogger<DomainRecord>(); 

        DescribeDomainRecordsRequest request = null;

        public DomainRecord(DomainRecordOptions options)
        {
            Options = options;
            Init();
            logger.LogInformation("DomainRecord init...");
        }

        private void Init()
        {
            var profile = DefaultProfile.GetProfile(Options.RegionId, Options.AccessKeyId, Options.AccessKeySecret);
            client = new DefaultAcsClient(profile);
            request = new DescribeDomainRecordsRequest();

            //request.Url = "http://domain.aliyuncs.com/";
            request.DomainName = Options.DomainName;
            request.TypeKeyWord = Options.DomainType;
            //request.ActionName = "DescribeDomainRecords";
        }

        private async Task<UpdateDomainRecordRequest> CheckIP()
        {
            string ip = await IPHelper.GetIP_Amazon(logger);
            UpdateDomainRecordRequest updateRequest = null;

            if (string.IsNullOrEmpty(ip))
            {
                return updateRequest;
            }

            try
            {
                //IAcsClient提供了两种类型的调用结果返回, 一种方式是通过调用doAction方法获取取得原始的api 调用结果, 即返回HttpResponse类型的结果. 示例代码如下：
                //HttpResponse httpResponse = client.doAction(describeCdnServiceRequest);
                //System.out.println(httpResponse.getUrl());
                //System.out.println(new String(httpResponse.getContent()));
                //另一种方式, 通过调用 getAcsResponse 方法, 获取反序列化后的对象, 示例代码如下:
                var response = client.GetAcsResponse(request);
                var domainList = response.DomainRecords;

                if (domainList.Count == 1)
                {
                    var domainRecord = domainList[0];
                    logger.LogInformation("domain name:" + domainRecord.DomainName);
                    logger.LogInformation("domain type:" + domainRecord.Type + "\r\n" + "domain value:" + domainRecord.Value);
                    if (domainRecord.Value != ip)
                    {
                        logger.LogInformation("当前外网IP：" + ip + "\r\n" + "解析IP：" + domainRecord.Value);
                        logger.LogInformation("IP发生变化，开始修改域名解析..." + domainRecord.DomainName);

                        updateRequest = new UpdateDomainRecordRequest();
                        updateRequest.RecordId = domainList[0].RecordId;
                        updateRequest.RR = domainList[0].RR;
                        updateRequest.Type = domainList[0].Type;
                        updateRequest.Value = ip;
                    }
                }
                else
                {
                    throw new Exception($"域名数：{domainList.Count}");
                }

            }
            catch (ServerException e)
            {
                logger.LogError(e, e.Message);
            }
            catch (ClientException e)
            {
                logger.LogError(e, e.Message);
            }
            
            return updateRequest;
        }

        public async Task CheckAndModify()
        {
            var result = await CheckIP();
            if (result != null)
            {
                ModifyRecord(result);
            }
        }

        private void ModifyRecord(UpdateDomainRecordRequest updateRequest)
        {
            try
            {
                var updateResponse = client.GetAcsResponse(updateRequest);
                logger.LogInformation("解析完成！");
            }
            catch (ServerException e)
            {
                logger.LogError(e, $"解析异常！异常信息：{e.Message}");
            }
            catch (ClientException e)
            {
                logger.LogError(e, $"解析异常！异常信息：{e.Message}");
            }
        }
    }
}
