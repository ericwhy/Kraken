namespace Koretech.Tools.KrakenGenerator {
    public static class KrakenGeneratorExtensions {
        public static int AsInteger(this string value) {
            int outValue;
            if(int.TryParse(value, out outValue))
            {
                return outValue;
            }
            return 0;
        }

        public static bool AsBoolean(this string value) {
            if(value.ToLower() == "true") {
                return true;
            }
            return false;
        }
    }
}
