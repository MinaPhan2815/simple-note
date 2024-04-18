using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Notes.Models;
using System;
using Xamarin.Forms;
using System.Linq;
using System.Linq.Expressions;

namespace Notes.Data
{
    public class NoteDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public NoteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
        }

        public Task<List<Note>> GetNotesAsync()
        {
            return _database.Table<Note>().ToListAsync();
        }

        public Task<Note> GetNoteAsync(int id)
        {
            return _database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.ID != 0)
            {
                return _database.UpdateAsync(note);
            }
            else
            {
                return _database.InsertAsync(note);
            }
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            return _database.DeleteAsync(note);
        }

        public Task<List<Note>> GetDeletedNotesAsync()
        {
            return _database.Table<Note>()
                            .Where(n => n.IsDeleted)
                            .ToListAsync();
        }

        public Task<int> MoveToTrashAsync(Note note)
        {
            note.IsDeleted = true;
            return _database.UpdateAsync(note);
        }

        public Task<int> RestoreNoteAsync(Note note)
        {
            note.IsDeleted = false;
            return _database.UpdateAsync(note);
        }

        public Task<int> DeleteNotePermanentlyAsync(Note note)
        {
            return _database.DeleteAsync(note);
        }

        public Task<int> DeleteAllNotesPermanentlyAsync()
        {
            return _database.ExecuteAsync("DELETE FROM Note WHERE IsDeleted = 1");
        }

        public Task<int> RestoreAllNotesAsync()
        {
            return _database.ExecuteAsync("UPDATE Note SET IsDeleted = 0 WHERE IsDeleted = 1");
        }
        public async Task<List<Note>> GetNotesAsync(string searchQuery = null)
        {
            var sortOrder = GetSortOrder();
            List<Note> notes;

            if (string.IsNullOrEmpty(searchQuery))
            {
                notes = await GetSortedNotesAsync(sortOrder);
            }
            else
            {
                searchQuery = searchQuery.ToLower();

                notes = await GetSortedNotesAsync(sortOrder);

                notes = notes.Where(n => n.Text.ToLower().Contains(searchQuery) || 
                n.Title.ToLower().Contains(searchQuery)).ToList();
            }

            return notes;
        }


        public async Task<List<Note>> GetSortedNotesAsync(string orderBy)
        {
            var unsortedNotes = await _database.Table<Note>().ToListAsync();

            switch (orderBy)
            {
                case "Mới nhất (Mặc định)":
                    return unsortedNotes.OrderByDescending(n => n.Date).ToList();
                case "Cũ nhất":
                    return unsortedNotes.OrderBy(n => n.Date).ToList();
                case "A-Z":
                    return unsortedNotes.OrderBy(n => n.Title).ThenBy(n => n.Text).ToList();
                case "Z-A":
                    return unsortedNotes.OrderByDescending(n => n.Title).ThenByDescending(n => n.Text).ToList();
                default:
                    return unsortedNotes.OrderByDescending(n => n.Date).ToList();
            }
        }

        public async Task SaveSortOrderAsync(string sortOrder)
        {
            Application.Current.Properties["SortOrder"] = sortOrder;
            await Application.Current.SavePropertiesAsync();
        }

        public string GetSortOrder()
        {
            if (Application.Current.Properties.ContainsKey("SortOrder"))
            {
                return Application.Current.Properties["SortOrder"].ToString();
            }
            return "Mới nhất (Mặc định)";
        }


    }
}
