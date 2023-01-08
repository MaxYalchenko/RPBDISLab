using CompanyApp.Models;
namespace CompanyApp.Data
{
    public static class Initializer
    {
        public static void Initialize(Context context)
        {
            context.Database.EnsureCreated();
            if (context.Subdivisions.Any())
            {
                return;
            }
            for (int i = 1; i <= 100; i++)
            {
                string name = "Subdivision" + new Random().Next(100).ToString();
                int amount = new Random().Next(100) + 50;
                context.Subdivisions.Add(new Subdivision { subdivisionName = name, amountSubdivision = amount });
            }
            //context.SaveChanges();
            for (int i = 1; i <= 100; i++)
            {
                string name = "SubdivisionFact" + new Random().Next(100).ToString();
                DateTime date = DateTime.Now.AddDays(-new Random().Next(100));
                int index = new Random().Next(100) + 50;
                int id = new Random().Next(context.Subdivisions.Count());
                int subdivisionid = context.Subdivisions.ToList()[id].subdivisionId;
                context.SubdivisionFacts.Add(new SubdivisionFact { subdivisionFactDate = date, subdivisionFactIndex = index, subdivisionId = subdivisionid });
            }
            //context.SaveChanges();
            for (int i = 1; i <= 100; i++)
            {

                string name = "SubdivisionPlan" + new Random().Next(100).ToString();
                DateTime date = DateTime.Now.AddDays(-new Random().Next(100));
                int index = new Random().Next(100) + 50;
                int id = new Random().Next(context.Subdivisions.Count());
                int subdivisionid = context.Subdivisions.ToList()[id].subdivisionId;
                context.SubdivisionPlans.Add(new SubdivisionPlan { subdivisionPlanDate = date, subdivisionPlanIndex = index, subdivisionId = subdivisionid });
            }
            //context.SaveChanges();
            for (int i = 1; i <= 100; i++)
            {
                string name = "Workpeople" + new Random().Next(100).ToString();
                int amount = new Random().Next(100) + 50;
                int id = new Random().Next(context.Subdivisions.Count());
                int subdivisionid = context.Subdivisions.ToList()[id].subdivisionId;
                string achive = "Achivements" + new Random().Next(100).ToString();

                context.Workpeoples.Add(new Workpeople { peopleName = name, amountPeople = amount, Achievements = achive, subdivisionId = subdivisionid });
            }
            //context.SaveChanges();
            for (int i = 1; i <= 100; i++)
            {
                string name = "PeoplePlan" + new Random().Next(100).ToString();
                DateTime date = DateTime.Now.AddDays(-new Random().Next(100));
                int index = new Random().Next(100) + 50;
                int id = new Random().Next(context.Subdivisions.Count());
                int workpeopleid = new Random().Next(100);
                context.PeoplePlans.Add(new PeoplePlan { peoplePlanDate = date, peoplePlanIndex = index, workPeopleId = workpeopleid });
            }
            //context.SaveChanges();
            for (int i = 1; i <= 200; i++)
            {
                string name = "PeopleFact" + new Random().Next(100).ToString();
                DateTime date = DateTime.Now.AddDays(-new Random().Next(100));
                int index = new Random().Next(100) + 50;
                int id = new Random().Next(context.Subdivisions.Count());
                int workpeopleid = new Random().Next(100);
                context.PeopleFacts.Add(new PeopleFact { peopleFactDate = date, peopleFactIndex = index, workPeopleId = workpeopleid });
            }
            //context.SaveChanges();
        }
    }
}
