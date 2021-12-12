using ESourcing.Sourcing.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESourcing.Sourcing.Data
{
    public class SourcingContextSeed
    {
        public static void SeedData(IMongoCollection<Auction> auctionCollection)
        {
            bool exist = auctionCollection.Find(p => true).Any();
            if (!exist)
            {
                auctionCollection.InsertManyAsync(GetPreConfig());
            }
        }

        private static IEnumerable<Auction> GetPreConfig()
        {
            return new List<Auction>()
            {
                new Auction()
                {
                    Name="Auction 1",
                     Description="auction desc 1",
                     CreatedAt=DateTime.Now,
                      StartedAt=DateTime.Now,
                       FinishedAt=DateTime.Now.AddDays(10),
                       ProductId="111111111111111111111",
                        Quantity=5,
                         Status=(int)Status.Active,
                          IncludedSellers=new List<string>()
                          {
                              "erol1@test.com",
                              "erol2@test.com",
                              "erol3@test.com"
                          }


                },
                new Auction()
                {
                    Name="Auction 2",
                     Description="auction desc 2",
                     CreatedAt=DateTime.Now,
                      StartedAt=DateTime.Now,
                       FinishedAt=DateTime.Now.AddDays(20),
                       ProductId="222222222222222222",
                        Quantity=5,
                         Status=(int)Status.Active,
                          IncludedSellers=new List<string>()
                          {
                              "erol1@test.com",
                              "erol2@test.com",
                              "erol3@test.com"
                          }


                },
                  new Auction()
                {
                    Name="Auction 3",
                     Description="auction desc 3",
                     CreatedAt=DateTime.Now,
                      StartedAt=DateTime.Now,
                       FinishedAt=DateTime.Now.AddDays(20),
                       ProductId="333333333333333",
                        Quantity=5,
                         Status=(int)Status.Active,
                          IncludedSellers=new List<string>()
                          {
                              "erol1@test.com",
                              "erol2@test.com",
                              "erol3@test.com"
                          }


                }
            };
        }
    }
}
