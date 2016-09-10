using System.ComponentModel.DataAnnotations;

namespace A03.Models.ViewModels
{
    /// <summary>
    /// Used to store info for updating a course's start and end dates
    /// 
    /// Note on validation:
    /// Required and other types of ModelBinding didn't work, some problems with dependencies
    /// and/or frameworks so I ended up scrapping that and validating input myself :)
    /// </summary>
    public class UpdateCourseViewModel
    {
        /// <summary>
        /// The course's start date
        /// </summary>
        [StringLength(10)]
        [RegularExpression("(^(((0[1-9]|1[0-9]|2[0-8])[\\.](0[1-9]|1[012]))|((29|30|31)[\\.](0[13578]|1[02]))|" +
                                  "((29|30)[\\.](0[4,6,9]|11)))[\\.](19|[2-9][0-9])\\d\\d$)|(^29[\\.]02[\\.](19|[2-9][0-9])" +
                                  "(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)")]
        public string StartDate { get; set; }

        /// <summary>
        /// The course's end date
        /// </summary>
        [StringLength(10)]
        [RegularExpression("(^(((0[1-9]|1[0-9]|2[0-8])[\\.](0[1-9]|1[012]))|((29|30|31)[\\.](0[13578]|1[02]))|" +
                                  "((29|30)[\\.](0[4,6,9]|11)))[\\.](19|[2-9][0-9])\\d\\d$)|(^29[\\.]02[\\.](19|[2-9][0-9])" +
                                  "(00|04|08|12|16|20|24|28|32|36|40|44|48|52|56|60|64|68|72|76|80|84|88|92|96)$)")]
        public string EndDate { get; set; }
    }
}
