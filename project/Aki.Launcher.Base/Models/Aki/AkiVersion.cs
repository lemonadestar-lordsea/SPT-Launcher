namespace Aki.Launch.Models.Aki
{
    public class AkiVersion
    {
        public int Major;
        public int Minor;
        public int Build;

        public string Tag { get; set; } = null;

        public AkiVersion(string AkiVerison)
        {

            if (AkiVerison.Contains('-'))
            {
                string[] versionInfo = AkiVerison.Split('-');

                AkiVerison = versionInfo[0];

                Tag = versionInfo[1];
                return;
            }

            string[] splitVersion = AkiVerison.Split('.');

            if(splitVersion.Length == 3)
            {
                int.TryParse(splitVersion[0], out Major);
                int.TryParse(splitVersion[1], out Minor);
                int.TryParse(splitVersion[2], out Build);
            }
        }
    }
}
