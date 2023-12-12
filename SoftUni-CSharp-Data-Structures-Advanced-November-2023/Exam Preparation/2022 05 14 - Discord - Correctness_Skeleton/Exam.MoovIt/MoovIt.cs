using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.MoovIt
{
    public class MoovIt : IMoovIt
    {
        private Dictionary<string, Route> routes = new Dictionary<string, Route>();
        private HashSet<(string, string, double)> routeSignatures = new HashSet<(string, string, double)>();

        public int Count => this.routes.Count;

        public void AddRoute(Route route)
        {
            var routeSignature = (route.LocationPoints.First(), route.LocationPoints.Last(), route.Distance);

            if (this.routes.ContainsKey(route.Id) || routeSignatures.Contains(routeSignature))
                throw new ArgumentException();

            this.routes.Add(route.Id, route);
            routeSignatures.Add(routeSignature);
        }

        public void ChooseRoute(string routeId)
        {
            if (!this.routes.ContainsKey(routeId))
                throw new ArgumentException();

            this.routes[routeId].Popularity++;
        }

        public bool Contains(Route route)
        {
            return this.routes.Values.Any(r => AreRoutesEqual(r, route));
        }

        public IEnumerable<Route> GetFavoriteRoutes(string destinationPoint)
        {
            return this.routes.Values
                .Where(r => r.IsFavorite && r.LocationPoints.Last() == destinationPoint)
                .OrderBy(r => r.Distance)
                .ThenByDescending(r => r.Popularity);
        }

        public Route GetRoute(string routeId)
        {
            if (!this.routes.ContainsKey(routeId))
                throw new ArgumentException();

            return this.routes[routeId];
        }

        public IEnumerable<Route> GetTop5RoutesByPopularityThenByDistanceThenByCountOfLocationPoints()
        {
            return this.routes.Values
                .OrderByDescending(r => r.Popularity)
                .ThenBy(r => r.Distance)
                .ThenBy(r => r.LocationPoints.Count)
                .Take(5);
        }

        public void RemoveRoute(string routeId)
        {
            if (!this.routes.ContainsKey(routeId))
                throw new ArgumentException();

            this.routes.Remove(routeId);
        }

        public IEnumerable<Route> SearchRoutes(string startPoint, string endPoint)
        {
            return this.routes.Values
                .Where(r => r.LocationPoints.Contains(startPoint) && r.LocationPoints.Contains(endPoint) && r.LocationPoints.IndexOf(startPoint) < r.LocationPoints.IndexOf(endPoint))
                .OrderBy(r => r.IsFavorite ? 0 : 1)
                .ThenBy(r => GetDistanceBetweenPoints(r, startPoint, endPoint))
                .ThenByDescending(r => r.Popularity);
        }

        private static bool AreRoutesEqual(Route route1, Route route2)
        {
            return route1.LocationPoints.First() == route2.LocationPoints.First() &&
                   route1.LocationPoints.Last() == route2.LocationPoints.Last() &&
                   route1.Distance == route2.Distance;
        }

        private static int GetDistanceBetweenPoints(Route route, string startPoint, string endPoint)
        {
            var start = route.LocationPoints.IndexOf(startPoint);
            var end = route.LocationPoints.IndexOf(endPoint);
            return end - start;
        }
    }
}
