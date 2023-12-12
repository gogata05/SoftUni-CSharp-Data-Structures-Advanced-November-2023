using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.PackageManagerLite
{
    public class PackageManager : IPackageManager
    {
        private Dictionary<string, Package> packages;
        private Dictionary<string, HashSet<string>> dependencies;

        public PackageManager()
        {
            packages = new Dictionary<string, Package>();
            dependencies = new Dictionary<string, HashSet<string>>();
        }

        public void RegisterPackage(Package package)
        {
            if (packages.ContainsKey(package.Id))
                throw new ArgumentException();

            packages[package.Id] = package;
        }

        public void RemovePackage(string packageId)
        {
            if (!packages.ContainsKey(packageId))
                throw new ArgumentException();

            var dependants = dependencies.Where(d => d.Value.Contains(packageId)).Select(d => d.Key).ToList();
            foreach (var dependant in dependants)
            {
                dependencies[dependant].Remove(packageId);
            }

            packages.Remove(packageId);
            dependencies.Remove(packageId);
        }

        public void AddDependency(string packageId, string dependencyId)
        {
            if (!packages.ContainsKey(packageId) || !packages.ContainsKey(dependencyId))
                throw new ArgumentException();

            if (!dependencies.ContainsKey(packageId))
                dependencies[packageId] = new HashSet<string>();

            if (!dependencies[packageId].Add(dependencyId))
                throw new ArgumentException();
        }

        public bool Contains(Package package)
        {
            return packages.ContainsKey(package.Id);
        }

        public int Count()
        {
            return packages.Count;
        }

        public IEnumerable<Package> GetDependants(Package package)
        {
            if (!packages.ContainsKey(package.Id))
                throw new ArgumentException();

            return dependencies
                .Where(d => d.Value.Contains(package.Id))
                .Select(d => packages[d.Key]);
        }

        public IEnumerable<Package> GetIndependentPackages()
        {
            return packages.Values.Where(p => !dependencies.Any(d => d.Value.Contains(p.Id)));
        }

        public IEnumerable<Package> GetOrderedPackagesByReleaseDateThenByVersion()
        {
            return packages.Values
                .OrderBy(p => p.ReleaseDate)
                .ThenBy(p => p.Version)
                .ThenBy(p => p.Id);
        }
    }
}
