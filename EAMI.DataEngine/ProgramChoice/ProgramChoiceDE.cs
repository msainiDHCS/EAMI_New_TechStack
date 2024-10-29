namespace EAMI.DataEngine
{
    public class ProgramChoiceDE: IProgramChoiceDE
    {
        public ProgramChoiceDE()
        {

        }
        public string GetConnectionString(int prgId)
        {
            if (prgId == 1)
            {
                return "ManagedCareDB";
            }
            else if (prgId == 2)
            {
                return "DentalDB";
            }
            else if (prgId == 3)
            {
                return "PharmacyDB";
            }
            else
            {
                return "Invalid ProgramId";
            }
        }
    }
}
