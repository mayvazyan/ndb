using System;

namespace ITCreatings.Ndb.NdbConsole.Formatters
{
    public enum Outcome
    {
        Passed, Failed, Inconclusive
    }

    public class MsTest2008Formatter : XmlFormatter
    {
        private int total;
        private int inconclusive;
        private int passed;
        private int failed;

        public MsTest2008Formatter(string filename) : base(filename)
        {
            writer.WriteComment("MsTest2008Formatter output");
            writer.WriteStartElement("TestRun", @"http://microsoft.com/schemas/VisualStudio/TeamTest/2006");
//            writer.WriteAttributeString("xmlns", "http://microsoft.com/schemas/VisualStudio/TeamTest/2006");
            writer.WriteAttributeString("id", Guid.NewGuid().ToString());
            writer.WriteAttributeString("name", "");
            writer.WriteAttributeString("runUser", "");
            writer.WriteStartElement("Results");
        }

        public override void AppendUnitTestResult(string testName, Outcome outcome, string message)
        {
//            if (total > 0)
//                return;
            total ++;
            switch (outcome)
            {
                case Outcome.Passed:
                    passed++;
                    break;
                
                case Outcome.Failed:
                    failed++;
                    break;
                    
                case Outcome.Inconclusive:
                    inconclusive++;
                    break;
            }
            writer.WriteStartElement("UnitTestResult");
            writer.WriteAttributeString("testName", testName);
            writer.WriteAttributeString("duration", "01:01:01.0000001");
            writer.WriteAttributeString("outcome", outcome.ToString());

            writer.WriteAttributeString("computerName", "");
            writer.WriteAttributeString("testId", Guid.NewGuid().ToString());
            writer.WriteAttributeString("testListId", Guid.NewGuid().ToString());
            writer.WriteAttributeString("executionId", Guid.NewGuid().ToString());
            writer.WriteAttributeString("testType", "");

            writer.WriteStartElement("Output");
                writer.WriteStartElement("ErrorInfo");
                    writer.WriteStartElement("Message");
                    writer.WriteString(message);
                    writer.WriteEndElement();
                writer.WriteEndElement();

    //            writer.WriteStartElement("DebugTrace");
    //            writer.WriteString(message);
    //            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        protected override void WriteSummary()
        {
            writer.WriteStartElement("ResultSummary");
            writer.WriteAttributeString("outcome", "Completed");

            writer.WriteStartElement("Counters");
            writer.WriteAttributeString("total", total.ToString());
            writer.WriteAttributeString("executed", total.ToString());
            writer.WriteAttributeString("passed", passed.ToString());
            writer.WriteAttributeString("failed", failed.ToString());
            writer.WriteAttributeString("inconclusive", inconclusive.ToString());

            writer.WriteAttributeString("error", "0");
            writer.WriteAttributeString("timeout", "0");
            writer.WriteAttributeString("aborted", "0");
            writer.WriteAttributeString("passedButRunAborted", "0");
            writer.WriteAttributeString("notRunnable", "0");
            writer.WriteAttributeString("notExecuted", "0");
            writer.WriteAttributeString("disconnected", "0");
            writer.WriteAttributeString("warning", "0");
            writer.WriteAttributeString("completed", "0");
            writer.WriteAttributeString("inProgress", "0");
            writer.WriteAttributeString("pending", "0");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
/*
<?xml version="1.0" encoding="UTF-8" ?> 
<TestRun id="17ae7d3b-31cc-44f5-bc6d-a66f9bc674e2" name="Michael@ARIES 2009-07-14 17:48:09" runUser="ARIES\Michael" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2006">
    <ResultSummary outcome="Completed">
      <Counters total="9" executed="9" passed="9" error="0" failed="0" timeout="0" aborted="0" inconclusive="0" passedButRunAborted="0" notRunnable="0" notExecuted="0" disconnected="0" warning="0" completed="0" inProgress="0" pending="0" /> 
    </ResultSummary>

<UnitTestResult executionId="f7f3c657-3c97-426f-b462-a3a6065bc8de" testId="d86af368-29a5-5398-0747-c7dd545fe050" testName="LocationGetPrimaryByOrganizationTest" computerName="ARIES" duration="00:00:01.7739751" startTime="2009-07-14T17:48:10.8972628+03:00" endTime="2009-07-14T17:48:13.1784895+03:00" testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" outcome="Passed" testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d">
 <Output>
  <DebugTrace>exec LocationGetPrimaryByOrganization @p0 @p0 = 1</DebugTrace> 
  </Output>
  </UnitTestResult>
</Results>
</TestRun>

//*/
}
