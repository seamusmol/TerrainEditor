using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Geometry
{
    public class CollisionResults
    {
        private List<CollisionResult> Results { get; set; } = new List<CollisionResult>();

        public int Count{get;set;}

        public CollisionResults()
        {

        }

        public CollisionResult GetClosest()
        {
            Results = Results.OrderBy(x => x.Distance).ToList();
            if (Results.Count == 0)
            {
                return null;
            }
            return Results[0];
        }
        
        public void Add(CollisionResult NewResult)
        {
            if (!Results.Contains(NewResult))
            {
                Results.Add(NewResult);
                Count = Results.Count;
                Results = Results.OrderBy(x => x.Distance).ToList();
            }
        }

        public void AddRange(CollisionResults NewResults)
        {
            Results.AddRange(NewResults.Results);
            Count = Results.Count();
            Results = Results.OrderBy(x => x.Distance).ToList();
        }

        public void AddRange(List<CollisionResult> NewResults)
        {
            Results.AddRange(NewResults);
            Count = Results.Count;
            Results = Results.OrderBy(x => x.Distance).ToList();
        }

    }
}
