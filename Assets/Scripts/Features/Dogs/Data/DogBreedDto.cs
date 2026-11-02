using System;

namespace ClickerTestTask.Features.Dogs.Data
{
    [Serializable]
    public class DogBreedDto
    {
        public string id;
        public string type;
        public DogBreedAttributes attributes;
    }

    [Serializable]
    public class DogBreedAttributes
    {
        public string name;
        public string description;
    }
}