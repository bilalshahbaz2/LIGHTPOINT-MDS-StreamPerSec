using CommandProtocol.Requestable;
using CommandProtocol.Transferable;
using Ganss.Excel;
using MappingProtocol.Entities;
using MappingProtocol.Repos.GlobalMappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ServiceProtocol.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactsetProcessor.Mappers
{
    public class FactsetFieldMapper : IFieldMapper
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        private Dictionary<string, List<string>> mapping = new Dictionary<string, List<string>>();

        private Dictionary<string, string> sourceCollectorMapping = new Dictionary<string, string>();

        private List<GlobalMapping> globalMappingDb = new List<GlobalMapping>();


        private List<string> factsetFields = new List<string>();
        IConfiguration configuration;
        private readonly IServiceScopeFactory serviceProvider;

        public FactsetFieldMapper(IConfiguration _configuration, IServiceScopeFactory serviceProvider)  
        {
            configuration = _configuration;
            this.serviceProvider = serviceProvider;
            readExcelFile();
        }
        public void readExcelFile()
        {
            string execPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string excelPath = Path.Combine(execPath, "FactsetBbgMapping.xlsx");

            try
            {
                var Data = new ExcelMapper(excelPath).Fetch<mapExcel>();
                foreach (var item in Data)
                {
                    if (item.factsetMenumonics != null)
                    {
                        factsetFields.Add(item.factsetMenumonics);
                        if (mapping.ContainsKey(item.factsetMenumonics))
                        {
                            if (!string.IsNullOrEmpty(item.bbgMenmonics))
                            {
                                mapping[item.factsetMenumonics].Add(item.bbgMenmonics);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.bbgMenmonics))
                            {
                                List<string> value = new List<string>();
                                value.Add(item.bbgMenmonics);
                                mapping.Add(item.factsetMenumonics, value);
                            }
                            else
                            {
                                List<string> value = new List<string>();
                                mapping.Add(item.factsetMenumonics, value);
                            }


                        }

                        if (!string.IsNullOrEmpty(item.factsetMenumonics) && !string.IsNullOrEmpty(item.bbgMenmonics))
                        {
                            if (!sourceCollectorMapping.ContainsKey(item.bbgMenmonics))
                                sourceCollectorMapping.Add(item.bbgMenmonics, item.factsetMenumonics);

                        }
                    }
                    
                        
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Exception message {ex.Message}");
                throw new Exception(ex.Message);
            }
        }


        public List<string> GetFields()
        {
            return factsetFields;
        }

        public Dictionary<string, string> Map(Dictionary<string, string> FactsetFields)
        {
            Dictionary<string, string> transformedFields = new Dictionary<string, string>();
            foreach (var item in FactsetFields)
            {
                if (mapping.ContainsKey(item.Key))
                {
                    List<string> keys = mapping.GetValueOrDefault(item.Key);
                    foreach (var item2 in keys)
                    {
                        if (!string.IsNullOrEmpty(item2))
                        {
                            var value = item.Value.Trim('@');
                            if (value == "NA")
                            {
                                if(!transformedFields.ContainsKey(item2))
                                transformedFields.Add(item2, String.Empty);
                            }
                            else
                            {
                                if(!transformedFields.ContainsKey(item2))
                                transformedFields.Add(item2, item.Value);
                            }
                        }
                    }
                }
            }
            return transformedFields;
        }
        
        public Dictionary<string, string> ToSourceCollector(Dictionary<string, string> response)
        {
           
            Dictionary<string, string> mappedMnemonics = new Dictionary<string, string>();

            foreach (var item in response)
            {
                if(sourceCollectorMapping.TryGetValue(item.Key, out string value))
                {
                    mappedMnemonics.Add(value, item.Value);
                }
                else
                {
                    logger.Error($"Unbale to find mapping for live updates mnemonic : {value}");
                }
            }

            return mappedMnemonics;
        }


        public ResponseBag ToSourceCollector(Dictionary<string, string> FactsetFields, IncomingRequest request)
        {
            ResponseBag responseBag = new ResponseBag();
            responseBag.Items = new List<ResponseBagItem>();

                ResponseBagItem responseBagItem = new ResponseBagItem();
            var security = new CommandProtocol.Transferable.SecurityDefinition();
                    security.IdentifierType = "Ticker";
                    security.LastUpdate = DateTime.Now.ToString();
                    security.Message = "Factset";
                    security.SecurityIdentifier = request.RequestBag.Securities[0].SecurityIdentifier;


                    responseBagItem.Security = security;

                    var fieldDiscriptor = MapSubResponseToFieldDiscriptor(FactsetFields, request.RequestBag.Fields.Distinct<string>().ToList());

                    responseBagItem.FieldValues = fieldDiscriptor;

                    responseBag.Items.Add(responseBagItem);
            
            return responseBag;
        }



        public ResponseBag ToSourceCollector(List<Dictionary<string, string>> FactsetFields, IncomingRequest request)
        {
            var mapper = this.serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IGlobalMappingRepo>();
            globalMappingDb = mapper.GetGlobalMapping().Result;
            ResponseBag responseBag = new ResponseBag();
            responseBag.Items = new List<ResponseBagItem>();

            foreach (var response in FactsetFields)
            {
                ResponseBagItem responseBagItem = new ResponseBagItem();
                var security = new CommandProtocol.Transferable.SecurityDefinition();

                if (response.TryGetValue("identifier", out string value))
                {
                    security.IdentifierType = "Ticker";
                    security.LastUpdate = DateTime.Now.ToString();
                    security.Message = "Factset";
                    security.SecurityIdentifier = request.RequestBag.Securities[0].SecurityIdentifier;


                    responseBagItem.Security = security;

                    var fieldDiscriptor = MapResponseToFieldDiscriptor(response, request.RequestBag.Fields.Distinct<string>().ToList());

                    responseBagItem.FieldValues = fieldDiscriptor;

                    responseBag.Items.Add(responseBagItem);
                }
                else
                {
                    logger.Error("Ticker not found");
                }
            }
            return responseBag;
        }

        private Dictionary<string, FieldDescriptor> MapResponseToFieldDiscriptor(Dictionary<string, string> factsetFields, List<string> fields)
        {
            var result = new Dictionary<string, FieldDescriptor>();
            foreach (var field in fields)
            {
                if(sourceCollectorMapping.TryGetValue(field, out string factsetField))
                {
                    factsetFields.TryGetValue(factsetField, out string value);
                    
                    if(factsetField == "SEC_TYPE")
                    {
                        value = TransformSecurityType(value);
                    }


                    if (value != null) {
                        value = value.Trim('@');
                        if (value == "NA")
                        {
                            value = string.Empty;
                        }
                    }
                    
                        result.Add(field, new FieldDescriptor()
                    {
                        Value = value,
                        CollectorCode = factsetField,
                        HasError = false,
                        Key = field,
                        Message = string.Empty,
                        OriginatingSource = "Factset",
                        Timestamp = DateTime.Now.ToString()
                    });
                }
                else
                {
                    if(!result.ContainsKey(field))
                    {
                        result.Add(field, new FieldDescriptor()
                        {
                            Value = string.Empty,
                            CollectorCode = string.Empty,
                            HasError = true,
                            Key = field,
                            Message = "Unable to find mapping for factset mnemonic to bbg mnemonic.",
                            OriginatingSource = "Factset",
                            Timestamp = DateTime.Now.ToString()
                        });
                    }
                    else
                    {
                        logger.Warn($"Field {field} : Already expost in Result dictionary");
                    }
                    
                }
            }
            return result;
        }


        private Dictionary<string, FieldDescriptor> MapSubResponseToFieldDiscriptor(Dictionary<string, string> factsetFields, List<string> requestedfields)
        {
            var result = new Dictionary<string, FieldDescriptor>();
            foreach (var field in factsetFields)
            {
                if (mapping.TryGetValue(field.Key, out List<string> bbgFields))
                {
                    foreach (var item in bbgFields)
                    {
                        if(requestedfields.Contains(item))
                        {
                            factsetFields.TryGetValue(field.Key, out string value);

                            if (field.Key == "SEC_TYPE")
                            {
                                value = TransformSecurityType(value);
                            }

                            if (value != null)
                            {
                                value = value.Trim('@');
                                if (value == "NA")
                                {
                                    value = string.Empty;
                                }
                            }
                            if (!result.ContainsKey(item))
                            {
                                result.Add(item, new FieldDescriptor()
                                {
                                    Value = value,
                                    CollectorCode = field.Key,
                                    HasError = false,
                                    Key = item,
                                    Message = string.Empty,
                                    OriginatingSource = "Factset",
                                    Timestamp = DateTime.Now.ToString()
                                });
                            }
                        }
                    }

                }
            }
            return result;
        }


        private string TransformSecurityType(string factsetSecurity)
        {
            var globalMapObj = globalMappingDb.FirstOrDefault(x => x.CollectorValue == factsetSecurity);
            if (globalMapObj != null)
            {
                return globalMapObj.BBValue;
            }
            return null;
        }
    }

    public class mapExcel
    {
        public string factsetMenumonics { get; set; }
        public string bbgMenmonics { get; set; }
    }

}
