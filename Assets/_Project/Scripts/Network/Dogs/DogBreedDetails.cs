namespace Assets._Project.Scripts.Network.Dogs
{
    public class DogBreedDetails
    {
        public string Name { get; }
        public string Description { get; }

        public DogBreedDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
