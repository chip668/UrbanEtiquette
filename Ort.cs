namespace Anzeige
{
    public class Ort
    {
        public string OrtCode { get; private set; }
        public string Name { get; private set; }
        public string StadtLandkreis { get; private set; }
        public string Bundesland { get; private set; }

        public Ort(string dataString)
        {
            string[] attributes = dataString.Split(';');
            if (attributes.Length >= 4)
            {
                OrtCode = attributes[0].Trim();
                Name = attributes[1].Trim();
                StadtLandkreis = attributes[2].Trim();
                Bundesland = attributes[3].Trim();
            }
        }
    }
}
