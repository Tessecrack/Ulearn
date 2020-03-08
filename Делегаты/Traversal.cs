using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        public static IEnumerable<int> GetBinaryTreeValues(BinaryTree<int> tree)
        {
            Predicate<BinaryTree<int>> predicate = (binaryTree) => binaryTree != null;
            Func<BinaryTree<int>, IEnumerable<BinaryTree<int>>> children = (binaryTree) =>
            {
                var list = new BinaryTree<int>[] { binaryTree.Left, binaryTree.Right };
                return list.Where(element => element != null); ;
            };
            Func<BinaryTree<int>, int> selector = (binaryTree) => binaryTree.Value;
            return GetWalkOnSequence(tree, predicate, selector, children);
        }

        public static IEnumerable<Product> GetProducts(ProductCategory tree)
        {
            Predicate<ProductCategory> predicate = (products) => products.Products.Count > 0;
            Func<ProductCategory, IEnumerable<Product>> selector = (products) => products.Products;
            Func<ProductCategory, IEnumerable<ProductCategory>> children = (product) => product.Categories;
            return GetWalkOnSequence(tree, predicate, selector, children).SelectMany(p => p);
        }

        public static IEnumerable<Job> GetEndJobs(Job tree)
        {
            Predicate<Job> predicate = (job) => job.Subjobs.Count == 0;
            Func<Job, Job> selector = (job) => job;
            Func<Job, IEnumerable<Job>> children = (job) => job.Subjobs;
            return GetWalkOnSequence(tree, predicate, selector, children);
        }

        public static IEnumerable<Tout> GetWalkOnSequence<Tin, Tout>
            (Tin element, Predicate<Tin> predicate, Func<Tin, Tout> selector, Func<Tin, IEnumerable<Tin>> children)
        {
            if (predicate(element)) yield return selector(element);
            foreach (var item in children(element))
                foreach (var item2 in GetWalkOnSequence(item, predicate, selector, children))
                    yield return item2;
        }
    }
}