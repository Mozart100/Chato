//using Arkovean.Chat.Automation.Responses;
//using FluentAssertions;
//using SailPoint.Infrastracture;
//using SailPoint.Models.Dtos;

//namespace Arkovean.Chat.Automation.Scenario
//{
//    public class CityLocatorScenario : ScenarioBase
//    {

//        public CityLocatorScenario(string baseUrl) : base(baseUrl)
//        {
//            BusinessLogicLogicCallbacks.Add(Level_0_Error);
//            //BusinessLogicLogicCallbacks.Add(SanityTest_Expect_5_Records);
//            //BusinessLogicLogicCallbacks.Add(SanityTest_Expect_2_Records);
//        }

//        public override string ScenarioName => "City Locator";
//        public override string Description => "Integration tests for CityLocatorService";

//        //private async Task SanityTest_Expect_5_Records()
//        //{
//        //    var response = await Get<GetCitiesResponse>($"{BaseUrl}/tes/level/10");
//        //    response.Cities.SafeCount().Should().Be(5);
//        //}

//        //private async Task SanityTest_Expect_2_Records()
//        //{
//        //    var response = await Get<GetCitiesResponse>($"{BaseUrl}/tes/level/2");
//        //    response.Cities.SafeCount().Should().Be(2);
//        //}

//        private async Task Level_0_Error()
//        {
//            var isErrorResponseException = false;
//            try
//            {
//                var response = await Get<GetCitiesResponse>($"{BaseUrl}/tes/level/0");
//            }
//            catch (ErrorResponseException errorResponseException)
//            {
//                isErrorResponseException = true;
//            }
//            catch (Exception ex)
//            {

//            }
//            finally
//            {
//                if (!isErrorResponseException)
//                {
//                    throw new Exception("Something went wrong");
//                }
//            }
//        }

//    }
//}