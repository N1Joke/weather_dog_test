namespace Assets._Project.Scripts.Network.Dogs
{
    [System.Serializable]
    public class DogBreed
    {
        public string Id { get; }
        public string Name { get; }

        public DogBreed(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
