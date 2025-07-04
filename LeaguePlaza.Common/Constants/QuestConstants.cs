namespace LeaguePlaza.Common.Constants
{
    public static class QuestConstants
    {
        public const string NoQuestDescriptionAvailable = "No description available";

        public const int RecommendedQuestsCount = 3;

        public const int QuestTitleMinLength = 1;
        public const int QuestTitleMaxLength = 50;
        public const int QuestDescriptionMaxLength = 500;
        public const int QuestImageUrlMaxLength = 200;
        public const int QuestFileMaxSize = 5 * 1024 * 1024;

        public const string QuestCardsContainerWithPagination = "~/Views/Quest/Partials/_QuestCardsContainerWithPagination.cshtml";

        public const string ImageUploadPath = "/quests/{0}/{1}/{2}";
        public const string MonsterHuntDefaultImageUrl = "https://www.dropbox.com/scl/fi/zxqv1fy2io88ytcdi3iqa/monster-hunt-default.jpg?rlkey=vkl9dt9q96af2qlv8gx5etsdy&st=03rctf0o&raw=1";
        public const string GatheringDefaultImageUrl = "https://www.dropbox.com/scl/fi/ns7u5n9zhqw9q3i5g6gsq/gathering-default.jpg?rlkey=zbrno8iqnhxdqgmm2xkg8moyh&st=gm6ja4j6&raw=1";
        public const string EscortDefaultImageUrl = "https://www.dropbox.com/scl/fi/977mmg7o6fxpr3e4i5k4p/escort-default.jpg?rlkey=fyekeazwrh373cyxqtu6kjxeg&st=2y5oj0ms&raw=1";
    }
}
