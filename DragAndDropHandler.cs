// Fichier pour gérer le drag-and-drop des projets
using ProjectManagerApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace ProjectManagerApp.Helpers
{
    public class DragAndDropHandler
    {
        private ObservableCollection<Projet> _projets;
        private Projet _draggedProjet;

        public ICommand ItemDragged { get; }
        public ICommand ItemDraggedOver { get; }
        public ICommand ItemDragLeave { get; }
        public ICommand ItemDropped { get; }

        public DragAndDropHandler(ObservableCollection<Projet> projets)
        {
            _projets = projets;
            ItemDragged = new Command<Projet>(OnItemDragged);
            ItemDraggedOver = new Command<Projet>(OnItemDraggedOver);
            ItemDragLeave = new Command<Projet>(OnItemDragLeave);
            ItemDropped = new Command<Projet>(OnItemDropped);
        }

        private void OnItemDragged(Projet projet)
        {
            if (projet != null)
            {
                projet.IsBeingDragged = true;
                _draggedProjet = projet;
            }
        }

        private void OnItemDraggedOver(Projet projet)
        {
            if (projet != null && _draggedProjet != projet)
            {
                projet.IsBeingDraggedOver = true;
            }
        }

        private void OnItemDragLeave(Projet projet)
        {
            if (projet != null)
            {
                projet.IsBeingDraggedOver = false;
            }
        }

        private async void OnItemDropped(Projet dropTarget)
        {
            if (_draggedProjet != null && dropTarget != null && _draggedProjet != dropTarget)
            {
                var oldIndex = _projets.IndexOf(_draggedProjet);
                var newIndex = _projets.IndexOf(dropTarget);

                // Déplacer l'élément dans la collection
                _projets.Move(oldIndex, newIndex);

                // Réinitialiser les indicateurs de drag-and-drop
                _draggedProjet.IsBeingDragged = false;
                dropTarget.IsBeingDraggedOver = false;
                _draggedProjet = null;

                // Mettre à jour l'ordre des projets dans la base de données
                await UpdateProjectOrderInDatabase();
            }
        }

        // Méthode pour mettre à jour l'ordre des projets dans la base de données
        private async Task UpdateProjectOrderInDatabase()
        {
            // Met à jour la propriété "Ordre" de chaque projet pour enregistrer leur position actuelle
            for (int i = 0; i < _projets.Count; i++)
            {
                _projets[i].Ordre = i;
                await App.Database.SaveProjetAsync(_projets[i]);
            }
        }

    }
}