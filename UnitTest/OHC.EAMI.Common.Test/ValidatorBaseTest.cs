using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OHC.EAMI.Common;
using System.Text.RegularExpressions;

namespace OHC.EAMI.Common.Test
{
    /// <summary>
    /// this class implements unit tests for Common.ValidatorBase type and serves  
    ///   as sample for simple or complex implementation of ValidationBase class
    /// </summary>
    [TestClass]
    public class ValidatorBaseTest
    {
        public static string Name = "SomeName";
        public static int Number = 555;
        public static DateTime Date = Convert.ToDateTime("2017-06-20");

        /// <summary>
        /// test simple (derived) validator implemented instance
        /// </summary>
        [TestMethod]
        public void TestSimpleValidatorInstance()
        {
            MockSimpleNameValidator nameValidator = new MockSimpleNameValidator();
            Assert.IsTrue(nameValidator.Execute(ValidatorBaseTest.Name), "expected name match did not occur");
            Assert.IsTrue(nameValidator.Execute(ValidatorBaseTest.Name.ToLower()), "expected name match did not occur");
            Assert.IsFalse(nameValidator.Execute(ValidatorBaseTest.Name + "qq"), "expacted name missmatch did not occur");
            Assert.IsFalse(nameValidator.IsCaseSensitive, "expected 'false' as default value");
            Assert.IsTrue(nameValidator.IsStopping, "expected 'true' as default avlue");
            Assert.IsTrue(nameValidator.ErrorCode == string.Empty, "expected empty string as default value");
            Assert.IsTrue(nameValidator.ValidatorName != string.Empty, "expected a validator name");
        }

        /// <summary>
        /// test simple (derived) validator instance with optional constructor
        /// </summary>
        [TestMethod]
        public void TestSimpleValidatorImplementedWCtor()
        {
            MockSimpleNameValidator nameValidator = new MockSimpleNameValidator(true);
            Assert.IsTrue(nameValidator.Execute(ValidatorBaseTest.Name), "expected name match did not occur");
            Assert.IsFalse(nameValidator.Execute(ValidatorBaseTest.Name.ToLower()), "expected name missmatch did not occur");
            Assert.IsFalse(nameValidator.Execute(ValidatorBaseTest.Name + "qq"), "expacted name missmatch did not occur");
            Assert.IsTrue(nameValidator.IsCaseSensitive, "expected 'true' value");
            Assert.IsTrue(nameValidator.IsStopping, "expected 'true' as default avlue");
            Assert.IsTrue(nameValidator.ErrorCode == string.Empty, "expected empty string as default value");
            Assert.IsTrue(nameValidator.ValidatorName != string.Empty, "expected a validator name");
        }

        /// <summary>
        /// test simple validator instance via its base type interface
        /// </summary>
        [TestMethod]
        public void TestSimpleBaseValidatorInstance()
        {
            ValidatorBase<string, bool> validator = new MockSimpleNameValidator();
            Assert.IsTrue(validator.Execute(ValidatorBaseTest.Name), "expected name match did not occur");
            Assert.IsTrue(validator.Execute(ValidatorBaseTest.Name.ToLower()), "expected name match did not occur");
            Assert.IsFalse(validator.Execute(ValidatorBaseTest.Name + "qq"), "expacted name missmatch did not occur");
            Assert.IsTrue(validator.IsStopping, "expected 'true' as default avlue");
            Assert.IsTrue(validator.ErrorCode == string.Empty, "expected empty string as default value");
            Assert.IsTrue(validator.ValidatorName != string.Empty, "expected a validator name");
        }

        /// <summary>
        /// test simple validator instance (with optional Ctor) via its base type interface
        /// </summary>
        [TestMethod]
        public void TestSimpleBaseValidatorInstanceWCtor()
        {
            ValidatorBase<string, bool> validator = new MockSimpleNameValidator(true);
            Assert.IsTrue(validator.Execute(ValidatorBaseTest.Name), "expected name match did not occur");
            Assert.IsFalse(validator.Execute(ValidatorBaseTest.Name.ToLower()), "expected name missmatch did not occur");
            Assert.IsFalse(validator.Execute(ValidatorBaseTest.Name + "qq"), "expacted name missmatch did not occur");
            Assert.IsTrue(validator.IsStopping, "expected 'true' as default avlue");
            Assert.IsTrue(validator.ErrorCode == string.Empty, "expected empty string as default value");
            Assert.IsTrue(validator.ValidatorName != string.Empty, "expected a validator name");            
        }

