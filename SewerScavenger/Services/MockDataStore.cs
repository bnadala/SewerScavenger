using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SewerScavenger.Models;
using Character = SewerScavenger.Models.Character;

namespace SewerScavenger.Services
{
    public sealed class MockDataStore : IDataStore
    {

        // Make this a singleton so it only exist one time because holds all the data records in memory
        private static MockDataStore _instance;

        public static MockDataStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MockDataStore();
                }
                return _instance;
            }
        }

        private List<Item> _itemDataset = new List<Item>();
        private List<Character> _characterDataset = new List<Character>();
        private List<Monster> _monsterDataset = new List<Monster>();
        private List<Score> _scoreDataset = new List<Score>();

        private MockDataStore()
        {
            var mockItems = new List<Item>
            {
                new Item { Id = Guid.NewGuid().ToString(), Name = "First item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Second item", Description="This is an item description." },
                new Item { Id = Guid.NewGuid().ToString(), Name = "Third item", Description="This is an item description." },
            };

            foreach (var data in mockItems)
            {
                _itemDataset.Add(data);
            }

            var mockCharacters = new List<Character>
            {
                new Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Plumber",
                    Description = "The Plumber is a master at cleaning out any clogs.",
                    Level = 1,
                    Image = "plumber.png",
                    Health = 21,
                    Attack = 9,
                    Defense = 15,
                    Speed = 8,
                    Move = 2,
                    Range = 1
                },
                new Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Macho Man",
                    Description = "The Macho Man rips monsters into shreds.",
                    Level = 1,
                    Image = "machoMan.png",
                    Health = 25,
                    Attack = 12,
                    Defense = 17,
                    Speed = 4,
                    Move = 2,
                    Range = 1
                },
                new Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Mall Cop",
                    Description = "The Mall Cop takes law enforcement to a whole new level.",
                    Level = 2,
                    Image = "mallCop.png",
                    Health = 24,
                    Attack = 15,
                    Defense = 13,
                    Speed = 6,
                    Move = 3,
                    Range = 1
                },
                new Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Electrician",
                    Description = "The electrician makes sure the sparks fly.",
                    Level = 2,
                    Image = "electrician.jpg",
                    Health = 9,
                    Attack = 16,
                    Defense = 15,
                    Speed = 21,
                    Move = 1,
                    Range = 3
                },
                new Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Bricklayer",
                    Description = "The Bricklayer throws bricks, laying the smack down on monsters.",
                    Level = 3,
                    Image = "bricklayer.png",
                    Health = 23,
                    Attack = 13,
                    Defense = 13,
                    Speed = 8,
                    Move = 1,
                    Range = 3
                },
                new Character
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Exterminator",
                    Description = "The Exterminator sprays lethal poison, cleaning up all ferocious pests.",
                    Level = 3,
                    Image = "exterminator.png",
                    Health = 22,
                    Attack = 11,
                    Defense = 14,
                    Speed = 8,
                    Move = 1,
                    Range = 3
                }
            };

            foreach (var data in mockCharacters)
            {
                _characterDataset.Add(data);
            }

            var mockMonsters = new List<Monster>
            {
                new Monster
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Hermit",
                    Description = "The Hermit doesn't like to be bothered and attacks when confronted.",
                    Image = "hermit.png",
                    Health = 1,
                    Attack = 2,
                    Defense = 1,
                    Speed = 3,
                    Move = 3,
                    Range = 2
                },
                new Monster
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Rogue Construction Worker",
                    Description = "The Rogue Construction Worker brings destruction to city sewer-goers.",
                    Image = "rogueConstructionWorker.gif",
                    Health = 2,
                    Attack = 2,
                    Defense = 2,
                    Speed = 2,
                    Move = 2,
                    Range = 2
                },
                new Monster
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Pet Fish",
                    Description = "The Pet Fish is one floppy friend you don't want to mess with.",
                    Image = "petFish.png",
                    Health = 1,
                    Attack = 1,
                    Defense = 1,
                    Speed = 1,
                    Move = 1,
                    Range = 1
                },
                new Monster
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Alligator",
                    Description = "The Alligator crunches the skulls of passersby.",
                    Image = "alligator.png",
                    Health = 3,
                    Attack = 2,
                    Defense = 3,
                    Speed = 1,
                    Move = 2,
                    Range = 2
                },
                new Monster
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Rodent of Unusual Size",
                    Description = "The ROUS frequent the fire swamps and sewers, looking for prey.",
                    Image = "ROUS.png",
                    Health = 2,
                    Attack = 2,
                    Defense = 1,
                    Speed = 2,
                    Move = 3,
                    Range = 1
                },
                new Monster
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Wookie",
                    Description = "The Wookie doesn't take guff from anybody.",
                    Image = "wookie.jpg",
                    Health = 3,
                    Attack = 3,
                    Defense = 3,
                    Speed = 1,
                    Move = 2,
                    Range = 3
                }
            };

            foreach (var data in mockMonsters)
            {
                _monsterDataset.Add(data);
            }
        }

        // Item
        public async Task<bool> AddAsync_Item(Item data)
        {
            _itemDataset.Add(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync_Item(Item data)
        {
            var myData = _itemDataset.FirstOrDefault(arg => arg.Id == data.Id);
            if (myData == null)
            {
                return false;
            }

            myData.Update(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync_Item(Item data)
        {
            var myData = _itemDataset.FirstOrDefault(arg => arg.Id == data.Id);
            _itemDataset.Remove(myData);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetAsync_Item(string id)
        {
            return await Task.FromResult(_itemDataset.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetAllAsync_Item(bool forceRefresh = false)
        {
            return await Task.FromResult(_itemDataset);
        }


        // Character
        public async Task<bool> AddAsync_Character(Character data)
        {
            _characterDataset.Add(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync_Character(Character data)
        {
            var myData = _characterDataset.FirstOrDefault(arg => arg.Id == data.Id);
            if (myData == null)
            {
                return false;
            }

            myData.Update(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync_Character(Character data)
        {
            var myData = _characterDataset.FirstOrDefault(arg => arg.Id == data.Id);
            _characterDataset.Remove(myData);

            return await Task.FromResult(true);
        }

        public async Task<Character> GetAsync_Character(string id)
        {
            return await Task.FromResult(_characterDataset.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Character>> GetAllAsync_Character(bool forceRefresh = false)
        {
            return await Task.FromResult(_characterDataset);
        }


        //Monster
        public async Task<bool> AddAsync_Monster(Monster data)
        {
            _monsterDataset.Add(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync_Monster(Monster data)
        {
            var myData = _monsterDataset.FirstOrDefault(arg => arg.Id == data.Id);
            if (myData == null)
            {
                return false;
            }

            myData.Update(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync_Monster(Monster data)
        {
            var myData = _monsterDataset.FirstOrDefault(arg => arg.Id == data.Id);
            _monsterDataset.Remove(myData);

            return await Task.FromResult(true);
        }

        public async Task<Monster> GetAsync_Monster(string id)
        {
            return await Task.FromResult(_monsterDataset.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Monster>> GetAllAsync_Monster(bool forceRefresh = false)
        {
            return await Task.FromResult(_monsterDataset);
        }

        // Score
        public async Task<bool> AddAsync_Score(Score data)
        {
            _scoreDataset.Add(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateAsync_Score(Score data)
        {
            var myData = _scoreDataset.FirstOrDefault(arg => arg.Id == data.Id);
            if (myData == null)
            {
                return false;
            }

            myData.Update(data);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync_Score(Score data)
        {
            var myData = _scoreDataset.FirstOrDefault(arg => arg.Id == data.Id);
            _scoreDataset.Remove(myData);

            return await Task.FromResult(true);
        }

        public async Task<Score> GetAsync_Score(string id)
        {
            return await Task.FromResult(_scoreDataset.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Score>> GetAllAsync_Score(bool forceRefresh = false)
        {
            return await Task.FromResult(_scoreDataset);
        }

    }
}