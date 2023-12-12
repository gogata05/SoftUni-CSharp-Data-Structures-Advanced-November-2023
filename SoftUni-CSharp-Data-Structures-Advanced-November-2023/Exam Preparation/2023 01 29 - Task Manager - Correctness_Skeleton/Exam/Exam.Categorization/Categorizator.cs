using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.Categorization
{
    public class Categorizator : ICategorizator
    {
        private readonly Dictionary<string, Category> categories = new Dictionary<string, Category>();
        private readonly Dictionary<string, HashSet<string>> childrenByParent = new Dictionary<string, HashSet<string>>();

        public void AddCategory(Category category)
        {
            if (categories.ContainsKey(category.Id))
            {
                throw new ArgumentException("Category already exists.");
            }

            categories[category.Id] = category;
            if (!childrenByParent.ContainsKey(category.Id))
            {
                childrenByParent[category.Id] = new HashSet<string>();
            }
        }

        public void AssignParent(string childCategoryId, string parentCategoryId)
        {
            if (!categories.ContainsKey(childCategoryId) || !categories.ContainsKey(parentCategoryId))
            {
                throw new ArgumentException("One or both categories not found.");
            }

            if (!childrenByParent.ContainsKey(parentCategoryId))
            {
                childrenByParent[parentCategoryId] = new HashSet<string>();
            }

            childrenByParent[parentCategoryId].Add(childCategoryId);
        }

        public bool Contains(Category category)
        {
            return categories.ContainsKey(category.Id);
        }

        public IEnumerable<Category> GetChildren(string categoryId)
        {
            if (!categories.ContainsKey(categoryId))
            {
                throw new ArgumentException("Category not found.");
            }

            var result = new HashSet<Category>();

            if (childrenByParent.TryGetValue(categoryId, out var children))
            {
                foreach (var childId in children)
                {
                    result.Add(categories[childId]);
                    result.UnionWith(GetChildren(childId));
                }
            }

            return result;
        }

        public IEnumerable<Category> GetHierarchy(string categoryId)
        {
            if (!categories.ContainsKey(categoryId))
            {
                throw new ArgumentException("Category not found.");
            }

            var hierarchy = new List<Category>();
            var currentCategoryId = categoryId;

            while (childrenByParent.ContainsKey(currentCategoryId))
            {
                hierarchy.Add(categories[currentCategoryId]);
                currentCategoryId = childrenByParent[currentCategoryId].FirstOrDefault();
            }

            hierarchy.Reverse();
            return hierarchy;
        }

        public IEnumerable<Category> GetTop3CategoriesOrderedByDepthOfChildrenThenByName()
        {
            return categories.Values
                .OrderByDescending(c => GetHierarchy(c.Id).Count())
                .ThenBy(c => c.Name)
                .Take(3);
        }

        public void RemoveCategory(string categoryId)
        {
            if (!categories.ContainsKey(categoryId))
            {
                throw new ArgumentException("Category not found.");
            }

            categories.Remove(categoryId);
            childrenByParent.Remove(categoryId);

            foreach (var parent in childrenByParent.Values)
            {
                parent.Remove(categoryId);
            }
        }

        public int Size()
        {
            return categories.Count;
        }
    }
}
