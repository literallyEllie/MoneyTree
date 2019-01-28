using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager {

    public MapData mapData;

    // The class which  manages the current map and data.
    public MapManager()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.name.StartsWith("Level_"))
        {
            return;
        }

        if ((mapData = LoadData<MapData>(ResPath(scene.name), true)) == null)
        {
            // Debug.Log("Map data did not already exist for " + scene.name + ", creating...");
            this.mapData = new MapData(scene.name);
            WriteData(mapData, FullPath(scene.name));
        }//  else Debug.Log("Map data for level " + scene.name + " loaded.");

    }

    // Loads map data from a path. Returns true if loaded, false if faialed or doesn't exist.
    public T LoadData<T>(string path, bool resource)
    {
        /*
        if (!File.Exists(path))
        {
            Debug.LogWarningFormat("{0} does not exist to path", path);
            // If the path doesn't exist, return the defualt value of the data type specified, likely null.
            return default(T);
        }
        */

        if (resource)
        {

            //if (PlayerPrefs.GetInt("FIRST_TIME_OPEN", 1) == 1)
            //{
                PlayerPrefs.SetInt("FIRST_TIME_OPEN", 0);
                // Debug.Log("First time opening game...");


                TextAsset asset = (TextAsset)Resources.Load(path, typeof(TextAsset));
                if (asset == null)
                {
                    Debug.LogErrorFormat("{0} is invalid path to asset", path);
                }

                return JsonUtility.FromJson<T>(asset.text);

            //}
            /*else
            {

                string tempPath = Path.Combine(Application.persistentDataPath, "data");
                tempPath = Path.Combine(tempPath, path + ".txt");

                if (!File.Exists(tempPath))
                {
                    Debug.LogErrorFormat("{0} is invalid path to asset", tempPath);
                    return default(T);
                }

                TextReader textReader = null;

                try
                {
                    // Read and parse.
                    textReader = new StreamReader(path);
                    var fileContents = textReader.ReadToEnd();
                    return JsonUtility.FromJson<T>(fileContents);
                }
                finally
                {
                    // Regardless of outcome, close file saftely.
                    if (textReader != null) textReader.Close();
                }

            }
            */

        }

        else
        {
            TextReader reader = null;
            try
            {
                // Read and parse.
                reader = new StreamReader(path);
                var fileContents = reader.ReadToEnd();
                return JsonUtility.FromJson<T>(fileContents);
            }
            finally
            {
                // Regardless of outcome, close file saftely.
                if (reader != null) reader.Close();
            }

        }

    }

    // Writes data to a specified path.
    public void WriteData<T>(T data, string path)
    {
        TextWriter writer = null;
        try
        {
            var contents = JsonUtility.ToJson(data);
            writer = new StreamWriter(path, false);
            writer.Write(contents);
        }
        finally
        {
            if (writer != null) writer.Close();
        }
    }

    // Computes the path from the map name
    public string ResPath(string map)
    {
        return string.Format("{0}_data", map);
    }

    public String FullPath(string map)
    {
        return string.Format("Assets/Resources/{0}_data.txt", map);
    }

    // Sorts the scoreboard and returns it
    public List<Highscore> GetSortedScoreboard()
    {
        if (mapData == null)
            throw new ArgumentNullException("map data");

        int pos = 1;

        // Sort through the highscores and return them in descending order.
        List<Highscore> sorted = mapData.scoreboard.OrderByDescending(s => s.TimeCompleted).ToList();
        sorted.ForEach(s => s.Position = pos++);

        return sorted.ToList();
    }

    // Gets next level from current level
    public static string GetNextLevel(string currentLevel)
    {
        switch (currentLevel)
        {
            case "Level_Introduction":
                return "Level_2";
            case "Level_2":
                // return "Level_3";
                break;
            case "Level_3":
                break;
        }

        // If not recognized, just return the introduction level.
        return "Level_Introduction";
    }


[Serializable]
    public class MapData
    {
        public string mapID;
        public Vector3 playerSpawn;
        public Vector3[] coinSpawns;
        public int minX, maxX, minY, maxY;
        public List<Highscore> scoreboard;

        // MapData is a serializable data class which contains the data to a map.
        public MapData (string mapID)
        {
            this.mapID = mapID;
        }

    }

}
