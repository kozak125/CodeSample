using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Voidwalker.Systems.Saving
{
    public static class DataFileHandler
    {
        public const string DATA_DIRECTORY_NAME = "CaptainLog.Entry";
        public const string BACKUP_DATA_DIRECTORY_NAME = DATA_DIRECTORY_NAME + ".Backup";
        private const string SAVE_SUBDIRECTORY_NAME = "Saves";
        private const string PARTITIONER_BUZZWORD = "Serialized data partitioner";

        public static void CreateSavesDirectory()
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, SAVE_SUBDIRECTORY_NAME));
        }

        public static (PersistentGameData persistentData, LevelSpecificGameData levelData) LoadPersistentData(string directoryPath)
        {
            string fullMainPath = Path.Combine(directoryPath, SAVE_SUBDIRECTORY_NAME, DATA_DIRECTORY_NAME);
            string fullBackupPath = Path.Combine(directoryPath, SAVE_SUBDIRECTORY_NAME, BACKUP_DATA_DIRECTORY_NAME);

            var loadedMainData = LoadDataFromFile(fullMainPath);

            if (loadedMainData.persistentData != null && loadedMainData.levelData != null)
            {
                return (loadedMainData.persistentData, loadedMainData.levelData);
            }

            File.Delete(fullMainPath);
            var loadedBackupData = LoadDataFromFile(fullBackupPath);

            if (loadedBackupData.persistentData != null && loadedBackupData.levelData != null)
            {
                return (loadedBackupData.persistentData, loadedBackupData.levelData);
            }

            File.Delete(fullBackupPath);

            return (null, null);
        }

        public static void SavePersistentData(PersistentGameData persistentGameData, LevelSpecificGameData levelSpecificGameData, string directoryPath)
        {
            string fullMainPath = Path.Combine(directoryPath, SAVE_SUBDIRECTORY_NAME, DATA_DIRECTORY_NAME);
            string fullBackupPath = Path.Combine(directoryPath, SAVE_SUBDIRECTORY_NAME, BACKUP_DATA_DIRECTORY_NAME);

            var mainSaveFile = new FileInfo(fullMainPath);
            var backupSaveFile = new FileInfo(fullBackupPath);

            if (mainSaveFile.Exists)
            {
                using StreamWriter backupWriter = backupSaveFile.CreateText();
                {
                    using StreamReader mainReader = mainSaveFile.OpenText();
                    {
                        backupWriter.Write(mainReader.ReadToEnd());
                    }
                }
            }

            string persistentDataToStore = JsonUtility.ToJson(persistentGameData, true);
            string levelSpecificDataToStore = JsonUtility.ToJson(levelSpecificGameData, true);
            string dataToStore = persistentDataToStore + "\n" + PARTITIONER_BUZZWORD + "\n" + levelSpecificDataToStore;

            using StreamWriter mainWriter = mainSaveFile.CreateText();
            {
                mainWriter.Write(dataToStore);
            }
        }

        public static (DateTime saveDateTime, DateTime backupSaveDateTime) GetSavesDateTime()
        {
            string fullMainPath = Path.Combine(Application.persistentDataPath, SAVE_SUBDIRECTORY_NAME, DATA_DIRECTORY_NAME);
            string fullBackupPath = Path.Combine(Application.persistentDataPath, SAVE_SUBDIRECTORY_NAME, BACKUP_DATA_DIRECTORY_NAME);
            (DateTime save, DateTime backupSave) savesTime = (DateTime.MinValue, DateTime.MinValue);

            if (File.Exists(fullMainPath))
            {
                savesTime.save = File.GetLastWriteTime(fullMainPath);
            }

            if (File.Exists(fullBackupPath))
            {
                savesTime.backupSave = File.GetLastWriteTime(fullBackupPath);
            }

            return savesTime;
        }

        public static void DeleteSavedData()
        {
            string fullMainPath = Path.Combine(Application.persistentDataPath, SAVE_SUBDIRECTORY_NAME, DATA_DIRECTORY_NAME);
            string fullBackupPath = Path.Combine(Application.persistentDataPath, SAVE_SUBDIRECTORY_NAME, BACKUP_DATA_DIRECTORY_NAME);

            File.Delete(fullMainPath);
            File.Delete(fullBackupPath);
        }

        private static (PersistentGameData persistentData, LevelSpecificGameData levelData) LoadDataFromFile(string fullPath)
        {
            PersistentGameData loadedPersistentData = null;
            LevelSpecificGameData loadedLevelData = null;
            var saveFile = new FileInfo(fullPath);

            if (saveFile.Exists)
            {
                try
                {
                    string persistentDataToLoad;
                    string levelDataToLoad;
                    using StreamReader reader = saveFile.OpenText();
                    {
                        // it's hardcoded as I don't expect more types in the future
                        string dataLine = string.Empty;
                        StringBuilder sb = new();

                        do
                        {
                            sb.AppendLine(dataLine);
                            dataLine = reader.ReadLine();
                        } while (dataLine != PARTITIONER_BUZZWORD && reader.Peek() >= 0);

                        persistentDataToLoad = sb.ToString();
                        levelDataToLoad = reader.ReadToEnd();
                    }

                    loadedPersistentData = JsonUtility.FromJson<PersistentGameData>(persistentDataToLoad);
                    loadedLevelData = JsonUtility.FromJson<LevelSpecificGameData>(levelDataToLoad);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    loadedPersistentData = null;
                    loadedLevelData = null;
                }
            }

            return (loadedPersistentData, loadedLevelData);
        }
    }
}
