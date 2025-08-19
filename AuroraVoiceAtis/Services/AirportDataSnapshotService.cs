using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraVoiceAtis.Services
{
    using AuroraVoiceAtis.Models;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml;

    namespace YourNamespace.Services
    {
        public interface IAirportDataSnapshotService
        {
            void SaveSnapshot(AirportDataSnapshot snapshot, string filePath = null);
            AirportDataSnapshot LoadSnapshot(string filePath = null);
        }

        public class AirportDataSnapshotService : IAirportDataSnapshotService
        {
            private const string DefaultFileName = "airport_data_snapshot.json";
            private readonly string _defaultFilePath;

            public AirportDataSnapshotService()
            {
                // Utilise le dossier AppData local de l'utilisateur
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var appFolder = Path.Combine(appDataPath, "AuroraVoiceAtis"); // Remplacez par le nom de votre application

                // Créer le dossier s'il n'existe pas
                Directory.CreateDirectory(appFolder);

                _defaultFilePath = Path.Combine(appFolder, DefaultFileName);
            }

            #region Méthodes Synchrones

            /// <summary>
            /// Sauvegarde une instance d'AirportDataSnapshot dans un fichier JSON de manière synchrone
            /// </summary>
            /// <param name="snapshot">L'instance à sauvegarder</param>
            /// <param name="filePath">Chemin du fichier (optionnel, utilise un chemin par défaut si null)</param>
            public void SaveSnapshot(AirportDataSnapshot snapshot, string filePath = null)
            {
                if (snapshot == null)
                    throw new ArgumentNullException(nameof(snapshot));

                try
                {
                    var targetPath = filePath ?? _defaultFilePath;

                    // Créer le dossier parent s'il n'existe pas
                    var directory = Path.GetDirectoryName(targetPath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var jsonSettings = new JsonSerializerSettings
                    {
                        Formatting = Newtonsoft.Json.Formatting.Indented,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        NullValueHandling = NullValueHandling.Include
                    };

                    var json = JsonConvert.SerializeObject(snapshot, jsonSettings);
                    File.WriteAllText(targetPath, json);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Erreur lors de la sauvegarde du snapshot: {ex.Message}", ex);
                }
            }

            /// <summary>
            /// Charge une instance d'AirportDataSnapshot depuis un fichier JSON de manière synchrone
            /// </summary>
            /// <param name="filePath">Chemin du fichier (optionnel, utilise un chemin par défaut si null)</param>
            /// <returns>L'instance chargée ou null si le fichier n'existe pas</returns>
            public AirportDataSnapshot LoadSnapshot(string filePath = null)
            {
                try
                {
                    var targetPath = filePath ?? _defaultFilePath;

                    if (!File.Exists(targetPath))
                        return null;

                    var json = File.ReadAllText(targetPath);

                    if (string.IsNullOrWhiteSpace(json))
                        return null;

                    var jsonSettings = new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        NullValueHandling = NullValueHandling.Include
                    };

                    return JsonConvert.DeserializeObject<AirportDataSnapshot>(json, jsonSettings);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Erreur lors du chargement du snapshot: {ex.Message}", ex);
                }
            }

            #endregion

            /// <summary>
            /// Obtient le chemin du fichier par défaut
            /// </summary>
            public string GetDefaultFilePath() => _defaultFilePath;

            /// <summary>
            /// Vérifie si un fichier de snapshot existe
            /// </summary>
            /// <param name="filePath">Chemin du fichier (optionnel)</param>
            /// <returns>True si le fichier existe</returns>
            public bool SnapshotExists(string filePath = null)
            {
                var targetPath = filePath ?? _defaultFilePath;
                return File.Exists(targetPath);
            }

            /// <summary>
            /// Supprime un fichier de snapshot
            /// </summary>
            /// <param name="filePath">Chemin du fichier (optionnel)</param>
            /// <returns>True si le fichier a été supprimé avec succès</returns>
            public bool DeleteSnapshot(string filePath = null)
            {
                try
                {
                    var targetPath = filePath ?? _defaultFilePath;
                    if (File.Exists(targetPath))
                    {
                        File.Delete(targetPath);
                        return true;
                    }
                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
