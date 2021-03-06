﻿using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace CodeJam.Collections
{
	[TestFixture]
	public class DictionaryCollectionExtensionsTest
	{
		#region List

		[Test]
		public void AddOrCreateList_ICollection()
		{
			var d = new Dictionary<string, ICollection<int>>();
			d.AddOrCreateList("a", 1);
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new List<int> { 2 };
			d.AddOrCreateList("b", 3);
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		[Test]
		public void AddOrCreateList_IList()
		{
			var d = new Dictionary<string, IList<int>>();
			d.AddOrCreateList("a", 1);
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new List<int> { 2 };
			d.AddOrCreateList("b", 3);
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		[Test]
		public void AddOrCreateList_List()
		{
			var d = new Dictionary<string, List<int>>();
			d.AddOrCreateList("a", 1);
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new List<int> { 2 };
			d.AddOrCreateList("b", 3);
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		#endregion

		#region HashSet

		[Test]
		public void AddOrCreateHashSet_ICollection()
		{
			var d = new Dictionary<string, ICollection<int>>();
			d.AddOrCreateHashSet("a", 1);
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new HashSet<int> { 2 };
			d.AddOrCreateHashSet("b", 3);
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

#if !LESSTHAN_NET40
		[Test]
		public void AddOrCreateHashSet_ISet()
		{
			var d = new Dictionary<string, ISet<int>>();
			d.AddOrCreateHashSet("a", 1);
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new HashSet<int> { 2 };
			d.AddOrCreateHashSet("b", 3);
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}
#endif

		[Test]
		public void AddOrCreateHashSet_HashSet()
		{
			var d = new Dictionary<string, HashSet<int>>();
			d.AddOrCreateHashSet("a", 1);
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new HashSet<int> { 2 };
			d.AddOrCreateHashSet("b", 3);
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		#endregion

		#region Collection

		[Test]
		public void AddOrCreateCollection_ICollection()
		{
			var d = new Dictionary<string, ICollection<int>>();
			d.AddOrCreateCollection("a", 1, () => new List<int>());
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new HashSet<int> { 2 };
			d.AddOrCreateCollection("b", 3, () => new List<int>());
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		[Test]
		public void AddOrCreateCollection_IList()
		{
			var d = new Dictionary<string, IList<int>>();
			d.AddOrCreateCollection("a", 1, () => new List<int>());
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new List<int> { 2 };
			d.AddOrCreateCollection("b", 3, () => new List<int>());
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

#if !LESSTHAN_NET40
		[Test]
		public void AddOrCreateCollection_ISet()
		{
			var d = new Dictionary<string, ISet<int>>();
			d.AddOrCreateCollection("a", 1, () => new HashSet<int>());
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new HashSet<int> { 2 };
			d.AddOrCreateCollection("b", 3, () => new HashSet<int>());
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}
#endif

		[Test]
		public void AddOrCreateCollection_List()
		{
			var d = new Dictionary<string, List<int>>();
			d.AddOrCreateCollection("a", 1, () => new List<int>());
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new List<int> { 2 };
			d.AddOrCreateCollection("b", 3, () => new List<int>());
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		[Test]
		public void AddOrCreateCollection_HashSet()
		{
			var d = new Dictionary<string, HashSet<int>>();
			d.AddOrCreateCollection("a", 1, () => new HashSet<int>());
			Assert.AreEqual(1, d["a"].First());

			d["b"] = new HashSet<int> { 2 };
			d.AddOrCreateCollection("b", 3, () => new HashSet<int>());
			Assert.AreEqual(2, d["b"].First());
			Assert.AreEqual(3, d["b"].Last());
		}

		#endregion
	}
}