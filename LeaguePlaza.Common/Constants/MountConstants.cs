namespace LeaguePlaza.Common.Constants
{
    public static class MountConstants
    {
        public const string NoMountDescriptionAvailable = "No description available";

        public const int RecommendedMountsCount = 3;

        public const int MountNameMinLength = 1;
        public const int MountNameMaxLength = 50;
        public const int MountDescriptionMaxLength = 500;
        public const int MountImageUrlMaxLength = 500;
        public const int MountFileMaxSize = 5 * 1024 * 1024;
        public const int MountMinRating = 1;
        public const int MountMaxRating = 5;

        public const string MountCardsContainerWithPagination = "~/Views/Mount/Partials/_MountCardsContainerWithPagination.cshtml";
        public const string MountRentHistoryContainerWithPagination = "~/Views/Mount/Partials/_MountRentHistoryContainerWithPagination.cshtml";

        public const string ImageUploadPath = "/mounts/{0}/{1}/{2}";
        public const string GroundDefaultImageUrl = "https://www.dropbox.com/scl/fi/wyvpahi0salv5ii2v5i8r/ground-default.jpg?rlkey=br72tc41gyn9b59bqk5ahyyod&st=w147fofs&raw=1";
        public const string FlyingDefaultImageUrl = "https://www.dropbox.com/scl/fi/9n7d7geaprae40gjhcvyr/flying-default.jpg?rlkey=ktap2t7jjgo2j8oatka34rxdu&st=pfuagymz&raw=1";
        public const string AquaticDefaultImageUrl = "https://www.dropbox.com/scl/fi/soux4avtf2hlpjw2gguth/aquatic-default.jpg?rlkey=hlf1n9g4pts8zrcfiaiglddyu&st=om0epfjz&raw=1";

        public const string MountRentSuccessMessage = "Mount rented successfully for the chosen interval";
        public const string MountRentFailMessage = "The mount is not available for the chosen interval";

        public const string MountRateSuccessMessage = "Mount rated successfully";

    }
}