        /// <summary>
        /// test complex validator instance
        /// </summary>
        [TestMethod]
        public void TestComplexValidatorInstance()
        {
            MockComplexNameValidator xNameValidator = new MockComplexNameValidator();

            //create good data context to test happy path validation
            MockDataContext dataContext = new MockDataContext();
            Assert.IsTrue(xNameValidator.Execute(dataContext).Status, "expected 'true' status value");

            //modify data context to perform negative validation test
            dataContext.Name = "Booom";
            CommonStatus result = xNameValidator.Execute(dataContext);
            Assert.IsFalse(result.Status, "expected 'false' status value");
            Assert.IsTrue(result.HasDetails(), "expected error details");
            Assert.IsTrue(result.MessageDetailList[0] != string.Empty, "expected detail message text");           
        }

        /// <summary>
        /// test complex GroupValidator instance
        /// </summary>
        [TestMethod]
        public void TestComplexGroupValidatorInstance()
        {
            MockComplexContextGroupValidator xContextValidator = new MockComplexContextGroupValidator();

            //create good data context to test happy path validation
            MockDataContext dataContext = new MockDataContext();
            Assert.IsTrue(xContextValidator.Execute(dataContext).Status, "expected 'true' status value");

            //modify all 3 data context properties to perform negative validation test
            dataContext.Name = "Baaam";
            dataContext.Number++;
            dataContext.Date = dataContext.Date.AddDays(5);
            CommonStatus result = xContextValidator.Execute(dataContext);
            Assert.IsFalse(result.Status, "expected 'false' status value");
            Assert.IsTrue(result.HasDetails(), "expected error details");
            Assert.IsTrue(result.MessageDetailList.Count == 3, "expected 3 message detials");
            Assert.IsTrue(result.GetCombinedMessage() != string.Empty, "expected valid text in CombinedMessage");
            Console.WriteLine(result.GetCombinedMessage(true));            
        }

        /// <summary>
        /// test complex GroupValidator instance (with stopping logic)
        /// </summary>
        [TestMethod]
        public void TestComplexGroupValidatorInstanceWStoppingLogic()
        {
            // stopping functionality will stop and exit after first failed validation
            MockComplexContextGroupValidator xContextValidator = new MockComplexContextGroupValidator(true);
            // here we modify data context to make it bad and execute group validator
            MockDataContext dataContext = new MockDataContext();
            dataContext.Name = "Baaam";
            dataContext.Number++;
            dataContext.Date = dataContext.Date.AddDays(5);
            CommonStatus result = xContextValidator.Execute(dataContext);
            Assert.IsFalse(result.Status, "expected 'false' status value");
            Assert.IsTrue(xContextValidator.IsStopping, "expected 'true' for IsStopping member");
            Assert.IsTrue(result.HasDetails(), "expected error details");
            Assert.IsTrue(result.MessageDetailList.Count == 1, "expected 1 message detial");
            Assert.IsTrue(result.GetCombinedMessage() != string.Empty, "expected valid text in CombinedMessage");
            Console.WriteLine();
            Console.WriteLine(result.GetCombinedMessage(true));
        }
    }




    /// <summary>
    /// mock data context class
    /// </summary>
    internal class MockDataContext
    {
        public MockDataContext()
        {
            this.Name = ValidatorBaseTest.Name;
            this.Number = ValidatorBaseTest.Number;
            this.Date = ValidatorBaseTest.Date;
        }
        public string Name { get; set; }
        public int Number { get; set; }
        public DateTime Date { get; set; }
    }    
    
    /// <summary>
    /// mock/sample Name Validator class that implements simple ValidatorBase abstract class
    /// </summary>
    internal class MockSimpleNameValidator : ValidatorBase<string, bool>
    {
        public MockSimpleNameValidator() : this(false)
        { }

        public MockSimpleNameValidator(bool isCaseSensitive)
            : base("Name Validator")
        {
            this.IsCaseSensitive = isCaseSensitive;            
        }

        public bool IsCaseSensitive { get; private set; }     

        public override bool Execute(string context)
        {
            return  (IsCaseSensitive && context == "SomeName") || (!IsCaseSensitive && context.ToUpper() == "SOMENAME");
        }
    }
    
    /// <summary>
    /// mock/sample X-Name Validator class that implements complex ValidatorBase abstract class
    /// </summary>
    internal class MockComplexNameValidator : ValidatorBase<MockDataContext, CommonStatus>
    {
        public MockComplexNameValidator()
            : this(false)
        { }

