using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using librets;
using System.Collections;
namespace RETsTest
{
    public class ConnectToEELE
    {
        static void Main(string[] args)
        {
            bool isLoggedIn = false;
           
            using (RetsSession retssession = new RetsSession("http://www.eele-rets.com:6160/rets/login"))
            {
                
                retssession.UseHttpGet(true);
                //retssession.
                isLoggedIn = retssession.Login("corcoran", "JDXzmkGa");
                              
                if (isLoggedIn)
                {
                  
                    var urls = retssession.GetCapabilityUrls();
                    Console.WriteLine(urls.GetLoginUrl());

                    RetsMetadata metadata = retssession.GetMetadata();
                    IEnumerable resources = metadata.GetAllResources();
                 
                    using (SearchRequest searchRequest = retssession.CreateSearchRequest(
                                                    "Property",
                                                    "CLISTINGS",
                                                    "(Modified=2016-12-12T00:00:00+)"))
                    {
                        searchRequest.SetQueryType(SearchRequest.QueryType.DMQL2);
                        searchRequest.SetStandardNames(false);
                        searchRequest.SetOffset(SearchRequest.OFFSET_NONE);
                        SearchResultSet results = retssession.Search(searchRequest);
                        Console.WriteLine("Record count: " + results.GetCount());
                        Console.WriteLine();
                        IEnumerable columns = results.GetColumns();
                        while (results.HasNext())
                        {
                            foreach (string column in columns)
                            {
                                Console.WriteLine(column + ": " + results.GetString(column));
                            }
                            Console.WriteLine();
                        }
                    }
                }

               

            }
        }

        static void dumpAllClasses(RetsMetadata metadata,
                                   MetadataResource resource)
        {
            string resourceName = resource.GetResourceID();
            IEnumerable classes = metadata.GetAllClasses(resourceName);
            foreach (MetadataClass aClass in classes)
            {
                Console.WriteLine("Resource name: " + resourceName + " [" +
                    resource.GetStandardName() + "]");
                Console.WriteLine("Class name: " + aClass.GetClassName() +
                    " [" + aClass.GetStandardName() + "]");
                dumpAllTables(metadata, aClass);
                Console.WriteLine();
            }
        }
        static void dumpAllTables(RetsMetadata metadata, MetadataClass aClass)
        {
            IEnumerable tables = metadata.GetAllTables(aClass);
            foreach (MetadataTable table in tables)
            {
                Console.WriteLine("Table name: " + table.GetSystemName() + " [" +
                    table.GetStandardName() + "]");
                Console.WriteLine("\tTable datatype: " + table.GetDataType());
                Console.WriteLine("\tUnique: " + table.IsUnique());
                Console.WriteLine("\tMax Length: " + table.GetMaximumLength());
            }
        }
    }
}
