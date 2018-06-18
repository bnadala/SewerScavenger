using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SewerScavenger.Models;
using SewerScavenger.ViewModels;
using Character = SewerScavenger.Models.Character;

namespace SewerScavenger.Services
{
    public sealed class SQLDataStore : IDataStore
    {

        // Make this a singleton so it only exist one time because holds all the data records in memory
        private static SQLDataStore _instance;

        public static SQLDataStore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SQLDataStore();
                }
                return _instance;
            }
        }

        private SQLDataStore()
        {

            App.Database.CreateTableAsync<Item>().Wait();
            App.Database.CreateTableAsync<Character>().Wait();
            App.Database.CreateTableAsync<Monster>().Wait();
            App.Database.CreateTableAsync<Score>().Wait();
        }

        // Create the Database Tables
        private void CreateTables()
        {
            App.Database.CreateTableAsync<Item>().Wait();
            App.Database.CreateTableAsync<Character>().Wait();
            App.Database.CreateTableAsync<Monster>().Wait();
            App.Database.CreateTableAsync<Score>().Wait();

        }

        // Delete the Datbase Tables by dropping them
        private void DeleteTables()
        {
            App.Database.DropTableAsync<Item>().Wait();
            App.Database.DropTableAsync<Character>().Wait();
            App.Database.DropTableAsync<Monster>().Wait();
            App.Database.DropTableAsync<Score>().Wait();
        }

        // Tells the View Models to update themselves.
        private void NotifyViewModelsOfDataChange()
        {
            ItemsViewModel.Instance.SetNeedsRefresh(true);
            MonstersViewModel.Instance.SetNeedsRefresh(true);
            CharactersViewModel.Instance.SetNeedsRefresh(true);
            ScoresViewModel.Instance.SetNeedsRefresh(true);
        }

        public void InitializeDatabaseNewTables()
        {
            // Delete the tables
            DeleteTables();

            // make them again
            CreateTables();

            // Populate them
            InitilizeSeedData();

            // Tell View Models they need to refresh
            NotifyViewModelsOfDataChange();
        }

        private async void InitilizeSeedData()
        {
            await AddAsync_Character(new Character { Id = Guid.NewGuid().ToString(),
                Name = "Plumber",
                Description = "The Plumber is a master at cleaning out any clogs.",
                Level = 1,
                Image = "plumber.png",
                Health = 21,
                Attack = 15,
                Defense = 15,
                Speed = 8,
                Move = 2,
                Range = 1
            });
            await AddAsync_Character(new Character { Id = Guid.NewGuid().ToString(),
                Name = "Macho Man",
                Description = "The Macho Man rips monsters into shreds.",
                Level = 1,
                Image = "machoMan.png",
                Health = 25,
                Attack = 18,
                Defense = 17,
                Speed = 4,
                Move = 2,
                Range = 1
            });
            await AddAsync_Character(new Character { Id = Guid.NewGuid().ToString(),
                Name = "Mall Cop",
                Description = "The Mall Cop takes law enforcement to a whole new level.",
                Level = 2,
                Image = "mallCop.png",
                Health = 24,
                Attack = 12,
                Defense = 13,
                Speed = 6,
                Move = 3,
                Range = 1
            });
            await AddAsync_Character(new Character { Id = Guid.NewGuid().ToString(),
                Name = "Electrician",
                Description = "The electrician makes sure the sparks fly.",
                Level = 2,
                Image = "electrician.jpg",
                Health = 9,
                Attack = 16,
                Defense = 15,
                Speed = 21,
                Move = 1,
                Range = 2
            });
            await AddAsync_Character(new Character { Id = Guid.NewGuid().ToString(),
                Name = "Bricklayer",
                Description = "The Bricklayer throws bricks, laying the smack down on monsters.",
                Level = 3,
                Image = "bricklayer.png",
                Health = 23,
                Attack = 13,
                Defense = 13,
                Speed = 8,
                Move = 1,
                Range = 2
            });
            await AddAsync_Character(new Character { Id = Guid.NewGuid().ToString(),
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
            });

            await AddAsync_Monster(new Monster { Id = Guid.NewGuid().ToString(),
                Name = "Hermit",
                Description = "The Hermit doesn't like to be bothered and attacks when confronted.",
                Image = "hermit.png",
                Health = 1,
                Attack = 2,
                Defense = 1,
                Speed = 4,
                Move = 2,
                Range = 1
            });
            await AddAsync_Monster(new Monster { Id = Guid.NewGuid().ToString(),
                Name = "Rogue Construction Worker",
                Description = "The Rogue Construction Worker brings destruction to city sewer-goers.",
                Image = "rogueConstructionWorker.gif",
                Health = 2,
                Attack = 2,
                Defense = 2,
                Speed = 2,
                Move = 1,
                Range = 1
            });
            await AddAsync_Monster(new Monster { Id = Guid.NewGuid().ToString(),
                Name = "Pet Fish",
                Description = "The Pet Fish is one floppy friend you don't want to mess with.",
                Image = "petFish.png",
                Health = 1,
                Attack = 1,
                Defense = 1,
                Speed = 1,
                Move = 1,
                Range = 1
            });
            await AddAsync_Monster(new Monster { Id = Guid.NewGuid().ToString(),
                Name = "Alligator",
                Description = "The Alligator crunches the skulls of passersby.",
                Image = "alligator.png",
                Health = 3,
                Attack = 2,
                Defense = 3,
                Speed = 1,
                Move = 1,
                Range = 1
            });
            await AddAsync_Monster(new Monster { Id = Guid.NewGuid().ToString(),
                Name = "Rodent of Unusual Size",
                Description = "The ROUS frequent the fire swamps and sewers, looking for prey.",
                Image = "ROUS.png",
                Health = 2,
                Attack = 2,
                Defense = 1,
                Speed = 2,
                Move = 2,
                Range = 1
            });
            await AddAsync_Monster(new Monster { Id = Guid.NewGuid().ToString(),
                Name = "Wookie",
                Description = "The Wookie doesn't take guff from anybody.",
                Image = "wookie.jpg",
                Health = 3,
                Attack = 3,
                Defense = 3,
                Speed = 1,
                Move = 2,
                Range = 2
            });
        }

        public async Task<bool> InsertUpdateAsync_Item(Item data)
        {

            // Check to see if the item exist
            var oldData = await GetAsync_Item(data.Id);
            if (oldData == null)
            {
                // If it does not exist, add it to the DB
                var InsertResult = await App.Database.InsertAsync(data);
                if (InsertResult == 1)
                {
                    return true;
                }
            }

            // Compare it, if different update in the DB
            var UpdateResult = await UpdateAsync_Item(data);
            if (UpdateResult)
            {
                return true;
            }

            return false;
        }
        
        // Item
        public async Task<bool> AddAsync_Item(Item data)
        {
            var result = await App.Database.InsertAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync_Item(Item data)
        {
            var result = await App.Database.UpdateAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync_Item(Item data)
        {
            var result = await App.Database.DeleteAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<Item> GetAsync_Item(string id)
        {
            // Need to add a try catch here, to catch when looking for something that does not exist in the db...
            try
            {
                var result = await App.Database.GetAsync<Item>(id);
                return result;
            }
            catch(Exception ex)
            {
                Debug.Write(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Item>> GetAllAsync_Item(bool forceRefresh = false)
        {
            var result = await App.Database.Table<Item>().ToListAsync();
            return result;
        }


        // Character
        public async Task<bool> AddAsync_Character(Character data)
        {
            var result = await App.Database.InsertAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync_Character(Character data)
        {
            var result = await App.Database.UpdateAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync_Character(Character data)
        {
            var result = await App.Database.DeleteAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<Character> GetAsync_Character(string id)
        {
            var result = await App.Database.GetAsync<Character>(id);
            return result;
        }

        public async Task<IEnumerable<Character>> GetAllAsync_Character(bool forceRefresh = false)
        {
            var result = await App.Database.Table<Character>().ToListAsync();
            return result;
        }


        //Monster
        public async Task<bool> AddAsync_Monster(Monster data)
        {
            var result = await App.Database.InsertAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync_Monster(Monster data)
        {
            var result = await App.Database.UpdateAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync_Monster(Monster data)
        {
            var result = await App.Database.DeleteAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<Monster> GetAsync_Monster(string id)
        {
            var result = await App.Database.GetAsync<Monster>(id);
            return result;
        }

        public async Task<IEnumerable<Monster>> GetAllAsync_Monster(bool forceRefresh = false)
        {
            var result = await App.Database.Table<Monster>().ToListAsync();
            return result;

        }

        // Score
        public async Task<bool> AddAsync_Score(Score data)
        {
            var result = await App.Database.InsertAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync_Score(Score data)
        {
            var result = await App.Database.UpdateAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync_Score(Score data)
        {
            var result = await App.Database.DeleteAsync(data);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        public async Task<Score> GetAsync_Score(string id)
        {
            var result = await App.Database.GetAsync<Score>(id);
            return result;
        }

        public async Task<IEnumerable<Score>> GetAllAsync_Score(bool forceRefresh = false)
        {
            var result = await App.Database.Table<Score>().ToListAsync();
            return result;

        }
    }
}