        public MockComplexNameValidator(bool isCaseSensitive)
            : base("X-NameValidator", "201", true)
        {
            this.IsCaseSensitive = isCaseSensitive;
        }

        public bool IsCaseSensitive { get; private set; }

        public override CommonStatus Execute(MockDataContext context)
        {
            CommonStatus cs = new CommonStatus(true);
            string errMsg = string.Empty;

            // expected or allowable name validation logic 
            if (IsCaseSensitive && !(context.Name == "SomeName"))
            {
                cs.Status = false;
                errMsg = "incorrect Name value (using case sensitive match; )";

            }
            else if (!IsCaseSensitive && !(context.Name.ToUpper() == "SOMENAME"))
            {
                cs.Status = false;
                errMsg = "incorrect X-Name value; ";
            }

            // -no numbers in the name- validation logic
            if (Regex.IsMatch(context.Name, @"\d"))
            {
                cs.Status = false;
                errMsg = errMsg + "Name cannot contain a number";
            }

            // format the message
            if (!cs.Status)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ERROR CODE: " + this.ErrorCode);
                sb.AppendLine("ERROR MSG : " + errMsg);
                sb.AppendLine("VALIDATION: " + this.ValidatorName);
                cs.AddMessageDetail(sb.ToString());
            }
            return cs;
        }
    }

    /// <summary>
    /// mock/sample X-Number Validator class that implements complex ValidatorBase abstract class
    /// </summary>
    internal class MockComplexNumberValidator : ValidatorBase<MockDataContext, CommonStatus>
    {
        public MockComplexNumberValidator()
            : base("X-NumberValidator", "202", true)
        { }

        public override CommonStatus Execute(MockDataContext context)
        {
            CommonStatus cs = new CommonStatus(true);

            // expected or allowable number validation logic 
            if (context.Number != 555)
            {
                cs.Status = false; 
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ERROR CODE: " + this.ErrorCode);
                sb.AppendLine("ERROR MSG : incorrect X-Number value");
                sb.AppendLine("VALIDATION: " + this.ValidatorName);
                cs.AddMessageDetail(sb.ToString());
            }

            return cs;
        }
    }

    /// <summary>
    /// mock/sample X-Date Validator class that implements complex ValidatorBase abstract class
    /// </summary>
    internal class MockComplexDateValidator : ValidatorBase<MockDataContext, CommonStatus>
    {
        public MockComplexDateValidator()
            : base("X-DateValidator", "203", true)
        { }

        public override CommonStatus Execute(MockDataContext context)
        {
            CommonStatus cs = new CommonStatus(true);

            // expected or allowable date validation logic 
            if (!context.Date.Equals(Convert.ToDateTime("2017-06-20")))
            {
                cs.Status = false;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("ERROR CODE: " + this.ErrorCode);
                sb.AppendLine("ERROR MSG : incorrect X-Date value");
                sb.AppendLine("VALIDATION: " + this.ValidatorName);
                cs.AddMessageDetail(sb.ToString());
            }

            return cs;
        }
    }

    /// <summary>
    /// a mock/sample X-Context group validator class that combines three different validations
    /// </summary>
    internal class MockComplexContextGroupValidator : ValidatorBase<MockDataContext, CommonStatus>
    {
        public MockComplexContextGroupValidator()
            : this(false)
        { }

        public MockComplexContextGroupValidator(bool isStopping)
            : base("X-ContextValidator", "200", isStopping)
        { }

        public override CommonStatus Execute(MockDataContext context)
        {
            CommonStatus overallStatus = new CommonStatus(true);
            foreach(ValidatorBase<MockDataContext, CommonStatus> validator in GetValidators())
            {                
                CommonStatus validatorStatus = validator.Execute(context);                
                overallStatus.AddCommonStatus(validatorStatus);
                
                // exit loop if the stopping validator logic is tripped
                if (this.IsStopping && validator.IsStopping && !validatorStatus.Status)
                {
                    break;
                }
            }
            return overallStatus;
        }

        /// <summary>
        /// get list of validator to be executed
        /// </summary>
        /// <returns>list of validator objects</returns>
        internal List<ValidatorBase<MockDataContext, CommonStatus>> GetValidators()
        {
            return new List<ValidatorBase<MockDataContext, CommonStatus>>()
            {                
                new MockComplexNameValidator(true),
                new MockComplexNumberValidator(),
                new MockComplexDateValidator()
            };
        }
    }
    
}
