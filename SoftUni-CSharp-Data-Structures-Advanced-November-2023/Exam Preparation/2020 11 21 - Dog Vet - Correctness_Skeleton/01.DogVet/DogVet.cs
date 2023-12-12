namespace _01.DogVet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DogVet : IDogVet
    {
        private Dictionary<string, Dog> dogsById;
        private Dictionary<string, Owner> ownersById;
        private Dictionary<Breed, HashSet<Dog>> dogsByBreed;
        private Dictionary<string, HashSet<Dog>> dogsByOwner;

        public DogVet()
        {
            this.dogsById = new Dictionary<string, Dog>();
            this.ownersById = new Dictionary<string, Owner>();
            this.dogsByBreed = new Dictionary<Breed, HashSet<Dog>>();
            this.dogsByOwner = new Dictionary<string, HashSet<Dog>>();
        }

        public int Size => this.dogsById.Count;

        public void AddDog(Dog dog, Owner owner)
        {
            if (this.dogsById.ContainsKey(dog.Id))
            {
                throw new ArgumentException();
            }

            if (!this.ownersById.ContainsKey(owner.Id))
            {
                this.ownersById[owner.Id] = owner;
            }

            if (!this.dogsByOwner.ContainsKey(owner.Id))
            {
                this.dogsByOwner[owner.Id] = new HashSet<Dog>();
            }

            if (this.dogsByOwner[owner.Id].Any(d => d.Name == dog.Name))
            {
                throw new ArgumentException();
            }

            this.dogsById[dog.Id] = dog;
            this.dogsByOwner[owner.Id].Add(dog);

            if (!this.dogsByBreed.ContainsKey(dog.Breed))
            {
                this.dogsByBreed[dog.Breed] = new HashSet<Dog>();
            }

            this.dogsByBreed[dog.Breed].Add(dog);
        }

        public bool Contains(Dog dog)
        {
            return this.dogsById.ContainsKey(dog.Id);
        }

        public Dog GetDog(string name, string ownerId)
        {
            this.ValidateOwnerExists(ownerId);

            var dog = this.dogsByOwner[ownerId].FirstOrDefault(d => d.Name == name);
            if (dog == null)
            {
                throw new ArgumentException();
            }

            return dog;
        }

        public Dog RemoveDog(string name, string ownerId)
        {
            var dog = this.GetDog(name, ownerId);
            this.dogsById.Remove(dog.Id);
            this.dogsByOwner[ownerId].Remove(dog);
            this.dogsByBreed[dog.Breed].Remove(dog);

            return dog;
        }

        public IEnumerable<Dog> GetDogsByOwner(string ownerId)
        {
            this.ValidateOwnerExists(ownerId);
            return this.dogsByOwner[ownerId];
        }

        public IEnumerable<Dog> GetDogsByBreed(Breed breed)
        {
            if (!this.dogsByBreed.ContainsKey(breed) || !this.dogsByBreed[breed].Any())
            {
                throw new ArgumentException();
            }

            return this.dogsByBreed[breed];
        }

        public void Vaccinate(string name, string ownerId)
        {
            var dog = this.GetDog(name, ownerId);
            dog.Vaccines++;
        }

        public void Rename(string oldName, string newName, string ownerId)
        {
            var dog = this.GetDog(oldName, ownerId);
            dog.Name = newName;
        }

        public IEnumerable<Dog> GetAllDogsByAge(int age)
        {
            var dogs = this.dogsById.Values.Where(d => d.Age == age).ToList();
            if (!dogs.Any())
            {
                throw new ArgumentException();
            }

            return dogs;
        }

        public IEnumerable<Dog> GetDogsInAgeRange(int lo, int hi)
        {
            return this.dogsById.Values.Where(d => d.Age >= lo && d.Age <= hi).ToList();
        }

        public IEnumerable<Dog> GetAllOrderedByAgeThenByNameThenByOwnerNameAscending()
        {
            return this.dogsById.Values
                .OrderBy(d => d.Age)
                .ThenBy(d => d.Name)
                .ThenBy(d => this.ownersById.Values.FirstOrDefault(o => this.dogsByOwner[o.Id].Contains(d)).Name)
                .ToList();
        }

        private void ValidateOwnerExists(string ownerId)
        {
            if (!this.ownersById.ContainsKey(ownerId))
            {
                throw new ArgumentException();
            }
        }
    }
}
