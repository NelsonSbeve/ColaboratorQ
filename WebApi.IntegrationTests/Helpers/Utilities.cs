using DataModel.Model;
using DataModel.Repository;
using Domain.Model;

namespace WebApi.IntegrationTests.Helpers;

public static class Utilities
{
    public static void InitializeDbForTests(AbsanteeContext db)
    {
        db.Colaborators.AddRange(GetSeedingColaboratorsDataModel());
        db.SaveChanges();
    }

    public static void ReinitializeDbForTests(AbsanteeContext db)
    {
        db.Colaborators.RemoveRange(db.Colaborators);
        db.SaveChanges();
    }

    public static List<ColaboratorDataModel> GetSeedingColaboratorsDataModel()
    {
        return new List<ColaboratorDataModel>
        {
            new ColaboratorDataModel(new Colaborator("Catarina Moreira", "catarinamoreira@email.pt", "street", "4000-000")),
            new ColaboratorDataModel(new Colaborator("a", "a@email.pt", "street", "4000-000")),
            new ColaboratorDataModel(new Colaborator("kasdjflkadjf lkasdfj laksdjf alkdsfjv alkdsfjv asl", "kasdjflkadjf@email.pt", "street", "4000-000"))
        };
    }
}
