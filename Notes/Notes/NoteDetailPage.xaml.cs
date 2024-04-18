using System;
using Xamarin.Forms;
using Notes.Models;

namespace Notes
{
    public partial class NoteDetailPage : ContentPage
    {
        public NoteDetailPage()
        {
            InitializeComponent();
        }

        async void OnUpdateButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;

            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = (Note)BindingContext
            });
        }
        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            var note = (Note)BindingContext;
            bool answer = await DisplayAlert("Trả lời mau:", "Quăng vô sọt rác nha ?", "OK babii", "Nâuu");

            if (answer)
            {
                note.IsDeleted = true;
                await App.Database.SaveNoteAsync(note);
                await Navigation.PopAsync();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Send(this, "NoteUpdated");
        }
    }
}
