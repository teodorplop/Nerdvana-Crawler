using NerdvanaCrawler.Code.Config;
using Newtonsoft.Json;

namespace NerdvanaCrawler.Code.Nerdvana
{
    [JsonConfigFile("Resources/nerdvanaGoogleSheetConfig.json")]
    public class NerdvanaGoogleSheetConfig : JsonConfigFile<NerdvanaGoogleSheetConfig>
    {
        [JsonProperty("students_collection_subsheet")]
        public string StudentsCollectionSubsheet { get; private set; } = "Studenți";
        [JsonProperty("student_name_column")]
        public string StudentNameColumn { get; private set; } = "Nume";
        [JsonProperty("student_nerdarena_column")]
        public string StudentNerdarenaColumn { get; private set; } = "Nerdarena";
        [JsonProperty("student_infoarena_column")]
        public string StudentInfoarenaColumn { get; private set; } = "Infoarena";
        [JsonProperty("student_kilonova_column")]
        public string StudentKilonovaColumn { get; private set; } = "Kilonova";


        [JsonProperty("homework_prefix")]
        public string HomeworkPrefix { get; private set; } = "Cursul";

        [JsonProperty("homework_total_column_string")]
        public string HomeworkTotalColumnString { get; private set; } = "Total";


        [JsonProperty("homework_overall_prefix")]
        public string HomeworkOverallPrefix { get; private set; } = "Temă";

        [JsonProperty("homework_overall_score_column_string")]
        public string HomeworkOverallScoreColumnString { get; private set; } = "Scor";

        [JsonProperty("homework_overall_max_score_column_string")]
        public string HomeworkOverallMaxScoreColumnString { get; private set; } = "Scor Maxim";

        [JsonProperty("homework_overall_percentage_column_string")]
        public string HomeworkOverallPercentageColumnString { get; private set; } = "%";
    }
}
