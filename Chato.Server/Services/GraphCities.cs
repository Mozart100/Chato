//using Chato.Server.Infrastracture;
//using System.Collections.Concurrent;

//namespace Arkovean.Chat.Services
//{
//    public class CityNode
//    {
//        public char Value { get; }
//        public Dictionary<char, CityNode> Children { get; } = new Dictionary<char, CityNode>();
//        public bool IsEndOfCity { get; set; }

//        public CityNode(char value)
//        {
//            Value = value;
//        }
//    }


//    public class LexicalCityTree
//    {
//        private CityNode root;

//        public LexicalCityTree(char letter)
//        {
//            root = new CityNode(letter);
//        }

//        public void InsertCity(string city)
//        {
//            var currentNode = root;
//            foreach (char character in city)
//            {
//                if (!currentNode.Children.TryGetValue(character, out CityNode nextNode))
//                {
//                    nextNode = new CityNode(character);
//                    currentNode.Children.Add(character, nextNode);
//                }

//                currentNode = nextNode;
//            }

//            currentNode.IsEndOfCity = true;
//        }

//        public List<string> SearchCities(string prefix, int level)
//        {
//            var result = new List<string>();
//            CityNode currentNode = root;

//            foreach (char character in prefix)
//            {
//                if (currentNode.Children.TryGetValue(character, out var nextNode))
//                {
//                    currentNode = nextNode;
//                }
//                else
//                {
//                    return result;
//                }
//            }

//            CollectCities(currentNode, prefix, level, result);
//            return result;
//        }

//        private void CollectCities(CityNode node, string currentPrefix, int level, List<string> result)
//        {
//            if (node.IsEndOfCity)
//            {
//                result.Add(currentPrefix);
//                if (--level <= 0)
//                {
//                    return;
//                }
//            }

//            foreach (var child in node.Children.Values)
//            {
//                CollectCities(child, currentPrefix + child.Value, level, result);
//            }

//        }

//        public IEnumerable<string> DisplayCitiesLexicographically()
//        {
//            var cities = new List<string>();
//            DisplayCitiesLexicographically(root, "", cities);
//            return cities;
//        }

//        private void DisplayCitiesLexicographically(CityNode node, string currentPrefix, List<string> cities)
//        {
//            foreach (var child in node.Children.Values)
//            {
//                DisplayCitiesLexicographically(child, currentPrefix + child.Value, cities);
//            }

//            if (node.IsEndOfCity)
//            {
//                cities.Add(currentPrefix);
//            }
//        }
//    }

//    public class GraphCities
//    {
//        ConcurrentDictionary<char, LexicalCityTree> _graph;

//        public GraphCities()
//        {
//            _graph = new ConcurrentDictionary<char, LexicalCityTree>();
//        }

//        public void InsertCity(string city)
//        {
//            city = city.ToLower();
//            var firstLetter = city[0];
//            LexicalCityTree cityTree = null;
//            if (!_graph.TryGetValue(firstLetter, out cityTree))
//            {
//                cityTree = new LexicalCityTree(firstLetter);
//                _graph[firstLetter] = cityTree;
//            }

//            cityTree.InsertCity(city);
//        }

//        public IEnumerable<string> SearchCities(string prefix, int level = 2)
//        {
//            List<string> result = null;
//            prefix = prefix.ToLower();
//            var firstLetter = prefix[0];

//            if (_graph.TryGetValue(firstLetter, out var cityTree))
//            {
//                result = cityTree.SearchCities(prefix, level);
//            }

//            return result;
//        }

//        public async Task<IEnumerable<string>> GetAllCitiesAsync()
//        {
//            var list = new List<string>();
//            foreach (var keyValue in _graph.ToArray())
//            {
//                var ptr = keyValue.Value.DisplayCitiesLexicographically();
//                if (!ptr.IsNullOrEmpty())
//                {
//                    foreach (var element in ptr)
//                    {
//                        list.Add(element);
//                    }
//                }
//            }

//            return list;
//        }
//    }
//}