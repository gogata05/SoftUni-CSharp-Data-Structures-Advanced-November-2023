using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.MobileX
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly Dictionary<string, Vehicle> vehiclesById;
        private readonly Dictionary<string, List<Vehicle>> vehiclesBySeller;
        private readonly Dictionary<string, List<Vehicle>> vehiclesByBrand;

        public VehicleRepository()
        {
            vehiclesById = new Dictionary<string, Vehicle>();
            vehiclesBySeller = new Dictionary<string, List<Vehicle>>();
            vehiclesByBrand = new Dictionary<string, List<Vehicle>>();
        }

        public int Count => vehiclesById.Count;

        public void AddVehicleForSale(Vehicle vehicle, string sellerName)
        {
            if (!vehiclesById.ContainsKey(vehicle.Id))
            {
                vehiclesById.Add(vehicle.Id, vehicle);

                if (!vehiclesBySeller.ContainsKey(sellerName))
                {
                    vehiclesBySeller[sellerName] = new List<Vehicle>();
                }
                vehiclesBySeller[sellerName].Add(vehicle);

                if (!vehiclesByBrand.ContainsKey(vehicle.Brand))
                {
                    vehiclesByBrand[vehicle.Brand] = new List<Vehicle>();
                }
                vehiclesByBrand[vehicle.Brand].Add(vehicle);
            }
        }

        public void RemoveVehicle(string vehicleId)
        {
            if (vehiclesById.TryGetValue(vehicleId, out var vehicle))
            {
                vehiclesById.Remove(vehicleId);
                vehiclesBySeller[vehiclesBySeller.FirstOrDefault(s => s.Value.Contains(vehicle)).Key].Remove(vehicle);
                vehiclesByBrand[vehicle.Brand].Remove(vehicle);
            }
        }

        public bool Contains(Vehicle vehicle)
        {
            return vehiclesById.ContainsKey(vehicle.Id);
        }

        public IEnumerable<Vehicle> GetVehicles(List<string> keywords)
        {
            return vehiclesById.Values
                .Where(v => keywords.Contains(v.Brand) || keywords.Contains(v.Location) || keywords.Contains(v.Color))
                .ToList();
        }

        public IEnumerable<Vehicle> GetVehiclesBySeller(string sellerName)
        {
            return vehiclesBySeller.TryGetValue(sellerName, out var vehicles) ? vehicles : Enumerable.Empty<Vehicle>();
        }

        public IEnumerable<Vehicle> GetVehiclesInPriceRange(double lowerBound, double upperBound)
        {
            return vehiclesById.Values
                .Where(v => v.Price >= lowerBound && v.Price <= upperBound)
                .ToList();
        }

        public Dictionary<string, List<Vehicle>> GetAllVehiclesGroupedByBrand()
        {
            return new Dictionary<string, List<Vehicle>>(vehiclesByBrand);
        }

        public IEnumerable<Vehicle> GetAllVehiclesOrderedByHorsepowerDescendingThenByPriceThenBySellerName()
        {
            return vehiclesById.Values
                .OrderByDescending(v => v.Horsepower)
                .ThenBy(v => v.Price)
                .ThenBy(v => vehiclesBySeller.FirstOrDefault(s => s.Value.Contains(v)).Key) // Corrected line
                .ToList();
        }

        public Vehicle BuyCheapestFromSeller(string sellerName)
        {
            if (vehiclesBySeller.TryGetValue(sellerName, out var vehicles))
            {
                return vehicles.OrderBy(v => v.Price).FirstOrDefault();
            }

            throw new ArgumentException($"No vehicles found for seller {sellerName}");
        }
    }
}
