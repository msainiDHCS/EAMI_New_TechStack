using EAMI.DataEngine;

namespace EAMI.RuleEngine
{
    public class ProgramChoiceRE : IProgramChoiceRE
    {
        private IProgramChoiceDE _programChoiceDE;

        public ProgramChoiceRE(IProgramChoiceDE programChoiceDE)
        {
            _programChoiceDE = programChoiceDE;
        }
        public string GetDataBaseName(int prgId)
        {
           string dbName = _programChoiceDE.GetConnectionString(prgId);
            return dbName;
        }
    }
}
