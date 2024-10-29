namespace EAMI.CommonEntity
{
    public class PaymentExcEntityInfo
    {
        public int PEEInfo_PK_ID { get; set; }
        public string PEE_Name {
            get
            {
                return this.PEE.EntityName;
            }
            
        }
        public PayeeEntity PEE { get; set; }
        public string PEE_FullCode
        {
            get 
            {                          
                return PEE.Code + (string.IsNullOrWhiteSpace(PEE_IdSfx) ? string.Empty : "-" + PEE_IdSfx);
            }
        }
        public string PEE_IdSfx { get; set; }
        public string PEE_AddressLine1 { get; set; }
        public string PEE_AddressLine2 { get; set; }
        public string PEE_AddressLine3 { get; set; }
        public string PEE_City { get; set; }
        public string PEE_State { get; set; }
        public string PEE_Zip { get; set; }
        public string PEE_AddressCSZ
        {
            get
            {
                Tuple<string, string> parsedZip = ParseZipCodePartsFromFullZipCode(PEE_Zip);
                return string.Concat(PEE_City, ", ", PEE_State, " ", 
                    parsedZip.Item2 == string.Empty ? parsedZip.Item1 : parsedZip.Item1 + "-" + parsedZip.Item2);
            }
        }
        public string PEE_ContractNumber { get; set; }
        public string PEE_VendorTypeCode { get; set; }

        public EftInfo PEE_EftInfo { get; set; }
        public bool IsEft
        {
            get
            {
                return (this.PEE_EftInfo != null && !string.IsNullOrWhiteSpace(this.PEE_EftInfo.PrvAccountNo));
            }
        }


        private Tuple<string, string> ParseZipCodePartsFromFullZipCode(string fullZipCode)
        {
            Tuple<string, string> result = new Tuple<string, string>(string.Empty, string.Empty);

            /* *
             * EXAMPLE:
             * 94080
             * 930368294
             * */

            try
            {
                string zip5 = string.Empty;
                string zip4 = string.Empty;

                //PARSE ZIP CODE

                //REPLACE: Remove spaces and hyphens
                string line = fullZipCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                //CUT: keep 9 right characters
                line = line.Substring(Math.Max(0, line.Length - 9));

                //GET zip5+4
                if (line.All(Char.IsDigit) && line.Length == 9)
                {
                    zip5 = line.Substring(0, 5);
                    zip4 = line.Substring(5, 4);
                }
                else
                {
                    //CUT: keep 5 right characters
                    line = line.Substring(Math.Max(0, line.Length - 5));

                    //GET zip5 only
                    if (line.All(Char.IsDigit) && line.Length == 5)
                    {
                        zip5 = line;
                    }
                }

                //Resilt
                result = new Tuple<string, string>(zip5, zip4);
            }
            catch { }

            return result;
        }
    }
}
